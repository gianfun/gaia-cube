using Leap;
using System;

namespace LeapInternal
{
	public class ImageFuture
	{
		public LEAP_IMAGE_FRAME_REQUEST_TOKEN Token;

		public Image imageObject
		{
			get;
			set;
		}

		public ImageData imageData
		{
			get;
			set;
		}

		public long Timestamp
		{
			get;
			set;
		}

		public ImageFuture(Image image, ImageData data, long timestamp, LEAP_IMAGE_FRAME_REQUEST_TOKEN token)
		{
			this.imageObject = image;
			this.imageData = data;
			this.Timestamp = timestamp;
			this.Token = token;
		}
	}
}
