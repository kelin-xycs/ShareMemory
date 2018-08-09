using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ShareMemory.Serialization
{
    class Util
    {


        private static Dictionary<Type, TypeInfo> _typeInfoDic = new Dictionary<Type, TypeInfo>();



        public static string ConvertForSerialize(object v)
        {
            Type t = v.GetType();

            if (t == typeof(string))
            {
                return (string)v;
            }

            if (t == typeof(DateTime))
            {
                return ((DateTime)v).Ticks.ToString();
            }

            return v.ToString();
        }

        public static object ConvertForDeSerialize(string value, Type conversionType)
        {
            if (conversionType == typeof(string))
            {
                return value;
            }

            if (conversionType == typeof(DateTime))
            {
                return new DateTime(long.Parse(value));
            }
            
            return System.Convert.ChangeType(value, conversionType);
        }

        public static TypeInfo GetTypeInfo(Type t)
        {
            TypeInfo typeInfo;

            if (_typeInfoDic.TryGetValue(t, out typeInfo))
            {
                return typeInfo;
            }

            typeInfo = new TypeInfo();

            typeInfo.constructorInfo = t.GetConstructor(Type.EmptyTypes);

            SAttribute s;

            foreach (PropertyInfo p in t.GetProperties())
            {

                s = p.GetCustomAttribute<SAttribute>();

                if (s == null)
                    continue;

                if (!p.PropertyType.IsValueType && p.PropertyType != typeof(string))
                {
                    throw new MetaDataException("只支持对 ValueType 和 String 类型的 Property 序列化 。 简单的讲 ， 不支持嵌套对象 。");
                }

                MethodInfo getM = p.GetGetMethod();
                MethodInfo setM = p.GetSetMethod();

                typeInfo.propertyList.Add(new Property(p.Name, getM, setM, p.PropertyType));
            }

            foreach (FieldInfo f in t.GetFields())
            {

                s = f.GetCustomAttribute<SAttribute>();

                if (s == null)
                    continue;

                if (!f.FieldType.IsValueType && f.FieldType != typeof(string))
                {
                    throw new MetaDataException("只支持对 ValueType 和 String 类型的 Field 序列化 。 简单的讲 ， 不支持嵌套对象 。");
                }

                typeInfo.fieldList.Add(f);
            }


            if (typeInfo.propertyList.Count == 0 && typeInfo.fieldList.Count == 0)
                throw new MetaDataException("应至少在一个 public Property 或 public Field 上 标注 [ S ] 标记 。");


            lock (_typeInfoDic)
            {
                if (!_typeInfoDic.ContainsKey(t))
                {
                    _typeInfoDic.Add(t, typeInfo);
                }
            }

            return typeInfo;
        }
    }

    class TypeInfo
    {
        public ConstructorInfo constructorInfo;

        public List<Property> propertyList = new List<Property>();

        public List<FieldInfo> fieldList = new List<FieldInfo>();
    }

    class Property
    {
        public string name;

        public MethodInfo getMethod;

        public MethodInfo setMethod;

        public Type type;

        public Property(string name, MethodInfo getMethod, MethodInfo setMethod, Type type)
        {
            this.name = name;
            this.getMethod = getMethod;
            this.setMethod = setMethod;
            this.type = type;
        }
    }
}
