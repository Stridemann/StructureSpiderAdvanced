using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class UIntValueReader : BaseValueReader
    {
        private uint CompareValue;
        private bool CanUsePointerValue;

        public UIntValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

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
                compareValue = (uint)LastReadPointer.ToInt32();
            else
                compareValue = M.ReadUInt(scanAddress);

            newRezult.IsEqual = CompareValue.Equals(compareValue);
            if (newRezult.IsEqual)
                newRezult.DisplayValue = compareValue.ToString();

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadUInt(address).ToString();
        }
    }
}