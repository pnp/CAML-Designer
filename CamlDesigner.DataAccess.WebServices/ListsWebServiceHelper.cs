using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using CamlDesigner.SharePoint.Objects;
using CamlDesigner.Common.Objects;
using CamlDesigner.Common;
using CamlDesigner.SharePoint.Common;

namespace CamlDesigner.DataAccess.SharePoint.WebServices
{
    public class ListsWebServiceHelper
    {
        private string sharePointUrl = null;
        private string username = null;
        private string password = null;
        private string domain = null;
        private bool useDefaultCredentials = false;

        private ListsWebService.Lists listsWebService = null;
        private System.Net.CookieCollection authCookies = null;

        #region Constructors
        public ListsWebServiceHelper(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.useDefaultCredentials = true;
            InitializeWebService(url);
        }

        public ListsWebServiceHelper(string url, string username, string password, string domain)
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

        public CamlDesigner.DataAccess.SharePoint.WebServices.ListsWebService.Lists ListsWebService
        {
            get
            {
                if (listsWebService == null)
                {
                    InitializeWebService(null);
                }
                return listsWebService;
            }
        }

        public System.Net.CookieCollection AuthenticatedCookies
        {
            get { return authCookies; }
            set
            {
                if (value != null)
                {
                    ListsWebService.CookieContainer = new System.Net.CookieContainer();
                    ListsWebService.CookieContainer.Add(value);
                }
                else
                    ListsWebService.CookieContainer = null;
            }
        }
        #endregion

        #region Public Methods
        public List<List> GetLists(string webUrl)
        {
            List<List> lists = null;

            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            XmlNode resultNode = ListsWebService.GetListCollection();
            if (resultNode != null && resultNode.ChildNodes.Count > 0)
                lists = new List<List>();

            foreach (XmlNode listNode in resultNode.ChildNodes)
            {
                List list = new List();

                if (listNode.Attributes["ID"] != null)
                    list.ID = new Guid(listNode.Attributes["ID"].Value);
                if (listNode.Attributes["Title"] != null)
                    list.Title = listNode.Attributes["Title"].Value;
                if (listNode.Attributes["Description"] != null)
                    list.Description = listNode.Attributes["Description"].Value;

                int templateID = 0;
                if (listNode.Attributes["ServerTemplate"] != null)
                    Int32.TryParse(listNode.Attributes["ServerTemplate"].Value, out templateID);
                list.TemplateID = templateID;

                if (listNode.Attributes["DefaultViewUrl"] != null)
                {
                    list.DefaultViewUrl = listNode.Attributes["DefaultViewUrl"].Value;
                    if (list.DefaultViewUrl.LastIndexOf('/') > -1)
                        list.Url = list.DefaultViewUrl.Substring(0, list.DefaultViewUrl.LastIndexOf('/'));
                    else
                        list.Url = list.DefaultViewUrl;
                    if (list.Url.EndsWith("Forms") && list.DefaultViewUrl.LastIndexOf('/') > -1)
                        list.Url = list.Url.Substring(0, list.DefaultViewUrl.LastIndexOf('/'));
                }

                if (listNode.Attributes["ImageUrl"] != null && !string.IsNullOrEmpty(listNode.Attributes["ImageUrl"].Value))
                    list.ImageUrl = listNode.Attributes["ImageUrl"].Value.Substring(listNode.Attributes["ImageUrl"].Value.LastIndexOf('/') + 1);
                if (listNode.Attributes["DocTemplateUrl"] != null && !string.IsNullOrEmpty(listNode.Attributes["DocTemplateUrl"].Value))
                    list.DocumentTemplateUrl = listNode.Attributes["DocTemplateUrl"].Value;


                int itemCount = 0;
                if (listNode.Attributes["ItemCount"] != null)
                    Int32.TryParse(listNode.Attributes["ItemCount"].Value, out itemCount);
                list.ItemCount = itemCount;

                list.Created = Utilities.CreateSystemDateTimeFromISO8601DateTime(listNode.Attributes["Created"].Value);
                list.Modified = Utilities.CreateSystemDateTimeFromISO8601DateTime(listNode.Attributes["Modified"].Value);

                bool enableAttachments = false;
                if (listNode.Attributes["EnableAttachments"] != null)
                    Boolean.TryParse(listNode.Attributes["EnableAttachments"].Value, out enableAttachments);
                list.EnableAttachments = enableAttachments;

                bool enableVersioning = false;
                if (listNode.Attributes["EnableVersioning"] != null)
                    Boolean.TryParse(listNode.Attributes["EnableVersioning"].Value, out enableVersioning);
                list.EnableVersioning = enableVersioning;

                int versionLimit = 0;
                if (listNode.Attributes["MajorVersionLimit"] != null)
                    Int32.TryParse(listNode.Attributes["MajorVersionLimit"].Value, out versionLimit);
                list.MajorVersionLimit = versionLimit;

                versionLimit = 0;
                if (listNode.Attributes["MajorWithMinorVersionsLimit"] != null)
                    Int32.TryParse(listNode.Attributes["MajorWithMinorVersionsLimit"].Value, out versionLimit);
                list.MajorVersionLimit = versionLimit;

                bool enableModeration = false;
                if (listNode.Attributes["EnableModeration"] != null)
                    Boolean.TryParse(listNode.Attributes["EnableModeration"].Value, out enableModeration);
                list.EnableVersioning = enableModeration;

                bool requireCheckout = false;
                if (listNode.Attributes["RequireCheckout"] != null)
                    Boolean.TryParse(listNode.Attributes["RequireCheckout"].Value, out requireCheckout);
                list.RequireCheckout = requireCheckout;

                if (listNode.Attributes["RootFolder"] != null)
                    list.RootFolderUrl = listNode.Attributes["RootFolder"].Value;
                if (listNode.Attributes["SendToLocation"] != null)
                    list.SendToLocationUrl = listNode.Attributes["SendToLocation"].Value;

                // we need the webId (because server OM loads parent if web cannot be found because of mistyped URL)
                list.WebUrl = webUrl;

                // TODO: er zijn nog andere attributes!
                lists.Add(list);
            }

            return lists;
        }

