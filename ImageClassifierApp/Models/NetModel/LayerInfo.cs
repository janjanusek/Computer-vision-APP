using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AiSdk.Configuration.Enums;
using AiSdk.Configuration.LayerModels;
using ImageClassifierApp.Objects.Extensions;
using ImageClassifierApp.Properties;

namespace ImageClassifierApp.Models.NetModel
{
    public class LayerInfo : INotifyPropertyChanged
    {
        public bool CanBeChanged => _model.CanBeChanged;
        private NetModelBase _model;
        public bool IsReadOnly => !_model.CanBeChanged;

        public LayerInfo(ModelLayerBase paLayer, NetModelBase paNetModelBase)
        {
            _model = paNetModelBase;
            Layer = paLayer;
            var conv = paLayer as NeuralNetConvReluLayerModel;
            var pooling = paLayer as NeuralNetConvolvingLayerModel;
            var output = paLayer as NeuralNetOutputLayerModel;
            var hidden = paLayer as FullyConectedLayerModel;
            if (conv != null || pooling != null)
            {
                Name = conv != null ? "conv relu layer" : "max pooling layer";
            }
            else if (output != null || hidden != null)
            {
                Name = output != null ? "output FCN layer" : "hidden FCN layer";
            }
            else if (paLayer is NeuralNetDropoutLayerModel)
            {
                Name = "dropout layer";
            }
            if (Layer != null)
                Layer.PropertyChanged += LayerOnPropertyChanged;
        }

        private void LayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.GetType()
                .GetProperties()
                .Where(p => !(p.GetValue(this) is NeuralNetModelLayer))
                .ForEach(p => OnPropertyChanged(p.Name))
                .ExecuteQuery();
            OnPropertyChanged(XamlValue1);
            OnPropertyChanged(XamlValue2);
        }

        public void ChangeModel([NotNull] NetModelBase paModel)
        {
            if (_model == null)
                throw new Exception("This method is only for changing model. If you want to add model please use constructor of the class.");
            _model = paModel;
        }

        public string XamlValue1
        {
            get
            {
                switch (Layer)
                {
                    case NeuralNetDropoutLayerModel dropout:
                        {
                            return $"ignored neurons from previous layer {dropout.IgnoredRate * 100} %";
                        }
                    case NeuralNetConvReluLayerModel convRelu:
                        {
                            return $"filters: {convRelu.Filters}, size {convRelu.FilterSize}x{convRelu.FilterSize}, padding: {convRelu.ZerroPadding}, stride: {convRelu.Stride}, norm: [ {convRelu.MinWeightBound} | {convRelu.MaxWeightBound} ]";
                        }
                    case NeuralNetOutputLayerModel output:
                        {
                            return $"norm: [ {output.MinWeightBound} | {output.MaxWeightBound} ]";
                        }
                    case NeuralNetHiddenLayerModel hidden:
                        {
                            return $"neurons: {hidden.Neurons}, norm: [ {hidden.MinWeightBound} | {hidden.MaxWeightBound} ]";
                        }
                    case NeuralNetPoolingLayerModel pooling:
                        {
                            return $"stride: {pooling.Stride}, filter size: {pooling.FilterSize}x{pooling.FilterSize}";
                        }
                }
                return null;
            }
        }
        public string XamlValue2
        {
            get
            {
                switch (Layer)
                {
                    case NeuralNetDropoutLayerModel neuralNetDropoutLayerModel:
                        {
                            return null;
                        }
                    case NeuralNetConvReluLayerModel neuralNetConvReluLayerModel:
                        {
                            return $"relu";
                        }
                    case FullyConectedLayerModel fullyConectedLayerModel:
                        {
                            return Activation?.ToString().InsertSpaceBeforeUpperCase() +
                                   $" {ErrorFunction?.ToString().InsertSpaceBeforeUpperCase()}";
                        }
                    case NeuralNetPoolingLayerModel neuralNetPoolingLayerModel:
                        {
                            return "max";
                        }
                }
                return null;
            }
        }

        public string Name { get; set; }
        public int? Filters => (Layer as NeuralNetConvReluLayerModel)?.Filters;
        public int? Neurons => (Layer as FullyConectedLayerModel)?.Neurons;
        public Activations? Activation => (Layer as FullyConectedLayerModel)?.Activation;
        public ErrorFunctions? ErrorFunction => (Layer as NeuralNetOutputLayerModel)?.ErrorFunction;
        public int? FiltersSize => (Layer as NeuralNetConvolvingLayerModel)?.FilterSize;
        public int? Stride => (Layer as NeuralNetConvolvingLayerModel)?.Stride;
        public int? Padding => (Layer as NeuralNetConvReluLayerModel)?.ZerroPadding;

        public float? MinBound => (Layer as NeuralNetModelLayer)?.MinWeightBound;
        public float? MaxBound => (Layer as NeuralNetModelLayer)?.MaxWeightBound;

        public ModelLayerBase Layer { get; set; }

        public LayerInfo Clone()
        {
            return new LayerInfo(Layer.Clone(), _model);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}