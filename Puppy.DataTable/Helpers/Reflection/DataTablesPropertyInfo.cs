using Puppy.DataTable.Attributes;
using System;

namespace Puppy.DataTable.Helpers.Reflection
{
    public class DataTablesPropertyInfo
    {
        public DataTablesPropertyInfo(System.Reflection.PropertyInfo propertyInfo, DataTablesAttributeBase[] attributes)
        {
            PropertyInfo = propertyInfo;
            Attributes = attributes;
        }

        public System.Reflection.PropertyInfo PropertyInfo { get; }

        public DataTablesAttributeBase[] Attributes { get; }

        public Type Type => PropertyInfo.PropertyType;
    }
}