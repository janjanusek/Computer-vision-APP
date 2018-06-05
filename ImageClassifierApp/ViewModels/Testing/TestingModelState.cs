using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Properties;

namespace ImageClassifierApp.ViewModels.Testing
{
    /// <summary>
    /// Because testing is possible on file model and on live training istance as well,
    /// This class represents one of mentioned strategies (design pattern strategy) for better readability
    /// </summary>
    public abstract class TestingStateHandlerBase : INotifyPropertyChanged, IDisposable
    {
        public event EventHandler Disposing;
        private readonly TestingViewModel _testingViewModel;
        public virtual DataSetMetaData DataSetMetaData => null;
        public DataSetModel DataSetModel => _testingViewModel.TestingDataSet;
        public virtual string ModelName => null;
        private float _successRatio;
        public float SuccessRatio
        {
            get => _successRatio;
            set => _successRatio = (float) (Math.Round(value, 6) * 100f);
        }

        public bool IsWorking
        {
            set { App.RunInUiThread(() => { _testingViewModel.IsWorking = value; }); }
        }

        protected TestingStateHandlerBase(TestingViewModel paTestingViewModel)
        {
            _testingViewModel = paTestingViewModel;
        }

        public abstract Task<int[][]> GenerateConfusionMatrix();
        public abstract Task Init(object paData);

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void Dispose();

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnDisposing()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }
    }
}
