using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CamlDesigner.Common
{
    public class Enumerations
    {
        public enum SortOrder
        {
            Ascending,
            Descending
        }

        public enum Collapse
        {
            True,
            False
        }

        public enum MigrationSteps
        {
            PrepareMigrationMappings,
            ExecuteMigrationAll,
            ExecuteMigrationDocLibs,
            ExecuteMigrationLists,
            ExecuteMigrationMeetingSites,
            ExecuteMigrationMeetingWorkspaces,
            ExecuteMigrationSecurity,
            CheckModificationsAfterMigration
        }

        public enum ApiConnectionType
        {
            ServerObjectModel,
            ClientObjectModel,
            WebServices
        }

        public enum SnippetType
        {
            CAML,
            ServerObjectModel,
            ClientObjectModelForDotnet,
            ClientObjectModelForJavaScript,
            ClientObjectModelForRestWithJson,
            ClientObjectModelForRestWithAtom,
            ClientObjectModelForSilverlight,
            Rest,
            WebServices,
            Powershell, 
            LINQ
        }

        public enum SharePointVersions
        {
            SP2007,
            SP2010,
            SP2013
        }

        public enum QueryType
        {
            CAMLQuery,
            SiteDataQuery
        }

        public enum QueryClauseType
        {
            ViewFields,
            OrderBy,
            Where,
            QueryOptions,
            GroupBy
        }

        public enum LanguageType
        {
            CSharp,
            VBNet
        }

        public enum DataTypes
        {
            Attachments,
            Boolean,
            Choice,
            Computed,
            Counter,
            DateTime,
            Lookup,
            MultiChoice,
            Number,
            Text,
            User,
            Note
        }

        public enum OptionalDateParameters
        {
            Today,
            Now,
            SpecificDate
        }

        public class Query
        {
            public const string Tag = "Query";

            public class Where
            {
                public const string Tag = "Where";

                public class FieldRef
                {
                    public const string Tag = "FieldRef";
                    public class Attributes
                    {
                        public const string Name = "Name";
                    }
                }
                public class ValueType
                {
                    public const string Tag = "Value";
                    public class Attributes
                    {
                        public const string Type = "Type";
                    }
                }
            }

            public class OrderBy
            {
                public const string Tag = "OrderBy";

                public class FieldRef
                {
                    public const string Tag = "FieldRef";
                    public class Attributes
                    {
                        public const string Name = "Name";
                        public const string Ascending = "Ascending";
                    }
                }
            }

        }

        public class ViewFields
        {
            public const string Tag = "ViewFields";

            public class FieldRef
            {
                public const string Tag = "FieldRef";
                public class Attributes
                {
                    public const string Name = "Name";
                }
            }
        }

        public class QueryOptions
        {
            public const string Tag = "QueryOptions";
            public const string IncludedMandatoryColumns = "IncludeMandatoryColumns";
            public const string DateInUtc = "DateInUtc";
            public const string Folder = "Folder";
            public const string Paging = "Paging";
            public const string MeetingInstanceID = "MeetingInstanceID";
            public const string ViewAttributes = "ViewAttributes";
            public const string Scope = "Scope";
        }

        public class SiteDataQueryScope
        {
            public string ScopeName { get; set; }
            public string ScopeValue { get; set; }

            public SiteDataQueryScope(string scopeName, string scopeValue)
            {
                this.ScopeName = scopeName;
                this.ScopeValue = scopeValue;
            }
        }

        public class Batch
        {
            public const string Tag = "Batch";
            public const string OnError = "OnError";
            public const string ListVersion = "ListVersion";
            public const string ViewName = "ViewName";

            public class Method
            {
                public const string Tag = "Method";
                public const string ID = "ID";

                public class Command
                {
                    public const string Tag = "Cmd";
                    public const string New = "New";
                    public const string Update = "Update";
                    public const string Delete = "Delete";
                }

                public class Field
                {
                    public const string Tag = "Field";
                    public const string Name = "Name";
                }
            }
        }

    }
}
