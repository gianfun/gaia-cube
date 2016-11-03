using Leap;
using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	public class ImageData : PooledObject
	{
		public bool isComplete = false;

		public byte[] pixelBuffer;

		private GCHandle _bufferHandle;

		private bool _isPinned = false;

		private object locker = new object();

		public ulong index;

		public long frame_id;

		public long timestamp;

		public eLeapImageType type;

		public eLeapImageFormat format;

		public uint bpp;

		public uint width;

		public uint height;

		public float RayOffsetX = 0.5f;

		public float RayOffsetY = 0.5f;

		public float RayScaleX = 0.125f;

		public float RayScaleY = 0.125f;

		public int DistortionSize;

		public ulong DistortionMatrixKey;

		public DistortionData DistortionData;

		public ImageData()
		{
		}

		public ImageData(ulong bufferLength, ulong index)
		{
			this.pixelBuffer = new byte[bufferLength];
			this.index = index;
		}

		public void CompleteImageData(eLeapImageType type, eLeapImageFormat format, uint bpp, uint width, uint height, long timestamp, long frame_id, float x_offset, float y_offset, float x_scale, float y_scale, DistortionData distortionData, int distortion_size, ulong distortion_matrix_version)
		{
			lock (this.locker)
			{
				this.type = type;
				this.format = format;
				this.bpp = bpp;
				this.width = width;
				this.height = height;
				this.timestamp = timestamp;
				this.frame_id = frame_id;
				this.DistortionData = distortionData;
				this.DistortionSize = distortion_size;
				this.DistortionMatrixKey = distortion_matrix_version;
				this.isComplete = true;
			}
		}

		public override void CheckIn()
		{
			base.CheckIn();
			this.unPinHandle();
			this.index = 0uL;
			this.isComplete = false;
		}

		public IntPtr getPinnedHandle()
		{
			IntPtr result;
			if (this.pixelBuffer == null)
			{
				result = IntPtr.Zero;
			}
			else
			{
				lock (this.locker)
				{
					if (!this._isPinned)
					{
						this._bufferHandle = GCHandle.Alloc(this.pixelBuffer, GCHandleType.Pinned);
						this._isPinned = true;
					}
				}
				result = this._bufferHandle.AddrOfPinnedObject();
			}
			return result;
		}

		public void unPinHandle()
		{
			lock (this.locker)
			{
				if (this._isPinned)
				{
					this._bufferHandle.Free();
					this._isPinned = false;
				}
			}
		}
	}
}
