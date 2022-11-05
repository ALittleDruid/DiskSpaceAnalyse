using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiskSpaceAnalyse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace DiskSpaceAnalyse.ViewModels
{
    [ObservableObject]
    public partial class FolderTreeViewModel
    {
        public FolderTreeModel FolderTree { get; } = new FolderTreeModel(string.Empty, null);
        [ObservableProperty]
        private string workState;
        public string RootPath { get; set; }

        [RelayCommand]
        private async Task OnActivated()
        {
            FolderTree.Children.Clear();
            DiskSpaceUtility.Analyse = true;
            WorkState = "Analysing, please wait";
            var t1 = Environment.TickCount;
            var d = new FolderTreeModel(RootPath, FolderTree);
            FolderTree.Children.Add(d);
            await d.Analyse();
            WorkState = $"Analyse finish, {(Environment.TickCount - t1) / 1000.0:0.##} s cost";
        }

        [RelayCommand]
        private void OnDeactivated()
        {
            DiskSpaceUtility.Analyse = false;
        }
    }
}
