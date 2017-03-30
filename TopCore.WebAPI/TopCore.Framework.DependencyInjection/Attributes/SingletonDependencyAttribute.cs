#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.DependencyInjection.Attributes </Project>
//     <File>
//         <Name> SingletonDependencyAttribute </Name>
//         <Created> 30 Mar 17 8:59:29 PM </Created>
//         <Key> ac1e0e6d-d7e7-4a0f-850c-1eab774526a1 </Key>
//     </File>
//     <Summary>
//         SingletonDependencyAttribute
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.Extensions.DependencyInjection;

namespace TopCore.Framework.DependencyInjection.Attributes
{
    public class SingletonDependencyAttribute : DependencyAttribute
    {
        public SingletonDependencyAttribute() : base(ServiceLifetime.Singleton)
        {
        }
    }
}