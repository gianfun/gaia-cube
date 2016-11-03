using System;

namespace LeapInternal
{
	public enum eLeapPolicyFlag : uint
	{
		eLeapPolicyFlag_BackgroundFrames = 1u,
		eLeapPolicyFlag_OptimizeHMD = 4u,
		eLeapPolicyFlag_AllowPauseResume = 8u,
		eLeapPolicyFlag_IncludeAllFrames = 32768u,
		eLeapPolicyFlag_NonExclusive = 8388608u
	}
}
