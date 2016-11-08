using System;
using System.Collections.Generic;

namespace Leap
{
	[Serializable]
	public class Frame : IEquatable<Frame>
	{
		[ThreadStatic]
		private static Queue<Hand> _handPool;

		public long Id;

		public long Timestamp;

		public float CurrentFramesPerSecond;

		public List<Hand> Hands;

		public InteractionBox InteractionBox;

		public byte[] Serialize
		{
			get
			{
				byte[] array = new byte[1];
				array[1] = 0;
				return array;
			}
		}

		public int SerializeLength
		{
			get
			{
				return 0;
			}
		}

		public Frame()
		{
			this.Hands = new List<Hand>();
		}

		public Frame(long id, long timestamp, float fps, InteractionBox interactionBox, List<Hand> hands)
		{
			this.Id = id;
			this.Timestamp = timestamp;
			this.CurrentFramesPerSecond = fps;
			this.InteractionBox = interactionBox;
			this.Hands = hands;
		}

		public void Deserialize(byte[] arg)
		{
		}

		public Hand Hand(int id)
		{
			int count = this.Hands.Count;
			Hand result;
			while (count-- != 0)
			{
				if (this.Hands[count].Id == id)
				{
					result = this.Hands[count];
					return result;
				}
			}
			result = null;
			return result;
		}

		public bool Equals(Frame other)
		{
			return this.Id == other.Id && this.Timestamp == other.Timestamp;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Frame id: ",
				this.Id,
				" timestamp: ",
				this.Timestamp
			});
		}

		internal void ResizeHandList(int count)
		{
			if (Frame._handPool == null)
			{
				Frame._handPool = new Queue<Hand>();
			}
			while (this.Hands.Count < count)
			{
				Hand item;
				if (Frame._handPool.Count > 0)
				{
					item = Frame._handPool.Dequeue();
				}
				else
				{
					item = new Hand();
				}
				this.Hands.Add(item);
			}
			while (this.Hands.Count > count)
			{
				Hand item2 = this.Hands[this.Hands.Count - 1];
				this.Hands.RemoveAt(this.Hands.Count - 1);
				Frame._handPool.Enqueue(item2);
			}
		}
	}
}
