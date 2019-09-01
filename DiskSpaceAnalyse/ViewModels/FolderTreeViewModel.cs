using Caliburn.Micro;
using DiskSpaceAnalyse.Models;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;

namespace DiskSpaceAnalyse.ViewModels
{
    public class FolderTreeViewModel : Screen
    {
        private readonly INavigationService navigationService;

        private Thread analyse;
        private string workState;
        private bool working;
        public FolderTreeModel FolderTree { get; } = new FolderTreeModel(string.Empty, null);
        public string WorkState
        {
            get => workState;
            set
            {
                workState = value;
                NotifyOfPropertyChange(() => WorkState);
            }
        }

        public FolderTreeViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            navigationService.Navigating += NavigationService_Navigating;
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (working && e.NavigationMode == NavigationMode.Back)
            {
                DiskSpaceUtility.Analyse = false;
                e.Cancel = true;
                UIThreadHelper.GetInstance().AddWork(() =>
                {
                    if (analyse != null)
                    {
                        analyse.Join();
                    }
                    Application.Current.Dispatcher.Invoke(() => navigationService.GoBack()); ;

                });
            }

        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            DiskSpaceUtility.Analyse = true;

            analyse = new Thread(() =>
            {
                working = true;
                WorkState = "Analysing, please wait";
                var t1 = Environment.TickCount;
                var d = new FolderTreeModel(DiskSpaceUtility.RootPath, FolderTree);
                FolderTree.Children.Add(d);
                d.Analyse();
                WorkState = $"Analyse finish, {(Environment.TickCount - t1) / 1000.0:0.##} s cost";
                working = false;
            })
            {
                Name = "Analyse"
            };
            analyse.Start();

        }

        protected override void OnDeactivate(bool close)
        {
            navigationService.Navigating -= NavigationService_Navigating;
            base.OnDeactivate(close);
        }
    }
}
