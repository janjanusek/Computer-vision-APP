using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AiSdk.Configuration.Configurator;
using AiSdk.NeuralNet.Gpu;
using AiSdk.NeuralNet.Interfaces;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Objects.Extensions;

namespace ImageClassifierApp.Models.NetModel
{
    public interface IModelVisualizer
    {
        IEnumerable<LayerInfo> Layers { get; }
    }
    
    public class NetModelCreator : NetModelBase
    {
        public override bool CanBeChanged => true;
        public ObservableCollection<LayerInfo> ModelLayers { get; set; }

        public NetModelCreator()
        {
            ModelLayers = new ObservableCollection<LayerInfo>();
        }

        public NetModelCreator(NetModelBase paModel) : this()
        {
            paModel.Layers.ForEach(l =>
            {
                var layer = l.Clone();
                layer.ChangeModel(this);
                ModelLayers.Add(layer);

            }).ExecuteQuery();
            ModelName = (string) paModel.ModelName.Clone();
        }

        public void AddLayer(LayerInfo paLayerInfo)
        {
            ModelLayers.Add(paLayerInfo.Clone());
            OnPropertyChanged(nameof(ModelLayers));
        }

        public void RemoveLayer(LayerInfo paLayerInfo)
        {
            ModelLayers.Remove(paLayerInfo);
            OnPropertyChanged(nameof(ModelLayers));
        }

        public bool Verify()
        {
            try
            {
                GenerateModel().ConfigurationModel.ValidateModel();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public PlainNetModel GenerateModel()
        {
            return new PlainNetModel()
            {
                ModelName = this.ModelName,
                ConfigurationModel = new NeuralNetConfigurationModel(ModelLayers.Select(li => li.Layer).ToArray())
            };
        }

        public override INetwork GetNetwork(GpuCard paGpuCard, DataSetModel paDataSetModel)
        {
            var model = this.GenerateModel().ConfigurationModel;
            try
            {
                model.ValidateModel();
                return model.GenerateNetwork();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override IEnumerable<LayerInfo> Layers => ModelLayers;
    }
}