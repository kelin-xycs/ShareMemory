using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShareMemory;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {

            Host host = Host.CreateAndListen("127.0.0.1", 9527);

        }
    }
}
