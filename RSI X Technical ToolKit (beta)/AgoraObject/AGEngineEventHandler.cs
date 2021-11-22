using System;

using agorartc;

namespace RSI_X_Desktop
{
    internal class AGEngineEventHandler : IRtcEngineEventHandlerBase
    {
        private IFormHostHolder form;
        public AGEngineEventHandler(IFormHostHolder form) 
        {
            this.form = form;
        }
        public override void OnJoinChannelSuccess(string channel, uint uid, int elapsed)
        {
            Console.WriteLine("OnJoinChannelSuccess");
        }

        public override void OnReJoinChannelSuccess(string channel, uint uid, int elapsed)
        {
            Console.WriteLine("OnReJoinChannelSuccess");
        }

        public override void OnConnectionLost()
        {
            Console.WriteLine("OnConnectionLost");
        }

        public override void OnConnectionInterrupted()
        {
            Console.WriteLine("OnConnectionInterrupted");
        }

        public override void OnLeaveChannel(RtcStats stats)
        {
            Console.WriteLine("OnLeaveChannel");
        }

        public override void OnRequestToken()
        {
            Console.WriteLine("OnRequestToken");
        }

        public override void OnUserJoined(uint uid, int elapsed)
        {
            Console.WriteLine("OnUserJoined");

            if (form.RemoteWnd == IntPtr.Zero) return;
            var ret = new VideoCanvas((ulong)form.RemoteWnd, uid);

            AgoraObject.Rtc.SetupRemoteVideo(ret);
        }

        public override void OnUserOffline(uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Console.WriteLine("OnUserOffline");
        }

        public override void OnAudioVolumeIndication(AudioVolumeInfo[] speakers, uint speakerNumber, int totalVolume)
        {
            Console.WriteLine("OnAudioVolumeIndication");
        }

        public override void OnUserMuteAudio(uint uid, bool muted)
        {
            Console.WriteLine("OnUserMuteAudio");
        }

        public override void OnWarning(WARN_CODE_TYPE warn, string msg)
        {
            Console.WriteLine("OnWarning {0}", warn);
        }

        public override void OnError(ERROR_CODE error, string msg)
        {
            Console.WriteLine("OnError {0}", error);
        }

        public override void OnRtcStats(RtcStats stats)
        {
            Console.WriteLine("OnRtcStats");
        }

        public override void OnAudioMixingFinished()
        {
            Console.WriteLine("OnAudioMixingFinished");
        }

        public override void OnAudioRouteChanged(AUDIO_ROUTE_TYPE routing)
        {
            Console.WriteLine("OnAudioRouteChanged");
        }

        public override void OnFirstRemoteVideoDecoded(uint uid, int width, int height, int elapsed)
        {
            Console.WriteLine("OnFirstRemoteVideoDecoded");
        }

        public override void OnVideoSizeChanged(uint uid, int width, int height, int rotation)
        {
            Console.WriteLine("OnVideoSizeChanged");
        }

        public override void OnClientRoleChanged(CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole)
        {
            Console.WriteLine("OnClientRoleChanged");
        }

        public override void OnUserMuteVideo(uint uid, bool muted)
        {
            Console.WriteLine("OnUserMuteVideo");
        }

        public override void OnMicrophoneEnabled(bool enabled)
        {
            Console.WriteLine("OnMicrophoneEnabled");
        }

        public override void OnApiCallExecuted(ERROR_CODE err, string api, string result)
        {
            Console.WriteLine("OnApiCallExecuted");
        }

        public override void OnFirstLocalAudioFrame(int elapsed)
        {
            Console.WriteLine("OnFirstLocalAudioFrame");
        }

        public override void OnFirstLocalAudioFramePublished(int elapsed)
        {
            Console.WriteLine("OnFirstLocalAudioFramePublished");
        }

        public override void OnFirstRemoteAudioFrame(uint uid, int elapsed)
        {
            Console.WriteLine("OnFirstRemoteAudioFrame");
        }

        public override void OnLastmileQuality(int quality)
        {
            Console.WriteLine("OnLastmileQuality");
        }

        public override void OnAudioQuality(uint uid, int quality, ushort delay, ushort lost)
        {
            Console.WriteLine("OnAudioQuality");
        }

        public override void OnStreamInjectedStatus(string url, uint uid, int status)
        {
            Console.WriteLine("OnStreamInjectedStatus");
        }

        public override void OnStreamUnpublished(string url)
        {
            Console.WriteLine("OnStreamUnpublished");
        }

