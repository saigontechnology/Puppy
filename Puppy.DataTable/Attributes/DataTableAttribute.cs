using Puppy.DataTable.Constants;
using Puppy.DataTable.Models.Config.Column;
using Puppy.DataTable.Utils.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace Puppy.DataTable.Attributes
{
    public class DataTableAttribute : DataTableAttributeBase
    {
        public string DisplayName { get; set; }

        public bool IsVisible { get; set; } = true;

        public bool IsSearchable { get; set; } = true;

        public bool IsSortable { get; set; } = true;

        public int Order { get; set; } = int.MaxValue;

        public Type DisplayNameResourceType { get; set; }

        public SortDirection SortDirection { get; set; } = SortDirection.None;

        public string MRenderFunction { get; set; }

        public string CssClass { get; set; }

        public string CssClassHeader { get; set; }

        public string Width { get; set; }

        /// <summary>
        ///     Additional HTML Element attributes for header column 
        /// </summary>
        /// <remarks> Ex: "data-toggle='tooltip' data-original-title='Tooltip Title'" </remarks>
        public string AdditionalAttributeHeader { get; set; }

        public override void ApplyTo(ColumnModel columnModel, PropertyInfo propertyInfo)
        {
            columnModel.DisplayName = this.ToDisplayName() ?? columnModel.Name;
            columnModel.IsSortable = IsSortable;
            columnModel.IsVisible = IsVisible;
            columnModel.IsSearchable = IsSearchable;
            columnModel.SortDirection = SortDirection;
            columnModel.MRenderFunction = MRenderFunction;
            columnModel.CssClass = CssClass;
            columnModel.CssClassHeader = CssClassHeader;
            columnModel.CustomAttributes = propertyInfo.GetCustomAttributes().ToArray();
            columnModel.Width = Width;
            columnModel.AdditionalAttributeHeader = AdditionalAttributeHeader;
        }
    }
}