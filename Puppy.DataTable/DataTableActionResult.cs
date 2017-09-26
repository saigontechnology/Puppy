using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Models.Request;
using Puppy.DataTable.Models.Response;
using Puppy.DataTable.Processing.Response;
using Puppy.DataTable.Utils;
using Puppy.DataTable.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puppy.DataTable
{
    public abstract class DataTableActionResult : IActionResult
    {
        public abstract Task ExecuteResultAsync(ActionContext context);

        /// <typeparam name="TSource"></typeparam>
        /// <param name="responseData">  
        ///     The properties of this can be marked up with [DataTablesAttribute] to control sorting/searchability/visibility
        /// </param>
        /// <param name="transform">     
        ///     // a transform for custom column rendering e.g. to do a custom date row =&gt; new {
        ///     CreatedDate = row.CreatedDate.ToString("dd MM yy") }
        /// </param>
        /// <param name="responseOption"></param>
        /// <returns></returns>
        public static DataTableActionResult<TSource> Create<TSource>(DataTableResponseDataModel responseData, Func<TSource, object> transform, ResponseOptionModel<TSource> responseOption = null)
        {
            transform = transform ?? (s => s);

            var result = new DataTableActionResult<TSource>(responseData);

            result.Data =
                result
                    .Data
                    .Transform<TSource, Dictionary<string, object>>
                    (
                        row => TransformTypeInfoHelper.MergeTransformValuesIntoDictionary(transform, row)
                    )
                    .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRules(result.Data, responseOption);

            return result;
        }

        public static DataTableActionResult<TSource> Create<TSource>(DataTableResponseDataModel responseData, ResponseOptionModel<TSource> responseOption = null)
        {
            var result = new DataTableActionResult<TSource>(responseData);

            var dictionaryTransform = DataTableTypeInfo<TSource>.ToDictionary(responseOption);

            result.Data =
                result
                    .Data
                    .Transform(dictionaryTransform)
                    .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRules(result.Data, responseOption);

            return result;
        }

        private static DataTableResponseDataModel ApplyOutputRules<TSource>(DataTableResponseDataModel responseData, ResponseOptionModel<TSource> responseOption)
        {
            responseOption = responseOption
                             ?? new ResponseOptionModel<TSource>
                             {
                                 ArrayOutputType = ArrayOutputType.BiDimensionalArray
                             };

            DataTableResponseDataModel outputData = responseData;

            switch (responseOption.ArrayOutputType)
            {
                case ArrayOutputType.ArrayOfObjects:
                    {
                        // Nothing is needed
                        break;
                    }
                default:
                    outputData = responseData.Transform<Dictionary<string, object>, object[]>(d => d.Values.ToArray());
                    break;
            }

            return outputData;
        }
    }

    public class DataTableActionResult<TSource> : DataTableActionResult
    {
        public DataTableResponseDataModel Data { get; set; }

        public DataTableActionResult(IQueryable<TSource> queryable, DataTableParamModel paramModel)
        {
            Data = queryable.GetDataTableResponse(paramModel);
        }

        public DataTableActionResult(DataTableResponseDataModel data)
        {
            Data = data;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            HttpResponse response = context.HttpContext.Response;

            return response.WriteAsync(JsonConvert.SerializeObject(Data));
        }
    }
}