        public List<Folder> GetFolders(string webUrl, string listName)
        {
            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            List<Folder> foldersCollection = null;

            XmlNode foldersNode = GetFolderListItems(listName);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(foldersNode.OuterXml);
            XmlNodeList folderNodes = xmlDoc.GetElementsByTagName("z:row");

            foreach (XmlNode folderNode in folderNodes)
            {
                Folder folder = new Folder();
                folder.Name = folderNode.Attributes["ows_ServerUrl"].Value;

                if (folderNode.Attributes["ows_Modified_x0020_By"] != null)
                    folder.Editor = folderNode.Attributes["ows_Modified_x0020_By"].Value;
                if (folderNode.Attributes["ows_Modified"] != null)
                    folder.Modified = Convert.ToDateTime(folderNode.Attributes["ows_Modified"].Value);

                if (folderNode.Attributes["ows_ServerUrl"] != null)
                {
                    string rootfolder = folderNode.Attributes["ows_FileDirRef"].Value;
                    rootfolder = "/" + rootfolder.Substring(rootfolder.IndexOf(";#") + 2);
                    folder.RelativeUrl = rootfolder;
                }

                if (foldersCollection == null)
                    foldersCollection = new List<Folder>();
                foldersCollection.Add(folder);
            }

            return foldersCollection;
        }

