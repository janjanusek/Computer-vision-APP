using System.Collections.Generic;
using System.Linq;
using AiSdk.Configuration.Configurator;
using AiSdk.NeuralNet.Gpu;
using AiSdk.NeuralNet.Interfaces;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Models.NetModel;
using Newtonsoft.Json;
using PropertyChanged;

namespace ImageClassifierApp.Models.Training
{
    [AddINotifyPropertyChangedInterface]
    public class TrainedNetModel : NetModelBase
    {
        public DataSetMetaData DataSetMetaData { get; set; }

        public TrainedNetModel(MementoConfigurationModel paConfigurationModel)
        {
            ConfigurationModel = paConfigurationModel;
        }

        public override INetwork GetNetwork(GpuCard paGpuCard, DataSetModel paDataSetModel)
        {
            if (DataSetMetaData.IsCompatible(paDataSetModel.MetaData) == false)
                return null;
            if (paGpuCard != null)
                ConfigurationModel.UseGpuCard(paGpuCard);
            var network = ConfigurationModel.GenerateNetwork();
            GeneratedNetworkType = ConfigurationModel.NetworkTypeName;
            return network;
        }

        [JsonIgnore]
        public override IEnumerable<LayerInfo> Layers => ConfigurationModel?.Select(ml => new LayerInfo(ml, this));

        protected override IConfigurationModel ConfigurationFromJson(string paJson)
        {
            return ConfigurationModelBase.FromJson<MementoConfigurationModel>(paJson);
        }
    }
}