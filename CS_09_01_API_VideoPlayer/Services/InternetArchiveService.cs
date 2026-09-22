using System.Net.Http;

/* CLASS INTERNET ARCHIVE SERVICE: INTERNET ARCHIVE API RELATED */

namespace CS_09_01_API_VideoPlayer.Services
{
    public class InternetArchiveService
    {
        // ATTRIBUTES
        private readonly HttpClient _client = new();  // http client

        private readonly string? _searchUrl = // archive search api
            JsonService.GetLocalSetting("ArchiveSearchUrl");

        private readonly string? _downloadUrl = //archive download apio
            JsonService.GetLocalSetting("ArchiveDownloadUrl");


        // API METHODS

        // ----HTTP GET: Search Internet Archive for videos
        public async Task<string?> GetVideosAsync(string searchText)
        {
            try
            {
                // make string url safe
                string safeSearchText = Uri.EscapeDataString(searchText);

                // combine base url with search query
                string url =
                    $"{_searchUrl}?q=title:({safeSearchText})" +
                    $"+AND+mediatype:movies&sort[]=downloads+desc&rows=15&output=json";

                // for debug
                System.Diagnostics.Debug.WriteLine($"REQUEST URL: '{url}'");

                // send GET request
                HttpResponseMessage responseMessage = await _client.GetAsync(url);

                // check if request successful (status 200-299)
                responseMessage.EnsureSuccessStatusCode();

                // read answer as string
                string responseBody =
                    await responseMessage.Content.ReadAsStringAsync();

                // return
                return responseBody;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Request-Fehler: " + ex.Message);

                return null;
            }
        }


        // ----HTTP GET: Get file metadata for an Internet Archive item
        public async Task<string?> GetVideoItemAsync(string identifier)
        {
            try
            {
                // clear identifier string
                string cleanIdentifier =
                    identifier?.Trim() ?? string.Empty;

                // combine base url with identifier
                string url =
                    $"{_downloadUrl}{cleanIdentifier}/{cleanIdentifier}_files.xml";

                System.Diagnostics.Debug.WriteLine(
                    $"REQUEST URL: '{url}'");

                string apiResponse =
                    await _client.GetStringAsync(url);

                return apiResponse;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "S3 Request-Fehler: " + ex.Message);

                return null;
            }
        }


        // ----HTTP GET: MP4 stream URL for MediaElement
        public async Task<string?> GetMP4UrlAsync(string identifier)
        {
            // clean identifier string
            string cleanIdentifier =
                identifier?.Trim() ?? string.Empty;

            // get xml response
            string? xmlResponse =
                await GetVideoItemAsync(cleanIdentifier);

            // null check
            if (string.IsNullOrEmpty(xmlResponse))
                return null;

            try
            {
                // parse
                var doc =
                    System.Xml.Linq.XDocument.Parse(xmlResponse);

                // get all <file> elements
                var fileElements =
                    doc.Descendants()
                       .Where(e => e.Name.LocalName == "file");

                // loop through file elements for .mp4 attribute
                foreach (var fileElement in fileElements)
                {
                    string fileName =
                        fileElement.Attribute("name")?.Value
                        ?? string.Empty;

                    if (fileName.EndsWith(
                        ".mp4",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        // combine base download url with file path
                        string finalUrl =
                            $"{_downloadUrl}{cleanIdentifier}/{fileName}";

                        System.Diagnostics.Debug.WriteLine(
                            $"MP4 URL: '{finalUrl}'");

                        return finalUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "XML-Parsing-Fehler: " + ex.Message);
            }

            return null;
        }
    }
}