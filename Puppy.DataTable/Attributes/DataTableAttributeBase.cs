using Puppy.DataTable.Models;
using System;

namespace Puppy.DataTable.Attributes
{
    public abstract class DataTableAttributeBase : Attribute
    {
        public abstract void ApplyTo(ColDefModel colDefModel, System.Reflection.PropertyInfo propertyInfo);
    }
}