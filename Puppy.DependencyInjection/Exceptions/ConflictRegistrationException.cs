#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> Puppy.DependencyInjection.Exceptions </Project>
//     <File>
//         <Name> ConflictRegistrationException </Name>
//         <Created> 02 Apr 17 1:51:39 AM </Created>
//         <Key> f75ed082-865f-4dc2-aaeb-de1f5724e263 </Key>
//     </File>
//     <Summary>
//         ConflictRegistrationException
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace Puppy.DependencyInjection.Exceptions
{
    public class ConflictRegistrationException : Exception
    {
        public ConflictRegistrationException(string message) : base(message)
        {
        }
    }
}