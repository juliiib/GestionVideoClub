namespace GestionVideoClub.Domain.Entities
{
    public class Movie
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Genre { get; private set; }
        public int Duration { get; private set; }
        public int Clasification { get; private set; }
        public int YearRelease { get; private set; }

        public Movie(string name, string genre, int duration, int clasification, int yearRelease)
        {
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
            Name = name;
            Genre = genre;
            Duration = duration;
            Clasification = clasification;
            YearRelease = yearRelease;
        }

        private Movie()
        {
            Name = string.Empty;
            Genre = string.Empty;
        }
    }
}
