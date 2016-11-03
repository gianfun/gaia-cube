using System;

namespace LeapInternal
{
	public enum eLeapDeviceType : uint
	{
		eLeapDeviceType_Peripheral = 3u,
		eLeapDeviceType_Legacy = 4097u,
		eLeapDeviceType_Hops,
		eLeapDeviceType_Pongo,
		eLeapDeviceType_Dragonfly = 4354u,
		eLeapDeviceType_Nightcrawler = 4609u
	}
}
