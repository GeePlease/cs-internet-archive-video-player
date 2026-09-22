using CS_09_01_API_VideoPlayer.Model;
using Newtonsoft.Json.Linq;
using System.IO;

/* CLASS JSON SERVICE: Parse HTTP RESPONSE JSON OBJECT TO VIDEO OBJECT (Video Class)*/

namespace CS_09_01_API_VideoPlayer.Services
{
    public class JsonService
    {
        // METHODS

        // ---- turn Json Object api response into Video Object list
        public static List<Video> ParseVideoResponse(string apiResponseString)
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

            // read local JSON file
            string json = File.ReadAllText(filePath);

            // parse JSON
            JObject settings = JObject.Parse(json);

            // return requested setting
            return settings[settingName]?.ToString();
        }
    }
}