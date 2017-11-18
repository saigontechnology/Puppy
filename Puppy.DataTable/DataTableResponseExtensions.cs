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

using Puppy.DataTable.Models.Request;
using Puppy.DataTable.Models.Response;
using Puppy.DataTable.Utils;
using Puppy.DataTable.Utils.Reflection;
using System;
using System.Linq;

namespace Puppy.DataTable
{
    public static class DataTableResponseExtensions
    {
        public static DataTableResponseDataModel<T> GetDataTableResponse<T>(this IQueryable<T> data, DataTableParamModel dataTableParamModel) where T : class, new()
        {
            var totalRecords = data.Count(); // annoying this, as it causes an extra evaluation..

            var filters = new DataTableFiltering();

            var outputProperties = DataTableTypeInfo<T>.Properties;

            var filteredData = filters.ApplyFiltersAndSort(dataTableParamModel, data, outputProperties);

            var totalDisplayRecords = filteredData.Count();

            var skipped = filteredData.Skip(dataTableParamModel.DisplayStart);

            var page = (dataTableParamModel.DisplayLength <= 0 ? skipped : skipped.Take(dataTableParamModel.DisplayLength)).ToArray();

            var result = new DataTableResponseDataModel<T>
            {
                TotalRecord = totalRecords,
                TotalDisplayRecord = totalDisplayRecords,
                Echo = dataTableParamModel.Echo,
                Data = page.Cast<object>().ToArray()
            };

            return result;
        }

        public static DataTableActionResult<T> GetDataTableActionResult<T>(this DataTableResponseDataModel<T> responseData, Func<T, object> transform, ResponseOptionModel<T> responseOption = null) where T : class, new()
        {
            return DataTableActionResult.Create(responseData, transform, responseOption);
        }

        public static DataTableActionResult<T> GetDataTableActionResult<T>(this DataTableResponseDataModel<T> responseData, ResponseOptionModel<T> responseOption = null) where T : class, new()
        {
            return DataTableActionResult.Create(responseData, responseOption);
        }
    }
}