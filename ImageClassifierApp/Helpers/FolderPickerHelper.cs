using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ImageClassifierApp.Services.Notifications;
using FileInfo = ImageClassifierApp.Models.Classification.FileInfo;

namespace ImageClassifierApp.Helpers
{
    /// <summary>
    /// Helper class for picking folders in OS
    /// </summary>
    public class FolderPickerHelper
    {
        public static string LastPickedFolderName { get; private set; }

        /// <summary>
        /// Method shows default Folder picker and automatically do shallow mirror of selected directory into Dir class instance
        /// </summary>
        /// <returns></returns>
        public static List<Dir> PickFolder()
        {
            LastPickedFolderName = null;
            var lDirs = new List<Dir>();
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    LastPickedFolderName = fbd.SelectedPath.Split('\\').LastOrDefault();
                    var dirs = Directory.GetDirectories(fbd.SelectedPath);
                    foreach (var dirPath in dirs)
                    {
                        var dir = new Dir(dirPath);
                        if (dir.Length != 0)
                            lDirs.Add(dir);
                    }
                }
            }
            return lDirs;
        }



        /// <summary>
        /// Method shows default Folder picker and automatically do shallow mirror of selected directory into Dir class instance
        /// </summary>
        /// <returns></returns>
        public static string PickFolderPath()
        {
            LastPickedFolderName = null;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
                else
                    return null;
            }
        }
    }

    public class Dir : IEnumerable<FileInfo>
    {
        private readonly LinkedList<FileInfo> _files;
        public string DirName { get; }

        public int Length => _files.Count;

        public Dir(string paPath)
        {
            DirName = paPath.Split('\\').Last();
            _files = new LinkedList<FileInfo>();
            try
            {
                LoadFiles(paPath);
            }
            catch (Exception e)
            {
                NotifyUser.NotifyUserByNotification(new Notification()
                {
                    Title = "Ooops!",
                    Message = "It seems that there showed up a problem during loading files. Make sure tha you have all privilegies.",
                    Exception = e
                });
            }
        }

        private static readonly string[] Filter = new[] { "png", "jpeg", "jpg", "bmp" };

        private void LoadFiles(string paPath)
        {
            foreach (var file in Directory.GetFiles(paPath).Where(p => Filter.Any(p.EndsWith)))
            {
                _files.AddLast(new FileInfo(file));
            }
        }

        public IEnumerator<FileInfo> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
