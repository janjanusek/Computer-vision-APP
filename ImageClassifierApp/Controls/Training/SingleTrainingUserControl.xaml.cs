using System.Windows.Controls;
using ImageClassifierApp.Helpers;

namespace ImageClassifierApp.Controls.Training
{
    /// <summary>
    /// Interaction logic for SingleTrainingUserControl.xaml
    /// </summary>
    public partial class SingleTrainingUserControl : UserControl
    {
        public SingleTrainingUserControl()
        {
            InitializeComponent();
            ListControlHelper.RegisterAutoSelect(ComboBox);
        }
    }
}
