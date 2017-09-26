using Newtonsoft.Json.Linq;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Utils;
using System;

namespace Puppy.DataTable.Models
{
    public class ColDefModel
    {
        public ColDefModel(string name, Type type)
        {
            Name = name;
            DisplayName = name;
            Type = type;
            Filter = new FilterDef(Type);
        }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool IsVisible { get; set; } = true;

        public bool IsSortable { get; set; } = true;

        public Type Type { get; set; }

        public bool IsSearchable { get; set; } = true;

        public string CssClass { get; set; } = string.Empty;

        public string CssClassHeader { get; set; } = string.Empty;

        public SortDirection SortDirection { get; set; } = SortDirection.None;

        /// <summary>
        ///     Define javascript function name to render data - mRender function name in JQuery DataTable)
        /// </summary>
        /// <remarks>
        ///     Function follow params: function &lt;your function name&gt;(data, type, row). Param
        ///     row: is a array column data
        /// </remarks>
        public string MRenderFunction { get; set; }

        public FilterDef Filter { get; set; }

        public JObject SearchCols { get; set; }

        public Attribute[] CustomAttributes { get; set; }

        public string Width { get; set; }
    }
}