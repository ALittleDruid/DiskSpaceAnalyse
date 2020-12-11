using Caliburn.Micro;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace DiskSpaceAnalyse.Models
{
    public class FolderTreeModel : PropertyChangedBase
    {
        private long size;
        private int folderCount;
        private int fileCount;

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
            get;
        }

        public FolderTreeModel Parent
        {
            get;
        }

        public BindableCollection<FolderTreeModel> Children { get; } = new BindableCollection<FolderTreeModel>();

        public string RootPath { get; }

        public FolderTreeModel(string path, FolderTreeModel parent)
        {
            RootPath = path;
            if (!string.IsNullOrEmpty(path))
            {
                FolderName = Path.GetFileName(path);
            }
            if (string.IsNullOrEmpty(FolderName))
            {
                FolderName = path;
            }
            Parent = parent;
        }

        public void Analyse()
        {
            if (DiskSpaceUtility.Analyse && Parent != null && !string.IsNullOrEmpty(RootPath) && Directory.Exists(RootPath))
            {
                DirectoryInfo di = new DirectoryInfo(RootPath);
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
                    }
                    catch
                    {
                    }
                    var tmp = new BindableCollection<FolderTreeModel>(Children);
                    Children.Clear();
                    Children.AddRange(tmp.OrderByDescending(x => x.Size));
                    Parent.Size += Size;
                }
            }

        }

        public void OpenFolder(FolderTreeModel model)
        {
            if (model != null && Directory.Exists(model.RootPath))
            {
                Process.Start(model.RootPath);
            }
        }

        public void CopyFolder(FolderTreeModel model)
        {
            if (model != null)
            {
                Clipboard.SetDataObject(model.RootPath);
            }
        }

        public async Task DeleteFolder(FolderTreeModel model)
        {
            await DeleteFolderAsync(model);
        }

        private async Task DeleteFolderAsync(FolderTreeModel model)
        {
            if (model != null && Directory.Exists(model.RootPath))
            {
                try
                {
                    int size = IntPtr.Size;
                    IntPtr intPtr;
                    Type type;
                    int len;
                    if (size == 4)
                    {
                        len = Marshal.SizeOf<SHFILEOPSTRUCTWWIN32>();
                        type = typeof(SHFILEOPSTRUCTWWIN32);
                        SHFILEOPSTRUCTWWIN32 tmp = new SHFILEOPSTRUCTWWIN32();
                        intPtr = Marshal.AllocHGlobal(len);
                        tmp.Func = 3;
                        tmp.From = model.RootPath + "\0";
                        tmp.Flags = 0x0040;
                        Marshal.StructureToPtr(tmp, intPtr, false);
                    }
                    else
                    {
                        len = Marshal.SizeOf<SHFILEOPSTRUCTWWIN64>();
                        type = typeof(SHFILEOPSTRUCTWWIN64);
                        SHFILEOPSTRUCTWWIN64 tmp = new SHFILEOPSTRUCTWWIN64();
                        intPtr = Marshal.AllocHGlobal(len);
                        tmp.Func = 3;
                        tmp.From = model.RootPath + "\0";
                        tmp.Flags = 0x0040;
                        Marshal.StructureToPtr(tmp, intPtr, false);
                    }

                    int n = await Task.Run(() => Win32API.SHFileOperationW(intPtr));
                    Marshal.DestroyStructure(intPtr, type);
                    Marshal.FreeHGlobal(intPtr);
                    if (n == 0)
                    {
                        FolderTreeModel p = Parent;
                        if (p != null)
                        {
                            p.Children.Remove(model);
                            p.FolderCount--;
                            while (p != null)
                            {
                                p.Size -= model.Size;
                                p = p.Parent;
                            }
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
