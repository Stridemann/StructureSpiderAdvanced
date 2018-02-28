using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class PointerValueReader : BaseValueReader
    {
        private IntPtr CompareValue;
        public PointerValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

        public override void SetCompareValue(string value)
        {
            var compareValue = Convert.ToInt64(value, 16);
            CompareValue = new IntPtr(compareValue);
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            if (!HasReadLastPointer) return newRezult;

            newRezult.IsEqual = CompareValue == LastReadPointer;
            if (newRezult.IsEqual)
                newRezult.DisplayValue = LastReadPointer.ToString("x");

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
    }
}