using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using MessageRPC;

using ShareMemory.Serialization;

namespace ShareMemory.Client
{
    public class Q
    {

        private RPC rpc;

        private string name;


        internal Q(RPC rpc, string name)
        {
            this.rpc = rpc;
            this.name = name;
        }

        public void En(object value)
        {

            if (value == null)
            {
                throw new ShareMemoryClientException("参数 value 不能为 null 。");
            }

            SMessage m = new SMessage();

            Serializer serializer = new Serializer();

            byte[] b = serializer.Serialize(value);

            m.Parameters.Add(new Para("method", "Enqueue"));
            m.Parameters.Add(new Para("name", this.name));

            m.Content = new MemoryStream(b);
            m.ContentLength = b.Length;

            this.rpc.Send(m);

        }

        public bool TryDe<T>(out T value)
        {

            SMessage m = new SMessage();

            m.Parameters.Add(new Para("method", "Dequeue"));
            m.Parameters.Add(new Para("name", this.name));

            RMessage rMsg = this.rpc.Send(m);

            Type t = typeof(T);

            if (rMsg.ContentLength == 0)
            {
                value = default(T);
                return false;
            }

            byte[] b = new byte[rMsg.ContentLength];

            Util.Read(rMsg.Content, b);

            DeSerializer deSerializer = new DeSerializer();

            value = deSerializer.DeSerialize<T>(b);

            return true;
        }

        public T De<T>()
        {
            T o;

            TryDe<T>(out o);

            return o;
        }
    }
}
