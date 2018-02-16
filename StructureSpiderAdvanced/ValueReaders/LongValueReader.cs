using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class LongValueReader : BaseValueReader
    {
        private long CompareValue;
        private bool CanUsePointerValue;

        public LongValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToInt64(value);
            CanUsePointerValue = M.PointerLength == 8;
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            long compareValue;
            if (HasReadLastPointer && CanUsePointerValue)
                compareValue = LastReadPointer.ToInt64();
            else
                compareValue = M.ReadLong(scanAddress);

            newRezult.IsEqual = CompareValue.Equals(compareValue);
            if (newRezult.IsEqual)
                newRezult.DisplayValue = LastReadPointer.ToString();

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadLong(address).ToString();
        }
    }
}