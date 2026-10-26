namespace GestionVideoClub.Domain.Entities
{
    public class Movie
    {
        public Guid Id { get; private set; }
        public string ImdbId { get; private set; }
        public string Name { get; private set; }
        public string Genre { get; private set; }
        public int Duration { get; private set; }
        public int Clasification { get; private set; }
        public int YearRelease { get; private set; }
        public string? Plot { get; private set; }
        public string? PosterUrl { get; private set; }

        public Movie(string imdbId, string name, string genre, int duration, int clasification, int yearRelease, string? plot, string? posterUrl)
        {
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                throw new ArgumentException("ImdbId cannot be null or empty.", nameof(imdbId));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(genre))
            {
                throw new ArgumentException("Genre cannot be null or empty.", nameof(genre));
            }

            if (duration <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), "Duration must be greater than zero.");
            }

            if (clasification <= 0) {
                throw new ArgumentOutOfRangeException(nameof(clasification), "Clasification must be greater than zero.");
            }

            if (yearRelease <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(yearRelease), "Year of release must be greater than zero.");
            }

            Id = Guid.NewGuid();
            ImdbId = imdbId;
            Name = name;
            Genre = genre;
            Duration = duration;
            Clasification = clasification;
            YearRelease = yearRelease;
            Plot = plot;
            PosterUrl = posterUrl;
        }

        private Movie()
        {
            ImdbId = string.Empty;
            Name = string.Empty;
            Genre = string.Empty;
        }
    }
}
