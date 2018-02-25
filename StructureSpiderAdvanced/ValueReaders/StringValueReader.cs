using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class StringValueReader : BaseValueReader
    {
        private string CompareValue;

        public StringValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }
        private Func<string, string, bool> StringChecker;

        public override void SetCompareValue(string value)
        {
            CompareValue = value;

            if (MVM.StringIgnoreCase)
                CompareValue = CompareValue.ToLower();

            switch(MVM.StringCompareType)
            {
                case StringCompareType.Equal:
                    StringChecker = (string s1, string s2) => s1.Equals(s2);
                    break;
                case StringCompareType.StartWith:
                    StringChecker = (string s1, string s2) => s1.StartsWith(s2);
                    break;
                case StringCompareType.Contains:
                    StringChecker = (string s1, string s2) => s1.Contains(s2);
                    break;
                case StringCompareType.EndsWith:
                    StringChecker = (string s1, string s2) => s1.EndsWith(s2);
                    break;
                default:
                    throw new NotImplementedException("String compare type is not supported in code: " + MVM.StringCompareType);
            }
        }

        public override ValueReadCompareResult ReadCompareValue(IntPtr scanAddress)
        {
            var newRezult = new ValueReadCompareResult();

            bool trimEnd = MVM.StringCompareType != StringCompareType.StartWith && MVM.StringCompareType != StringCompareType.Contains;
            newRezult.DisplayValue = ReadString(scanAddress, trimEnd);
            var readValue = newRezult.DisplayValue;

            if (MVM.StringIgnoreCase)
                readValue = readValue.ToLower();
         
            newRezult.IsEqual = StringChecker(readValue, CompareValue);

            return newRezult;
        }

        public override string ReadDisplayString(IntPtr address)
        {
            return ReadString(address, MVM.StringCompareType != StringCompareType.StartWith);
        }

        protected virtual string ReadString(IntPtr address, bool trimEnd)
        {
            return M.ReadString(address, MVM.StringLength, trimEnd);
        }
    }
}