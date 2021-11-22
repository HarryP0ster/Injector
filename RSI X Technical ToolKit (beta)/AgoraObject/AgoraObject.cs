using System;
using agorartc;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using HWND = System.IntPtr;

namespace RSI_X_Desktop
{
    enum CurForm 
    {
        FormTransLater,
        FormBroadcaster,
        FormAudience,
        FormEngineer,
        FormEngineer2,
        None
    }
    static class AgoraObject
    {
        
        public const string AppID = "31f0e571a89542b09049087e3283417f";

        public static bool IsJoin { get; private set; }

        public static bool IsLocalAudioMute { get; private set; }
        public static bool IsLocalVideoMute { get; private set; }

        public static bool IsAllRemoteAudioMute { get; private set; }
        public static bool IsAllRemoteVideoMute { get; private set; }

        public static bool IsAllTransLatersAudioMute { get; private set; }

        public static string CodeRoom { get; private set; } = "";
        public static string NickName { get; private set; } = "";
        public static string ClientID { get; private set; } = "";
        public static string RoomLang { get => RoomName.Split('_')[0]; }
        public static string RoomName { get; private set; } = ""; //Full name of the interpreters room without 8 digits
        public static string RoomTarg { get; private set; } = ""; //Full name of the target room without 8 digits
        public static CurForm CurrentForm = CurForm.None;

        //TODO: DELETE LATER
        //private static Random rnd = new Random();
        //DELETE LATER

        internal static AgoraRtcEngine Rtc;

        internal static Tokens room = new Tokens();

        internal static AgoraRtcChannel m_channelSrc;
        internal static AgoraRtcChannel m_channelTransl;
        internal static AgoraRtcChannel m_channelHost;
        internal static AgoraRtcChannel m_channelTarget;
        internal static Dictionary<string, AgoraRtcChannel> m_listChannels = new();

        private static int _translStreamID;
        private static int _hostStreamID;
        internal static int translStreamID { get => _translStreamID; }
        internal static int hostStreamID { get => _hostStreamID; }
        internal static AGChannelEventHandler srcHandler;
        internal static AGChannelEventHandler translHandler;
        internal static AGChannelEventHandler hostHandler;
        internal static AGChannelEventHandler targetHandler;
        private static IFormHostHolder workForm;
        public static IFormHostHolder GetWorkForm { get => workForm; }

        public static bool m_channelSrcJoin     { get; private set; } = false;
        public static bool m_channelTranslJoin  { get; private set; } = false;
        public static bool m_channelHostJoin    { get; private set; } = false;
        public static bool m_channelTargetJoin  { get; private set; } = false;

        private static bool m_channelTranslPublish = false;
        private static string  m_channelTargetPublish = String.Empty;
        private static string  m_currentChannelSrc = String.Empty;
        public readonly static System.Text.UTF8Encoding utf8enc = new();

        [DllImport("USER32.DLL")]
        static extern bool GetWindowRect(IntPtr hWnd, out System.Drawing.Rectangle lpRect);

        static AgoraObject() 
        {
            Rtc = AgoraRtcEngine.CreateRtcEngine();
            Rtc.Initialize(new RtcEngineContext(AppID));

            Rtc.SetVideoProfile(VIDEO_PROFILE_TYPE.VIDEO_PROFILE_LANDSCAPE_180P_3, false);
        }

        private static void SetPublishAudioProfile()
        {
            Rtc.SetAudioProfile(AUDIO_PROFILE_TYPE.AUDIO_PROFILE_MUSIC_HIGH_QUALITY, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_CHATROOM_GAMING);
        }
        private static void SetDefaultAudioProfile()
        {
            Rtc.SetAudioProfile(AUDIO_PROFILE_TYPE.AUDIO_PROFILE_MUSIC_STANDARD, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_CHATROOM_GAMING);
        }

        static public void UpdateNickName(string nick)
        { NickName = nick; }
        static public void UpdateRoomName(string name)
        { RoomName = name; }
        static public void UpdateClientID(string uid)
        { ClientID = uid; }

        #region token logic
        static public bool JoinRoom(string code)
        {
            CodeRoom = code;
            return room.TakeToken(code);
        }

        public static Tokens GetComplexToken()          => room;
        public static string GetHostToken()             => room.GetToken;
        public static string GetHostName()              => room.GetHostName;
        public static List<string> GetLangCollection()  => room.GetLanguages.Keys.ToList();
        #endregion

        #region Mute local audio/video
        static public ERROR_CODE MuteLocalAudioStream(bool mute)
        {
            ERROR_CODE res;

            res = Rtc.MuteLocalAudioStream(mute);

            if (res == ERROR_CODE.ERR_OK)
                IsLocalAudioMute = mute;

            return res;
        }

