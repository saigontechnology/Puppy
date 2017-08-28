#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Enums.cs </Name>
//         <Created> 28/08/17 11:57:06 AM </Created>
//         <Key> 6b96b366-9e37-4952-9a40-6c2bdfa64fb8 </Key>
//     </File>
//     <Summary>
//         Enums.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.AutoMapper
{
    public class Enums
    {
        public enum MapperResolveType
        {
            PerRequest = 1,
            PreResolve = 2,
            Singleton = 3
        }
    }
}