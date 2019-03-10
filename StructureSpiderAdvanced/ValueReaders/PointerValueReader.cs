using System;
using StructureSpiderAdvanced.ValueReaders.Base;

namespace StructureSpiderAdvanced.ValueReaders
{
    public class PointerValueReader : BaseValueReader
    {
        private IntPtr CompareValue;

        public PointerValueReader(Memory m, MainViewModel mvm) : base(m, mvm)
        {
        }

        public override void SetCompareValue(string value)
        {
            var compareValue = Convert.ToInt64(value, 16);
            CompareValue = new IntPtr(compareValue);
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            if (!HasReadLastPointer) return newRezult;

            //newRezult.IsSatisfying = CompareValue == LastReadPointer;
            newRezult.IsSatisfying = CheckSatisfies((long) CompareValue, (long) LastReadPointer);

            if (newRezult.IsSatisfying)
            {
                newRezult.DisplayValue = LastReadPointer.ToString("x");
                newRezult.ComparableValue = (long)LastReadPointer;
            }

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadPointer(address).ToString("x");
        }

        public override string ConvertCompareValue(string compareValue)
        {
            var convValue = Convert.ToInt64(compareValue, 16);
            return new IntPtr(convValue).ToString("x");
        }

        public override IComparable ReadComparable(IntPtr address)
        {
            if (M.Is64Bit)
                return M.ReadLong(address);

            return (long) M.ReadUInt(address);
        }

        public override IComparable ConvertToComparableValue(string compareValue)
        {
            return Convert.ToInt64(compareValue);
        }
    }
}
