using System;

namespace Leap
{
	public interface IController : IDisposable
	{
		event EventHandler<ConnectionEventArgs> Connect;

		event EventHandler<ConnectionLostEventArgs> Disconnect;

		event EventHandler<FrameEventArgs> FrameReady;

		event EventHandler<DeviceEventArgs> Device;

		event EventHandler<DeviceEventArgs> DeviceLost;

		event EventHandler<ImageEventArgs> ImageReady;

		event EventHandler<DeviceFailureEventArgs> DeviceFailure;

		event EventHandler<LogEventArgs> LogMessage;

		event EventHandler<PolicyEventArgs> PolicyChange;

		event EventHandler<ConfigChangeEventArgs> ConfigChange;

		event EventHandler<DistortionEventArgs> DistortionChange;

		bool IsConnected
		{
			get;
		}

		Config Config
		{
			get;
		}

		DeviceList Devices
		{
			get;
		}

		Frame Frame(int history = 0);

		Frame GetTransformedFrame(LeapTransform trs, int history = 0);

		Frame GetInterpolatedFrame(long time);

		void SetPolicy(Controller.PolicyFlag policy);

		void ClearPolicy(Controller.PolicyFlag policy);

		bool IsPolicySet(Controller.PolicyFlag policy);

		long Now();
	}
}
