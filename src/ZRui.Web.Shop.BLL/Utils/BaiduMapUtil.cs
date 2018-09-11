using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace ZRui.Web.BLL
{

    public class BaiduMapUtil
    {
        private const string ak = "yHvt9c2n34xtDQgdL1pGLXqRxekgrsNC";
        private const string directionUrl = "http://api.map.baidu.com/direction/v2/driving?origin={0},{1}&destination={2},{3}&ak={4}";
        private const string geocoderUrl = "http://api.map.baidu.com/geocoder/v2/?address={0}&output=json&ak={1}";
        private const string geoRecoderUrl = "http://api.map.baidu.com/geocoder/v2/?location={0},{1}&output=json&pois=1&ak={2}";
        private const string bdtoGdUrl = "https://restapi.amap.com/v3/assistant/coordinate/convert?locations={0},{1}&coordsys=baidu&output=json&key=da91a960c6e7f904d71166942f3694cc";
        private static readonly HttpClient httpClient;

        static BaiduMapUtil()
        {
            httpClient = new HttpClient();
        }

        public static int GetDistance(Tuple<double, double> origin, Tuple<double, double> destination)
        {
            string url = string.Format(directionUrl, origin.Item1, origin.Item2,
                destination.Item1, destination.Item2, ak);
            var res = httpClient.GetAsync(url);
            res.Wait();
            var content = res.Result.Content.ReadAsStringAsync();
            content.Wait();
            MapRespone_Distance mapRespone = JsonConvert.DeserializeObject<MapRespone_Distance>(content.Result);
            if (mapRespone.status == 0 && mapRespone.result.total > 0)
            {
                return mapRespone.result.routes[0].distance;
            }
            else
            {
                return -1;
            }

        }

        public static BdToGdResult GetBdToGd(double x, double y)
        {
            var bdtourl = string.Format(bdtoGdUrl, x, y);
            var result = httpClient.GetAsync(bdtourl);
            result.Wait();
            var content = result.Result.Content.ReadAsStringAsync();
            content.Wait();
            var json = JsonConvert.DeserializeObject<BdToGd>(content.Result);
            return json.result.FirstOrDefault();
        }



        /// <summary>
        /// 地理编码
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static (double lat, double lng)? GetGeoCoder(string address)
        {
            string url = string.Format(geocoderUrl, address, ak);
            var res = httpClient.GetAsync(url);
            res.Wait();
            var content = res.Result.Content.ReadAsStringAsync();
            content.Wait();
            MapRespone_Geocoder mapRespone = JsonConvert.DeserializeObject<MapRespone_Geocoder>(content.Result);
            if (mapRespone.status == 0)
            {
                return (mapRespone.result.location.lat, mapRespone.result.location.lng);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 全球逆地理编码服务
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static ReGeocoderResult GetReGeoCoder(double lat, double lng)
        {
            string url = string.Format(geoRecoderUrl, lat, lng, ak);
            var res = httpClient.GetAsync(url);
            res.Wait();
            var content = res.Result.Content.ReadAsStringAsync();
            content.Wait();
            var mapRespone = JsonConvert.DeserializeObject<MapRespone_ReGeocoder>(content.Result);
            if (mapRespone.status == 0)
            {
                return mapRespone.result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 百度地图坐标转腾讯地图坐标
        /// </summary>
        /// <param name="inputLai"></param>
        /// <param name="inputLogi"></param>
        /// <returns></returns>
        public static (double lati, double logi) CoverCoordinateToTX(double inputLai, double inputLogi)
        {
            double tx_lat;
            double tx_lon;
            double x_pi = 3.14159265358979324;
            double x = inputLogi - 0.0065, y = inputLai - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
            tx_lon = z * Math.Cos(theta);
            tx_lat = z * Math.Sin(theta);
            return (tx_lat, tx_lon);
        }

        /// <summary>
        /// 坐标转换，腾讯地图转换成百度地图坐标
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static (double lat, double lng) CoverCoordinateToBaidu(double lat, double lon)
        {
            double bd_lat;
            double bd_lon;
            double x_pi = 3.14159265358979324;
            double x = lon, y = lat;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);
            bd_lon = z * Math.Cos(theta) + 0.0065;
            bd_lat = z * Math.Sin(theta) + 0.006;
            return (bd_lat, bd_lon);
        }


        /// <summary>
        /// 百度地图坐标转高德地图坐标
        /// </summary>
        /// <param name="bd_lat"></param>
        /// <param name="bd_lng"></param>
        /// <returns></returns>
        public static (double lat, double lng) DdToGaoDe(double bd_lat, double bd_lng)
        {
            double PI = 3.14159265358979324 * 3000.0 / 180.0;
            double x = bd_lng - 0.0065, y = bd_lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * PI);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * PI);
            return (z * Math.Cos(theta), z * Math.Sin(theta));
        }


        private class MapRespone_ReGeocoder
        {
            public int status { get; set; }
            public ReGeocoderResult result { get; set; }
        }

        private class MapRespone_Distance
        {
            public int status { get; set; }
            public MapResult_Distance result { get; set; }
        }

        public class BdToGd {
            public List<BdToGdResult> result { get; set; }
        }

        public class BdToGdResult
        {
            public double x { get; set; }

            public double y { get; set; }
        }

        class MapResult_Distance
        {
            public int total { get; set; }
            public List<MapRoute_Distance> routes { get; set; }
        }

        class MapRoute_Distance
        {
            public int distance { get; set; }
        }

        class MapRespone_Geocoder
        {
            public int status { get; set; }
            public MapResult_Geocoder result { get; set; }
        }

        class MapResult_Geocoder
        {
            public Geocoder_Location location { get; set; }
        }

        class Geocoder_Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
    }

    public class ReGeocoderResult
    {
        public string formatted_address { get; set; }
        public string sematic_description { get; set; }
        public ProvinceCityDistrict addressComponent { get; set; }
    }

    public class ProvinceCityDistrict
    {
        public string province { get; set; }
        public string city { get; set; }
        public string district { get; set; }
    }
}
