using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMemory
{
    public class ShareMemoryException : Exception
    {
        internal ShareMemoryException(string message) : base(message)
        {

        }
    }
}
