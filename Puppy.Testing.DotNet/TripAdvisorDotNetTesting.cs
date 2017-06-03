using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Puppy.Eye.TripAdvisor.DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Puppy.Testing.DotNet
{
    [TestClass]
    public class TripAdvisorDotNetTesting
    {
        [TestMethod]
        public async Task RestaurantEye()
        {
            RestaurantEye restaurantEye = new RestaurantEye();

            // HCM
            string hoChiMinhCode = "Ho_Chi_Minh_City";
            var filePathListUrl =
                $@"D:\Projects\TOP\puppy\Puppy.Testing.DotNet\TripAdvisor_RestaurantEye_{hoChiMinhCode}_ListUrl.json";

            var filePathListDetail =
                $@"D:\Projects\TOP\puppy\Puppy.Testing.DotNet\TripAdvisor_RestaurantEye_{hoChiMinhCode}_ListDetail.json";

            Stopwatch stopwatch = Stopwatch.StartNew();
            List<string> listDetailUrlHoChiMinh = await restaurantEye.GetListDetailUrl(hoChiMinhCode);
            stopwatch.Stop();
            File.WriteAllText(filePathListUrl, "Total Time: " + stopwatch.Elapsed.TotalMilliseconds + " ms" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllLines(filePathListUrl, listDetailUrlHoChiMinh, Encoding.UTF8);

            stopwatch.Restart();
            List<RestaurantDetailModel> listDetail = new List<RestaurantDetailModel>();
            foreach (var detailUrl in listDetailUrlHoChiMinh)
            {
                RestaurantDetailModel detailModel = await restaurantEye.GetDetail(detailUrl);
                listDetail.Add(detailModel);
            }
            var jsonData = JsonConvert.SerializeObject(listDetail, Formatting.Indented);
            stopwatch.Stop();
            File.WriteAllText(filePathListDetail, "Total Time: " + stopwatch.Elapsed.TotalMilliseconds + " ms" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(filePathListDetail, jsonData, Encoding.UTF8);
        }
    }
}