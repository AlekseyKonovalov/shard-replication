using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy2
{
    public class TableNote
    {
        public TableNote(int BottomLine, int UpperLine, int Port)
        {
            this.BottomLine = BottomLine;
            this.UpperLine = UpperLine;
            this.Port = Port;
        }
        public int BottomLine;
        public int UpperLine;
        public int Port;
    }
}
