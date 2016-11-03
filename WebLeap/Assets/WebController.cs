using LeapInternal;
using System;
using System.Threading;
using UnityEngine;

namespace Leap
{
    public class WebController : IDisposable, ILeapController
    {
        public enum PolicyFlag
        {
            POLICY_DEFAULT,
            POLICY_BACKGROUND_FRAMES,
            POLICY_OPTIMIZE_HMD = 4,
            POLICY_ALLOW_PAUSE_RESUME = 8
        }

        protected WebConnection _connection;

        protected bool _disposed = false;

        protected Config _config;

        protected bool _hasInitialized = false;

        protected EventHandler<LeapEventArgs> _init;

        protected bool _hasConnected = false;

        protected EventHandler<ConnectionEventArgs> _connect;

        public event EventHandler<LeapEventArgs> Init
        {
            add
            {
                if (this._hasInitialized)
                {
                    value(this, new LeapEventArgs(LeapEvent.EVENT_INIT));
                }
                this._init = (EventHandler<LeapEventArgs>)Delegate.Combine(this._init, value);
            }
            remove
            {
                this._init = (EventHandler<LeapEventArgs>)Delegate.Remove(this._init, value);
            }
        }

        public event EventHandler<ConnectionEventArgs> Connect
        {
            add
            {
                if (this._hasConnected)
                {
                    value(this, new ConnectionEventArgs());
                }
                this._connect = (EventHandler<ConnectionEventArgs>)Delegate.Combine(this._connect, value);
            }
            remove
            {
                this._connect = (EventHandler<ConnectionEventArgs>)Delegate.Remove(this._connect, value);
            }
        }

        public event EventHandler<ConnectionLostEventArgs> Disconnect
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapConnectionLost = (EventHandler<ConnectionLostEventArgs>)Delegate.Combine(expr_07.LeapConnectionLost, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapConnectionLost = (EventHandler<ConnectionLostEventArgs>)Delegate.Remove(expr_07.LeapConnectionLost, value);
            }
        }

