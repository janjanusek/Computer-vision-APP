using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ImageClassifierApp.Properties;

namespace ImageClassifierApp.Controls.Buttons
{
    public class ButtonModel : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public ICommand Command { get; set; }
        public object CommandParam { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public UIElement Button { get; protected set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
