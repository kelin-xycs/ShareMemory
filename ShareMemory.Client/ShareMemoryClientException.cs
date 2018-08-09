using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMemory.Client
{
    public class ShareMemoryClientException : Exception
    {
        internal ShareMemoryClientException(string message) : base(message)
        {

        }
    }
}
