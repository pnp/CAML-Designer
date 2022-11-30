using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using CamlDesigner.SharePoint.Objects;
using CamlDesigner.Common.Objects;

namespace CamlDesigner.DataAccess.SharePoint.WebServices
{
    public class WebServiceHelper
    {
        bool useDefaultCredentials;
        string sharePointUrl;
        string username;
        string password;
        string domain;

        private System.Net.CookieCollection authCookies = null;

        private ListsWebServiceHelper listsWebServiceHelper = null;
        private WebsWebServiceHelper websWebServiceHelper = null;
        private ViewsWebServiceHelper viewsWebServiceHelper = null;
        private SiteDataWebServiceHelper siteDataWebServiceHelper = null;
        //private SecurityWebServiceHelper securityWebServiceHelper = null;
        private TaxonomyWebServiceHelper taxonomyWebServiceHelper = null;
        private UserGroupWebServiceHelper usergroupWebServiceHelper = null;

        #region Constructors
        public WebServiceHelper(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.sharePointUrl = url;
            this.useDefaultCredentials = true;

            // Initialize web services
            try
            {
                listsWebServiceHelper = new ListsWebServiceHelper(url);
                websWebServiceHelper = new WebsWebServiceHelper(url);
                viewsWebServiceHelper = new ViewsWebServiceHelper(url);
                siteDataWebServiceHelper = new SiteDataWebServiceHelper(url);
                //securityWebServiceHelper = new SecurityWebServiceHelper(url);
                taxonomyWebServiceHelper = new TaxonomyWebServiceHelper(url);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Request"))
                {
                    throw new ApplicationException("Authentication failed! Please, verify your credentials.");
                }
            }
        }

        public WebServiceHelper(string url, string username, string password, string domain)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.sharePointUrl = url;
            this.useDefaultCredentials = false;
            this.username = username;
            this.password = password;
            this.domain = domain;

