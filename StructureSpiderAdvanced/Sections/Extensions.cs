//Many thanks to https://github.com/ReClassNET/ReClass.NET
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;

namespace StructureSpiderAdvanced
{
    public static class Extensions
    {
        [DebuggerStepThrough]
        public static void FillWithZero(this byte[] b)
        {
            Contract.Requires(b != null);

            for (var i = 0; i < b.Length; ++i)
            {
                b[i] = 0;
            }
        }

        #region List

        [DebuggerStepThrough]
        public static T BinaryFind<T>(this IList<T> source, Func<T, int> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);

            var lo = 0;
            var hi = source.Count - 1;

            while (lo <= hi)
            {
                var median = lo + (hi - lo >> 1);
                var num = comparer(source[median]);
                if (num == 0)
                {
                    return source[median];
                }
                if (num > 0)
                {
                    lo = median + 1;
                }
                else
                {
                    hi = median - 1;
                }
            }

            return default(T);
        }

        #endregion
    }
}
