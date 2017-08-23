using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Puppy.Web.Models.Api
{
    public class FormViewModel : CollectionViewModel<FormFieldViewModel>
    {
        private static readonly IReadOnlyDictionary<Type, string> TypeLookup = new Dictionary<Type, string>
        {
            {typeof(string), "string"},
            {typeof(bool), "boolean"},
            {typeof(DateTimeOffset), "datetime"},
            {typeof(TimeSpan), "duration"},
            {typeof(int), "integer"}
        };

        public FormViewModel(string path, string method, string relation, IEnumerable<FormFieldViewModel> fields)
        {
            Meta = new PlaceholderLinkViewModel
            {
                Href = path,
                Method = method,
                Relations = new[] { relation }
            };

            Items = fields.ToArray();
        }

        public static FormViewModel FromModel<T>(string path, string method, string relation)
            where T : class, new()
        {
            var fields = typeof(T).GetTypeInfo()
                .DeclaredProperties.Select(p =>
                {
                    var attributes = p.GetCustomAttributes().ToArray();

                    return new FormFieldViewModel
                    {
                        Name = attributes.OfType<DisplayAttribute>().FirstOrDefault()?.Name ?? p.Name,
                        Required = attributes.OfType<RequiredAttribute>().Any(),
                        MaxLength = attributes.OfType<MaxLengthAttribute>().FirstOrDefault()?.Length,
                        MinLength = attributes.OfType<MinLengthAttribute>().FirstOrDefault()?.Length,
                        Type = GetType(p.PropertyType)
                    };
                });

            return new FormViewModel(path, method, relation, fields);
        }

        private static string GetType(Type type)
        {
            var nullableUnderlying = Nullable.GetUnderlyingType(type);
            if (nullableUnderlying != null)
                return GetType(nullableUnderlying);

            if (type.IsArray)
                return "array";

            string typeString;
            TypeLookup.TryGetValue(type, out typeString);
            return typeString;
        }
    }
}