        public override void OnStreamPublished(string url, ERROR_CODE error)
        {
            Console.WriteLine("OnStreamPublished");
        }

        public override void OnStreamMessageError(uint uid, int streamId, int code, int missed, int cached)
        {
            Console.WriteLine("OnStreamMessageError");
        }

        public override void OnStreamMessage(uint uid, int streamId, byte[] data, uint length)
        {
            Console.WriteLine("OnStreamMessage");
        }

        public override void OnConnectionBanned()
        {
            Console.WriteLine("OnConnectionBanned");
        }

        public override void OnRemoteVideoTransportStats(uint uid, ushort delay, ushort lost, ushort rxKBitRate)
        {
            Console.WriteLine("OnRemoteVideoTransportStats");
        }

        public override void OnRemoteAudioTransportStats(uint uid, ushort delay, ushort lost, ushort rxKBitRate)
        {
            Console.WriteLine("OnRemoteAudioTransportStats");
        }

        public override void OnTranscodingUpdated()
        {
            Console.WriteLine("OnTranscodingUpdated");
        }

        public override void OnAudioDeviceVolumeChanged(MEDIA_DEVICE_TYPE deviceType, int volume, bool muted)
        {
            Console.WriteLine("OnAudioDeviceVolumeChanged");
        }

        public override void OnActiveSpeaker(uint uid)
        {
            Console.WriteLine("OnActiveSpeaker");
        }

        public override void OnMediaEngineStartCallSuccess()
        {
            Console.WriteLine("OnMediaEngineStartCallSuccess");
        }

        public override void OnUserSuperResolutionEnabled(uint uid, bool enabled, SUPER_RESOLUTION_STATE_REASON reason)
        {
            Console.WriteLine("OnUserSuperResolutionEnabled");
        }

        public override void OnMediaEngineLoadSuccess()
        {
            Console.WriteLine("OnMediaEngineLoadSuccess");
        }

        public override void OnVideoStopped()
        {
            Console.WriteLine("OnVideoStopped");
        }

        public override void OnTokenPrivilegeWillExpire(string token)
        {
            Console.WriteLine("OnTokenPrivilegeWillExpire");
        }

        public override void OnNetworkQuality(uint uid, int txQuality, int rxQuality)
        {
            Console.WriteLine("OnNetworkQuality");
        }

        public override void OnLocalVideoStats(LocalVideoStats stats)
        {
            Console.WriteLine("OnLocalVideoStats");
        }

        public override void OnRemoteVideoStats(RemoteVideoStats stats)
        {
            Console.WriteLine("OnRemoteVideoStats");
        }

        public override void OnRemoteAudioStats(RemoteAudioStats stats)
        {
            Console.WriteLine("OnRemoteAudioStats");
        }

        public override void OnLocalAudioStats(LocalAudioStats stats)
        {
            Console.WriteLine("OnLocalAudioStats");
        }

        public override void OnFirstLocalVideoFrame(int width, int height, int elapsed)
        {
            Console.WriteLine("OnFirstLocalVideoFrame");
        }

        public override void OnFirstLocalVideoFramePublished(int elapsed)
        {
            Console.WriteLine("OnFirstLocalVideoFramePublished");
        }

        public override void OnFirstRemoteVideoFrame(uint uid, int width, int height, int elapsed)
        {
            Console.WriteLine("OnFirstRemoteVideoFrame");
        }

        public override void OnUserEnableVideo(uint uid, bool enabled)
        {
            Console.WriteLine("OnUserEnableVideo");
        }

        public override void OnAudioDeviceStateChanged(string deviceId, MEDIA_DEVICE_TYPE deviceType,
            MEDIA_DEVICE_STATE_TYPE deviceState)
        {
            Console.WriteLine("OnAudioDeviceStateChanged");
        }

        public override void OnCameraReady()
        {
            Console.WriteLine("OnCameraReady");
        }

        public override void OnCameraFocusAreaChanged(int x, int y, int width, int height)
        {
            Console.WriteLine("OnCameraFocusAreaChanged");
        }

        public override void OnCameraExposureAreaChanged(int x, int y, int width, int height)
        {
            Console.WriteLine("OnCameraExposureAreaChanged");
        }

        public override void OnRemoteAudioMixingBegin()
        {
            Console.WriteLine("OnRemoteAudioMixingBegin");
        }

        public override void OnRemoteAudioMixingEnd()
        {
            Console.WriteLine("OnRemoteAudioMixingEnd");
        }

