using System;
using System.Collections.Generic;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Classification;
using PropertyChanged;

namespace ImageClassifierApp.Services.Async
{
    /// <summary>
    /// Class represents dataset which loading is in progress
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class PendingDataSet
    {
        public PendingDataSet(IEnumerable<Dir> paDirs, DataSetModel paModel)
        {
            Dirs = new LinkedList<Dir>(paDirs);
            Model = paModel;
            foreach (var dir in Dirs)
            {
                Model.FileCategories.Add(new FileCategory() { CategoryName = dir.DirName });
            }
        }

        public string DataSetName => Model?.Name;
        public int Progress { get; set; }
        public bool Cancel { get; private set; }
        public LinkedList<Dir> Dirs { get; }
        public DataSetModel Model { get; }
        public bool OutOfMemory { get; private set; }
        public bool Exception { get; private set; }
        public Exception ExceptionDescription { get; set; }

        public void CancelLoading()
        {
            Cancel = true;
        }

        public void OutOfMemoryHappend(Exception paException)
        {
            ExceptionDescription = paException;
            Cancel = true;
            OutOfMemory = true;
        }

        public void ExceptionHappend(Exception paException)
        {
            ExceptionDescription = paException;
            Cancel = true;
            Exception = true;
        }
    }
}