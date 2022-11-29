namespace CamlDesigner2013.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class RootViewModel : TreeViewItemViewModel
    {
        private string siteUrl = null;
        private ReadOnlyCollection<SharePointViewModel> sharePointViewModels;

        // private SharePointViewModel sharePointViewModel = null;

        public RootViewModel(string viewModelType)
            : base(null, true)
        {
            List<SharePointViewModel> children = new List<SharePointViewModel>();
            children.Add(new SharePointViewModel(viewModelType));
            this.sharePointViewModels = new ReadOnlyCollection<SharePointViewModel>(children);
        }

        public ReadOnlyCollection<SharePointViewModel> SharePointViewModels
        {
            get { return this.sharePointViewModels; }
        }
    }
}
