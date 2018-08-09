using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ShareMemory.Client
{
    class Util
    {
        public static void Read(Stream stream, byte[] b)
        {

            int offset = 0;

            int readCount;

            while (true)
            {
                readCount = stream.Read(b, offset, 1024);

                if (readCount == 0)
                {
                    break;
                }

                offset += readCount;
            }
        }
    }
}
