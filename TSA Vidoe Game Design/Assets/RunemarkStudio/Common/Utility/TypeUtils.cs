using System;
using System.Collections.Generic;
using System.Linq;

namespace Runemark.Common
{
	public static class TypeUtils
	{
        static Dictionary<Type, string> shorthandMap = new Dictionary<Type, string>
        {
            { typeof(Boolean), "bool" },
            { typeof(Byte), "byte" },
            { typeof(Char), "char" },
            { typeof(Decimal), "decimal" },
            { typeof(Double), "double" },
            { typeof(Single), "float" },
            { typeof(Int32), "int" },
            { typeof(Int64), "long" },
            { typeof(SByte), "sbyte" },
            { typeof(Int16), "short" },
            { typeof(String), "string" },
            { typeof(UInt32), "uint" },
            { typeof(UInt64), "ulong" },
            { typeof(UInt16), "ushort" },
        };
        /// <summary>
        /// Returns a pretty name of a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetPrettyName(Type type)
        {
            return shorthandMap.ContainsKey(type) ? shorthandMap[type] : type.Name;
        }                
        /// <summary>
        /// Returns the default value of this type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDefaultValue<T>(){ return (T)GetDefaultValue(typeof(T)); }
        /// <summary>
        /// Returns the default value of this type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
		public static object GetDefaultValue(Type t)
		{
			if (t == null)
				return null;

			if (t.IsValueType)
				return Activator.CreateInstance(t);
			
			else if (t == typeof(string))
				return "";		

			return null;
		}


         public static bool ClassExists(string className)
        {
            var type = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from t in assembly.GetTypes()
                        where t.Name == className
                        select t).FirstOrDefault();
            return type != null;
        }
    }
}