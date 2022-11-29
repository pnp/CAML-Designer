using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CamlDesigner2013.Helpers
{
    [ValueConversion(typeof(object), typeof(string))]
    public class ConcatenateFieldsMultiValueConverter : IMultiValueConverter
    {
        private string TooltipFormat = "{0} (connecting using {1} for {2})";
        private string TooltipFormatForCredentials = "{0} (connecting using {1} for {2} with credentials of {3})";

        public object Convert(
                    object[] values,
                    Type targetType,
                    object parameter,
                    System.Globalization.CultureInfo culture
                 )
        {
            string strDelimiter;
            StringBuilder sb = new StringBuilder();

            if (parameter != null)
            {
                //Use the passed delimiter.
                strDelimiter = parameter.ToString();
            }
            else
            {
                //Use the default delimiter.
                strDelimiter = ", ";
            }

            //Concatenate all fields

            List<string> passedValues = new List<string>();

            foreach (object value in values)
            {
                if (value != null && value.ToString().Trim().Length > 0)
                {
                    passedValues.Add(value.ToString());
                }
            }

            sb.Append(string.Format(TooltipFormat, passedValues[0], passedValues[1], passedValues[2]));

            return sb.ToString();
        }

        public object[] ConvertBack(
                    object value,
                    Type[] targetTypes,
                    object parameter,
                    System.Globalization.CultureInfo culture
              )
        {
            throw new NotImplementedException("ConcatenateFieldsMultiValueConverter cannot convert back (bug)!");
        }

    }

}
