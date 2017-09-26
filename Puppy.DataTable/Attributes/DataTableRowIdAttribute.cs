using Puppy.DataTable.Models;
using System.Reflection;

namespace Puppy.DataTable.Attributes
{
    public class DataTableRowIdAttribute : DataTableAttributeBase
    {
        public bool EmitAsColumnName { get; set; }

        public override void ApplyTo(ColDefModel colDefModel, PropertyInfo propertyInfo)
        {
            // This attribute does not affect rendering
        }

        public DataTableRowIdAttribute()
        {
            EmitAsColumnName = true;
        }
    }
}