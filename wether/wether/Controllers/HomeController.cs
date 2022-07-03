using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using wether.Models;


namespace wether.Controllers
{
    public class HomeController :  Controller
    {
        private readonly ILogger<HomeController> _logger;
        public RootObject weatherObj;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 2)]
        public IActionResult Wether()
        {

            //Models.Weather obj;
            List<Models.WeatherRequest> cities = new List<WeatherRequest>();
            cities.Add(new WeatherRequest { CityCode = 1248991, CityName = "Colombo", Temp = 33.0, Status = "Clouds" });
            cities.Add(new WeatherRequest { CityCode = 1850147, CityName = "Tokyo", Temp = 8.6, Status = "Clear" });
            cities.Add(new WeatherRequest { CityCode = 2644210, CityName = "Liverpool", Temp = 16.5, Status = "Rain" });
            cities.Add(new WeatherRequest { CityCode = 2988507, CityName = "Paris", Temp = 22.4, Status = "Clear" });
            cities.Add(new WeatherRequest { CityCode = 2147714, CityName = "Sydney", Temp = 27.3, Status = "Rain" });
            cities.Add(new WeatherRequest { CityCode = 4930956, CityName = "Boston", Temp = 4.2, Status = "Mist" });
            cities.Add(new WeatherRequest { CityCode = 1796236, CityName = "Shanghai", Temp = 10.1, Status = "Clouds" });
            cities.Add(new WeatherRequest { CityCode = 3143244, CityName = "Oslo", Temp = -3.9, Status = "Clear" });
            cities.Add(new WeatherRequest { CityCode = 3143244, CityName = "Weligama", Temp = -3.9, Status = "Clear" });


            List<Models.RootObject> WeatherFrocast = new List<RootObject>();

            foreach (var i in cities)
            {
                var res = weatherBalloonAsync(i.CityCode,i.CityName);
                var min = Math.Round((res.main.temp_min) - 273.15);
                res.main.temp_max = Math.Round((res.main.temp_max) - 273.15);
                res.main.temp =  Math.Round((res.main.temp) - 273.15);
                res.visibility = res.visibility / 1000.0;
                res.main.temp_min = min;
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                res.sys.sunrise = (dateTime.AddSeconds(Convert.ToInt64(res.sys.sunrise)).ToLocalTime()).ToString("h.mmtt");
                res.sys.sunset = (dateTime.AddSeconds(Convert.ToInt64(res.sys.sunset)).ToLocalTime()).ToString("h.mmtt");
                res.dt = (dateTime.AddSeconds(Convert.ToInt64(res.dt)).ToLocalTime()).ToString("h.mmtt");
                res.weather[0].icon = "http://api.openweathermap.org/img/w/"+ res.weather[0].icon+ ".png";

             WeatherFrocast.Add(res);
            }
            var time = DateTime.Now.ToString("HH.mm");
            var date = DateTime.Now.ToString("MMM,yyyy");
            return View(WeatherFrocast);

        }

        private RootObject  weatherBalloonAsync(int cityCode,string cityName)
        {
            string appId = "e2f78158b52ec0e948639f0e2f57c1b8";

            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", cityName, appId);


            using (var client = new WebClient())
            {
                try
                {
                    string json = client.DownloadString(url);

                    weatherObj = JsonConvert.DeserializeObject<RootObject>(json);
                    return weatherObj;

                }
                catch (Exception e)
                {
                    return new RootObject();
                }
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
