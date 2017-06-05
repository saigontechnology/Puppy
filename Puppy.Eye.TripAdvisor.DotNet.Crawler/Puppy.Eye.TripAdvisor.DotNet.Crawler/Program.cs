using System;
using System.Data.Entity.Migrations;
using System.Diagnostics;

namespace Puppy.Eye.TripAdvisor.DotNet.Crawler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            var stopwatch = Stopwatch.StartNew();
            var restaurantEye = new RestaurantEye();

            Console.WriteLine($"Start Crawler At: {DateTime.Now}");

            //var listUrlDetail = restaurantEye.GetListDetailUrl("g293925", "Ho_Chi_Minh_City").Result;
            var listUrlDetail = restaurantEye.GetListDetailUrl("g293924", "Hanoi").Result;
            listUrlDetail.AddRange(restaurantEye.GetListDetailUrl("g298085", "Da_Nang_Quang_Nam_Province").Result);
            stopwatch.Stop();
            Console.WriteLine(
                $"Done Get List URL Detail: {listUrlDetail.Count} url, {stopwatch.Elapsed.TotalSeconds} s");
            stopwatch.Restart();

            using (var dbContext = new TripAdvisorDataModel())
            {
                foreach (var url in listUrlDetail)
                {
                    var restaurantEyeDetail = new RestaurantEye();
                    var detail = restaurantEyeDetail.GetDetail(url).Result;
                    if (detail != null)
                    {
                        dbContext.Restaurants.AddOrUpdate(detail);
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            continue;
                        }
                    }

                    Console.WriteLine($"Crawl Done URL: {url}");
                }

                stopwatch.Stop();
                Console.WriteLine($"Done Get Details: {stopwatch.Elapsed.TotalSeconds} s");
                Console.WriteLine($"End Crawler At: {DateTime.Now}");
            }

            Console.ReadKey();
        }
    }
}