using System;
using System.Collections.Generic;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;

namespace StructureSpiderAdvanced
{
    public class MainViewModel : BindableBase
    {
        public static MainViewModel Instance;
        public MainViewModel()
        {
            Instance = this;
            DataTypes.AddRange(Enum.GetValues(typeof(DataType)) as IEnumerable<DataType>);
            StringCompareTypes.AddRange(Enum.GetValues(typeof(StringCompareType)) as IEnumerable<StringCompareType>);
        }

        ///////////////////
        private bool interfaceEnabled = true;
        public bool InterfaceEnabled
        {
            get { return interfaceEnabled; }
            set
            {
                if (interfaceEnabled == value) return;
                interfaceEnabled = value;
                RaisePropertyChanged(nameof(InterfaceEnabled));
            }
        }
        ///////////////////
        private float progress;
        public float Progress
        {
            get { return progress; }
            set
            {
                if (progress == value) return;
                progress = value;
                RaisePropertyChanged(nameof(Progress));
            }
        }
        ///////////////////
        private int maxEntries;
        private int currentEntries;
        public int CurrentEntries
        {
            get { return currentEntries; }
            set
            {
                if (currentEntries == value) return;
                currentEntries = value;

                if (currentEntries == 0)
                {
                    maxEntries = 0;
                    Progress = 1;
                    currentEntries = 0;
                    RaisePropertyChanged(nameof(CurrentEntries));
                    return;
                }

                maxEntries = Math.Max(maxEntries, currentEntries);
                Progress = 1 - ((float)currentEntries / maxEntries);
                RaisePropertyChanged(nameof(CurrentEntries));
            }
        }
        ///////////////////
        private int pointersEvaluated;
        public int PointersEvaluated
        {
            get { return pointersEvaluated; }
            set
            {
                if (pointersEvaluated == value) return;
                pointersEvaluated = value;
                RaisePropertyChanged(nameof(PointersEvaluated));
            }
        }
        ///////////////////
        private int pointersFound;
        public int PointersFound
        {
            get { return pointersFound; }
            set
            {
                if (pointersFound == value) return;
                pointersFound = value;
                RaisePropertyChanged(nameof(PointersFound));
            }
        }

        ////////////////////////////////////////////////
        public ObservableCollection<VisibleResult> VisibleResults { get; set; } = new AsyncObservableCollection<VisibleResult>();
        public void AddRezultAsync(VisibleResult rezult)
        {
            MainWindow.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)(() => VisibleResults.Add(rezult)));
        }

        ////////////////////////////////////////////////
        public ObservableCollection<string> Processes { get; set; } = new ObservableCollection<string>();

        private string selectedProcessName;
        public string SelectedProcessName
        {
            get { return selectedProcessName; }
            set
            {
                selectedProcessName = value;
                RaisePropertyChanged(nameof(SelectedProcessName));
            }
        }

        public int MaxLevel { get; set; } = 1;
        public ushort MaxScanLength { get; set; } = 4000;

        private ushort alignment = 8;
        public ushort Alignment
        {
            get { return alignment; }
            set
            {
                alignment = value;
                RaisePropertyChanged(nameof(Alignment));
            }
        }

        private DataType selectedDataType = DataType.Pointer;
        public DataType SelectedDataType
        {
            get { return selectedDataType; }
            set
            {
                selectedDataType = value;
                RaisePropertyChanged(nameof(SelectedDataType));
            }
        }

        public long StartSearchAddress { get; set; } = 0x0;


        private string scanValue = "";
        public string ScanValue
        {
            get { return scanValue; }
            set
            {
                scanValue = value;
                RaisePropertyChanged(nameof(ScanValue));
            }
        }

        private bool useMethodTable = true;
        public bool UseMethodTable
        {
            get { return useMethodTable; }
            set
            {
                useMethodTable = value;
                RaisePropertyChanged(nameof(UseMethodTable));
            }
        }

        public bool NoLooping { get; set; } = true;

        public ushort MethodTableLength { get; set; } = 3;
        public ushort StringLength { get; set; } = 256;
        public StringCompareType StringCompareType { get; set; } = StringCompareType.Equal;
        public bool StringIgnoreCase { get; set; } = false;

        public bool useStartOffsets;
        public bool UseStartOffsets
        {
            get { return useStartOffsets; }
            set
            {
                useStartOffsets = value;
                RaisePropertyChanged(nameof(UseStartOffsets));
            }
        }

        public bool useEndOffsets;
        public bool UseEndOffsets
        {
            get { return useEndOffsets; }
            set
            {
                useEndOffsets = value;
                RaisePropertyChanged(nameof(UseEndOffsets));
            }
        }

        public string StartOffsets { get; set; }
        public string EndOffsets { get; set; }

        ////////////////////////////////////////////////
        public List<int> MaxLevels { get; set; } = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public List<DataType> DataTypes { get; set; } = new List<DataType>();
        public List<int> Alignments { get; set; } = new List<int>() { 1, 2, 4, 8 };
        public List<StringCompareType> StringCompareTypes { get; set; } = new List<StringCompareType>();
    }

    public enum StringCompareType
    {
        Equal,
        StartWith,
        Contains,
        EndsWith
    }

    public enum DataType
    {
        Pointer,
        Int,
        UInt,
        Byte,
        Long,
        Float,
        String, 
        StringU,
    }

    public class VisibleResult : BindableBase
    {
        public int Level { get; set; }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                if (address == value) return;
                address = value;
                RaisePropertyChanged(nameof(Address));
            }
        }

        private string value;
        public string Value
        {
            get { return value; }
            set
            {
                if (this.value == value) return;
                this.value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        public List<int> Offsets = new List<int>();

        public string Offset0 { get { return GetOffset(0); } set { } }
        public string Offset1 { get { return GetOffset(1); } set { } }
        public string Offset2 { get { return GetOffset(2); } set { } }
        public string Offset3 { get { return GetOffset(3); } set { } }
        public string Offset4 { get { return GetOffset(4); } set { } }
        public string Offset5 { get { return GetOffset(5); } set { } }
        public string Offset6 { get { return GetOffset(6); } set { } }
        public string Offset7 { get { return GetOffset(7); } set { } }
        public string Offset8 { get { return GetOffset(8); } set { } }
        public string Offset9 { get { return GetOffset(9); } set { } }

        private string GetOffset(int index)
        {
            if (index >= Offsets.Count)
                return "";
            return Offsets[index].ToString("x");
        }
    }
}