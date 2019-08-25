using System;
using StructureSpiderAdvanced.ValueReaders.Base;

namespace StructureSpiderAdvanced.ValueReaders
{
    public class IntValueReader : BaseValueReader
    {
        private bool CanUsePointerValue;
        private int CompareValue;

        public IntValueReader(Memory m, MainViewModel mvm) : base(m, mvm)
        {
        }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToInt32(value);
            CanUsePointerValue = M.PointerLength == 4;
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            int compareValue;

            if (HasReadLastPointer && CanUsePointerValue)
                compareValue = (int)LastReadPointer.ToInt64();
            else
                compareValue = M.ReadInt(scanAddress);

            //newRezult.IsSatisfying = CompareValue == compareValue;
            newRezult.IsSatisfying = CheckSatisfies(CompareValue, compareValue);

            if (newRezult.IsSatisfying)
            {
                newRezult.DisplayValue = compareValue.ToString();
                newRezult.ComparableValue = compareValue;
            }

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadInt(address).ToString();
        }

        public override IComparable ReadComparable(IntPtr address)
        {
            return M.ReadInt(address);
        }

        public override IComparable ConvertToComparableValue(string compareValue)
        {
            return Convert.ToInt32(compareValue);
        }
    }
}
