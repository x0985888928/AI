using ML.NET.Models;
using Microsoft.ML;

namespace ML.NET.Services
{
    public class PricePredictionService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly PredictionEngine<PricePredictionInput, PricePredictionOutput> _predictionEngine;

        public PricePredictionService(IConfiguration configuration)
        {
            // 1. 建立 MLContext
            _mlContext = new MLContext();

            // 2. 組合 model.zip 的實際路徑
            //    假設 model.zip 放在專案下的 MLModels 目錄
            var modelFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "MLModels",
                "model.zip"
            );

            // 3. 載入模型
            using var stream = File.OpenRead(modelFilePath);
            _model = _mlContext.Model.Load(stream, out var inputSchema);

            // 4. 創建 PredictionEngine
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<PricePredictionInput, PricePredictionOutput>(_model);
        }

        // 提供一個方法用於預測
        public float Predict(PricePredictionInput input)
        {
            var result = _predictionEngine.Predict(input);
            return result.Score;
        }
    }
}
