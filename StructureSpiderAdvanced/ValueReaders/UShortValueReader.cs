using System;
using StructureSpiderAdvanced.ValueReaders.Base;

namespace StructureSpiderAdvanced.ValueReaders
{
    public class UShortValueReader : BaseValueReader
    {
        private ushort CompareValue;

        public UShortValueReader(Memory m, MainViewModel mvm) : base(m, mvm)
        {
        }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToUInt16(value);
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            var compareValue = M.ReadUShort(scanAddress);

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
            return M.ReadUShort(address).ToString();
        }

        public override IComparable ReadComparable(IntPtr address)
        {
            return M.ReadUShort(address);
        }

        public override IComparable ConvertToComparableValue(string compareValue)
        {
            return Convert.ToUInt16(compareValue);
        }
    }
}
