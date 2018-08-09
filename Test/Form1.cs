using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareMemory.Serialization;

namespace Test
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
            

            Serializer serializer = new Serializer();
            byte[] b = serializer.Serialize(o);

            DeSerializer deSerializer = new DeSerializer();
            TestObj o2 = deSerializer.DeSerialize<TestObj>(b);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            TestObj[] a = new TestObj[6];

            TestObj o;

            for (int i=0; i<6; i++)
            {
                o = new TestObj();

                o.i = i;
                o.d = 3.14;
                o.Dt = DateTime.Now;
                o.Str = "Hello world ." + " " + i;

                a[i] = o;
            }

            Serializer serializer = new Serializer();
            byte[] b = serializer.Serialize(a);

            DeSerializer deSerializer = new DeSerializer();
            TestObj[] a2 = deSerializer.DeSerialize<TestObj[]>(b);
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i = 4;

            Serializer serializer = new Serializer();
            byte[] b = serializer.Serialize(i);

            DeSerializer deSerializer = new DeSerializer();
            int i2 = deSerializer.DeSerialize<int>(b);


            float f = 3.14f;

            b = serializer.Serialize(f);

            float f2 = deSerializer.DeSerialize<float>(b);


            DateTime dt = DateTime.Now;

            b = serializer.Serialize(dt);

            DateTime dt2 = deSerializer.DeSerialize<DateTime>(b);


            string s = "Good Morning !";

            b = serializer.Serialize(s);

            string s2 = deSerializer.DeSerialize<string>(b);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int[] intArray = new int[6];

            for (int i=0; i<intArray.Length; i++)
            {
                intArray[i] = i + 5;
            }

            Serializer serializer = new Serializer();
            byte[] b = serializer.Serialize(intArray);

            DeSerializer deSerializer = new DeSerializer();
            int[] intArray2 = deSerializer.DeSerialize<int[]>(b);


            float[] floatArray = new float[6];

            for (int i = 0; i < intArray.Length; i++)
            {
                floatArray[i] = i + 5.2f;
            }

            b = serializer.Serialize(floatArray);

            float[] floatArray2 = deSerializer.DeSerialize<float[]>(b);


            DateTime[] dtArray = new DateTime[6];

            DateTime dt = DateTime.Now;

            for (int i = 0; i < intArray.Length; i++)
            {
                dtArray[i] = dt.AddMinutes(i);
            }

            b = serializer.Serialize(dtArray);

            DateTime[] dtArray2 = deSerializer.DeSerialize<DateTime[]>(b);


            string[] strArray = new string[6];

            string s = "Good";

            for (int i = 0; i < intArray.Length; i++)
            {
                strArray[i] = s + " " + i;
            }

            b = serializer.Serialize(strArray);

            string[] strArray2 = deSerializer.DeSerialize<string[]>(b);
        }

        private void button5_Click(object sender, EventArgs e)
        {   

            byte[] bytes = new byte[6] { 1, 2, 3, 4, 5, 6 };

            Serializer serializer = new Serializer();
            byte[] b = serializer.Serialize(bytes);

            DeSerializer deSerializer = new DeSerializer();
            byte[] bytes2 = deSerializer.DeSerialize<byte[]>(b);



            sbyte[] sbytes = new sbyte[6] { -3, -2, -1, 0, 1, 2 };

            b = serializer.Serialize(sbytes);

            sbyte[] sbytes2 = deSerializer.DeSerialize<sbyte[]>(b);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                TestObj2 o = new TestObj2();

                Serializer serializer = new Serializer();
                byte[] b = serializer.Serialize(o);

                DeSerializer deSerializer = new DeSerializer();
                TestObj2 o2 = deSerializer.DeSerialize<TestObj2>(b);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                TestObj3 o = new TestObj3();

                Serializer serializer = new Serializer();
                byte[] b = serializer.Serialize(o);

                DeSerializer deSerializer = new DeSerializer();
                TestObj3 o2 = deSerializer.DeSerialize<TestObj3>(b);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
