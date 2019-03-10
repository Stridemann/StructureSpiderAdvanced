using System;
using StructureSpiderAdvanced.ValueReaders.Base;

namespace StructureSpiderAdvanced.ValueReaders
{
    public class LongValueReader : BaseValueReader
    {
        private bool CanUsePointerValue;
        private long CompareValue;

        public LongValueReader(Memory m, MainViewModel mvm) : base(m, mvm)
        {
        }

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

            //newRezult.IsSatisfying = Math.Abs(CompareValue - compareValue) < float.Epsilon;
            newRezult.IsSatisfying = CheckSatisfies(CompareValue, compareValue);

            if (newRezult.IsSatisfying)
            {
                newRezult.DisplayValue = LastReadPointer.ToString();
                newRezult.ComparableValue = compareValue;
            }

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadLong(address).ToString();
        }

        public override IComparable ReadComparable(IntPtr address)
        {
            return M.ReadLong(address);
        }

        public override IComparable ConvertToComparableValue(string compareValue)
        {
            return Convert.ToInt64(compareValue);
        }
    }
}
