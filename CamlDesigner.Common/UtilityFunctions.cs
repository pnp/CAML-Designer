using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace CamlDesigner.SharePoint.Common
{
    public class UtilityFunctions
    {
        #region HTML METHODS

        public static String RemoveHTMLFromString(String toEncode)
        {
            // decode HTML first to avoid missing encoded HTML
            toEncode = HttpUtility.HtmlDecode(toEncode);

            if (!String.IsNullOrEmpty(toEncode))
            {
                // remove HTML tags
                toEncode = Regex.Replace(toEncode, @"<(.|\n)*?>", String.Empty);
            }

            // encode to HTML to avoid invalid characters
            toEncode = HttpUtility.HtmlEncode(toEncode);

            return toEncode;
        }

        #endregion;


        #region STRING METHODS
        public static string FormatIntegerArrayToString(int[] integerArray)
        {
            string valuestring = string.Empty;
            if (integerArray != null)
            {
                for (int i = 0; i < integerArray.Length; i++)
                {
                    if (!string.IsNullOrEmpty(valuestring))
                        valuestring += ";";
                    valuestring += integerArray[i];
                }
            }
            return valuestring;
        }

        #endregion
    }
}
