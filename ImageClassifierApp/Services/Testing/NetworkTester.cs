using System;
using System.Collections.Generic;
using System.Linq;
using AiSdk.NeuralNet.Interfaces;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Objects.Extensions;

namespace ImageClassifierApp.Services.Testing
{
    public class NetworkTester
    {
        private INetwork _network;
        private DataSetModel _dataSet;
        public LinkedList<NetworkTesterResult> Results { get; private set; }
        public float Correctness { get; set; }


        public class NetworkTesterResult
        {
            public double[] DecisionPercentage { get; private set; }
            public int BelongsTo { get; private set; }
            public int GuessedBelongsTo { get; private set; }

            public NetworkTesterResult(double[] paDecision, int paBelongsTo)
            {
                DecisionPercentage = paDecision;
                BelongsTo = paBelongsTo;
                GuessedBelongsTo = DecisionPercentage.IndexOf(DecisionPercentage.Max());
            }
        }

        public NetworkTester(INetwork paNetwork, DataSetModel paDataSet)
        {
            _network = paNetwork;
            _dataSet = paDataSet;
            Results = new LinkedList<NetworkTesterResult>();
        }

        public void RunTest()
        {
            Results.Clear();
            var testData = _dataSet.DataSet;
            for (int i = 0; i < testData.Length; i++)
            {
                for (int j = 0; j < testData[i].Length; j++)
                {
                    var answer = _network.Guess(testData[i][j]);
                    var sum = answer.Sum(n => Math.Exp(n));
                    var decision = answer.Select(n => Math.Exp(n) / sum).ToArray();
                    this.Results.AddLast(new NetworkTesterResult(decision, i));
                }
            }
            Correctness = (float) (Results.Sum(r => r.BelongsTo == r.GuessedBelongsTo ? 1 : 0) * 1.0 / Results.Count );
        }

        public int[][] GenerateConfusionMatrixData()
        {
            var matrix = new int[_dataSet.FileCategories.Count][];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new int[matrix.Length];
            }

            Correctness = 0;
            foreach (var networkTesterResult in Results)
            {
                matrix[networkTesterResult.BelongsTo][networkTesterResult.GuessedBelongsTo]++;
                if (networkTesterResult.BelongsTo == networkTesterResult.GuessedBelongsTo)
                    Correctness++;
            }
            Correctness /= this.Results.Count;
            return matrix;
        }
    }
}
