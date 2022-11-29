using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CamlDesigner2013.Helpers
{
    public class UI
    {
        public static Color GetColorFromHexString(string s)
        {
            s = s.Replace("#", string.Empty);
            byte a = System.Convert.ToByte(s.Substring(0, 2), 16);
            byte r = System.Convert.ToByte(s.Substring(2, 2), 16);
            byte g = System.Convert.ToByte(s.Substring(4, 2), 16);
            byte b = System.Convert.ToByte(s.Substring(6, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
