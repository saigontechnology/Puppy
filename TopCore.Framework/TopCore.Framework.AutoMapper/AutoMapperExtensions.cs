#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> AutoMapperExtensions.cs </Name>
//         <Created> 24 Apr 17 1:25:34 AM </Created>
//         <Key> ef316320-0a7a-4999-b90f-543679175c88 </Key>
//     </File>
//     <Summary>
//         AutoMapperExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System.Reflection;
using AutoMapper;

namespace TopCore.Framework.AutoMapper
{
	public static class AutoMapperExtensions
	{
		public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(
			this IMappingExpression<TSource, TDestination> expression)
		{
			var flags = BindingFlags.Public | BindingFlags.Instance;
			var sourceType = typeof(TSource);
			var destinationProperties = typeof(TDestination).GetProperties(flags);

			foreach (var property in destinationProperties)
				if (sourceType.GetProperty(property.Name, flags) == null)
					expression.ForMember(property.Name, opt => opt.Ignore());
			return expression;
		}
	}
}