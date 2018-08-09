using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ShareMemory.Serialization
{
    public class Serializer
    {
        public byte[] Serialize(object o)
        {

            Type t = o.GetType();


            if (t == typeof(byte[]))  //  byte 数组 本身就是序列化的输出格式，不需要再序列化，所以直接返回
            {
                return (byte[])o;
            }
                
            if (t.IsValueType || t == typeof(string))
            {
                return Encoding.UTF8.GetBytes(Util.ConvertForSerialize(o));
            }


            MyStringBuilder mySb;

            Type elementType;

            if (t.IsArray)
            {
                elementType = t.GetElementType();

                if (elementType.IsValueType || elementType == typeof(string))
                {
                    mySb = SerializeValueTypeAndStringArray(t, elementType, o);
                }
                else
                {
                    mySb = SerializeSimpleObjectArray(t, elementType, o);
                }
            }
            else
            {
                mySb = SerializeSimpleObject(t, o);
            }

            char[] chars = mySb.ToCharArray();

            return Encoding.UTF8.GetBytes(chars);
        }

        private MyStringBuilder SerializeValueTypeAndStringArray(Type t, Type elementType, object o)
        {

            Array array = (Array)o;

            MyStringBuilder mySb = new MyStringBuilder();

            mySb.Append("a " + array.Length.ToString() + " ");

            object element;

            string value;

            for (int i=0; i<array.Length; i++)
            {
                element = array.GetValue(i);

                value = Util.ConvertForSerialize(element);

                mySb.Append(value.Length.ToString() + " ");
                mySb.Append(value);  //  单独 Append Value ，可以避免 字符串相加 时的 拷贝字符串 操作，对于 大字符串，这个效应比较显著
            }

            return mySb;
        }

        private MyStringBuilder SerializeSimpleObjectArray(Type t, Type elementType, object o)
        {
            Array array = (Array)o;

            MyStringBuilder mySb = new MyStringBuilder();

            mySb.Append("a " + array.Length.ToString() + " ");

            MyStringBuilder mySb2;

            object element;

            for (int i=0; i<array.Length; i++)
            {

                element = array.GetValue(i);

                mySb2 = SerializeSimpleObject(elementType, element);

                mySb.Append(mySb2.Length.ToString() + " ");

                mySb.Append(mySb2);
            }

            return mySb;
        }

        private MyStringBuilder SerializeSimpleObject(Type t, object o)
        {
            TypeInfo typeInfo = Util.GetTypeInfo(t);

            MyStringBuilder mySb = new MyStringBuilder();

            mySb.Append("o ");

            FieldInfo f;
            Property p;

            object v;
            string value;

            for (int i=0; i<typeInfo.fieldList.Count; i++)
            {
                f = typeInfo.fieldList[i];

                
                mySb.Append(f.Name.Length + " " + f.Name);
                

                v = f.GetValue(o);

                value = Util.ConvertForSerialize(v);

                mySb.Append(value.Length + " ");
                mySb.Append(value);  //  单独 Append Value ，可以避免 字符串相加 时的 拷贝字符串 操作，对于 大字符串，这个效应比较显著
            }

            for (int i = 0; i < typeInfo.propertyList.Count; i++)
            {
                p = typeInfo.propertyList[i];


                mySb.Append(p.name.Length + " " + p.name);

                v = p.getMethod.Invoke(o, null);

                value = Util.ConvertForSerialize(v);

                mySb.Append(value.Length + " ");
                mySb.Append(value);  //  单独 Append Value ，可以避免 字符串相加 时的 拷贝字符串 操作，对于 大字符串，这个效应比较显著
            }

            return mySb;
        }

    }
}
