using System;
using System.Threading;

namespace Leap
{
    public interface ILeapController
    {
        Config Config { get; }
        DeviceList Devices { get; }
        SynchronizationContext EventContext { get; set; }
        bool IsConnected { get; }
        bool IsServiceConnected { get; }

        event EventHandler<ConfigChangeEventArgs> ConfigChange;
        event EventHandler<ConnectionEventArgs> Connect;
        event EventHandler<DeviceEventArgs> Device;
        event EventHandler<DeviceFailureEventArgs> DeviceFailure;
        event EventHandler<DeviceEventArgs> DeviceLost;
        event EventHandler<ConnectionLostEventArgs> Disconnect;
        event EventHandler<DistortionEventArgs> DistortionChange;
        event EventHandler<FrameEventArgs> FrameReady;
        event EventHandler<ImageEventArgs> ImageReady;
        event EventHandler<ImageRequestFailedEventArgs> ImageRequestFailed;
        event EventHandler<LeapEventArgs> Init;
        event EventHandler<InternalFrameEventArgs> InternalFrameReady;
        event EventHandler<LogEventArgs> LogMessage;
        event EventHandler<PolicyEventArgs> PolicyChange;

        void ClearPolicy(Controller.PolicyFlag policy);
        FailedDeviceList FailedDevices();
        Frame Frame();
        Frame Frame(int history);
        void Frame(Frame toFill);
        void Frame(Frame toFill, int history);
        Frame GetInterpolatedFrame(long time);
        void GetInterpolatedFrame(Frame toFill, long time);
        Frame GetTransformedFrame(LeapTransform trs, int history = 0);
        bool IsPolicySet(Controller.PolicyFlag policy);
        long Now();
        Image RequestImages(long frameId, Image.ImageType type);
        Image RequestImages(long frameId, Image.ImageType type, byte[] imageBuffer);
        void SetPolicy(Controller.PolicyFlag policy);
        void StartConnection();
        void StopConnection();
    }
}