        public List<Field> GetFields(string webUrl, string listName, bool excludeHiddenFields)
        {
            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            List<Field> fieldsCollection = null;
            XmlNode listNode = ListsWebService.GetList(listName);
            if (listNode != null && listNode.ChildNodes.Count > 0)
                fieldsCollection = new List<Field>();

            foreach (XmlNode fieldNode in listNode.SelectNodes("*[local-name()='Fields']/*[local-name()='Field']"))
            {
                Field field = new Field();
                field.Hidden = false;
                field.DataType = fieldNode.Attributes["Type"].Value;

                // store the field name, field type and field ID 
                if (fieldNode.Attributes["ID"] != null)
                    field.ID = new Guid(fieldNode.Attributes["ID"].Value);

                if (fieldNode.Attributes["DisplayName"] != null)
                    field.DisplayName = fieldNode.Attributes["DisplayName"].Value;

                if (fieldNode.Attributes["Hidden"] != null)
                {
                    bool hidden = false;
                    Boolean.TryParse(fieldNode.Attributes["Hidden"].Value, out hidden);
                    field.Hidden = hidden;
                }

                if (fieldNode.Attributes["Name"] != null)
                    field.InternalName = fieldNode.Attributes["Name"].Value;

                if (fieldNode.Attributes["Required"] != null && fieldNode.Attributes["Required"].Value == "TRUE")
                    field.Required = true;
                else
                    field.Required = false;

                if (fieldNode.Attributes["AuthoringInfo"] != null)
                    field.AuthoringInfo = fieldNode.Attributes["AuthoringInfo"].Value;


                if (!excludeHiddenFields || field.InternalName == "FileRef" || (excludeHiddenFields && !field.Hidden))
                {
                    switch (field.DataType)
                    {
                        case "DateTime":
                            DateTimeField dateTimeField = new DateTimeField(field.ID, field.DisplayName, field.InternalName, field.DataType, field.Required, field.Hidden, field.AuthoringInfo);
                            if (fieldNode.Attributes["Format"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["Format"].Value))
                                dateTimeField.DisplayFormat = fieldNode.Attributes["Format"].Value;
                            fieldsCollection.Add(dateTimeField);
                            break;
                        case "Choice":
                        case "MultiChoice":
                            ChoiceField choiceField = new ChoiceField(field.ID, field.DisplayName, field.InternalName, field.DataType, field.Required, field.Hidden, field.AuthoringInfo);
                            choiceField.Choices = new List<string>();

                            foreach (XmlNode choiceNode in fieldNode.SelectNodes("*[local-name()='CHOICES']/*[local-name()='CHOICE']"))
                            {
                                if (!String.IsNullOrEmpty(choiceNode.InnerText))
                                {
                                    if (!choiceField.Choices.Contains(choiceNode.InnerText))
                                        choiceField.Choices.Add(choiceNode.InnerText);
                                }
                            }
                            fieldsCollection.Add(choiceField);
                            break;

                        case "Lookup":
                            LookupField lookupField = new LookupField(field.ID, field.DisplayName, field.InternalName, field.DataType, field.Required, field.Hidden, field.AuthoringInfo);

                            if (field.InternalName == "FileRef")
                            {
                                lookupField.LookupListName = listName;
                                lookupField.ShowField = field.InternalName;
                            }
                            else if (fieldNode.Attributes["List"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["List"].Value))
                            {
                                string listidstring = fieldNode.Attributes["List"].Value;
                                Guid lookupListId = Guid.Empty;
                                if (Guid.TryParse(listidstring, out lookupListId))
                                    lookupField.LookupListId = listidstring;
                                else
                                    lookupField.LookupListName = listidstring;

                                bool listFound = false;

                                // retrieve the name of the list
                                List<List> lists = GetLists(null);
                                if (lists != null)
                                {
                                    // TODO: replace this with linq
                                    foreach (List list in lists)
                                    {
                                        if (!string.IsNullOrEmpty(lookupField.LookupListId) && list.ID == new Guid(lookupField.LookupListId))
                                        {
                                            lookupField.LookupListName = list.Title;
                                            listFound = true;
                                        }
                                        else if (!string.IsNullOrEmpty(lookupField.LookupListName) && list.Title == lookupField.LookupListName)
                                        {
                                            lookupField.LookupListId = list.ID.ToString();
                                            listFound = true;
                                        }
                                        if (listFound) break;
                                    }
                                }

                                if (fieldNode.Attributes["ShowField"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["ShowField"].Value))
                                    lookupField.ShowField = fieldNode.Attributes["ShowField"].Value;
                            }

                            if (fieldNode.Attributes["WebId"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["WebId"].Value))
                                lookupField.LookupWebId = new Guid(fieldNode.Attributes["WebId"].Value);

                            fieldsCollection.Add(lookupField);
                            break;

                        case "Note":
                            NoteField noteField = new NoteField(field.ID, field.DisplayName, field.InternalName, field.DataType, field.Required, field.Hidden, field.AuthoringInfo);

                            if (fieldNode.Attributes["NumLines"] != null && fieldNode.Attributes["NumLines"].Value.Length > 0)
                                noteField.NumberOfLines = System.Convert.ToInt32(fieldNode.Attributes["NumLines"].Value);
                            if (fieldNode.Attributes["RichText"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["RichText"].Value))
                            {
                                if (fieldNode.Attributes["RichText"].Value == "TRUE")
                                    noteField.RichText = true;
                                else
                                    noteField.RichText = false;
                            }
                            if (fieldNode.Attributes["RichTextMode"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["RichTextMode"].Value))
                                noteField.RichTextMode = fieldNode.Attributes["RichTextMode"].Value;
                            break;

                        case "User":
                        case "UserMulti":
                            UserField userField = new UserField(field.ID, field.DisplayName, field.InternalName, field.DataType, field.Required, field.Hidden, field.AuthoringInfo);
                            if (fieldNode.Attributes["UserSelectionMode"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["UserSelectionMode"].Value))
                                userField.UserSelectionMode = fieldNode.Attributes["UserSelectionMode"].Value;
                            userField.MultiSelect = false;
                            fieldsCollection.Add(userField);
                            break;

                        case "TaxonomyFieldType":
                        case "TaxonomyFieldTypeMulti":
                            // TODO: Find out correct Guids, PeterK

                            TaxonomyField taxField = new TaxonomyField(field.ID, field.DisplayName, field.InternalName, field.DataType, field.Required, field.Hidden, field.AuthoringInfo, Guid.Empty, Guid.Empty);

                            // the values for SspId and TermSetId are stored deeper in the xml:
                            // <Customization><ArrayOfProperty><Property><Name>SspId</Name><Value>guid</Value></Property>....</ArrayOfProperty></Customization>
                            XmlNode childNode = fieldNode.ChildNodes[1];
                            XmlNode termNodes = childNode.ChildNodes[0];

                            //XmlNodeList propertyNodes = fieldNode.SelectNodes("/*[local-name()='Customization']/*[local-name()='ArrayOfProperty']/*[local-name()='Property']/*[local-name()='Name']");
                            //XmlNode sspIdNode = fieldNode.SelectSingleNode("/*[local-name()='Customization']/*[local-name()='ArrayOfProperty']/*[local-name()='Property']/*[local-name()='Name']");
                            //XmlNode sspIdNode = fieldNode.SelectSingleNode("/Customization/ArrayOfProperty/Property[Name='SspId']");

                            //if (fieldNode.Attributes["SspId"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["SspId"].Value))
                            //    taxField.TermStoreId = new Guid(fieldNode.Attributes["SspId"].Value);
                            //if (fieldNode.Attributes["TermSetId"] != null && !string.IsNullOrEmpty(fieldNode.Attributes["TermSetId"].Value))
                            //    taxField.TermStoreId = new Guid(fieldNode.Attributes["TermSetId"].Value);
                            fieldsCollection.Add(taxField);
                            break;

                        default:
                            fieldsCollection.Add(field);
                            break;
                    }
                }
            }

            return fieldsCollection;
        }

