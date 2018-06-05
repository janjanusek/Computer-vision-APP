using System.Linq;
using PropertyChanged;

namespace ImageClassifierApp.Models.Classification
{
    [AddINotifyPropertyChangedInterface]
    public class FileInfo
    {
        private string _path;
        public string FilePath
        {
            get => _path;
            set
            {
                _path = value;
                if (string.IsNullOrEmpty(value) == false)
                {
                    FileName = FilePath?.Split('\\').LastOrDefault();
                    Name = FilePath?.Split('.').FirstOrDefault();
                }
            }
        }
        public string FileName { get; private set; }
        public string Name { get; private set; }

        public FileInfo()
        {
            
        }

        public FileInfo(string paPath)
        {
            FilePath = paPath;
        }
    }
}
