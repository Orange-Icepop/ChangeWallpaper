using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChangeWallpaper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Program......");
            Console.WriteLine("Checking config file...");
            Wallpaper.Initalize();
            try
            {
                bool result = WinAPI.SetWallpaper(Wallpaper.GetCurrentImagePath());
                if (result)
                {
                    Console.WriteLine("Wallpaper changed successfully!");
                }
                else
                {
                    throw new Exception("Failed to change wallpaper.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
        }
    }
    public class WinAPI
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SystemParametersInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uAction, int uParam, string lpvParam, uint fuWinIni);
        private const uint SPI_SETDESKWALLPAPER = 0x0014;
        private const uint SPIF_UPDATEINIFILE = 0x0001;
        private const uint SPIF_SENDWININICHANGE = 0x0002;
        public static bool SetWallpaper(string path)
        {
            StringBuilder sb = new StringBuilder(path);
            sb.Append("\0");
            return SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, sb.ToString(), SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
    public class Wallpaper
    {
        public static string configPath = "config.json";
        public static void Initalize()// 初始化配置文件
        {
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Config file not found, creating new one...");
                var initialConfig = new
                {
                    exception_setting = false,
                    monday = string.Empty,
                    tuesday = string.Empty,
                    wednesday = string.Empty,
                    thursday = string.Empty,
                    friday = string.Empty,
                    saturday = string.Empty,
                    sunday = string.Empty,
                    exception = string.Empty,
                };
                string json = JsonConvert.SerializeObject(initialConfig, Formatting.Indented);
                File.WriteAllText(configPath, json);
                Console.WriteLine("Config file created");
            }
            else Console.WriteLine("Config file found");
        }

        public static string CurrentDate()//当前日期
        {
            string week = DateTime.Now.DayOfWeek.ToString();
            switch (week)
            {
                case "Monday":
                    return "monday";
                case "Tuesday":
                    return "tuesday";
                case "Wednesday":
                    return "wednesday";
                case "Thursday":
                    return "thursday";
                case "Friday":
                    return "friday";
                case "Saturday":
                    return "saturday";
                case "Sunday":
                    return "sunday";
                default: throw new Exception("Invalid day of week");
            };

        }

        public static object GetKey(string key)//读取json
        {
            string config = File.ReadAllText(configPath);
            JObject configJson = JObject.Parse(config);
            if (configJson.TryGetValue(key, out JToken value))
            {
                switch(value.Type)
                {
                    case JTokenType.String:
                        return value.ToString();
                    case JTokenType.Boolean:
                        return value.ToObject<bool>();
                    default: throw new Exception("Invalid key type");
                };
            }
            else
            {
                throw new Exception("Invalid key");
            }
        }

        public static string GetCurrentImagePath()//获取图片路径
        {
            bool exception = (bool)GetKey("exception_setting");
            if (exception)
            {
                if (GetKey("exception").ToString() == string.Empty)
                {
                    throw new Exception("No wallpaper set for exception");
                }
                else return GetKey("exception").ToString();
            }
            else
            {
                if (GetKey(CurrentDate()).ToString() == string.Empty)
                {
                    throw new Exception("No wallpaper set for this day");
                }
                else return GetKey(CurrentDate()).ToString();
            }
        }
    }
}
