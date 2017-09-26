#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DataTableResponseHelper.cs </Name>
//         <Created> 26/09/17 10:50:54 AM </Created>
//         <Key> 7a8717f3-fc05-46a1-a0a1-580968bc9485 </Key>
//     </File>
//     <Summary>
//         DataTableResponseHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Linq;
using Puppy.DataTable.Models;
using Puppy.DataTable.Utils;
using Puppy.DataTable.Utils.Reflection;

namespace Puppy.DataTable
{
    public static class DataTableResponseExtensions
    {
        public static DataTableResponseDataModel GetDataTableResponse<TSource>(this IQueryable<TSource> data, DataTableParamModel dataTableParamModel)
        {
            var totalRecords = data.Count(); // annoying this, as it causes an extra evaluation..

            var filters = new DataTableFiltering();

            var outputProperties = DataTablesTypeInfo<TSource>.Properties;

            var filteredData = filters.ApplyFiltersAndSort(dataTableParamModel, data, outputProperties);

            var totalDisplayRecords = filteredData.Count();

            var skipped = filteredData.Skip(dataTableParamModel.iDisplayStart);

            var page = (dataTableParamModel.iDisplayLength <= 0 ? skipped : skipped.Take(dataTableParamModel.iDisplayLength)).ToArray();

            var result = new DataTableResponseDataModel
            {
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalDisplayRecords,
                sEcho = dataTableParamModel.sEcho,
                aaData = page.Cast<object>().ToArray()
            };

            return result;
        }

        public static DataTableActionResult<TSource> GetDataTableActionResult<TSource>(this DataTableResponseDataModel responseData, Func<TSource, object> transform, ResponseOptionModel<TSource> responseOption = null)
        {
            return DataTableActionResult.Create(responseData, transform, responseOption);
        }

        public static DataTableActionResult<TSource> GetDataTableActionResult<TSource>(this DataTableResponseDataModel responseData, ResponseOptionModel<TSource> responseOption = null)
        {
            return DataTableActionResult.Create(responseData, responseOption);
        }
    }
}