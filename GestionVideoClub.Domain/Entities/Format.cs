namespace GestionVideoClub.Domain.Entities
{
    public class Format
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int ImageQuality { get; private set; }
        public int AdditionalCost { get; private set; }

        public Format(string name, int imageQuality, int additionalCost)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (imageQuality <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(imageQuality), "Image quality must be greater than zero.");
            }
            if (additionalCost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(additionalCost), "Additional cost cannot be negative.");
            }

            Id = Guid.NewGuid();
            Name = name;
            ImageQuality = imageQuality;
            AdditionalCost = additionalCost;
        }

        private Format()
        {
            Name = string.Empty;
        }
    }
}
