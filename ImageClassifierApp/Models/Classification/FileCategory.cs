using System.Collections.Generic;

namespace ImageClassifierApp.Models.Classification
{
    public class FileCategory
    {
        public string CategoryName { get; set; }
        public LinkedList<FileInfo> CategoryFiles { get; set; }

        public FileCategory()
        {
            CategoryFiles = new LinkedList<FileInfo>();
        }

        public void AddToCategory(FileInfo paFileInfo)
        {
            CategoryFiles.AddLast(paFileInfo);
        }
    }
}
