#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ObjectExtensions.cs </Name>
//         <Created> 24/08/17 1:26:59 PM </Created>
//         <Key> ea4c74e4-ea68-44b1-8908-b2c4db4ce477 </Key>
//     </File>
//     <Summary>
//         ObjectExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using AutoMapper;

namespace Puppy.AutoMapper
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Converts an object to another using AutoMapper library. Creates a new object of
        ///     <typeparamref name="TDestination" />. There must be a mapping between objects before
        ///     calling this method.
        /// </summary>
        /// <typeparam name="TDestination"> Type of the destination object </typeparam>
        /// <param name="source"> Source object </param>
        public static TDestination MapTo<TDestination>(this object source) where TDestination : class, new()
        {
            return Mapper.Map<TDestination>(source);
        }

        /// <summary>
        ///     Execute a mapping from the source object to the existing destination object There
        ///     must be a mapping between objects before calling this method.
        /// </summary>
        /// <typeparam name="TSource"> Source type </typeparam>
        /// <typeparam name="TDestination"> Destination type </typeparam>
        /// <param name="source">      Source object </param>
        /// <param name="destination"> Destination object </param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) where TDestination : class, new()
        {
            return Mapper.Map(source, destination);
        }
    }
}