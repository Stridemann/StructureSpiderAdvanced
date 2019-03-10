using System;

namespace StructureSpiderAdvanced.ValueReaders.Base
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

        public bool CheckTableRezult(VisibleResult rezult, IntPtr baseAddress, IComparable compareValue, RezultRefreshType refreshType)
        {
            var processingPointer = baseAddress;

            for (var i = 0; i < rezult.Offsets.Count; i++)
            {
                var offset = rezult.Offsets[i];

                //if (offset == rezult.Offsets.Last())  //I nade huge nistake -_\
                if (i == rezult.Offsets.Count - 1)
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

            if (refreshType == RezultRefreshType.DeleteBroken)
            {
                return true;
            }


            if (refreshType == RezultRefreshType.FilterValues)
            {
                if (MVM.SelectedDataType == DataType.String || MVM.SelectedDataType == DataType.StringU)
                {
                    var readDisplayValue = ReadDisplayString(processingPointer);
                    var compareString = (string)compareValue;
                    if (MVM.StringIgnoreCase)
                    {
                        compareString = compareString.ToLower();
                        readDisplayValue = readDisplayValue.ToLower();
                    }

                    if (MVM.StringCompareType == StringCompareType.Contains)
                    {
                        if (!readDisplayValue.Contains(compareString))
                            return false;
                    }
                    else if (MVM.StringCompareType == StringCompareType.StartWith)
                    {
                        if (!readDisplayValue.StartsWith(compareString))
                            return false;
                    }
                    else if (MVM.StringCompareType == StringCompareType.EndsWith)
                    {
                        if (!readDisplayValue.EndsWith(compareString))
                            return false;
                    }
                    else if (MVM.StringCompareType == StringCompareType.Equal)
                    {
                        if (compareString != readDisplayValue)
                            return false;
                    }
                }
                else
                {
                    var compValue = ReadComparable(processingPointer);

                    if (!CheckSatisfies(compareValue, compValue))
                        return false;

                    var readDisplayValue = ReadDisplayString(processingPointer);
                    rezult.Value = readDisplayValue;
                    rezult.Address = processingPointer.ToString("x");
                }
            }
            else if (refreshType == RezultRefreshType.RefreshValues)
            {
                var readDisplayValue = ReadDisplayString(processingPointer);
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
        public abstract IComparable ReadComparable(IntPtr address);

        public virtual string ConvertCompareValue(string compareValue)
        {
            return compareValue;
        }

        public abstract IComparable ConvertToComparableValue(string compareValue);

        public static void ThrowRefreshTypeException(RezultRefreshType refreshType)
        {
            throw new NotImplementedException("Result refresh type is not implemented in code: " + refreshType);
        }

        public bool CheckSatisfies(IComparable value, IComparable other)
        {
            dynamic otherComp = value;
            var result = other.CompareTo(otherComp);

            switch (MVM.CompareType)
            {
                case DataCompareType.Equal:
                    return result == 0;
                case DataCompareType.NotEqual:
                    return result != 0;
                case DataCompareType.Bigger:
                    return result > 0;
                case DataCompareType.Less:
                    return result < 0;
                case DataCompareType.BiggerOrEqual:
                    return result >= 0;
                case DataCompareType.LessOrEqual:
                    return result <= 0;
                default:
                    throw new NotImplementedException($"Compare type {MVM.CompareType} is not defined n code.");
            }
        }
    }

    public struct ValueReadCompareResult
    {
        public bool IsSatisfying { get; set; }
        public string DisplayValue { get; set; }
        public IComparable ComparableValue { get; set; }
    }
}
