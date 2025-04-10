using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPricePrediction
{
    internal class Model_input
    {

        //Front Camera	Back Camera	Processor Screen Size

        [LoadColumn(0)]
        public string CompanyName { get; set; } = default!;

        [LoadColumn(1)]
        public string Brand { get; set; } = default!;
        [LoadColumn(2)]
        public string Version { get; set; } = default!;

        [LoadColumn(3)]
        public string TYPE { get; set; } = default!;

        [LoadColumn(4)]
        public float ROM { get; set; } = default!;

        [LoadColumn(5)]
        public float MobileWeight { get; set; } = default!;

        [LoadColumn(6)]
        public float RAM { get; set; }  // 例如已把 '8GB' -> 8

        [LoadColumn(7)]
        public float BatteryCapacity { get; set; }

        [LoadColumn(8)]
        public float LaunchedYear { get; set; }

        [LoadColumn(9)]
        [ColumnName("Label")]  // 這是我們要預測的目標欄位
        public float LaunchedPrice { get; set; }

    }
}
