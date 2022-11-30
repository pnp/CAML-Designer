using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CamlDesigner.SharePoint.Objects;
using System.Security;
using System.Net;
//using O365 = Microsoft.SharePoint.Client;

namespace CamlDesigner.DataAccess.SharePoint.WebServices
{
    public class WebsWebServiceHelper
    {
        private string sharePointUrl = null;
        private  string username = null;
        private string password = null;
        private string domain = null;
        private bool useDefaultCredentials = false;

        private WebsWebService.Webs websWebService = null;
        private System.Net.CookieCollection authCookies = null;

        #region Constructors
        public WebsWebServiceHelper(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.useDefaultCredentials = true;
            InitializeWebService(url);
        }

        public WebsWebServiceHelper(string url, string username, string password, string domain)
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

        public WebsWebService.Webs WebsWebService
        {
            get
            {
                if (websWebService == null)
                {
                    InitializeWebService(null);
                }
                return websWebService;
            }
        }

        public System.Net.CookieCollection AuthenticatedCookies
        {
            get { return authCookies; }
            set
            {
                if (value != null)
                {
                    WebsWebService.CookieContainer = new System.Net.CookieContainer();
                    WebsWebService.CookieContainer.Add(value);
                }
                else
                    WebsWebService.CookieContainer = null;
            }
        }
        #endregion

        #region Public Methods
        public List<Web> GetWebs(string webUrl)
        {
            List<Web> webs = null;

            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            // get all websites in this site collection
            XmlNode resultNode = WebsWebService.GetWebCollection();
            if (resultNode != null && resultNode.ChildNodes.Count > 0)
                webs = new List<Web>();

            foreach (XmlNode webNode in resultNode)
            {
                Web web = new Web();

                if (webNode.Attributes["Title"] != null)
                    web.Title = webNode.Attributes["Title"].Value;
                if (webNode.Attributes["Url"] != null)
                    web.Url = webNode.Attributes["Url"].Value;
                if (webNode.Attributes["ID"] != null)
                    web.ID = new Guid(webNode.Attributes["ID"].Value);

                webs.Add(web);
            }
            return webs;
        }

        public List<Web> GetWebAndSubWebs(string webUrl)
        {
            List<Web> webs = null;

            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            // get all websites in this site collection
            XmlNode resultNode = websWebService.GetAllSubWebCollection();
            //XmlNode resultNode = WebsWebService.GetWebCollection();
            if (resultNode != null && resultNode.ChildNodes.Count > 0)
                webs = new List<Web>();

            foreach (XmlNode webNode in resultNode)
            {
                Web web = new Web();

                if (webNode.Attributes["Title"] != null)
                    web.Title = webNode.Attributes["Title"].Value;
                if (webNode.Attributes["Url"] != null)
                    web.Url = webNode.Attributes["Url"].Value;

                RefreshWebService(web.Url);
                XmlNode detailsNode = websWebService.GetWeb(web.Url);
                if (detailsNode.Attributes["Id"] != null)
                    web.ID = new Guid(detailsNode.Attributes["Id"].Value);
                if (detailsNode.Attributes["Description"] != null)
                    web.Description = detailsNode.Attributes["Description"].Value;

                webs.Add(web);
            }

            this.SharePointUrl = webUrl;
            return webs;
        }

        public Web GetWeb(string webUrl)
        {
            Web web = null;

            if (!string.IsNullOrEmpty(webUrl))
            {
                if (webUrl.EndsWith("/"))
                {
                    webUrl = webUrl.Substring(0, webUrl.Length - 1);
                }
                RefreshWebService(webUrl);
            }

            XmlNode webNode = WebsWebService.GetWeb(webUrl);
            if (webNode != null)
            {
                web = new Web();
                if (webNode.Attributes["Title"] != null)
                    web.Title = webNode.Attributes["Title"].Value;
                if (webNode.Attributes["Id"] != null)
                    web.ID = new Guid(webNode.Attributes["Id"].Value);
                if (webNode.Attributes["Url"] != null)
                    web.Url = webNode.Attributes["Url"].Value;
            }
            return web;
        }

        public string GetWebUrlById(Guid webId)
        {
            if (webId == Guid.Empty) return null;

            string webUrl = null;

            // strip the url
            string url = null;
            if (sharePointUrl.IndexOf("_vti_bin") >= 0)
                url = sharePointUrl.Substring(0, sharePointUrl.IndexOf("/_vti_bin"));
            else
                url = sharePointUrl;

            // Get current web 
            Web web = GetWeb(url);
            if (web.ID == webId)
                webUrl = web.Url;
            else
            {
                string oldUrl = sharePointUrl;

                // strip the URL to the site collection URL
                if (url.LastIndexOf("/") > 7)
                    url = url.Substring(0, url.LastIndexOf("/"));

                web = GetWeb(url);
                if (web.ID == webId)
                    webUrl = web.Url;

                // reset the web service to the previous url
                RefreshWebService(oldUrl);
            }

            return webUrl;
        }

