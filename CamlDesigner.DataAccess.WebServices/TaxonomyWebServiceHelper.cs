using CamlDesigner.SharePoint.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CamlDesigner.DataAccess.SharePoint.WebServices
{
    public class TaxonomyWebServiceHelper
    {
        private string sharePointUrl = null;
        private string username = null;
        private string password = null;
        private string domain = null;
        private bool useDefaultCredentials = false;

        private TaxonomyWebService.Taxonomywebservice taxonomyWebService = null;
        private System.Net.CookieCollection authCookies = null;

        #region Constructors
        public TaxonomyWebServiceHelper(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.useDefaultCredentials = true;
            InitializeWebService(url);
        }

        public TaxonomyWebServiceHelper(string url, string username, string password, string domain)
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
                if (!string.IsNullOrEmpty(sharePointUrl) && sharePointUrl != value)
                {
                    RefreshWebService(value);
                }
            }
        }

        public TaxonomyWebService.Taxonomywebservice TaxonomyWebService
        {
            get
            {
                if (taxonomyWebService == null)
                {
                    InitializeWebService(null);
                }
                return taxonomyWebService;
            }
        }

        public System.Net.CookieCollection AuthenticatedCookies
        {
            get { return authCookies; }
            set
            {
                if (value != null)
                {
                    TaxonomyWebService.CookieContainer = new System.Net.CookieContainer();
                    TaxonomyWebService.CookieContainer.Add(value);
                }
                else
                    TaxonomyWebService.CookieContainer = null;
            }
        }
        #endregion

        #region Public Methods

        public List<TaxonomyValue> GetTaxonomyValues(string webUrl, Guid termStoreId, Guid termSetId)
        {
            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);
            
            List<TaxonomyValue> values = null;

            //Dim oldtimestamp As String = "<timeStamp>633992461437070000</timeStamp>"    
            //Dim clientVersion As String = "<version>3</version>"    
            //Dim termStoreIds As String = "<termStoreId>TERMSTOREID</termStoreId>"    
            //Dim termSetIds As String = "<termSetId>TERMSETID</termSetId>"    
            //Dim lcidSpanish As Integer = CultureInfo.GetCultureInfo("es-ES").LCID    
            //Dim timeStamp As String = Nothing

            string clientVersion = "<version>3</version>";
            string termStoreIdString = string.Format("<termStoreId>{0}</termStoreId>", termStoreId.ToString());
            string termSetIdString = string.Format("<termSetId>{0}</termSetId>", termSetId.ToString());
            string timestamp = null;

            string result = TaxonomyWebService.GetTermSets(termStoreIdString, termSetIdString, 1033, null, clientVersion, out timestamp);

            if (!string.IsNullOrEmpty(result))
            {
                values = new List<TaxonomyValue>();
            }

            return values;
        }
        #endregion

        #region Private Methods
        private void InitializeWebService(string url)
        {
            if (!string.IsNullOrEmpty(url))
                sharePointUrl = String.Format("{0}/_vti_bin/TaxonomyClientService.asmx", url);

            if (string.IsNullOrEmpty(sharePointUrl))
                throw new Exception("SharePoint URL cannot be null or empty.");

            taxonomyWebService = new TaxonomyWebService.Taxonomywebservice();
            taxonomyWebService.PreAuthenticate = true;
            taxonomyWebService.Url = sharePointUrl;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(domain))
                    taxonomyWebService.Credentials = new System.Net.NetworkCredential(username, password);
                else
                    taxonomyWebService.Credentials = new System.Net.NetworkCredential(username, password, domain);
            }
            else
            {
                taxonomyWebService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            // TODO: Test if the user has access
        }

        private void RefreshWebService(string url)
        {
            if (!string.IsNullOrEmpty(sharePointUrl) && sharePointUrl != url)
            {
                sharePointUrl = String.Format("{0}/_vti_bin/TaxonomyClientService.asmx", url);
                if (taxonomyWebService.Url != sharePointUrl)
                    taxonomyWebService.Url = sharePointUrl;
            }
        }

        #endregion
    }
}
