using System;
using agorartc;
using System.Threading;

namespace ConsoleAppOut
{
    class Program
    {
        static int parentID;
        static System.Diagnostics.Process proc;
        static void Main(string[] args)
        {

            XAgoraObject agoraObject = new XAgoraObject();

            var retInput = agoraObject.SetupOutputDevices(args[2]);

            Console.WriteLine(retInput);

            var retPubl = agoraObject.Publish(args[0], args[1]);
            Console.WriteLine(retPubl);

            if (retInput != ERROR_CODE.ERR_OK ||
                retPubl != ERROR_CODE.ERR_OK)
                return;

            parentID = System.Convert.ToInt32(args[3]);
            proc = System.Diagnostics.Process.GetProcessById(parentID);
            proc.WaitForExit();

            //proc.Exited += ParentClose;

            //Console.WriteLine(args[1]);
            //Console.WriteLine(agoraObject.nameDevice);

            //while (true)
            //    System.Threading.Thread.Sleep(100);
        }

        private static void ParentClose(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
    class XAgoraObject
    {
        AgoraRtcEngine Rtc;
        AgoraAudioRecordingDeviceManager audioInDeviceManager;
        AgoraAudioPlaybackDeviceManager audioPlaybackDeviceManager;
        AgoraAudioPlaybackDeviceManager audioOutDeviceManager;

        public const string AppID = "31f0e571a89542b09049087e3283417f";
        public string nameDevice;
        public bool IsJoin { get; private set; }

        public XAgoraObject()
        {
            Rtc = AgoraRtcEngine.CreateRtcEngine();
            Rtc.MuteLocalVideoStream(true);
            Rtc.Initialize(new RtcEngineContext(AppID));
            audioInDeviceManager = Rtc.CreateAudioRecordingDeviceManager();
            audioOutDeviceManager = Rtc.CreateAudioPlaybackDeviceManager();
        }

        public ERROR_CODE SetupOutputDevices(string ind)
        {
            return audioOutDeviceManager.SetCurrentDevice(ind);
        }

        public ERROR_CODE Publish(string token, string name)
        {
            ERROR_CODE res = Rtc.JoinChannel(token, name, "", 0);

            if (res == ERROR_CODE.ERR_OK)
                IsJoin = true;
            audioOutDeviceManager.GetCurrentDeviceInfo(out string idOUT, out string nameOUT);
            nameDevice = nameOUT;

            Console.WriteLine("\n\n\n\nHello World!");

            return res;
        }

        public void UnPublish()
        {
            Rtc.LeaveChannel();
            IsJoin = false;
        }
    }
}
