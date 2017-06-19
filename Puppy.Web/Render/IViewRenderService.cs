#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IViewRenderService.cs </Name>
//         <Created> 15/06/2017 10:44:39 PM </Created>
//         <Key> be7388e8-95a8-486a-b452-9d6bd612ac47 </Key>
//     </File>
//     <Summary>
//         IViewRenderService.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Threading.Tasks;

namespace Puppy.Web.Render
{
    public interface IViewRenderService
    {
        /// <summary>
        ///     Render Razor View as String 
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model">   </param>
        /// <returns></returns>
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}