        public event EventHandler<FrameEventArgs> FrameReady
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapFrame = (EventHandler<FrameEventArgs>)Delegate.Combine(expr_07.LeapFrame, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapFrame = (EventHandler<FrameEventArgs>)Delegate.Remove(expr_07.LeapFrame, value);
            }
        }

        public event EventHandler<InternalFrameEventArgs> InternalFrameReady
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapInternalFrame = (EventHandler<InternalFrameEventArgs>)Delegate.Combine(expr_07.LeapInternalFrame, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapInternalFrame = (EventHandler<InternalFrameEventArgs>)Delegate.Remove(expr_07.LeapInternalFrame, value);
            }
        }

        public event EventHandler<DeviceEventArgs> Device
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapDevice = (EventHandler<DeviceEventArgs>)Delegate.Combine(expr_07.LeapDevice, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapDevice = (EventHandler<DeviceEventArgs>)Delegate.Remove(expr_07.LeapDevice, value);
            }
        }

        public event EventHandler<DeviceEventArgs> DeviceLost
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapDeviceLost = (EventHandler<DeviceEventArgs>)Delegate.Combine(expr_07.LeapDeviceLost, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapDeviceLost = (EventHandler<DeviceEventArgs>)Delegate.Remove(expr_07.LeapDeviceLost, value);
            }
        }

        public event EventHandler<ImageEventArgs> ImageReady
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapImageReady = (EventHandler<ImageEventArgs>)Delegate.Combine(expr_07.LeapImageReady, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapImageReady = (EventHandler<ImageEventArgs>)Delegate.Remove(expr_07.LeapImageReady, value);
            }
        }

        public event EventHandler<ImageRequestFailedEventArgs> ImageRequestFailed
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapImageRequestFailed = (EventHandler<ImageRequestFailedEventArgs>)Delegate.Combine(expr_07.LeapImageRequestFailed, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapImageRequestFailed = (EventHandler<ImageRequestFailedEventArgs>)Delegate.Remove(expr_07.LeapImageRequestFailed, value);
            }
        }

        public event EventHandler<DeviceFailureEventArgs> DeviceFailure
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapDeviceFailure = (EventHandler<DeviceFailureEventArgs>)Delegate.Combine(expr_07.LeapDeviceFailure, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapDeviceFailure = (EventHandler<DeviceFailureEventArgs>)Delegate.Remove(expr_07.LeapDeviceFailure, value);
            }
        }

        public event EventHandler<LogEventArgs> LogMessage
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapLogEvent = (EventHandler<LogEventArgs>)Delegate.Combine(expr_07.LeapLogEvent, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapLogEvent = (EventHandler<LogEventArgs>)Delegate.Remove(expr_07.LeapLogEvent, value);
            }
        }

        public event EventHandler<PolicyEventArgs> PolicyChange
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapPolicyChange = (EventHandler<PolicyEventArgs>)Delegate.Combine(expr_07.LeapPolicyChange, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapPolicyChange = (EventHandler<PolicyEventArgs>)Delegate.Remove(expr_07.LeapPolicyChange, value);
            }
        }

        public event EventHandler<ConfigChangeEventArgs> ConfigChange
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapConfigChange = (EventHandler<ConfigChangeEventArgs>)Delegate.Combine(expr_07.LeapConfigChange, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapConfigChange = (EventHandler<ConfigChangeEventArgs>)Delegate.Remove(expr_07.LeapConfigChange, value);
            }
        }

        public event EventHandler<DistortionEventArgs> DistortionChange
        {
            add
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapDistortionChange = (EventHandler<DistortionEventArgs>)Delegate.Combine(expr_07.LeapDistortionChange, value);
            }
            remove
            {
                WebConnection expr_07 = this._connection;
                expr_07.LeapDistortionChange = (EventHandler<DistortionEventArgs>)Delegate.Remove(expr_07.LeapDistortionChange, value);
            }
        }

        public SynchronizationContext EventContext
        {
            get
            {
                return this._connection.EventContext;
            }
            set
            {
                this._connection.EventContext = value;
            }
        }

        public bool IsServiceConnected
        {
            get
            {
                return this._connection.IsServiceConnected;
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.IsServiceConnected && this.Devices.Count > 0;
            }
        }

        public Config Config
        {
            get
            {
                if (this._config == null)
                {
                    this._config = new Config(this._connection.ConnectionKey);
                }
                return this._config;
            }
        }

        public DeviceList Devices
        {
            get
            {
                return this._connection.Devices;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                }
                this._disposed = true;
            }
        }

        public WebController() : this(0)
		{
        }

        public WebController(int connectionKey)
        {
            this._connection = WebConnection.GetConnection(connectionKey);
            this._connection.EventContext = SynchronizationContext.Current;
            this._connection.LeapInit += new EventHandler<LeapEventArgs>(this.OnInit);
            this._connection.LeapConnection += new EventHandler<ConnectionEventArgs>(this.OnConnect);
            WebConnection expr_72 = this._connection;
            expr_72.LeapConnectionLost = (EventHandler<ConnectionLostEventArgs>)Delegate.Combine(expr_72.LeapConnectionLost, new EventHandler<ConnectionLostEventArgs>(this.OnDisconnect));
            Debug.Log("Gonna Start Connection");
            this._connection.Start();
        }

        public void StartConnection()
        {
            this._connection.Start();
        }

        public void StopConnection()
        {
            this._connection.Stop();
        }

        public Image RequestImages(long frameId, Image.ImageType type)
        {
            return this._connection.RequestImages(frameId, type);
        }

        public Image RequestImages(long frameId, Image.ImageType type, byte[] imageBuffer)
        {
            return this._connection.RequestImages(frameId, type, imageBuffer);
        }

        public void SetPolicy(Controller.PolicyFlag policy)
        {
            this._connection.SetPolicy(policy);
        }

        public void ClearPolicy(Controller.PolicyFlag policy)
        {
            this._connection.ClearPolicy(policy);
        }

        public bool IsPolicySet(Controller.PolicyFlag policy)
        {
            return this._connection.IsPolicySet(policy);
        }

        public Frame Frame(int history)
        {
            Frame frame = new Frame();
            LEAP_TRACKING_EVENT lEAP_TRACKING_EVENT;
            this._connection.Frames.Get(out lEAP_TRACKING_EVENT, history);
            frame.CopyFrom(ref lEAP_TRACKING_EVENT);
            return frame;
        }

        public void Frame(Frame toFill, int history)
        {
            LEAP_TRACKING_EVENT lEAP_TRACKING_EVENT;
            this._connection.Frames.Get(out lEAP_TRACKING_EVENT, history);
            toFill.CopyFrom(ref lEAP_TRACKING_EVENT);
        }

        public Frame Frame()
        {
            return this.Frame(0);
        }

        public void Frame(Frame toFill)
        {
            this.Frame(toFill, 0);
        }

        public Frame GetTransformedFrame(LeapTransform trs, int history = 0)
        {
            return new Frame().CopyFrom(this.Frame(history)).Transform(trs);
        }

        public Frame GetInterpolatedFrame(long time)
        {
            return this._connection.GetInterpolatedFrame(time);
        }

        public void GetInterpolatedFrame(Frame toFill, long time)
        {
            this._connection.GetInterpolatedFrame(toFill, time);
        }

        public long Now()
        {
            return LeapC.GetNow();
        }

        public FailedDeviceList FailedDevices()
        {
            return this._connection.FailedDevices;
        }

        protected virtual void OnInit(object sender, LeapEventArgs eventArgs)
        {
            this._hasInitialized = true;
        }

        protected virtual void OnConnect(object sender, ConnectionEventArgs eventArgs)
        {
            this._hasConnected = true;
        }

        protected virtual void OnDisconnect(object sender, ConnectionLostEventArgs eventArgs)
        {
            this._hasInitialized = false;
            this._hasConnected = false;
        }
    }
}