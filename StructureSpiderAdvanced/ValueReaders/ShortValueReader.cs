using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class ShortValueReader : BaseValueReader
    {
        private short CompareValue;

        public ShortValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToInt16(value);
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            var comparingValue = M.ReadShort(scanAddress);
            newRezult.IsEqual = CompareValue == comparingValue;
            if (newRezult.IsEqual)
                newRezult.DisplayValue = comparingValue.ToString();

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadShort(address).ToString();
        }
    }
}