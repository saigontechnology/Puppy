using Puppy.DataTable.Models;
using System.Reflection;

namespace Puppy.DataTable.Attributes
{
    public class DataTablesRowIdAttribute : DataTablesAttributeBase
    {
        public bool EmitAsColumnName { get; set; }

        public override void ApplyTo(ColDefModel colDefModel, PropertyInfo pi)
        {
            // This attribute does not affect rendering
        }

        public DataTablesRowIdAttribute()
        {
            EmitAsColumnName = true;
        }
    }
}