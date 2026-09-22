



namespace CS_09_01_API_VideoPlayer.Services
{
    public class GeminiService
    {

        // ATTRIBUTES
        private readonly Google.GenAI.Client _client;

        // CONSTRUCTOR
        public GeminiService()
        {
            // get api key in constructor
            string? apiKey = JsonService.GetLocalSetting("GeminiApiKey");

            // null check - exception
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Gemini API key not configured.");
            }

            // create client
            _client = new Google.GenAI.Client(apiKey: apiKey);
        }


        // METHODS

        // ---- Generate movie information using Gemini AI
        public async Task<string> GetAiSummaryAsync(string title, string identifier)
        {
            string prompt =
                $"Recherchiere den Film '{title}' mit den Stichworten {identifier} und verfasse einen kurzen, natürlich fließenden Antworttext. " +
                "Verwende folgenden Aufbau und beende fehlende Punkte einfach: " +
                "Titel, Regie, Genre, Erscheinungsjahr und eine kurze Handlung. " +
                "Anforderungen: " +
                "- Antworte in fließendem Fließtext ohne Formatierungszeichen (keine Aufzählungen, keine Sternchen, kein Fettgedrucktes). " +
                "- Nutze einen sachlichen, aber natürlich erzählenden Sprachstil. " +
                "- Maximal 90 Wörter. " +
                "- Falls zu einem Punkt keine Informationen vorhanden sind, lass ihn stillschweigend weg.";

            try
            {
                // get API key from local settings
                string? apiKey = JsonService.GetLocalSetting("GeminiApiKey");

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return "Gemini API key not configured.";
                }

                // create Gemini client
                var client = new Google.GenAI.Client(apiKey: apiKey);

                // generate AI response
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-3.5-flash",
                    contents: prompt);

                return response.Text ?? "Keine Informationen.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("KI-Fehler: " + ex.Message);
                return "Fehler beim Laden der KI-Zusammenfassung.";
            }
        }

    // END CLASS
    }
}