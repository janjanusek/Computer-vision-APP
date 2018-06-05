using System.Collections.Generic;
using System.Linq;
using AiSdk.Configuration.Configurator;
using AiSdk.NeuralNet.Gpu;
using AiSdk.NeuralNet.Interfaces;
using ImageClassifierApp.Models.Classification;
using PropertyChanged;

namespace ImageClassifierApp.Models.NetModel
{
    [AddINotifyPropertyChangedInterface]
    public class PlainNetModel : NetModelBase
    {
        public override bool CanBeChanged => false;
        protected override IConfigurationModel ConfigurationFromJson(string paJson)
        {
            return ConfigurationModelBase.FromJson<NeuralNetConfigurationModel>(paJson);
        }

        public override INetwork GetNetwork(GpuCard paGpuCard, DataSetModel paDataSetModel)
        {
            var metaData = paDataSetModel.MetaData;
            var model = (NeuralNetConfigurationModel)ConfigurationModel.Clone();
            model.InitInputAndOutput(metaData.Width, metaData.Height, metaData.Deep, metaData.OutputSize);
            if (paGpuCard != null && paGpuCard is NullGpu == false)
            {
                model.UseGpuCard(paGpuCard);
            }
            var network = model.GenerateNetwork();
            GeneratedNetworkType = model.NetworkTypeName;
            return network;
        }

        public override IEnumerable<LayerInfo> Layers => ConfigurationModel?.Select(ml => new LayerInfo(ml, this));
    }
}