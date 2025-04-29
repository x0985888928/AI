using Microsoft.ML;
using Microsoft.ML.Data;
using MLPricePrediction;
using System;
using System.IO;

namespace MLPricePredictionConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. 建立 MLContext
            var mlContext = new MLContext(seed: 0);

            // 2. 讀取 CSV -> IDataView
            string dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "price_data.csv");
            IDataView fullData = mlContext.Data.LoadFromTextFile<Model_input>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ','
            );

            // 3. 切割成 訓練(75%) 與 測試(25%)
            var trainTestSplit = mlContext.Data.TrainTestSplit(fullData, testFraction: 0.25);
            var trainData = trainTestSplit.TrainSet;
            var testData = trainTestSplit.TestSet;

            // 4. 建立轉換器(類別->OneHot / 合併特徵等)
            var dataProcessPipeline = mlContext.Transforms.Categorical.OneHotEncoding("CompanyNameEncoded", "CompanyName")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding("BrandEncoded", "Brand"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding("TypeEncoded", "TYPE"))
                .Append(mlContext.Transforms.Concatenate("Features",
                    "CompanyNameEncoded",
                    "BrandEncoded",
                    "TypeEncoded", "ROM",
                    "MobileWeight",
                    "RAM",
                    "BatteryCapacity",
                    "LaunchedYear"
                ));
            //.Append(mlContext.Transforms.CopyColumns("Label", "LaunchedPrice"));
            // 其實我們已在 ModelInput 用 [ColumnName("Label")] 也可以

            // 5. 選擇演算法(例: FastTreeRegression)


            var trainer = mlContext.Regression.Trainers.FastTree(labelColumnName: "Label",featureColumnName: "Features");
            //var trainer = mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
            //var trainer = mlContext.Regression.Trainers.LightGbm(labelColumnName: "Label", featureColumnName: "Features");

            var trainingPipeline = dataProcessPipeline.Append(trainer);

            // 6. 訓練模型
            Console.WriteLine("=== Start training model ===");
            var trainedModel = trainingPipeline.Fit(trainData);

            Console.WriteLine("=== Training finished ===");

            // 7. 評估模型
            var predictions = trainedModel.Transform(testData);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");
            Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError}");
            Console.WriteLine($"MAE: {metrics.MeanAbsoluteError}");
            Console.WriteLine($"R^2: {metrics.RSquared}");

            //// 8. 測試單筆預測(可選)
            //var predEngine = mlContext.Model.CreatePredictionEngine<Model_input, ModelOutput>(trainedModel);
            //var sample = new Model_input
            //{
            //    CompanyName = "Apple",
            //    Processor = "A14 Bionic",
            //    RAM = 6,
            //    Battery = 2815,
            //    Capacity = 128,
            //    LaunchedYear = 2021
            //};
            //var result = predEngine.Predict(sample);
            //Console.WriteLine($"Sample predicted price: {result.Score}");

            // 9. 儲存模型
            mlContext.Model.Save(trainedModel, trainData.Schema, "model.zip");
            Console.WriteLine("=== Model saved to model.zip ===");

        }
    }
}