        public List<ContentType> GetContentTypes(string webUrl, string listName)
        {
            List<ContentType> contentTypes = null;

            if (!string.IsNullOrEmpty(webUrl))
                RefreshWebService(webUrl);

            XmlNode resultNode = ListsWebService.GetListContentTypes(listName, string.Empty);
            if (resultNode != null && resultNode.ChildNodes.Count > 0)
                contentTypes = new List<ContentType>();

            foreach (XmlNode ctNode in resultNode.ChildNodes)
            {
                ContentType contentType = new ContentType();

                // store and trim content type ID
                if (ctNode.Attributes["ID"] != null)
                {
                    string contentTypeID = ctNode.Attributes["ID"].Value;
                    contentTypeID = contentTypeID.Substring(0, contentTypeID.Length - 34);
                    contentType.ID = contentTypeID;
                }

                if (ctNode.Attributes["Name"] != null)
                    contentType.Name = ctNode.Attributes["Name"].Value;

                contentTypes.Add(contentType);
            }

            return contentTypes;
        }

        public List<LookupValue> GetLookupValues(string webUrl, string listGuid, string showField)
        {
            if (string.IsNullOrEmpty(listGuid)) return null;

            bool urlHasChanged = false;
            if (!string.IsNullOrEmpty(webUrl))
            {
                string url = String.Format("{0}/_vti_bin/Lists.asmx", webUrl);
                if (listsWebService != null && listsWebService.Url != url)
                {
                    urlHasChanged = true;
                    listsWebService.Url = url;
                }
            }

            // The GetListItems method cannot retrieve the last based on the list Guid, you first
            // have to retrieve the real list name.

            List<LookupValue> values = null;

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode viewNode = xmlDoc.CreateElement(Enumerations.ViewFields.Tag);
            viewNode.InnerXml = "<FieldRef Name='" + showField + "' />";

            XmlNode queryNode = null;
            XmlNode queryOptionsNode = null;

            if (showField == "FileRef")
            {
                queryNode = xmlDoc.CreateElement(Enumerations.Query.Tag);
                queryNode.InnerXml += CAMLConstants.FolderQuery;

                queryOptionsNode = xmlDoc.CreateElement(Enumerations.QueryOptions.Tag);
                queryOptionsNode.InnerXml += CAMLConstants.QueryOptions.RecursiveAll;
            }

            XmlNode xmlListNode = GetListItems(listGuid, viewNode, queryNode, queryOptionsNode, 0);
            if (xmlListNode != null)
            {
                xmlDoc.LoadXml(xmlListNode.InnerXml);
                XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsMgr.AddNamespace("def", "http://schemas.microsoft.com/sharepoint/soap/");
                nsMgr.AddNamespace("z", "#RowsetSchema");
                string xpathstring = "//z:row[@ows_" + showField + "]";
                XmlNodeList fieldNodes = xmlDoc.SelectNodes(xpathstring, nsMgr);

                if (fieldNodes != null && fieldNodes.Count > 0)
                    values = new List<LookupValue>();

                // fill the combobox
                foreach (XmlNode node in fieldNodes)
                {
                    // get the item ID
                    int itemID = 0;
                    if (node.Attributes.GetNamedItem("ows_ID") != null)
                        int.TryParse(node.Attributes.GetNamedItem("ows_ID").Value, out itemID);
                    string fieldvalue = null;

                    // get the showField
                    if (node.Attributes.GetNamedItem("ows_" + showField) != null)
                        fieldvalue = node.Attributes.GetNamedItem("ows_" + showField).Value;
                    if (itemID > 0 && fieldvalue != null && fieldvalue != string.Empty)
                    {
                        if (fieldvalue.Contains(";#"))
                        {
                            fieldvalue = fieldvalue.Substring(fieldvalue.IndexOf(";#") + 2);
                        }
                        if (showField == "FileRef" && values.Count == 0 && fieldvalue.Contains("/"))
                        {
                            values.Add(new LookupValue(0, fieldvalue.Substring(0, fieldvalue.IndexOf('/'))));
                        }

                        values.Add(new LookupValue(itemID, fieldvalue));
                    }
                }
            }

            if (urlHasChanged)
            {
                //string url = String.Format("{0}/_vti_bin/Lists.asmx", webUrl);
                listsWebService.Url = sharePointUrl;
            }

            return values;
        }

