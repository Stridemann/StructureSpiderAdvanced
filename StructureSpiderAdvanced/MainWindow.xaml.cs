using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Threading;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;

namespace StructureSpiderAdvanced
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        private const string LAST_PROCESS_FILE = "LastProcess.txt";
        public MainViewModel ViewModel { get; set; } = new MainViewModel();
        private readonly DispatcherTimer DTimer = new DispatcherTimer();
        private readonly Stopwatch stopWatch = new Stopwatch();
        private ProcessManager PManager;
        private Thread SearchThread;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            LoadLastProcessFilterName();
            UpdateListOfProcesses();

            DTimer.Tick += new EventHandler(Dt_Tick);
            DTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            var currentTime = String.Format("{0:00}:{1:00}:{2:00}:{3:000}", 0, 0, 0, 0);
            ElapsedTimeLabel.Content = currentTime;

            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBox_ProcessFilter.Text))
            {
                File.WriteAllText(LAST_PROCESS_FILE, TextBox_ProcessFilter.Text);
            }
        }

        private void LoadLastProcessFilterName()
        {
            if (!File.Exists(LAST_PROCESS_FILE))
            {
                return;
            }

            var lastProcessFilter = File.ReadAllText(LAST_PROCESS_FILE);
            TextBox_ProcessFilter.Text = lastProcessFilter;
        }

        void Dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;

                var currentTime = String.Format("{0:00}:{1:00}:{2:00}:{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

                ElapsedTimeLabel.Content = currentTime;
            }
        }

        private bool InitProcessManager()
        {
            var cutIndex = ViewModel.SelectedProcessName.IndexOf(",");

            if (cutIndex == -1)
            {
                MessageBox.Show("Process is not selected");
                return false;
            }

            var processes = Process.GetProcessesByName(ViewModel.SelectedProcessName.Substring(0, cutIndex));

            if (processes.Length == 0)
            {
                MessageBox.Show("Selected process doesn't exist");
                return false;
            }

            if (PManager != null)
            {
                PManager.Dispose();
                PManager = null;
            }

            PManager = new ProcessManager();
            PManager.SetProcess(processes[0].Id);
            return true;
        }

        private void StopScan()
        {
            SearchButton.Content = "Search";
            ViewModel.InterfaceEnabled = true;
            stopWatch.Stop();
            DTimer.Stop();
        }

        private void SearchAsync()
        {
            var memory = PManager.GetMemoryForProcess();

            DTimer.Start();
            stopWatch.Restart();

            List<int> endsOffsets = null;

            try
            {
                if (!string.IsNullOrEmpty(ViewModel.EndsWithFilterStr))
                {
                    endsOffsets = new List<int>(ViewModel.EndsWithFilterStr.Split(',').Select(x => Convert.ToInt32(x, 16)));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error parsing 'Ends with offset' string. Input example: 0x12A, c123D, 0X12");
                StopScanAsync();
                SearchThread = null;
                return;
            }

            var Scaner = new Scanner(ViewModel, memory, endsOffsets);

            StopScanAsync();
            SearchThread = null;
        }

        private void StopScanAsync()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) (() => StopScan()));
        }

        private void UpdateListOfProcesses()
        {
            var processes = Process.GetProcesses();
            ViewModel.Processes.Clear();

            foreach (var process in processes)
            {
                if (!process.ProcessName.ToUpper().Contains(TextBox_ProcessFilter.Text.ToUpper())) continue;

                bool is32 = false;

                try
                {
                    is32 = ProcessManager.Is32BitProcess(process);
                }
                catch
                {
                    continue;
                }

                var bits = is32 ? "x32" : "x64";
                ViewModel.Processes.Add($"{process.ProcessName}, ({bits}), Id: {process.Id}");
            }

            if (ViewModel.Processes.Count == 1)
            {
                ViewModel.SelectedProcessName = ViewModel.Processes[0];
            }
        }

        /// GUI buttons ////////////////////////////////////////////////
        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.InterfaceEnabled)
            {
                if (SearchThread != null)
                {
                    SearchThread.Abort();
                    SearchThread = null;
                }

                StopScan();
                return;
            }

            var scanValueValidation = ScanValueValidationRule.ExternalValidate(ViewModel.ScanValue);

            if (!scanValueValidation.IsValid)
            {
                ScanValueTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource(); // Update input validation
                return;
            }

            if (!InitProcessManager())
                return;

            SearchButton.Content = "Cancel";
            ViewModel.InterfaceEnabled = false;

            BackupLastScan();
            ViewModel.VisibleResults.Clear();

            SearchThread = new Thread(SearchAsync);
            SearchThread.Start();
        }

        private Stack<AsyncObservableCollection<VisibleResult>> PrevScanBackup = new Stack<AsyncObservableCollection<VisibleResult>>();

        private void BackupLastScan()
        {
            if (ViewModel.VisibleResults.Count > 0)
            {
                PrevScanBackup.Push(new AsyncObservableCollection<VisibleResult>(ViewModel.VisibleResults));
            }

            ViewModel.CanUndoScan = PrevScanBackup.Count > 0;
        }

        private void UndoScan(object sender, RoutedEventArgs e)
        {
            if (PrevScanBackup.Count > 0)
            {
                ViewModel.VisibleResults = PrevScanBackup.Pop();
            }

            ViewModel.CanUndoScan = PrevScanBackup.Count > 0;
        }

        private void RefreshProcesses(object sender, RoutedEventArgs e)
        {
            UpdateListOfProcesses();
        }

        private void TextBox_ProcessFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateListOfProcesses();
        }

        private void OnSearchTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScanValueTextBox == null) return;
            ScanValueTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource(); // Update input validation
        }

        /// Bottom window buttons ////////////////////////////////////////////////
        private void RefreshValues(object sender, RoutedEventArgs e)
        {
            RefreshTable(RezultRefreshType.RefreshValues);
        }

        private void DeleteBrokenPointers(object sender, RoutedEventArgs e)
        {
            RefreshTable(RezultRefreshType.DeleteBroken);
        }

        private void FilterValues(object sender, RoutedEventArgs e)
        {
            BackupLastScan();
            RefreshTable(RezultRefreshType.FilterValues);
        }

        private void RefreshTable(RezultRefreshType refreshType)
        {
            if (!InitProcessManager())
                return;

            var memory = PManager.GetMemoryForProcess();
            var comparer = Scanner.GetValueReaderByDataType(ViewModel.SelectedDataType, memory, ViewModel);

            var startPointer = new IntPtr(ViewModel.StartSearchAddress);

            var convertedCompareValue = comparer.ConvertToComparableValue(ViewModel.ScanValue);

            foreach (var pointer in ViewModel.VisibleResults.ToList())
            {
                if (!comparer.CheckTableRezult(pointer, startPointer, convertedCompareValue, refreshType))
                {
                    ViewModel.VisibleResults.Remove(pointer);
                }
            }
        }
    }

    public enum RezultRefreshType
    {
        RefreshValues,
        DeleteBroken,
        FilterValues
    }
}
