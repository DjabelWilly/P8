namespace TourGuide.Dtos
{
    public class NearbyAttractionDto
    {
        public string AttractionName { get; set; } = null!;
        public double AttractionLatitude { get; set; }
        public double AttractionLongitude { get; set; }
        public double UserLatitude { get; set; }
        public double UserLongitude { get; set; }
        public double Distance { get; set; }
        public int RewardPoints { get; set; }
    }
}
