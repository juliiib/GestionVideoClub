namespace GestionVideoClub.Domain.Entities
{
    public class Copy
    {
        public enum CopyState { healthy, damaged }

        public Guid Id { get; private set; }
        public string InternalCode { get; private set; }
        public CopyState State { get; private set; }
        public string Location { get; private set; }
        public bool IsAvailable { get; private set; }

        public Guid MovieId { get; private set; }
        public Guid FormatId { get; private set; }

        public Movie Movie { get; private set; }
        public Format Format { get; private set; }

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

            Id = Guid.NewGuid();
            InternalCode = internalCode;
            State = state;
            Location = location;
            IsAvailable = isAvailable;
            Movie = movie ?? throw new ArgumentNullException(nameof(movie), "Movie cannot be null.");
            MovieId = movie.Id;
            Format = format ?? throw new ArgumentNullException(nameof(format), "Format cannot be null.");
            FormatId = format.Id;
        }

        private Copy()
        {
            InternalCode = string.Empty;
            Location = string.Empty;
            Movie = null!;
            Format = null!;
        }

        public void SetState(CopyState newState)
        {
            State = newState;

            if (newState == CopyState.damaged )
            {
                IsAvailable = false;
            }
        }

        public void SetAvailability(bool availability)
        {
            IsAvailable = availability;
        }
    }
}