        static public ERROR_CODE MuteLocalVideoStream(bool mute)
        {
            ERROR_CODE res;

            res = Rtc.MuteLocalVideoStream(mute);

            if (res == ERROR_CODE.ERR_OK)
                IsLocalVideoMute = mute;

            return res;
        }
        #endregion
        
        #region  mute remote video\audio
        static public void MuteAllRemoteAudioStream(bool mute)
        {
            Rtc.MuteAllRemoteAudioStreams(mute);
            m_channelHost?.MuteAllRemoteAudioStreams(mute);
            m_channelSrc?.MuteAllRemoteAudioStreams(mute);
            m_channelTransl?.MuteAllRemoteAudioStreams(mute);

            IsAllRemoteAudioMute = mute;
        }

        static public void MuteAllRemoteVideoStream(bool mute) 
        {
            Rtc.MuteAllRemoteVideoStreams(mute);
            m_channelHost?.MuteAllRemoteVideoStreams(mute);
            m_channelTransl?.MuteAllRemoteVideoStreams(mute);

            IsAllRemoteVideoMute = mute;
        }

        static public void MuteAllTransLatersAudioStream(bool mute) 
        {
            m_channelTransl?.MuteAllRemoteAudioStreams(mute);

            IsAllTransLatersAudioMute = m_channelTransl != null &&
                                        mute;
        }
        #endregion

        static public void SetWndEventHandler(IFormHostHolder form)
        {
            Rtc.InitEventHandler(new AGEngineEventHandler(form));
            srcHandler = new AGChannelEventHandler(form, CHANNEL_TYPE.CHANNEL_SRC);
            translHandler = new AGChannelEventHandler(form, CHANNEL_TYPE.CHANNEL_TRANSL);
            hostHandler = new AGChannelEventHandler(form, CHANNEL_TYPE.CHANNEL_HOST);
            targetHandler = new AGChannelEventHandler(form, CHANNEL_TYPE.CHANNEL_DEST);
            workForm = form;
        }

        #region Screen/Window capture
        public static bool EnableScreenCapture()
        {
            int wdth = Screen.PrimaryScreen.Bounds.Width;
            int hgt = Screen.PrimaryScreen.Bounds.Height;
            ScreenCaptureParameters capParam = new ScreenCaptureParameters(wdth, hgt);
            Rectangle region = new Rectangle();
            region.width = wdth;
            region.height = hgt;
            capParam.bitrate = 1200;
            capParam.frameRate = 30;
            Rtc.StartScreenCaptureByScreenRect(region, region, capParam);
            return true;
        }
        public static bool EnableWindowCapture(HWND index)
        {
            Rectangle region = new Rectangle();
            System.Drawing.Rectangle Rectangle2 = new();
            GetWindowRect((System.IntPtr)index, out Rectangle2);
            int wdth = Rectangle2.Width;
            int hgt = Rectangle2.Height;
            ScreenCaptureParameters capParam = new ScreenCaptureParameters(wdth, hgt);
            region.x = 0;
            region.y = 0;
            region.width = wdth;
            region.height = hgt;
            capParam.bitrate = 1200;
            capParam.frameRate = 30;
            Rtc.StartScreenCaptureByWindowId((ulong)index, region, capParam);
            return true;
        }
        #endregion
        
        #region Engine channel
        static public ERROR_CODE JoinChannel(string chName, string token)
        {
            ERROR_CODE res = Rtc.JoinChannel(token, chName, "", 0);

            if (res == ERROR_CODE.ERR_OK)
                IsJoin = true;
            Rtc.StartPreview();

            return res;
        }
        static public ERROR_CODE LeaveChannel()
        {
            ERROR_CODE res = Rtc.LeaveChannel();

            if (res == ERROR_CODE.ERR_OK)
                IsJoin = false;

            return res;
        }
        #endregion
        
        #region Channel src
        public static bool JoinChannelSrc(langHolder lh_holder)
        {
            if (m_currentChannelSrc == lh_holder.langFull) return true;
            if (m_currentChannelSrc == String.Empty &&
                lh_holder.Equals(langHolder.Empty) == false)
                m_currentChannelSrc = lh_holder.langFull;

            m_listChannels[m_currentChannelSrc].MuteAllRemoteAudioStreams(true);
            m_listChannels[lh_holder.langFull].MuteAllRemoteAudioStreams(false);
            m_currentChannelSrc = lh_holder.langFull;
            return true;
            //return JoinChannelSrc(lh_holder.langFull, lh_holder.token, 0, "");
        }
        public static bool JoinChannelSrc(string lpChannelName, string token, uint nUID, string info)
        {
            LeaveSrcChannel();

            m_channelSrc = Rtc.CreateChannel(lpChannelName);
            m_channelSrc.InitChannelEventHandler(srcHandler);
            m_channelSrc.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);

