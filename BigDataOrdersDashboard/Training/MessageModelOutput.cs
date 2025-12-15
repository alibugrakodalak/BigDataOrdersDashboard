using Microsoft.ML.Data;

namespace BigDataOrdersDashboard.Training
{
    public class MessageModelOutput
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; } = "";
        public float[] Score { get; set; }
    }
}
