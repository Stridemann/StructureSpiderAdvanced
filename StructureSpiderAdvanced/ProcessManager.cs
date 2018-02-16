using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StructureSpiderAdvanced
{
    public class ProcessManager : IDisposable
    {
        public IntPtr ProcHandle { get; private set; }
        public bool X64Process { get; private set; }
        public Process Process { get; private set; }
        public bool FoundProcess { get; private set; }
        public void SetProcess(int pId)
        {
            try
            {
                Process = Process.GetProcessById(pId);
                X64Process = !Is32BitProcess(Process);
                ProcHandle = OpenProcessHandle(Process, ProcessAccessFlags.All);

                FoundProcess = true;
                _closed = false;

                Process.Exited += Process_Exited;
            }
            catch (Win32Exception ex)
            {
                throw new Exception("You should run program as an administrator", ex);
            }
        }
        private void Process_Exited(object sender, EventArgs e)
        {
            Dispose();
            FoundProcess = false;
            _closed = true;
        }
        public Memory GetMemoryForProcess()
        {
            if(!FoundProcess)
            {
                throw new InvalidOperationException("Process handle is not initialized.");
            }
            if (X64Process)
                return new Memory_x64(ProcHandle);

            return new Memory(ProcHandle);
        }

        ~ProcessManager()
        {
            Dispose();
        }

        private bool _closed;
        public void Dispose()
        {
            if (!_closed)
            {
                _closed = true;
                CloseHandle(ProcHandle);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);
        public static bool Is32BitProcess(Process p)
        {
            bool retVal;

            if (!IsWow64Process(p.Handle, out retVal))
                throw new InvalidOperationException("Can't define type of process!");

            return retVal;
        }
        public static IntPtr OpenProcessHandle(Process process, ProcessAccessFlags flags)
        {
            return OpenProcess(flags, false, process.Id);
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }
    }
}
