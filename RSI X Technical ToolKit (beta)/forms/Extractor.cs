using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using agorartc;
using System.Diagnostics;

namespace RSI_X_Desktop.forms
{
    public partial class Extractor : Form
    {
        private string HostName = string.Empty;
        //Публичный класс, который описывает пары языковых кнопок и списки подключенного оборудования
        public class BtnCmbPair
        {
            internal ReaLTaiizor.Controls.SkyButton btn;
            ComboBox cmb;
            //Переменная для хранения количества языков 
            int indexLang;
            //Логика активности языков
            public bool langNotActive { get; private set; } = true;
            //Индекс количества подключенного оборудования
            public int GetIndexID()
            {
                if (cmb == null)
                    return -1;
                return cmb.SelectedIndex;
            }

            public int GetIndexLang() { return indexLang; }
            public string GetLang() { return btn.Text; }

            public BtnCmbPair(ReaLTaiizor.Controls.SkyButton btn, ComboBox cmb, int indexLang)
            {
                this.btn = btn;
                this.cmb = cmb;
                this.indexLang = indexLang;
            }

            public void ButtonRelay_Click()
            {
                ButtonRelay_Click(btn, new EventArgs());
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

        AgoraAudioPlaybackDeviceManager audioOutDeviceManager;

        private int selectedTargetLangs;
        static readonly Color ButtonPushColor = Color.BurlyWood;

        List<string> devicesOutInd = new();
        List<string> devicesOutName = new();
        List<BtnCmbPair> BtnCmbPairs = new();
        List<BtnCmbPair> BtnCmbPairs2 = new();
        List<Process> XAgora = new();

        public Extractor()
        {
            InitializeComponent();
            audioOutDeviceManager = AgoraObject.Rtc.CreateAudioPlaybackDeviceManager();

            for (int i = 0; i < audioOutDeviceManager.GetDeviceCount(); i++)
            {
                string device, id;

                var ret = audioOutDeviceManager.GetDeviceInfoByIndex(i, out device, out id);

                devicesOutName.Add(device);
                devicesOutInd.Add(id);
            }

            UpdateRelayLangs();
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
        //Обновляем список языков и оборудования для работы
        private void UpdateRelayLangs()
        {
            //Переменная для языков
            var langs = AgoraObject.GetComplexToken();
            var controls = panelRelayButtons.Controls;
            int defHeight = 35;
            int offset = 1;
            int locOffset = 1;

            ReaLTaiizor.Controls.SkyButton btn;
            ComboBox cmb;
            //Соответствие пар
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

                BtnCmbPairs.Add(pair);
                offset++;
                ButtonRelay_Click(btn, new EventArgs());
                pair.UpdateColors(btn);
            }


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

                (sender as ReaLTaiizor.Controls.SkyButton).BackColor = !langActive ? ButtonPushColor : DefaultBackColor;
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

                (sender as ReaLTaiizor.Controls.SkyButton).BackColor = !langActive ? ButtonPushColor : DefaultBackColor;
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
            mButton_start.Text = "Start";
        }

        internal void Publish()
        {
            CancelPublish();
            XAgora = new List<Process>();
            int index = 1;
            foreach (var pair in BtnCmbPairs)
            {
                if (pair.langNotActive == false)
                {

                    SkyBtnUpdate(pair.btn, Color.FromArgb(80, 80, 80), Color.FromArgb(64, 64, 64), Color.FromArgb(64, 64, 64),
                        Color.FromArgb(50, 50, 50), Color.White, Color.LightGray, Color.FromArgb(45, 45, 45), Color.DarkOrange,
                        Color.Orange);
                    pair.btn.Refresh();

                    int indId = pair.GetIndexID();
                    string ind = devicesOutInd[indId];

                    langHolder lh = AgoraObject.room.GetTargetRoomsAt(pair.GetIndexLang() + 1);
                    int id = System.Diagnostics.Process.GetCurrentProcess().Id;

                    List<string> args = new() { lh.token, lh.langFull, ind, id.ToString() };

                    Process proc = new Process();
                    proc.StartInfo.CreateNoWindow = true;
                    proc = System.Diagnostics.Process.Start("appOut.exe", args);
                    System.Threading.Thread.Sleep(60);

                    XAgora.Add(proc);
                    index++;
                }
            }
            IsPublishing = true;
            mButton_start.Text = "Stop";
        }

        private void CancelPublish()
        {
            foreach (var proc in XAgora)
            {
                proc.Kill();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (IsPublishing)
                UnPublish();
            else
                Publish();
        }

        private void Ingestor_FormClosed(object sender, FormClosedEventArgs e)
        {
            AgoraObject.LeaveHostChannel();
            AgoraObject.LeaveSrcChannel();
            AgoraObject.LeaveTranslChannel();
            AgoraObject.LeaveTargetChannel();
            CancelPublish();
            Owner.Show();
            Owner.Refresh();
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
