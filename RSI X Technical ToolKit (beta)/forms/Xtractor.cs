using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using Un4seen.BassAsio;
using agorartc;
using System.Diagnostics;

namespace RSI_X_Desktop.forms
{
    public enum CHANNEL_CHANGE
    {
        CHANNEL_CHANGE_RELAY,
        CHANNEL_PUBLISH,
        CHANNEL_UNPUBLISH
    };

    public partial class Xtractor : Form
    {
        public class BtnCmbPair 
        {
            internal ReaLTaiizor.Controls.SkyButton btn;
            ComboBox cmb;
            int indexLang;
            public bool langNotActive { get; private set; } = true;

            public int GetIndexID()
            {
                if (cmb == null)
                    return -1;
                return cmb.SelectedIndex; 
            }

            public int GetIndexLang() { return indexLang; }
            public string GetLang()   { return btn.Text; }

            public BtnCmbPair(ReaLTaiizor.Controls.SkyButton btn, ComboBox cmb, int indexLang) 
            {
                this.btn = btn;
                this.cmb = cmb;
                this.indexLang = indexLang;
            }

            public void ButtonRelay_Click(object sender, EventArgs e)
            {
                UpdateColors(sender as ReaLTaiizor.Controls.SkyButton);
            }

            public void UpdateColors(ReaLTaiizor.Controls.SkyButton btn)
            {
                int m_intRel = indexLang;
                bool? langActiveT = AgoraObject.room.IsActiveInterpRoomsAt(m_intRel);

                if (langActiveT != null)
                {
                    langNotActive = (bool)langActiveT;
                    AgoraObject.room.SetActiveInterpRoomsAt(m_intRel, !langNotActive);

                    if (langNotActive)
                    {
                        SkyBtnUpdate(btn, Color.FromArgb(80, 80, 80), Color.FromArgb(64, 64, 64), Color.FromArgb(64, 64, 64),
                            Color.FromArgb(50, 50, 50), Color.White, Color.LightGray, Color.FromArgb(45, 45, 45), Color.FromArgb(45, 45, 45),
                                Color.FromArgb(53, 53, 53));
                    }
                    else
                    {
                        SkyBtnUpdate(btn, Color.FromArgb(80, 80, 80), Color.FromArgb(64, 64, 64), Color.FromArgb(64, 64, 64),
                            Color.FromArgb(50, 50, 50), Color.White, Color.LightGray, Color.FromArgb(45, 45, 45), Color.Red,
                            Color.DarkRed);
                    }
                }
            }
        }


        public bool GetOutCode { get; private set; } = false;
        public string NickName { get; private set; } = string.Empty;
        public int RoomIndex { get; private set; } = 0;
        public bool IsPublishing { get; private set; } = false;

        AgoraAudioRecordingDeviceManager audioInDeviceManager;

        private int selectedTargetLangs;
        static readonly Color ButtonPushColor = Color.BurlyWood;

        List<string> devicesOutInd = new();
        List<string> devicesOutName = new();
        List<BtnCmbPair> BtnCmbPairs = new();
        List<Process> XAgora = new();

        public Xtractor()
        {
            InitializeComponent();
            
            label_nameOfConference.Text = AgoraObject.GetComplexToken().GetRoomName;

            audioInDeviceManager = AgoraObject.Rtc.CreateAudioRecordingDeviceManager();

            for (int i = 0; i < audioInDeviceManager.GetDeviceCount(); i++)
            {
                string device, id;

                var ret = audioInDeviceManager.GetDeviceInfoByIndex(i, out device, out id);

                devicesOutName.Add(device);
                devicesOutInd.Add(id);
                //if (ret == ERROR_CODE.ERR_OK && device.ToLower().IndexOf("in") == 0)
                //{
                //    //int offset = device.ToLower().IndexOf("out");
                //}
            }

            UpdateRelayLangs(); 
            UpdateInterpreterRoomCB();
        }

