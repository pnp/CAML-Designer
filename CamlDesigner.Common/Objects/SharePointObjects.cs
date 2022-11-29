using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


namespace CamlDesigner.SharePoint.Objects
{
    public class Site
    {
        public string Url { get; set; }
    }

    public class Web
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string WebTemplate { get; set; }
        public bool HasUniquePermissions { get; set; }
        public List<List> ListCollection { get; set; }
        public List<Web> WebCollection { get; set; }
    }

    public class List
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TemplateID { get; set; }
        public string Url { get; set; }
        public string DefaultViewUrl { get; set; }
        public string DocumentTemplateUrl { get; set; }
        public string DefaultDisplayFormUrl { get; set; }
        public string DefaultNewFormUrl { get; set; }
        public string DefaultEditFormUrl { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int ItemCount { get; set; }
        public bool Hidden { get; set; }
        public bool HasUniquePermissions { get; set; }
        public string RootFolderUrl { get; set; }
        public string SendToLocationUrl { get; set; }
        public bool ContentTypesEnabled { get; set; }
        public bool EnableAttachments { get; set; }
        public bool EnableFolderCreation { get; set; }
        public bool EnableVersioning { get; set; }
        public bool EnableMinorVersion { get; set; }
        public int MajorVersionLimit { get; set; }
        public int MajorWithMinorVersionsLimit { get; set; }
        public bool EnableModeration { get; set; }
        public bool RequireCheckout { get; set; }
        public bool HasExternalDataSource { get; set; }
        public List<ContentType> ContentTypeCollection { get; set; }
        public List<Field> FieldCollection { get; set; }
        public List<View> ViewCollection { get; set; }
        public string WebUrl { get; set; }

        public Dictionary<string, string> DocumentSets { get; set; }
    }

    public class View
    {
        public Guid ID { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public bool IsDefaultView { get; set; }
        public bool IsPersonalView { get; set; }
        public string Editor { get; set; } // Editor
        public string ViewXML { get; set; }
        public string Query { get; set; }
        public string ViewFields { get; set; }
        public string Aggregations { get; set; }
        public int RowLimit { get; set; }
        public bool Paged { get; set; }
    }

    public class Folder
    {
        public string Name { get; set; }
        public string RelativeUrl { get; set; }
        public string Editor { get; set; }
        public DateTime Modified { get; set; }
        public bool HasUniquePermissions { get; set; }
    }

    public class ContentType
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public List<Field> FieldCollection { get; set; }

        public ContentType()
        {
        }

        public ContentType(string id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }

    public class Field

    {
        public Guid ID { get; set; }
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public string DataType { get; set; }
        public string AuthoringInfo { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }
        public bool ReadOnly { get; set; }

        public Field()
        {
        }

        public Field(Guid id, string displayName, string internalName, string dataType)
        {
            this.ID = id;
            this.InternalName = internalName;
            this.DisplayName = displayName;
            this.DataType = dataType;
        }

        public Field(Guid id, string displayName, string internalName, string dataType, bool required, bool hidden, string authoringInfo)
            :this(id, displayName, internalName, dataType)
        {
            this.Required = required;
            this.Hidden = hidden;
            this.AuthoringInfo = authoringInfo;
        }
    }

    public class DateTimeField : Field
    {
        public string DisplayFormat { get; set; }

        public DateTimeField(Guid id, string displayName, string internalName, string dataType, bool required, bool hidden, string authoringInfo)
            : base(id, displayName, internalName, dataType, required, hidden, authoringInfo)
        {
        }
    }

    public class ChoiceField : Field
    {
        public List<string> Choices { get; set; }
        public bool multiSelect { get; set; }

        public ChoiceField(Guid id, string displayName, string internalName, string dataType, bool required, bool hidden, string authoringInfo)
            : base(id, displayName, internalName, dataType, required, hidden, authoringInfo)
        {
        }
    }

    public class LookupField : Field
    {
        public string LookupListId { get; set; }
        public string LookupListName { get; set; }
        public string ShowField { get; set; }
        public Guid LookupWebId { get; set; }
        public bool MultiSelect { get; set; }

        public LookupField(Guid id, string displayName, string internalName, string dataType, bool required, bool hidden, string authoringInfo)
            : base(id, displayName, internalName, dataType, required, hidden, authoringInfo)
        {
        }
    }

    public class NoteField : Field
    {
        public int NumberOfLines { get; set; }
        public bool RichText { get; set; }
        public string RichTextMode { get; set; }

        public NoteField(Guid id, string displayName, string internalName, string dataType, bool required, bool hidden, string authoringInfo)
            : base(id, displayName, internalName, dataType, required, hidden, authoringInfo)
        {
        }
    }

    public class UserField : Field
    {
        public string UserSelectionMode { get; set; }
        public bool MultiSelect { get; set; }

        public UserField(Guid id, string displayName, string internalName, string dataType, bool required, bool hidden, string authoringInfo)
            : base(id, displayName, internalName, dataType, required, hidden, authoringInfo)
        {
        }
    }

    public class TaxonomyField : Field
    {
        public Guid TermStoreId { get; set; }
        public Guid TermSetId { get; set; }
        //public int WssId { get; set; }
        public bool MultiSelect { get; set; }

        public TaxonomyField(Guid id, string displayName, string internalName, string dataType, bool required, bool hidden, string authoringInfo, Guid sspId, Guid termSetId)
            : base(id, displayName, internalName, dataType, required, hidden, authoringInfo)
        {
            this.TermSetId = termSetId;
            this.TermStoreId = sspId;
        }
    }

    public class ViewField
    {
        public Field Field { get; set; }
        public bool IsNullable { get; set; }

        public ViewField(Field field)
        {
            this.Field = field;
            this.IsNullable = false;
        }

        public ViewField(Field field, bool isNullable)
        {
            this.Field = field;
            this.IsNullable = isNullable;
        }
    }

    public class OrderByField
    {
        public Field Field { get; set; }
        public CamlDesigner.Common.Enumerations.SortOrder SortOrder { get; set; }

        public OrderByField(Field field)
        {
            this.Field = field;
        }

        public OrderByField(Field field, CamlDesigner.Common.Enumerations.SortOrder sortOrder)
        {
            this.Field = field;
            this.SortOrder = sortOrder;
        }
    }

    public class GroupByField
    {
        public Field Field { get; set; }
        public CamlDesigner.Common.Enumerations.Collapse Collapse { get; set; }

        public GroupByField(Field field)
        {
            this.Field = field;
        }

        public GroupByField(Field field, CamlDesigner.Common.Enumerations.Collapse collapse)
        {
            this.Field = field;
            this.Collapse = collapse;
        }
    }

    public class WhereField
    {
        public Field Field { get; set; }
        public string WhereOperator { get; set; }
        public List<object> Values { get; set; }
        public string AndOrOperator { get; set; }
        public int PositionInList { get; set; }
        // special properties for specific data types
        public bool DateOnly { get; set; }
        public bool ByLookupId { get; set; }
        public bool IncludeTimeValue { get; set; }
        public string TimeValue { get; set; }
        public bool IncludeOffset { get; set; }
        public OffsetValue OffsetValue { get; set; }
        public CamlDesigner.Common.Enumerations.OptionalDateParameters OptionalDateParameter { get; set; }

        public WhereField(Field field)
        {
            this.Field = field;
            this.WhereOperator = "Eq";
            this.PositionInList = -1;
        }

        public WhereField(Field field, List<object> values)
        {
            this.Field = field;
            this.Values = values;
            this.WhereOperator = "Eq";
            this.PositionInList = -1;
        }

        public WhereField(Field field, string whereOperator, List<object> values)
        {
            this.Field = field;
            this.Values = values;
            this.WhereOperator = whereOperator;
            this.PositionInList = -1;
        }

        public WhereField(string andOrOperator, Field field, string whereOperator, List<object> values)
        {
            this.Field = field;
            this.Values = values;
            this.WhereOperator = whereOperator;
            if (!string.IsNullOrEmpty(andOrOperator))
                this.AndOrOperator = andOrOperator;
            this.PositionInList = -1;
        }

        public void AddValue(object value)
        {
            // TODO: should also test if all values are of the same data type
            if (value != null)
            {
                    if (this.Values == null)
                        this.Values = new List<object>();

                    this.Values.Add(value);
            }
        }
        public void AddListOfValues(System.Collections.Generic.List<object> values)
        {
            if (values != null)
            {
                this.Values = values;
            }
        }
    }

    public class GroupValue
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public GroupValue(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }

    public class LookupValue
    {
        public int ID { get; set; }
        public string Value { get; set; }

        public LookupValue(int id, string value)
        {
            this.ID = id;
            this.Value = value;
        }
    }

    public class TaxonomyValue
    {
        public Guid ID { get; set; }
        public string Value { get; set; }
        public List<TaxonomyValue> Terms { get; set; }
        public Guid ParentId { get; set; }
        public int[] WssIds { get; set; }

        public TaxonomyValue(Guid id, string value, Guid parentId, int[] wssIds)
        {
            this.ID = id;
            this.Value = value;
            this.ParentId = parentId;
            this.WssIds = wssIds;
            this.Terms = new List<TaxonomyValue>();
        }
    }

    public class OffsetValue
    {
        public string Sign { get; set; }
        public int Value { get; set; }
    }

    public class QueryOptions
    {
        // query options for CAMLQuery
        public bool IncludeMandatoryColumns { get; set; }
        public bool UtcDate { get; set; }
        public bool IncludeAttachmentUrls { get; set; }
        public bool IncludeAttachmentVersion { get; set; }
        public bool ExpandUserField { get; set; }
        public int RowLimit { get; set; }
        public bool ViewFieldsOnly { get; set; }
        public bool WorkWithFilesAndFolders { get; set; }
        public bool QueryFilesAndFoldersAllFoldersDeep { get; set; }
        public bool QueryFoldersAllFoldersDeep { get; set; }
        public bool QueryFilesAllFoldersDeep { get; set; }
        public bool QueryFilesAndFoldersInRootFolder { get; set; }
        public bool QueryFoldersInRootFolder { get; set; }
        public bool QueryFoldersInSubFolder { get; set; }
        public bool QueryFilesInRootFolder { get; set; }
        public bool QueryFilesAndFoldersInSubFolder { get; set; }
        public bool QueryFilesInSubFolder { get; set; }
        public bool QueryFilesInSubFolderDeep { get; set; }
        public bool QueryFilesAndFoldersInSubFolderDeep { get; set; }
        public string SubFolder { get; set; }        
        //string Paging {get;set;}

        // query options for SiteDataQuery
        public string ListTemplate { get; set; }
        public string WebScope { get; set; }
    }

    public class ListQueryOptions
    {
        public string DocumentSetUrl { get; set; }
    }
}
