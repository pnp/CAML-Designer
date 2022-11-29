using CamlDesigner.Common;
using CamlDesigner.Common.Objects;
using CamlDesigner.DataAccess;
using CamlDesigner.DataAccess.SPO;
using CamlDesigner.SharePoint.Common;
using CamlDesigner.SharePoint.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace CAMLDesigner.BusinessObjects
{
    public class Wrapper
    {
        private CamlDesigner.DataAccess.SPO.ClientOMHelper csomHelper = null;
        //private BPoint.SharePoint.DataAccess.SP2013.OMHelper sp2013omHelper = null;
        //private CamlDesigner.SharePoint.DataAccess.SP2013.ClientOMHelper sp2013ClientOmHelper = null;
        //private CamlDesigner.SharePoint.DataAccess.WebServiceHelper webServiceHelper = null;

        private Dictionary<string, List<TaxonomyValue>> fetchedTaxonomyValues = new Dictionary<string, List<TaxonomyValue>>();

        //private SharePointVersion sharePointVersion;
        private Enumerations.ApiConnectionType connectionType;

        private string sharePointUrl;
        private string username;
        private string password;
        private string domain;

        #region Constructors
        public Wrapper(string url, Enumerations.ApiConnectionType connectionType)
        {
            this.sharePointUrl = url;
            // this.sharePointVersion = sharePointVersion;
            this.connectionType = connectionType;
        }

        public Wrapper(string url, Enumerations.ApiConnectionType connectionType,
            string username, string password, string domain)
            : this(url, connectionType)
        {
            this.username = username;
            this.password = password;
            this.domain = domain;
        }
        #endregion

        #region Properties
        public string Url
        {
            get { return sharePointUrl; }
            set { sharePointUrl = value; }
        }

        public Enumerations.ApiConnectionType ConnectionType
        {
            get { return connectionType; }
            set { connectionType = value; }
        }

        //private OMHelper SP2013OMHelper
        //{
        //    get
        //    {
        //        if (sp2013omHelper == null && !string.IsNullOrEmpty(sharePointUrl))
        //        {
        //            sp2013omHelper = new OMHelper(sharePointUrl);
        //        }
        //        return sp2013omHelper;
        //    }
        //}

        private ClientOMHelper CSOMHelper
        {
            get
            {
                if (csomHelper == null && !string.IsNullOrEmpty(sharePointUrl))
                {

                    //csomHelper = new ClientOMHelper(sharePointUrl, true);
                    csomHelper = new ClientOMHelper(sharePointUrl, null, true, username, password);
                }

                return csomHelper;
            }
        }

        //private WebServiceHelper WebServiceHelper
        //{
        //    get
        //    {
        //        if (webServiceHelper == null && !string.IsNullOrEmpty(sharePointUrl))
        //        {
        //            System.Net.CookieCollection authCookies = null;
        //            // check if the URL is a SharePoint online url
        //            if (sharePointUrl.Contains("microsoft.com") || sharePointUrl.Contains("sharepoint.com"))
        //            {
        //                ClientOMHelper clientOM = new ClientOMHelper(sharePointUrl, null, false, username, password);
        //                authCookies = clientOM.GetAuthenticatedCookies();
        //            }

        //            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password) && string.IsNullOrEmpty(domain))
        //                webServiceHelper = new WebServiceHelper(sharePointUrl);
        //            else
        //                webServiceHelper = new WebServiceHelper(sharePointUrl, username, password, domain);

        //            if (authCookies != null)
        //                webServiceHelper.AuthenticatedCookies = authCookies;
        //        }
        //        return webServiceHelper;
        //    }

        //}
        #endregion

        #region Public static Methods
        public static Enumerations.SharePointVersions GetSharePointVersion(string siteUrl)
        {
            return ClientOMHelper.GetSharePointVersion(siteUrl);
        }

        #endregion

        #region Public Methods
        public Web GetWeb(string webUrl)
        {
            Web web = null;

            switch (connectionType)
            {
                //case Enumerations.ApiConnectionType.ServerObjectModel:
                //    web = SP2013OMHelper.GetWeb(webUrl);
                //    break;
                case Enumerations.ApiConnectionType.ClientObjectModel:
                    web = CSOMHelper.GetWeb(webUrl);
                    break;
                //case Enumerations.ApiConnectionType.WebServices:
                //    // check if the url connects to SharePoint online
                //    web = WebServiceHelper.GetWeb(webUrl);
                //    break;
            }

            return web;
        }

        public List<Web> GetWebs(string webUrl)
        {
            this.sharePointUrl = webUrl;
            List<Web> webs = null;

            switch (connectionType)
            {
                //case Enumerations.ApiConnectionType.ServerObjectModel:
                //    webs = SP2013OMHelper.GetWebs(webUrl);
                //    break;
                case Enumerations.ApiConnectionType.ClientObjectModel:
                    webs = csomHelper.GetWebs(webUrl);
                    break;
                //case Enumerations.ApiConnectionType.WebServices:
                //    webs = WebServiceHelper.GetWebs(webUrl);
                //    break;
            }

            return webs;
        }

        public List<List> GetLists()
        {
            List<List> lists = null;
            switch (connectionType)
            {
                //case Enumerations.ApiConnectionType.ServerObjectModel:
                //    lists = SP2013OMHelper.GetLists(this.sharePointUrl);
                //    break;
                case Enumerations.ApiConnectionType.ClientObjectModel:
                    lists = csomHelper.GetLists(this.sharePointUrl);
                    break;
                //case Enumerations.ApiConnectionType.WebServices:
                //    lists = WebServiceHelper.GetLists(this.sharePointUrl);
                //    break;
            }
            return lists;
        }

        public List<Field> GetFields(string listName, bool excludeHiddenFields, bool isCalendar)
        {
            List<Field> fields = null;
            switch (connectionType)
            {
                //case Enumerations.ApiConnectionType.ServerObjectModel:
                //    fields = SP2013OMHelper.GetFields(listName, excludeHiddenFields);
                //    break;
                case Enumerations.ApiConnectionType.ClientObjectModel:
                    fields = csomHelper.GetFields(listName, excludeHiddenFields);
                    break;
                //case Enumerations.ApiConnectionType.WebServices:
                //    fields = WebServiceHelper.GetFields(listName, excludeHiddenFields);
                //    break;
            }
            return fields;
        }

        public List<GroupValue> GetGroups()
        {
            List<GroupValue> groups = null;
            switch (connectionType)
            {
                //case Enumerations.ApiConnectionType.ServerObjectModel:
                //    groups = SP2013OMHelper.GetGroups();
                //    break;
                case Enumerations.ApiConnectionType.ClientObjectModel:
                    groups = csomHelper.GetGroups();
                    break;
                //case Enumerations.ApiConnectionType.WebServices:
                //    groups = WebServiceHelper.GetGroups();
                //    break;
            }
            return groups;
        }

        public List<LookupValue> GetLookupValues(string listGuid, string showField, Guid lookupWebId)
        {
            List<LookupValue> values = null;
            switch (connectionType)
            {
                //case Enumerations.ApiConnectionType.ServerObjectModel:
                //    values = SP2013OMHelper.GetLookupValues(listGuid, showField, lookupWebId);
                //    break;
                case Enumerations.ApiConnectionType.ClientObjectModel:
                    values = csomHelper.GetLookupValues(listGuid, showField, lookupWebId);
                    break;
                //case Enumerations.ApiConnectionType.WebServices:
                //    values = WebServiceHelper.GetLookupValues(listGuid, showField, lookupWebId);
                //    break;
            }

            return values;
        }

        public TaxonomyValue GetTaxonomyValue(Guid termStoreId, Guid termSetId, string input)
        {
            TaxonomyValue value = null;

            switch (connectionType)
            {
                //case Enumerations.ApiConnectionType.ServerObjectModel:
                //    value = SP2013OMHelper.GetTaxonomyValue(termStoreId, termSetId, input);
                //    break;

                case Enumerations.ApiConnectionType.ClientObjectModel:
                    value = csomHelper.GetTaxonomyValue(termStoreId, termSetId, input);
                    break;

                //case Enumerations.ApiConnectionType.WebServices:
                //    //value = WebServiceHelper.GetTaxonomyValue(termStoreId, termSetId, input);
                //    break;
            }

            return value;
        }

        public List<TaxonomyValue> GetTaxonomyValues(Guid termStoreId, Guid termSetId)
        {
            List<TaxonomyValue> values = null;

            string cacheKey = string.Format("{0}|{1}", termStoreId.ToString().ToLower(), termSetId.ToString().ToLower());

            if (fetchedTaxonomyValues.ContainsKey(cacheKey))
            {
                return fetchedTaxonomyValues[cacheKey];
            }

            // TODO: Add temporary caching for taxonomy, so that taxonomy is fetched only once.

            switch (connectionType)
            {
                //case Enumerations.ApiConnectionType.ServerObjectModel:
                //    values = SP2013OMHelper.GetTaxonomyValues(termStoreId, termSetId);
                //    break;

                case Enumerations.ApiConnectionType.ClientObjectModel:
                    values = csomHelper.GetTaxonomyValues(termStoreId, termSetId);
                    break;

                //case Enumerations.ApiConnectionType.WebServices:
                //    values = WebServiceHelper.GetTaxonomyValues(termStoreId, termSetId);
                //    break;
            }

            fetchedTaxonomyValues.Add(cacheKey, values);

            return values;
        }

        //public List<string> GetSiteColumnGroups()
        //{
        //    List<string> siteColumnGroups = null;

        //    switch (connectionType)
        //    {
        //        case Enumerations.ApiConnectionType.ServerObjectModel:
        //            //fields = SP2013OMHelper.GetSiteColumnGroups();
        //            break;
        //        case Enumerations.ApiConnectionType.ClientObjectModel:
        //            //fields = csomHelper.GetSiteColumnGroups();
        //            break;
        //        case Enumerations.ApiConnectionType.WebServices:
        //            //siteColumnGroups = WebServiceHelper.GetSiteColumnGroups();
        //            break;
        //    }

        //    return siteColumnGroups;
        //}

        public DataTable ExecuteQuery(string listName, MainObject mainobject, Enumerations.QueryType queryType)
        {
            // if no CAML query is constructed, the list doesn't need to be queries for list items
            if (mainobject.CamlDocument == null || mainobject.CamlDocument.ChildNodes.Count == 0 || mainobject.CamlDocument.InnerXml == "<CAML />") return null;

            DataTable resultTable = null;

            switch (connectionType)
            {
                //case Enumerations.ApiConnectionType.ServerObjectModel:
                //    resultTable = SP2013OMHelper.ExecuteQuery(listName, mainobject, queryType);
                //    break;
                case Enumerations.ApiConnectionType.ClientObjectModel:
                    resultTable = csomHelper.ExecuteQuery(listName, mainobject, queryType);
                    break;
                //case Enumerations.ApiConnectionType.WebServices:
                //    resultTable = WebServiceHelper.ExecuteQuery(listName, mainobject, queryType);
                    break;
            }

            return resultTable;
        }

        // this method is used to format the pure CAML into a code snippet in the desired language
        public string FormatCamlString(MainObject mainobject,  string listName, 
            Enumerations.SnippetType snippetType,
            Enumerations.LanguageType languageType,
            Enumerations.QueryType queryType,
            GeneralInfo generalInfo)
        {
            string formattedCamlString = null;

            if (mainobject.CamlDocument == null || mainobject.CamlDocument.ChildNodes.Count == 0) return string.Empty;

            if (snippetType == Enumerations.SnippetType.CAML)
            {
                return Builder.IndentCAML(mainobject.CamlDocument);
            }
            else
            {
                // retrieve the ViewFields element
                XmlNode viewfieldsNode = mainobject.CamlDocument.SelectSingleNode("//ViewFields");

                // retrieve the Where element
                XmlNode whereNode = mainobject.CamlDocument.SelectSingleNode("//Where");

                // retrieve the OrderBy element
                XmlNode orderByNode = mainobject.CamlDocument.SelectSingleNode("//OrderBy");

                // retrieve the QueryOptions element
                XmlNode queryOptionsNode = mainobject.CamlDocument.SelectSingleNode("//QueryOptions");

                switch (snippetType)
                {
                    //case Enumerations.SnippetType.ServerObjectModel:
                    //    formattedCamlString = OMHelper.FormatCamlString(listName, languageType, viewfieldsNode, whereNode, orderByNode, mainobject, queryType);
                    //    break;

                    case Enumerations.SnippetType.ClientObjectModelForDotnet:
                    case Enumerations.SnippetType.ClientObjectModelForJavaScript:
                        formattedCamlString = ClientOMHelper.FormatCamlString(listName, languageType, viewfieldsNode, whereNode, orderByNode, mainobject, snippetType);
                        break;

                    case Enumerations.SnippetType.ClientObjectModelForRestWithJson:
                    case Enumerations.SnippetType.ClientObjectModelForRestWithAtom:
                        formattedCamlString = ClientOMHelper.FormatCamlString(listName, languageType, viewfieldsNode, whereNode, orderByNode, mainobject, snippetType);
                        break;

                    case Enumerations.SnippetType.ClientObjectModelForSilverlight:
                        break;

                    //case Enumerations.SnippetType.WebServices:
                    //    formattedCamlString = ListsWebServiceHelper.FormatCamlString(listName, languageType, viewfieldsNode, whereNode, orderByNode, queryOptionsNode, mainobject);
                    //    break;

                    case Enumerations.SnippetType.Rest:
                        break;

                    case Enumerations.SnippetType.Powershell:
                        formattedCamlString = PowershellHelper.FormatCamlString(listName, languageType, mainobject, generalInfo.SharePointUrl);
                        break;


                }
            }

            return formattedCamlString;
        }


        public void GetSiteColumns()
        {
            throw new NotImplementedException();
        }

        public void GetContentTypes()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
