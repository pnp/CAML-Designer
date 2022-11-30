using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using CamlDesigner.SharePoint.Objects;

namespace CamlDesigner.DataAccess.SharePoint.WebServices
{
    public class SiteDataWebServiceHelper
    {
        private string sharePointUrl = null;
        private  string username = null;
        private string password = null;
        private string domain = null;
        private bool useDefaultCredentials = false;

        private SiteDataWebService.SiteData siteDataWebService = null;
        private System.Net.CookieCollection authCookies = null;

        #region Constructors
        public SiteDataWebServiceHelper(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.useDefaultCredentials = true;
            InitializeWebService(url);
        }

        public SiteDataWebServiceHelper(string url, string username, string password, string domain)
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

        public SiteDataWebService.SiteData SiteDataWebService
        {
            get
            {
                if (siteDataWebService == null)
                {
                    InitializeWebService(null);
                }
                return siteDataWebService;
            }
        }

        public System.Net.CookieCollection AuthenticatedCookies
        {
            get { return authCookies; }
            set
            {
                if (value != null)
                {
                    SiteDataWebService.CookieContainer = new System.Net.CookieContainer();
                    SiteDataWebService.CookieContainer.Add(value);
                }
                else
                    SiteDataWebService.CookieContainer = null;
            }
        }
        #endregion

        #region Public Methods
        public void GetWebDetails (ref Web web)
        {
            /*
            SiteDataWebService._sWebMetadata webMetadata;
            SiteDataWebService._sWebWithTime[] webs;
            SiteDataWebService._sListWithTime[] lists;
            SiteDataWebService._sFPUrl[] vFPUrls;
            string strRoles;
            string[] rolesUsers;
            string[] rolesGroups;

            SiteDataWebService.GetWeb(out webMetadata, out webs, out lists, out vFPUrls,
                out strRoles, out rolesUsers, out rolesGroups);

            web.HasUniquePermissions = !webMetadata.InheritedSecurity;
            */
        }

        #endregion

        #region Private Methods
        private void InitializeWebService(string url)
        {
            if (!string.IsNullOrEmpty(url))
                sharePointUrl = String.Format("{0}/_vti_bin/SiteData.asmx", url);

            if (string.IsNullOrEmpty(sharePointUrl))
                throw new Exception("SharePoint URL cannot be null or empty.");

            siteDataWebService = new SiteDataWebService.SiteData();
            siteDataWebService.Url = sharePointUrl;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(domain))
                    siteDataWebService.Credentials = new System.Net.NetworkCredential(username, password);
                else
                    siteDataWebService.Credentials = new System.Net.NetworkCredential(username, password, domain);
            }
            else
            {
                siteDataWebService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            // TODO: Test a method to see if the user has access
        }

        private void RefreshWebService(string url)
        {
            if (!string.IsNullOrEmpty(sharePointUrl) && sharePointUrl != url)
            {
                sharePointUrl = String.Format("{0}/_vti_bin/SiteData.asmx", url);
                if (siteDataWebService.Url != sharePointUrl)
                    siteDataWebService.Url = sharePointUrl;
            }
        }
        #endregion
    }
}