        public List<string> GetSiteColumnGroups(string webUrl)
        {
            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            SortedList<string, string> existingGroups = null;

            XmlNode columnsNode = WebsWebService.GetColumns();

            foreach (XmlNode columnNode in columnsNode.SelectNodes("*[local-name()='Field']"))
            {
                if (existingGroups == null)
                    existingGroups = new SortedList<string, string>();

                if (columnNode.Attributes["Group"] != null && !string.IsNullOrEmpty(columnNode.Attributes["Group"].Value))
                {
                    if (!existingGroups.ContainsKey(columnNode.Attributes["Group"].Value) && columnNode.Attributes["Group"].Value != "_Hidden")
                        existingGroups.Add(columnNode.Attributes["Group"].Value, columnNode.Attributes["Group"].Value);
                }
            }
            return existingGroups.Keys.ToList<string>();
        }

        public List<string> GetContentTypeGroups(string webUrl)
        {
            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            List<string> existingGroups = null;

            return existingGroups;
        }

        public List<Field> GetFieldsForContentType(string webUrl, string contentTypeID)
        {
            List<Field> fieldsCollection = null;

            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            XmlNode ctNode = websWebService.GetContentType(contentTypeID);

            if (ctNode != null && ctNode.ChildNodes.Count > 0)
                fieldsCollection = new List<Field>();

            foreach (XmlNode fieldNode in ctNode.SelectNodes("*[local-name()='Fields']/*[local-name()='Field']"))
            {
                Field field = new Field();
                // store the field name, field type and field ID 
                if (fieldNode.Attributes["ID"] != null)
                {
                    field.ID = new Guid(fieldNode.Attributes["ID"].Value);
                }

                if (fieldNode.Attributes["DisplayName"] != null)
                {
                    field.DisplayName = fieldNode.Attributes["DisplayName"].Value;
                }

                if (fieldNode.Attributes["Name"] != null)
                {
                    field.InternalName = fieldNode.Attributes["Name"].Value;
                }

                if (fieldNode.Attributes["Type"] != null)
                {
                    field.DataType = fieldNode.Attributes["Type"].Value;
                }
                fieldsCollection.Add(field);
            }

            return fieldsCollection;
        }
        #endregion

        #region Private Methods
        private void InitializeWebService(string url)
        {
            if (!string.IsNullOrEmpty(url))
                sharePointUrl = String.Format("{0}/_vti_bin/Webs.asmx", url);

            if (string.IsNullOrEmpty(sharePointUrl))
                throw new Exception("SharePoint URL cannot be null or empty.");

            bool isSharePointOnline = url.Contains("microsoft.com") || url.Contains("sharepoint.com");

            websWebService = new WebsWebService.Webs();
            websWebService.Url = sharePointUrl;

            if (isSharePointOnline)
            {
                //SecureString securePassword = new SecureString();
                //char[] pwd = password.ToCharArray();

                //foreach (char c in pwd)
                //{
                //    securePassword.AppendChar(c);
                //}
                //O365.SharePointOnlineCredentials onlineCredentials = new O365.SharePointOnlineCredentials(username, securePassword);
                //websWebService.UseDefaultCredentials = false;

                ////websWebService.Credentials = new O365.SharePointOnlineCredentials(username, securePassword);

                //Uri uri = new Uri(url);
                //string authCookieValue = onlineCredentials.GetAuthenticationCookie(uri);
                //websWebService.CookieContainer = new CookieContainer();
                //websWebService.CookieContainer.Add(new Cookie("FedAuth",
                //authCookieValue.TrimStart("SPOIDCRL=".ToCharArray()),// Remove the prefix from the cookie's value
                //String.Empty,
                //uri.Authority));
            }
            else
            {
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    if (string.IsNullOrEmpty(domain))
                        websWebService.Credentials = new System.Net.NetworkCredential(username, password);
                    else
                        websWebService.Credentials = new System.Net.NetworkCredential(username, password, domain);
                }
                else
                {
                    websWebService.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }
            }

            // Test a method to see if the user has access
            //string test = websWebService.Url;
        }

        private void RefreshWebService(string url)
        {
            if (!string.IsNullOrEmpty(sharePointUrl) && sharePointUrl != url)
            {
                if (url.Contains("_vti_bin/webs.asmx"))
                    sharePointUrl = url;
                else
                    sharePointUrl = String.Format("{0}/_vti_bin/Webs.asmx", url);
    
                if (websWebService.Url != sharePointUrl)
                    websWebService.Url = sharePointUrl;
            }
        }
        #endregion
    }
}
