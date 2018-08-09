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
    public class Dic
    {

        private RPC rpc;

        private string name;


        internal Dic(RPC rpc, string name)
        {
            this.rpc = rpc;
            this.name = name;
        }

        public void Set(string key, object value)
        {

            if (key == null)
            {
                throw new ShareMemoryClientException("参数 key 不能为 null 。");
            }

            if (value == null)
            {
                throw new ShareMemoryClientException("参数 value 不能为 null 。");
            }

            SMessage m = new SMessage();

            Serializer serializer = new Serializer();

            byte[] b = serializer.Serialize(value);

            m.Parameters.Add(new Para("method", "SetDicObj"));
            m.Parameters.Add(new Para("name", this.name));
            m.Parameters.Add(new Para("key", key));

            m.Content = new MemoryStream(b);
            m.ContentLength = b.Length;

            this.rpc.Send(m);

        }

        public bool TryGet<T>(string key, out T value)
        {

            if (key == null)
            {
                throw new ShareMemoryClientException("参数 key 不能为 null 。");
            }

            SMessage m = new SMessage();

            m.Parameters.Add(new Para("method", "GetDicObj"));
            m.Parameters.Add(new Para("name", this.name));
            m.Parameters.Add(new Para("key", key));

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

        public T Get<T>(string key)
        {
            T o;

            TryGet<T>(key, out o);

            return o;
        }

        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ShareMemoryClientException("参数 key 不能为 null 。");
            }

            SMessage m = new SMessage();

            m.Parameters.Add(new Para("method", "RemoveDicObj"));
            m.Parameters.Add(new Para("name", this.name));
            m.Parameters.Add(new Para("key", key));

            this.rpc.Send(m);
        }
    }
}
