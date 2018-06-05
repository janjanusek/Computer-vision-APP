using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using ImageClassifierApp.Controls.Buttons;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Properties;
using ImageClassifierApp.ViewModels.Testing;
using PropertyChanged;

namespace ImageClassifierApp.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : IDisposable
    {
        public PartialViewModelBase CurrentPartialViewModel { get; set; }
        public ObservableCollection<PartialViewModelBase> ViewModels { get; set; }
        public int HeapAllocated { get; set; }
        private DispatcherTimer _timer;

        public MainViewModel()
        {
            ViewModels = new ObservableCollection<PartialViewModelBase>()
            {
                new DatasetsViewModel(),
                new NetBuilderViewModel(),
                new TrainingNetViewModel(),
                new TestingViewModel()
            };
            CurrentPartialViewModel = ViewModels.First();

            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _timer.Tick += TimerOnTickGetHeapAllocated;
            _timer.Start();
        }

        /// <summary>
        /// Updated info about current ussage of GC heap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void TimerOnTickGetHeapAllocated(object sender, EventArgs eventArgs)
        {
            HeapAllocated = GcHelper.GetHeapAllocatedMemory();
        }

        public void SelectViewModel(PartialViewModelBase paViewModelBase)
        {
            App.RunInUiThread(() =>
            {
                CurrentPartialViewModel = paViewModelBase;
            });
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= TimerOnTickGetHeapAllocated;
                _timer = null;
            }
        }
    }

    /// <summary>
    /// Every view model showed as component of the main MainWindow must be of this type
    /// because this type defines basic structure of UI such as top buttons, title
    /// if there is currently some long running task in progress and user control related to the ViewModel
    /// </summary>
    public abstract class PartialViewModelBase : INotifyPropertyChanged
    {
        public ObservableCollection<ButtonModel> SupportedButtons { get; }

        public UserControl View { get; private set; }

        public string Title { get; protected set; }
        public bool IsWorking { get; set; }

        protected PartialViewModelBase()
        {
            SupportedButtons = new ObservableCollection<ButtonModel>();
        }

        public virtual Task Init()
        {
            if (View == null)
            {
                View = GetUserControl();
                View.DataContext = this;
            }
            if (SupportedButtons.Any() == false)
                RegisterButtons();
            return Task.CompletedTask;
        }

        public virtual Task<bool> SetData(object paData)
        {
            return Task.Run(() => false);
        }

        protected abstract void RegisterButtons();

        protected void RegisterButton(ButtonModel paButton)
        {
            SupportedButtons.Add(paButton);
        }

        protected abstract UserControl GetUserControl();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
