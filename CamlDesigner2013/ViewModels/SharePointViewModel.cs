using CamlDesigner2013.Controls;
using CamlDesigner2013.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CamlDesigner2013.ViewModels
{
    public class SharePointViewModel : TreeViewItemViewModel
    {
        private readonly ReadOnlyCollection<ListViewModel> listViewModels;
        private string viewModelType = null;

        public SharePointViewModel(string viewModelType)
            : base(null, true)
        {
            if (this.CamlWrapper != null)
            {
                CamlDesigner.SharePoint.Objects.Web web = null;
                // store the site name in the recent connection
                try
                {
                    web = this.CamlWrapper.GetWeb(App.GeneralInformation.SharePointUrl);
                }
                catch (Exception ex)
                {

                    Logger.WriteToLogFile("An error has occured in SharePointViewModel Class constructor", ex);
                    throw ex;
                }

                if (web != null)
                {
                    App.CurrentConnection.SiteName = web.Title;
                    Helpers.IO.UpdateRecent(App.CurrentConnection);

                    // retrieve the lists to populate the treeview
                    this.viewModelType = viewModelType;
                    switch (viewModelType)
                    {
                        case "list":
                            List<CamlDesigner.SharePoint.Objects.List> listCollection = this.CamlWrapper.GetLists();
                            List<ListViewModel> children = new List<ListViewModel>();
                            foreach (CamlDesigner.SharePoint.Objects.List list in listCollection)
                            {
                                ListViewModel listViewModel = new ListViewModel(list);
                                listViewModel.TreeViewItemViewModelSelected += new TreeViewItemViewModelDelegate(this.ListViewModel_TreeViewItemViewModelSelected);
                                this.Children.Add(listViewModel);
                                children.Add(listViewModel);
                            }
                            this.listViewModels = new ReadOnlyCollection<ListViewModel>(children);
                            break;

                    }
                }
            }
        }

        public string SiteUrl
        {
            get { return App.GeneralInformation.SharePointUrl; }
        }

        public ReadOnlyCollection<ListViewModel> ListViewModels
        {
            get { return this.listViewModels; }
        }

        private CAMLDesigner.BusinessObjects.Wrapper CamlWrapper
        {
            get
            {
                if ((App.CamlWrapper == null && App.GeneralInformation != null)
                    || (App.CamlWrapper != null && App.GeneralInformation != null)
                    && (App.CamlWrapper.Url != App.GeneralInformation.SharePointUrl)
                    && (App.CamlWrapper.ConnectionType != App.GeneralInformation.ConnectionType))
                {
                    if (App.GeneralInformation.ConnectionType ==  CamlDesigner.Common.Enumerations.ApiConnectionType.WebServices
                        || App.GeneralInformation.ConnectionType ==  CamlDesigner.Common.Enumerations.ApiConnectionType.ClientObjectModel)
                    {
                        App.CamlWrapper = new CAMLDesigner.BusinessObjects.Wrapper(App.GeneralInformation.SharePointUrl,
                            App.GeneralInformation.ConnectionType,
                            App.GeneralInformation.Username,
                            App.GeneralInformation.Password,
                            App.GeneralInformation.Domain);
                    }
                    else
                    {
                        App.CamlWrapper = new CAMLDesigner.BusinessObjects.Wrapper(App.GeneralInformation.SharePointUrl,
                            App.GeneralInformation.ConnectionType);
                    }
                }
                return App.CamlWrapper;
            }
        }

        protected override void LoadChildren()
        {
            switch (this.viewModelType)
            {
                case "list":
                    List<CamlDesigner.SharePoint.Objects.List> listCollection = this.CamlWrapper.GetLists();
                    foreach (CamlDesigner.SharePoint.Objects.List list in listCollection)
                        this.Children.Add(new ListViewModel(list));

                    break;
            }
        }

        public void ListViewModel_TreeViewItemViewModelSelected(TreeViewItemViewModel model, EventArgs e)
        {
            // TODO: all selected viewmodels need to be set to null
            if (model != null)
            {
                App.SelectedListViewModel = model as ListViewModel;

                // the Source fields list must be cleared and populated with the fields of the newly selected list.
            }
        }
    }
}
