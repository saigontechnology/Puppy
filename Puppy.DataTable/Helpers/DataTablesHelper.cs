using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Puppy.DataTable.Helpers.Reflection;
using Puppy.DataTable.Models;
using Puppy.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Puppy.DataTable.Helpers
{
    public static class DataTablesHelper
    {
        public static DataTableConfigModel DataTableModel<TController, TResult>(this IHtmlHelper html, string id,
            Expression<Func<TController, DataTablesResult<TResult>>> exp, IEnumerable<ColDefModel> columns = null)
        {
            if (columns?.Any() != null)
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
            return new DataTableConfigModel(id, ajaxUrl, columns.Select(c => new ColDefModel(c, typeof(string))));
        }
    }
}