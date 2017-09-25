using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Helpers.Reflection;
using Puppy.DataTable.Models;
using Puppy.DataTable.Processing;
using Puppy.DataTable.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Puppy.DataTable
{
    public abstract class DataTablesResult : IActionResult
    {
        public abstract Task ExecuteResultAsync(ActionContext context);

        /// <typeparam name="TSource"></typeparam>
        /// <param name="q">                  
        ///     A queryable for the data. The properties of this can be marked up with
        ///     [DataTablesAttribute] to control sorting/searchability/visibility
        /// </param>
        /// <param name="dataTableParamModel"></param>
        /// <param name="transform">          
        ///     //a transform for custom column rendering e.g. to do a custom date row =&gt; new {
        ///     CreatedDate = row.CreatedDate.ToString("dd MM yy") }
        /// </param>
        /// <param name="responseOptions">    </param>
        /// <returns></returns>
        public static DataTablesResult<TSource> Create<TSource>(IQueryable<TSource> q, DataTableParamModel dataTableParamModel,
            Func<TSource, object> transform, ResponseOptionModel<TSource> responseOptions = null)
        {
            transform = transform ?? (s => s);
            var result = new DataTablesResult<TSource>(q, dataTableParamModel);

            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(row => TransformTypeInfo.MergeTransformValuesIntoDictionary(transform, row))
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRules(result.Data, responseOptions);

            return result;
        }

        public static DataTablesResult<TSource> Create<TSource>(IQueryable<TSource> q, DataTableParamModel dataTableParamModel,
            ResponseOptionModel<TSource> responseOptions = null)
        {
            var result = new DataTablesResult<TSource>(q, dataTableParamModel);

            var dictionaryTransform = DataTablesTypeInfo<TSource>.ToDictionary(responseOptions);
            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(dictionaryTransform)
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRules(result.Data, responseOptions);

            return result;
        }

        private static DataTablesResponseDataModel ApplyOutputRules<TSource>(DataTablesResponseDataModel sourceData, ResponseOptionModel<TSource> responseOptions)
        {
            responseOptions = responseOptions ?? new ResponseOptionModel<TSource>() { ArrayOutputType = ArrayOutputType.BiDimensionalArray };
            DataTablesResponseDataModel outputData = sourceData;

            switch (responseOptions.ArrayOutputType)
            {
                case ArrayOutputType.ArrayOfObjects:
                    // Nothing is needed
                    break;

                case ArrayOutputType.BiDimensionalArray:
                default:
                    outputData = sourceData.Transform<Dictionary<string, object>, object[]>(d => d.Values.ToArray());
                    break;
            }

            return outputData;
        }

        /// <param name="transform">Should be a Func<T, TTransform></param>
        public static DataTablesResult Create(IQueryable queryable, DataTableParamModel dataTableParamModel, object transform,
            ResponseOptionModel responseOptions = null)
        {
            var s = "Create";
            var openCreateMethod = typeof(DataTablesResult).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 1);
            var queryableType = queryable.GetType().GetGenericArguments()[0];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType, typeof(object));
            return (DataTablesResult)closedCreateMethod.Invoke(null, new object[] { queryable, dataTableParamModel, transform, responseOptions });
        }
    }

    public class DataTablesResult<TSource> : DataTablesResult
    {
        public DataTablesResponseDataModel Data { get; set; }

        public DataTablesResult(IQueryable<TSource> q, DataTableParamModel dataTableParamModel)
        {
            this.Data = dataTableParamModel.GetDataTablesResponse(q);
        }

        public DataTablesResult(DataTablesResponseDataModel data)
        {
            this.Data = data;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            HttpResponse response = context.HttpContext.Response;

            return response.WriteAsync(JsonConvert.SerializeObject(this.Data));
        }
    }
}