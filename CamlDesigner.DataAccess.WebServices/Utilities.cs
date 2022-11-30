using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CamlDesigner.DataAccess.SharePoint.WebServices
{
    public class Utilities
    {
        public static string CreateISO8601DateTimeFromSystemDateTime(DateTime date)
        {
            // TODO: check ivm utd date
            // a sharePoint datetime is from the format yyyy-MM-ddThh:mm:ssZ

            string s = string.Format(System.Globalization.CultureInfo.InvariantCulture, date.ToString("yyyy-MM-ddThh:mm:ssZ"));
            return s;
        }

        public static DateTime CreateSystemDateTimeFromISO8601DateTime(string datestring)
        {
            // TODO: check ivm utd date
            // a sharePoint datetime is from the Created="20101220 10:55:17" Modified="20101220 10:55:18"
            return new DateTime(System.Convert.ToInt32(datestring.Substring(0, 4)), 
                System.Convert.ToInt32(datestring.Substring(4, 2)), 
                System.Convert.ToInt32(datestring.Substring(6, 2)),
                System.Convert.ToInt32(datestring.Substring(9, 2)), 
                System.Convert.ToInt32(datestring.Substring(12, 2)), 
                System.Convert.ToInt32(datestring.Substring(15, 2)));
        }
    }
}
