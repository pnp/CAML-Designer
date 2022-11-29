using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CamlDesigner.SharePoint.Objects;
using CamlDesigner.Common.Objects;
using CamlDesigner.Common;

namespace CamlDesigner.SharePoint.Common
{
    public class Builder
    {
        // private XmlDocument camlDocument = null;
        // private XmlNode rootNode = null;
        // private XmlNode queryNode = null;
        // private XmlNode orderByNode = null;
        // private XmlNode whereNode = null;
        // private XmlNode queryOptionsNode = null;
        // private XmlNode viewFieldsNode = null;
        // private XmlNode batchNode = null;

        #region Site Columns
        public static string GenerateCAMLSiteColumn(
            string name,
            string displayname,
            string datatype,
            string description,
            string group,
            bool? required,
            bool? allowDeletion,
            bool? showInFileDlg,
            bool? showInDisplayForm,
            bool? showInNewForm,
            bool? showInEditForm,
            bool? showInViewsForm,
            bool? showInListSettings,
            bool? showInVersionHistory)
        {
            if (string.IsNullOrEmpty(displayname))
                throw new ArgumentNullException("Please fill out a display name.");

            if (string.IsNullOrEmpty(datatype))
                throw new ArgumentNullException("Please fill out a data type.");

            // correct eventual errors in the name
            if (name.Contains(" "))
                name = name.Replace(" ", "_x0020_");
            else if (string.IsNullOrEmpty(name))
                name = displayname.Replace(" ", "_x0020_");

            // correct an eventual empty group
            if (string.IsNullOrEmpty(group))
                group = "Custom";

            string caml = string.Format(
                "<Field ID='{0}' Name='{1}' DisplayName='{2}' DataType='{3}' "
                + "Sealed='TRUE' Group='{4}' (param) "
                + "SourceID='http://schemas.microsoft.com/sharepoint/v3' "
                + "xmlns='http://schemas.microsoft.com/sharepoint/soap/' />",
                Guid.NewGuid().ToString(),
                name,
                displayname,
                datatype,
                group);

            if (!string.IsNullOrEmpty(description))
                caml = caml.Replace("(param)", string.Format(" Description='{0}' (param)", description));

            caml = ReplaceParamInString(caml, required, "Required");
            caml = ReplaceParamInString(caml, allowDeletion, "AllowDeletion");
            caml = ReplaceParamInString(caml, showInFileDlg, "ShowInFileDlg");
            caml = ReplaceParamInString(caml, showInDisplayForm, "ShowInDisplayForm");
            caml = ReplaceParamInString(caml, showInNewForm, "ShowInNewForm");
            caml = ReplaceParamInString(caml, showInEditForm, "ShowInEditForm");
            caml = ReplaceParamInString(caml, showInViewsForm, "ShowInViewsForm");
            caml = ReplaceParamInString(caml, showInListSettings, "ShowInListSettings");
            caml = ReplaceParamInString(caml, showInVersionHistory, "ShowInVersionHistory");

            return caml;
        }

        public static string GenerateTextSiteColumn(
            string name,
            string displayname,
            string datatype,
            string description,
            string group,
            bool? required,
            bool? allowDeletion,
            bool? showInFileDlg,
            bool? showInDisplayForm,
            bool? showInNewForm,
            bool? showInEditForm,
            bool? showInViewsForm,
            bool? showInListSettings,
            bool? showInVersionHistory,
            int maximumNumberOfCharacters,
            bool? enforceUniqueValues)
        {
            string caml = GenerateCAMLSiteColumn(
                name,
                displayname,
                datatype,
                description,
                group,
                required,
                allowDeletion,
                showInFileDlg,
                showInDisplayForm,
                showInNewForm,
                showInEditForm,
                showInViewsForm,
                showInListSettings,
                showInVersionHistory);

            caml = ReplaceParamInString(caml, enforceUniqueValues, "EnforceUniqueValues");

            if (maximumNumberOfCharacters > 0)
                caml = caml.Replace("(param)", string.Format("MaxLength='{0}' (param)", maximumNumberOfCharacters.ToString()));

            caml = caml.Replace("(param)", "");
            return caml;
        }

