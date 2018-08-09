using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.IO;
using System.Threading;

namespace ShareMemory
{
    class MemoryManager
    {

        private Dictionary<string, Dictionary<string, byte[]>> dicDic = new Dictionary<string, Dictionary<string, byte[]>>();

        private Dictionary<string, string> locks = new Dictionary<string, string>();

        private Queue<Lock> lockq = new Queue<Lock>();

        private Dictionary<string, Queue<byte[]>> queueDic = new Dictionary<string, Queue<byte[]>>();


        public MemoryManager()
        {

        }

        public void Init()
        {
            CreateDics();

            CreateQueues();

            CreateLockRecycleThread();
        }

        private void CreateDics()
        {
            string dics = ConfigurationManager.AppSettings["ShareMemory.Dics"];

            if (dics == null)
                return;

            string[] tokens = dics.Split(new char[] { ',' });

            string t;

            for (int i = 0; i < tokens.Length; i++)
            {
                t = tokens[i].Trim();

                if (string.IsNullOrEmpty(t))
                {
                    throw new ShareMemoryException("Dictionary 的名字不能为空 \"\" 。");
                }

                dicDic.Add(t, new Dictionary<string, byte[]>());
            }
        }

        private void CreateQueues()
        {
            string queues = ConfigurationManager.AppSettings["ShareMemory.Queues"];

            if (queues == null)
                return;

            string[] tokens = queues.Split(new char[] { ',' });

            string t;

            for (int i = 0; i < tokens.Length; i++)
            {
                t = tokens[i].Trim();

                if (string.IsNullOrEmpty(t))
                {
                    throw new ShareMemoryException("Queue 的名字不能为空 \"\" 。");
                }

                queueDic.Add(t, new Queue<byte[]>());
            }
        }

        private void CreateLockRecycleThread()
        {
            Thread thread = new Thread(RecycleLock);
            thread.Start();
        }

        private void RecycleLock()
        {
            DateTime d1 = DateTime.Now;
            DateTime d2;

            while(true)
            {
                Thread.Sleep(1000);

                d2 = DateTime.Now;

                //  每 30 秒回收一次
                if ((d2 - d1).TotalSeconds <= 30)
                {
                    continue;
                }

                DoRecycleLock();

                d1 = DateTime.Now;
            }
        }
        
        private void DoRecycleLock()
        {
            Lock l;
            string lockId;

            int count = locks.Count;

            for (int i = 0; i < count; i++)
            {
                l = null;
                lockId = null;

                lock (lockq)
                {
                    if (lockq.Count > 0)
                    {
                        l = lockq.Dequeue();
                    }
                }

                if (l == null)
                    break;

                lock (locks)
                {
                    //  如果在 locks 中没有 l.name 对应的 键 ， 则说明 Lock 不存在（可能已经被应用程序正常解锁），不需要回收
                    if (!locks.TryGetValue(l.name, out lockId))
                        continue;
                }

                //  如果在 locks 中存在 l.name 对应的 键 ， 但 lockId 不一样， 则说明这是一个新的 Lock
                //  l 对应的 Lock 应该已经被应用程序正常解锁 ， 也不需要做处理
                if (l.lockId != lockId)
                    continue;

                //  如果超过 60 秒仍未解锁，则自动解锁
                if ((DateTime.Now - l.createTime).TotalSeconds > 60)
                {
                    lock (locks)
                    {
                        locks.Remove(l.name);
                    }

                    continue;
                }

                //  如果超时时间未到 ， 还不需要自动解锁 ， 则将 l 放回 queue ， 等待下一次回收时检查是否需要自动解锁
                lock(lockq)
                {
                    lockq.Enqueue(l);
                }

            }
        }

        public void SetDicObj(string name, string key, byte[] b)
        {
            Dictionary<string, byte[]> dic;

            if (!dicDic.TryGetValue(name, out dic))
            {
                throw new ShareMemoryException("名字为 \"" + name + "\" 的 Dictionary 不存在 。");
            }

            lock(dic)
            {
                dic[key] = b;
            }
        }

        public byte[] GetDicObj(string name, string key)
        {
            Dictionary<string, byte[]> dic;

            if (!dicDic.TryGetValue(name, out dic))
            {
                throw new ShareMemoryException("名字为 \"" + name + "\" 的 Dictionary 不存在 。");
            }

            byte[] b;

            bool isExist;

            lock(dic)
            {
                isExist = dic.TryGetValue(key, out b);
            }
            
            if (!isExist)
            {
                return null;
            }

            return b;
        }

        public void RemoveDicObj(string name, string key)
        {

            Dictionary<string, byte[]> dic;

            if (!dicDic.TryGetValue(name, out dic))
            {
                throw new ShareMemoryException("名字为 \"" + name + "\" 的 Dictionary 不存在 。");
            }

            lock(dic)
            {
                dic.Remove(key);
            }
        }

        public void Enqueue(string name, byte[] b)
        {
            Queue<byte[]> queue;

            if (!queueDic.TryGetValue(name, out queue))
            {
                throw new ShareMemoryException("名字为 \"" + name + "\" 的 Queue 不存在 。");
            }

            lock(queue)
            {
                queue.Enqueue(b);
            }
        }

        public byte[] Dequeue(string name)
        {
            Queue<byte[]> queue;

            if (!queueDic.TryGetValue(name, out queue))
            {
                throw new ShareMemoryException("名字为 \"" + name + "\" 的 Queue 不存在 。");
            }

            lock (queue)
            {
                if (queue.Count == 0)
                {
                    return null;
                }
                    
                return queue.Dequeue();
            }
        }

        public string TryLock(string lockName)
        {
            string lockId = Guid.NewGuid().ToString();

            lock (locks)
            {
                if (locks.ContainsKey(lockName))
                {
                    return null;
                }

                locks.Add(lockName, lockId);
                lockq.Enqueue(new Lock(lockName, lockId, DateTime.Now));

                return lockId;
            }
        }

        public void UnLock(string lockName, string lockId)
        {
            string id;

            lock (locks)
            {
                if (!locks.TryGetValue(lockName, out id))
                {
                    throw new ShareMemoryException("名字为 \"" + lockName + "\" 的 Lock 不存在 ， 不能 UnLock 。");
                }

                if (lockId != id)
                {
                    throw new ShareMemoryException("lockId 不匹配 ， 不能 UnLock 。");
                }
                
                locks.Remove(lockName);
            }
        }

        private class Lock
        {
            public string name;
            public string lockId;
            public DateTime createTime;

            public Lock(string name, string lockId, DateTime createTime)
            {
                this.name = name;
                this.lockId = lockId;
                this.createTime = createTime;
            }
        }
    }
}
