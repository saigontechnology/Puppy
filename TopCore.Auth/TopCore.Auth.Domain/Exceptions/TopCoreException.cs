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

namespace TopCore.Auth.Domain.Exceptions
{
    public class TopCoreException : Exception
    {
        public TopCoreException(ErrorCode code, string message = null) : base(message)
        {
            Code = code;
        }

        public TopCoreException(ErrorCode code, params (string PropertyName, object value)[] arrayPropertyValue)
        {
            Code = code;
            ArrayPropertyValue = arrayPropertyValue;
        }

        public TopCoreException(ErrorCode code, string message, params (string PropertyName, object value)[] arrayPropertyValue) : base(message)
        {
            Code = code;
            ArrayPropertyValue = arrayPropertyValue;
        }

        public TopCoreException(ErrorCode code, string message, Exception innerException, params (string PropertyName, object value)[] arrayPropertyValue) : base(message, innerException)
        {
            Code = code;
            ArrayPropertyValue = arrayPropertyValue;
        }

        public ErrorCode Code { get; }

        public (string PropertyName, object value)[] ArrayPropertyValue { get; }
    }
}