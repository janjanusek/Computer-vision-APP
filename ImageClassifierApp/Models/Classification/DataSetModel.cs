using System.Collections.Generic;
using System.Linq;
using PropertyChanged;

namespace ImageClassifierApp.Models.Classification
{
    /// <summary>
    /// Class represents dataset and it's metadata after preprocessing
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class DataSetModel
    {
        public List<FileCategory> FileCategories { get; set; }
        public string Name { get; set; }
        public int Length => FileCategories?.Sum(c => c.CategoryFiles?.Count ?? 0) ?? 0;
        public float[][][] DataSet { get; private set; }
        public DataSetMetaData MetaData { get; set; }


        public DataSetModel()
        {
            FileCategories = new List<FileCategory>();
            MetaData = new DataSetMetaData();
        }

        public void SetDataSet(float[][][] paDataSet)
        {
            DataSet = paDataSet;
            MetaData.OutputSize = paDataSet.Length;
            MetaData.SetClassLabels(FileCategories.Select(fc => fc.CategoryName).ToArray());
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
