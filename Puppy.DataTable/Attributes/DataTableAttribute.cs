using Puppy.DataTable.Constants;
using Puppy.DataTable.Models;
using Puppy.DataTable.Utils.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace Puppy.DataTable.Attributes
{
    public class DataTableAttribute : DataTableAttributeBase
    {
        public string DisplayName { get; set; }

        public bool IsSearchable { get; set; } = true;

        public bool IsSortable { get; set; } = true;

        public int Order { get; set; } = int.MaxValue;

        public Type DisplayNameResourceType { get; set; }

        public SortDirection SortDirection { get; set; } = SortDirection.None;

        public string MRenderFunction { get; set; }

        public string CssClass { get; set; }

        public string CssClassHeader { get; set; }

        public string Width { get; set; }

        public bool IsVisible { get; set; } = true;

        public override void ApplyTo(ColDefModel colDefModel, PropertyInfo propertyInfo)
        {
            colDefModel.DisplayName = this.ToDisplayName() ?? colDefModel.Name;
            colDefModel.IsSortable = IsSortable;
            colDefModel.IsVisible = IsVisible;
            colDefModel.IsSearchable = IsSearchable;
            colDefModel.SortDirection = SortDirection;
            colDefModel.MRenderFunction = MRenderFunction;
            colDefModel.CssClass = CssClass;
            colDefModel.CssClassHeader = CssClassHeader;
            colDefModel.CustomAttributes = propertyInfo.GetCustomAttributes().ToArray();
            colDefModel.Width = Width;
        }
    }
}