using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CamlDesigner.SharePoint.Common
{
    public class UtilityFunctionsCAML
    {
        public static string GetCAMLDataType(string datatype)
        {
            string cAMLdatatype = null;

            if (string.IsNullOrEmpty(datatype))
                throw new ArgumentNullException(datatype);

            switch (datatype)
            {
                case "Single line of text":
                    cAMLdatatype = "Text";
                    break;
                case "Multiple lines of text":
                    cAMLdatatype = "Note";
                    break;
                default:
                    cAMLdatatype = "Text";
                    break;
            }
            return cAMLdatatype;
        }

        public static string CreateISO8601DateTimeFromSystemDateTime(DateTime date)
        {
            // TODO: check ivm utd date
            // a sharePoint datetime is from the format yyyy-MM-ddThh:mm:ssZ
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, date.ToString("yyyy-MM-ddThh:mm:ssZ"));
        }

        public static int GetTemplateID(string templateName)
        {

            int templateID = -1;

            if (!int.TryParse(templateName, out templateID))
            {
                switch (templateName)
                {
                    case "GenericList":
                        templateID = 100;
                        break;
                    case "DocumentLibrary":
                        templateID = 101;
                        break;
                    case "Survey":
                        templateID = 102;
                        break;
                    case "Links":
                        templateID = 103;
                        break;
                    case "Announcements":
                        templateID = 104;
                        break;
                    case "Contacts":
                        templateID = 105;
                        break;
                    case "Events":
                        templateID = 106;
                        break;
                    case "Tasks":
                        templateID = 107;
                        break;
                    case "DiscussionBoard":
                        templateID = 108;
                        break;
                    case "PictureLibrary":
                        templateID = 109;
                        break;
                    case "DataSources":
                        templateID = 110;
                        break;
                    case "WebTemplateCatalog":
                        templateID = 111;
                        break;
                    case "UserInformation":
                        templateID = 112;
                        break;
                    case "WebPartCatalog":
                        templateID = 113;
                        break;
                    case "ListTemplateCatalog":
                        templateID = 114;
                        break;
                    case "XMLForm":
                        templateID = 115;
                        break;
                    case "MasterPageCatalog":
                        templateID = 116;
                        break;
                    case "NoCodeWorkflows":
                        templateID = 117;
                        break;
                    case "WorkflowProcess":
                        templateID = 118;
                        break;
                    case "WebPageLibrary":
                        templateID = 119;
                        break;
                    case "CustomGrid":
                        templateID = 120;
                        break;
                    case "SolutionCatalog":
                        templateID = 121;
                        break;
                    case "NoCodePublic":
                        templateID = 122;
                        break;
                    case "ThemeCatalog":
                        templateID = 123;
                        break;
                    case "DataConnectionLibrary":
                        templateID = 130;
                        break;
                    case "WorkflowHistory":
                        templateID = 140;
                        break;
                    case "GanttTasks":
                        templateID = 150;
                        break;
                    case "Meetings":
                        templateID = 200;
                        break;
                    case "Agenda":
                        templateID = 201;
                        break;
                    case "MeetingUser":
                        templateID = 202;
                        break;
                    case "Decision":
                        templateID = 204;
                        break;
                    case "MeetingObjective":
                        templateID = 207;
                        break;
                    case "TextBox":
                        templateID = 210;
                        break;
                    case "ThingsToBring":
                        templateID = 211;
                        break;
                    case "HomePageLibrary":
                        templateID = 212;
                        break;
                    case "Posts":
                        templateID = 301;
                        break;
                    case "Comments":
                        templateID = 302;
                        break;
                    case "Categories":
                        templateID = 303;
                        break;
                    case "Facility":
                        templateID = 402;
                        break;
                    case "Whereabouts":
                        templateID = 403;
                        break;
                    case "CallTrack":
                        templateID = 404;
                        break;
                    case "Circulation":
                        templateID = 405;
                        break;
                    case "Timecard":
                        templateID = 420;
                        break;
                    case "Holidays":
                        templateID = 421;
                        break;
                    case "IMEDic":
                        templateID = 499;
                        break;
                    case "ExternalList":
                        templateID = 600;
                        break;
                    case "IssueTracking":
                        templateID = 1100;
                        break;
                    case "AdminTasks":
                        templateID = 1200;
                        break;
                    case "HealthRules":
                        templateID = 1220;
                        break;
                    case "HealthReports":
                        templateID = 1221;
                        break;
                    default:
                        templateID = 100;
                        break;
                }
            }
            return templateID;
        }
    }
}
