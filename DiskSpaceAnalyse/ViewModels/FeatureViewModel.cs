using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace DiskSpaceAnalyse.ViewModels
{
    [ObservableObject]
    public partial class FeatureViewModel 
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string rootPath;

        public event Action<string> Click;

        [RelayCommand]
        private void ShowFeature()
        {
            Click?.Invoke(RootPath);
        }
    }
}
