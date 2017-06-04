using System;
using System.Collections.Generic;
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

            var listUrlDetail = restaurantEye.GetListDetailUrl("g293925", "Ho_Chi_Minh_City").Result;
            stopwatch.Stop();
            Console.WriteLine(
                $"Done Get List URL Detail: {listUrlDetail.Count} url, {stopwatch.Elapsed.TotalSeconds} s");
            stopwatch.Restart();
            var listDetail = new List<RestaurantDetailModel>();
            var listDetailError = new List<RestaurantDetailModel>();

            using (var dbContext = new TripAdvisorDataModel())
            {
                foreach (var url in listUrlDetail)
                {
                    var restaurantEyeDetail = new RestaurantEye();
                    var detail = restaurantEyeDetail.GetDetail(url).Result;
                    if (detail != null)
                    {
                        listDetail.Add(detail);
                        dbContext.Restaurants.AddOrUpdate(detail);
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            listDetailError.Add(detail);
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