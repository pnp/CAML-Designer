using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CustomObjects = CamlDesigner.SharePoint.Objects;

namespace CamlDesigner.DataAccess.SharePoint.WebServices
{
    public class UserGroupWebServiceHelper
    {
        private string sharePointUrl = null;
        private  string username = null;
        private string password = null;
        private string domain = null;
        private bool useDefaultCredentials = false;

        private UserGroupWebService.UserGroup userGroupWebService = null;
        private System.Net.CookieCollection authCookies = null;

        #region Constructors
        public UserGroupWebServiceHelper(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.useDefaultCredentials = true;
            InitializeWebService(url);
        }

        public UserGroupWebServiceHelper(string url, string username, string password, string domain)
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

        public UserGroupWebService.UserGroup UserGroupWebService
        {
            get
            {
                if (userGroupWebService == null)
                {
                    InitializeWebService(null);
                }
                return userGroupWebService;
            }
        }

        public System.Net.CookieCollection AuthenticatedCookies
        {
            get { return authCookies; }
            set
            {
                if (value != null)
                {
                    UserGroupWebService.CookieContainer = new System.Net.CookieContainer();
                    UserGroupWebService.CookieContainer.Add(value);
                }
                else
                    UserGroupWebService.CookieContainer = null;
            }
        }
        #endregion

        #region Public Methods

        public List<CustomObjects.GroupValue> GetGroups()
        {
            List<CustomObjects.GroupValue> groups = null;
            //if (!string.IsNullOrEmpty(webUrl))
            //    RefreshWebService(webUrl);

            XmlNode groupsNode = userGroupWebService.GetGroupCollectionFromWeb();

            if (groupsNode != null && groupsNode.ChildNodes.Count > 0)
                groups = new List<CustomObjects.GroupValue>();

            /*
            <Groups xmlns=\"http://schemas.microsoft.com/sharepoint/soap/directory/\">
                <Group ID=\"7\" Name=\"Demo Members\" Description=\"Use this group to grant people contribute permissions to the SharePoint site: Demo\" OwnerID=\"5\" OwnerIsUser=\"False\" />
                <Group ID=\"5\" Name=\"Demo Owners\" Description=\"Use this group to grant people full control permissions to the SharePoint site: Demo\" OwnerID=\"5\" OwnerIsUser=\"False\" />
                <Group ID=\"6\" Name=\"Demo Visitors\" Description=\"Use this group to grant people read permissions to the SharePoint site: Demo\" OwnerID=\"5\" OwnerIsUser=\"False\" />   
                <Group ID=\"3\" Name=\"Excel Services Viewers\" Description=\"Members of this group can view pages, list items, and documents.  If the document has a server rendering available, they can only view the document using the server rendering.\" OwnerID=\"1073741823\" OwnerIsUser=\"True\" />
                <Group ID=\"13\" Name=\"Ex U2U\" Description=\"\" OwnerID=\"1073741823\" OwnerIsUser=\"True\" />
            </Groups>"
             */
            foreach (XmlNode groupNode in groupsNode.ChildNodes[0].ChildNodes)
            {
                int id = 0;
                string name = string.Empty;

                if (groupNode.Attributes["ID"] != null)
                    int.TryParse(groupNode.Attributes["ID"].Value, out id);

                if (groupNode.Attributes["Name"] != null)
                    name = groupNode.Attributes["Name"].Value;

                CustomObjects.GroupValue groupvalue = new CustomObjects.GroupValue(id, name);

                groups.Add(groupvalue);
            }

            return groups;
        }
        #endregion

        #region Private Methods
        private void InitializeWebService(string url)
        {
            if (!string.IsNullOrEmpty(url))
                sharePointUrl = String.Format("{0}/_vti_bin/usergroup.asmx", url);

            if (string.IsNullOrEmpty(sharePointUrl))
                throw new Exception("SharePoint URL cannot be null or empty.");

            userGroupWebService = new UserGroupWebService.UserGroup();
            userGroupWebService.Url = sharePointUrl;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(domain))
                    userGroupWebService.Credentials = new System.Net.NetworkCredential(username, password);
                else
                    userGroupWebService.Credentials = new System.Net.NetworkCredential(username, password, domain);
            }
            else
            {
                userGroupWebService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            // TODO: Test a method to see if the user has access
        }

        private void RefreshWebService(string url)
        {
            if (!string.IsNullOrEmpty(sharePointUrl) && sharePointUrl != url)
            {
                sharePointUrl = String.Format("{0}/_vti_bin/usergroup.asmx", url);
                if (userGroupWebService.Url != sharePointUrl)
                    userGroupWebService.Url = sharePointUrl;
            }
        }
        #endregion
    }
}