        #endregion

        #region ViewFields
        public static void GenerateViewFields(MainObject mainobject)
        {
            string caml = null;

            if (mainobject.ViewFieldList != null && mainobject.ViewFieldList.Count > 0)
            {
                for (int i = 0; i < mainobject.ViewFieldList.Count; i++)
                {
                    ViewField viewfield = mainobject.ViewFieldList[i];
                    if (viewfield.IsNullable)
                        caml += string.Format("<FieldRef Name='{0}' Nullable='{1}' />", viewfield.Field.InternalName, "TRUE");
                    else
                        caml += string.Format("<FieldRef Name='{0}' />", viewfield.Field.InternalName);
                }
            }

            // check if there is already a ViewFields part
            FormatCamlString(ref mainobject, caml, "ViewFields");
        }

        #endregion

        #region GroupBy
        public static void GenerateGroupByFields(MainObject mainobject)
        {
            StringBuilder caml = new StringBuilder();
            string collapsed = string.Empty;
            
            if (mainobject.GroupByFieldList != null && mainobject.GroupByFieldList.Count > 0)
            {
                for (int i = 0; i < mainobject.GroupByFieldList.Count; i++)
                {
                    GroupByField groupByField = mainobject.GroupByFieldList[i];
                    if (i == 0)
                    {
                        collapsed = groupByField.Collapse.ToString();
                    }

                    caml.Append(string.Format("<FieldRef Name='{0}' />", groupByField.Field.InternalName));
                }
            }

            string completeNode = string.Format("<GroupBy  Collapse ='{0}'>{1}</GroupBy>", collapsed,caml.ToString());

            // check if there is already a OrderBy part
            FormatCamlString(ref mainobject, completeNode, "GroupBy");
        }
        #endregion


        #region OrderBy
        public static void GenerateOrderByFields(MainObject mainobject)
        {
            string caml = null;

            if (mainobject.OrderByFieldList != null && mainobject.OrderByFieldList.Count > 0)
            {
                for (int i = 0; i < mainobject.OrderByFieldList.Count; i++)
                {
                    OrderByField orderByField = mainobject.OrderByFieldList[i];
                    if (orderByField.SortOrder == Enumerations.SortOrder.Ascending)
                        caml += string.Format("<FieldRef Name='{0}' />", orderByField.Field.InternalName);
                    else
                        caml += string.Format("<FieldRef Name='{0}' Ascending='{1}' />", orderByField.Field.InternalName, "FALSE");
                }
            }

            // check if there is already a OrderBy part
            FormatCamlString(ref mainobject, caml, "OrderBy");
        }
        #endregion

