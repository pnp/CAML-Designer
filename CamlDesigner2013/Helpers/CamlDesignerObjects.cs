using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamlDesigner2013
{
    public enum TreeViewState
    {
        ShowWebs,
        ShowLists,
        ShowFields,
        ShowSiteColumns,
        ShowContentTypes
    }

    public enum QueryType
    {
        ViewFields,
        OrderBy,
        Where,
        QueryOptions
    }
}
