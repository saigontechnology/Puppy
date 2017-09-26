using Puppy.DataTable.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Puppy.DataTable.Utils
{
    public class TransformTypeInfoHelper
    {
        public static Dictionary<string, object> MergeTransformValuesIntoDictionary<TInput, TTransform>(Func<TInput, TTransform> transformInput, TInput tInput)
        {
            // Get the the properties from the input as a dictionary
            var dict = DataTableTypeInfo<TInput>.ToDictionary(tInput);

            // Get the transform object
            var transform = transformInput(tInput);

            if (transform == null)
            {
                return dict;
            }

            foreach (var propertyInfo in transform.GetType().GetTypeInfo().GetProperties())
            {
                dict[propertyInfo.Name] = propertyInfo.GetValue(transform, null);
            }

            return dict;
        }
    }
}