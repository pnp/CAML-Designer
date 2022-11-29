using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamlDesigner.Common
{
    public class CAMLConstants
    {
        public const string FolderQuery = "<Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></Where>";

        public class QueryOptions
        {
            public const string RecursiveAll = "<ViewAttributes Scope='RecursiveAll'/>";
        }
    }
}
