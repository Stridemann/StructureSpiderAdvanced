using System;

namespace StructureSpiderAdvanced.ValueReaders
{
    public class UStringValueReader : StringValueReader
    {
        public UStringValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

        public override string ReadDisplayString(IntPtr address)
        {
            return M.ReadStringU(address, MVM.StringLength);
        }

        protected override string ReadString(IntPtr address, bool trimEnd)
        {
            return M.ReadStringU(address, MVM.StringLength, trimEnd);
        }
    }
}
