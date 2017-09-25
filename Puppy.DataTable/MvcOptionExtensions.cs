using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Puppy.DataTable
{
    public static class MvcOptionExtensions
    {
        public static void UseHtmlEncodeModelBinding(this MvcOptions opts)
        {
            var binderToFind = opts.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(DataTablesModelBinderProvider));

            if (binderToFind != null) return;

            opts.ModelBinderProviders.Insert(0, new DataTablesModelBinderProvider());
        }
    }
}