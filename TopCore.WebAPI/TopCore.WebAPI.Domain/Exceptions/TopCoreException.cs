#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Domain.Exceptions </Project>
//     <File>
//         <Name> TopCoreException </Name>
//         <Created> 12/04/2017 09:19:27 AM </Created>
//         <Key> 81483575-3439-4eb8-b5ff-c7257bd731c6 </Key>
//     </File>
//     <Summary>
//         TopCoreException
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using TopCore.WebAPI.Domain.Exceptions;

namespace TopCore.Auth.Domain.Exceptions
{
    public class TopCoreException : Exception
    {
        public ErrorCode Code { get; }

        public TopCoreException(ErrorCode code)
        {
            Code = code;
        }

        public TopCoreException(ErrorCode code, string message) : base(message)
        {
            Code = code;
        }

        public TopCoreException(ErrorCode code, string message, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }
    }
}