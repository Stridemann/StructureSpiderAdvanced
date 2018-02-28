﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class ByteValueReader : BaseValueReader
    {
        private byte CompareValue;

        public ByteValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

        public override void SetCompareValue(string value)
        {
            CompareValue = Convert.ToByte(value);
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            var comparingValue = M.ReadByte(scanAddress);
            newRezult.IsEqual = CompareValue == comparingValue;
            if (newRezult.IsEqual)
                newRezult.DisplayValue = comparingValue.ToString();

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadByte(address).ToString();
        }
    }
}