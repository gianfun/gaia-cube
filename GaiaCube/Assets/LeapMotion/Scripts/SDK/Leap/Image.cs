using LeapInternal;
using System;

namespace Leap
{
	public class Image : IDisposable
	{
		public enum FormatType
		{
			INFRARED,
			IBRG
		}

		public enum PerspectiveType
		{
			INVALID,
			STEREO_LEFT,
			STEREO_RIGHT,
			MONO
		}

		public enum ImageType
		{
			DEFAULT,
			RAW
		}

		public enum RequestFailureReason
		{
			Image_Unavailable,
			Images_Disabled,
			Insufficient_Buffer,
			Unknown_Error
		}

		private ImageData imageData;

		private ulong referenceIndex = 0uL;

		private bool _disposed = false;

		public bool IsComplete
		{
			get
			{
				return this.imageData != null && this.imageData.isComplete;
			}
		}

		public byte[] Data
		{
			get
			{
				byte[] result;
				if (this.IsValid && this.imageData.isComplete)
				{
					result = this.imageData.pixelBuffer;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public float[] Distortion
		{
			get
			{
				float[] result;
				if (this.IsValid && this.imageData.isComplete)
				{
					result = this.imageData.DistortionData.Data;
				}
				else
				{
					result = new float[0];
				}
				return result;
			}
		}

		public long SequenceId
		{
			get
			{
				long result;
				if (this.IsValid)
				{
					result = this.imageData.frame_id;
				}
				else
				{
					result = -1L;
				}
				return result;
			}
		}

		public int Width
		{
			get
			{
				int result;
				if (this.IsValid)
				{
					result = (int)this.imageData.width;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}

		public int Height
		{
			get
			{
				int result;
				if (this.IsValid)
				{
					result = (int)this.imageData.height;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}

		public int BytesPerPixel
		{
			get
			{
				int result;
				if (this.IsValid)
				{
					result = (int)this.imageData.bpp;
				}
				else
				{
					result = 1;
				}
				return result;
			}
		}

		public Image.FormatType Format
		{
			get
			{
				Image.FormatType result;
				if (this.IsValid)
				{
					eLeapImageFormat format = this.imageData.format;
					if (format != eLeapImageFormat.eLeapImageType_IR)
					{
						if (format != eLeapImageFormat.eLeapImageType_RGBIr_Bayer)
						{
							result = Image.FormatType.INFRARED;
						}
						else
						{
							result = Image.FormatType.IBRG;
						}
					}
					else
					{
						result = Image.FormatType.INFRARED;
					}
				}
				else
				{
					result = Image.FormatType.INFRARED;
				}
				return result;
			}
		}

		public Image.ImageType Type
		{
			get
			{
				Image.ImageType result;
				if (this.IsValid)
				{
					switch (this.imageData.type)
					{
					case eLeapImageType.eLeapImageType_Default:
						result = Image.ImageType.DEFAULT;
						break;
					case eLeapImageType.eLeapImageType_Raw:
						result = Image.ImageType.RAW;
						break;
					default:
						result = Image.ImageType.DEFAULT;
						break;
					}
				}
				else
				{
					result = Image.ImageType.DEFAULT;
				}
				return result;
			}
		}

		public int DistortionWidth
		{
			get
			{
				int result;
				if (this.IsValid && this.imageData.isComplete)
				{
					result = this.imageData.DistortionSize * 2;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}

		public int DistortionHeight
		{
			get
			{
				int result;
				if (this.IsValid && this.imageData.isComplete)
				{
					result = this.imageData.DistortionSize;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}

		public float RayOffsetX
		{
			get
			{
				float result;
				if (this.IsValid && this.imageData.isComplete)
				{
					result = this.imageData.RayOffsetX;
				}
				else
				{
					result = 0f;
				}
				return result;
			}
		}

		public float RayOffsetY
		{
			get
			{
				float result;
				if (this.IsValid && this.imageData.isComplete)
				{
					result = this.imageData.RayOffsetY;
				}
				else
				{
					result = 0f;
				}
				return result;
			}
		}

		public float RayScaleX
		{
			get
			{
				float result;
				if (this.IsValid && this.imageData.isComplete)
				{
					result = this.imageData.RayScaleX;
				}
				else
				{
					result = 0f;
				}
				return result;
			}
		}

		public float RayScaleY
		{
			get
			{
				float result;
				if (this.IsValid && this.imageData.isComplete)
				{
					result = this.imageData.RayScaleY;
				}
				else
				{
					result = 0f;
				}
				return result;
			}
		}

		public long Timestamp
		{
			get
			{
				long result;
				if (this.IsValid && this.imageData.isComplete)
				{
					result = this.imageData.timestamp;
				}
				else
				{
					result = 0L;
				}
				return result;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.imageData != null && this.referenceIndex == this.imageData.index;
			}
		}

		public static Image Invalid
		{
			get
			{
				return new Image();
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
					this.imageData.CheckIn();
				}
				this._disposed = true;
			}
		}

		public Image(ImageData data)
		{
			this.imageData = data;
			this.referenceIndex = data.index;
		}

		~Image()
		{
			this.Dispose(false);
		}

		public void DataWithArg(byte[] dst)
		{
			if (this.IsValid && this.imageData.isComplete)
			{
				Buffer.BlockCopy(this.Data, 0, dst, 0, this.Data.Length);
			}
		}

		public void DistortionWithArg(float[] dst)
		{
			if (this.IsValid && this.imageData.isComplete)
			{
				Buffer.BlockCopy(this.Distortion, 0, dst, 0, this.Distortion.Length);
			}
		}

		public Image()
		{
		}

		public Vector PixelToRectilinear(Image.PerspectiveType camera, Vector pixel)
		{
			Vector result;
			if (this.IsValid && this.imageData.isComplete)
			{
				Connection connection = Connection.GetConnection(0);
				result = connection.PixelToRectilinear(camera, pixel);
			}
			else
			{
				result = Vector.Zero;
			}
			return result;
		}

		public Vector RectilinearToPixel(Image.PerspectiveType camera, Vector ray)
		{
			Vector result;
			if (this.IsValid && this.imageData.isComplete)
			{
				Connection connection = Connection.GetConnection(0);
				result = connection.RectilinearToPixel(camera, ray);
			}
			else
			{
				result = Vector.Zero;
			}
			return result;
		}

		public bool Equals(Image other)
		{
			return this.IsValid && other.IsValid && this.SequenceId == other.SequenceId && this.Type == other.Type && this.Timestamp == other.Timestamp;
		}

		public override string ToString()
		{
			string result;
			if (this.IsValid && this.imageData.isComplete)
			{
				result = string.Concat(new object[]
				{
					"Image sequence",
					this.SequenceId,
					", format: ",
					this.Format,
					", type: ",
					this.Type
				});
			}
			else if (this.IsValid)
			{
				result = string.Concat(new object[]
				{
					"Incomplete image sequence",
					this.SequenceId,
					", format: ",
					this.Format,
					", type: ",
					this.Type
				});
			}
			else
			{
				result = "Invalid Image";
			}
			return result;
		}
	}
}
