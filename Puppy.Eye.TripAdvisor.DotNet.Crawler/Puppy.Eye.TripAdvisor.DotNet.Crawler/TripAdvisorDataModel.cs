namespace Puppy.Eye.TripAdvisor.DotNet.Crawler
{
    using System.Data.Entity;

    public class TripAdvisorDataModel : DbContext
    {
        public TripAdvisorDataModel()
            : base("name=TripAdvisor")
        {
        }

        public virtual DbSet<RestaurantDetailModel> Restaurants { get; set; }
    }
}