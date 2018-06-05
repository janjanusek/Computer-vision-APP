using System.Collections.Generic;
using System.Linq;

namespace ImageClassifierApp.Controls.DragAndDrop
{
    public class FileTypeModel
    {
        public string Description { get; set; }
        private readonly List<string> _fileTypes;
        public bool Any => _fileTypes.Any();
        public string[] FileTypes => _fileTypes.ToArray();

        public FileTypeModel()
        {
            _fileTypes = new List<string>();
        }

        public FileTypeModel(params string[] paFileTypes) : this()
        {
            foreach (var fileType in paFileTypes)
            {
                AddFileType(fileType);
            }
        }

        public void AddFileType(string paType)
        {
            _fileTypes.Add($"{paType.ToLower().Replace(".", string.Empty)}");
        }

        public override string ToString()
        {
            return $"{Description} | {string.Join(" ", _fileTypes.Select(t=> $"*.{t};"))}";
        }
    }
}
