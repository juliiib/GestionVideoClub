namespace GestionVideoClub.Models
{
    public class Copy
    {
        private static int nextID = 0;

        public enum CopyState { healthy, damaged }

        public int ID { get; }
        public string InternalCode { get; }
        public CopyState State { get; private set; }
        public string Location { get; }
        public bool IsAvailable { get; private set; }

        public Movie Movie { get; }
        public Format Format { get; }

        public Copy(string internalCode, CopyState state, string location, bool isAvailable, Movie movie, Format format)
        {
            if (string.IsNullOrWhiteSpace(internalCode))
            {
                throw new ArgumentException("Internal code cannot be null or empty.", nameof(internalCode));
            }
           
            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentException("Location cannot be null or empty.", nameof(location));
            }

            ID = nextID++;
            InternalCode = internalCode;
            State = state;
            Location = location;
            IsAvailable = isAvailable;
            Movie = movie ?? throw new ArgumentNullException(nameof(movie), "Movie cannot be null.");
            Format = format ?? throw new ArgumentNullException(nameof(format), "Format cannot be null.");
        }
    }
}
