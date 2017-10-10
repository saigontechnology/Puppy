#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> MultipleResponsesOperationFilter.cs </Name>
//         <Created> 10/10/17 10:32:15 AM </Created>
//         <Key> 9a050e66-9a68-4be0-864a-f6ec68f713f3 </Key>
//     </File>
//     <Summary>
//         MultipleResponsesOperationFilter.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Puppy.Swagger.Filters
{
    /// <summary>
    ///     Enable multiple SwaggerResponseAttribute with same <see cref="Microsoft.AspNetCore.Http.StatusCodes" /> and Support
    ///     to create and use custom attribute inheritance <see cref="SwaggerResponseAttribute" />
    /// </summary>
    public class MultipleResponsesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var responseAttrs =
                context
                    .ApiDescription
                    .ActionDescriptor
                    .FilterDescriptors
                    .Where(x => x.Filter is SwaggerResponseAttribute)
                    .Select(x => x.Filter as SwaggerResponseAttribute)
                    .GroupBy(x => x.StatusCode);

            foreach (var grouping in responseAttrs)
            {
                var existResponse = operation.Responses.First(x => x.Key == grouping.Key.ToString());
                operation.Responses.Remove(existResponse);

                foreach (var responseAttribute in grouping)
                {
                    var response = new Response
                    {
                        Description = responseAttribute.Description,
                        Examples = existResponse.Value.Examples,
                        Headers = existResponse.Value.Headers,
                        Schema = existResponse.Value.Schema
                    };

                    var key = grouping.Key.ToString();

                    var index = 2;

                    while (operation.Responses.ContainsKey(key))
                    {
                        key = $"{grouping.Key} ({index})";
                        index++;
                    }

                    operation.Responses.Add(key, response);
                }
            }
        }
    }
}