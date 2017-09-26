using Puppy.DataTable.Models.Config.Column;
using System.Reflection;

namespace Puppy.DataTable.Attributes
{
    public class DataTableRowIdAttribute : DataTableAttributeBase
    {
        public bool EmitAsColumnName { get; set; }

        public override void ApplyTo(ColumnModel columnModel, PropertyInfo propertyInfo)
        {
            // This attribute does not affect rendering
        }

        public DataTableRowIdAttribute()
        {
            EmitAsColumnName = true;
        }
    }
}