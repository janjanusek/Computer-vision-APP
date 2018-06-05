using System.IO;
using Microsoft.Win32;

namespace ImageClassifierApp.Helpers
{
    public class FilePickerHelper
    {
        /// <summary>
        /// Method invokes default file picker and save string content into 
        /// specified file path with specified file type
        /// </summary>
        /// <param name="paContent"></param>
        /// <param name="paFileType"></param>
        public static void SaveFile(string paContent, string paFileType)
        {
            var path = AskForFilePath(paFileType);
            if (string.IsNullOrEmpty(path))
                return;
            if(File.Exists(path))
                File.Delete(path);
            using (var stream = File.CreateText(path))
            {
                stream.Write(paContent);
            }
        }

        /// <summary>
        /// Method launch default file picker and ask user 
        /// for file path of specified file type
        /// </summary>
        /// <param name="paFileType"></param>
        /// <returns></returns>
        public static string AskForFilePath(string paFileType)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = $"{paFileType} (*.{paFileType.ToLowerInvariant()})|*.{paFileType.ToLowerInvariant()};"
            };
            if (saveFileDialog.ShowDialog() == true)
                return saveFileDialog.FileName;
            return null;
        }
    }
}