        public override void OnAudioEffectFinished(int soundId)
        {
            Console.WriteLine("OnAudioEffectFinished");
        }

        public override void OnVideoDeviceStateChanged(string deviceId, MEDIA_DEVICE_TYPE deviceType,
            MEDIA_DEVICE_STATE_TYPE deviceState)
        {
            Console.WriteLine("OnVideoDeviceStateChanged");
        }

        public override void OnRemoteVideoStateChanged(uint uid, REMOTE_VIDEO_STATE state,
            REMOTE_VIDEO_STATE_REASON reason, int elapsed)
        {
            Console.WriteLine("OnRemoteVideoStateChanged");
        }

        public override void OnUserEnableLocalVideo(uint uid, bool enabled)
        {
            Console.WriteLine("OnUserEnableLocalVideo");
        }

        public override void OnLocalPublishFallbackToAudioOnly(bool isFallbackOrRecover)
        {
            Console.WriteLine("OnLocalPublishFallbackToAudioOnly");
        }

        public override void OnRemoteSubscribeFallbackToAudioOnly(uint uid, bool isFallbackOrRecover)
        {
            Console.WriteLine("OnRemoteSubscribeFallbackToAudioOnly");
        }

        public override void OnConnectionStateChanged(CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
        {
            Console.WriteLine("OnConnectionStateChanged");
        }

        public override void OnRtmpStreamingStateChanged(string url, RTMP_STREAM_PUBLISH_STATE state,
            RTMP_STREAM_PUBLISH_ERROR errCode)
        {
            Console.WriteLine("OnRtmpStreamingStateChanged");
        }

        public override void OnLocalUserRegistered(uint uid, string userAccount)
        {
            Console.WriteLine("OnLocalUserRegistered");
        }

        public override void OnUserInfoUpdated(uint uid, UserInfo info)
        {
            Console.WriteLine("OnUserInfoUpdated");
        }

        public override void OnLocalAudioStateChanged(LOCAL_AUDIO_STREAM_STATE state, LOCAL_AUDIO_STREAM_ERROR error)
        {
            Console.WriteLine("OnLocalAudioStateChanged");
        }

        public override void OnRemoteAudioStateChanged(uint uid, REMOTE_AUDIO_STATE state,
            REMOTE_AUDIO_STATE_REASON reason, int elapsed)
        {
            Console.WriteLine("OnRemoteAudioStateChanged");
        }

        public override void OnAudioPublishStateChanged(string channel, STREAM_PUBLISH_STATE oldState,
            STREAM_PUBLISH_STATE newState, int elapseSinceLastState)
        {
            Console.WriteLine("OnAudioPublishStateChanged");
        }

        public override void OnVideoPublishStateChanged(string channel, STREAM_PUBLISH_STATE oldState,
            STREAM_PUBLISH_STATE newState, int elapseSinceLastState)
        {
            Console.WriteLine("OnVideoPublishStateChanged");
        }

        public override void OnAudioMixingStateChanged(AUDIO_MIXING_STATE_TYPE audioMixingStateType,
            AUDIO_MIXING_ERROR_TYPE audioMixingErrorType)
        {
            Console.WriteLine("OnAudioMixingStateChanged");
        }

        public override void OnFirstRemoteAudioDecoded(uint uid, int elapsed)
        {
            Console.WriteLine("OnFirstRemoteAudioDecoded");
        }

        public override void OnLocalVideoStateChanged(LOCAL_VIDEO_STREAM_STATE localVideoState,
            LOCAL_VIDEO_STREAM_ERROR error)
        {
            Console.WriteLine("OnLocalVideoStateChanged");
        }

        public override void OnNetworkTypeChanged(NETWORK_TYPE networkType)
        {
            Console.WriteLine("OnNetworkTypeChanged");
        }

        public override void OnLastmileProbeResult(LastmileProbeResult result)
        {
            Console.WriteLine("OnLastmileProbeResult");
        }

        public override void OnChannelMediaRelayStateChanged(CHANNEL_MEDIA_RELAY_STATE state,
            CHANNEL_MEDIA_RELAY_ERROR code)
        {
            Console.WriteLine("OnChannelMediaRelayStateChanged");
        }

        public override void OnChannelMediaRelayEvent(CHANNEL_MEDIA_RELAY_EVENT code)
        {
            Console.WriteLine("OnChannelMediaRelayEvent");
        }
    }
}
