using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMemory.Serialization
{
    class StrSpan
    {
        public int iLeft;
        public int iRight;
        public bool isEmpty = false;

        public StrSpan(int iLeft, int iRight)
        {
            this.iLeft = iLeft;
            this.iRight = iRight;
        }
    }

    class StrUtil
    {
        public static int FindForward(char[] chars, int beginIndex, char c)
        {
            
            for (int i = beginIndex; i <= chars.Length; i++)
            {
                if (chars[i] == c)
                {
                    return i;
                }   
            }

            return -1;
        }
    }
}
