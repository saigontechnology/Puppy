#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.DependencyInjection.Exceptions </Project>
//     <File>
//         <Name> AlreadyLoaddedAssemblyException </Name>
//         <Created> 02 Apr 17 1:51:39 AM </Created>
//         <Key> f75ed082-865f-4dc2-aaeb-de1f5724e263 </Key>
//     </File>
//     <Summary>
//         AlreadyLoaddedAssemblyException
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace TopCore.Framework.DependencyInjection.Exceptions
{
    public class ConflictRegisterException : Exception
    {
        public ConflictRegisterException(string message) : base(message)
        {
        }
    }
}