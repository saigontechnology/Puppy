#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> Puppy.DependencyInjection.Attributes </Project>
//     <File>
//         <Name> PerRequestDependencyAttribute </Name>
//         <Created> 30 Mar 17 8:57:50 PM </Created>
//         <Key> dca5ebc8-61d4-414f-9696-36e27508dddb </Key>
//     </File>
//     <Summary>
//         PerRequestDependencyAttribute
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.Extensions.DependencyInjection;

namespace Puppy.DependencyInjection.Attributes
{
    public class PerRequestDependencyAttribute : DependencyAttribute
    {
        public PerRequestDependencyAttribute() : base(ServiceLifetime.Scoped)
        {
        }
    }
}