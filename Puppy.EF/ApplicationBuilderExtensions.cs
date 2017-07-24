#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ApplicationBuilderExtensions.cs </Name>
//         <Created> 25/07/17 1:54:29 AM </Created>
//         <Key> b692d2ea-3613-4787-80af-ba47b2bf8829 </Key>
//     </File>
//     <Summary>
//         ApplicationBuilderExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Puppy.EF.Interfaces;

namespace Puppy.EF
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     <para>
        ///         Applies any pending migrations for the context to the database. Will create the
        ///         database if it does not already exist.
        ///     </para>
        /// </summary>
        public static IApplicationBuilder DatabaseMigrate(this IApplicationBuilder app)
        {
            IBaseDbContext dbContext = app.ApplicationServices.GetService<IBaseDbContext>();
            dbContext.Database.Migrate();

            return app;
        }
    }
}