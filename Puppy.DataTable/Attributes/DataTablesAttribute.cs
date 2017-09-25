using Puppy.DataTable.Constants;
using Puppy.DataTable.Helpers.Reflection;
using Puppy.DataTable.Models;
using System;
using System.Linq;
using System.Reflection;

namespace Puppy.DataTable.Attributes
{
    public class DataTablesAttribute : DataTablesAttributeBase
    {
        public string DisplayName { get; set; }

        public bool IsSearchable { get; set; } = true;

        public bool IsSortable { get; set; } = true;

        public int Order { get; set; } = int.MaxValue;

        public Type DisplayNameResourceType { get; set; }

        public SortDirection SortDirection { get; set; } = SortDirection.None;

        public string MRenderFunction { get; set; }

        public String CssClass { get; set; }

        public String CssClassHeader { get; set; }

        public string Width { get; set; }

        public bool IsVisible { get; set; } = true;

        public override void ApplyTo(ColDefModel colDefModel, PropertyInfo pi)
        {
            colDefModel.DisplayName = this.ToDisplayName() ?? colDefModel.Name;
            colDefModel.IsSortable = IsSortable;
            colDefModel.IsVisible = IsVisible;
            colDefModel.IsSearchable = IsSearchable;
            colDefModel.SortDirection = SortDirection;
            colDefModel.MRenderFunction = MRenderFunction;
            colDefModel.CssClass = CssClass;
            colDefModel.CssClassHeader = CssClassHeader;
            colDefModel.CustomAttributes = pi.GetCustomAttributes().ToArray();
            colDefModel.Width = Width;
        }
    }
}