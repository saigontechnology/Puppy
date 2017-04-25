#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Topcore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Topcore → Interface </Project>
//     <File>
//         <Name> IRepository.cs </Name>
//         <Created> 23 Apr 17 11:06:31 PM </Created>
//         <Key> ec5b64d7-7c48-4f24-be4f-1ce91574429a </Key>
//     </File>
//     <Summary>
//         IRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Framework.EF.Interfaces;

namespace TopCore.Auth.Domain.Interfaces.Data
{
    public interface IRepository<T> : IBaseRepository<T> where T : class
    {
    }
}