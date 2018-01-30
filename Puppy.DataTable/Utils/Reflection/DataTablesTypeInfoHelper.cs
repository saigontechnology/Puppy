using Puppy.DataTable.Attributes;

namespace Puppy.DataTable.Utils.Reflection
{
    internal static class DataTablesTypeInfoHelper
    {
        public static string ToDisplayName(this DataTableAttribute attribute)
        {
            if (string.IsNullOrWhiteSpace(attribute.DisplayName) || (attribute.DisplayNameResourceType == null && DataTableGlobalConfig.SharedResourceType == null))
            {
                return attribute.DisplayName;
            }

            var value = ResourceHelper.GetResourceLookup<string>(attribute.DisplayNameResourceType, attribute.DisplayName);

            return value;
        }
    }
}