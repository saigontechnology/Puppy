using Puppy.DataTable.Attributes;
using System;

namespace Puppy.DataTable.Utils.Reflection
{
    public class DataTablePropertyInfo
    {
        public DataTablePropertyInfo(System.Reflection.PropertyInfo propertyInfo, DataTableAttributeBase[] attributes)
        {
            PropertyInfo = propertyInfo;
            Attributes = attributes;
        }

        public System.Reflection.PropertyInfo PropertyInfo { get; }

        public DataTableAttributeBase[] Attributes { get; }

        public Type Type => PropertyInfo.PropertyType;
    }
}