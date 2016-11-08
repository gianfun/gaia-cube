using System;
using System.Collections.Generic;

namespace LeapInternal
{
	public class PendingImages
	{
		private List<ImageFuture> _pending = new List<ImageFuture>(20);

		private uint _pendingTimeLimit = 90000u;

		private object _locker = new object();

		public uint pendingTimeLimit
		{
			get
			{
				return this._pendingTimeLimit;
			}
			set
			{
				this._pendingTimeLimit = value;
			}
		}

		public void Add(ImageFuture pendingImage)
		{
			lock (this._locker)
			{
				this._pending.Add(pendingImage);
			}
		}

		public ImageFuture FindAndRemove(LEAP_IMAGE_FRAME_REQUEST_TOKEN token)
		{
			ImageFuture result;
			lock (this._locker)
			{
				for (int i = 0; i < this._pending.Count; i++)
				{
					ImageFuture imageFuture = this._pending[i];
					if (imageFuture.Token.requestID == token.requestID)
					{
						this._pending.RemoveAt(i);
						result = imageFuture;
						return result;
					}
				}
			}
			result = null;
			return result;
		}

		public int purgeOld(IntPtr connection)
		{
			long now = LeapC.GetNow();
			int num = 0;
			lock (this._locker)
			{
				for (int i = this._pending.Count - 1; i >= 0; i--)
				{
					ImageFuture imageFuture = this._pending[i];
					if (now - imageFuture.Timestamp > (long)((ulong)this.pendingTimeLimit))
					{
						this._pending.RemoveAt(i);
						LeapC.CancelImageFrameRequest(connection, imageFuture.Token);
						num++;
					}
				}
			}
			return num;
		}

		public int purgeAll(IntPtr connection)
		{
			int result = 0;
			lock (this._locker)
			{
				result = this._pending.Count;
				for (int i = 0; i < this._pending.Count; i++)
				{
					LeapC.CancelImageFrameRequest(connection, this._pending[i].Token);
				}
				this._pending.Clear();
			}
			return result;
		}
	}
}