            ChannelMediaOptions options = new ();
            options.autoSubscribeAudio = true;
            options.autoSubscribeVideo = false;

            ERROR_CODE ret = m_channelSrc.JoinChannel(token, info, nUID, options);

            m_channelSrcJoin = (0 == ret);

            return 0 == ret;
        }
        public static void LeaveSrcChannel()
        {
            if (m_channelSrcJoin)
                m_channelSrc?.LeaveChannel();
            m_channelSrcJoin = false;

        }
        #endregion
        
        #region Channel host
        public static bool JoinChannelHost(langHolder lh_holder)
        {
            return JoinChannelHost(lh_holder.langFull, lh_holder.token, 0, "");
        }
        public static bool JoinChannelHost(string lpChannelName, string token, uint nUID, string info)
        {
            ERROR_CODE ret;
            LeaveHostChannel();

            m_channelHost = Rtc.CreateChannel(lpChannelName);
            m_channelHost.InitChannelEventHandler(hostHandler);
            m_channelHost.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER); 
            m_channelHost.SetDefaultMuteAllRemoteVideoStreams(false);
           
            ChannelMediaOptions options = new();
            options.autoSubscribeAudio = true;
            options.autoSubscribeVideo = true;

            ret = m_channelHost.JoinChannelWithUserAccount(token, NickName, options);
            //ERROR_CODE ret = m_channelHost.JoinChannel(token, info, nUID, options);

            m_channelHostJoin = (0 == ret);
            var code = m_channelHost.CreateDataStream(out _hostStreamID, true, true);

