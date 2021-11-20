using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Linq;
using Screen = Caliburn.Micro.Screen;

namespace DiskSpaceAnalyse.ViewModels
{
    public class MenuViewModel : Screen
    {
        private readonly INavigationService navigationService;

        public MenuViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            var drivews = DriveInfo.GetDrives();
            Features = new BindableCollection<FeatureViewModel>();
            foreach (var item in drivews)
            {
                if (item.DriveType == DriveType.Network)
                {
                    continue;
                }
                Features.Add(new FeatureViewModel(item.Name, DiskSpaceUtility.GetDiskDetail(item.TotalSize, item.TotalFreeSpace), item.Name));
            }
            Features.Add(new FeatureViewModel("Select a Folder", "Select a folder to analyse", string.Empty));
        }

        public BindableCollection<FeatureViewModel> Features { get; }

        public void ShowFeature(FeatureViewModel feature)
        {
            if (feature != null)
            {
                string last = DiskSpaceUtility.RootPath;
                DiskSpaceUtility.RootPath = feature.RootPath;
                if (string.IsNullOrEmpty(feature.RootPath))
                {
                    using (var dilog = new CommonOpenFileDialog
                    {
                        IsFolderPicker = true
                    })
                    {
                        var d = dilog.ShowDialog();
                        if (d == CommonFileDialogResult.Ok)
                        {
                            DiskSpaceUtility.RootPath = dilog.FileNames.FirstOrDefault();
                        }
                    }
                }
                if (string.IsNullOrEmpty(DiskSpaceUtility.RootPath))
                {
                    DiskSpaceUtility.RootPath = last;
                }
                else
                {
                    navigationService.NavigateToViewModel(typeof(FolderTreeViewModel));
                }
            }
        }
    }
}