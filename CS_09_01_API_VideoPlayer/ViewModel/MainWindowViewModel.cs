
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CS_09_01_API_VideoPlayer.Model;
using CS_09_01_API_VideoPlayer.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace CS_09_01_API_VideoPlayer.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {



        // ATTRIBUTES
        // ---- instance of IntAernet Archive Apis class
        InternetArchiveService ArchiveApi = new InternetArchiveService();

        // ---- instance of Gemeni Ai Api class
        GeminiService GemAiApi = new GeminiService();

        // ---- observable property for user search inpt
        [ObservableProperty]
        private string? _searchText;

        // ---- observable property for selected video from results list
        [ObservableProperty]
        private Video? _selectedVideo;

        // ---- observable property for media player vide source
        [ObservableProperty]
        private Uri? _mediaPlayerSource;

        // ---- observable collection for video results in list (shown in data grid)
        [ObservableProperty]
        private ObservableCollection<Video> _videoResults = new();

        // ---- observable property for aii description
        [ObservableProperty]
        private string? _aiDescription = "Film suchen und aus Ergebnisliste auswählen.";




        // CONSTRUCTOR
        public MainWindowViewModel()
        {
        }


        // METHODS
        // --- Relay Command: Search via Api
        [RelayCommand]
        public async Task TriggerSearch()
        {

            // check if search text is empty
            if (string.IsNullOrWhiteSpace(SearchText)) 
            {
                System.Diagnostics.Debug.WriteLine("Search Text = null");
                return;
            }

            // get result data
            string result = await ArchiveApi.GetVideosAsync(SearchText);

            // turn into Video class compatible format withi JsonService Class
            var parsedList = JsonService.ParseVideoResponse(result);
            VideoResults = new ObservableCollection<Video>(parsedList);


            // show result in debug console
            System.Diagnostics.Debug.WriteLine(result);
        }

        // METHODS
        // --- Event Method
        partial void OnSelectedVideoChanged(Video? item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Identifier))
                return;

            LoadVideo(item.Identifier);
        }



        // --- get video url and load
        private async void LoadVideo(string identifier)
        {
            // api mp4 filter for url
            string? mp4Url = await ArchiveApi.GetMP4UrlAsync(identifier);

            if (!string.IsNullOrWhiteSpace(mp4Url))
            {
                MediaPlayerSource = new Uri(mp4Url, UriKind.Absolute);
            }

            System.Diagnostics.Debug.WriteLine($"FERTIGE MP4 URL: {MediaPlayerSource}");

            // ai api summary
            if (SelectedVideo != null)
            {
                AiDescription = "Informationen werden gesucht...";

                // get ai summery from ApiService
                string title = SelectedVideo.Title ?? "Unbekannter Titel";
                AiDescription = await GemAiApi.GetAiSummaryAsync(title, identifier);
            }


        }





        // END CLASS

    }
}
