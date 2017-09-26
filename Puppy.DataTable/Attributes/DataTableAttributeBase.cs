using Puppy.DataTable.Models.Config.Column;
using System;
using System.Reflection;

namespace Puppy.DataTable.Attributes
{
    public abstract class DataTableAttributeBase : Attribute
    {
        public abstract void ApplyTo(ColumnModel columnModel, PropertyInfo propertyInfo);
    }
}