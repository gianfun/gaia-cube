using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	public class LeapC
	{
		public static int DistortionSize = 64;

		private LeapC()
		{
		}

        /*
		[DllImport("LeapC", EntryPoint = "LeapGetNow")]
		public static extern long GetNow();
        */
        public static long GetNow()
        {
            return 0;
        }


        /*
		[DllImport("LeapC", EntryPoint = "LeapCreateClockRebaser")]
		public static extern eLeapRS CreateClockRebaser(out IntPtr phClockRebaser);
        */
        //[DllImport("LeapC", EntryPoint = "LeapCreateClockRebaser")]
        public static eLeapRS CreateClockRebaser(out IntPtr phClockRebaser)
        {
            phClockRebaser = IntPtr.Zero;
            return eLeapRS.eLeapRS_Success;
        }

        /*
        [DllImport("LeapC", EntryPoint = "LeapDestroyClockRebaser")]
		public static extern eLeapRS DestroyClockRebaser(IntPtr hClockRebaser);
        */
        public static eLeapRS DestroyClockRebaser(IntPtr hClockRebaser)
        {
            return eLeapRS.eLeapRS_Success;
        }

        /*
        [DllImport("LeapC", EntryPoint = "LeapUpdateRebase")]
		public static extern eLeapRS UpdateRebase(IntPtr hClockRebaser, long userClock, long leapClock);
        */
        public static eLeapRS UpdateRebase(IntPtr hClockRebaser, long userClock, long leapClock)
        {
            return eLeapRS.eLeapRS_Success;
        }

        /*
        [DllImport("LeapC", EntryPoint = "LeapRebaseClock")]
		public static extern eLeapRS RebaseClock(IntPtr hClockRebaser, long userClock, out long leapClock);
        */
        public static eLeapRS RebaseClock(IntPtr hClockRebaser, long userClock, out long leapClock)
        {
            leapClock = userClock;
            return eLeapRS.eLeapRS_Success;
        }

        /*
        [DllImport("LeapC", EntryPoint = "LeapCreateConnection")]
		public static extern eLeapRS CreateConnection(ref LEAP_CONNECTION_CONFIG pConfig, out IntPtr pConnection);
        */
        public static eLeapRS CreateConnection(ref LEAP_CONNECTION_CONFIG pConfig, out IntPtr pConnection)
        {
            pConnection = IntPtr.Zero;
            return eLeapRS.eLeapRS_Success;
        }

        /*
		[DllImport("LeapC", EntryPoint = "LeapCreateConnection")]
		private static extern eLeapRS CreateConnection(IntPtr nulled, out IntPtr pConnection);
        */
		private static eLeapRS CreateConnection(IntPtr nulled, out IntPtr pConnection){
            pConnection = IntPtr.Zero;
            return eLeapRS.eLeapRS_Success;
        }

        public static eLeapRS CreateConnection(out IntPtr pConnection)
		{
			return LeapC.CreateConnection(IntPtr.Zero, out pConnection);
		}

        /*
		[DllImport("LeapC", EntryPoint = "LeapGetConnectionInfo")]
		public static extern eLeapRS GetConnectionInfo(IntPtr hConnection, out LEAP_CONNECTION_INFO pInfo);
        */
        public static eLeapRS GetConnectionInfo(IntPtr hConnection, out LEAP_CONNECTION_INFO pInfo)
        {
            pInfo = new LEAP_CONNECTION_INFO();
            pInfo.size = 0;
            pInfo.status = eLeapConnectionStatus.eLeapConnectionStatus_Connected;
            return eLeapRS.eLeapRS_Success;
        }


        /*
		[DllImport("LeapC", EntryPoint = "LeapOpenConnection")]
		public static extern eLeapRS OpenConnection(IntPtr hConnection);
        */
        public static eLeapRS OpenConnection(IntPtr hConnection)
        {
            return eLeapRS.eLeapRS_Success;
        }


        /*
		[DllImport("LeapC", EntryPoint = "LeapGetDeviceList")]
		public static extern eLeapRS GetDeviceList(IntPtr hConnection, [In] [Out] LEAP_DEVICE_REF[] pArray, out uint pnArray);
        */
        public static eLeapRS GetDeviceList(IntPtr hConnection, [In] [Out] LEAP_DEVICE_REF[] pArray, out uint pnArray)
        {
            pnArray = 0;
            return eLeapRS.eLeapRS_Success;
        }
        /*
		[DllImport("LeapC", EntryPoint = "LeapGetDeviceList")]
		private static extern eLeapRS GetDeviceList(IntPtr hConnection, [In] [Out] IntPtr pArray, out uint pnArray);
        */
        private static eLeapRS GetDeviceList(IntPtr hConnection, [In] [Out] IntPtr pArray, out uint pnArray)
        {
            pnArray = 0;
            return eLeapRS.eLeapRS_Success;
        }


        public static eLeapRS GetDeviceCount(IntPtr hConnection, out uint deviceCount)
		{
			return LeapC.GetDeviceList(hConnection, IntPtr.Zero, out deviceCount);
		}

		[DllImport("LeapC", EntryPoint = "LeapOpenDevice")]
		public static extern eLeapRS OpenDevice(LEAP_DEVICE_REF rDevice, out IntPtr pDevice);

		[DllImport("LeapC", CharSet = CharSet.Ansi, EntryPoint = "LeapGetDeviceInfo")]
		public static extern eLeapRS GetDeviceInfo(IntPtr hDevice, out LEAP_DEVICE_INFO info);

		[DllImport("LeapC", EntryPoint = "LeapSetPolicyFlags")]
		public static extern eLeapRS SetPolicyFlags(IntPtr hConnection, ulong set, ulong clear);

		[DllImport("LeapC", EntryPoint = "LeapSetDeviceFlags")]
		public static extern eLeapRS SetDeviceFlags(IntPtr hDevice, ulong set, ulong clear, out ulong prior);

		[DllImport("LeapC", EntryPoint = "LeapPollConnection")]
		public static extern eLeapRS PollConnection(IntPtr hConnection, uint timeout, ref LEAP_CONNECTION_MESSAGE msg);

		[DllImport("LeapC", EntryPoint = "LeapGetFrameSize")]
		public static extern eLeapRS GetFrameSize(IntPtr hConnection, long timestamp, out ulong pncbEvent);

		[DllImport("LeapC", EntryPoint = "LeapInterpolateFrame")]
		public static extern eLeapRS InterpolateFrame(IntPtr hConnection, long timestamp, IntPtr pEvent, ulong ncbEvent);

		[DllImport("LeapC", EntryPoint = "LeapRequestImages")]
		public static extern eLeapRS RequestImages(IntPtr hConnection, ref LEAP_IMAGE_FRAME_DESCRIPTION description, out LEAP_IMAGE_FRAME_REQUEST_TOKEN pToken);

		[DllImport("LeapC", EntryPoint = "LeapCancelImageFrameRequest")]
		public static extern eLeapRS CancelImageFrameRequest(IntPtr hConnection, LEAP_IMAGE_FRAME_REQUEST_TOKEN token);

		[DllImport("LeapC")]
		public static extern LEAP_VECTOR LeapPixelToRectilinear(IntPtr hConnection, eLeapPerspectiveType camera, LEAP_VECTOR pixel);

		[DllImport("LeapC")]
		public static extern LEAP_VECTOR LeapRectilinearToPixel(IntPtr hConnection, eLeapPerspectiveType camera, LEAP_VECTOR rectilinear);

		[DllImport("LeapC", EntryPoint = "LeapCloseDevice")]
		public static extern void CloseDevice(IntPtr pDevice);

		[DllImport("LeapC", EntryPoint = "LeapDestroyConnection")]
		public static extern void DestroyConnection(IntPtr connection);

		[DllImport("LeapC", EntryPoint = "LeapSaveConfigValue")]
		private static extern eLeapRS SaveConfigValue(IntPtr hConnection, string key, IntPtr value, out uint requestId);

		[DllImport("LeapC", EntryPoint = "LeapRequestConfigValue")]
		public static extern eLeapRS RequestConfigValue(IntPtr hConnection, string name, out uint request_id);

		public static eLeapRS SaveConfigValue(IntPtr hConnection, string key, bool value, out uint requestId)
		{
			return LeapC.SaveConfigWithValueType(hConnection, key, new LEAP_VARIANT_VALUE_TYPE
			{
				type = eLeapValueType.eLeapValueType_Boolean,
				boolValue = (value ? 1 : 0)
			}, out requestId);
		}

		public static eLeapRS SaveConfigValue(IntPtr hConnection, string key, int value, out uint requestId)
		{
			return LeapC.SaveConfigWithValueType(hConnection, key, new LEAP_VARIANT_VALUE_TYPE
			{
				type = eLeapValueType.eLeapValueType_Int32,
				intValue = value
			}, out requestId);
		}

		public static eLeapRS SaveConfigValue(IntPtr hConnection, string key, float value, out uint requestId)
		{
			return LeapC.SaveConfigWithValueType(hConnection, key, new LEAP_VARIANT_VALUE_TYPE
			{
				type = eLeapValueType.eLeapValueType_Float,
				floatValue = value
			}, out requestId);
		}

		public static eLeapRS SaveConfigValue(IntPtr hConnection, string key, string value, out uint requestId)
		{
			LEAP_VARIANT_REF_TYPE valueStruct;
			valueStruct.type = eLeapValueType.eLeapValueType_String;
			valueStruct.stringValue = value;
			return LeapC.SaveConfigWithRefType(hConnection, key, valueStruct, out requestId);
		}

		private static eLeapRS SaveConfigWithValueType(IntPtr hConnection, string key, LEAP_VARIANT_VALUE_TYPE valueStruct, out uint requestId)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(valueStruct));
			eLeapRS result = (eLeapRS)3791716352u;
			try
			{
				Marshal.StructureToPtr(valueStruct, intPtr, false);
				result = LeapC.SaveConfigValue(hConnection, key, intPtr, out requestId);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}

		private static eLeapRS SaveConfigWithRefType(IntPtr hConnection, string key, LEAP_VARIANT_REF_TYPE valueStruct, out uint requestId)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(valueStruct));
			eLeapRS result = (eLeapRS)3791716352u;
			try
			{
				Marshal.StructureToPtr(valueStruct, intPtr, false);
				result = LeapC.SaveConfigValue(hConnection, key, intPtr, out requestId);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}
	}
}
