#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Domain.ViewModels </Project>
//     <File>
//         <Name> ApiErrorViewModel </Name>
//         <Created> 12/04/2017 09:21:24 AM </Created>
//         <Key> 9626e038-c848-4955-8ce2-4f20e371d277 </Key>
//     </File>
//     <Summary>
//         ApiErrorViewModel
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using EnumsNET;
using System.Collections.Generic;
using TopCore.Auth.Domain.Exceptions;

namespace TopCore.Auth.Domain.ViewModels
{
    public class ApiErrorViewModel
    {
        public ApiErrorViewModel(ErrorCode code, string message = null)
        {
            Code = code;
            Message = string.IsNullOrWhiteSpace(message) ? code.AsString(EnumFormat.Description) : message;
            Module = code.AsString(EnumFormat.DisplayName); ;
        }

        public ErrorCode Code { get; set; }

        public string Message { get; set; }

        public string Module { get; set; }

        public List<(string PropertyName, object value)> ListPropertyValue { get; set; }
    }
}