            return 0 == ret;
        }
        public static void LeaveHostChannel()
        {
            if (m_channelHostJoin)
                m_channelHost?.LeaveChannel();
            m_channelHostJoin = false;
        }
        #endregion
        
        #region Channel Transl
        public static bool JoinChannelTransl(langHolder lh_holder)
        {
            return JoinChannelTransl(lh_holder.langFull, lh_holder.token, 0, "");
        }
        public static bool JoinChannelTransl(string lpChannelName, string token, uint nUID, string info)
        {
            ERROR_CODE ret;
            LeaveTranslChannel();

            m_channelTransl = Rtc.CreateChannel(lpChannelName);
            m_channelTransl.InitChannelEventHandler(translHandler);
            m_channelTransl.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

            ChannelMediaOptions options = new();
            options.autoSubscribeAudio = true;
            options.autoSubscribeVideo = true;

            //string m_selfAccount = rnd.Next().ToString();
            SetDefaultAudioProfile();
            ret = m_channelTransl.JoinChannelWithUserAccount(token, NickName, options);

            m_channelTranslJoin = (0 == ret);
            var code = m_channelTransl.CreateDataStream(out _translStreamID, true, true);

            return 0 == ret;
        }
        public static void LeaveTranslChannel()
        {
            m_channelTransl?.MuteAllRemoteAudioStreams(true);
            m_channelTransl?.MuteAllRemoteVideoStreams(true);

            if (m_channelTranslPublish) m_channelTransl?.Unpublish();
            if (m_channelTranslJoin) m_channelTransl?.LeaveChannel();

            m_channelTranslPublish = false;
            m_channelTranslJoin = false;
        }
        
        #endregion
        
        #region Channel target
        public static bool JoinChannelTarget(langHolder lh_holder)
        {
            //return JoinChannelTarget(lh_holder.langFull, lh_holder.token, 0, "");
            return true;
        }
        public static bool JoinChannelTarget(string lpChannelName, string token, uint nUID, string info)
        {
            LeaveTargetChannel();

            m_channelTarget = Rtc.CreateChannel(lpChannelName);
            m_channelTarget.InitChannelEventHandler(targetHandler);
            m_channelTarget.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

            ChannelMediaOptions options = new();
            options.autoSubscribeAudio = false;
            options.autoSubscribeVideo = false;

            SetPublishAudioProfile();
            ERROR_CODE ret = m_channelTarget.JoinChannel(token, info, nUID, options);

            m_channelTargetJoin = (0 == ret);

            return 0 == ret;
        }
        public static void LeaveTargetChannel()
        {
            //if (m_channelTargetPublish) m_channelTarget?.Unpublish();
            if (m_channelTargetJoin) m_channelTarget?.LeaveChannel();

            //m_channelTargetPublish = false;
            m_channelTargetJoin = false;
        }
        #endregion

        #region ChannelInterpreters
        public static void UpdateChannelRelayDict()
        {
            foreach (var lg in room.GetTargetLangs) 
            {
                if (lg.langFull == room.GetHostName) 
                {
                    m_listChannels.Add(room.GetHostName, m_channelHost);
                    continue;
                }

                m_listChannels.Add(
                    lg.langFull,
                    JoinToDestChannelAsAudinecer(lg.langFull, lg.token, 0, "")
                    );
            }
        }
        public static AgoraRtcChannel JoinToDestChannelAsAudinecer(string lpChannelName, string token, uint nUID, string info)
        {
            ERROR_CODE ret;

            var channel = Rtc.CreateChannel(lpChannelName);
            channel.InitChannelEventHandler(new AGChannelEventHandler(workForm, CHANNEL_TYPE.CHANNEL_DEST));
            channel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);

            ChannelMediaOptions options = new();
            options.autoSubscribeAudio = true;
            options.autoSubscribeVideo = true;

            SetDefaultAudioProfile();
            //ret = channel.JoinChannel(token, info, nUID, options);
            ret = channel.JoinChannelWithUserAccount(token, NickName, options);
            channel.MuteAllRemoteAudioStreams(true);

            return channel;
        }
            public static AgoraRtcChannel JoinToDestChannelAsBroadcaster(string lpChannelName, string token, uint nUID, string info)
        {
            ERROR_CODE ret;

            var channel = Rtc.CreateChannel(lpChannelName);
            channel.InitChannelEventHandler(new AGChannelEventHandler(workForm, CHANNEL_TYPE.CHANNEL_DEST));
            channel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

            ChannelMediaOptions options = new();
            options.autoSubscribeAudio = true;
            options.autoSubscribeVideo = true;

            SetDefaultAudioProfile();
            ret = channel.JoinChannelWithUserAccount(token, NickName, options);
            //channel.MuteAllRemoteAudioStreams(true);

            return channel;
        }
        public static void LeaveChannelsRelay()
        {
            foreach(var channel in m_listChannels.Values)
            {
                channel?.Unpublish();
                channel?.LeaveChannel();
                channel?.Dispose();
            }
            m_listChannels.Clear();
        }
        #endregion

        #region publish
        static public ERROR_CODE TogglePublish(CHANNEL_TYPE channel, langHolder langDest)
        {
            ERROR_CODE ret = ERROR_CODE.ERR_NOT_INITIALIZED;

            if (m_channelTargetPublish != String.Empty &&
                m_channelTargetPublish != langDest.langFull)
            {
                m_listChannels[m_channelTargetPublish].Unpublish();
                m_listChannels[m_channelTargetPublish].LeaveChannel();
                m_listChannels[m_channelTargetPublish].Dispose();
                m_listChannels[m_channelTargetPublish] = null;
            }

            foreach (var lg in room.GetTargetLangs)
            {
                if (langDest.Equals(lg)) {
                    m_listChannels[langDest.langFull].LeaveChannel();
                    m_listChannels[langDest.langFull].Dispose();
                    m_listChannels[langDest.langFull] = null;
                    m_listChannels[langDest.langFull] = JoinToDestChannelAsBroadcaster(lg.langFull, lg.token, 0, "");
                }

                if (m_listChannels.ContainsKey(lg.langFull) == false ||
                    m_listChannels[lg.langFull] != null) continue;
                m_listChannels[m_channelTargetPublish] = JoinToDestChannelAsAudinecer(lg.langFull, lg.token, 0, "");
                m_channelTargetPublish = string.Empty;
            }

            if (m_channelTranslPublish) 
            {
                m_channelTransl.Unpublish();
                m_channelTranslPublish = false;
            }

            if (langDest.Equals(langHolder.Empty) == false) 
            {
                m_channelTargetPublish = langDest.langFull;
                m_channelTarget = m_listChannels[langDest.langFull];
            }

            switch (channel) 
            {
                case CHANNEL_TYPE.CHANNEL_SRC:
                case CHANNEL_TYPE.CHANNEL_HOST:
                    break;
                case CHANNEL_TYPE.CHANNEL_TRANSL:
                    if (m_channelTransl != null && m_channelTranslJoin) 
                        ret = m_channelTransl.Publish();

                    m_channelTranslPublish = (0 == ret);
                    break;
                case CHANNEL_TYPE.CHANNEL_DEST:
                    if (m_channelTarget != null) 
                        ret = m_channelTarget.Publish();

                    m_channelTargetPublish = langDest.langFull;
                    break;
            }
            return ret;
        }
        internal static bool IsPublish()
        {
            return m_channelTargetPublish != string.Empty;
        }
        #endregion

        public static void UpdateUserVolume(uint uid, int volume)
        {
            if (m_channelTransl != null)
                m_channelTransl.SetRemoteVoicePosition(uid, 0, volume);
        }

        internal static void UpdateTargRoom(string langFull)
        {
            if (langFull != string.Empty)
                langFull = langFull.Remove(3,2);

            RoomTarg = langFull;
        }
    }
}