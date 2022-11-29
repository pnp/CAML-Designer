using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CamlDesigner.SharePoint.Objects;
using System.Xml;

namespace CamlDesigner2013.CamlEventHandler
{
    public delegate void CAMLEventHandler(CamlEventArgs args);

    public class CamlEventArgs
    {
        public CamlEventArgs(
            string listName,
            XmlDocument camlDocument,
            QueryOptions queryOptions,
             CamlDesigner.Common.Enumerations.ApiConnectionType connectionType,
             CamlDesigner.Common.Enumerations.SnippetType snippetType,
             CamlDesigner.Common.Enumerations.LanguageType languageType,
             CamlDesigner.Common.Enumerations.QueryType queryType)
        {
            this.ListName = listName;
            this.CamlDocument = camlDocument;
            this.QueryOptions = queryOptions;
            this.ConnectionType = connectionType;
            this.LanguageType = languageType;
            this.QueryType = queryType;
        }

        public XmlDocument CamlDocument
        {
            get;
            set;
        }

        public  CamlDesigner.Common.Enumerations.ApiConnectionType ConnectionType
        {
            get;
            set;
        }

        public  CamlDesigner.Common.Enumerations.SnippetType SnippetType
        {
            get;
            set;
        }

        public  CamlDesigner.Common.Enumerations.LanguageType LanguageType
        {
            get;
            set;
        }

        public  CamlDesigner.Common.Enumerations.QueryType QueryType
        {
            get;
            set;
        }

        public string ListName
        {
            get;
            set;
        }

        public QueryOptions QueryOptions
        {
            get;
            set;
        }
    }
}
