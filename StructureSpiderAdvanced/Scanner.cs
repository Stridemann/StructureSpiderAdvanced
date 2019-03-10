using System;
using System.Collections.Generic;
using System.Windows.Forms;
using StructureSpiderAdvanced.ValueReaders;
using StructureSpiderAdvanced.ValueReaders.Base;

namespace StructureSpiderAdvanced
{
    public class Scanner
    {
        private readonly Memory M;
        private Queue<SubClassScan> PossibleSubPointers = new Queue<SubClassScan>();
        private MainViewModel MVM;
        private BaseValueReader ValueReader;
        private HashSet<long> ProcessedPointers = new HashSet<long>();

        public Scanner(MainViewModel viewModel, Memory m)
        {
            M = m;
            MVM = viewModel;
            PossibleSubPointers.Clear();
            ProcessedPointers.Clear();
            MVM.PointersFound = 0;
            MVM.PointersEvaluated = 0;
            MVM.ValuesScanned = 0;

            var pLength = (ushort) M.PointerLength;

            if (viewModel.SelectedDataType == DataType.Pointer ||
                viewModel.SelectedDataType == DataType.String ||
                viewModel.SelectedDataType == DataType.StringU
            )
            {
                if (MVM.Alignment != pLength)
                {
                    var bits = m.Is64Bit ? "x64" : "x32";

                    var result = MessageBox.Show($"Do you really want to scan with alignment {MVM.Alignment} " +
                                                 $"(For {bits} process structure alignment is {pLength})? " + Environment.NewLine +
                                                 $"For this scan type recommended value is {pLength}" + Environment.NewLine +
                                                 $"Yes - continue using {MVM.Alignment}" + Environment.NewLine +
                                                 $"No - set to {pLength} (Recommended)", "Alighnment", MessageBoxButtons.YesNo);

                    if (result == DialogResult.No)
                        MVM.Alignment = pLength;
                }
            }

            var scansCount = MVM.MaxScanLength / MVM.Alignment;
            MVM.MaxScanLength = (ushort) (scansCount * MVM.Alignment);

            ValueReader = GetValueReaderByDataType(viewModel.SelectedDataType, M, MVM);
            ValueReader.SetCompareValue(MVM.ScanValue);

            var startPointer = new IntPtr(MVM.StartSearchAddress);
            PossibleSubPointers.Enqueue(new SubClassScan() {Address = startPointer});
            MVM.PointersFound = 1;

            while (PossibleSubPointers.Count > 0)
            {
                var process = PossibleSubPointers.Dequeue();
                MVM.PointersEvaluated++;
                MVM.CurrentEntries = PossibleSubPointers.Count;
                DoScan(process);
            }
        }

        public static BaseValueReader GetValueReaderByDataType(DataType dataType, Memory m, MainViewModel mvm)
        {
            switch (dataType)
            {
                case DataType.Pointer:
                    return new PointerValueReader(m, mvm);
                case DataType.Byte:
                    return new ByteValueReader(m, mvm);
                case DataType.Float:
                    return new FloatValueReader(m, mvm);
                case DataType.Int:
                    return new IntValueReader(m, mvm);
                case DataType.UInt:
                    return new UIntValueReader(m, mvm);
                case DataType.Short:
                    return new ShortValueReader(m, mvm);
                case DataType.UShort:
                    return new UShortValueReader(m, mvm);
                case DataType.Long:
                    return new LongValueReader(m, mvm);
                case DataType.String:
                    return new StringValueReader(m, mvm);
                case DataType.StringU:
                    return new UStringValueReader(m, mvm);
                default:
                    throw new NotImplementedException($"Scan type is not defined in code: {dataType}");
            }
        }

        private void DoScan(SubClassScan subScan)
        {
            var allowDeeper = subScan.Level < MVM.MaxLevel;
            var address = subScan.Address;

            for (int offset = 0; offset < MVM.MaxScanLength; offset += MVM.Alignment)
            {
                var scanAddress = address + offset;

                if (ValueReader.CheckPointer(scanAddress) && allowDeeper)
                {
                    var allowAdd = true;

                    if (MVM.NoLooping)
                    {
                        var longPointer = ValueReader.LastReadPointer.ToInt64();
                        allowAdd = !ProcessedPointers.Contains(longPointer);

                        if (allowAdd)
                            ProcessedPointers.Add(longPointer);
                    }

                    if (allowAdd)
                    {
                        var newPointer = new SubClassScan() {Address = ValueReader.LastReadPointer, Level = subScan.Level + 1,};

                        newPointer.Offsets = new List<int>(subScan.Offsets)
                        {
                            offset
                        };

                        PossibleSubPointers.Enqueue(newPointer);
                        MVM.CurrentEntries = PossibleSubPointers.Count;
                        MVM.PointersFound++;
                    }
                }

                var readCompareRezult = ValueReader.ReadCompareValue(scanAddress);

                if (readCompareRezult.IsSatisfying)
                {
                    var newScanResult = new VisibleResult()
                    {
                        Offsets = new List<int>(subScan.Offsets) {offset}, 
                        Address = scanAddress.ToString("x"), 
                        Level = subScan.Level, 
                        Value = readCompareRezult.DisplayValue,
                        ComparableValue = readCompareRezult.ComparableValue,
                    };
                    MVM.AddRezultAsync(newScanResult);
                }

                MVM.ValuesScanned++;
            }
        }
    }

    public class SubClassScan
    {
        public IntPtr Address;
        public int Level;
        public List<int> Offsets = new List<int>();
    }
}
