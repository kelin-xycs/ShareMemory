using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;

using ShareMemory.Client;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestObj o = new TestObj();

            o.i = 2;
            o.d = 3.14;
            o.f = 1.22f;  //  未加 [S] 标记的 Field 不会序列化
            o.Dt = DateTime.Now;
            o.Str = "Hello world .";
            o.de = 6.6666667M;  //  未加 [S] 标记的 Property 不会序列化

            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Set("testObj1", o);

            WriteMsg("Set \"testObj1\" success .\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            TestObj o = dic.Get<TestObj>("testObj1");

            if (o == null)
            {
                WriteMsg("\"testObj1\" is null .\r\n");
                return;
            }
            
            WriteObj("Get \"testObj1\"", o);
            WriteLine();
        }

        private void WriteTestObjArray(string name, TestObj[] a)
        {
            WriteMsg(name);

            TestObj o;

            for (int i=0; i<a.Length; i++)
            {
                o = a[i];

                WriteMsg("a[" + i + "] =");

                WriteMsg("  o.i = " + o.i);
                WriteMsg("  o.d = " + o.d);
                WriteMsg("  o.Dt = " + o.Dt);
                WriteMsg("  o.Str = " + o.Str);
            }
        }

        private void WriteIntArray(string name, int[] a)
        {
            WriteMsg(name);

            int o;

            for (int i = 0; i < a.Length; i++)
            {
                o = a[i];

                WriteMsg("a[" + i + "] = " + o);
            }
        }

        private void WriteStringArray(string name, string[] a)
        {
            WriteMsg(name);

            string o;

            for (int i = 0; i < a.Length; i++)
            {
                o = a[i];

                WriteMsg("a[" + i + "] = \"" + o + "\"");
            }
        }

        private void WriteObj(string name, TestObj o)
        {
            WriteMsg(name);

            WriteMsg("  o.i = " + o.i);
            WriteMsg("  o.d = " + o.d);
            WriteMsg("  o.Dt = " + o.Dt);
            WriteMsg("  o.Str = " + o.Str);
        }

        private void WriteLine()
        {
            txtMsg.AppendText("\r\n");
        }

        private void WriteMsg(string msg)
        {
            txtMsg.AppendText(DateTime.Now.ToString("HH:mm:ss") + "    " + msg + "\r\n");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMsg.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");
            
            string s = "Hello .";

            dic.Set("s", s);

            WriteMsg("Set \"s\" = \"" + s + "\" success .\r\n");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            string s = dic.Get<string>("s");

            if (s == null)
            {
                WriteMsg("\"s\" is null .\r\n");
                return;
            }

            WriteMsg("Get \"s\" = \"" + s + "\"\r\n");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            int i = 2;

            dic.Set("i", i);

            WriteMsg("Set \"i\" = " + i + " success .\r\n");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            int i;
            
            //  Dic.TryGet<T>() 方法是对 ValueType 设计的，因为 ValueType 不能根据返回值为 null 来判断在 ShareMemory 中是否存在
            //  所以要用 TryGet<T>() 的方式，其它类型用 Get<T>() 方法即可。
            //  调用 Get<T>() 方法，返回值为 null 表示对象在 ShareMemory 中不存在
            if (!dic.TryGet<int>("i", out i))
            {
                WriteMsg("\"i\" is not Existed in Dic .\r\n");
                return;
            }

            WriteMsg("Get \"i\" = " + i + "\r\n");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int[] a = new int[6];

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = i + 5;
            }

            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Set("int Array", a);

            WriteMsg("Set \"int Array\" success .\r\n");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            int[] a = dic.Get<int[]>("int Array");

            if (a == null)
            {
                WriteMsg("\"int Array\" is null .\r\n");
                return;
            }

            WriteIntArray("Get \"int Array\"", a);
            WriteLine();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            TestObj[] a = new TestObj[6];

            TestObj o;

            for (int i = 0; i < 6; i++)
            {
                o = new TestObj();

                o.i = i;
                o.d = 3.14;
                o.Dt = DateTime.Now;
                o.Str = "Hello world ." + " " + i;

                a[i] = o;
            }

            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Set("testObj Array", a);

            WriteMsg("Set \"testObj Array\" success .\r\n");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            TestObj[] a = dic.Get<TestObj[]>("testObj Array");

            if (a == null)
            {
                WriteMsg("\"testObj Array\" is null .\r\n");
                return;
            }

            WriteTestObjArray("Get \"testObj Array\"", a);
            WriteLine();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string[] a = new string[6];

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = "Hello " + i + " .";
            }

            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Set("string Array", a);

            WriteMsg("Set \"string Array\" success .\r\n");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            string[] a = dic.Get<string[]>("string Array");

            if (a == null)
            {
                WriteMsg("\"string Array\" is null .\r\n");
                return;
            }

            WriteStringArray("Get \"string Array\"", a);
            WriteLine();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Remove("i");

            WriteMsg("Remove \"i\" success .\r\n");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Remove("s");

            WriteMsg("Remove \"s\" success .\r\n");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Remove("testObj1");

            WriteMsg("Remove \"testObj1\" success .\r\n");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Remove("int Array");

            WriteMsg("Remove \"int Array\" success .\r\n");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Remove("string Array");

            WriteMsg("Remove \"string Array\" success .\r\n");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Dic dic = helper.GetDic("Dic1");

            dic.Remove("testObj Array");

            WriteMsg("Remove \"testObj Array\" success .\r\n");
        }


        private bool tryLocking1;
        private bool locked1;
        private string lockId1;

        private void button19_Click(object sender, EventArgs e)
        {

            if (this.tryLocking1)
            {
                return;
            }

            if (this.locked1)
            {
                return;
            }

            this.tryLocking1 = true;

            Thread thread = new Thread(
                () =>
                {
                    this.Invoke(new Action<string>(this.WriteMsg), new object[] { "线程 1  TryLock .." });

                    Helper helper = new Helper("127.0.0.1", 9527);

                    lockId1 = helper.TryLock("testLog");

                    
                    while (true)
                    {
                        if (lockId1 != null)
                        {
                            break;
                        }

                        Thread.Sleep(1);

                        lockId1 = helper.TryLock("testLog");
                    }
                    

                    this.tryLocking1 = false;
                    this.locked1 = true;

                    this.Invoke(new Action<string>(this.WriteMsg), new object[] { "线程 1  TryLock success ." });
                }
            );
            
            thread.Start();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                Helper helper = new Helper("127.0.0.1", 9527);

                helper.UnLock("testLog", this.lockId1);

                this.Invoke(new Action<string>(this.WriteMsg), new object[] { "线程 1  UnLock success ." });
            }
            catch(Exception ex)
            {
                WriteMsg(ex.ToString());
            }

            this.locked1 = false;
        }

        private bool tryLocking2;
        private bool locked2;
        private string lockId2;

        private void button22_Click(object sender, EventArgs e)
        {

            if (this.tryLocking2)
            {
                return;
            }

            if (this.locked2)
            {
                return;
            }

            this.tryLocking2 = true;

            Thread thread = new Thread(
                () =>
                {
                    this.Invoke(new Action<string>(this.WriteMsg), new object[] { "线程 2  TryLock .." });

                    Helper helper = new Helper("127.0.0.1", 9527);

                    lockId2 = helper.TryLock("testLog");


                    while (true)
                    {
                        if (lockId2 != null)
                        {
                            break;
                        }

                        Thread.Sleep(1);

                        lockId2 = helper.TryLock("testLog");
                    }


                    this.tryLocking2 = false;
                    this.locked2 = true;

                    this.Invoke(new Action<string>(this.WriteMsg), new object[] { "线程 2  TryLock success ." });
                }
            );

            thread.Start();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                Helper helper = new Helper("127.0.0.1", 9527);

                helper.UnLock("testLog", this.lockId2);

                this.Invoke(new Action<string>(this.WriteMsg), new object[] { "线程 2  UnLock success ." });
            }
            catch(Exception ex)
            {
                WriteMsg(ex.ToString());
            }

            this.locked2 = false;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            
            TestObj o = new TestObj();

            o.i = 2;
            o.d = 3.14;
            o.f = 1.22f;  //  未加 [S] 标记的 Field 不会序列化
            o.Dt = DateTime.Now;
            o.Str = "Hello world .";
            o.de = 6.6666667M;  //  未加 [S] 标记的 Property 不会序列化

            Helper helper = new Helper("127.0.0.1", 9527);

            Q q = helper.GetQ("Queue1");

            q.En(o);

            WriteMsg("Enqueue a instance of TestObj success .\r\n");
        }

        private void button24_Click(object sender, EventArgs e)
        {
            Helper helper = new Helper("127.0.0.1", 9527);

            Q q = helper.GetQ("Queue1");

            TestObj o = q.De<TestObj>();

            if (o == null)
            {
                WriteMsg("Queue is empty .\r\n");
                return;
            }

            WriteObj("Dequeue", o);
            WriteLine();
        }
    }
}
