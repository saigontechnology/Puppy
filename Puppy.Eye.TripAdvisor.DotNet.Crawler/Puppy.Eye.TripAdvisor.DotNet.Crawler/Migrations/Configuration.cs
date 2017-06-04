using System.Data.Entity.Migrations;

namespace Puppy.Eye.TripAdvisor.DotNet.Crawler.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<TripAdvisorDataModel>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(TripAdvisorDataModel context)
        {
        }
    }
}