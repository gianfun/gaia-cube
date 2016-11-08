using System;
using System.Runtime.InteropServices;

namespace LeapInternal
{
	public static class StructMarshal<T> where T : struct
	{
		[StructLayout(LayoutKind.Sequential)]
		private class StructContainer
		{
			public T value;
		}

		[ThreadStatic]
		private static StructMarshal<T>.StructContainer _container;

		private static int _sizeofT;

		public static int Size
		{
			get
			{
				return StructMarshal<T>._sizeofT;
			}
		}

		static StructMarshal()
		{
			StructMarshal<T>._sizeofT = Marshal.SizeOf(typeof(T));
		}

		public static void CopyIntoDestination(IntPtr dstPtr, ref T t)
		{
			StructMarshal<T>.CopyIntoArray(dstPtr, ref t, 0);
		}

		public static void CopyIntoArray(IntPtr arrayPtr, ref T t, int index)
		{
			if (StructMarshal<T>._container == null)
			{
				StructMarshal<T>._container = new StructMarshal<T>.StructContainer();
			}
			StructMarshal<T>._container.value = t;
			Marshal.StructureToPtr(StructMarshal<T>._container, new IntPtr(arrayPtr.ToInt64() + (long)(StructMarshal<T>._sizeofT * index)), false);
		}

		public static void PtrToStruct(IntPtr ptr, out T t)
		{
			if (StructMarshal<T>._container == null)
			{
				StructMarshal<T>._container = new StructMarshal<T>.StructContainer();
			}
			try
			{
				Marshal.PtrToStructure(ptr, StructMarshal<T>._container);
				t = StructMarshal<T>._container.value;
			}
			catch (Exception ex)
			{
				Logger.Log(string.Concat(new object[]
				{
					"Problem converting structure ",
					typeof(T),
					" from ptr ",
					ptr,
					" : ",
					ex.Message
				}));
				t = default(T);
			}
		}

		public static void ArrayElementToStruct(IntPtr ptr, int arrayIndex, out T t)
		{
			StructMarshal<T>.PtrToStruct(new IntPtr(ptr.ToInt64() + (long)(StructMarshal<T>._sizeofT * arrayIndex)), out t);
		}
	}
}
