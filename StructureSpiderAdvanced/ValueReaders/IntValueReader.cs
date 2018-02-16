using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class IntValueReader : BaseValueReader
    {
        private int CompareValue;
        private bool CanUsePointerValue;

        public IntValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

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
                compareValue = LastReadPointer.ToInt32();
            else
                compareValue = M.ReadInt(scanAddress);

            newRezult.IsEqual = CompareValue.Equals(compareValue);
            if (newRezult.IsEqual)
                newRezult.DisplayValue = compareValue.ToString();

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadInt(address).ToString();
        }
    }
}