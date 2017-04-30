#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.DependencyInjection.Attributes </Project>
//     <File>
//         <Name> PerResolveDependencyAttribute </Name>
//         <Created> 30 Mar 17 8:58:59 PM </Created>
//         <Key> 4576960b-190a-40d7-89ae-a436590805f2 </Key>
//     </File>
//     <Summary>
//         PerResolveDependencyAttribute
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.Extensions.DependencyInjection;

namespace TopCore.Framework.DependencyInjection.Attributes
{
	public class PerResolveDependencyAttribute : DependencyAttribute
	{
		public PerResolveDependencyAttribute() : base(ServiceLifetime.Transient)
		{
		}
	}
}