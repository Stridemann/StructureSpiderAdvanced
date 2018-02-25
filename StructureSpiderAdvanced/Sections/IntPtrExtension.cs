//Many thanks to https://github.com/ReClassNET/ReClass.NET
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace StructureSpiderAdvanced
{
    public static class IntPtrExtension
    {

        [Pure]
        [DebuggerStepThrough]
        public static bool IsNull(this IntPtr ptr)
        {
            return ptr == IntPtr.Zero;
        }

        [Pure]
        [DebuggerStepThrough]
        public static IntPtr Add(this IntPtr lhs, IntPtr rhs)
        {
            return new IntPtr(lhs.ToInt64() + rhs.ToInt64());
        }

        [Pure]
        [DebuggerStepThrough]
        public static bool InRange(this IntPtr address, IntPtr start, IntPtr end)
        {
            var val = address.ToInt64();
            return start.ToInt64() <= val && val <= end.ToInt64();
        }

        [Pure]
        [DebuggerStepThrough]
        public static int CompareTo(this IntPtr lhs, IntPtr rhs)
        {
            return lhs.ToInt64().CompareTo(rhs.ToInt64());
        }

        [Pure]
        [DebuggerStepThrough]
        public static int CompareToRange(this IntPtr address, IntPtr start, IntPtr end)
        {
            if (InRange(address, start, end))
            {
                return 0;
            }
            return CompareTo(address, start);
        }
    }
}
