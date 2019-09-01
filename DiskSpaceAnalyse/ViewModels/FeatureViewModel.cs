namespace DiskSpaceAnalyse.ViewModels
{
    public class FeatureViewModel
    {
        public FeatureViewModel(string title, string description, string rootpath)
        {
            RootPath = rootpath;
            Title = title;
            Description = description;
        }

        public string Title { get; }

        public string Description { get; }

        public string RootPath { get; }
    }
}