using System;
using StructureSpiderAdvanced.ValueReaders.Base;

namespace StructureSpiderAdvanced.ValueReaders
{
    public class FloatValueReader : BaseValueReader
    {
        private bool CanUsePointerValue;
        private float CompareValue;

        public FloatValueReader(Memory m, MainViewModel mvm) : base(m, mvm)
        {
        }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToSingle(value);
            CanUsePointerValue = M.PointerLength == 4;
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            float compareValue;

            if (HasReadLastPointer && CanUsePointerValue)
                compareValue = Int32ToSingle(LastReadPointer.ToInt32());
            else
                compareValue = M.ReadFloat(scanAddress);

            //newRezult.IsSatisfying = Math.Abs(CompareValue - comparingValue) < float.Epsilon;
            newRezult.IsSatisfying = CheckSatisfies(CompareValue, compareValue);

            if (newRezult.IsSatisfying)
            {
                newRezult.DisplayValue = compareValue.ToString();
                newRezult.ComparableValue = compareValue;
            }

            return newRezult;
        }

        public static unsafe float Int32ToSingle(int value)
        {
            return *(float*) &value;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadFloat(address).ToString();
        }

        public override IComparable ReadComparable(IntPtr address)
        {
            return M.ReadFloat(address);
        }

        public override IComparable ConvertToComparableValue(string compareValue)
        {
            return Convert.ToSingle(compareValue);
        }
    }
}
