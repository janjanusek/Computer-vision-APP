using System;
using System.Linq;
using System.Threading.Tasks;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Services.Image;
using ImageClassifierApp.Services.Normalization;
using PropertyChanged;

namespace ImageClassifierApp.Services.Async
{
    /// <summary>
    /// Class provides auto async load of data set int separate task
    /// When dataset is loaded event is raised
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class DataSetLoader
    {
        private static DataSetLoader _instance;
        public static DataSetLoader Instance => _instance ?? (_instance = new DataSetLoader());
        public event EventHandler<PendingDataSet> ModelLoaded;
        private readonly object _sync = new object();

        private DataSetLoader()
        {
        }

        public async void RegisterDataSet(PendingDataSet paPendingDataSet)
        {
            GcHelper.RegisterForScheduledCollection();
            var task = Task.Run(() => LoadDataSetAsync(paPendingDataSet));
            await task;
            task.Dispose();
            GcHelper.ForceCollect();
            OnModelLoaded(paPendingDataSet);
            GcHelper.UnRegisterScheduledCollection();
        }


        private void LoadDataSetAsync(PendingDataSet paPendingDataSet)
        {
            var pending = paPendingDataSet;
            var allFiles = pending.Dirs.Sum(d => d.Length);
            var dataSet = new float[pending.Dirs.Count][][];
            var i = 0;
            var fileIndex = 0;
            var categories = pending.Model.FileCategories;
            var categoryIndex = 0;
            foreach (var dir in pending.Dirs)
            {
                var category = categories[categoryIndex++];
                dataSet[i] = new float[dir.Length][];
                var j = 0;
                Parallel.ForEach(dir, file =>
                {
                    try
                    {
                        var processor = new ImageProcessor(pending.Model);
                        var normalizer = new NormalizerService(0, 255, pending.Model.MetaData.MinBound, pending.Model.MetaData.MaxBound);
                        if (pending.Cancel)
                            return;
                        lock (_sync)
                        {
                            category.AddToCategory(file);
                        }
                        var pixels = processor.ToPixels(file.FilePath);
                        lock (_sync)
                        {
                            dataSet[i][j++] = normalizer.Normalize(pixels);
                        }
                        pending.Progress =
                            (int)Math.Round(100.0 * fileIndex++ / allFiles, MidpointRounding.AwayFromZero);
                    }
                    catch (OutOfMemoryException e)
                    {
                        pending.OutOfMemoryHappend(e);
                        return;
                    }
                    catch (Exception e)
                    {
                        pending.ExceptionHappend(e);
                    }
                });
                //foreach (var file in dir)
                //{
                //    try
                //    {
                //        if (pending.Cancel)
                //            return;
                //        category.AddToCategory(file);
                //        var pixels = processor.ToPixels(file.FilePath);//GetPixelsFromFile(file.FilePath);
                //        dataSet[i][j++] = normalizer.Normalize(pixels);
                //        pending.Progress =
                //            (int)Math.Round(100.0 * fileIndex++ / allFiles, MidpointRounding.AwayFromZero);
                //    }
                //    catch (OutOfMemoryException e)
                //    {
                //        pending.OutOfMemoryHappend(e);
                //        return;
                //    }
                //    catch (Exception e)
                //    {
                //        pending.ExceptionHappend(e);
                //    }
                //}
                i++;
            }
            pending.Model.SetDataSet(dataSet);
        }


        protected virtual void OnModelLoaded(PendingDataSet e)
        {
            ModelLoaded?.Invoke(this, e);
        }
    }
}