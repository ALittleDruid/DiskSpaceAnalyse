using Caliburn.Micro;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace DiskSpaceAnalyse.Models
{
    public class FolderTreeModel : PropertyChangedBase
    {
        private long size;
        private int fileCount;
        private int folderCount;
        private string folderName;

        public long Size
        {
            get => size;
            set
            {
                size = value;
                NotifyOfPropertyChange(() => Size);
            }
        }
        public int FileCount
        {
            get => fileCount;
            set
            {
                fileCount = value;
                NotifyOfPropertyChange(() => FileCount);
            }
        }
        public int FolderCount
        {
            get => folderCount;
            set
            {
                folderCount = value;
                NotifyOfPropertyChange(() => FolderCount);
            }
        }
        public string FolderName
        {
            get => folderName;
            set
            {
                folderName = value;
                NotifyOfPropertyChange(() => FolderName);
            }
        }
        public FolderTreeModel Parent
        {
            get;
            private set;
        }
        public BindableCollection<FolderTreeModel> Children { get; } = new BindableCollection<FolderTreeModel>();

        public FolderTreeModel(string rootPath, FolderTreeModel parent)
        {
            FolderName = rootPath;
            Parent = parent;
        }

        public void Analyse()
        {
            if (DiskSpaceUtility.Analyse && Parent != null && !string.IsNullOrEmpty(FolderName) && Directory.Exists(FolderName))
            {
                DirectoryInfo di = new DirectoryInfo(FolderName);
                if ((di.Attributes & FileAttributes.Directory) != 0)
                {
                    try
                    {
                        var files = di.GetFiles();
                        var dirs = di.GetDirectories();
                        FileCount = files.Length;
                        FolderCount = dirs.Length;
                        foreach (FileInfo fi in files)
                        {
                            Size += fi.Length;
                        }
                        foreach (var item in dirs)
                        {
                            if ((item.Attributes & FileAttributes.ReparsePoint) != 0)
                            {
                                continue;
                            }
                            var d = new FolderTreeModel(item.FullName, this);
                            Children.Add(d);
                        }
                        foreach (var item in Children)
                        {
                            item.Analyse();
                        }
                        var tmp = new BindableCollection<FolderTreeModel>(Children);
                        Children.Clear();
                        Children.AddRange(tmp.OrderByDescending(x => x.Size));
                        Parent.Size += Size;
                    }
                    catch
                    {
                    }
                }
            }

        }

        public void OpenFolder(FolderTreeModel model)
        {
            if (model != null && Directory.Exists(model.FolderName))
            {
                Process.Start(model.FolderName);
            }
        }

        public void CopyFolder(FolderTreeModel model)
        {
            if (model != null)
            {
                Clipboard.SetDataObject(model.FolderName);
            }
        }

        public void DeleteFolder(FolderTreeModel model)
        {
            if (model != null && Directory.Exists(model.FolderName))
            {
                try
                {
                    Directory.Delete(model.FolderName, true);
                    if (Parent != null)
                    {
                        Parent.Children.Remove(model);
                        FolderTreeModel p = Parent;
                        while (p != null)
                        {
                            p.Size -= model.Size;
                            p = p.Parent;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
