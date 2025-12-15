namespace BigDataOrdersDashboard.DTOS.LoyaltyMLDtos
{
    public class LoyaltlScoreMLDataDto
    {
        public string CustomerName { get; set; }
        public float Recency { get; set; }
        public float Frequency { get; set; }
        public float Monetary { get; set; }
        public float LoyaltyScore { get; set; }

    }
}
