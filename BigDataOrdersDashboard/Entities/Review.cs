using System.ComponentModel.DataAnnotations.Schema;

namespace BigDataOrdersDashboard.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public string PurchaseType { get; set; }
        public byte Rating { get; set; }
        public string Sentiment { get; set; }
        public string ReviewText { get; set; }
        [Column(TypeName = "date")]
        public DateTime ReviewDate { get; set; }
        public Product Product { get; set; }
        public Customer Customer { get; set; }
    }
}
