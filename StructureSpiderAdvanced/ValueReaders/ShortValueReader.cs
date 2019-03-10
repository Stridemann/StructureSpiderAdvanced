using System;
using StructureSpiderAdvanced.ValueReaders.Base;

namespace StructureSpiderAdvanced.ValueReaders
{
    public class ShortValueReader : BaseValueReader
    {
        private short CompareValue;

        public ShortValueReader(Memory m, MainViewModel mvm) : base(m, mvm)
        {
        }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToInt16(value);
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            var compareValue = M.ReadShort(scanAddress);

            //newRezult.IsSatisfying = CompareValue == comparingValue;
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
            return M.ReadShort(address).ToString();
        }

        public override IComparable ReadComparable(IntPtr address)
        {
            return M.ReadShort(address);
        }

        public override IComparable ConvertToComparableValue(string compareValue)
        {
            return Convert.ToInt16(compareValue);
        }
    }
}
