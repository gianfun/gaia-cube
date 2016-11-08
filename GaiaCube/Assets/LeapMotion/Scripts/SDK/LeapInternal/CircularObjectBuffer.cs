using System;

namespace LeapInternal
{
	public class CircularObjectBuffer<T> where T : new()
	{
		private T[] array;

		private int current = 0;

		private object locker = new object();

		public int Count
		{
			get;
			private set;
		}

		public int Capacity
		{
			get;
			private set;
		}

		public bool IsEmpty
		{
			get;
			private set;
		}

		public CircularObjectBuffer(int capacity)
		{
			this.Capacity = capacity;
			this.array = new T[this.Capacity];
			this.current = 0;
			this.Count = 0;
			this.IsEmpty = true;
		}

		public virtual void Put(ref T item)
		{
			lock (this.locker)
			{
				if (!this.IsEmpty)
				{
					this.current++;
					if (this.current >= this.Capacity)
					{
						this.current = 0;
					}
				}
				if (this.Count < this.Capacity)
				{
					this.Count++;
				}
				lock (this.array)
				{
					this.array[this.current] = item;
				}
				this.IsEmpty = false;
			}
		}

		public void Get(out T t, int index = 0)
		{
			lock (this.locker)
			{
				if (this.IsEmpty || index > this.Count - 1 || index < 0)
				{
					t = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
				}
				else
				{
					int num = this.current - index;
					if (num < 0)
					{
						num += this.Capacity;
					}
					t = this.array[num];
				}
			}
		}

		public void Resize(int newCapacity)
		{
			lock (this.locker)
			{
				if (newCapacity > this.Capacity)
				{
					T[] array = new T[newCapacity];
					int num = 0;
					for (int i = this.Count - 1; i >= 0; i--)
					{
						T t;
						this.Get(out t, i);
						array[num++] = t;
					}
					this.array = array;
					this.Capacity = newCapacity;
				}
			}
		}
	}
}
