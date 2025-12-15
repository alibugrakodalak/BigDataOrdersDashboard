using Microsoft.ML.Data;

namespace BigDataOrdersDashboard.DTOS.LoyaltyMLDtos
{
    public class LoyaltyScoreMLPredictionDto
    {
        [ColumnName("Score")]
        public float LoyaltyScore { get; set; }
    }
}