        internal ReaLTaiizor.Controls.SkyButton CreateButton(string text, int height, int width)
        {
            return new ReaLTaiizor.Controls.SkyButton()
            {
                Text = text,
                Height = height,
                Width = width,
                Font = new Font("Bahnschrift Condensed", 14, FontStyle.Bold),
            };
        }

        private void UpdateRelayLangs()
        {
            var langs = AgoraObject.GetComplexToken();
            var controls = panelRelayButtons.Controls;
            int defHeight = 35;
            int offset = 1;

            ReaLTaiizor.Controls.SkyButton btn;
            ComboBox cmb;
            BtnCmbPair pair;

            for (int i = 0; i < langs.GetTranslLangs.Count; i++)
            {
                var lang = langs.GetTranslLangs[i];
                if (lang.langShort == "HOST") continue;

                btn = CreateButton(lang.langShort, defHeight, 80);
                btn.Name = "REL" + i.ToString();
                btn.Height = 35;
                btn.Font = new Font("Segoe UI Semibold", 12);
                btn.Cursor = Cursors.Hand;
                btn.Location = new Point(24, defHeight * offset);
                //ButtonRelay_Click(btn, new EventArgs());
                //btn.BackColor = ButtonPushColor;
                btn.Show();

                cmb = new ComboBox();
                cmb.Name = "CMB" + i.ToString();
                cmb.Height = defHeight;
                cmb.Width = 450;
                cmb.Font = new Font("Segoe UI Semibold", 12);
                cmb.Cursor = Cursors.Hand;
                cmb.Location = new Point(120, defHeight * offset);
                cmb.DropDownStyle = ComboBoxStyle.DropDownList;

                if (devicesOutName.Count > 0)
                {
                    foreach (var dev in devicesOutName)
                        cmb.Items.Add(dev);
                    cmb.SelectedIndex = 0;
                }
                cmb.Show();

                controls.Add(cmb);
                controls.Add(btn);

                pair = new(btn, cmb, i);

                btn.Click += pair.ButtonRelay_Click;
                //cmb.SelectedIndexChanged += pair.Cmb_SelectedIndexChanged;

                BtnCmbPairs.Add(pair);
                offset++;
                ButtonRelay_Click(btn, new EventArgs());
                pair.UpdateColors(btn);
            }
        }

        private void UpdateTargetLangs()
        {
        }

        private void UpdateInterpreterRoomCB() 
        {
        }

        private void ButtonTarget_Click(object sender, EventArgs e)
        {
            // buttons with name TAR#
            string m_index = ((ReaLTaiizor.Controls.SkyButton)sender).Name[3..];
            int m_intTar = Convert.ToInt32(m_index);

            bool? langActiveT = AgoraObject.room.IsActiveTargetRoomsAt(m_intTar);

            if (langActiveT != null)
            {
                bool langActive = (bool)langActiveT;

                if (selectedTargetLangs == 1 && langActive) return;

                selectedTargetLangs = langActive ?
                    selectedTargetLangs - 1 :
                    selectedTargetLangs + 1;
#if DEBUG
                langHolder lh = AgoraObject.room.GetTargetRoomsAt(m_intTar);
#endif
                AgoraObject.room.SetActiveTargetRoomsAt(m_intTar, !langActive);

                (sender as ReaLTaiizor.Controls.SkyButton).NormalBGColorA = !langActive ? Xtractor.ButtonPushColor : DefaultBackColor;
                (sender as ReaLTaiizor.Controls.SkyButton).NormalBGColorB = !langActive ? Xtractor.ButtonPushColor : DefaultBackColor;
            }
        }

