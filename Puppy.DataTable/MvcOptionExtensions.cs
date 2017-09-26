using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Puppy.DataTable
{
    public static class MvcOptionExtensions
    {
        public static void AddDataTableModelBinderProvider(this MvcOptions opts)
        {
            var isProviderAdded = opts.ModelBinderProviders.Any(x => x.GetType() == typeof(DataTablesModelBinderProvider));

            if (isProviderAdded) return;

            opts.ModelBinderProviders.Insert(0, new DataTablesModelBinderProvider());
        }
    }
}