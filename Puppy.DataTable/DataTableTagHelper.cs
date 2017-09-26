using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Puppy.DataTable.Models.Config;
using Puppy.DataTable.Models.Config.Column;
using Puppy.DataTable.Utils.Extensions;
using Puppy.DataTable.Utils.Reflection;
using Puppy.Web;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Puppy.DataTable
{
    public static class DataTableTagHelper
    {
        public static DataTableModel DataTableModel<TController, TResult>(
            this IHtmlHelper html,
            string id,
            Expression<Func<TController, DataTableActionResult<TResult>>> exp,
            params ColumnModel[] columns)
        {
            if (columns?.Any() != true)
            {
                columns = typeof(TResult).GetColumns();
            }

            var methodInfo = exp.MethodInfo();

            var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf(nameof(Controller), StringComparison.CurrentCultureIgnoreCase));

            var urlHelper = new UrlHelper(html.ViewContext);
            var ajaxUrl = urlHelper.AbsoluteAction(methodInfo.Name, controllerName);
            return new DataTableModel(id, ajaxUrl, columns);
        }

        public static DataTableModel DataTableModel(this IHtmlHelper html, Type t, string id, string uri)
        {
            return new DataTableModel(id, uri, t.GetColumns());
        }

        public static DataTableModel DataTableModel<T>(string id, string uri)
        {
            return new DataTableModel(id, uri, typeof(T).GetColumns());
        }

        public static DataTableModel DataTableModel<TResult>(this IHtmlHelper html, string id, string uri)
        {
            return DataTableModel(html, typeof(TResult), id, uri);
        }

        public static DataTableModel DataTableModel(this IHtmlHelper html, string id, string ajaxUrl, params string[] columns)
        {
            var columnDefs = columns.Select(c => new ColumnModel(c, typeof(string))).ToArray();
            return new DataTableModel(id, ajaxUrl, columnDefs);
        }
    }
}