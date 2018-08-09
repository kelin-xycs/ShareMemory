using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMemory.Serialization
{
    public class SerializeException : Exception
    {
        internal SerializeException(string message) : base(message)
        {

        }
    }

    public class DeSerializeException : Exception
    {
        internal DeSerializeException(string message)
            : base(message)
        {

        }
    }

    public class MetaDataException : Exception
    {
        internal MetaDataException(string message)
            : base(message)
        {

        }
    }
}
