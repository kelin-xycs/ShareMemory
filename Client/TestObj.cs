using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShareMemory.Serialization;

namespace Client
{
    class TestObj
    {
        [S]
        public int i;
        [S]
        public double d;

        //  未加 [S] 标记的 Field 不会序列化
        public float f;

        [S]
        public DateTime Dt
        {
            get;
            set;
        }

        [S]
        public string Str
        {
            get;
            set;
        }

        //  未加 [S] 标记的 Property 不会序列化
        public Decimal de
        {
            get;
            set;
        }

    }
}