        #region Where
        public static void GenerateWhereFields(MainObject mainobject)
        {
            string caml = null;

            if (mainobject.WhereFieldList != null && mainobject.WhereFieldList.Count > 0)
            {
                for (int i = 0; i < mainobject.WhereFieldList.Count; i++)
                {
                    WhereField whereField = mainobject.WhereFieldList[i];
                    if (whereField == null)
                        continue;

                    string subcamlstring = null;

                    if (whereField.WhereOperator != null && (whereField.WhereOperator == "IsNull" || whereField.WhereOperator == "IsNotNull"))
                    {
                        // <IsNull><FieldRef Name='PublishingStartDate'/></IsNull>
                        subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /></{0}>", whereField.WhereOperator, whereField.Field.InternalName);
                    }
                    else if (whereField.WhereOperator != null && whereField.WhereOperator == "In")
                    {
                        subcamlstring = GetCamlFromMultipleValues(whereField);
                    }
                    else if (whereField.WhereOperator != null && whereField.Values != null && whereField.Values.Count == 1)
                    {
                        // TODO: if count > 0, a IN operator should be used
                        subcamlstring = GetCamlFromSingleValue(whereField);
                    }

                    if (i > 0 && !string.IsNullOrEmpty(caml) && !string.IsNullOrEmpty(subcamlstring))
                    {
                        if (string.IsNullOrEmpty(whereField.AndOrOperator))
                            whereField.AndOrOperator = "And";
                        caml = string.Format("<{0}>{1}{2}</{0}>", whereField.AndOrOperator, caml, subcamlstring);
                    }
                    else if (i == 0 && !string.IsNullOrEmpty(subcamlstring))
                    {
                        caml = subcamlstring;
                    }
                }
            }

            // Remark: don't test on queryOptions.QueryFilesInFolder because this options sets the ViewAttribute to Scope='FilesOnly' (to be tested)
            if (mainobject.QueryOptions != null && (mainobject.QueryOptions.QueryFilesAllFoldersDeep || mainobject.QueryOptions.QueryFoldersAllFoldersDeep
                || mainobject.QueryOptions.QueryFoldersInRootFolder || mainobject.QueryOptions.QueryFilesInRootFolder || mainobject.QueryOptions.QueryFoldersInSubFolder))
            {
                // check the QueryOptions and eventually add an extra condition
                string fvalue = "0";
                if (mainobject.QueryOptions.QueryFoldersAllFoldersDeep || mainobject.QueryOptions.QueryFoldersInRootFolder || mainobject.QueryOptions.QueryFoldersInSubFolder)
                    fvalue = "1";

                if (!string.IsNullOrEmpty(caml))
                {
                    caml = string.Format("<And>{0}<Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>{1}</Value></Eq></And>", caml, fvalue);
                }
                else
                {
                    caml = string.Format("<Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>{0}</Value></Eq>", fvalue);
                }
            }

            // check if there is already a Where part
            FormatCamlString(ref mainobject, caml, "Where");
        }

