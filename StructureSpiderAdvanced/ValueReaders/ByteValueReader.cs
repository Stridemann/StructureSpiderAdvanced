using System;
using StructureSpiderAdvanced.ValueReaders.Base;

namespace StructureSpiderAdvanced.ValueReaders
{
    public class ByteValueReader : BaseValueReader
    {
        private byte CompareValue;

        public ByteValueReader(Memory m, MainViewModel mvm) : base(m, mvm)
        {
        }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToByte(value);
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();
            var compareValue = M.ReadByte(scanAddress);

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
            return M.ReadByte(address).ToString();
        }

        public override IComparable ReadComparable(IntPtr address)
        {
            return M.ReadByte(address);
        }

        public override IComparable ConvertToComparableValue(string compareValue)
        {
            return Convert.ToByte(compareValue);
        }
    }
}
