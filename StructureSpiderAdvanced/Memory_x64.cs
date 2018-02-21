using System;

namespace StructureSpiderAdvanced
{
    public class Memory_x64 : Memory
    {
        public Memory_x64(IntPtr procHandle) : base(procHandle)
        {
            PointerStaticMinValue = 0x7FF000000000;       
            PointerStaticMaxValue = 0x7FFF00000000;
            PointerMinValue =   0x100000000;
            PointerMaxValue =   0xF000000000;
            //          
            PointerLength = 8;
            Is64Bit = true;
        }

        public override IntPtr ReadPointer(IntPtr addr)
        {
            var bytes = ReadBytes(addr, 8);
            var intptrNum = BitConverter.ToInt64(bytes, 0);
            return new IntPtr(intptrNum);
        }
    }
}
