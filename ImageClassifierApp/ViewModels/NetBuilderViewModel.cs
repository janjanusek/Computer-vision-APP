using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using ImageClassifierApp.Commands;
using ImageClassifierApp.Controls.Builder;
using ImageClassifierApp.Controls.Buttons;
using ImageClassifierApp.Controls.DragAndDrop;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Objects.Extensions;
using ImageClassifierApp.Services.Notifications;
using ImageClassifierApp.Services.SharedResources;
using PropertyChanged;

namespace ImageClassifierApp.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class NetBuilderViewModel : PartialViewModelBase
    {
        public const string CleanNetModel = "cleanmodel";
        public const string DirtyNetModel = "dirtymodel";
        public ObservableCollection<NetModelBase> LoadedNetModels => SharedDataService.Instance.LoadedModels;
        public NetModelBase SelectedNetModel { get; set; }

        public NetBuilderViewModel()
        {
            Title = "builder";
        }

        protected override void RegisterButtons()
        {
            base.RegisterButton(new CharCodeButtonModel()
            {
                Title = "create net",
                Command = new OpenNetCreatorWindowCommand(),
                CommandParam = this,
                CharCode = char.ConvertFromUtf32(0xE710)
            });
        }

        protected override UserControl GetUserControl()
        {
            return new BuilderUserControl();
        }

        public async void LoadNeuralNet(object sender, DragAndDropEventArgs e)
        {
            IsWorking = true;
            IEnumerable<string> files;
            if (e.FilesPaths.ContainsKey(DirtyNetModel))
            {
                files = e.FilesPaths[DirtyNetModel];
                using (var task = LoadQuery<TrainedNetModel>(files))
                {
                    await task;
                }
            }
            if (e.FilesPaths.ContainsKey(CleanNetModel))
            {
                files = e.FilesPaths[CleanNetModel];
                using (var task = LoadQuery<PlainNetModel>(files))
                {
                    await task;
                }
            }
            IsWorking = false;
        }

        private Task LoadQuery<T>(IEnumerable<string> paItems) where T : NetModelBase
        {
            const string message = "Loading of file '{0}' failed. Make sure that it's not corrupted.";
            return paItems.TryOrDefault<string, Exception>(File.ReadAllText, (ex, s) =>
                {
                    NotifyUser.NotifyUserByMessage(string.Format(message, s));
                })
                .Where(json => string.IsNullOrEmpty(json) == false)
                .Select(ParseModel<T>)
                .Where(m => m != null)
                .ForEachAsync(m => App.RunInUiThread(() =>
                {
                    LoadedNetModels.Add(m);
                }))
                .ExecuteQueryAsync();
        }

        private TModel ParseModel<TModel>(string paJson) where TModel : NetModelBase
        {
            try
            {
                return NetModelBase.FromJsonToInstance<TModel>(paJson);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return default(TModel);
            }
        }
    }
}