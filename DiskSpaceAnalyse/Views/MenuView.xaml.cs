using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DiskSpaceAnalyse.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiskSpaceAnalyse.Views
{
    /// <summary>
    /// MenuView.xaml 的交互逻辑
    /// </summary>
    public partial class MenuView : Page
    {
        private bool loaded;
        private FolderTreeView folderTreeView;
        public MenuView()
        {
            InitializeComponent();
            folderTreeView = new FolderTreeView();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (loaded) {
                return;
            }
            loaded = true;
            var drivews = DriveInfo.GetDrives();
            foreach (var item in drivews)
            {
                if (item.DriveType == DriveType.Network)
                {
                    continue;
                }
                var control = new FeatureView();
                control.ViewModel.Click += ViewModel_Click;
                control.ViewModel.Title = item.Name;
                control.ViewModel.Description = DiskSpaceUtility.GetDiskDetail(item.TotalSize, item.TotalFreeSpace);
                control.ViewModel.RootPath = item.Name;
                Features.Items.Add(control);
            }
            var folder = new FeatureView();
            folder.ViewModel.Click += ViewModel_Click;
            folder.ViewModel.Title = "Select a Folder";
            folder.ViewModel.Description = "Select a folder to analyse";
            folder.ViewModel.RootPath = string.Empty;
            Features.Items.Add(folder);
        }

        private void ViewModel_Click(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                using (var dilog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true
                })
                {
                    var d = dilog.ShowDialog();
                    if (d == CommonFileDialogResult.Ok)
                    {
                        folderTreeView.ViewModel.RootPath = dilog.FileName;
                        NavigationService.Navigate(folderTreeView);
                    }
                }
            }
            else
            {
                folderTreeView.ViewModel.RootPath = rootPath;
                NavigationService.Navigate(folderTreeView);
            }
        }
    }
}
