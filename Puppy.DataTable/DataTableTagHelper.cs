using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Puppy.DataTable.Models;
using Puppy.DataTable.Utils.Reflection;
using Puppy.Web;

namespace Puppy.DataTable
{
    public static class DataTableTagHelper
    {
        public static DataTableConfigModel DataTableModel<TController, TResult>(this IHtmlHelper html, string id,
            Expression<Func<TController, DataTableActionResult<TResult>>> exp, params ColDefModel[] columns)
        {
            if (columns?.Any() != true)
            {
                columns = typeof(TResult).ColDefs();
            }

            var mi = exp.MethodInfo();
            var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller", StringComparison.CurrentCultureIgnoreCase));
            var urlHelper = new UrlHelper(html.ViewContext);
            var ajaxUrl = urlHelper.AbsoluteAction(mi.Name, controllerName);
            return new DataTableConfigModel(id, ajaxUrl, columns);
        }

        public static DataTableConfigModel DataTableModel(this IHtmlHelper html, Type t, string id, string uri)
        {
            return new DataTableConfigModel(id, uri, t.ColDefs());
        }

        public static DataTableConfigModel DataTableModel<T>(string id, string uri)
        {
            return new DataTableConfigModel(id, uri, typeof(T).ColDefs());
        }

        public static DataTableConfigModel DataTableModel<TResult>(this IHtmlHelper html, string id, string uri)
        {
            return DataTableModel(html, typeof(TResult), id, uri);
        }

        public static DataTableConfigModel DataTableModel(this IHtmlHelper html, string id, string ajaxUrl, params string[] columns)
        {
            var columnDefs = columns.Select(c => new ColDefModel(c, typeof(string))).ToArray();
            return new DataTableConfigModel(id, ajaxUrl, columnDefs);
        }
    }
}