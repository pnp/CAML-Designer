namespace CamlDesigner.DataAccess.SPO
{
    using CamlDesigner.Common;
    using CamlDesigner.Common.Objects;
    using CamlDesigner.SharePoint.Objects;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    //using MSDN.Samples.ClaimsAuth;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;
    using CustomObjects = CamlDesigner.SharePoint.Objects;

    public class ClientOMHelper
    {
        bool isSharePointOnline = false;
        bool initializeContext = false;
        string spUrl = null;
        string spEcthubUrl = null;

        private string username = null;
        private string password = null;
        //private string domain = null;

        //private string clientId = null;
        //private string clientSecret = null;
        //private string tenantId = null;

        // this collection needs to be preserved for further use
        List<CustomObjects.Field> fieldsCollection;

        private Microsoft.SharePoint.Client.ClientContext clientContext = null;
        // private Microsoft.SharePoint.Client.Site siteCollection = null;
        private Microsoft.SharePoint.Client.Web spWeb = null;
        //private static System.Net.CookieCollection authCookies = null;

        private Microsoft.SharePoint.Client.ClientContext ecthubContext = null;
        private Microsoft.SharePoint.Client.Site ecthubSiteCollection = null;


        public ClientOMHelper(string spUrl, bool initializeContext) : base()
        {
            this.spUrl = spUrl;
            this.initializeContext = initializeContext;
            //var t = new Thread(GetClientContext);
            //t.SetApartmentState(ApartmentState.STA);
            //t.Start();
            //while (t.IsAlive) { };
        }

        //private void GetClientContext()
        //{
        //    if (this.initializeContext)
        //    {
        //        clientContext = ClaimClientContext.GetAuthenticatedContext(this.sp2013url, 600, 600);
        //        //Thread.Sleep(3000);
        //    }
        //}

        //public static ClientContext GetClientContext(string siteUrl, string clientId, string clientSecret, string userAgent)
        //{
        //    ClientContext clientContext = new AuthenticationManager().GetAppOnlyAuthenticatedContext(siteUrl, clientId, clientSecret);

        //    clientContext.ExecutingWebRequest += delegate (object sender, WebRequestEventArgs e)
        //    {
        //        e.WebRequestExecutor.WebRequest.UserAgent = userAgent;
        //    };
        //    clientContext.RequestTimeout = Timeout.Infinite;

        //    return clientContext;
        //}

        public ClientOMHelper(string spurl, string specthuburl, bool initializeContext, string username, string password)
        {
            this.spUrl = spurl;
            this.spEcthubUrl = specthuburl;
            this.username = username;
            this.password = password;

            if (initializeContext)
            {
                InitializeClientContext();
            }
        }

        #region Properties
        public string Url
        {
            get { return spUrl; }
            set
            {
                if (spUrl != value)
                {
                    spUrl = value;
                    InitializeClientContext();
                }
            }
        }

        public string EnterpriseContentTypeHubUrl
        {
            get { return spEcthubUrl; }
            set
            {
                if (spEcthubUrl != value)
                {
                    spEcthubUrl = value;
                    InitializeClientContext();
                }
            }
        }

        private Microsoft.SharePoint.Client.ClientContext ClientContext
        {
            get
            {
                if (clientContext == null && !string.IsNullOrEmpty(spUrl))
                {
                    InitializeClientContext();
                }
                return clientContext;
            }
        }

        private Microsoft.SharePoint.Client.Web SPWeb
        {
            get
            {
                //Thread.Sleep(3000);
                if (spWeb == null)
                {
                    spWeb = ClientContext.Web;
                    ClientContext.Load(spWeb);
                    clientContext.ExecuteQuery();
                }
                return spWeb;
            }
        }

        private List<CustomObjects.Field> FieldsCollection
        {
            get
            {
                if (fieldsCollection == null)
                {
                    this.fieldsCollection = new List<CustomObjects.Field>();
                }

                return fieldsCollection;
            }
        }
        #endregion

        public static Enumerations.SharePointVersions GetSharePointVersion(string siteUrl)
        {
            Enumerations.SharePointVersions spVersion = Enumerations.SharePointVersions.SP2013;

            ClientContext context = new ClientContext(siteUrl);
            context.ExecuteQuery();
            string hive = context.ServerVersion.Major.ToString();
            switch (hive)
            {
                case "12":
                    spVersion = Enumerations.SharePointVersions.SP2007;
                    break;
                case "14":
                    spVersion = Enumerations.SharePointVersions.SP2010;
                    break;
                case "15":
                    spVersion = Enumerations.SharePointVersions.SP2013;
                    break;
            }
            return spVersion;
        }

        //public System.Net.CookieCollection GetAuthenticatedCookies()
        //{
        //    Thread thread = new Thread(SetAuthenticatedCookiesForSharePointOnline);
        //    thread.SetApartmentState(ApartmentState.STA);
        //    thread.Start();
        //    while (thread.IsAlive) { };
        //    return authCookies;
        //}

        public CustomObjects.Web GetWeb(string webUrl)
        {
            if (!string.IsNullOrEmpty(webUrl))
                RefreshWeb(webUrl);

            //spWeb = SPWeb;
            CustomObjects.Web web = new CustomObjects.Web();
            web.Title = SPWeb.Title;
            web.ID = SPWeb.Id;
            web.Url = webUrl;
            return web;
        }

        public List<CustomObjects.Web> GetWebs(string webUrl)
        {
            if (!string.IsNullOrEmpty(webUrl))
                RefreshWeb(webUrl);

            List<CustomObjects.Web> webs = new List<CustomObjects.Web>();

            foreach (Microsoft.SharePoint.Client.Web subweb in spWeb.Webs)
            {
                CustomObjects.Web web = new CustomObjects.Web();
                web.Title = subweb.Title;
                web.ID = subweb.Id;
                web.Url = subweb.ServerRelativeUrl;
                webs.Add(web);
            }

            return webs;
        }

        public List<CustomObjects.List> GetLists(string webUrl)
        {
            if (!string.IsNullOrEmpty(webUrl))
                RefreshWeb(webUrl);

            List<CustomObjects.List> lists = new List<CustomObjects.List>();
            //Microsoft.SharePoint.Client.ListCollection listCollection = spWeb.Lists; 
            //ClientContext.Load(spWeb.Lists,
            //    listCollection => listCollection.Include(prop => prop.Title,
            //                       prop => prop.Id, prop => prop.Hidden));

            ClientContext.Load(SPWeb.Lists);
            ClientContext.ExecuteQuery();
            foreach (Microsoft.SharePoint.Client.List splist in SPWeb.Lists)
            {
                CustomObjects.List list = new CustomObjects.List();
                list.ID = splist.Id;
                list.Title = splist.Title;
                list.Description = splist.Description;
                list.ImageUrl = splist.ImageUrl;
                list.TemplateID = splist.BaseTemplate; // 106 = calendar
                list.ContentTypesEnabled = splist.ContentTypesEnabled;
                list.WebUrl = webUrl;

                if (!string.IsNullOrEmpty(splist.DocumentTemplateUrl))
                    list.DocumentTemplateUrl = splist.DocumentTemplateUrl;

                // TODO: the following was commented out
                /*
                DefaultDisplayFormUrl: "/Lists/Announcements/DispForm.aspx"
                DefaultEditFormUrl: "/Lists/Announcements/EditForm.aspx"
                DefaultNewFormUrl: "/Lists/Announcements/NewForm.aspx"
                DefaultViewUrl: "/Lists/Announcements/AllItems.aspx"
                EnableAttachments: true
                EnableFolderCreation: false
                EnableMinorVersions: false
                EnableModeration: false
                EnableVersioning: false
                ForceCheckout: false
                HasExternalDataSource: false
                Hidden: false
                */

                lists.Add(list);
            }

            return lists;
        }

        public List<CustomObjects.ContentType> GetContentTypes(string webUrl, string listName)
        {
            List<CustomObjects.ContentType> contentTypes = null;

            Microsoft.SharePoint.Client.List sp2013list = SPWeb.Lists.GetByTitle(listName);

            if (sp2013list != null && sp2013list.ContentTypesEnabled && sp2013list.ContentTypes.Count > 0)
            {
                contentTypes = new List<CustomObjects.ContentType>();
                foreach (Microsoft.SharePoint.Client.ContentType sp2013ct in sp2013list.ContentTypes)
                {
                    CustomObjects.ContentType contentType = new CustomObjects.ContentType();
                    contentType.ID = sp2013ct.Id.ToString();
                    contentType.Name = sp2013ct.Name;
                    contentTypes.Add(contentType);
                }
            }

            return contentTypes;
        }

        public List<CustomObjects.Field> GetFields(string listName, bool excludeHiddenFields)
        {
            // retrieve the list
            Microsoft.SharePoint.Client.List spList = SPWeb.Lists.GetByTitle(listName);
            Microsoft.SharePoint.Client.FieldCollection spFields = spList.Fields;
            ClientContext.Load(spList);
            ClientContext.Load(spFields);
            ClientContext.ExecuteQuery();

            // empty the list of fields
            fieldsCollection = null;

            foreach (Microsoft.SharePoint.Client.Field spField in spFields)
            {
                if (!excludeHiddenFields || spField.InternalName == "FileRef" || (excludeHiddenFields && !spField.Hidden))
                {
                    switch (spField.TypeAsString)
                    {
                        case "DateTime":
                            CustomObjects.DateTimeField dateTimeField = new CustomObjects.DateTimeField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, null);
                            Microsoft.SharePoint.Client.FieldDateTime dtField = spField as Microsoft.SharePoint.Client.FieldDateTime;
                            if (dtField != null)
                            {
                                dateTimeField.DisplayFormat = dtField.DisplayFormat.ToString();
                            }
                            FieldsCollection.Add(dateTimeField);
                            break;

                        case "Choice":
                            CustomObjects.ChoiceField choiceField = new CustomObjects.ChoiceField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, null);
                            Microsoft.SharePoint.Client.FieldChoice cField = spField as Microsoft.SharePoint.Client.FieldChoice;
                            if (cField != null && cField.Choices != null && cField.Choices.Length > 0)
                            {
                                choiceField.Choices = new List<string>();
                                foreach (string choice in cField.Choices)
                                {
                                    choiceField.Choices.Add(choice);
                                }
                            }
                            FieldsCollection.Add(choiceField);
                            break;

                        case "MultiChoice":
                            // TODO: hou rekening met de gedefinieerde render control (checkboxes, list)
                            CustomObjects.ChoiceField mchoiceField = new CustomObjects.ChoiceField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, null);
                            Microsoft.SharePoint.Client.FieldMultiChoice mcField = spField as Microsoft.SharePoint.Client.FieldMultiChoice;
                            if (mcField != null && mcField.Choices != null && mcField.Choices.Length > 0)
                            {
                                mchoiceField.Choices = new List<string>();
                                foreach (string choice in mcField.Choices)
                                {
                                    mchoiceField.Choices.Add(choice);
                                }
                            }
                            FieldsCollection.Add(mchoiceField);
                            break;

                        case "Lookup":
                        case "LookupMulti":
                            CustomObjects.LookupField lookupField = new CustomObjects.LookupField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, null);
                            Microsoft.SharePoint.Client.FieldLookup lField = spField as Microsoft.SharePoint.Client.FieldLookup;
                            if (lField != null)
                            {
                                if (!string.IsNullOrEmpty(lField.LookupList) && lField.IsRelationship)
                                {
                                    Guid lookupListId = Guid.Empty;
                                    if (Guid.TryParse(lField.LookupList, out lookupListId))
                                    {
                                        Microsoft.SharePoint.Client.List list = ClientContext.Web.Lists.GetById(lookupListId);
                                        ClientContext.Load(list, l => l.Title);
                                        ClientContext.ExecuteQuery();
                                        lookupField.LookupListName = list.Title;
                                    }

                                    if (!string.IsNullOrEmpty(lField.LookupField))
                                        lookupField.ShowField = lField.LookupField;
                                }
                                else if (lField.InternalName == "FileRef")
                                {
                                    // list all folders in 
                                    lookupField.LookupListName = listName;
                                    lookupField.ShowField = lField.InternalName;
                                }

                                lookupField.LookupWebId = lField.LookupWebId;
                            }
                            FieldsCollection.Add(lookupField);
                            break;

                        case "Note":
                            CustomObjects.NoteField noteField = new CustomObjects.NoteField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, null);
                            Microsoft.SharePoint.Client.FieldMultiLineText nField = spField as Microsoft.SharePoint.Client.FieldMultiLineText;
                            if (nField != null)
                            {
                                noteField.NumberOfLines = nField.NumberOfLines;
                                noteField.RichText = nField.RichText;
                                //noteField.RichTextMode = nField..RichTextMode.ToString();
                            }
                            FieldsCollection.Add(noteField);
                            break;

                        case "User":
                        case "UserMulti":
                            CustomObjects.UserField userField = new CustomObjects.UserField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, null);
                            Microsoft.SharePoint.Client.FieldUser uField = spField as Microsoft.SharePoint.Client.FieldUser;
                            if (uField != null)
                            {
                                userField.UserSelectionMode = uField.SelectionMode.ToString();
                                userField.MultiSelect = false;
                            }
                            FieldsCollection.Add(userField);
                            break;

                        case "TaxonomyFieldType":
                        case "TaxonomyFieldTypeMulti":
                            // <Field Type=\"TaxonomyFieldType\" DisplayName=\"Domain\" List=\"{9c19292c-21fa-4888-ad11-e29310230f15}\" WebId=\"fb701c67-1cb8-4c92-a9f3-0b235a83a10a\" 
                            //         ShowField=\"Term1033\" Required=\"FALSE\" EnforceUniqueValues=\"FALSE\" ID=\"{a7c689ee-e041-4f41-91be-c6023508aab6}\" SourceID=\"{3de05a16-5240-407e-aa86-0e3b7c748959}\" 
                            //         StaticName=\"Domain\" Name=\"Domain\" ColName=\"int5\" RowOrdinal=\"0\" Version=\"1\"><Default /><Customization><ArrayOfProperty><Property><Name>SspId</Name><Value 
                            //          xmlns:q1=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q1:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">c227e26c-c756-4296-8812-155d99166ecc</Value></Property><Property><Name>GroupId</Name></Property><Property><Name>TermSetId</Name><Value xmlns:q2=\"http://www.w3.org/2001/XMLSchema\" 
                            //          p4:type=\"q2:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">3f9e98cd-d2ff-4832-a04d-2455595f5312</Value></Property><Property><Name>AnchorId</Name><Value xmlns:q3=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q3:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">73efb8e9-84a0-4f61-9432-4adeb2f5bdc0</Value> 
                            //          </Property><Property><Name>UserCreated</Name><Value xmlns:q4=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q4:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>Open</Name><Value xmlns:q5=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q5:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>TextField</Name><Value xmlns:q6=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q6:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{d6cf82a3-005f-456a-bb46-128a3a7fe547}</Value></Property><Property><Name>IsPathRendered</Name><Value xmlns:q7=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q7:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>IsKeyword</Name><Value xmlns:q8=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q8:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>TargetTemplate</Name></Property><Property><Name>CreateValuesInEditForm</Name><Value xmlns:q9=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q9:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>FilterAssemblyStrongName</Name><Value xmlns:q10=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q10:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Value></Property><Property><Name>FilterClassName</Name><Value xmlns:q11=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q11:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">Microsoft.SharePoint.Taxonomy.TaxonomyField</Value></Property><Property><Name>FilterMethodName</Name><Value xmlns:q12=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q12:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">GetFilteringHtml</Value></Property><Property><Name>FilterJavascriptProperty</Name><Value xmlns:q13=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q13:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">FilteringJavascript</Value></Property></ArrayOfProperty></Customization></Field>"

                            //<Field Type=\"TaxonomyFieldTypeMulti\" DisplayName=\"Document Types\" List=\"{c6a87187-419e-417b-ad97-d69096803778}\" WebId=\"05f72f32-fe4b-4dd4-b27d-8e2f3a5d31e4\" ShowField=\"Term1033\" Required=\"FALSE\" EnforceUniqueValues=\"FALSE\" Mult=\"TRUE\" Sortable=\"FALSE\" ID=\"{adbe5d68-1e31-4fdf-8ec9-a9cfcbac2fc1}\" SourceID=\"{8a9968c8-8cac-434d-883d-3d485ae4e4a9}\" 
                            //StaticName=\"Document_x0020_Types\" Name=\"Document_x0020_Types\" ColName=\"int3\" RowOrdinal=\"0\" Version=\"1\"><Default /><Customization><ArrayOfProperty><Property><Name>SspId</Name><Value xmlns:q1=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q1:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">1ed04d45-9c18-46e5-b6b8-68ed5d1fcde2</Value></Property>
                            //<Property><Name>GroupId</Name></Property><Property><Name>TermSetId</Name><Value xmlns:q2=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q2:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">d337e462-7dc5-41fd-9fad-bfec60b35186</Value></Property><Property><Name>AnchorId</Name><Value xmlns:q3=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q3:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">00000000-0000-0000-0000-000000000000</Value></Property>
                            //<Property><Name>UserCreated</Name><Value xmlns:q4=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q4:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>Open</Name><Value xmlns:q5=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q5:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property>
                            //<Property><Name>TextField</Name><Value xmlns:q6=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q6:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{be1eed6c-9021-44b1-9eb5-35f0672aeef9}</Value></Property><Property><Name>IsPathRendered</Name><Value xmlns:q7=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q7:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property>
                            //<Property><Name>IsKeyword</Name><Value xmlns:q8=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q8:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>TargetTemplate</Name></Property><Property><Name>CreateValuesInEditForm</Name><Value xmlns:q9=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q9:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property>
                            //<Property><Name>FilterAssemblyStrongName</Name><Value xmlns:q10=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q10:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">Microsoft.SharePoint.Taxonomy, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Value></Property><Property><Name>FilterClassName</Name><Value xmlns:q11=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q11:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">Microsoft.SharePoint.Taxonomy.TaxonomyField</Value></Property>
                            //<Property><Name>FilterMethodName</Name><Value xmlns:q12=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q12:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">GetFilteringHtml</Value></Property><Property><Name>FilterJavascriptProperty</Name><Value xmlns:q13=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q13:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">FilteringJavascript</Value></Property></ArrayOfProperty></Customization></Field>"

                            XElement schemaRoot = XElement.Parse(spField.SchemaXml);
                            string sspIdAsString = schemaRoot.Descendants("Property").SingleOrDefault(p => p.Element("Name").Value.Equals("SspId")).Element("Value").Value;
                            string termSetIdAsString = schemaRoot.Descendants("Property").SingleOrDefault(p => p.Element("Name").Value.Equals("TermSetId")).Element("Value").Value;

                            Guid sspId = Guid.Empty, termSetId = Guid.Empty;
                            Guid.TryParse(sspIdAsString, out sspId);
                            Guid.TryParse(termSetIdAsString, out termSetId);

                            CustomObjects.TaxonomyField taxonomyField = new CustomObjects.TaxonomyField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, null, sspId, termSetId);

                            if (spField.TypeAsString == "TaxonomyFieldTypeMulti")
                                taxonomyField.MultiSelect = true;
                            FieldsCollection.Add(taxonomyField);
                            break;

                        default:
                            CustomObjects.Field field = new CustomObjects.Field()
                            {
                                ID = spField.Id,
                                DisplayName = spField.Title,
                                InternalName = spField.InternalName,
                                DataType = spField.TypeAsString,
                                Required = spField.Required,
                                Hidden = spField.Hidden
                            };

                            FieldsCollection.Add(field);
                            break;

                    }
                }
            }

            return fieldsCollection;
        }

        public List<CustomObjects.Field> GetFields(string webUrl, string contentTypeID)
        {
            fieldsCollection = null;

            Microsoft.SharePoint.Client.Web sp2013web = clientContext.Site.OpenWeb(webUrl);
            sp2013web = ClientContext.Web;
            Microsoft.SharePoint.Client.ContentType sp2013ct = sp2013web.ContentTypes.GetById(contentTypeID);

            if (sp2013ct != null)
            {
                fieldsCollection = new List<CustomObjects.Field>();

                foreach (Microsoft.SharePoint.Client.Field sp2010field in sp2013ct.Fields)
                {
                    CustomObjects.Field field = new CustomObjects.Field();
                    field.ID = sp2010field.Id;
                    field.DisplayName = field.DisplayName;
                    field.InternalName = field.InternalName;
                    field.DataType = field.DataType;
                    fieldsCollection.Add(field);
                }

            }

            return fieldsCollection;
        }

        public List<CustomObjects.GroupValue> GetGroups()
        {
            List<CustomObjects.GroupValue> groups = null;

            GroupCollection groupCollection = clientContext.Web.SiteGroups;
            ClientContext.Load(groupCollection);
            clientContext.ExecuteQuery();

            if (groupCollection != null)
            {
                groups = new List<GroupValue>();
                foreach (Group group in groupCollection)
                {
                    groups.Add(new GroupValue(group.Id, group.Title));
                }
            }

            return groups;
        }

        public List<CustomObjects.LookupValue> GetLookupValues(string listGuid, string showField, Guid webId)
        {
            List<CustomObjects.LookupValue> values = null;

            if (webId == SPWeb.Id)
                values = GetLookupValues(ClientContext, SPWeb, listGuid, showField);
            else
            {
                // if the webId is different, it means that it concerns a cross-site lookup, which is generally created on root web level.
                // Load the site so that the Guid can be checked
                ClientContext.Load(ClientContext.Site.RootWeb);
                clientContext.ExecuteQuery();

                if (clientContext.Site.RootWeb.Id == webId)
                {
                    ClientContext tempContext = new ClientContext(spUrl);
                    Microsoft.SharePoint.Client.Web lookupWeb = tempContext.Site.OpenWebById(webId);
                    tempContext.Load(lookupWeb);
                    tempContext.ExecuteQuery();
                    values = GetLookupValues(tempContext, lookupWeb, listGuid, showField);
                }
            }

            return values;
        }

        private List<CustomObjects.LookupValue> GetLookupValues(ClientContext context, Microsoft.SharePoint.Client.Web web, string listTitleOrGuid, string showField)
        {
            if (string.IsNullOrEmpty(listTitleOrGuid)) return null;

            // check if the incoming string is a guid or a real list name
            Guid listId = Guid.Empty;
            try
            {
                listId = new Guid(listTitleOrGuid);
            }
            catch { }

            // retrieve the list
            Microsoft.SharePoint.Client.List spList = null;
            if (listId != Guid.Empty)
                spList = SPWeb.Lists.GetById(listId);
            else
                spList = SPWeb.Lists.GetByTitle(listTitleOrGuid);

            ClientContext.Load(spList);
            ClientContext.ExecuteQuery();

            List<CustomObjects.LookupValue> values = null;

            // TODO: bouw iets in wanneer er te veel list items zijn
            if (spList != null && spList.ItemCount > 0)
            {
                values = new List<CustomObjects.LookupValue>();
                Microsoft.SharePoint.Client.CamlQuery query = new Microsoft.SharePoint.Client.CamlQuery();
                Microsoft.SharePoint.Client.ListItemCollection listItems = null;

                if (showField == "FileRef")
                {
                    query.ViewXml =
                          @"<View Scope='RecursiveAll'>  
                                <Query> 
                                   <Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></Where> 
                                </Query> 
                          </View>";

                    // first add the rootfolder
                    Microsoft.SharePoint.Client.Folder folder = spList.RootFolder;
                    ClientContext.Load(folder);
                    ClientContext.ExecuteQuery();
                    values.Add(new CustomObjects.LookupValue(0, spList.RootFolder.ServerRelativeUrl));
                }

                listItems = spList.GetItems(query);
                ClientContext.Load(listItems);
                clientContext.ExecuteQuery();

                foreach (Microsoft.SharePoint.Client.ListItem listitem in listItems)
                    values.Add(new CustomObjects.LookupValue(listitem.Id, listitem[showField].ToString()));
            }

            return values;
        }

        public List<CustomObjects.TaxonomyValue> GetTaxonomyValues(Guid termStoreId, Guid termSetId)
        {
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(ClientContext);
            ClientContext.Load(taxonomySession.TermStores);
            TermStore defaultTermStore = taxonomySession.TermStores.GetById(termStoreId);
            TermSet termSet = defaultTermStore.GetTermSet(termSetId);
            TermCollection terms = termSet.GetAllTerms();

            ClientContext.Load(taxonomySession);
            ClientContext.Load(defaultTermStore);
            ClientContext.Load(termSet);
            ClientContext.Load(terms);
            ClientContext.ExecuteQuery();

            Dictionary<Guid, TaxonomyValue> taxonomyDictionary = new Dictionary<Guid, TaxonomyValue>();

            if (terms != null)
            {
                foreach (Term term in terms)
                {
                    int[] wssIds = GetWssIdsForTerm(term);
                    taxonomyDictionary.Add(term.Id, new TaxonomyValue(term.Id, term.Name, Guid.Empty, wssIds));
                }

                foreach (Term term in terms)
                {
                    TaxonomyValue parent = taxonomyDictionary[term.Id];

                    if (term.TermsCount > 0)
                    {
                        TermCollection childTerms = term.Terms;
                        ClientContext.Load(childTerms);
                        ClientContext.ExecuteQuery();
                        AddTermsToDictionary(parent, childTerms, ref taxonomyDictionary);
                    }
                }
            }

            return taxonomyDictionary.Values.ToList();
        }

        public CustomObjects.TaxonomyValue GetTaxonomyValue(Guid termStoreId, Guid termSetId, string input)
        {
            TaxonomyValue taxonomyValue = null;

            // check if the input is an integer or a string
            string termName = null;
            int wssId = 0;
            int.TryParse(input, out wssId);
            if (wssId > 0)
            {
                // retrieve the taxonomy value based on the wss Id
                termName = GetTermForWssId(wssId);
            }
            else
                termName = input;

            if (!string.IsNullOrEmpty(termName))
            {
                try
                {
                    TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(ClientContext);
                    ClientContext.Load(taxonomySession.TermStores);
                    TermStore defaultTermStore = taxonomySession.TermStores.GetById(termStoreId);
                    TermSet termSet = defaultTermStore.GetTermSet(termSetId);
                    Term term = termSet.Terms.GetByName(termName);

                    ClientContext.Load(taxonomySession);
                    ClientContext.Load(defaultTermStore);
                    ClientContext.Load(termSet);
                    ClientContext.Load(term);
                    ClientContext.ExecuteQuery();

                    // get the wss Id of the term
                    int[] wssIds = null;
                    if (termName == input)
                        wssIds = GetWssIdsForTerm(term);
                    else
                        wssIds = new int[] { wssId };

                    taxonomyValue = new TaxonomyValue(term.Id, term.Name, Guid.Empty, wssIds);
                }
                catch (Exception ex)
                {
                    if (ex is ServerException)
                    {
                        //TODO: term is wrong or not found
                        taxonomyValue = new TaxonomyValue(Guid.Empty, "Term is not found or hasn't been used yet", Guid.Empty, new int[] { 0 });
                    }
                }
            }


            return taxonomyValue;
        }

        public void AddTermsToDictionary(TaxonomyValue parent, TermCollection terms, ref Dictionary<Guid, TaxonomyValue> taxonomyDictionary)
        {
            foreach (Term child in terms)
            {
                TaxonomyValue tv = null;
                if (taxonomyDictionary.ContainsKey(child.Id))
                {
                    tv = taxonomyDictionary[child.Id];
                    if (tv.ParentId != parent.ID)
                    {
                        tv.ParentId = parent.ID;
                    }
                }
                else
                {
                    int[] wssIds = GetWssIdsForTerm(child);
                    tv = new TaxonomyValue(child.Id, child.Name, parent.ID, wssIds);
                    parent.Terms.Add(tv);
                }

                if (child.TermsCount > 0 && tv != null)
                {
                    TermCollection childTerms = child.Terms;
                    ClientContext.Load(childTerms);
                    ClientContext.ExecuteQuery();
                    AddTermsToDictionary(tv, childTerms, ref taxonomyDictionary);
                }
            }
        }

        private string GetTermForWssId(int wssId)
        {
            string termName = null;

            Microsoft.SharePoint.Client.List taxonomyList = SPWeb.Lists.GetByTitle("TaxonomyHiddenList");
            clientContext.Load(taxonomyList);
            clientContext.ExecuteQuery();

            if (taxonomyList != null)
            {
                try
                {
                    ListItem item = taxonomyList.GetItemById(wssId);
                    clientContext.Load(item);
                    clientContext.ExecuteQuery();
                    if (item != null)
                    {
                        termName = item["Term"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ServerException)
                    {
                        throw;
                    }
                }
            }

            return termName;
        }

        public int[] GetWssIdsForTerm(Term term)
        {
            List<int> idList = null;

            Microsoft.SharePoint.Client.List taxonomyList = SPWeb.Lists.GetByTitle("TaxonomyHiddenList");
            clientContext.Load(taxonomyList);
            clientContext.ExecuteQuery();

            if (taxonomyList != null)
            {
                // build the query
                Microsoft.SharePoint.Client.CamlQuery query = new Microsoft.SharePoint.Client.CamlQuery();
                query.ViewXml =
                    "<View><Query>"
                    + "<Where><Eq><FieldRef Name='IdForTerm'/><Value Type='Text'>" + term.Id + "</Value></Eq></Where>"
                    + "</Query></View>";

                Microsoft.SharePoint.Client.ListItemCollection items = taxonomyList.GetItems(query);
                clientContext.Load(items);
                clientContext.ExecuteQuery();

                if (items.Count > 0)
                {
                    idList = new List<int>();
                    foreach (ListItem item in items)
                    {
                        idList.Add(item.Id);
                    }
                }
            }

            if (idList == null)
                return null;
            else
                return idList.ToArray();
        }

        public DataTable ExecuteQuery(string listName, MainObject mainobject, CamlDesigner.Common.Enumerations.QueryType queryType)
        {
            DataTable resultTable = null;

            // retrieve the ViewFields element
            mainobject.ViewFieldsNode = mainobject.CamlDocument.SelectSingleNode("//ViewFields");

            // retrieve the Where element
            mainobject.WhereNode = mainobject.CamlDocument.SelectSingleNode("//Where");

            // retrieve the OrderBy element
            mainobject.OrderByNode = mainobject.CamlDocument.SelectSingleNode("//OrderBy");

            // retrieve the QueryOptions element
            mainobject.QueryOptionsNode = mainobject.CamlDocument.SelectSingleNode("//QueryOptions");


            if (mainobject.WhereNode != null || mainobject.OrderByNode != null || mainobject.ViewFieldsNode != null || mainobject.QueryOptions != null)
            {
                resultTable = ExecuteCAMLQuery(listName, mainobject);

                //switch (queryType)
                //{
                //    case Common.Enumerations.QueryType.CAMLQuery:
                //        resultTable = ExecuteCAMLQuery(listName, viewfieldsNode, whereNode, orderByNode, queryOptionsNode, queryOptions);
                //        break;
                //    case Common.Enumerations.QueryType.SiteDataQuery:
                //        resultTable = ExecuteSiteDataQuery(viewfieldsNode, whereNode, orderByNode, queryOptionsNode, queryOptions);
                //        break;
                //}
            }

            return resultTable;
        }

        private DataTable ExecuteCAMLQuery(string listName, MainObject mainobject)
        {
            DataTable resultTable = null;

            // retrieve the list
            Microsoft.SharePoint.Client.List spList = SPWeb.Lists.GetByTitle(listName);
            clientContext.Load(spList);
            clientContext.ExecuteQuery();


            if (spList != null && spList.ItemCount > 0 && (mainobject.WhereNode != null || mainobject.OrderByNode != null || mainobject.ViewFieldsNode != null || mainobject.QueryOptions != null))
            {
                // build the query
                Microsoft.SharePoint.Client.CamlQuery query = new Microsoft.SharePoint.Client.CamlQuery();

                query.ViewXml = "<View>";

                // check if there are query options concerning files and folders
                if (mainobject.QueryOptions != null)
                {
                    if (mainobject.QueryOptions.QueryFilesAllFoldersDeep || mainobject.QueryOptions.QueryFoldersAllFoldersDeep || mainobject.QueryOptions.QueryFilesAndFoldersAllFoldersDeep)
                    {
                        query.ViewXml = "<View Scope=\"RecursiveAll\">";
                    }
                    else if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                    {
                        if (mainobject.QueryOptions.QueryFilesInSubFolderDeep || mainobject.QueryOptions.QueryFoldersInSubFolder)
                            query.ViewXml = "<View Scope=\"Recursive\">";
                        else if (mainobject.QueryOptions.QueryFilesAndFoldersInSubFolderDeep)
                            query.ViewXml = "<View Scope=\"RecursiveAll\">";
                        else if (mainobject.QueryOptions.QueryFilesInSubFolder)
                            query.ViewXml = "<View Scope=\"FilesOnly\">";
                    }
                }

                // build the ViewFields part
                if (mainobject.ViewFieldsNode != null)
                {
                    query.ViewXml += mainobject.ViewFieldsNode.OuterXml;
                }

                // build the query part
                if (mainobject.WhereNode != null || mainobject.OrderByNode != null || mainobject.ViewFieldsNode != null)
                {
                    query.ViewXml += "<Query>";

                    // build the Where clause, taking into account queries on files and folders
                    if (mainobject.WhereNode != null)
                        query.ViewXml += mainobject.WhereNode.OuterXml;

                    if (mainobject.OrderByNode != null)
                        query.ViewXml += mainobject.OrderByNode.OuterXml;
                    query.ViewXml += "</Query>";

                    // TODO: this still seems to return all columns
                    if (mainobject.ViewFieldsNode != null)
                        query.ViewXml += mainobject.ViewFieldsNode.OuterXml;
                }

                // build the queryoptions part
                if (mainobject.QueryOptions != null)
                {
                    // following options doesn't seem to work with the client object model
                    //if (queryOptions.IncludeMandatoryColumns)
                    //    query.ViewXml += "<IncludeMandatoryColumns>True</IncludeMandatoryColumns>";
                    //if (queryOptions.ExpandUserField)
                    //    query.ViewXml += "<ExpandUserField>True</ExpandUserField>";
                    //if (queryOptions.IncludeAttachmentUrls)
                    //    query.IncludeAttachmentUrls = true;
                    //if (queryOptions.IncludeAttachmentVersion)
                    //    query.IncludeAttachmentVersion = true;


                    if (mainobject.QueryOptions.UtcDate)
                        query.DatesInUtc = true;

                    if (mainobject.QueryOptions.RowLimit > 0)
                        query.ViewXml += string.Format("<RowLimit>{0}</RowLimit>", mainobject.QueryOptions.RowLimit.ToString());

                    if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                        query.FolderServerRelativeUrl = mainobject.QueryOptions.SubFolder;
                }

                query.ViewXml += "</View>";

                // execute the query
                try
                {
                    ListItemCollection listItems = spList.GetItems(query);
                    ClientContext.Load(listItems);
                    clientContext.ExecuteQuery();

                    resultTable = GetDataTable(listItems);
                }
                catch { }

            }

            return resultTable;
        }

        //private DataTable ExecuteSiteDataQuery(XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, XmlNode queryOptionsNode, CustomObjects.QueryOptions queryOptions)
        //{
        //    DataTable resultTable = null;

        //    if (whereNode != null || orderByNode != null || viewfieldsNode != null || queryOptions != null)
        //    {
        //        // build the query
        //        Microsoft.SharePoint.Client.CamlQuery query = new Microsoft.SharePoint.Client.CamlQuery();
        //    }

        //    return resultTable;
        //}

        private DataTable GetDataTable(ListItemCollection listItems)
        {
            DataTable resultTable = null;

            if (listItems != null && listItems.Count > 0 && fieldsCollection != null)
            {

                try
                {
                    resultTable = new DataTable();

                    // retrieve all fields 

                    foreach (ListItem item in listItems)
                    {
                        //item.FieldValues contains DisplayName as key and the value as Value
                        if (resultTable.Columns == null || resultTable.Columns.Count == 0)
                        {
                            foreach (KeyValuePair<string, object> fieldValue in item.FieldValues)
                            {
                                // look for the field that has the same display name
                                CustomObjects.Field field = null;
                                try
                                {
                                    field = fieldsCollection.FirstOrDefault(f => f.InternalName == fieldValue.Key);
                                    if (field != null)
                                    {
                                        DataColumn column = new DataColumn(field.InternalName);
                                        column.DataType = field.DataType.GetType();
                                        resultTable.Columns.Add(column);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                } // return a null resultTable if query contains an error
                            }
                        }

                        // add a new row to the DataTable
                        DataRow row = resultTable.NewRow();
                        resultTable.Rows.Add(row);

                        // insert the values
                        foreach (KeyValuePair<string, object> fieldValue in item.FieldValues)
                        {
                            if (resultTable.Columns.Contains(fieldValue.Key))
                                row[fieldValue.Key] = GetFieldValue(fieldValue.Value);
                        }
                    }
                }
                catch { } // return a null resultTable if query contains an error
            }
            return resultTable;
        }

        private string GetFieldValue(object itemValue)
        {
            // FieldLookupValue
            string valueAsString = null;

            if (itemValue != null)
            {
                if (itemValue is string[])
                {
                    string[] values = itemValue as string[];
                    foreach (string value in values)
                    {
                        if (!string.IsNullOrEmpty(valueAsString))
                            valueAsString += ";#";
                        valueAsString += value;
                    }
                    // TODO: remove the 2 last characters
                }
                else if (itemValue is FieldLookupValue)
                    valueAsString = ((FieldLookupValue)itemValue).LookupValue;
                else if (itemValue is FieldUserValue)
                    valueAsString = ((FieldUserValue)itemValue).LookupValue;
                else if (itemValue is FieldUserValue[])
                    valueAsString = ((FieldUserValue[])itemValue)[0].LookupValue;
                else if (itemValue is Object[])
                {
                    string[] arr = ((IEnumerable)itemValue).Cast<object>()
                                   .Select(x => x.ToString())
                                   .ToArray();
                    if (arr != null && arr.Count() > 0)
                    {
                        for (int i = 0; i < arr.Count(); i++)
                        {
                            if (i > 0)
                                valueAsString += ";#";
                            valueAsString += arr[i];
                        }
                    }
                }
                else
                    valueAsString = itemValue.ToString();
            }

            return valueAsString;
        }

        #region Static methods for code snippet generation

        public static string FormatCamlString(string listName,
            CamlDesigner.Common.Enumerations.LanguageType languageType,
            XmlNode viewfieldsNode,
            XmlNode whereNode,
            XmlNode orderByNode,
            MainObject mainobject,
            CamlDesigner.Common.Enumerations.SnippetType snippetType)
        {
            string formattedString = null;
            switch (snippetType)
            {
                case Common.Enumerations.SnippetType.ClientObjectModelForDotnet:
                    if (languageType == CamlDesigner.Common.Enumerations.LanguageType.CSharp)
                        formattedString = FormatCamlStringInCSharpForDotNet(listName, viewfieldsNode, whereNode, orderByNode, mainobject);
                    else
                        formattedString = FormatCamlStringInVbNetForDotNet(listName, viewfieldsNode, whereNode, orderByNode, mainobject);
                    break;
                case Common.Enumerations.SnippetType.ClientObjectModelForJavaScript:
                    if (languageType == CamlDesigner.Common.Enumerations.LanguageType.CSharp)
                        formattedString = FormatCamlStringInCSharpForJavaScript(listName, viewfieldsNode, whereNode, orderByNode, mainobject);
                    else
                        formattedString = FormatCamlStringInVbNetForJavaScript(listName, viewfieldsNode, whereNode, orderByNode, mainobject);
                    break;
                case Common.Enumerations.SnippetType.ClientObjectModelForSilverlight:
                    if (languageType == CamlDesigner.Common.Enumerations.LanguageType.CSharp)
                        formattedString = FormatCamlStringInCSharpForSilverlight(listName, viewfieldsNode, whereNode, orderByNode, mainobject);
                    else
                        formattedString = FormatCamlStringInVbNetForSilverlight(listName, viewfieldsNode, whereNode, orderByNode, mainobject);
                    break;
                case Enumerations.SnippetType.ClientObjectModelForRestWithJson:
                case Enumerations.SnippetType.ClientObjectModelForRestWithAtom:
                    if (languageType == CamlDesigner.Common.Enumerations.LanguageType.CSharp)
                        formattedString = FormatCamlStringInCSharpForRest(listName, viewfieldsNode, whereNode, orderByNode, mainobject, snippetType);
                    else
                        formattedString = FormatCamlStringInVbNetForRest(listName, viewfieldsNode, whereNode, orderByNode, mainobject, snippetType);

                    break;
            }

            return formattedString;
        }

        //public static string FormatCamlString(string listName, 
        //    CamlDesigner.SharePoint.Common.Enumerations.LanguageType languageType,
        //    XmlNode viewfieldsNode,
        //    XmlNode whereNode,
        //    XmlNode orderByNode,
        //    MainObject mainobject, 
        //    CamlDesigner.SharePoint.Common.Enumerations.SnippetType snippetType)
        //{
        //    string formattedString = null;
        //    if (languageType == CamlDesigner.SharePoint.Common.Enumerations.LanguageType.CSharp)
        //        formattedString = FormatCamlStringInCSharpForRest(listName, viewfieldsNode, whereNode, orderByNode, mainobject, snippetType);
        //    else
        //        formattedString = FormatCamlStringInVbNetForRest(listName, viewfieldsNode, whereNode, orderByNode, mainobject, snippetType);

        //    return formattedString;
        //}
        #endregion

        #region Static methods for code snippet generation for CSOM .NET

        private static string FormatCamlStringInCSharpForDotNet(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, MainObject mainobject)
        {
            StringBuilder sb = new StringBuilder("ClientContext clientContext = new ClientContext(\"your site\"); \n");
            sb.Append(string.Format("Microsoft.SharePoint.Client.List spList = clientContext.Web.Lists.GetByTitle(\"{0}\"); \n", listName));
            sb.Append("clientContext.Load(spList); \n");
            sb.Append("clientContext.ExecuteQuery(); \n");
            sb.Append("\n");
            sb.Append("if (spList != null && spList.ItemCount > 0) \n");
            sb.Append("{\n");
            sb.Append("   Microsoft.SharePoint.Client.CamlQuery camlQuery = new CamlQuery(); \n");


            if (whereNode != null || orderByNode != null || viewfieldsNode != null || mainobject.QueryOptions != null)
            {
                // part of the query options concerning files and folders generate an additional attribute in the <View> element
                sb.Append("   camlQuery.ViewXml = \n");

                string viewstring = "<View{0}>";
                string scopestring = string.Empty;

                // check if there are query options concerning files and folders
                if (mainobject.QueryOptions != null)
                {
                    if (mainobject.QueryOptions.QueryFilesAllFoldersDeep || mainobject.QueryOptions.QueryFoldersAllFoldersDeep || mainobject.QueryOptions.QueryFilesAndFoldersAllFoldersDeep)
                    {
                        scopestring = " Scope='RecursiveAll'";
                    }
                    else if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                    {
                        if (mainobject.QueryOptions.QueryFilesInSubFolderDeep || mainobject.QueryOptions.QueryFoldersInSubFolder)
                            scopestring = " Scope='Recursive'";
                        else if (mainobject.QueryOptions.QueryFilesAndFoldersInSubFolderDeep)
                            scopestring = " Scope='RecursiveAll'";
                        else if (mainobject.QueryOptions.QueryFilesInSubFolder)
                            scopestring = " Scope='FilesOnly'";
                    }
                }

                sb.Append("      @\"" + string.Format(viewstring, scopestring) + "  \n");

                string querystring = CamlDesigner.SharePoint.Common.Builder.BuildQuerystring(whereNode, orderByNode);
                if (!string.IsNullOrEmpty(querystring))
                {
                    sb.Append("            <Query> \n");
                    sb.Append("               " + querystring + " \n");
                    sb.Append("            </Query> \n");
                }

                // TODO: is this correct???
                if (viewfieldsNode != null)
                {
                    sb.Append("             " + viewfieldsNode.OuterXml.Replace("\"", "'") + " \n");
                }

                // handle RowLimit
                if (mainobject.QueryOptions != null && mainobject.QueryOptions.RowLimit > 0)
                    sb.Append("            <RowLimit>" + mainobject.QueryOptions.RowLimit.ToString() + "</RowLimit> \n");

                sb.Append("      </View>\";  \n");

                // handle QueryOptions
                if (mainobject.QueryOptions != null)
                {
                    if (mainobject.QueryOptions.UtcDate)
                        sb.Append("   camlQuery.DatesInUtc = true; \n");

                    if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                        sb.Append("   camlQuery.FolderServerRelativeUrl = \"" + mainobject.QueryOptions.SubFolder + "\"; \n");
                }
            }
            sb.Append("\n");

            sb.Append("   ListItemCollection listItems = spList.GetItems(camlQuery); \n");
            sb.Append("   clientContext.Load(listItems); \n");
            sb.Append("   clientContext.ExecuteQuery(); \n");
            sb.Append("}\n");

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private static string FormatCamlStringInVbNetForDotNet(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, MainObject mainobject)
        {
            StringBuilder sb = null;

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }
        #endregion

        #region Static methods for code snippet generation for CSOM JavaScript
        /*
        function get_EmployeeSales(employeeName) {
                try {
                    this.controlToLoad = this.baseControl + "_" + "tblEmployeeSales";
                    var context = new SP.ClientContext.get_current();

                    // Load the web object    
                    this.web = context.get_web();

                    //Get the list
                    var list = this.web.get_lists().getByTitle('Sales');

                    // Get all items based on query
                    var query = '<View Scope=\'RecursiveAll\'>' +
                        '<Query>' +
                            '<Where>' +
                            '<Eq>' +
                                '<FieldRef Name=\'EmployeeName\' />' +
                                '<Value Type=\'LookUp\'>' + employeeName + '</Value>' +
                            '</Eq>' +
                            '</Where>' +
                        '</Query>' +
                             '</View>';

                    var camlQuery = new SP.CamlQuery();
                    camlQuery.set_viewXml(query);

                    this.salesDetails = list.getItems(camlQuery);                    

                    // Load the web in the context and retrieve only selected columns to improve performance                            
                    context.load(this.salesDetails, 'Include(ID,EmployeeName,SalesAchieved,SaleDate,Customer,Title)');

                    //Make a query call to execute the above statements
                    context.executeQueryAsync(Function.createDelegate(this, this.get_EmployeeSales_onSuccess), Function.createDelegate(this, this.get_EmployeeSales_onFailure));                    
                }
                catch (e) {
                    alert("error occurred" + e.toString());
                }
            }
         */
        private static string FormatCamlStringInCSharpForJavaScript(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, MainObject mainobject)
        {
            StringBuilder sb = new StringBuilder("var clientContext = new SP.ClientContext.get_current(); \n");
            sb.Append(string.Format("var spList = clientContext.get_web().get_lists.getByTitle(\"{0}\"); \n", listName));
            //sb.Append("clientContext.Load(spList); \n");
            //sb.Append("clientContext.ExecuteQuery(); \n");
            sb.Append("\n");
            sb.Append("if (spList != null && spList.ItemCount > 0) \n");
            sb.Append("{\n");
            sb.Append("   var camlQuery = new SP.CamlQuery(); \n");


            if (whereNode != null || orderByNode != null || viewfieldsNode != null || mainobject.QueryOptions != null)
            {
                // part of the query options concerning files and folders generate an additional attribute in the <View> element
                sb.Append("   camlQuery.ViewXml = \n");

                string viewstring = "<View{0}>";
                string scopestring = string.Empty;

                // check if there are query options concerning files and folders
                if (mainobject.QueryOptions != null)
                {
                    if (mainobject.QueryOptions.QueryFilesAllFoldersDeep || mainobject.QueryOptions.QueryFoldersAllFoldersDeep || mainobject.QueryOptions.QueryFilesAndFoldersAllFoldersDeep)
                    {
                        scopestring = " Scope='RecursiveAll'";
                    }
                    else if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                    {
                        if (mainobject.QueryOptions.QueryFilesInSubFolderDeep || mainobject.QueryOptions.QueryFoldersInSubFolder)
                            scopestring = " Scope='Recursive'";
                        else if (mainobject.QueryOptions.QueryFilesAndFoldersInSubFolderDeep)
                            scopestring = " Scope='RecursiveAll'";
                        else if (mainobject.QueryOptions.QueryFilesInSubFolder)
                            scopestring = " Scope='FilesOnly'";
                    }
                }

                sb.Append("      @\"" + string.Format(viewstring, scopestring) + "  \n");

                string querystring = CamlDesigner.SharePoint.Common.Builder.BuildQuerystring(whereNode, orderByNode);
                if (!string.IsNullOrEmpty(querystring))
                {
                    sb.Append("            <Query> \n");
                    sb.Append("               " + querystring + " \n");
                    sb.Append("            </Query> \n");
                }

                // TODO: is this correct???
                if (viewfieldsNode != null)
                {
                    sb.Append("             " + viewfieldsNode.OuterXml.Replace("\"", "'") + " \n");
                }

                // handle RowLimit
                if (mainobject.QueryOptions != null && mainobject.QueryOptions.RowLimit > 0)
                    sb.Append("            <RowLimit>" + mainobject.QueryOptions.RowLimit.ToString() + "</RowLimit> \n");

                sb.Append("      </View>\";  \n");

                // handle QueryOptions
                if (mainobject.QueryOptions != null)
                {
                    if (mainobject.QueryOptions.UtcDate)
                        sb.Append("   camlQuery.DatesInUtc = true; \n");

                    if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                        sb.Append("   camlQuery.FolderServerRelativeUrl = \"" + mainobject.QueryOptions.SubFolder + "\"; \n");
                }
            }
            sb.Append("\n");

            sb.Append("   ListItemCollection listItems = spList.GetItems(camlQuery); \n");
            sb.Append("   clientContext.Load(listItems); \n");
            sb.Append("   clientContext.ExecuteQuery(); \n");
            sb.Append("}\n");

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private static string FormatCamlStringInVbNetForJavaScript(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, MainObject mainobject)
        {
            StringBuilder sb = null;

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }
        #endregion

        #region Static methods for code snippet generation for CSOM Rest with Json

        private static string FormatCamlStringInCSharpForRest(string listName,
            XmlNode viewfieldsNode,
            XmlNode whereNode,
            XmlNode orderByNode,
            MainObject mainobject,
            CamlDesigner.Common.Enumerations.SnippetType snippetType)
        {

            if (whereNode == null && orderByNode == null && viewfieldsNode == null && mainobject.QueryOptions == null) return null;

            StringBuilder sb = new StringBuilder("$.ajax({ \n");

            // Document Libraries can be handled differently
            // in case of taxonomy fields or complex queries, we have to pass the caml query:
            //   http://site/_api/Web/lists/getByTitle(‘TestFilter’)/GetItems(query=@v1)?
            //       @v1={“ViewXml”:”<View><Query><Where><Eq><FieldRef%20Name=’FilterDemo’/><Value%20Type=’TaxonomyFieldType’>A1</Value></Eq></Where></Query></View>”}
            bool needsQuery = CheckIfQueryMustBePassed(mainobject.WhereFieldList);

            if (mainobject.QueryOptions == null || !mainobject.QueryOptions.WorkWithFilesAndFolders || mainobject.QueryOptions.QueryFilesAndFoldersAllFoldersDeep)
            {
                FormatCamlStringForListItemsInCSharpForRestWithJson(listName, viewfieldsNode, whereNode, orderByNode, mainobject, ref sb, ref needsQuery);
            }
            else
            {
                FormatCamlStringForDocumentsInCSharpForRestWithJson(listName, mainobject.ViewFieldList, mainobject.WhereFieldList, orderByNode, mainobject, ref sb);
            }

            if (sb.ToString().Length > 0)
            {
                if (needsQuery)
                    sb.Append("   type: \"POST\", \n");
                else
                    sb.Append("   type: \"GET\", \n");

                if (snippetType == Common.Enumerations.SnippetType.ClientObjectModelForRestWithJson)
                {
                    if (!needsQuery)
                        sb.Append("   headers: {\"accept\": \"application/json;odata=verbose\"}, \n");
                    else
                    {
                        //headers: {
                        //    "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                        //    "Accept": "application/json; odata=verbose",
                        //    "Content-Type": "application/json; odata=verbose"
                        //},
                        sb.Append("   headers: { \n");
                        sb.Append("         \"X-RequestDigest\": $(\"#__REQUESTDIGEST\").val(), \n");
                        sb.Append("         \"Accept\": \"application/json;odata=verbose\", \n");
                        sb.Append("         \"Content-Type\": \"application/json; odata=verbose\" \n");
                        sb.Append("   }, \n");
                    }
                }
                else
                {
                    sb.Append("   contentType: \"application/atom+xml;type=entry\", \n");
                    sb.Append("   headers: {\"accept\": \"application/atom+xml\"}, \n");
                }

                sb.Append("   success: function (data) { \n");
                sb.Append("      if (data.d.results) { \n");
                sb.Append("         // TODO: handle the data  \n");
                sb.Append("         alert('handle the data'); \n");
                sb.Append("      } \n");
                sb.Append("   }, \n");
                sb.Append("   error: function (xhr) { \n");
                sb.Append("      alert(xhr.status + ': ' + xhr.statusText); \n");
                sb.Append("   } \n");
                sb.Append("}); \n");

                sb.Append("");
            }

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private static void FormatCamlStringForListItemsInCSharpForRestWithJson(string listName,
            XmlNode viewfieldsNode,
            XmlNode whereNode,
            XmlNode orderByNode,
            MainObject mainobject,
            ref StringBuilder sb,
            ref bool needsQuery)
        {
            Dictionary<string, string> restSnippets = new Dictionary<string, string>();
            restSnippets.Add("select", null);
            restSnippets.Add("orderby", null);
            restSnippets.Add("filter", null);
            restSnippets.Add("expand", null);
            restSnippets.Add("queryoptions", null);
            restSnippets.Add("query", null);


            needsQuery = FormatRestSnippetInCSharpForWhereFields(mainobject.WhereFieldList, whereNode, ref restSnippets);

            if (!needsQuery)
            {
                // by default:
                // url: _spPageContextInfo.webAbsoluteUrl +"/_api/web/lists/getbytitle('Developers')/Items?$select=Title,FirstName,JobTitle",
                sb.Append(string.Format("   url: _spPageContextInfo.webAbsoluteUrl + \"/_api/web/lists/getbytitle('{0}')/Items?", listName));

                FormatRestSnippetInCSharpForViewfields(mainobject.ViewFieldList, ref restSnippets);
                FormatRestSnippetInCSharpForOrderByFields(orderByNode, ref restSnippets);
                FormatRestSnippetInCSharpForQueryOptions(mainobject, ref restSnippets);

                bool hasCriteria = false;
                if (!string.IsNullOrEmpty(restSnippets["select"]))
                {
                    sb.Append(string.Format("{0}", restSnippets["select"]));
                    hasCriteria = true;
                }
                if (!string.IsNullOrEmpty(restSnippets["orderby"]))
                {
                    if (hasCriteria)
                        sb.Append("&");
                    else
                        hasCriteria = true;
                    sb.Append(string.Format("{0}", restSnippets["orderby"]));
                }
                if (!string.IsNullOrEmpty(restSnippets["expand"]))
                {
                    if (hasCriteria)
                        sb.Append("&");
                    else
                        hasCriteria = true;
                    sb.Append(string.Format("{0}", restSnippets["expand"]));
                }
                if (!string.IsNullOrEmpty(restSnippets["filter"]))
                {
                    if (hasCriteria)
                        sb.Append("&");
                    else
                        hasCriteria = true;
                    sb.Append(string.Format("{0}", restSnippets["filter"]));
                }
                if (!string.IsNullOrEmpty(restSnippets["queryoptions"]))
                {
                    if (hasCriteria)
                        sb.Append("&");
                    else
                        hasCriteria = true;
                    sb.Append(string.Format("{0}", restSnippets["queryoptions"]));
                }
            }
            else
            {
                /*
                 * sample code:
                    var caml = "<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>Bosch</Value></Eq></Where></Query></View>";
                    var endpoint = "http://sp2013-as1/_api/web/lists/GetByTitle('Developers')/GetItems(query=@v1)?@v1={\"ViewXml\":\"" + caml + "\"}";
                 */

                string query = null;
                if (whereNode != null && !string.IsNullOrEmpty(whereNode.OuterXml))
                    query = query + whereNode.OuterXml;
                if (orderByNode != null && !string.IsNullOrEmpty(orderByNode.OuterXml))
                    query = query + orderByNode.OuterXml;

                if (!string.IsNullOrEmpty(query))
                {
                    // replace double quotes by single quotes
                    query = "<Query>" + query + "</Query>";

                    if (viewfieldsNode != null && !string.IsNullOrEmpty(viewfieldsNode.OuterXml))
                        query = viewfieldsNode.OuterXml + query;

                    query = query.Replace("\"", "'");

                    sb.Append(string.Format("   url: _spPageContextInfo.webAbsoluteUrl + \"/_api/web/lists/getbytitle('{0}')/GetItems", listName));
                    sb.Append("(query=@v1)?@v1={\\\"ViewXml\\\":\\\"<View>" + query + "</View>\\\"}");

                }
            }

            sb.Append("\", \n");
        }

        //TODO: are there other cases where CAML query must be passed for REST request? f.e. more than 2 filters?
        private static bool CheckIfQueryMustBePassed(SortedList<int, CamlDesigner.SharePoint.Objects.WhereField> whereFieldsList)
        {
            bool needsQuery = false;

            if (whereFieldsList != null && whereFieldsList.Count > 0 && whereFieldsList.Count < 3)
            {
                // check if the whereFieldsList contains a taxonomy field
                for (int f = 0; f < whereFieldsList.Count; f++)
                {
                    WhereField field = whereFieldsList[f];

                    if (field.Field.DataType == "TaxonomyFieldType" || field.Field.DataType == "TaxonomyFieldTypeMulti")
                    {
                        needsQuery = true;
                        break;
                    }
                    else if ((field.Field.DataType == "User" || field.Field.DataType == "UserMulti")
                        && field.Values != null
                        && (field.Values[0].ToString() == "CurrentUserGroups"
                        || field.Values[0].ToString() == "SPWeb.Groups"
                        || field.Values[0].ToString() == "SPWeb.AllUsers"
                        || field.Values[0].ToString() == "SPWeb.Users"
                        || field.Values[0].ToString().StartsWith("SPGroup")))
                    {
                        needsQuery = true;
                        break;
                    }
                }
            }

            return needsQuery;
        }

        private static void FormatCamlStringForDocumentsInCSharpForRestWithJson(string listName,
            SortedList<int, CamlDesigner.SharePoint.Objects.ViewField> viewFieldsList,
            SortedList<int, CamlDesigner.SharePoint.Objects.WhereField> whereFieldsList,
            XmlNode orderByNode,
            MainObject mainobject,
            ref StringBuilder sb)
        {
            // Documentation:
            // --------------
            // Following options do not work with Rest
            // QueryFoldersAllFoldersDeep and QueryFilesAllFoldersDeep
            // For the option QueryFilesAndFoldersAllFoldersDeep a normal query can be executed
            if (mainobject.QueryOptions.QueryFilesAndFoldersInRootFolder)
            {
                sb.Append(string.Format("   url: _spPageContextInfo.webAbsoluteUrl + \"/_api/web/lists/getbytitle('{0}')/RootFolder", listName));
            }
            else if (mainobject.QueryOptions.QueryFilesInRootFolder)
            {
                sb.Append(string.Format("   url: _spPageContextInfo.webAbsoluteUrl + \"/_api/web/lists/getbytitle('{0}')/RootFolder/Files", listName));
            }
            else if (mainobject.QueryOptions.QueryFoldersInRootFolder)
            {
                sb.Append(string.Format("   url: _spPageContextInfo.webAbsoluteUrl + \"/_api/web/lists/getbytitle('{0}')/RootFolder/Folders", listName));
            }
            else if (mainobject.QueryOptions.QueryFilesAndFoldersInSubFolder && !string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
            {
                sb.Append(string.Format("   url: _spPageContextInfo.webAbsoluteUrl + \"/_api/web/GetFolderByServerRelativeUrl('{0}')", mainobject.QueryOptions.SubFolder));
            }
            else if (mainobject.QueryOptions.QueryFilesInSubFolder && !string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
            {
                sb.Append(string.Format("   url: _spPageContextInfo.webAbsoluteUrl + \"/_api/web/GetFolderByServerRelativeUrl('{0}')/Files", mainobject.QueryOptions.SubFolder));
            }
            else if (mainobject.QueryOptions.QueryFoldersInSubFolder && !string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
            {
                sb.Append(string.Format("   url: _spPageContextInfo.webAbsoluteUrl + \"/_api/web/GetFolderByServerRelativeUrl('{0}')/Folders", mainobject.QueryOptions.SubFolder));
            }

            // end the line
            sb.Append("\", \n");
        }

        private static void FormatRestSnippetInCSharpForViewfields(SortedList<int, CamlDesigner.SharePoint.Objects.ViewField> viewFieldsList, ref Dictionary<string, string> restSnippets)
        {
            // simple example:
            // "?$select=Title,FirstName,WorkPhone"
            // example with lookup field:
            // $select=Title,Country/Title &$expand=Country/Id &$filter=Country/Id eq 1

            if (viewFieldsList != null)
            {
                StringBuilder viewfieldsStringBuilder = new StringBuilder();
                StringBuilder expandStringBuilder = new StringBuilder();

                int i = 0;
                foreach (KeyValuePair<int, ViewField> pair in viewFieldsList)
                {
                    ViewField viewfield = pair.Value as ViewField;
                    if (viewfield != null)
                    {
                        if (i > 0)
                            viewfieldsStringBuilder.Append(",");
                        if (!string.IsNullOrEmpty(viewfield.Field.InternalName))
                        {
                            if (viewfield.Field.DataType == "Lookup")
                            {
                                // example with lookup field:
                                // $select=Title,Country/Title &$expand=Country/Id &$filter=Country/Id eq 1
                                // example: ?$select=Title,Employer/Id &$expand=Employer/Id &$filter=Employer/Id eq 1
                                LookupField lookupField = viewfield.Field as LookupField;
                                if (lookupField != null)
                                {
                                    // handle the viewfield part
                                    viewfieldsStringBuilder.Append(lookupField.InternalName + "/" + lookupField.ShowField);
                                    // handle the expand part
                                    string lookupstring = lookupField.InternalName + "/" + lookupField.ShowField;
                                    if (string.IsNullOrEmpty(restSnippets["expand"]) || !restSnippets["expand"].Contains(lookupstring))
                                    {
                                        if (string.IsNullOrEmpty(restSnippets["expand"]))
                                            restSnippets["expand"] = "$expand=";
                                        else
                                            restSnippets["expand"] += ", ";
                                        restSnippets["expand"] = restSnippets["expand"] + lookupstring;
                                    }
                                }
                            }
                            else
                                viewfieldsStringBuilder.Append(viewfield.Field.InternalName);
                        }
                    }

                    // TODO: handle nullable attribute
                    i++;

                }

                if (i > 0)
                    restSnippets["select"] = string.Format("$select={0}", viewfieldsStringBuilder.ToString());
            }
        }

        private static bool FormatRestSnippetInCSharpForWhereFields(SortedList<int, CamlDesigner.SharePoint.Objects.WhereField> whereFieldsList,
            XmlNode whereNode, ref Dictionary<string, string> restSnippets)
        {
            // return false, otherwise query is added to REST request
            if (whereFieldsList == null) return false;

            bool isBuilt = false;
            bool needsQuery = CheckIfQueryMustBePassed(whereFieldsList);

            if (!needsQuery)
            {
                // $filter=Name eq 'test'

                for (int f = 0; f < whereFieldsList.Count; f++)
                {
                    WhereField field = whereFieldsList[f];
                    if (f > 0)
                    {
                        if (!string.IsNullOrEmpty(field.AndOrOperator))
                            restSnippets["filter"] = restSnippets["filter"] + " " + field.AndOrOperator.ToLower() + " ";
                        else
                            restSnippets["filter"] = restSnippets["filter"] + " and ";
                    }

                    GetFilterClauseForRestJson(field, ref restSnippets);
                }

                if (!string.IsNullOrEmpty(restSnippets["filter"]))
                {
                    restSnippets["filter"] = "$filter=" + restSnippets["filter"];
                    isBuilt = true;
                }
            }

            return !isBuilt;
        }

        private static void FormatRestSnippetInCSharpForOrderByFields(XmlNode orderByNode, ref Dictionary<string, string> restSnippets)
        {
            if (orderByNode != null)
            {
                // "&$orderby=ID desc"
                StringBuilder osb = new StringBuilder();
                int i = 0;
                foreach (XmlNode node in orderByNode.ChildNodes)
                {
                    if (i > 0)
                        osb.Append(",");
                    if (node.Attributes["Name"] != null)
                        osb.Append(node.Attributes["Name"].Value);
                    if (node.Attributes["Ascending"] != null && node.Attributes["Ascending"].Value == "FALSE")
                        osb.Append(" desc");

                    i++;
                }
                if (i > 0)
                {
                    restSnippets["orderby"] = string.Format("$orderby={0}", osb.ToString());
                }
            }
        }

        private static void FormatRestSnippetInCSharpForQueryOptions(MainObject mainobject, ref Dictionary<string, string> restSnippets)
        {
            // TODO: queryoptions
            if (mainobject.QueryOptions != null)
            {
                StringBuilder qsb = new StringBuilder();

                if (mainobject.QueryOptions.RowLimit > 0)
                {
                    if (!string.IsNullOrEmpty(restSnippets["queryoptions"]))
                        restSnippets["queryoptions"] += "&";
                    restSnippets["queryoptions"] += string.Format("$top={0}", mainobject.QueryOptions.RowLimit.ToString());
                }
            }
        }


        private static string FormatCamlStringInVbNetForRest(string listName,
            XmlNode viewfieldsNode,
            XmlNode whereNode,
            XmlNode orderByNode,
            MainObject mainobject,
            CamlDesigner.Common.Enumerations.SnippetType snippetType)
        {
            StringBuilder sb = new StringBuilder("$.ajax({ \n");

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private static void GetFilterClauseForRestJson(WhereField whereField, ref Dictionary<string, string> restSnippets)
        {
            string filter = null;

            switch (whereField.WhereOperator)
            {
                //TODO: datetime values
                case "BeginsWith":
                    filter = "startswith(" + whereField.Field.InternalName + ",'" + whereField.Values[0] + "')";
                    break;
                case "Contains":
                    filter = "substringof('" + whereField.Values[0] + "'," + whereField.Field.InternalName + ")";
                    break;
                default:
                    // TODO: multiple values
                    switch (whereField.Field.DataType)
                    {
                        case "Lookup":
                            // example: ?$select=Title,Employer/Id &$expand=Employer/Id &$filter=Employer/Id eq 1
                            LookupField lookupField = whereField.Field as LookupField;
                            if (lookupField != null)
                            {
                                string lookupstring = lookupField.InternalName + "/" + lookupField.ShowField;
                                // handle the expand part
                                if (string.IsNullOrEmpty(restSnippets["expand"]) || !restSnippets["expand"].Contains(lookupstring))
                                {
                                    if (string.IsNullOrEmpty(restSnippets["expand"]))
                                        restSnippets["expand"] = "$expand=";
                                    else
                                        restSnippets["expand"] += ", ";
                                    restSnippets["expand"] = restSnippets["expand"] + lookupstring;
                                }
                                // handle the filter part
                                filter = lookupField.InternalName + "/" + lookupField.ShowField + " " + whereField.WhereOperator.ToLower() + " " + GetFilterValueForRestJson(whereField);
                            }
                            break;
                        default:
                            filter = whereField.Field.InternalName + " " + whereField.WhereOperator.ToLower() + " " + GetFilterValueForRestJson(whereField);
                            break;
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(filter))
                restSnippets["filter"] += filter;
        }

        private static string GetFilterValueForRestJson(WhereField whereField)
        {
            string filtervalue = null;

            if (whereField.Values != null)
            {
                switch (whereField.Field.DataType)
                {
                    //TODO: 
                    // - multi values
                    // - users
                    // - datetime values with time
                    case "Boolean":
                        if (whereField.Values[0].ToString().ToLower() == "true")
                            filtervalue = "1";
                        else
                            filtervalue = "0";
                        break;
                    case "DateTime":
                        // example: 2012-10-30T12:00:00Z into 10/30/2010
                        if (whereField.Values[0].ToString() == "Today")
                        {
                            if (!whereField.IncludeTimeValue)
                            {
                                DateTime datevalue = DateTime.Now;
                                filtervalue = string.Format("'{0}/{1}/{2}'", datevalue.Month.ToString(), datevalue.Day.ToString(), datevalue.Year.ToString());
                            }
                            else
                            {
                                DateTime datevalue = DateTime.Today;
                                filtervalue = string.Format("'{0}/{1}/{2} {3}:{4}:{5}'", datevalue.Month.ToString(), datevalue.Day.ToString(), datevalue.Year.ToString(),
                                    datevalue.Hour.ToString(), datevalue.Minute.ToString(), datevalue.Second.ToString());
                            }
                        }
                        else
                        {
                            DateTime datevalue = System.Convert.ToDateTime(whereField.Values[0].ToString());
                            if (!whereField.IncludeTimeValue)
                                filtervalue = string.Format("'{0}/{1}/{2}'", datevalue.Month.ToString(), datevalue.Day.ToString(), datevalue.Year.ToString());
                            else
                                filtervalue = string.Format("'{0}/{1}/{2} {3}'", datevalue.Month.ToString(), datevalue.Day.ToString(), datevalue.Year.ToString(), whereField.TimeValue);
                        }

                        //filtervalue = string.Format("'{0}/{1}/{2}'", whereField.Values[0].ToString().Substring(6, 2), whereField.Values[0].ToString().Substring(9, 2), whereField.Values[0].ToString().Substring(0, 1));
                        break;
                    case "Lookup":
                        // example: ?$select=Title,Employer/Id &$expand=Employer/Id &$filter=Employer/Id eq 1
                        LookupField lookupField = whereField.Field as LookupField;
                        LookupValue lookupValue = whereField.Values[0] as LookupValue;

                        //if (whereField.ByLookupId)
                        //    filtervalue = string.Format("'{0}'", lookupValue.ID.ToString());
                        //else
                        filtervalue = string.Format("'{0}'", lookupValue.Value);
                        break;
                    default:
                        // TODO: multiple values
                        filtervalue = string.Format("'{0}'", whereField.Values[0]);
                        break;
                }
            }

            return filtervalue;
        }

        #endregion

        #region Static methods for code snippet generation for CSOM Silverlight

        private static string FormatCamlStringInCSharpForSilverlight(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, MainObject mainobject)
        {
            StringBuilder sb = null;

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private static string FormatCamlStringInVbNetForSilverlight(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, MainObject mainobject)
        {
            StringBuilder sb = null;

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }
        #endregion


        #region helper methods

        private void InitializeClientContext()
        {
            try
            {
                if (string.IsNullOrEmpty(spUrl))
                {
                    throw new Exception("The SharePoint URL is empty.");
                }

                // Office 365 urls can have true domain names... checking for microsoft.com / sharepoint.com is not enough. 
                // TODO: Add UI Support to specify Office 365

                // check the URL
                isSharePointOnline = spUrl.Contains("microsoft.com") || spUrl.Contains("sharepoint.com");

                if (isSharePointOnline)
                {
                    EnsureSPOClientContext();
                }
                else
                {
                    EnsureSPClientContext();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SecureString ConvertToSecureString(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }

        private void EnsureSPOClientContext()
        {
            try
            {
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    SecureString securePwd = ConvertToSecureString(password);

                    clientContext = new ClientContext(spUrl);
                    clientContext.Credentials = new SharePointOnlineCredentials(username, securePwd);

                    // TODO: wat met two-factor authentication?
                    //var authManager = new OfficeDevPnP.Core.AuthenticationManager();
                    //// This method calls a pop up window with the login page and it also prompts  
                    //// for the multi factor authentication code.  
                    //ClientContext ctx = authManager.GetWebLoginClientContext(siteUrl); 
                    
                    spWeb = clientContext.Web;
                    clientContext.Load(spWeb);
                    clientContext.ExecuteQuery();

                    if (!string.IsNullOrEmpty(spEcthubUrl))
                    {
                        ecthubContext = new Microsoft.SharePoint.Client.ClientContext(spEcthubUrl);
                        clientContext.Credentials = new SharePointOnlineCredentials(username, securePwd);
                        ecthubSiteCollection = ecthubContext.Site;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Are we using a central exception handler??? //PeterK
            }
        }

        private void EnsureSPClientContext()
        {
            clientContext = new Microsoft.SharePoint.Client.ClientContext(spUrl);
            spWeb = clientContext.Web;
            clientContext.Load(spWeb);
            clientContext.ExecuteQuery();

            this.fieldsCollection = new List<CustomObjects.Field>();

            if (!string.IsNullOrEmpty(spEcthubUrl))
            {
                ecthubContext = new Microsoft.SharePoint.Client.ClientContext(spEcthubUrl);
                ecthubSiteCollection = ecthubContext.Site;
            }

        }

        private void RefreshWeb(string webUrl)
        {
            if (this.spUrl != webUrl)
            {
                EnsureSPOClientContext();
                spWeb = ClientContext.Web;
                ClientContext.Load(spWeb);
                clientContext.ExecuteQuery();
            }
        }
        #endregion
    }
}
