namespace BigDataOrdersDashboard.DTOS.LoyaltyDtos
{
    public class LoyaltyScoreDtos
    {
        public string CustomerName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public double LoyaltyScore { get; set; }
    }
}
