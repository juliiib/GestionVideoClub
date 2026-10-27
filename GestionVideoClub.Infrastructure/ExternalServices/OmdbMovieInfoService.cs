using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Domain.Models;
using Microsoft.Extensions.Options;

namespace GestionVideoClub.Infrastructure.ExternalServices
{
    public class OmdbMovieInfoService : IMovieInfoService
    {
        private readonly HttpClient httpClient;
        private readonly OmdbSettings settings;

        public OmdbMovieInfoService(HttpClient httpClient, IOptions<OmdbSettings> settings)
        {
            this.httpClient = httpClient;
            this.settings = settings.Value;
        }

        public async Task<MovieInfo?> SearchByTitleAsync(string title, int? year)
        {
            var url = $"?t={Uri.EscapeDataString(title)}" +
                (year.HasValue ? $"&y={year.Value}" : string.Empty) +
                $"&type=movie&apikey={settings.ApiKey}";

            var response = await httpClient.GetFromJsonAsync<OmdbResponse>(url)
                ?? throw new HttpRequestException("OMDb no devolvió ninguna respuesta.");

            // OMDb responde HTTP 200 aun cuando no encuentra la película: en ese caso
            // "Response" viene en "False" y el dato útil está en "Error", no en un código HTTP.
            if (response.Response != "True")
            {
                return null;
            }

            var duration = ParseDuration(response.Runtime);
            var parsedYear = ParseYear(response.Year);

            return new MovieInfo(
                response.ImdbId ?? string.Empty,
                response.Title ?? string.Empty,
                response.Genre ?? string.Empty,
                duration,
                parsedYear,
                NullIfNotAvailable(response.Plot),
                NullIfNotAvailable(response.Poster));
        }

        private static int ParseDuration(string? runtime)
        {
            var match = Regex.Match(runtime ?? string.Empty, @"\d+");
            if (!match.Success)
            {
                throw new InvalidOperationException("OMDb no informa la duración de la película.");
            }

            return int.Parse(match.Value, CultureInfo.InvariantCulture);
        }

        private static int ParseYear(string? year)
        {
            var match = Regex.Match(year ?? string.Empty, @"\d{4}");
            if (!match.Success)
            {
                throw new InvalidOperationException("OMDb no informa el año de la película.");
            }

            return int.Parse(match.Value, CultureInfo.InvariantCulture);
        }

        private static string? NullIfNotAvailable(string? value) =>
            string.IsNullOrWhiteSpace(value) || value == "N/A" ? null : value;

        private class OmdbResponse
        {
            [JsonPropertyName("Response")]
            public string? Response { get; set; }

            [JsonPropertyName("Error")]
            public string? Error { get; set; }

            [JsonPropertyName("imdbID")]
            public string? ImdbId { get; set; }

            [JsonPropertyName("Title")]
            public string? Title { get; set; }

            [JsonPropertyName("Year")]
            public string? Year { get; set; }

            [JsonPropertyName("Runtime")]
            public string? Runtime { get; set; }

            [JsonPropertyName("Genre")]
            public string? Genre { get; set; }

            [JsonPropertyName("Plot")]
            public string? Plot { get; set; }

            [JsonPropertyName("Poster")]
            public string? Poster { get; set; }
        }
    }
}
