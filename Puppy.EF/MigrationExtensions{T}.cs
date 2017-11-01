#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> MigrationExtensions.cs </Name>
//         <Created> 25/07/17 1:54:29 AM </Created>
//         <Key> b692d2ea-3613-4787-80af-ba47b2bf8829 </Key>
//     </File>
//     <Summary>
//         MigrationExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Puppy.EF.Interfaces;
using System;

namespace Puppy.EF
{
    public static class MigrationExtensions
    {
        /// <summary>
        ///     <para>
        ///         Applies any pending migrations for the context to the database. Will create the
        ///         database if it does not already exist.
        ///     </para>
        /// </summary>
        public static IServiceProvider MigrateDatabase<T>(this IServiceProvider services) where T : IBaseDbContext
        {
            T dbContext = services.GetRequiredService<T>();
            dbContext.Database.Migrate();
            return services;
        }

        /// <summary>
        ///     <para>
        ///         Applies any pending migrations for the context to the database. Will create the
        ///         database if it does not already exist.
        ///     </para>
        /// </summary>
        public static IApplicationBuilder MigrateDatabase<T>(this IApplicationBuilder app) where T : IBaseDbContext
        {
            T dbContext = app.ApplicationServices.GetService<T>();
            dbContext.Database.Migrate();
            return app;
        }
    }
}