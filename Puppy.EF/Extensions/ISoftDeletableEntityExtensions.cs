#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ISoftDeletableEntityExtensions.cs </Name>
//         <Created> 25/08/17 1:06:00 AM </Created>
//         <Key> 466d138e-5754-4a36-b7c9-b47a12f27f19 </Key>
//     </File>
//     <Summary>
//         ISoftDeletableEntityExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.EF.Interfaces.Entity;

namespace Puppy.EF.Extensions
{
    public static class ISoftDeletableEntityExtensions
    {
        public static bool IsDeleted<T>(this T iSoftDeletableEntity) where T : class, ISoftDeletableEntity
        {
            return iSoftDeletableEntity.DeletedTime != null;
        }
    }
}