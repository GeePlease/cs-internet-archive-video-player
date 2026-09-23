using CS_09_01_API_VideoPlayer.Model;
using Newtonsoft.Json.Linq;
using System.IO;

/* CLASS JSON SERVICE: Parse HTTP RESPONSE JSON OBJECT TO VIDEO OBJECT (Video Class)*/

namespace CS_09_01_API_VideoPlayer.Services
{
    public class JsonService
    {
        // METHODS

        public static List<Video> ParseVideoResponse(string apiResponseString)
        {
            try
            {
                // convert response to jobject
                JObject apiResponse = JObject.Parse(apiResponseString);

                // get api response json array
                var docsArray = apiResponse["response"]?["docs"] as JArray;

                // return empty list if no json array
                if (docsArray == null)
                {
                    return new List<Video>();
                }

                // create list of Video class Objects from json response
                List<Video> videoList = new List<Video>();

                // map each json element to Video class
                foreach (var item in docsArray)
                {
                    Video myVideo = new Video
                    {
                        Title = item["title"]?.ToString(),
                        Creator = item["creator"]?.ToString(),
                        Description = item["description"]?.ToString(),
                        Identifier = item["identifier"]?.ToString()
                    };

                    videoList.Add(myVideo);
                }

                return videoList;

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(
                    "JSON Parsing Error: " + ex.Message);

                return new List<Video>();

            }
        }


        // ---- get setting from local non-tracked JSON file
        public static string? GetLocalSetting(string settingName)
        {
            string filePath = Path.Combine(
                AppContext.BaseDirectory,
                "appsettings.Local.json");

            // return null if local settings file does not exist
            if (!File.Exists(filePath))
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Local settings file not found: {filePath}");

                return null;
            }

            // tc to catch file reading errors
            try
            {
                // read local JSON file
                string json = File.ReadAllText(filePath);

                // parse JSON
                JObject settings = JObject.Parse(json);

                // return requested setting
                return settings[settingName]?.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Local Settings Error: " + ex.Message);

                return null;
            }
        }
    }
}