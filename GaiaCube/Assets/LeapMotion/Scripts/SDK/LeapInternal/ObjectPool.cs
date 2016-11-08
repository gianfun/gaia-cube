using System;

namespace LeapInternal
{
	public class ObjectPool<T> where T : PooledObject, new()
	{
		private const double _growRate = 1.5;

		private T[] pool;

		private ulong age = 0uL;

		public bool Growable
		{
			get;
			set;
		}

		public int Capacity
		{
			get
			{
				return this.pool.Length;
			}
		}

		public ObjectPool(int initialCapacity, bool growable = false)
		{
			this.pool = new T[initialCapacity];
			this.Growable = growable;
		}

		public T CheckOut()
		{
			ulong num = 18446744073709551615uL;
			uint num2 = 0u;
			bool flag = false;
			uint num3 = 0u;
			while ((ulong)num3 < (ulong)((long)this.Capacity))
			{
				if (this.pool[(int)((UIntPtr)num3)] == null || this.pool[(int)((UIntPtr)num3)].age == 0uL)
				{
					num2 = num3;
					flag = true;
					break;
				}
				if (this.pool[(int)((UIntPtr)num3)].age < num)
				{
					num = this.pool[(int)((UIntPtr)num3)].age;
					num2 = num3;
				}
				num3 += 1u;
			}
			if (!flag)
			{
				if (this.Growable)
				{
					num2 = (uint)this.pool.Length;
					this.expand();
				}
			}
			if (this.pool[(int)((UIntPtr)num2)] == null)
			{
				this.pool[(int)((UIntPtr)num2)] = Activator.CreateInstance<T>();
			}
			this.pool[(int)((UIntPtr)num2)].poolIndex = (ulong)num2;
			this.pool[(int)((UIntPtr)num2)].age = (this.age += 1uL);
			return this.pool[(int)((UIntPtr)num2)];
		}

		public T FindByPoolIndex(ulong index)
		{
			T result;
			for (int i = 0; i < this.pool.Length; i++)
			{
				T t = this.pool[i];
				if (t != null && t.poolIndex == index && t.age > 0uL)
				{
					result = t;
					return result;
				}
			}
			result = default(T);
			return result;
		}

		private void addItem(uint index)
		{
			this.pool[(int)((UIntPtr)index)] = Activator.CreateInstance<T>();
			this.pool[(int)((UIntPtr)index)].poolIndex = (ulong)index;
			this.pool[(int)((UIntPtr)index)].age = 0uL;
		}

		private void expand()
		{
			int num = (int)Math.Floor((double)this.Capacity * 1.5);
			T[] array = new T[num];
			uint num2 = 0u;
			while ((ulong)num2 < (ulong)((long)this.pool.Length))
			{
				array[(int)((UIntPtr)num2)] = this.pool[(int)((UIntPtr)num2)];
				num2 += 1u;
			}
			this.pool = array;
		}
	}
}
