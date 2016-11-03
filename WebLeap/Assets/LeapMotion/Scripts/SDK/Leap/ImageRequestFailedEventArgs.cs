using System;

namespace Leap
{
	public class ImageRequestFailedEventArgs : LeapEventArgs
	{
		public long frameId
		{
			get;
			set;
		}

		public Image.ImageType imageType
		{
			get;
			set;
		}

		public Image.RequestFailureReason reason
		{
			get;
			set;
		}

		public string message
		{
			get;
			set;
		}

		public long requiredBufferSize
		{
			get;
			set;
		}

		public ImageRequestFailedEventArgs(long frameId, Image.ImageType imageType) : base(LeapEvent.EVENT_IMAGE_REQUEST_FAILED)
		{
			this.frameId = frameId;
			this.imageType = imageType;
		}

		public ImageRequestFailedEventArgs(long frameId, Image.ImageType imageType, Image.RequestFailureReason reason, string message, long requiredBufferSize) : base(LeapEvent.EVENT_IMAGE_REQUEST_FAILED)
		{
			this.frameId = frameId;
			this.imageType = imageType;
			this.reason = reason;
			this.message = message;
			this.requiredBufferSize = requiredBufferSize;
		}
	}
}
