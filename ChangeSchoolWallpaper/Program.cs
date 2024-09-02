using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace ChangeSchoolWallpaper
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
                WinAPI.SetWallpaper(Wallpaper.GetCurrentImagePath());
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
        private static extern int SystemParametersInfo(uint uAction, int uParam, string lpvParam, uint fuWinIni);
        private const uint SPI_SETDESKWALLPAPER = 0x0014;
        private const uint SPIF_UPDATEINIFILE = 0x0001;
        private const uint SPIF_SENDWININICHANGE = 0x0002;
        public static int SetWallpaper(string path)
        {
            StringBuilder sb = new StringBuilder(path);
            return SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
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
                    monday = String.Empty,
                    tuesday = String.Empty,
                    wednesday = String.Empty,
                    thursday = String.Empty,
                    friday = String.Empty,
                    saturday = String.Empty,
                    sunday = String.Empty,
                    exception = String.Empty,
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
            return week switch
            {
                "Monday" => "monday",
                "Tuesday" => "tuesday",
                "Wednesday" => "wednesday",
                "Thursday" => "thursday",
                "Friday" => "friday",
                "Saturday" => "saturday",
                "Sunday" => "sunday",
                _ => throw new Exception("Invalid day of week")
            };
        }

        public static object GetKey(string key)//读取json
        {
            string config = File.ReadAllText(configPath);
            JObject configJson = JObject.Parse(config);
            if (configJson.TryGetValue(key, out JToken value))
            {
                return value.Type switch
                {
                    JTokenType.String => value.ToString(),
                    JTokenType.Boolean => value.ToObject<bool>(),
                    _ => throw new Exception("Invalid key type")
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
                if (GetKey("exception") == String.Empty)
                {
                    throw new Exception("No wallpaper set for exception");
                }
                else return GetKey("exception").ToString();
            }
            else
            {
                if (GetKey(CurrentDate()) == String.Empty)
                {
                    throw new Exception("No wallpaper set for this day");
                }
                else return GetKey(CurrentDate()).ToString();
            }
        }
    }
}