        public DataTable ExecuteQuery(string listName, MainObject mainobject)
        {
            // retrieve the ViewFields element
            mainobject.ViewFieldsNode = mainobject.CamlDocument.SelectSingleNode(string.Format("//{0}", Enumerations.ViewFields.Tag));

            // retrieve the Where element
            mainobject.WhereNode = mainobject.CamlDocument.SelectSingleNode(string.Format("//{0}", Enumerations.Query.Where.Tag));
            // retrieve the OrderBy element
            mainobject.OrderByNode = mainobject.CamlDocument.SelectSingleNode(string.Format("//{0}", Enumerations.Query.OrderBy.Tag));

            // create another XML document, otherwise the original camlDocument is modified
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode queryNode = xmlDoc.CreateElement(Enumerations.Query.Tag);
            if (mainobject.WhereNode != null)
                queryNode.InnerXml += mainobject.WhereNode.OuterXml;
            if (mainobject.OrderByNode != null)
                queryNode.InnerXml += mainobject.OrderByNode.OuterXml;

            // retrieve the QueryOptions element
            XmlNode queryOptionsNode = mainobject.CamlDocument.SelectSingleNode(string.Format("//{0}", Enumerations.QueryOptions.Tag));
            // replace the quotes
            if (queryOptionsNode != null && !string.IsNullOrEmpty(queryOptionsNode.InnerXml))
                queryOptionsNode.InnerXml = queryOptionsNode.InnerXml.Replace('"', '\"');

            // retrieve the RowLimit from the queryOptions node
            int rowLimit = 0;
            if (queryOptionsNode != null && queryOptionsNode.HasChildNodes)
            {
                XmlNode rowLimitNode = queryOptionsNode.SelectSingleNode("//RowLimit");
                if (rowLimitNode != null)
                {
                    int.TryParse(rowLimitNode.InnerText, out rowLimit);
                }
            }

            XmlNode resultNode = GetListItems(listName, mainobject.ViewFieldsNode, queryNode, queryOptionsNode, rowLimit);
            DataTable resultTable = null;
            if (resultNode != null)
            {
                System.IO.StringReader sr = new System.IO.StringReader(resultNode.OuterXml);
                XmlTextReader tr = new XmlTextReader(sr);
                DataSet ds = new DataSet("resultDataSet");
                ds.ReadXml(tr);
                if (ds != null && ds.Tables.Count >= 2)
                    resultTable = ds.Tables[1];
            }

            return resultTable;
        }



