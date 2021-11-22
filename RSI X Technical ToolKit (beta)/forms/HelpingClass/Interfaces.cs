using System;

namespace RSI_X_Desktop
{
    public interface IFormHostHolder
    {
        public IntPtr RemoteWnd { get; }
        public void UpdateRemoteWnd();
        public void RefreshLocalWnd();
        public void SetLocalVideoPreview();
        public void DevicesClosed(System.Windows.Forms.Form Wnd);
        public void SetTrackBarVolume(int volume);
    }
    public interface IFormInterpreterHolder 
    { }
}