        private void ButtonRelay_Click(object sender, EventArgs e)
        {
            // buttons with name REL#
            string s_index = ((ReaLTaiizor.Controls.SkyButton)sender).Name[3..];
            int m_intRel = Convert.ToInt32(s_index);
            bool? langActiveT = AgoraObject.room.IsActiveInterpRoomsAt(m_intRel);

            if (langActiveT != null)
            {
                bool langActive = (bool)langActiveT;
#if DEBUG
                langHolder lh = AgoraObject.room.GetInterpRoomsAt(m_intRel);
#endif
                AgoraObject.room.SetActiveInterpRoomsAt(m_intRel, !langActive);

                (sender as ReaLTaiizor.Controls.SkyButton).NormalBGColorA = !langActive ? Xtractor.ButtonPushColor : DefaultBackColor;
                (sender as ReaLTaiizor.Controls.SkyButton).NormalBGColorB = !langActive ? Xtractor.ButtonPushColor : DefaultBackColor;
            }
        }

        internal void UnPublish()
        {
            CancelPublish();

            foreach (var pair in BtnCmbPairs)
            {
                if (pair.langNotActive == false)
                {
                    //pair.ButtonRelay_Click();
                    SkyBtnUpdate(pair.btn, Color.FromArgb(80, 80, 80), Color.FromArgb(64, 64, 64), Color.FromArgb(64, 64, 64),
                    Color.FromArgb(50, 50, 50), Color.White, Color.LightGray, Color.FromArgb(45, 45, 45), Color.Red,
                    Color.DarkRed);
                    pair.btn.Refresh();
                }
            }
            IsPublishing = false;
            mButton_start.Text = "Start!";
        }

        //Это нужно дописать. Вместо микрофона переводчика мы берём звук из физического порта подключённого по драйверу ASIO устройства.
        //Смотреть документацию здесь: http://bass.radio42.com/help/html/12c0dcd4-280e-02ed-3fb1-942599521e15.htm
        //И здесь: http://bass.radio42.com/help/html/4a5b6aaf-7c59-0ba8-8ca0-7019303e3879.htm
        internal void Publish()
        {
            CancelPublish();
            XAgora = new List<Process>();
            int index = 0;
            foreach (var pair in BtnCmbPairs)
            {
                if (pair.langNotActive == false)
                {

                    int indId = pair.GetIndexID();
                    string ind = devicesOutInd[indId];

                    SkyBtnUpdate(pair.btn, Color.FromArgb(80, 80, 80), Color.FromArgb(64, 64, 64), Color.FromArgb(64, 64, 64),
                        Color.FromArgb(50, 50, 50), Color.White, Color.LightGray, Color.FromArgb(45, 45, 45), Color.DarkOrange,
                        Color.Orange);
                    pair.btn.Refresh();
                    langHolder lh = AgoraObject.room.GetTargetRoomsAt(pair.GetIndexLang() + 1);
                    int id = System.Diagnostics.Process.GetCurrentProcess().Id;

                    List<string> args = new() { lh.token, lh.langFull, ind , id.ToString()};

                    Process proc = new Process();
                    proc.StartInfo.CreateNoWindow = true;
                    proc = System.Diagnostics.Process.Start("appIn.exe", args);
                    System.Threading.Thread.Sleep(60);

                    XAgora.Add(proc);
                    index++;
                }
            }
            IsPublishing = true;
            mButton_start.Text = "Stop?";
        }

        private void comboBoxInterpreterRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            //RoomIndex = comboBoxBassDevice.SelectedIndex;
        }

