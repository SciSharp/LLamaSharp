using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LLama.Experimental.Utils
{
    internal static class ClassStringFormatter
    {
        public static string Format<T>(T obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            string res = $"{type.Name}(";
            foreach (var property in properties)
            {
                object? value = property.GetValue(obj);
                res += $"\n  {property.Name} = {value},";
            }
            if(properties.Length == 0)
            {
                res += ")";
            }
            else
            {
                res += "\n)";
            }
            return res;
        }
    }
}
