using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class UShortValueReader : BaseValueReader
    {
        private ushort CompareValue;

        public UShortValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToUInt16(value);
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            var comparingValue = M.ReadUShort(scanAddress);
            newRezult.IsEqual = CompareValue == comparingValue;
            if (newRezult.IsEqual)
                newRezult.DisplayValue = comparingValue.ToString();

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadUShort(address).ToString();
        }
    }
}