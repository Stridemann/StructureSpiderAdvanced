using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureSpiderAdvanced
{
    public class UStringValueReader : StringValueReader
    {
        public UStringValueReader(Memory m, MainViewModel mvm) : base(m, mvm) { }

        protected override string ReadString(IntPtr address)
        {
            return M.ReadStringU(address, MVM.StringLength);
        }
    }
}
