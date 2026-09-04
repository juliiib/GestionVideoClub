namespace GestionVideoClub.Models
{
    public class Movie
    {
        private static int nextID = 1;
        
        public int ID { get; }
        public string Name { get; }
        public string Genre { get;  }
        public int Duration { get; }
        public int Clasification { get; }
        public int YearRelease { get; }

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

            ID = nextID++;
            Name = name;
            Genre = genre;
            Duration = duration;
            Clasification = clasification;
            YearRelease = yearRelease;
        }

    }
}
