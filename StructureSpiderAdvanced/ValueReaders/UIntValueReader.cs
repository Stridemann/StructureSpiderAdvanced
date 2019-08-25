using System;
using StructureSpiderAdvanced.ValueReaders.Base;

namespace StructureSpiderAdvanced.ValueReaders
{
    public class UIntValueReader : BaseValueReader
    {
        private uint CompareValue;
        private bool CanUsePointerValue;

        public UIntValueReader(Memory m, MainViewModel mvm) : base(m, mvm)
        {
        }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToUInt32(value);
            CanUsePointerValue = M.PointerLength == 4;
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            uint compareValue;

            if (HasReadLastPointer && CanUsePointerValue)
                compareValue = (uint) LastReadPointer.ToInt64();
            else
                compareValue = M.ReadUInt(scanAddress);

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
            return M.ReadUInt(address).ToString();
        }

        public override IComparable ReadComparable(IntPtr address)
        {
            return M.ReadUInt(address);
        }

        public override IComparable ConvertToComparableValue(string compareValue)
        {
            return Convert.ToUInt32(compareValue);
        }
    }
}
