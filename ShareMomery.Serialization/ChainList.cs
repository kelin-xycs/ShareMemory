using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMemory.Serialization
{
    class ChainList<T>
    {

        private ChainListNode<T> head;
        private ChainListNode<T> tail;

        private ChainListNode<T> current;

        public void Add(ChainListNode<T> node)
        {
            if (this.head == null)
            {
                this.head = node;
                this.tail = node;

                return;
            }

            this.tail.Next = node;
            this.tail = node;
        }

        public void Append(ChainList<T> chainList)
        {
            if (this.head == null)
            {
                this.head = chainList.head;
                this.tail = chainList.tail;

                return;
            }

            this.tail.Next = chainList.head;
            this.tail = chainList.tail;
        }

        public bool MoveNext()
        {
            if (this.current == null)
            {
                if (this.head == null)
                    return false;

                this.current = this.head;
                return true;
            }

            if (this.current.Next == null)
            {
                return false;
            }

            this.current = this.current.Next;
            return true;
        }

        public ChainListNode<T> Visit()
        {
            if (this.current == null)
            {
                throw new ChainListException("当前位置 Node 为 null ， 请确认是否调用了 MoveNext() 方法且 MoveNext() 方法返回 true 。");
            }

            return this.current;
        }
    }

    class ChainListNode<T>
    {
        public ChainListNode<T> Next;
        public T Element;

        public ChainListNode(T element)
        {
            this.Element = element;
        }
    }

    public class ChainListException : Exception
    {
        internal ChainListException(string message) : base(message)
        {

        }
    }
}
