using Leap;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace LeapInternal
{
    public class Connection
    {
        private static Dictionary<int, Connection> connectionDictionary = new Dictionary<int, Connection>();

        private DeviceList _devices = new DeviceList();

        private FailedDeviceList _failedDevices;

        private PendingImages _pendingImageRequestList = new PendingImages();

        private ObjectPool<ImageData> _imageDataCache;

        private ObjectPool<ImageData> _imageRawDataCache;

        private int _frameBufferLength = 60;

        private int _imageBufferLength = 80;

        private ulong _standardImageBufferSize = 307200uL;

        private ulong _standardRawBufferSize = 2457600uL;

        private DistortionData _currentDistortionData = new DistortionData();

        private IntPtr _leapConnection;

        private bool _isRunning = false;

        private Thread _polster;

        private ulong _requestedPolicies = 0uL;

        private ulong _activePolicies = 0uL;

        private Dictionary<uint, string> _configRequests = new Dictionary<uint, string>();

        private EventHandler<LeapEventArgs> _leapInit;

        private EventHandler<ConnectionEventArgs> _leapConnectionEvent;

        public EventHandler<ConnectionLostEventArgs> LeapConnectionLost;

        public EventHandler<DeviceEventArgs> LeapDevice;

        public EventHandler<DeviceEventArgs> LeapDeviceLost;

        public EventHandler<DeviceFailureEventArgs> LeapDeviceFailure;

        public EventHandler<PolicyEventArgs> LeapPolicyChange;

        public EventHandler<FrameEventArgs> LeapFrame;

        public EventHandler<InternalFrameEventArgs> LeapInternalFrame;

        public EventHandler<ImageEventArgs> LeapImageReady;

        public EventHandler<ImageRequestFailedEventArgs> LeapImageRequestFailed;

        public EventHandler<LogEventArgs> LeapLogEvent;

        public EventHandler<SetConfigResponseEventArgs> LeapConfigResponse;

        public EventHandler<ConfigChangeEventArgs> LeapConfigChange;

        public EventHandler<DistortionEventArgs> LeapDistortionChange;

        private bool _disposed = false;

        private eLeapRS _lastResult;

        public event EventHandler<LeapEventArgs> LeapInit
        {
            add
            {
                this._leapInit = (EventHandler<LeapEventArgs>)Delegate.Combine(this._leapInit, value);
                if (this._leapConnection != IntPtr.Zero)
                {
                    value(this, new LeapEventArgs(LeapEvent.EVENT_INIT));
                }
            }
            remove
            {
                this._leapInit = (EventHandler<LeapEventArgs>)Delegate.Remove(this._leapInit, value);
            }
        }

        public event EventHandler<ConnectionEventArgs> LeapConnection
        {
            add
            {
                this._leapConnectionEvent = (EventHandler<ConnectionEventArgs>)Delegate.Combine(this._leapConnectionEvent, value);
                if (this.IsServiceConnected)
                {
                    value(this, new ConnectionEventArgs());
                }
            }
            remove
            {
                this._leapConnectionEvent = (EventHandler<ConnectionEventArgs>)Delegate.Remove(this._leapConnectionEvent, value);
            }
        }

        public int ConnectionKey
        {
            get;
            private set;
        }

        public CircularObjectBuffer<LEAP_TRACKING_EVENT> Frames
        {
            get;
            set;
        }

        public SynchronizationContext EventContext
        {
            get;
            set;
        }

        public bool IsServiceConnected
        {
            get
            {
                bool result;
                if (this._leapConnection == IntPtr.Zero)
                {
                    result = false;
                }
                else
                {
                    LEAP_CONNECTION_INFO lEAP_CONNECTION_INFO = default(LEAP_CONNECTION_INFO);
                    lEAP_CONNECTION_INFO.size = (uint)Marshal.SizeOf(lEAP_CONNECTION_INFO);
                    eLeapRS connectionInfo = LeapC.GetConnectionInfo(this._leapConnection, out lEAP_CONNECTION_INFO);
                    this.reportAbnormalResults("LeapC GetConnectionInfo call was ", connectionInfo);
                    result = (lEAP_CONNECTION_INFO.status == eLeapConnectionStatus.eLeapConnectionStatus_Connected);
                }
                return result;
            }
        }

        public DeviceList Devices
        {
            get
            {
                if (this._devices == null)
                {
                    this._devices = new DeviceList();
                }
                return this._devices;
            }
        }

        public FailedDeviceList FailedDevices
        {
            get
            {
                if (this._failedDevices == null)
                {
                    this._failedDevices = new FailedDeviceList();
                }
                return this._failedDevices;
            }
        }

        public static Connection GetConnection(int connectionKey = 0)
        {
            Connection connection;
            if (!Connection.connectionDictionary.TryGetValue(connectionKey, out connection))
            {
                connection = new Connection(connectionKey);
                Connection.connectionDictionary.Add(connectionKey, connection);
            }
            return connection;
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
                this.Stop();
                LeapC.DestroyConnection(this._leapConnection);
                this._disposed = true;
            }
        }

        ~Connection()
        {
            this.Dispose(false);
        }

        private Connection(int connectionKey)
        {
            this.ConnectionKey = connectionKey;
            this._leapConnection = IntPtr.Zero;
            this.Frames = new CircularObjectBuffer<LEAP_TRACKING_EVENT>(this._frameBufferLength);
            this._imageDataCache = new ObjectPool<ImageData>(this._imageBufferLength, false);
            this._imageRawDataCache = new ObjectPool<ImageData>(this._imageBufferLength, false);
        }

        public void Start()
        {
            if (!this._isRunning)
            {
                if (this._leapConnection == IntPtr.Zero)
                {
                    eLeapRS eLeapRS = LeapC.CreateConnection(out this._leapConnection);
                    if (eLeapRS != eLeapRS.eLeapRS_Success || this._leapConnection == IntPtr.Zero)
                    {
                        this.reportAbnormalResults("LeapC CreateConnection call was ", eLeapRS);
                        return;
                    }
                    eLeapRS = LeapC.OpenConnection(this._leapConnection);
                    if (eLeapRS != eLeapRS.eLeapRS_Success)
                    {
                        this.reportAbnormalResults("LeapC OpenConnection call was ", eLeapRS);
                        return;
                    }
                }
                this._isRunning = true;
                this._polster = new Thread(new ThreadStart(this.processMessages));
                this._polster.IsBackground = true;
                this._polster.Start();
            }
        }

        public void Stop()
        {
            if (this._isRunning)
            {
                this._isRunning = false;
                this._polster.Join();
            }
        }

        private void processMessages()
        {
            try
            {
                this._leapInit.DispatchOnContext(this, this.EventContext, new LeapEventArgs(LeapEvent.EVENT_INIT));
                while (this._isRunning)
                {
                    LEAP_CONNECTION_MESSAGE lEAP_CONNECTION_MESSAGE = default(LEAP_CONNECTION_MESSAGE);
                    uint timeout = 1000u;
                    eLeapRS eLeapRS = LeapC.PollConnection(this._leapConnection, timeout, ref lEAP_CONNECTION_MESSAGE);
                    if (eLeapRS != eLeapRS.eLeapRS_Success)
                    {
                        this.reportAbnormalResults("LeapC PollConnection call was ", eLeapRS);
                    }
                    else
                    {
                        eLeapEventType type = lEAP_CONNECTION_MESSAGE.type;

                        switch (type)
                        {
                            case eLeapEventType.eLeapEventType_Connection:
                                {
                                    LEAP_CONNECTION_EVENT lEAP_CONNECTION_EVENT;
                                    StructMarshal<LEAP_CONNECTION_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_CONNECTION_EVENT);
                                    this.handleConnection(ref lEAP_CONNECTION_EVENT);
                                    break;
                                }
                            case eLeapEventType.eLeapEventType_ConnectionLost:
                                {
                                    LEAP_CONNECTION_LOST_EVENT lEAP_CONNECTION_LOST_EVENT;
                                    StructMarshal<LEAP_CONNECTION_LOST_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_CONNECTION_LOST_EVENT);
                                    this.handleConnectionLost(ref lEAP_CONNECTION_LOST_EVENT);
                                    break;
                                }
                            case eLeapEventType.eLeapEventType_Device:
                                {
                                    LEAP_DEVICE_EVENT lEAP_DEVICE_EVENT;
                                    StructMarshal<LEAP_DEVICE_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_DEVICE_EVENT);
                                    this.handleDevice(ref lEAP_DEVICE_EVENT);
                                    break;
                                }
                            case eLeapEventType.eLeapEventType_DeviceFailure:
                                {
                                    LEAP_DEVICE_FAILURE_EVENT lEAP_DEVICE_FAILURE_EVENT;
                                    StructMarshal<LEAP_DEVICE_FAILURE_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_DEVICE_FAILURE_EVENT);
                                    this.handleFailedDevice(ref lEAP_DEVICE_FAILURE_EVENT);
                                    break;
                                }
                            case eLeapEventType.eLeapEventType_PolicyChange:
                                {
                                    LEAP_POLICY_EVENT lEAP_POLICY_EVENT;
                                    StructMarshal<LEAP_POLICY_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_POLICY_EVENT);
                                    this.handlePolicyChange(ref lEAP_POLICY_EVENT);
                                    break;
                                }
                            default:
                                switch (type)
                                {
                                    case eLeapEventType.eLeapEventType_Tracking:
                                        {
                                            LEAP_TRACKING_EVENT lEAP_TRACKING_EVENT;
                                            StructMarshal<LEAP_TRACKING_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_TRACKING_EVENT);
                                            this.handleTrackingMessage(ref lEAP_TRACKING_EVENT);
                                            break;
                                        }
                                    case eLeapEventType.eLeapEventType_ImageRequestError:
                                        {
                                            LEAP_IMAGE_FRAME_REQUEST_ERROR_EVENT lEAP_IMAGE_FRAME_REQUEST_ERROR_EVENT;
                                            StructMarshal<LEAP_IMAGE_FRAME_REQUEST_ERROR_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_IMAGE_FRAME_REQUEST_ERROR_EVENT);
                                            this.handleFailedImageRequest(ref lEAP_IMAGE_FRAME_REQUEST_ERROR_EVENT);
                                            break;
                                        }
                                    case eLeapEventType.eLeapEventType_ImageComplete:
                                        {
                                            LEAP_IMAGE_COMPLETE_EVENT lEAP_IMAGE_COMPLETE_EVENT;
                                            StructMarshal<LEAP_IMAGE_COMPLETE_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_IMAGE_COMPLETE_EVENT);
                                            this.handleImageCompletion(ref lEAP_IMAGE_COMPLETE_EVENT);
                                            break;
                                        }
                                    case eLeapEventType.eLeapEventType_LogEvent:
                                        {
                                            LEAP_LOG_EVENT lEAP_LOG_EVENT;
                                            StructMarshal<LEAP_LOG_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_LOG_EVENT);
                                            this.reportLogMessage(ref lEAP_LOG_EVENT);
                                            break;
                                        }
                                    case eLeapEventType.eLeapEventType_DeviceLost:
                                        {
                                            LEAP_DEVICE_EVENT lEAP_DEVICE_EVENT2;
                                            StructMarshal<LEAP_DEVICE_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_DEVICE_EVENT2);
                                            this.handleLostDevice(ref lEAP_DEVICE_EVENT2);
                                            break;
                                        }
                                    case eLeapEventType.eLeapEventType_ConfigResponse:
                                        this.handleConfigResponse(ref lEAP_CONNECTION_MESSAGE);
                                        break;
                                    case eLeapEventType.eLeapEventType_ConfigChange:
                                        {
                                            LEAP_CONFIG_CHANGE_EVENT lEAP_CONFIG_CHANGE_EVENT;
                                            StructMarshal<LEAP_CONFIG_CHANGE_EVENT>.PtrToStruct(lEAP_CONNECTION_MESSAGE.eventStructPtr, out lEAP_CONFIG_CHANGE_EVENT);
                                            this.handleConfigChange(ref lEAP_CONFIG_CHANGE_EVENT);
                                            break;
                                        }
                                    default:
                                        Logger.Log("Unhandled message type " + Enum.GetName(typeof(eLeapEventType), lEAP_CONNECTION_MESSAGE.type));
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception arg)
            {
                Logger.Log("Exception: " + arg);
                this._isRunning = false;
            }
        }

        private void handleTrackingMessage(ref LEAP_TRACKING_EVENT trackingMsg)
        {
            this.Frames.Put(ref trackingMsg);
            if (this.LeapInternalFrame != null)
            {
                this.LeapInternalFrame.DispatchOnContext(this, this.EventContext, new InternalFrameEventArgs(ref trackingMsg));
            }
            if (this.LeapFrame != null)
            {
                this.LeapFrame.DispatchOnContext(this, this.EventContext, new FrameEventArgs(new Frame().CopyFrom(ref trackingMsg)));
            }
        }

        public ulong GetInterpolatedFrameSize(long time)
        {
            ulong result = 0uL;
            eLeapRS frameSize = LeapC.GetFrameSize(this._leapConnection, time, out result);
            this.reportAbnormalResults("LeapC get interpolated frame call was ", frameSize);
            return result;
        }

        public void GetInterpolatedFrame(Frame toFill, long time)
        {
            ulong interpolatedFrameSize = this.GetInterpolatedFrameSize(time);
            IntPtr intPtr = Marshal.AllocHGlobal((int)interpolatedFrameSize);
            eLeapRS eLeapRS = LeapC.InterpolateFrame(this._leapConnection, time, intPtr, interpolatedFrameSize);
            this.reportAbnormalResults("LeapC get interpolated frame call was ", eLeapRS);
            if (eLeapRS == eLeapRS.eLeapRS_Success)
            {
                LEAP_TRACKING_EVENT lEAP_TRACKING_EVENT;
                StructMarshal<LEAP_TRACKING_EVENT>.PtrToStruct(intPtr, out lEAP_TRACKING_EVENT);
                toFill.CopyFrom(ref lEAP_TRACKING_EVENT);
            }
            Marshal.FreeHGlobal(intPtr);
        }

        public Frame GetInterpolatedFrame(long time)
        {
            Frame frame = new Frame();
            this.GetInterpolatedFrame(frame, time);
            return frame;
        }

        public Image RequestImages(long frameId, Image.ImageType imageType)
        {
            ImageData imageData;
            int num;
            if (imageType == Image.ImageType.DEFAULT)
            {
                imageData = this._imageDataCache.CheckOut();
                imageData.type = eLeapImageType.eLeapImageType_Default;
                num = (int)this._standardImageBufferSize;
            }
            else
            {
                imageData = this._imageRawDataCache.CheckOut();
                imageData.type = eLeapImageType.eLeapImageType_Raw;
                num = (int)this._standardRawBufferSize;
            }
            if (imageData.pixelBuffer == null || imageData.pixelBuffer.Length != num)
            {
                imageData.pixelBuffer = new byte[num];
            }
            imageData.frame_id = frameId;
            return this.RequestImages(imageData);
        }

        public Image RequestImages(long frameId, Image.ImageType imageType, byte[] buffer)
        {
            ImageData imageData = new ImageData();
            if (imageType == Image.ImageType.DEFAULT)
            {
                imageData.type = eLeapImageType.eLeapImageType_Default;
            }
            else
            {
                imageData.type = eLeapImageType.eLeapImageType_Raw;
            }
            imageData.frame_id = frameId;
            imageData.pixelBuffer = buffer;
            return this.RequestImages(imageData);
        }

        private Image RequestImages(ImageData imageData)
        {
            Image result;
            if (!this._isRunning)
            {
                result = Image.Invalid;
            }
            else
            {
                LEAP_IMAGE_FRAME_DESCRIPTION lEAP_IMAGE_FRAME_DESCRIPTION = default(LEAP_IMAGE_FRAME_DESCRIPTION);
                lEAP_IMAGE_FRAME_DESCRIPTION.frame_id = imageData.frame_id;
                lEAP_IMAGE_FRAME_DESCRIPTION.type = imageData.type;
                lEAP_IMAGE_FRAME_DESCRIPTION.pBuffer = imageData.getPinnedHandle();
                lEAP_IMAGE_FRAME_DESCRIPTION.buffer_len = (ulong)imageData.pixelBuffer.LongLength;
                LEAP_IMAGE_FRAME_REQUEST_TOKEN token;
                eLeapRS eLeapRS = LeapC.RequestImages(this._leapConnection, ref lEAP_IMAGE_FRAME_DESCRIPTION, out token);
                if (eLeapRS == eLeapRS.eLeapRS_Success)
                {
                    imageData.isComplete = false;
                    imageData.index = (ulong)token.requestID;
                    Image image = new Image(imageData);
                    this._pendingImageRequestList.Add(new ImageFuture(image, imageData, LeapC.GetNow(), token));
                    result = image;
                }
                else
                {
                    imageData.unPinHandle();
                    this.reportAbnormalResults("LeapC Image Request call was ", eLeapRS);
                    result = Image.Invalid;
                }
            }
            return result;
        }

        private void handleImageCompletion(ref LEAP_IMAGE_COMPLETE_EVENT imageMsg)
        {
            LEAP_IMAGE_PROPERTIES lEAP_IMAGE_PROPERTIES;
            StructMarshal<LEAP_IMAGE_PROPERTIES>.PtrToStruct(imageMsg.properties, out lEAP_IMAGE_PROPERTIES);
            ImageFuture imageFuture = this._pendingImageRequestList.FindAndRemove(imageMsg.token);
            if (imageFuture != null)
            {
                if (this._currentDistortionData.Version != imageMsg.matrix_version || !this._currentDistortionData.IsValid)
                {
                    this._currentDistortionData = new DistortionData();
                    this._currentDistortionData.Version = imageMsg.matrix_version;
                    this._currentDistortionData.Width = (float)LeapC.DistortionSize;
                    this._currentDistortionData.Height = (float)LeapC.DistortionSize;
                    if (this._currentDistortionData.Data == null || (float)this._currentDistortionData.Data.Length != 2f * this._currentDistortionData.Width * this._currentDistortionData.Height * 2f)
                    {
                        this._currentDistortionData.Data = new float[(int)(2f * this._currentDistortionData.Width * this._currentDistortionData.Height * 2f)];
                    }
                    LEAP_DISTORTION_MATRIX lEAP_DISTORTION_MATRIX;
                    StructMarshal<LEAP_DISTORTION_MATRIX>.PtrToStruct(imageMsg.distortionMatrix, out lEAP_DISTORTION_MATRIX);
                    Array.Copy(lEAP_DISTORTION_MATRIX.matrix_data, this._currentDistortionData.Data, lEAP_DISTORTION_MATRIX.matrix_data.Length);
                    if (this.LeapDistortionChange != null)
                    {
                        this.LeapDistortionChange.DispatchOnContext(this, this.EventContext, new DistortionEventArgs(this._currentDistortionData));
                    }
                }
                imageFuture.imageData.CompleteImageData(lEAP_IMAGE_PROPERTIES.type, lEAP_IMAGE_PROPERTIES.format, lEAP_IMAGE_PROPERTIES.bpp, lEAP_IMAGE_PROPERTIES.width, lEAP_IMAGE_PROPERTIES.height, imageMsg.info.timestamp, imageMsg.info.frame_id, lEAP_IMAGE_PROPERTIES.x_offset, lEAP_IMAGE_PROPERTIES.y_offset, lEAP_IMAGE_PROPERTIES.x_scale, lEAP_IMAGE_PROPERTIES.y_scale, this._currentDistortionData, LeapC.DistortionSize, imageMsg.matrix_version);
                Image imageObject = imageFuture.imageObject;
                if (this.LeapImageReady != null)
                {
                    this.LeapImageReady.DispatchOnContext(this, this.EventContext, new ImageEventArgs(imageObject));
                }
            }
        }

        private void handleFailedImageRequest(ref LEAP_IMAGE_FRAME_REQUEST_ERROR_EVENT failed_image_evt)
        {
            ImageFuture imageFuture = this._pendingImageRequestList.FindAndRemove(failed_image_evt.token);
            if (imageFuture != null)
            {
                imageFuture.imageData.CheckIn();
                ImageRequestFailedEventArgs imageRequestFailedEventArgs = new ImageRequestFailedEventArgs(failed_image_evt.description.frame_id, imageFuture.imageObject.Type);
                switch (failed_image_evt.error)
                {
                    case eLeapImageRequestError.eLeapImageRequestError_ImagesDisabled:
                        imageRequestFailedEventArgs.message = "Images are disabled by the current configuration settings.";
                        imageRequestFailedEventArgs.reason = Image.RequestFailureReason.Images_Disabled;
                        break;
                    case eLeapImageRequestError.eLeapImageRequestError_Unavailable:
                        imageRequestFailedEventArgs.message = "The image was request too late and is no longer available.";
                        imageRequestFailedEventArgs.reason = Image.RequestFailureReason.Image_Unavailable;
                        break;
                    case eLeapImageRequestError.eLeapImageRequestError_InsufficientBuffer:
                        imageRequestFailedEventArgs.message = "The buffer specified for the request was too small.";
                        imageRequestFailedEventArgs.reason = Image.RequestFailureReason.Insufficient_Buffer;
                        if (failed_image_evt.description.type == eLeapImageType.eLeapImageType_Default && this._standardImageBufferSize < failed_image_evt.required_buffer_len)
                        {
                            this._standardImageBufferSize = failed_image_evt.required_buffer_len;
                        }
                        else if (failed_image_evt.description.type == eLeapImageType.eLeapImageType_Raw && this._standardRawBufferSize < failed_image_evt.required_buffer_len)
                        {
                            this._standardRawBufferSize = failed_image_evt.required_buffer_len;
                        }
                        break;
                    default:
                        imageRequestFailedEventArgs.message = "The image request failed for an undetermined reason.";
                        imageRequestFailedEventArgs.reason = Image.RequestFailureReason.Unknown_Error;
                        break;
                }
                imageRequestFailedEventArgs.requiredBufferSize = (long)failed_image_evt.required_buffer_len;
                if (this.LeapImageRequestFailed != null)
                {
                    this.LeapImageRequestFailed.DispatchOnContext(this, this.EventContext, imageRequestFailedEventArgs);
                }
            }
            this._pendingImageRequestList.purgeOld(this._leapConnection);
        }

        private void handleConnection(ref LEAP_CONNECTION_EVENT connectionMsg)
        {
            if (this._leapConnectionEvent != null)
            {
                this._leapConnectionEvent.DispatchOnContext(this, this.EventContext, new ConnectionEventArgs());
            }
        }

        private void handleConnectionLost(ref LEAP_CONNECTION_LOST_EVENT connectionMsg)
        {
            if (this.LeapConnectionLost != null)
            {
                this.LeapConnectionLost.DispatchOnContext(this, this.EventContext, new ConnectionLostEventArgs());
            }
        }

        private void handleDevice(ref LEAP_DEVICE_EVENT deviceMsg)
        {
            IntPtr handle = deviceMsg.device.handle;
            if (!(handle == IntPtr.Zero))
            {
                LEAP_DEVICE_INFO lEAP_DEVICE_INFO = default(LEAP_DEVICE_INFO);
                IntPtr hDevice;
                eLeapRS eLeapRS = LeapC.OpenDevice(deviceMsg.device, out hDevice);
                uint num = 14u;
                lEAP_DEVICE_INFO.serial_length = num;
                lEAP_DEVICE_INFO.serial = Marshal.AllocCoTaskMem((int)num);
                lEAP_DEVICE_INFO.size = (uint)Marshal.SizeOf(lEAP_DEVICE_INFO);
                eLeapRS = LeapC.GetDeviceInfo(hDevice, out lEAP_DEVICE_INFO);
                if (eLeapRS == (eLeapRS)3791716355u)
                {
                    Marshal.FreeCoTaskMem(lEAP_DEVICE_INFO.serial);
                    lEAP_DEVICE_INFO.serial = Marshal.AllocCoTaskMem((int)lEAP_DEVICE_INFO.serial_length);
                    lEAP_DEVICE_INFO.size = (uint)Marshal.SizeOf(lEAP_DEVICE_INFO);
                    eLeapRS = LeapC.GetDeviceInfo(handle, out lEAP_DEVICE_INFO);
                }
                if (eLeapRS == eLeapRS.eLeapRS_Success)
                {
                    Device device = new Device(handle, lEAP_DEVICE_INFO.h_fov, lEAP_DEVICE_INFO.v_fov, lEAP_DEVICE_INFO.range / 1000u, lEAP_DEVICE_INFO.baseline / 1000u, lEAP_DEVICE_INFO.caps == 65536u, lEAP_DEVICE_INFO.status == 1u, Marshal.PtrToStringAnsi(lEAP_DEVICE_INFO.serial));
                    Marshal.FreeCoTaskMem(lEAP_DEVICE_INFO.serial);
                    this._devices.AddOrUpdate(device);
                    if (this.LeapDevice != null)
                    {
                        this.LeapDevice.DispatchOnContext(this, this.EventContext, new DeviceEventArgs(device));
                    }
                }
            }
        }

        private void handleLostDevice(ref LEAP_DEVICE_EVENT deviceMsg)
        {
            Device device = this._devices.FindDeviceByHandle(deviceMsg.device.handle);
            if (device != null)
            {
                this._devices.Remove(device);
                if (this.LeapDeviceLost != null)
                {
                    this.LeapDeviceLost.DispatchOnContext(this, this.EventContext, new DeviceEventArgs(device));
                }
            }
        }

        private void handleFailedDevice(ref LEAP_DEVICE_FAILURE_EVENT deviceMsg)
        {
            string serial = "Unavailable";
            string message;
            switch (deviceMsg.status)
            {
                case (eLeapDeviceStatus)3892379649u:
                    message = "Bad Calibration. Device failed because of a bad calibration record.";
                    break;
                case (eLeapDeviceStatus)3892379650u:
                    message = "Bad Firmware. Device failed because of a firmware error.";
                    break;
                case (eLeapDeviceStatus)3892379651u:
                    message = "Bad Transport. Device failed because of a USB communication error.";
                    break;
                case (eLeapDeviceStatus)3892379652u:
                    message = "Bad Control Interface. Device failed because of a USB control interface error.";
                    break;
                default:
                    message = "Device failed for an unknown reason";
                    break;
            }
            Device device = this._devices.FindDeviceByHandle(deviceMsg.hDevice);
            if (device != null)
            {
                this._devices.Remove(device);
            }
            if (this.LeapDeviceFailure != null)
            {
                this.LeapDeviceFailure.DispatchOnContext(this, this.EventContext, new DeviceFailureEventArgs((uint)deviceMsg.status, message, serial));
            }
        }

        private void handleConfigChange(ref LEAP_CONFIG_CHANGE_EVENT configEvent)
        {
            string text = "";
            this._configRequests.TryGetValue(configEvent.requestId, out text);
            if (text != null)
            {
                this._configRequests.Remove(configEvent.requestId);
            }
            if (this.LeapConfigChange != null)
            {
                this.LeapConfigChange.DispatchOnContext(this, this.EventContext, new ConfigChangeEventArgs(text, configEvent.status != 0, configEvent.requestId));
            }
        }

        private void handleConfigResponse(ref LEAP_CONNECTION_MESSAGE configMsg)
        {
            LEAP_CONFIG_RESPONSE_EVENT lEAP_CONFIG_RESPONSE_EVENT;
            StructMarshal<LEAP_CONFIG_RESPONSE_EVENT>.PtrToStruct(configMsg.eventStructPtr, out lEAP_CONFIG_RESPONSE_EVENT);
            string text = "";
            this._configRequests.TryGetValue(lEAP_CONFIG_RESPONSE_EVENT.requestId, out text);
            if (text != null)
            {
                this._configRequests.Remove(lEAP_CONFIG_RESPONSE_EVENT.requestId);
            }
            uint requestId = lEAP_CONFIG_RESPONSE_EVENT.requestId;
            Config.ValueType dataType;
            object value;
            if (lEAP_CONFIG_RESPONSE_EVENT.value.type != eLeapValueType.eLeapValueType_String)
            {
                switch (lEAP_CONFIG_RESPONSE_EVENT.value.type)
                {
                    case eLeapValueType.eLeapValueType_Boolean:
                        dataType = Config.ValueType.TYPE_BOOLEAN;
                        value = lEAP_CONFIG_RESPONSE_EVENT.value.boolValue;
                        break;
                    case eLeapValueType.eLeapValueType_Int32:
                        dataType = Config.ValueType.TYPE_INT32;
                        value = lEAP_CONFIG_RESPONSE_EVENT.value.intValue;
                        break;
                    case eLeapValueType.eLeapValueType_Float:
                        dataType = Config.ValueType.TYPE_FLOAT;
                        value = lEAP_CONFIG_RESPONSE_EVENT.value.floatValue;
                        break;
                    default:
                        dataType = Config.ValueType.TYPE_UNKNOWN;
                        value = new object();
                        break;
                }
            }
            else
            {
                LEAP_CONFIG_RESPONSE_EVENT_WITH_REF_TYPE lEAP_CONFIG_RESPONSE_EVENT_WITH_REF_TYPE;
                StructMarshal<LEAP_CONFIG_RESPONSE_EVENT_WITH_REF_TYPE>.PtrToStruct(configMsg.eventStructPtr, out lEAP_CONFIG_RESPONSE_EVENT_WITH_REF_TYPE);
                dataType = Config.ValueType.TYPE_STRING;
                value = lEAP_CONFIG_RESPONSE_EVENT_WITH_REF_TYPE.value.stringValue;
            }
            SetConfigResponseEventArgs eventArgs = new SetConfigResponseEventArgs(text, dataType, value, requestId);
            if (this.LeapConfigResponse != null)
            {
                this.LeapConfigResponse.DispatchOnContext(this, this.EventContext, eventArgs);
            }
        }

        private void reportLogMessage(ref LEAP_LOG_EVENT logMsg)
        {
            if (this.LeapLogEvent != null)
            {
                this.LeapLogEvent.DispatchOnContext(this, this.EventContext, new LogEventArgs(this.publicSeverity(logMsg.severity), logMsg.timestamp, logMsg.message));
            }
        }

        private MessageSeverity publicSeverity(eLeapLogSeverity leapCSeverity)
        {
            MessageSeverity result;
            switch (leapCSeverity)
            {
                case eLeapLogSeverity.eLeapLogSeverity_Unknown:
                    result = MessageSeverity.MESSAGE_UNKNOWN;
                    break;
                case eLeapLogSeverity.eLeapLogSeverity_Critical:
                    result = MessageSeverity.MESSAGE_CRITICAL;
                    break;
                case eLeapLogSeverity.eLeapLogSeverity_Warning:
                    result = MessageSeverity.MESSAGE_WARNING;
                    break;
                case eLeapLogSeverity.eLeapLogSeverity_Information:
                    result = MessageSeverity.MESSAGE_INFORMATION;
                    break;
                default:
                    result = MessageSeverity.MESSAGE_UNKNOWN;
                    break;
            }
            return result;
        }

        private void handlePolicyChange(ref LEAP_POLICY_EVENT policyMsg)
        {
            if (this.LeapPolicyChange != null)
            {
                this.LeapPolicyChange.DispatchOnContext(this, this.EventContext, new PolicyEventArgs((ulong)policyMsg.current_policy, this._activePolicies));
            }
            this._activePolicies = (ulong)policyMsg.current_policy;
            if (this._activePolicies != this._requestedPolicies)
            {
            }
        }

        public void SetPolicy(Controller.PolicyFlag policy)
        {
            ulong num = (ulong)this.flagForPolicy(policy);
            this._requestedPolicies |= num;
            num = this._requestedPolicies;
            ulong clear = ~this._requestedPolicies;
            eLeapRS result = LeapC.SetPolicyFlags(this._leapConnection, num, clear);
            this.reportAbnormalResults("LeapC SetPolicyFlags call was ", result);
        }

        public void ClearPolicy(Controller.PolicyFlag policy)
        {
            ulong num = (ulong)this.flagForPolicy(policy);
            this._requestedPolicies &= ~num;
            eLeapRS result = LeapC.SetPolicyFlags(this._leapConnection, 0uL, num);
            this.reportAbnormalResults("LeapC SetPolicyFlags call was ", result);
        }

        private eLeapPolicyFlag flagForPolicy(Controller.PolicyFlag singlePolicy)
        {
            eLeapPolicyFlag result;
            switch (singlePolicy)
            {
                case Controller.PolicyFlag.POLICY_DEFAULT:
                    result = (eLeapPolicyFlag)0u;
                    return result;
                case Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES:
                    result = eLeapPolicyFlag.eLeapPolicyFlag_BackgroundFrames;
                    return result;
                case Controller.PolicyFlag.POLICY_OPTIMIZE_HMD:
                    result = eLeapPolicyFlag.eLeapPolicyFlag_OptimizeHMD;
                    return result;
            }
            result = (eLeapPolicyFlag)0u;
            return result;
        }

        public bool IsPolicySet(Controller.PolicyFlag policy)
        {
            ulong num = (ulong)this.flagForPolicy(policy);
            return (this._activePolicies & num) == num;
        }

        public uint GetConfigValue(string config_key)
        {
            uint num = 0u;
            eLeapRS result = LeapC.RequestConfigValue(this._leapConnection, config_key, out num);
            this.reportAbnormalResults("LeapC RequestConfigValue call was ", result);
            this._configRequests[num] = config_key;
            return num;
        }

        public uint SetConfigValue<T>(string config_key, T value) where T : IConvertible
        {
            uint num = 0u;
            Type type = value.GetType();
            eLeapRS result;
            if (type == typeof(bool))
            {
                result = LeapC.SaveConfigValue(this._leapConnection, config_key, Convert.ToBoolean(value), out num);
            }
            else if (type == typeof(int))
            {
                result = LeapC.SaveConfigValue(this._leapConnection, config_key, Convert.ToInt32(value), out num);
            }
            else if (type == typeof(float))
            {
                result = LeapC.SaveConfigValue(this._leapConnection, config_key, Convert.ToSingle(value), out num);
            }
            else
            {
                if (type != typeof(string))
                {
                    throw new ArgumentException("Only boolean, Int32, float, and string types are supported.");
                }
                result = LeapC.SaveConfigValue(this._leapConnection, config_key, Convert.ToString(value), out num);
            }
            this.reportAbnormalResults("LeapC SaveConfigValue call was ", result);
            this._configRequests[num] = config_key;
            return num;
        }

        public Vector PixelToRectilinear(Image.PerspectiveType camera, Vector pixel)
        {
            LEAP_VECTOR pixel2 = new LEAP_VECTOR(pixel);
            LEAP_VECTOR lEAP_VECTOR = LeapC.LeapPixelToRectilinear(this._leapConnection, (camera == Image.PerspectiveType.STEREO_LEFT) ? eLeapPerspectiveType.eLeapPerspectiveType_stereo_left : eLeapPerspectiveType.eLeapPerspectiveType_stereo_right, pixel2);
            return new Vector(lEAP_VECTOR.x, lEAP_VECTOR.y, lEAP_VECTOR.z);
        }

        public Vector RectilinearToPixel(Image.PerspectiveType camera, Vector ray)
        {
            LEAP_VECTOR rectilinear = new LEAP_VECTOR(ray);
            LEAP_VECTOR lEAP_VECTOR = LeapC.LeapRectilinearToPixel(this._leapConnection, (camera == Image.PerspectiveType.STEREO_LEFT) ? eLeapPerspectiveType.eLeapPerspectiveType_stereo_left : eLeapPerspectiveType.eLeapPerspectiveType_stereo_right, rectilinear);
            return new Vector(lEAP_VECTOR.x, lEAP_VECTOR.y, lEAP_VECTOR.z);
        }

        private void reportAbnormalResults(string context, eLeapRS result)
        {
            if (result != eLeapRS.eLeapRS_Success && result != this._lastResult)
            {
                string message = context + " " + result;
                if (this.LeapLogEvent != null)
                {
                    this.LeapLogEvent.DispatchOnContext(this, this.EventContext, new LogEventArgs(MessageSeverity.MESSAGE_CRITICAL, LeapC.GetNow(), message));
                }
            }
            this._lastResult = result;
        }
    }
}
