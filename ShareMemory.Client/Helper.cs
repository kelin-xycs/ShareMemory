using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessageRPC;

namespace ShareMemory.Client
{
    public class Helper
    {
        private RPC rpc;

        public Helper(string ip, int port)
        {
            this.rpc = new RPC(ip, port);
        }

        public Dic GetDic(string name)
        {
            return new Dic(this.rpc, name);
        }

        public Q GetQ(string name)
        {
            return new Q(this.rpc, name);
        }

        public string TryLock(string lockName)
        {
            if (lockName == null)
            {
                throw new ShareMemoryClientException("参数 lockName 不能为 null 。");
            }

            SMessage m = new SMessage();

            m.Parameters.Add(new Para("method", "TryLock"));
            m.Parameters.Add(new Para("name", lockName));

            RMessage rMsg = this.rpc.Send(m);

            string lockId;

            if (rMsg.Parameters.TryGetValue("lockId", out lockId))
            {
                return lockId;
            }

            return null;
        }

        public void UnLock(string lockName, string lockId)
        {
            if (lockName == null)
            {
                throw new ShareMemoryClientException("参数 lockName 不能为 null 。");
            }

            if (lockId == null)
            {
                throw new ShareMemoryClientException("参数 lockId 不能为 null 。");
            }

            SMessage m = new SMessage();

            m.Parameters.Add(new Para("method", "UnLock"));
            m.Parameters.Add(new Para("name", lockName));
            m.Parameters.Add(new Para("lockId", lockId));

            this.rpc.Send(m);
        }
    }
}