        private void Xtractor_Load(object sender, EventArgs e)
        {
            BassNet.Registration("rhenrhee@gmail.com", "2X37312318152222");

            // get the Asio devices and init them
            //this.comboBoxAsioOutputDevice.Items.AddRange(BassAsio.BASS_ASIO_GetDeviceInfos());
            //for (int i = 0; i < this.comboBoxAsioOutputDevice.Items.Count; i++)
            //{
            //    if (!BassAsio.BASS_ASIO_Init(i, BASSASIOInit.BASS_ASIO_DEFAULT))
            //        this.comboBoxAsioOutputDevice.Items[i] = BassAsio.BASS_ASIO_ErrorGetCode();
            //}
            //this.comboBoxAsioOutputDevice.SelectedIndex = 0;
            //this.comboBoxAsioInputDevice.SelectedIndex = this.comboBoxAsioOutputDevice.SelectedIndex;

            //this.comboBoxAsioInputDevice.Items.AddRange(BassAsio.BASS_ASIO_GetDeviceInfos());
            //if (comboBoxAsioInputDevice.Items.Count > 0)
            //    comboBoxAsioInputDevice.SelectedIndex = 0;

            // get the bass devices, init them and set the default one
            //this.comboBoxBassDevice.Items.AddRange(Bass.BASS_GetDeviceInfos());
            //int n = 0;
            //foreach (BASS_DEVICEINFO info in this.comboBoxBassDevice.Items)
            //{
            //    Bass.BASS_Init(n, 48000, BASSInit.BASS_DEVICE_DEFAULT, this.Handle);
            //    if (info.IsDefault)
            //    {
            //        this.comboBoxBassDevice.SelectedItem = info;
            //    }
            //    n++;
            //}
        }

        private void comboBoxAsioInputDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            //// get the available Asio channels
            ////this.comboBoxAsioInputChannel1.Items.Clear();
            ////this.comboBoxAsioInputChannel1.Text = "";
            //int n = this.comboBoxAsioInputDevice.SelectedIndex;

            //if (BassAsio.BASS_ASIO_GetDevice() == n || BassAsio.BASS_ASIO_SetDevice(n))
            //{
            //    BASS_ASIO_INFO info = BassAsio.BASS_ASIO_GetInfo();
            //    if (info != null)
            //    {
            //        // assuming stereo input
            //        for (int i = 0; i < info.inputs; i += 2)
            //        {
            //            BASS_ASIO_CHANNELINFO chanInfo = BassAsio.BASS_ASIO_ChannelGetInfo(true, i);
            //            if (chanInfo != null) 
            //                ;
            //                //this.comboBoxAsioInputChannel1.DataSource = (devicesOutName);
            //        }
            //        //if (this.comboBoxAsioInputChannel1.Items.Count > 0)
            //        //    this.comboBoxAsioInputChannel1.SelectedIndex = 0;
            //    }
            //    else
            //    {
            //        //MessageBox.Show(String.Format("AsioError = {0}", BassAsio.BASS_ASIO_ErrorGetCode()));
            //    }
            //}
            //else
            //{
            //    //MessageBox.Show(String.Format("AsioError = {0}", BassAsio.BASS_ASIO_ErrorGetCode()));
            //}
        }
        private void CancelPublish()
        {
            foreach (var proc in XAgora)
            {
                proc.Kill();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (IsPublishing)
                UnPublish();
            else
                Publish();
        }

        private void Xtractor_FormClosed(object sender, FormClosedEventArgs e)
        {
            AgoraObject.LeaveHostChannel();
            AgoraObject.LeaveSrcChannel();
            AgoraObject.LeaveTranslChannel();
            AgoraObject.LeaveTargetChannel();
            BassAsio.BASS_ASIO_Free();
            CancelPublish();
            Owner.Show();
            Owner.Refresh();
        }

        private void mButton_start_Click(object sender, EventArgs e)
        {
            if (IsPublishing)
                UnPublish();
            else
                Publish();
        }

        private void mButton_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static void SkyBtnUpdate(ReaLTaiizor.Controls.SkyButton btn, Color BorA, Color BorB, Color BorC, Color BorD, Color Fore, Color ForeShad,
                        Color BackColor, Color NormA, Color NormB)
        {
            btn.NormalBorderColorA = BorA;
            btn.NormalBorderColorB = BorB;
            btn.NormalBorderColorC = BorC;
            btn.NormalBorderColorD = BorD;
            btn.NormalForeColor = Fore;
            btn.NormalShadowForeColor = ForeShad;
            btn.BackColor = BackColor;
            btn.NormalBGColorA = NormA;
            btn.NormalBGColorB = NormB;
        }
    }
}
