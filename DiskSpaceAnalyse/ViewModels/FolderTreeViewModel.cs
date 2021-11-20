using Caliburn.Micro;
using DiskSpaceAnalyse.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace DiskSpaceAnalyse.ViewModels
{
    public class FolderTreeViewModel : Screen
    {
        private readonly INavigationService navigationService;

        private string workState;
        private bool working;
        private bool back;
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
                back = true;
            }

        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (string.IsNullOrEmpty(DiskSpaceUtility.RootPath))
            {
                navigationService.GoBack();
                return;
            }
            DiskSpaceUtility.Analyse = true;
            back = false;
            Task.Run(() =>
            {
                working = true;
                WorkState = "Analysing, please wait";
                var t1 = Environment.TickCount;
                var d = new FolderTreeModel(DiskSpaceUtility.RootPath, FolderTree);
                FolderTree.Children.Add(d);
                d.Analyse();
                WorkState = $"Analyse finish, {(Environment.TickCount - t1) / 1000.0:0.##} s cost";
                working = false;
                if (back)
                {
                    Application.Current.Dispatcher.Invoke(navigationService.GoBack);
                }
            });
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            navigationService.Navigating -= NavigationService_Navigating;
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
