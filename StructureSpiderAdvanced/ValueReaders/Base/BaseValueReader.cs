using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StructureSpiderAdvanced
{
    public abstract class BaseValueReader
    {
        protected readonly Memory M;
        protected readonly MainViewModel MVM;
        public BaseValueReader(Memory m, MainViewModel mvm)
        {
            M = m;
            MVM = mvm;
        }

        public IntPtr LastReadPointer { get; private set; }
        public bool HasReadLastPointer { get; private set; }
        public bool CheckPointer(IntPtr scanAddress)
        {
            LastReadPointer = M.ReadPointer(scanAddress);
            HasReadLastPointer = M.IsSimplePointer(LastReadPointer);

            if (!HasReadLastPointer) return false;

            if (MVM.UseMethodTable)
                HasReadLastPointer = CheckMethodTable(LastReadPointer);

            return HasReadLastPointer;
        }

        public abstract void SetCompareValue(string value);

        // Check if it is a vtable. Check if the first 3 values are pointers to a code section.
        private bool CheckMethodTable(IntPtr startAddr, int count = 3)
        {
            try
            {
                var pointerToVMT = M.ReadPointer(startAddr);
                var pointerToVMTType = M.CheckPointer(pointerToVMT);

                if (!MVM.UseMemoryPage && pointerToVMTType == SectionCategory.CODE)
                    pointerToVMTType = SectionCategory.DATA;

                if (pointerToVMTType != SectionCategory.DATA)
                {
                    return false;
                }

                var pLen = M.PointerLength;
                for (int i = 0; i < count; i++)
                {
                    var checkPointer = M.ReadPointer(pointerToVMT);
                    if (M.CheckPointer(checkPointer) != SectionCategory.CODE)
                    {
                        return false;
                    }

                    pointerToVMT += pLen;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public abstract ValueReadCompareResult ReadCompareValue(IntPtr scanAddress);

        public bool CheckTableRezult(VisibleResult rezult, IntPtr baseAddress, string compareString, RezultRefreshType refreshType)
        {
            var processingPointer = baseAddress;
            foreach (var offset in rezult.Offsets)
            {
                if (offset == rezult.Offsets.Last())
                {
                    processingPointer += offset;
                    break;
                }

                processingPointer = M.ReadPointer(processingPointer + offset);
                if (M.CheckPointer(processingPointer) != SectionCategory.HEAP)
                {
                    if (refreshType == RezultRefreshType.DeleteBroken || refreshType == RezultRefreshType.FilterValues)
                    {
                        return false;
                    }
                    else if (refreshType == RezultRefreshType.RefreshValues)
                    {
                        rezult.Value = "-";
                        rezult.Address = "-";
                        return true;
                    }
                    else
                    {
                        ThrowRefreshTypeException(refreshType);
                    }
                }
            }

            if (refreshType == RezultRefreshType.DeleteBroken) return true;
            var readDisplayValue = ReadDisplayString(processingPointer);

            if(refreshType == RezultRefreshType.FilterValues)
            {
                if (!compareString.Equals(readDisplayValue))
                    return false;
            }
            else if(refreshType == RezultRefreshType.RefreshValues)
            {
                rezult.Value = readDisplayValue;
                rezult.Address = processingPointer.ToString("x");      
            }
            else
            {
                ThrowRefreshTypeException(refreshType);
            }
            return true;
        }
        
        public abstract string ReadDisplayString(IntPtr address);

        public virtual string ConvertCompareValue(string compareValue)
        {
            return compareValue;
        }

        public static void ThrowRefreshTypeException(RezultRefreshType refreshType)
        {
            throw new NotImplementedException("Result refresh type is not implemented in code: " + refreshType);
        }

    }

    public struct ValueReadCompareResult
    {
        public bool IsEqual { get; set; }
        public string DisplayValue { get; set; }
    }
}
