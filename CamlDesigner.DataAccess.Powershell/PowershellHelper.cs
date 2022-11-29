using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using CamlDesigner.Common.Objects;

namespace CamlDesigner.DataAccess
{
    public class PowershellHelper
    {
        #region Static methods for code snippet generation
        public static string FormatCamlString(string listName, CamlDesigner.Common.Enumerations.LanguageType languageType,
          MainObject mainobject, string sharepointURL)
        {
            return FormatCamlStringInCSharp(listName, mainobject, sharepointURL);
        }

        private static string FormatCamlStringInCSharp(string listName, MainObject mainobject, string sharepointURL)
        {
            StringBuilder sb = new StringBuilder(string.Format("$spweb = get-spweb {0} \n $splist = $spweb.Lists.TryGetList(\"{1}\") \n", sharepointURL, listName));
            sb.Append("if ($splist) \n");
            sb.Append("{ \n");

            string querystring = CamlDesigner.SharePoint.Common.Builder.BuildQuerystring(mainobject.WhereNode, mainobject.OrderByNode);

            sb.Append("   $query = New-Object Microsoft.SharePoint.SPQuery; \n");

            if (!string.IsNullOrEmpty(querystring))
            {
                querystring = CamlDesigner.SharePoint.Common.Builder.IndentCAML(querystring);
                sb.Append("   $query.Query = \n");
                
                if (mainobject.ViewFieldsOnly)
                {
                    sb.Append("   $query.ViewFieldsOnly = $true \n"); 
                }

                sb.Append(string.Format("   \"{0}\"; \n", querystring));
            }

            if (mainobject.ViewFieldsNode != null)
            {
                sb.Append(string.Format("   $query.ViewFields = \"{0}\"; \n", mainobject.ViewFieldsNode.InnerXml.Replace("\"", "'")));
            }

            if (mainobject.ViewFieldsOnly)
            {
                sb.Append("   $query.ViewFieldsOnly = $true; \n");
            }

            if (mainobject.QueryOptions != null)
            {
                if (mainobject.QueryOptions.IncludeMandatoryColumns)
                    sb.Append("   $query.IncludeMandatoryColumns = true; \n");

                if (mainobject.QueryOptions.IncludeAttachmentUrls)
                    sb.Append("   $query.IncludeAttachmentUrls = true; \n");
                if (mainobject.QueryOptions.IncludeAttachmentVersion)
                    sb.Append("   $query.IncludeAttachmentVersion = true; \n");

                if (mainobject.QueryOptions.ExpandUserField)
                    sb.Append("   $query.ExpandUserField = true; \n");

                if (mainobject.QueryOptions.UtcDate)
                    sb.Append("   $query.DatesInUtc = true; \n");

                if (mainobject.QueryOptions.RowLimit > 0)
                    sb.Append(string.Format("   $query.RowLimit = {0}; \n", mainobject.QueryOptions.RowLimit.ToString()));
                else
                    sb.Append("   $query.RowLimit = #ByDefault_2147483647 \n");

                // Handle QueryOptions concerning Files and Folders
                if (mainobject.QueryOptions.QueryFilesAllFoldersDeep || mainobject.QueryOptions.QueryFoldersAllFoldersDeep || mainobject.QueryOptions.QueryFilesAndFoldersAllFoldersDeep)
                {
                    sb.Append("   $query.ViewAttributes = \"Scope='RecursiveAll'\"; \n");
                }
                else if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                {
                    if (mainobject.QueryOptions.QueryFilesInSubFolderDeep)
                        sb.Append("   $query.ViewAttributes = \"Scope='Recursive'\"; \n");
                    else if (mainobject.QueryOptions.QueryFilesAndFoldersInSubFolderDeep)
                        sb.Append("   $query.ViewAttributes = \"Scope='RecursiveAll'\"; \n");
                    else if (mainobject.QueryOptions.QueryFilesInSubFolder)
                        sb.Append("   $query.ViewAttributes = \"Scope='FilesOnly'\"; \n");
                }

                if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                    sb.Append(string.Format("   $query.Folder = $splist.RootFolder.SubFolders[\"{0}\"]; \n", mainobject.QueryOptions.SubFolder));
            }

            sb.Append("   $items = $splist.GetItems($query); \n");
            sb.Append("} \n");

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        #endregion
    }
}
