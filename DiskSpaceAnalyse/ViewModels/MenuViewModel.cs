using Caliburn.Micro;
using System.IO;
using System.Windows.Forms;
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
                Features.Add(new FeatureViewModel(item.Name, DiskSpaceUtility.GetDiskDetail(item.TotalSize, item.TotalFreeSpace), item.Name));
            }
            Features.Add(new FeatureViewModel("Select a Folder", "Select a folder to analyse", string.Empty));
        }

        public BindableCollection<FeatureViewModel> Features { get; }

        public void ShowFeature(FeatureViewModel feature)
        {
            if (feature != null)
            {
                DiskSpaceUtility.RootPath = feature.RootPath;
                if (string.IsNullOrEmpty(feature.RootPath))
                {
                    using (var dilog = new FolderBrowserDialog
                    {
                        Description = "Select a folder to analyse"
                    })
                    {
                        var d = dilog.ShowDialog();
                        if (d == DialogResult.OK || d == DialogResult.Yes)
                        {
                            DiskSpaceUtility.RootPath = dilog.SelectedPath;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(DiskSpaceUtility.RootPath))
                {
                    navigationService.NavigateToViewModel(typeof(FolderTreeViewModel));
                }
            }
        }
    }
}