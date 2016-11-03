using System;

namespace LeapInternal
{
	public class PooledObject
	{
		public ulong poolIndex;

		public ulong age = 0uL;

		public virtual void CheckIn()
		{
			this.age = 0uL;
			this.poolIndex = 0uL;
		}
	}
}