        #endregion

        #region Private Methods
        private void InitializeWebService(string url)
        {
            if (!string.IsNullOrEmpty(url))
                sharePointUrl = String.Format("{0}/_vti_bin/Lists.asmx", url);

            if (string.IsNullOrEmpty(sharePointUrl))
                throw new Exception("SharePoint URL cannot be null or empty.");

            listsWebService = new ListsWebService.Lists();
            listsWebService.PreAuthenticate = true;
            listsWebService.Url = sharePointUrl;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(domain))
                    listsWebService.Credentials = new System.Net.NetworkCredential(username, password);
                else
                    listsWebService.Credentials = new System.Net.NetworkCredential(username, password, domain);
            }
            else
            {
                listsWebService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            // TODO: Test if the user has access
            //XmlNode resultNode = listsWebService.GetList("Test");
        }

        private void RefreshWebService(string url)
        {
            if (!string.IsNullOrEmpty(sharePointUrl) && sharePointUrl != url)
            {
                sharePointUrl = String.Format("{0}/_vti_bin/Lists.asmx", url);
                if (listsWebService.Url != sharePointUrl)
                    listsWebService.Url = sharePointUrl;
            }
        }

        public XmlNode GetListItems(string listName, XmlNode viewFieldsNode, XmlNode queryNode, XmlNode queryOptionsNode, int rowLimit)
        {
            // Validate incoming arguments
            if (listName == null)
                throw new ArgumentNullException("List name cannot be null or empty.", listName);

            if (viewFieldsNode == null && queryNode == null && queryOptionsNode == null)
                throw new ApplicationException("The incoming nodes are all empty.");

            // create the necessary xml nodes
            XmlDocument xmlDoc = new XmlDocument();

            if (viewFieldsNode == null)
                viewFieldsNode = xmlDoc.CreateElement(Enumerations.ViewFields.Tag);
            else if (viewFieldsNode.Name != Enumerations.ViewFields.Tag)
                throw new ApplicationException("The ViewFields node is invalid.");

            if (queryNode == null)
                queryNode = xmlDoc.CreateElement(Enumerations.Query.Tag);
            else if (queryNode.Name != Enumerations.Query.Tag)
                throw new ApplicationException("The Query node is invalid.");

            if (queryOptionsNode == null)
                queryOptionsNode = xmlDoc.CreateElement(Enumerations.QueryOptions.Tag);
            else if (queryOptionsNode.Name != Enumerations.QueryOptions.Tag)
                throw new ApplicationException("The QueryOptions node is invalid.");

            XmlNode resultNode = null;
            try
            {
                if (rowLimit > 0)
                    resultNode = ListsWebService.GetListItems(listName, null, queryNode, viewFieldsNode, rowLimit.ToString(), queryOptionsNode, null);
                else
                    resultNode = ListsWebService.GetListItems(listName, null, queryNode, viewFieldsNode, null, queryOptionsNode, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultNode;
        }

        public XmlNode GetFolderListItems(string listName)
        {
            // get all folders using GetListItems
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode query = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode viewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode queryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            queryOptions.InnerXml = @"<IncludeMandatoryColumns>TRUE</IncludeMandatoryColumns>
                                <ViewAttributes Scope='RecursiveAll'/>
                                <DateInUtc>TRUE</DateInUtc>";
            viewFields.InnerXml = "";
            query.InnerXml = @"<Where><Eq><FieldRef Name='FSObjType' /><Value Type='Lookup'>1</Value></Eq></Where>";

            return ListsWebService.GetListItems(listName, null, query, viewFields, null, queryOptions, null);
        }

        #endregion

        #region Static methods for code snippet generation

        public static string FormatCamlString(string listName, Enumerations.LanguageType languageType,
            XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, XmlNode queryOptions, MainObject mainobject)
        {
            if (languageType == Enumerations.LanguageType.CSharp)
                return FormatCamlStringInCSharp(listName, viewfieldsNode, whereNode, orderByNode, queryOptions, mainobject);
            else
                return FormatCamlStringInVbNet(listName, viewfieldsNode, whereNode, orderByNode, queryOptions, mainobject);
        }

        private static string FormatCamlStringInCSharp(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, XmlNode queryOptionsNode, MainObject mainobject)
        {
            StringBuilder sb = new StringBuilder("System.Xml.XmlDocument doc = new System.Xml.XmlDocument(); \n");

            sb.Append("XmlNode queryNode = doc.CreateElement(\"Query\"); \n");
            string querystring = Builder.BuildQuerystring(whereNode, orderByNode);
            if (!string.IsNullOrEmpty(querystring))
            {
                querystring = querystring.Replace("\"", "'");
                sb.Append(string.Format("queryNode.InnerXml = \"{0}\"; \n", querystring));
            }
            //sb.Append("doc.ChildNodes[0].AppendChild(queryNode); \n");
            sb.Append("\n");

            sb.Append("XmlNode viewfieldsNode = doc.CreateElement(\"ViewFields\"); \n");
            if (viewfieldsNode != null)
                sb.Append(string.Format("viewfieldsNode.InnerXml = \"{0}\"; \n", viewfieldsNode.InnerXml.Replace("\"", "'")));
            //sb.Append("doc.ChildNodes[0].AppendChild(node); \n");
            sb.Append("\n");

            sb.Append("XmlNode queryOptionsNode = doc.CreateElement(\"QueryOptions\"); \n");
            //sb.Append("doc.ChildNodes[0].AppendChild(node); \n");

            // retrieve the RowLimit from the queryOptions node
            string rowLimitString = "null";
            if (queryOptionsNode != null && queryOptionsNode.HasChildNodes)
            {
                XmlNode rowLimitNode = queryOptionsNode.SelectSingleNode("//RowLimit");
                if (rowLimitNode != null)
                {
                    rowLimitString = rowLimitNode.InnerText;
                    // remove this node from the queryOptionsNode
                    queryOptionsNode.RemoveChild(rowLimitNode);
                }
            }

            if (queryOptionsNode != null && queryOptionsNode.HasChildNodes)
                sb.Append(string.Format("queryOptionsNode.InnerXml = \"{0}\"; \n", queryOptionsNode.InnerXml.Replace("\"", "'")));
            sb.Append("\n");

            sb.Append(string.Format("System.Xml.XmlNode items = listsWS.GetListItems(\"{0}\", null, queryNode, viewfieldsNode, {1}, queryOptionsNode, null); \n", listName, rowLimitString));

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private static string FormatCamlStringInVbNet(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, XmlNode queryOptionsNode, MainObject mainobject)
        {
            StringBuilder sb = null;

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        #endregion


    }
}
