using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class FloatValueReader : BaseValueReader
    {
        private float CompareValue;
        private bool CanUsePointerValue;

        public FloatValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

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



            newRezult.IsEqual = CompareValue.Equals(compareValue);
            if (newRezult.IsEqual)
                newRezult.DisplayValue = compareValue.ToString();

            return newRezult;
        }

        public static unsafe float Int32ToSingle(int value)
        {
            return *(float*)(&value);
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadFloat(address).ToString();
        }
    }
}