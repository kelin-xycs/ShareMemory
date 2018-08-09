using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.InteropServices;

using MessageRPC;

namespace ShareMemory
{
    public class Host
    {


        private MemoryManager mm = new MemoryManager();

        private MessageRPC.Host rpcHost;



        public static Host CreateAndListen(string ip, int port)
        {
            Host host = new Host();

            host.Init();

            MessageRPC.Host rpcHost = MessageRPC.Host.CreateAndListen(ip, port, host.ProcessMessage);

            host.rpcHost = rpcHost;

            return host;
        }

        private Host()
        {
            
        }

        private void Init()
        {
            this.mm.Init();
        }

        private SMessage ProcessMessage(RMessage m)
        {
            string method;

            if (!m.Parameters.TryGetValue("method", out method))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"method\"， 应通过 \"method\" 参数指定要调用的方法 。");
            }

            switch (method)
            {
                case "SetDicObj":
                    return SetDicObj(m);
                case "GetDicObj" :
                    return GetDicObj(m);
                case "RemoveDicObj":
                    return RemoveDicObj(m);
                case "Enqueue":
                    return Enqueue(m);
                case "Dequeue":
                    return Dequeue(m);
                case "TryLock":
                    return TryLock(m);
                case "UnLock":
                    return UnLock(m);
                default :
                    throw new ShareMemoryException("无效的 method ： \"" + method + "\" 。");
                    
            }
        }

        private SMessage SetDicObj(RMessage m)
        {
            string name;

            if (!m.Parameters.TryGetValue("name", out name))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"name\"， 应通过 \"name\" 参数指定要访问的 Dictionary 的 名字 。");
            }

            string key;

            if (!m.Parameters.TryGetValue("key", out key))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"key\"， 应通过 \"key\" 参数指定要存的值的 键 。");
            }

            if (m.Content == null)
            {
                throw new ShareMemoryException("Message 中缺少 Content ， 应通过 Content 传递要放到 Dictionary 里的 对象 的 序列化数据 。");
            }

            byte[] b = new byte[m.ContentLength];

            Read(m.Content, b);
                
            this.mm.SetDicObj(name, key, b);

            SMessage sMsg = new SMessage();

            return sMsg;
        }

        private void Read(Stream stream, byte[] b)
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

        private SMessage GetDicObj(RMessage m)
        {
            string name;

            if (!m.Parameters.TryGetValue("name", out name))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"name\"， 应通过 \"name\" 参数指定要访问的 Dictionary 的 名字 。");
            }

            string key;

            if (!m.Parameters.TryGetValue("key", out key))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"key\"， 应通过 \"key\" 参数指定要取的值的 键 。");
            }

            byte[] b = this.mm.GetDicObj(name, key);

            SMessage sMsg = new SMessage();
            
            if (b != null)
            {
                sMsg.Content = new MemoryStream(b);
                sMsg.ContentLength = b.Length;
            }
            
            return sMsg;
        }

        private SMessage RemoveDicObj(RMessage m)
        {
            string name;

            if (!m.Parameters.TryGetValue("name", out name))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"name\"， 应通过 \"name\" 参数指定要访问的 Dictionary 的 名字 。");
            }

            string key;

            if (!m.Parameters.TryGetValue("key", out key))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"key\"， 应通过 \"key\" 参数指定要取的值的 键 。");
            }

            this.mm.RemoveDicObj(name, key);

            SMessage sMsg = new SMessage();

            return sMsg;
        }

        private SMessage Enqueue(RMessage m)
        {
            string name;

            if (!m.Parameters.TryGetValue("name", out name))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"name\"， 应通过 \"name\" 参数指定要访问的 Queue 的 名字 。");
            }

            if (m.Content == null)
            {
                throw new ShareMemoryException("Message 中缺少 Content ， 应通过 Content 传递要放到 Queue 里的 对象 的 序列化数据 。");
            }

            byte[] b = new byte[m.ContentLength];

            Read(m.Content, b);

            this.mm.Enqueue(name, b);

            SMessage sMsg = new SMessage();

            return sMsg;
        }

        private SMessage Dequeue(RMessage m)
        {
            string name;

            if (!m.Parameters.TryGetValue("name", out name))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"name\"， 应通过 \"name\" 参数指定要访问的 Queue 的 名字 。");
            }

            byte[] b = this.mm.Dequeue(name);

            SMessage sMsg = new SMessage();

            if (b != null)
            {
                sMsg.Content = new MemoryStream(b);
                sMsg.ContentLength = b.Length;
            }

            return sMsg;
        }

        private SMessage TryLock(RMessage m)
        {
            string name;

            if (!m.Parameters.TryGetValue("name", out name))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"name\"， 应通过 \"name\" 参数指定要创建的 Lock 的名字 。");
            }
            
            string lockId = this.mm.TryLock(name);

            SMessage sMsg = new SMessage();

            if (lockId != null)
            {
                sMsg.Parameters.Add(new Para("lockId", lockId));
            }

            return sMsg;
        }

        private SMessage UnLock(RMessage m)
        {
            string name;

            if (!m.Parameters.TryGetValue("name", out name))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"name\"， 应通过 \"name\" 参数指定要解锁的 Lock 的名字 。");
            }

            string lockId;

            if (!m.Parameters.TryGetValue("lockId", out lockId))
            {
                throw new ShareMemoryException("Message 中缺少参数 \"lockId\"， 应通过 \"lockId\" 参数指定要解锁的 Lock 的 lockId 。");
            }

            this.mm.UnLock(name, lockId);

            SMessage sMsg = new SMessage();

            return sMsg;
        }
    }
}
