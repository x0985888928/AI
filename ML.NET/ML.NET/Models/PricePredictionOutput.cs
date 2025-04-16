using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ML.NET.Models
{
    public class PricePredictionOutput
    {
        public float Score { get; set; }
    }
}
