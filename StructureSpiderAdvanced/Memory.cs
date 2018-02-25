using System;
using System.Text;
using System.Runtime.InteropServices;

namespace StructureSpiderAdvanced
{
    public class Memory
    {
        protected long PointerStaticMinValue;
        protected long PointerStaticMaxValue;
        protected long PointerMinValue;
        protected long PointerMaxValue;

        protected readonly IntPtr ProcHandle;
        public readonly ProcessSections SectionsController;

        public int PointerLength { get; protected set; }
        public bool Is64Bit { get; protected set; }

        public Memory(IntPtr procHandle, ProcessSections sectionsController)
        {
            SectionsController = sectionsController;
            PointerStaticMinValue = 0x10000000;
            PointerStaticMaxValue = 0xF0000000;
            PointerMinValue = 0x10000;
            PointerMaxValue = 0xF0000000;
            PointerLength = 4;
            ProcHandle = procHandle;
        }

        public byte[] ReadBytes(IntPtr addr, int length)
        {
            return ReadMem(addr, length);
        }

        public int ReadByte(IntPtr addr)
        {
            var bytes = ReadMem(addr, 1);
            return bytes[0];
        }

        public int ReadInt(IntPtr addr)
        {
            var bytes = ReadMem(addr, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public uint ReadUInt(IntPtr addr)
        {
            var bytes = ReadMem(addr, 4);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public float ReadFloat(IntPtr addr)
        {
            var bytes = ReadMem(addr, 4);
            return BitConverter.ToSingle(bytes, 0);
        }

        public long ReadLong(IntPtr addr)
        {
            var bytes = ReadMem(addr, 8);
            return BitConverter.ToInt64(bytes, 0);
        }

        public virtual IntPtr ReadPointer(IntPtr addr)//will be overrided by Memory_x64 class
        {
            var bytes = ReadMem(addr, 4);
            var intptrNum = BitConverter.ToUInt32(bytes, 0);
            return new IntPtr(intptrNum);
        }

        public string ReadString(IntPtr addr, int length = 256, bool replaceNull = true)
        {
            var checkAddr = addr.ToInt64();
            if (checkAddr <= 65536 && checkAddr >= -1)
            {
                return string.Empty;
            }
            string result = Encoding.ASCII.GetString(ReadMem(addr, length));
            return replaceNull ? RTrimNull(result) : result;
        }

        public string ReadStringU(IntPtr addr, int length = 256, bool replaceNull = true)
        {
            var checkAddr = addr.ToInt64();
            if (checkAddr <= 65536 && checkAddr >= -1)
            {
                return string.Empty;
            }
            byte[] mem = ReadMem(addr, length);
            if (mem.Length == 0)
            {
                return String.Empty;
            }
            if (mem[0] == 0 && mem[1] == 0)
                return string.Empty;

            string rezult = Encoding.Unicode.GetString(mem);
            return replaceNull ? RTrimNull(rezult) : rezult;
        }

        private static string RTrimNull(string text)
        {
            int num = text.IndexOf('\0');
            return num > 0 ? text.Substring(0, num) : text;
        }

        public SectionCategory CheckPointer(IntPtr pointer)
        {
            if (pointer.IsNull())
                return SectionCategory.Unknown;

            if(!MainViewModel.Instance.UseMemoryPage)
            {
                var address = pointer.ToInt64();
                if (address >= PointerStaticMinValue && address <= PointerStaticMaxValue)
                    return SectionCategory.CODE;
                if (address >= PointerMinValue && address <= PointerMaxValue)
                    return SectionCategory.HEAP;
                return SectionCategory.Unknown;
            }
          
            Section section;
            try
            {
                section = SectionsController.GetSectionToPointer(pointer);
            }
            catch
            {
                return SectionCategory.Unknown;
            }

            if(section != null)
                return section.Category;

            return SectionCategory.Unknown;
        }

        // If the section contains data, it is at least a pointer to a class or something.
        public bool IsSimplePointer(IntPtr pointer)
        {
            var pointerType = CheckPointer(pointer);
            return pointerType == SectionCategory.HEAP || pointerType == SectionCategory.DATA;
        }

        public static bool ReadProcessMemory(IntPtr handle, IntPtr baseAddress, byte[] buffer)
        {
            return ReadProcessMemory(handle, baseAddress, buffer, buffer.Length, out IntPtr bytesRead);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hWnd, IntPtr baseAddr, byte[] buffer, int size, out IntPtr bytesRead);

        private byte[] ReadMem(IntPtr addr, int size)
        {
            var array = new byte[size];
            ReadProcessMemory(ProcHandle, addr, array);
            return array;
        }


  
    }
}
