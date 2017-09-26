using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Puppy.DataTable.Utils
{
    public class FilterDef : Hashtable
    {
        internal object[] values { set => this["values"] = value; }

        internal string type { set => this["type"] = value; }

        public FilterDef(Type t)
        {
            SetDefaultValuesAccordingToColumnType(t);
        }

        private static readonly List<Type> DateTypes = new List<Type> { typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?) };

        private void SetDefaultValuesAccordingToColumnType(Type t)
        {
            if (t == null)
            {
                Remove("type");
            }
            else if (DateTypes.Contains(t))
            {
                type = "date-range";
            }
            else if (t == typeof(bool))
            {
                type = "select";
                values = new object[] { "True", "False" };
            }
            else if (t == typeof(bool?))
            {
                type = "select";
                values = new object[] { "True", "False", "null" };
            }
            else if (t.GetTypeInfo().IsEnum)
            {
                type = "checkbox";
                values = t.EnumValLabPairs();
            }
            else
            {
                type = "text";
            }
        }
    }
}