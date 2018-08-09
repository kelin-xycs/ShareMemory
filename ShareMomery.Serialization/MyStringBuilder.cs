using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMemory.Serialization
{
    class MyStringBuilder
    {


        private ChainList<string> chainList = new ChainList<string>();

        private int length;


        public void Append(string s)
        {
            ChainListNode<string> node = new ChainListNode<string>(s);

            this.chainList.Add(node);

            this.length += s.Length;
        }

        public void Append(MyStringBuilder mySb)
        {
            this.chainList.Append(mySb.chainList);

            this.length += mySb.length;
        }

        public int Length
        {
            get { return this.length; }
        }

        public char[] ToCharArray()
        {
            char[] chars = new char[this.length];

            string s;

            int i = 0;

            while(true)
            {
                if (!this.chainList.MoveNext())
                    break;

                s = this.chainList.Visit().Element;

                s.CopyTo(0, chars, i, s.Length);

                i += s.Length;
            }

            return chars;
        }
    }
}
