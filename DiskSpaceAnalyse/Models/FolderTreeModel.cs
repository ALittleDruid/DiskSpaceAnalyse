using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinCopies.Util;

namespace DiskSpaceAnalyse.Models
{
    [ObservableObject]
    public partial class FolderTreeModel
    {
        [ObservableProperty]
        private long size;
        [ObservableProperty]
        private int folderCount;
        [ObservableProperty]
        private int fileCount;

        public string FolderName
        {
            get;
        }

        public FolderTreeModel Parent
        {
            get;
        }

        public ObservableCollection<FolderTreeModel> Children { get; } = new ObservableCollection<FolderTreeModel>();

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

        public Task Analyse()
        {
            return Task.Run(async () => await EnterDirectory());
        }

        private async Task EnterDirectory()
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
                        var r = dirs.Where(x => (x.Attributes & FileAttributes.ReparsePoint) == 0).Select(x => new FolderTreeModel(x.FullName, this)).ToList();
                        if (r.Any())
                        {
                            var tasks = r.Select(async x => await x.EnterDirectory());
                            await Task.WhenAll(tasks);
                            Application.Current?.Dispatcher.Invoke(() =>
                            {
                                Children.AddRange(r.OrderByDescending(x => x.Size));
                            });
                        }
                        Parent.Size += Size;

                        //var r = dirs.Where(x => (x.Attributes & FileAttributes.ReparsePoint) == 0).Select(x =>
                        //{
                        //    var tmp = new FolderTreeModel(x.FullName, this);
                        //    tmp.EnterDirectory();
                        //    return tmp;
                        //}).OrderByDescending(x => x.Size).ToList();
                        //if (r.Any())
                        //{
                        //    Application.Current.Dispatcher.Invoke(() =>
                        //    {
                        //        Children.AddRange(r);
                        //    });
                        //}
                        //Parent.Size += Size;
                    }
                    catch
                    {
                    }
                }
            }
        }

        [RelayCommand]
        private void OpenFolder()
        {
            if (Directory.Exists(RootPath))
            {
                Process.Start("explorer.exe", RootPath);
            }
        }

        [RelayCommand]
        public void CopyFolder()
        {
            Clipboard.SetDataObject(RootPath);
        }

        [RelayCommand]
        public async Task DeleteFolder()
        {
            await DeleteFolderAsync();
        }

        private async Task DeleteFolderAsync()
        {
            if (Directory.Exists(RootPath))
            {
                try
                {
                    int n = await Task.Run(() =>
                    {
                        SHFILEOPSTRUCTWWIN64 tmp = new SHFILEOPSTRUCTWWIN64
                        {
                            Func = 3,
                            From = RootPath + "\0",
                            Flags = 0x0040
                        };
                        return SHFileOperation(tmp);
                    });
                    if (n == 0)
                    {
                        FolderTreeModel p = Parent;
                        if (p != null)
                        {
                            p.Children.Remove(this);
                            p.FolderCount--;
                            while (p != null)
                            {
                                p.Size -= Size;
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

        [DllImport("Shell32", CharSet = CharSet.Unicode)]
        private static extern int SHFileOperation(in SHFILEOPSTRUCTWWIN64 handle);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public ref struct SHFILEOPSTRUCTWWIN64
    {
        public IntPtr Hwnd;
        public uint Func;
        public string From;
        public string To;
        public ushort Flags;
        public int AnyOperationsAborted;
        public IntPtr NameMappings;
        public string ProgressTitle;
    }
}
