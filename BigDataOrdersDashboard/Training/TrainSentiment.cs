using Microsoft.ML;

namespace BigDataOrdersDashboard.Training
{
    public class TrainSentiment
    {
        public void Train()
        {
            var ml = new MLContext(seed: 42);

            var data = ml.Data.LoadFromTextFile<MessageModelInput>(
                path: "wwwroot/data/100_messages.csv",
                hasHeader: true,
                separatorChar: ','
            );

            var split = ml.Data.TrainTestSplit(data, 0.2);

            var pipeline = ml.Transforms.Text.FeaturizeText(
                                "TextFeats",
                                nameof(MessageModelInput.MessageText))
                .Append(ml.Transforms.Text.FeaturizeText(
                                "SubjectFeats",
                                nameof(MessageModelInput.MessageSubject)))
                .Append(ml.Transforms.Concatenate(
                                "Features",
                                "TextFeats",
                                "SubjectFeats"))
                .Append(ml.Transforms.Conversion.MapValueToKey(
                                "Label",
                                nameof(MessageModelInput.SentimentLabel)))
                .Append(ml.MulticlassClassification.Trainers
                                .SdcaMaximumEntropy())
                .Append(ml.Transforms.Conversion.MapKeyToValue(
                                "PredictedLabel"));

            var model = pipeline.Fit(split.TrainSet);

            ml.Model.Save(model, split.TrainSet.Schema, "wwwroot/models/sentimentModel.zip");
        }
    }
}
