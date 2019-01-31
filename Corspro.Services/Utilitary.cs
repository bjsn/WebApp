using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Corspro.Services
{
    public static class Utilitary
    {

        public static Boolean AreObjectsEquals(Object x, Object y)
        {
            if (x == null && y == null)
                return true;
            else if ((x == null && y != null) || (x != null && y == null))
                return false;

            Type tx = x.GetType();
            Type ty = y.GetType();

            if (tx != ty)
                return false;

            foreach (FieldInfo field in tx.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.FieldType.IsValueType && (field.GetValue(x).ToString() != field.GetValue(y).ToString()))
                    return false;
                else if (field.FieldType.IsClass && !AreObjectsEquals(field.GetValue(x), field.GetValue(y)))
                    return false;
            }

            return true;
        }

        public static string SafeToUpper(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value.ToUpper();
        }

    }
}
