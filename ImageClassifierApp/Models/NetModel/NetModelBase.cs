using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AiSdk.Configuration.Configurator;
using AiSdk.Configuration.LayerModels;
using AiSdk.NeuralNet.Gpu;
using AiSdk.NeuralNet.Interfaces;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Properties;
using Newtonsoft.Json;
using PropertyChanged;

namespace ImageClassifierApp.Models.NetModel
{
    [AddINotifyPropertyChangedInterface]
    public abstract class NetModelBase : IModelVisualizer, INotifyPropertyChanged
    {
        [JsonIgnore]
        public ObservableCollection<LayerInfo> SupportedLayers { get; }
        [JsonIgnore]
        public string GeneratedNetworkType { get; set; }

        [JsonIgnore]
        protected static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            TypeNameHandling = TypeNameHandling.All
        };

        protected NetModelBase()
        {
            SupportedLayers = new ObservableCollection<LayerInfo>()
            {
                new LayerInfo(new NeuralNetConvReluLayerModel(), this),
                new LayerInfo(new NeuralNetPoolingLayerModel(), this),
                new LayerInfo(new NeuralNetHiddenLayerModel(), this),
                new LayerInfo(new NeuralNetDropoutLayerModel(), this),
                new LayerInfo(new NeuralNetOutputLayerModel(), this)
            };
        }

        public string ModelName { get; set; }
        public string ModelJson { get; set; }

        [JsonIgnore]
        public IConfigurationModel ConfigurationModel { get; set; }
        public abstract INetwork GetNetwork(GpuCard paGpuCard, DataSetModel paDataSetModel);

        public virtual string GetFileType()
        {
            throw new NotImplementedException();
        }
        
        public abstract IEnumerable<LayerInfo> Layers { get; }
        public virtual bool CanBeChanged => false;

        public string ToJson()
        {
            ModelJson = ConfigurationModel.ToJson();
            return JsonConvert.SerializeObject(this, JsonSettings);
        }

        protected virtual IConfigurationModel ConfigurationFromJson(string paJson)
        {
            return ConfigurationModelBase.FromJson<ConfigurationModelBase>(paJson);
        }

        public static TNetModel FromJsonToInstance<TNetModel>(string paJson) where TNetModel : NetModelBase
        {
            var model = JsonConvert.DeserializeObject<TNetModel>(paJson, JsonSettings);
            model.ConfigurationModel = model.ConfigurationFromJson(model.ModelJson);
            model.ModelJson = null;
            return model;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NetModelBaseDesign : NetModelBase
    {
        public override INetwork GetNetwork(GpuCard paGpuCard, DataSetModel dataSetModel)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<LayerInfo> Layers => new LayerInfo[0];
    }
}