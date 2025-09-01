using System.Runtime.InteropServices;
using System.Text.Json;

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
        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SystemParametersInfo")]
        private static extern int SystemParametersInfo(uint uAction, int uParam, string lpvParam, uint fuWinIni);
        private const uint SPI_SETDESKWALLPAPER = 0x0014;
        private const uint SPIF_UPDATEINIFILE = 0x0001;
        private const uint SPIF_SENDWININICHANGE = 0x0002;
        public static int SetWallpaper(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException($"Wallpaper file not found at {path}");
            return SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
    public static class Wallpaper
    {
        public const string configPath = "config.json";

        public static void Initalize()
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
                // 使用System.Text.Json进行序列化
                string json = JsonSerializer.Serialize(initialConfig, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
                Console.WriteLine("Config file created");
            }
            else Console.WriteLine("Config file found");
        }

        public static string GetKey(string key)
        {
            string config = File.ReadAllText(configPath);
            // 使用JsonDocument替代JObject
            using JsonDocument doc = JsonDocument.Parse(config);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty(key, out JsonElement value))
            {
                return value.ValueKind switch
                {
                    JsonValueKind.String => value.GetString() ?? string.Empty,
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    _ => throw new Exception("Invalid key type")
                };
            }
            else
            {
                throw new Exception("Invalid key");
            }
        }
        public static string CurrentDate()//当前日期
        {
            return DateTime.Now.DayOfWeek.ToString().ToLower();
        }


        public static string GetCurrentImagePath()//获取图片路径
        {
            var exception = GetKey("exception_setting");
            if (exception == "true")
            {
                if (string.IsNullOrEmpty(GetKey("exception")))
                {
                    throw new Exception("No wallpaper set for exception");
                }
                else return GetKey("exception");
            }
            else
            {
                if (string.IsNullOrEmpty(GetKey(CurrentDate())))
                {
                    throw new Exception("No wallpaper set for this day");
                }
                else return GetKey(CurrentDate());
            }
        }
    }
}