        private static string GetCamlFromSingleValue(WhereField whereField)
        {
            // there was already a test on the existance of a value.
            string subcamlstring = null;

            switch (whereField.Field.DataType)
            {
                case "Boolean":
                    if (System.Convert.ToBoolean(whereField.Values[0]))
                        subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'>{3}</Value></{0}>",
                                whereField.WhereOperator, whereField.Field.InternalName, whereField.Field.DataType, "1");
                    else
                        subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'>{3}</Value></{0}>",
                                whereField.WhereOperator, whereField.Field.InternalName, whereField.Field.DataType, "0");
                    break;
                case "Lookup":
                case "LookupMulti":
                    // <FieldRef Name="Country" /><Value Type="Lookup">India</Value>
                    // <FieldRef Name="Country" LookupId="True" /><Value Type="Lookup">15</Value>
                    if (whereField.Values[0] != null && whereField.Values[0] is LookupValue)
                    {
                        if (whereField.ByLookupId)
                        {
                            subcamlstring = string.Format(
                                "<{0}><FieldRef Name='{1}' LookupId='True' /><Value Type='{2}'>{3}</Value></{0}>",
                                    whereField.WhereOperator,
                                    whereField.Field.InternalName,
                                    whereField.Field.DataType,
                                    ((LookupValue)whereField.Values[0]).ID);
                        }
                        else
                        {
                            subcamlstring = string.Format(
                                "<{0}><FieldRef Name='{1}' /><Value Type='{2}'>{3}</Value></{0}>",
                                    whereField.WhereOperator,
                                    whereField.Field.InternalName,
                                    whereField.Field.DataType,
                                    ((LookupValue)whereField.Values[0]).Value);
                        }
                    }
                    break;
                case "TaxonomyFieldType":
                case "TaxonomyFieldTypeMulti":
                    // <FieldRef Name='Technology' /><Value Type='TaxonomyFieldType'>SharePoint 2010</Value>
                    // <FieldRef Name='Technology' LookupId='True' /><Value Type='Lookup'>15</Value>
                    if (whereField.Values[0] != null)
                    {
                        string value = null;
                        if (whereField.Values[0] is string)
                        {
                            value = whereField.Values[0].ToString();
                        }
                        else if (whereField.Values[0] is CamlDesigner.SharePoint.Objects.TaxonomyValue)
                        {
                            if (whereField.ByLookupId)
                                value = ((CamlDesigner.SharePoint.Objects.TaxonomyValue)whereField.Values[0]).ID.ToString();
                            else
                                value = ((CamlDesigner.SharePoint.Objects.TaxonomyValue)whereField.Values[0]).Value;
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            if (whereField.ByLookupId)
                                subcamlstring = string.Format("<{0}><FieldRef Name='{1}' LookupId='True' /><Value Type='Integer'>{2}</Value></{0}>",
                                    whereField.WhereOperator,
                                    whereField.Field.InternalName,
                                    value);
                            else
                                subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'>{3}</Value></{0}>",
                                    whereField.WhereOperator,
                                    whereField.Field.InternalName,
                                    whereField.Field.DataType,
                                    value);
                        }
                    }
                    break;
                case "DateTime":
                    // TODO: check ivm utd date
                    if (whereField.OptionalDateParameter == CamlDesigner.Common.Enumerations.OptionalDateParameters.Today)
                    {
                        if (!whereField.IncludeOffset)
                        {
                            subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'><Today /></Value></{0}>",
                                    whereField.WhereOperator, whereField.Field.InternalName, whereField.Field.DataType);
                        }
                        else
                        {
                            if (whereField.OffsetValue != null)
                            {
                                if (string.IsNullOrEmpty(whereField.OffsetValue.Sign) || whereField.OffsetValue.Sign == "+")
                                    subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'><Today Offset='{3}'/></Value></{0}>",
                                            whereField.WhereOperator, whereField.Field.InternalName, whereField.Field.DataType, whereField.OffsetValue.Value.ToString());
                                else
                                    subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'><Today Offset='{3}{4}'/></Value></{0}>",
                                            whereField.WhereOperator, whereField.Field.InternalName, whereField.Field.DataType,
                                            whereField.OffsetValue.Sign, whereField.OffsetValue.Value.ToString());
                            }
                        }
                    }
                    else if (whereField.Values[0] is DateTime &&
                        whereField.OptionalDateParameter == CamlDesigner.Common.Enumerations.OptionalDateParameters.SpecificDate)
                    {
                        // a sharePoint datetime is from the format yyyy-MM-ddThh:mm:ssZ
                        string datestring = UtilityFunctionsCAML.CreateISO8601DateTimeFromSystemDateTime((DateTime)whereField.Values[0]);
                        if (!whereField.IncludeTimeValue)
                        {
                            subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'>{3}</Value></{0}>",
                                    whereField.WhereOperator, whereField.Field.InternalName, whereField.Field.DataType, datestring);
                        }
                        else
                        {
                            // TODO: validate the time value
                            if (!string.IsNullOrEmpty(whereField.TimeValue))
                            {
                                datestring = datestring.Substring(0, datestring.IndexOf('T') + 1);
                                datestring = datestring + whereField.TimeValue + "Z";
                            }

                            subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}' IncludeTimeValue='TRUE'>{3}</Value></{0}>",
                                    whereField.WhereOperator, whereField.Field.InternalName, whereField.Field.DataType, datestring);
                        }
                    }
                    break;
                case "User":
                case "UserMulti":
                    // <FieldRef Name=’AssignedTo’ LookupId=’TRUE’/><Value Type=’Integer’><UserID/></Value>
                    int userID = 0;
                    if (int.TryParse(whereField.Values[0].ToString(), out userID))
                    {
                        subcamlstring = string.Format("<{0}><FieldRef Name='{1}' LookupId='True' /><Value Type='Integer'>{2}</Value></{0}>",
                                whereField.WhereOperator, whereField.Field.InternalName, whereField.Values[0].ToString());
                    }
                    else if (whereField.Values[0].ToString() == "UserID")
                    {
                        subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='Integer'><UserID/></Value></{0}>",
                                whereField.WhereOperator, whereField.Field.InternalName);
                    }
                    else if (whereField.Values[0].ToString() == "CurrentUserGroups"
                        || whereField.Values[0].ToString() == "SPWeb.Groups"
                        || whereField.Values[0].ToString() == "SPWeb.AllUsers"
                        || whereField.Values[0].ToString() == "SPWeb.Users")
                    {
                        subcamlstring = string.Format("<Membership Type='{0}'><FieldRef Name='{1}' /></Membership>",
                              whereField.Values[0].ToString(), whereField.Field.InternalName);
                    }
                    else if (whereField.Values[0].ToString().StartsWith("SPGroup"))
                    {
                        string group = string.Empty;
                        if (whereField.Values[0].ToString().Length > 7)
                            group = whereField.Values[0].ToString().Substring(whereField.Values[0].ToString().IndexOf("SPGroup") + 7);
                        subcamlstring = string.Format("<Membership Type='SPGroup' ID='{0}'><FieldRef Name='{1}' /></Membership>",
                              group, whereField.Field.InternalName);
                    }
                    else
                    {
                        subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='User'>{2}</Value></{0}>",
                                whereField.WhereOperator, whereField.Field.InternalName, whereField.Values[0].ToString());
                    }
                    break;
                default:
                    subcamlstring = string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'>{3}</Value></{0}>",
                            whereField.WhereOperator, whereField.Field.InternalName, whereField.Field.DataType, whereField.Values[0].ToString());
                    break;
            }

            return subcamlstring;
        }

        private static string GetCamlFromMultipleValues(WhereField whereField)
        {
            // <In>  
            //   <FieldRef Name='Designation' />  
            //   <Values>  
            //      <Value Type='Text'>Engineer</Value>  
            //      <Value Type='Text'>Architect</Value>  
            //   </Values>  
            // </In>  
            string subcamlstring = null;

            if (whereField.Values != null)
            {
                switch (whereField.Field.DataType)
                {
                    case "MultiChoice":
                        //TODO: uitwerken
                        foreach (string item in whereField.Values)
                        {
                            subcamlstring += string.Format("<Value Type='{0}'>{1}</Value>", whereField.Field.DataType, item);
                        }
                        subcamlstring = string.Format("<FieldRef Name='{0}'/><Values>{1}</Values>", whereField.Field.InternalName, subcamlstring);
                        break;
                    case "Lookup":
                    case "LookupMulti":
                        // <FieldRef Name="Country" /><Value Type="Lookup">India</Value>
                        // <FieldRef Name="Country" LookupId="True" /><Value Type="Lookup">15</Value>
                        foreach (LookupValue value in whereField.Values)
                        {
                            if (whereField.ByLookupId)
                            {
                                subcamlstring += string.Format("<Value Type='{0}'>{1}</Value>", whereField.Field.DataType, value.ID);
                            }
                            else
                            {
                                subcamlstring += string.Format("<Value Type='{0}'>{1}</Value>", whereField.Field.DataType, value.Value);
                            }
                        }
                        if (whereField.ByLookupId)
                        {
                            subcamlstring = string.Format("<FieldRef Name='{0}' LookupId='True' /><Values>{1}</Values>", whereField.Field.InternalName, subcamlstring);
                        }
                        else
                        {
                            subcamlstring = string.Format("<FieldRef Name='{0}' /><Values>{1}</Values>", whereField.Field.InternalName, subcamlstring);
                        }
                        break;
                    case "TaxonomyFieldType":
                    case "TaxonomyFieldTypeMulti":
                        // <FieldRef Name='Technology' /><Value Type='TaxonomyFieldType'>SharePoint 2010</Value>
                        // <FieldRef Name='Technology' LookupId='True' /><Value Type='Lookup'>15</Value>
                        foreach (TaxonomyValue value in whereField.Values)
                        {
                            if (whereField.ByLookupId)
                            {
                                subcamlstring += string.Format("<Value Type='Lookup'>{0}</Value>", value.WssIds);
                            }
                            else
                            {
                                subcamlstring += string.Format("<Value Type='{0}'>{1}</Value>", whereField.Field.DataType, value.Value);
                            }
                        }
                        if (whereField.ByLookupId)
                        {
                            subcamlstring = string.Format("<FieldRef Name='{0}' LookupId='True' /><Values>{1}</Values>", whereField.Field.InternalName, subcamlstring);
                        }
                        else
                        {
                            subcamlstring = string.Format("<FieldRef Name='{0}' /><Values>{1}</Values>", whereField.Field.InternalName, subcamlstring);
                        }
                        break;
                    case "DateTime":
                        // TODO: check ivm utd date
                        if (whereField.OptionalDateParameter == CamlDesigner.Common.Enumerations.OptionalDateParameters.SpecificDate)
                        {
                            foreach (object value in whereField.Values)
                            {
                                if (value != null && value is DateTime)
                                {
                                    // a sharePoint datetime is from the format yyyy-MM-ddThh:mm:ssZ
                                    string datestring = UtilityFunctionsCAML.CreateISO8601DateTimeFromSystemDateTime((DateTime)value);

                                    if (!whereField.IncludeTimeValue)
                                    {
                                        subcamlstring += string.Format("<Value Type='{0}'>{1}</Value>", whereField.Field.DataType, datestring);
                                    }
                                    else
                                    {
                                        // TODO: validate the time value
                                        if (!string.IsNullOrEmpty(whereField.TimeValue))
                                        {
                                            datestring = datestring.Substring(0, datestring.IndexOf('T') + 1);
                                            datestring = datestring + whereField.TimeValue + "Z";
                                        }

                                        subcamlstring += string.Format("<Value Type='{0}' IncludeTimeValue='TRUE'>{1}</Value>", whereField.Field.DataType, datestring);
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(subcamlstring))
                            {
                                subcamlstring = string.Format("<FieldRef Name='{0}' /><Values>{1}</Values>", whereField.Field.InternalName, subcamlstring);
                            }
                        }
                        break;
                    case "User":
                        bool isLookup = false;
                        string userValue = null;
                        foreach (object value in whereField.Values)
                        {
                            foreach (char letter in whereField.Values[0].ToString())
                            {
                                if (letter.ToString() != "+")
                                    userValue += letter.ToString();
                                else
                                {
                                    HandleUserValue(userValue, ref subcamlstring, ref isLookup);
                                    userValue = null;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(userValue))
                        {
                            HandleUserValue(userValue, ref subcamlstring, ref isLookup);
                        }

                        // TODO: can membership have multiple FieldRefs
                        if (!string.IsNullOrEmpty(subcamlstring))
                        {
                            if (isLookup)
                                subcamlstring = string.Format("<FieldRef Name='{0}' LookupId='True' /><Values>{1}</Values>", whereField.Field.InternalName, subcamlstring);
                            else
                                subcamlstring = string.Format("<FieldRef Name='{0}' /><Values>{1}</Values>", whereField.Field.InternalName, subcamlstring);
                        }
                        break;
                    default:
                        // all strings are stored in the same value, separated by a + sign
                        string stringValue = string.Empty;
                        foreach (char letter in whereField.Values[0].ToString())
                        {
                            if (letter.ToString() != "+")
                                stringValue += letter.ToString();
                            else
                            {
                                subcamlstring += string.Format("<Value Type='{0}'>{1}</Value>", whereField.Field.DataType, stringValue);
                                stringValue = string.Empty;
                            }
                        }
                        if (!string.IsNullOrEmpty(stringValue))
                            subcamlstring += string.Format("<Value Type='{0}'>{1}</Value>", whereField.Field.DataType, stringValue);

                        if (!string.IsNullOrEmpty(subcamlstring))
                            subcamlstring = string.Format("<FieldRef Name='{0}' /><Values>{1}</Values>", whereField.Field.InternalName, subcamlstring);

                        break;
                }
            }

            if (!string.IsNullOrEmpty(subcamlstring))
            {
                subcamlstring = string.Format("<In>{0}</In>", subcamlstring);
            }

            return subcamlstring;
        }

        private static void HandleUserValue(string userValue, ref string subcamlstring, ref bool isLookup)
        {
            // handle the user value
            int userID = 0;
            if (userValue != null && int.TryParse(userValue, out userID))
            {
                isLookup = true;
                subcamlstring += string.Format("<Value Type='Integer'>{0}</Value>", userValue);
            }
            else
            {
                subcamlstring += string.Format("<Value Type='User'>{0}</Value>", userValue);
            }
        }

        #endregion

        #region QueryOptions
        public static void GenerateQueryOptions(MainObject mainobject)
        {
            string caml = null;

            if (mainobject.QueryOptions != null)
            {
                if (mainobject.QueryOptions.IncludeMandatoryColumns)
                    caml += "<IncludeMandatoryColumns>True</IncludeMandatoryColumns>";

                if (mainobject.QueryOptions.RowLimit > 0)
                    caml += string.Format("<RowLimit>{0}</RowLimit>", mainobject.QueryOptions.RowLimit.ToString());

                if (mainobject.QueryOptions.IncludeAttachmentUrls)
                    caml += "<IncludeAttachmentUrls>True</IncludeAttachmentUrls>";
                if (mainobject.QueryOptions.IncludeAttachmentVersion)
                    caml += "<IncludeAttachmentVersion>True</IncludeAttachmentVersion>";

                if (mainobject.QueryOptions.ExpandUserField)
                    caml += "<ExpandUserField>True</ExpandUserField>";

                if (mainobject.QueryOptions.UtcDate)
                    caml += "<DateInUtc>True</DateInUtc>";

                if (mainobject.QueryOptions.QueryFilesAndFoldersAllFoldersDeep
                    || mainobject.QueryOptions.QueryFoldersAllFoldersDeep
                    || mainobject.QueryOptions.QueryFilesAllFoldersDeep
                    || mainobject.QueryOptions.QueryFilesAndFoldersInSubFolder
                    || mainobject.QueryOptions.QueryFilesInSubFolder
                    || mainobject.QueryOptions.QueryFoldersInSubFolder
                    || mainobject.QueryOptions.QueryFilesInSubFolderDeep
                    || mainobject.QueryOptions.QueryFilesAndFoldersInSubFolderDeep
                    || !string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                {
                    if (mainobject.QueryOptions.QueryFilesAndFoldersAllFoldersDeep || mainobject.QueryOptions.QueryFoldersAllFoldersDeep || mainobject.QueryOptions.QueryFilesAllFoldersDeep)
                        caml += "<ViewAttributes Scope='RecursiveAll' />";
                    else if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                    {
                        if (mainobject.QueryOptions.QueryFilesInSubFolderDeep)
                            caml += "<ViewAttributes Scope='Recursive' />";
                        else if (mainobject.QueryOptions.QueryFilesAndFoldersInSubFolderDeep)
                            caml += "<ViewAttributes Scope='RecursiveAll' />";
                        else if (mainobject.QueryOptions.QueryFilesInSubFolder)
                            caml += "<ViewAttributes Scope='FilesOnly' />";
                    }

                    if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                    {
                        caml += string.Format("<Folder>{0}</Folder>", mainobject.QueryOptions.SubFolder);
                    }

                    // TODO: add the OptimizeFor element to optimize query retrieval
                    // caml += "<OptimizeFor>FolderUrls</OptimizeFor>";
                }

                if (!string.IsNullOrEmpty(mainobject.QueryOptions.ListTemplate))
                    caml += string.Format("<Lists ServerTemplate='{0}'/>", mainobject.QueryOptions.ListTemplate);

                if (!string.IsNullOrEmpty(mainobject.QueryOptions.WebScope))
                    caml += string.Format("<Webs Scope='{0}'/>", mainobject.QueryOptions.WebScope);
            }

            // check if there is already a OrderBy part
            FormatCamlString(ref mainobject, caml, "QueryOptions");
        }

        #endregion

        #region Format CAML string based on data access method
        private static string ReplaceParamInString(string caml, bool? boolvalue, string attributeName)
        {
            if (boolvalue != null)
            {
                if ((bool)boolvalue)
                    caml = caml.Replace("(param)", string.Format("{0}='TRUE' (param)", attributeName));
                else
                    caml = caml.Replace("(param)", string.Format("{0}='FALSE' (param)", attributeName));
            }

            return caml;
        }

        // this method is used to construct the pure CAML
        private static void FormatCamlString(ref MainObject mainobject, string caml, string nodeElement)
        {
            if (mainobject.CamlDocument == null)
            {
                mainobject.CamlDocument = new XmlDocument();
                XmlNode camlNode = mainobject.CamlDocument.CreateElement("CAML");
                mainobject.CamlDocument.AppendChild(camlNode);
            }

            // create node if node is not there and there is inner caml to add
            // remove node if node exists and inner caml is empty (can be the case with a changing Where clause because of files and folders
            XmlNode node = mainobject.CamlDocument.SelectSingleNode("//" + nodeElement);
            if (node == null && !string.IsNullOrEmpty(caml))
            {
                node = mainobject.CamlDocument.CreateElement(nodeElement);
                mainobject.CamlDocument.ChildNodes[0].AppendChild(node);
            }
            else if (node != null && string.IsNullOrEmpty(caml))
            {
                mainobject.CamlDocument.ChildNodes[0].RemoveChild(node);
            }

            // if there is caml, add it to the node
            if (node != null)
            {
                if (string.IsNullOrEmpty(caml))
                    node.InnerXml = string.Empty;
                else
                    node.InnerXml = caml;
            }
        }

        public static string IndentCAML(XmlDocument camlDocument)
        {
            string result = null;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            XmlTextWriter xw = new XmlTextWriter(ms, System.Text.Encoding.Unicode);
            System.IO.StreamReader sr = null;
            try
            {
                xw.Formatting = Formatting.Indented;
                xw.QuoteChar = '\'';
                xw.Indentation = 3;
                camlDocument.WriteContentTo(xw);
                xw.Flush();
                ms.Flush();
                ms.Position = 0;
                sr = new System.IO.StreamReader(ms);
                result = sr.ReadToEnd();

                // remove <CAML>/r/n and </CAML>/r/n 
                if (!string.IsNullOrEmpty(result) && result.Contains("<CAML>\r\n"))
                    result = result.Replace("<CAML>\r\n", string.Empty);
                if (!string.IsNullOrEmpty(result) && result.Contains("\r\n</CAML>"))
                    result = result.Replace("\r\n</CAML>", string.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                xw.Close();
                ms.Close();
                sr.Close();
            }
            return result;
        }

        public static string IndentCAML(string camlstring)
        {
            if (string.IsNullOrEmpty(camlstring)) return null;

            // build a new XML document for indentation purposes
            XmlDocument doc = new XmlDocument();
            XmlNode queryNode = doc.CreateElement("CAML");
            doc.AppendChild(queryNode);
            queryNode.InnerXml = camlstring;

            return IndentCAML(doc);
        }

        public static string BuildQuerystring(XmlNode whereNode, XmlNode orderByNode)
        {
            string querystring = null;
            if (whereNode != null)
                querystring = whereNode.OuterXml.Replace("\"", "'");

            if (orderByNode != null)
            {
                if (string.IsNullOrEmpty(querystring))
                    querystring = orderByNode.OuterXml.Replace("\"", "'");
                else
                    querystring += orderByNode.OuterXml.Replace("\"", "'");
            }

            return querystring;
        }

        #endregion

    }
}
