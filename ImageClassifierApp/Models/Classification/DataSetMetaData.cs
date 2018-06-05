using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ImageClassifierApp.Objects.Extensions;
using ImageClassifierApp.Properties;

namespace ImageClassifierApp.Models.Classification
{
    public class DataSetMetaData : INotifyPropertyChanged
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public int Deep
        {
            get
            {
                if (GrayScale)
                    return 1;
                return (ChannelA ? 1 : 0) + (ChannelR ? 1 : 0) + (ChannelG ? 1 : 0) + (ChannelB ? 1 : 0);
            }
        }
        public bool ChannelA { get; set; }
        public bool ChannelR { get; set; }
        public bool ChannelG { get; set; }
        public bool ChannelB { get; set; }

        private bool _grayScale;
        public bool GrayScale
        {
            get => _grayScale;
            set
            {
                _grayScale = value;
                if (value)
                {
                    ChannelA = ChannelR = ChannelG = ChannelB = false;
                }
            }
        }
        public float MinBound { get; set; }
        public float MaxBound { get; set; }

        public string Channels
        {
            get
            {
                if (GrayScale)
                    return $"gray scale ({Deep})";
                return (ChannelA ? "A" : string.Empty) + (ChannelR ? "R" : string.Empty) +
                       (ChannelG ? "G" : string.Empty) + (ChannelB ? "B" : string.Empty) + $"({Deep})";
            }

        }
        public int OutputSize { get; set; }
        public string Description => $"ch:{Channels} h:{Height}px, w:{Width}px, [{MinBound}, {MaxBound}]";

        public List<string> ClassLabels { get; private set; }

        public DataSetMetaData()
        {
            ChannelA = ChannelR = ChannelG = ChannelB = true;
            Height = Width = 28;
            MaxBound = 1f;
            MinBound = -1f;
            ClassLabels = new List<string>();
            this.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            const string key = "Channel";
            if (propertyChangedEventArgs.PropertyName.StartsWith(key))
            {
                var channelValue = this.GetType().GetProperty(propertyChangedEventArgs.PropertyName)?.GetValue(this) as bool?;
                if (channelValue == true)
                {
                    GrayScale = false;
                }
            }
            var invalid = this.GetType().GetProperties().Where(p => p.PropertyType == typeof(bool)).All(p => ((bool)p.GetValue(this)) == false);
            if (invalid)
                this.GetType().GetProperties().Where(p => p.PropertyType == typeof(bool) && p.Name.StartsWith(key)).ForEach(p => p.SetValue(this, true)).ExecuteQuery();
        }

        public bool IsCompatible(DataSetMetaData paMetaData)
        {
            return
                Height == paMetaData.Height &&
                Width == paMetaData.Width &&
                Deep == paMetaData.Deep &&
                ChannelA == paMetaData.ChannelA &&
                ChannelR == paMetaData.ChannelR &&
                ChannelG == paMetaData.ChannelG &&
                ChannelB == paMetaData.ChannelB &&
                OutputSize == paMetaData.OutputSize;
        }

        public void SetClassLabels(string[] paLabels)
        {
            ClassLabels = new List<string>(paLabels);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
