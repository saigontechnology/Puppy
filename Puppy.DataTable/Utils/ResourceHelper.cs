using System;
using System.Reflection;

namespace Puppy.DataTable.Utils
{
    public class ResourceHelper
    {
        public static T GetResourceLookup<T>(Type resourceType, string resourceName)
        {
            resourceType = resourceType ?? DataTableGlobalConfig.SharedResourceType;

            if (resourceType == null || resourceName == null)
            {
                return default(T);
            }

            var property = resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

            if (property == null)
            {
                return default(T);
            }

            return (T)property.GetValue(null, null);
        }
    }
}