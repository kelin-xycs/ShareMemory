using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ShareMemory.Serialization
{
    public class DeSerializer
    {

        

        public T DeSerialize<T>(byte[] b)
        {

            Type t = typeof(T);

            //  byte 数组 本身就是序列化的输出格式，不需要再序列化，也不需要反序列化，所以直接返回
            //  可参考 Serializer.Serialize() 方法
            if (t == typeof(byte[]))  
            {
                return (T)((object)b);
            }

            if (t.IsValueType || t == typeof(string))
            {
                return (T)DeSerializeValueTypeAndString(t, b);
            }


            Type elementType;

            if (t.IsArray)
            {
                elementType = t.GetElementType();

                if (elementType.IsValueType || elementType == typeof(string))
                {
                    return (T)DeSerializeValueTypeAndStringArray(t, b);
                }
                else
                {
                    return (T)DeSerializeSimpleObjectArray(t, b);
                }
                
            }
            else
            {
                return (T)DeSerializeSimpleObject(t, b);
            }
        }

        private object DeSerializeValueTypeAndStringArray(Type t, byte[] b)
        {

            char[] chars = Encoding.UTF8.GetChars(b);

            Type elementType = t.GetElementType();



            if (b.Length < 3)
            {
                throw new DeSerializeException("字节流长度过短 ，不是有效的字节流 。 字节流长度 ： " + b.Length + " 。");
            }

            if (chars[0] != 'a')
            {
                throw new DeSerializeException("数组字节流第 1 个字符应该是 \"a\" 。");
            }

            if (chars[1] != ' ')
            {
                throw new DeSerializeException("数组字节流第 2 个字符应该是 空格 \" \" 。");
            }


            int i = 2;
            int j;

            int elementCount;


            j = StrUtil.FindForward(chars, i, ' ');


            if (j == -1)
            {
                throw new DeSerializeException("找不到 Array 的 elementCount 项 的 结尾符 空格 \" \" 。 从第 " + i + " 个字符开始寻找 。");
            }

            if (j == i)
            {
                throw new DeSerializeException("Array 的 elementCount 项 应是 数字 。 在第 " + i + " 个字符 。");
            }

            if (!int.TryParse(new string(chars, i, j - 2 + 1), out elementCount))
            {
                throw new DeSerializeException("Array 的 elementCount 项 应是 数字 。 在第 " + i + " 个字符 。");
            }


            i = j + 1;


            Array array = Array.CreateInstance(elementType, elementCount);

            int arrayIndex = 0;

            int length;

            object o;

            string value;

            while (true)
            {
                if (i >= chars.Length)
                {
                    break;
                }

                j = StrUtil.FindForward(chars, i, ' ');

                if (j == -1)
                {
                    throw new DeSerializeException("找不到 Array 的 element 项 的 length 项 的 结尾符 空格 \" \" 。 从第 " + i + "个字符开始寻找 。");
                }

                if (j == i)
                {
                    throw new DeSerializeException("Array 的 element 项 的 length 项 应是 数字 。 在第 " + i + " 个字符 。");
                }

                if (!int.TryParse(new string(chars, i, j - i), out length))
                {
                    throw new DeSerializeException("Array 的 element 项 的 length 项 应是 数字 。 在第 " + i + " 个字符 。");
                }




                value = new string(chars, j + 1, length);

                o = Util.ConvertForDeSerialize(value, elementType);


                array.SetValue(o, arrayIndex++);


                i = j + 1 + length;

            }

            return array;
        }

        private object DeSerializeSimpleObject(Type t, byte[] b)
        {

            char[] chars = Encoding.UTF8.GetChars(b);

            return DeSerializeSimpleObject(t, chars, 0, chars.Length - 1);

        }

        private object DeSerializeValueTypeAndString(Type t, byte[] b)
        {

            string value = Encoding.UTF8.GetString(b);

            return Util.ConvertForDeSerialize(value, t);

        }

        private object DeSerializeSimpleObjectArray(Type t, byte[] b)
        {

            char[] chars = Encoding.UTF8.GetChars(b);


            Type elementType = t.GetElementType();



            if (b.Length < 3)
            {
                throw new DeSerializeException("字节流长度过短 ，不是有效的字节流 。 字节流长度 ： " + b.Length + " 。");
            }

            if (chars[0] != 'a')
            {
                throw new DeSerializeException("数组字节流第 1 个字符应该是 \"a\" 。");
            }

            if (chars[1] != ' ')
            {
                throw new DeSerializeException("数组字节流第 2 个字符应该是 空格 \" \" 。");
            }


            int i = 2;
            int j;

            int elementCount;


            j = StrUtil.FindForward(chars, i, ' ');


            if (j == -1)
            {
                throw new DeSerializeException("找不到 Array 的 elementCount 项 的 结尾符 空格 \" \" 。 从第 " + i + " 个字符开始寻找 。");
            }

            if (j == i)
            {
                throw new DeSerializeException("Array 的 elementCount 项 应是 数字 。 在第 " + i + " 个字符 。");
            }

            if (!int.TryParse(new string(chars, i, j - 2 + 1), out elementCount))
            {
                throw new DeSerializeException("Array 的 elementCount 项 应是 数字 。 在第 " + i + " 个字符 。");
            }

            
            i = j + 1;


            Array array = Array.CreateInstance(elementType, elementCount);

            int arrayIndex = 0;

            int length;

            object o;

            while (true)
            {
                if (i >= chars.Length)
                {
                    break;
                }

                j = StrUtil.FindForward(chars, i, ' ');

                if (j == -1)
                {
                    throw new DeSerializeException("找不到 Array 的 element 项 的 length 项 的 结尾符 空格 \" \" 。 从第 " + i + "个字符开始寻找 。");
                }

                if (j == i)
                {
                    throw new DeSerializeException("Array 的 element 项 的 length 项 应是 数字 。 在第 " + i + " 个字符 。");
                }

                if (!int.TryParse(new string(chars, i, j - i), out length))
                {
                    throw new DeSerializeException("Array 的 element 项 的 length 项 应是 数字 。 在第 " + i + " 个字符 。");
                }



                o = DeSerializeSimpleObject(elementType, chars, j + 1, j + length);


                array.SetValue(o, arrayIndex++);


                i = j + 1 + length;

            }

            return array;
        }

        private object DeSerializeSimpleObject(Type t, char[] chars, int beginIndex, int endIndex)
        {

            Dictionary<string, string> dataDic = GetDataDic(chars, beginIndex, endIndex);


            TypeInfo typeInfo = Util.GetTypeInfo(t);


            if (typeInfo.constructorInfo == null)
            {
                throw new DeSerializeException("反序列化的对象类型必须要有一个无参数的构造函数 。");
            }


            object o = typeInfo.constructorInfo.Invoke(null);

            FieldInfo f;
            Property p;

            string value;

            object v;

            for (int i=0; i<typeInfo.fieldList.Count; i++)
            {
                f = typeInfo.fieldList[i];

                if (!dataDic.TryGetValue(f.Name, out value))
                {
                    continue;
                }
                
                v = Util.ConvertForDeSerialize(value, f.FieldType);

                f.SetValue(o, v);
            }

            for (int i = 0; i < typeInfo.propertyList.Count; i++)
            {
                p = typeInfo.propertyList[i];

                if (!dataDic.TryGetValue(p.name, out value))
                {
                    continue;
                }

                v = Util.ConvertForDeSerialize(value, p.type);

                p.setMethod.Invoke(o, new object[] { v });
            }

            return o;
            
        }

        private Dictionary<string, string> GetDataDic(char[] chars, int beginIndex, int endIndex)
        {


            Dictionary<string, string> dataDic = new Dictionary<string, string>();


            if (endIndex - beginIndex < 2)
            {
                throw new DeSerializeException("用于反序列化对象的字节流长度过短 ， 不是有效的字节流 。 字节流 beginIndex ： " + beginIndex + " endIndex ： " + endIndex + " 。");
            }

            if (chars[beginIndex] != 'o')
            {
                throw new DeSerializeException("对象字节流第 1 个字符应该是 \"o\" 。 在第 " + beginIndex + " 个字符 。");
            }


            int i = beginIndex + 1;


            if (chars[i] != ' ')
            {
                throw new DeSerializeException("对象字节流第 2 个字符应该是 空格 \" \" 。 在第 " + i + " 个字符 。");
            }


            i = beginIndex + 2;

            int j;

            int length;

            string name;
            string value;

            while (true)
            {
                if (i > endIndex)
                {
                    break;
                }

                j = StrUtil.FindForward(chars, i, ' ');

                if (j == -1)
                {
                    throw new DeSerializeException("找不到 Property Field 的 name 项 的 length 项 的 结尾符 空格 \" \" 。 从第 " + i + " 个字符开始寻找 。");
                }

                if (j == i)
                {
                    throw new DeSerializeException("Property Field 的 name 项 的 length 项 应是 数字 。 在第 " + i + " 个字符 。");
                }

                if (!int.TryParse(new string(chars, i, j - i), out length))
                {
                    throw new DeSerializeException("Property Field 的 name 项 的 length 项 应是 数字 。 在第 " + i + " 个字符 。");
                }


                name = new string(chars, j + 1, length);


                i = j + 1 + length;



                j = StrUtil.FindForward(chars, i, ' ');


                if (j == -1)
                {
                    throw new DeSerializeException("找不到 Property Field 的 value 项 的 length 项 的 结尾符 空格 \" \" 。 从第 " + i + "个字符开始寻找 。");
                }

                if (j == i)
                {
                    throw new DeSerializeException("Property Field 的 value 项 的 length 项 应是 数字 。 在第 " + i + " 个字符 。");
                }

                if (!int.TryParse(new string(chars, i, j - i), out length))
                {
                    throw new DeSerializeException("Property Field 的 value 项 的 length 项 应是 数字 。 在第 " + i + " 个字符 。");
                }


                value = new string(chars, j + 1, length);


                i = j + 1 + length;


                dataDic.Add(name, value);
            }

            return dataDic;
        }

        

    }

    

}