            // Initialize web services
            try
            {
                listsWebServiceHelper = new ListsWebServiceHelper(url, username, password, domain);
                websWebServiceHelper = new WebsWebServiceHelper(url, username, password, domain);
                viewsWebServiceHelper = new ViewsWebServiceHelper(url, username, password, domain);
                siteDataWebServiceHelper = new SiteDataWebServiceHelper(url, username, password, domain);
                //securityWebServiceHelper = new SecurityWebServiceHelper(url, username, password, domain);
                taxonomyWebServiceHelper = new TaxonomyWebServiceHelper(url, username, password, domain);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("The request failed with HTTP status 401"))
                {
                    throw new ApplicationException("Authentication failed! Please, verify your credentials.");
                }
            }

        }
        #endregion

        #region Properties
        public string SharePointUrl
        {
            get { return sharePointUrl; }
            set
            {
                if (sharePointUrl != value)
                {
                    sharePointUrl = value;
                    RefreshWebServices(value);
                }
            }
        }

        public System.Net.CookieCollection AuthenticatedCookies
        {
            get { return authCookies; }
            set
            {
                ListsWebServiceHelper.AuthenticatedCookies = value;
                //WebsWebServiceHelper.AuthenticatedCookies = value;
                //SecurityWebServiceHelper.AuthenticatedCookies = value;
                //SiteDataWebServiceHelper.AuthenticatedCookies = value;
                //TaxonomyWebServiceHelper.AuthenticatedCookies = value;
                //ViewsWebServiceHelper.AuthenticatedCookies = value;
                //WebsWebServiceHelper.AuthenticatedCookies = value;
            }
        }

        public ListsWebServiceHelper ListsWebServiceHelper
        {
            get
            {
                if (listsWebServiceHelper == null)
                {
                    if (useDefaultCredentials)
                        listsWebServiceHelper = new ListsWebServiceHelper(sharePointUrl);
                    else
                        listsWebServiceHelper = new ListsWebServiceHelper(sharePointUrl, username, password, domain);
                }
                return listsWebServiceHelper;
            }
        }

        public WebsWebServiceHelper WebsWebServiceHelper
        {
            get
            {
                if (websWebServiceHelper == null)
                {
                    if (useDefaultCredentials)
                        websWebServiceHelper = new WebsWebServiceHelper(sharePointUrl);
                    else
                        websWebServiceHelper = new WebsWebServiceHelper(sharePointUrl, username, password, domain);
                }
                return websWebServiceHelper;
            }
        }

        public ViewsWebServiceHelper ViewsWebServiceHelper
        {
            get
            {
                if (viewsWebServiceHelper == null)
                {
                    if (useDefaultCredentials)
                        viewsWebServiceHelper = new ViewsWebServiceHelper(sharePointUrl);
                    else
                        viewsWebServiceHelper = new ViewsWebServiceHelper(sharePointUrl, username, password, domain);
                }
                return viewsWebServiceHelper;
            }
        }

        public TaxonomyWebServiceHelper TaxonomyWebServiceHelper
        {
            get
            {
                if (taxonomyWebServiceHelper == null)
                {
                    if (useDefaultCredentials)
                        taxonomyWebServiceHelper = new TaxonomyWebServiceHelper(sharePointUrl);
                    else
                        taxonomyWebServiceHelper = new TaxonomyWebServiceHelper(sharePointUrl, username, password, domain);
                }
                return taxonomyWebServiceHelper;
            }
        }

        public SiteDataWebServiceHelper SiteDataWebServiceHelper
        {
            get
            {
                if (siteDataWebServiceHelper == null)
                {
                    if (useDefaultCredentials)
                        siteDataWebServiceHelper = new SiteDataWebServiceHelper(sharePointUrl);
                    else
                        siteDataWebServiceHelper = new SiteDataWebServiceHelper(sharePointUrl, username, password, domain);
                }
                return siteDataWebServiceHelper;
            }
        }

        public UserGroupWebServiceHelper UserGroupWebServiceHelper
        {
            get
            {
                if (usergroupWebServiceHelper == null)
                {
                    if (useDefaultCredentials)
                        usergroupWebServiceHelper = new UserGroupWebServiceHelper(sharePointUrl);
                    else
                        usergroupWebServiceHelper = new UserGroupWebServiceHelper(sharePointUrl, username, password, domain);
                }
                return usergroupWebServiceHelper;
            }
        }

        /*
        public SecurityWebServiceHelper SecurityWebServiceHelper
        {
            get
            {
                if (securityWebServiceHelper == null)
                {
                    if (useDefaultCredentials)
                        securityWebServiceHelper = new DataAccess.SecurityWebServiceHelper(sharePointUrl);
                    else
                        securityWebServiceHelper = new DataAccess.SecurityWebServiceHelper(sharePointUrl, username, password, domain);
                }
                return securityWebServiceHelper;
            }
        }
         */

        #endregion

        public Web GetWeb(string webUrl)
        {
            return WebsWebServiceHelper.GetWeb(webUrl);
        }

        public List<Web> GetWebs(string webUrl)
        {
            return WebsWebServiceHelper.GetWebs(webUrl);
        }

        public List<Web> GetWebAndSubWebs(string webUrl)
        {
            return WebsWebServiceHelper.GetWebAndSubWebs(webUrl);
        }

        public List<Web> GetWebAndSubWebsWithDetails(string webUrl)
        {
            List<Web> webs = WebsWebServiceHelper.GetWebAndSubWebs(webUrl);
            if (webs != null)
            {
                for (int i = 0; i < webs.Count; i++)
                {
                    Web web = webs[i];
                    SiteDataWebServiceHelper.SharePointUrl = web.Url;
                    SiteDataWebServiceHelper.GetWebDetails(ref web);
                }
            }
            return webs;
        }

        public List<GroupValue> GetGroups()
        {
            return UserGroupWebServiceHelper.GetGroups();
        }

        public List<View> GetViews(string webUrl, string listName)
        {
            return ViewsWebServiceHelper.GetViews(webUrl, listName);
        }

        public List<List> GetLists(string webUrl)
        {
            return ListsWebServiceHelper.GetLists(webUrl);
        }

        public List<Folder> GetFolders(string webUrl, string listName)
        {
            List<Folder> folderCollection = ListsWebServiceHelper.GetFolders(webUrl, listName);

            if (folderCollection != null)
            {
                foreach (Folder folder in folderCollection)
                {
                    string result = null; // SecurityWebServiceHelper.GetFolderPermissions(folder.RelativeUrl);

                    if (!string.IsNullOrEmpty(result))
                    {
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(result);
                            XmlNode permissionNode = doc.SelectSingleNode("//UniquePermissions");
                            if (permissionNode != null && permissionNode.InnerText == "true")
                                folder.HasUniquePermissions = true;
                            else
                                folder.HasUniquePermissions = false;
                        }
                        catch { }
                    }
                }
            }
            return folderCollection;
        }

        public List<string> GetSiteColumnGroups()
        {
            return WebsWebServiceHelper.GetSiteColumnGroups(null);
        }

        public List<Field> GetFields(string listName, bool excludeHiddenFields)
        {
            return ListsWebServiceHelper.GetFields(null, listName, excludeHiddenFields);
        }

        public List<Field> GetFields(string webUrl, string listName, bool excludeHiddenFields)
        {
            return ListsWebServiceHelper.GetFields(webUrl, listName, excludeHiddenFields);
        }

        public List<ContentType> GetContentTypes(string listName)
        {
            return ListsWebServiceHelper.GetContentTypes(null, listName);
        }

        public List<ContentType> GetContentTypes(string webUrl, string listName)
        {
            return ListsWebServiceHelper.GetContentTypes(webUrl, listName);
        }

        public List<Field> GetFieldsForContentType(string contentTypeID)
        {
            return WebsWebServiceHelper.GetFieldsForContentType(null, contentTypeID);
        }

        public List<Field> GetFieldsForContentType(string webUrl, string contentTypeID)
        {
            return WebsWebServiceHelper.GetFieldsForContentType(webUrl, contentTypeID);
        }

        public List<LookupValue> GetLookupValues(string listGuid, string showField, Guid lookupWebId)
        {
            // get the URL corresponding to the Web Id
            string webUrl = WebsWebServiceHelper.GetWebUrlById(lookupWebId);
            if (webUrl == sharePointUrl)
                return ListsWebServiceHelper.GetLookupValues(null, listGuid, showField);
            else
                return ListsWebServiceHelper.GetLookupValues(webUrl, listGuid, showField);
        }

        public List<LookupValue> GetLookupValues(string webUrl, string listGuid, string showField)
        {
            return ListsWebServiceHelper.GetLookupValues(webUrl, listGuid, showField);
        }

        public List<TaxonomyValue> GetTaxonomyValues(Guid termStoreId, Guid termSetId)
        {
            return TaxonomyWebServiceHelper.GetTaxonomyValues(null, termStoreId, termSetId);
        }

        public List<TaxonomyValue> GetLookupValues(string webUrl, Guid termStoreId, Guid termSetId)
        {
            return TaxonomyWebServiceHelper.GetTaxonomyValues(webUrl, termStoreId, termSetId);
        }

        public DataTable ExecuteQuery(string listName, MainObject mainobject , CamlDesigner.Common.Enumerations.QueryType queryType)
        {
            return ListsWebServiceHelper.ExecuteQuery(listName, mainobject);
        }


        #region helper methods


        private void RefreshWebServices(string websiteUrl)
        {
            ListsWebServiceHelper.SharePointUrl = websiteUrl;
            WebsWebServiceHelper.SharePointUrl = websiteUrl;
            SiteDataWebServiceHelper.SharePointUrl = websiteUrl;
        }
        #endregion
    }
}
