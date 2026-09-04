namespace GestionVideoClub.Models
{
    public class Format
    {
        private static int nextID = 1;

        public int ID { get; }
        public string Name { get; }
        public int ImageQuality { get; }
        public int AdditionalCost { get; }

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

            ID = nextID++;
            Name = name;
            ImageQuality = imageQuality;
            AdditionalCost = additionalCost;
        }
    }
}
