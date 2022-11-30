using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CamlDesigner.SharePoint.Objects;

namespace CamlDesigner.DataAccess.SharePoint.WebServices
{
    public class ViewsWebServiceHelper
    {
        private string sharePointUrl = null;
        private  string username = null;
        private string password = null;
        private string domain = null;
        private bool useDefaultCredentials = false;

        private ViewsWebService.Views viewsWebService = null;
        private System.Net.CookieCollection authCookies = null;

        #region Constructors
        public ViewsWebServiceHelper(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.useDefaultCredentials = true;
            InitializeWebService(url);
        }

        public ViewsWebServiceHelper(string url, string username, string password, string domain)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.useDefaultCredentials = false;
            this.username = username;
            this.password = password;
            this.domain = domain;

            // Initialize web services
            InitializeWebService(url);
        }
        #endregion

        #region Public Properties

        public string SharePointUrl
        {
            get { return sharePointUrl; }
            set 
            {
                RefreshWebService(value);
            }
        }

        public ViewsWebService.Views ViewsWebService
        {
            get
            {
                if (viewsWebService == null)
                {
                    InitializeWebService(null);
                }
                return viewsWebService;
            }
        }

        public System.Net.CookieCollection AuthenticatedCookies
        {
            get { return authCookies; }
            set
            {
                if (value != null)
                {
                    ViewsWebService.CookieContainer = new System.Net.CookieContainer();
                    ViewsWebService.CookieContainer.Add(value);
                }
                else
                    ViewsWebService.CookieContainer = null;
            }
        }
        #endregion

        #region Public Methods

        public List<View> GetViews(string webUrl, string listName)
        {
            List<View> views = null;
            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            XmlNode viewsNode = viewsWebService.GetViewCollection(listName);

            if (viewsNode != null && viewsNode.ChildNodes.Count > 0)
                views = new List<View>();

            foreach (XmlNode viewNode in viewsNode)
            {
                View view = new View();
                if (viewNode.Attributes["Hidden"] == null || viewNode.Attributes["Hidden"].Value == "FALSE")
                {
                    view.ID = new Guid(viewNode.Attributes["Name"].Value); // this is the GUID of the view
                    view.DisplayName = viewNode.Attributes["DisplayName"].Value;
                    view.Type = viewNode.Attributes["Type"].Value;
                    view.IsDefaultView = false;
                    // check if it is a public view or a personal view
                    if (viewNode.Attributes["DefaultView"] != null && viewNode.Attributes["DefaultView"].Value == "TRUE")
                    {
                        view.IsDefaultView = true;
                    }

                    // create a new view
                    view.IsPersonalView = false;
                    if (viewNode.Attributes["Personal"] != null && viewNode.Attributes["Personal"].Value == "TRUE")
                    {
                        view.IsPersonalView = true;
                    }

                    // get the details of the view
                    XmlNode detailsNode = viewsWebService.GetView(listName, view.ID.ToString());
                    if (detailsNode != null)
                    {
                        view.ViewXML = detailsNode.OuterXml;

                        foreach (XmlNode childNode in detailsNode.ChildNodes)
                        {
                            switch (childNode.Name)
                            {
                                case "Query":
                                    view.Query = childNode.OuterXml;
                                    break;
                                case "ViewFields":
                                    view.ViewFields = childNode.OuterXml;
                                    break;
                                case "Aggregations":
                                    view.Aggregations = childNode.OuterXml;
                                    break;
                                case "RowLimit":
                                    int rowLimit = 30;
                                    int.TryParse(childNode.InnerText, out rowLimit);
                                    view.RowLimit = rowLimit;
                                    if (childNode.Attributes["Paged"] != null && !string.IsNullOrEmpty(childNode.Attributes["Paged"].Value))
                                    {
                                        bool paged = false;
                                        bool.TryParse(childNode.Attributes["Paged"].Value, out paged);
                                        view.Paged = paged;
                                    }
                                    break;
                            }
                        }
                    }
                }
                views.Add(view);
            }

            return views;
        }
        #endregion

        #region Private Methods
        private void InitializeWebService(string url)
        {
            if (!string.IsNullOrEmpty(url))
                sharePointUrl = String.Format("{0}/_vti_bin/Views.asmx", url);

            if (string.IsNullOrEmpty(sharePointUrl))
                throw new Exception("SharePoint URL cannot be null or empty.");

            viewsWebService = new ViewsWebService.Views();
            viewsWebService.Url = sharePointUrl;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(domain))
                    viewsWebService.Credentials = new System.Net.NetworkCredential(username, password);
                else
                    viewsWebService.Credentials = new System.Net.NetworkCredential(username, password, domain);
            }
            else
            {
                viewsWebService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            // TODO: Test a method to see if the user has access
        }

        private void RefreshWebService(string url)
        {
            if (!string.IsNullOrEmpty(sharePointUrl) && sharePointUrl != url)
            {
                sharePointUrl = String.Format("{0}/_vti_bin/Views.asmx", url);
                if (viewsWebService.Url != sharePointUrl)
                    viewsWebService.Url = sharePointUrl;
            }
        }
        #endregion
    }
}
