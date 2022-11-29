using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamlDesigner2013.ViewModels
{
    public class ListViewModel : TreeViewItemViewModel
    {
        private CamlDesigner.SharePoint.Objects.List list = null;

        public ListViewModel(CamlDesigner.SharePoint.Objects.List list)
        {
            this.list = list;
        }

        public Guid ID
        {
            get
            {
                if (this.list != null)
                    return this.list.ID;
                else
                    return Guid.Empty;
            }
        }

        public string Title
        {
            get
            {
                if (this.list != null)
                    return this.list.Title;
                else
                    return string.Empty;
            }
        }

        public string WebUrl
        {
            get
            {
                if (this.list != null)
                    return this.list.WebUrl;
                else
                    return string.Empty;
            }
        }

        public int TemplateID
        {
            get
            {
                if (this.list != null)
                    return this.list.TemplateID;
                else
                    return -1;
            }
        }

        protected override void LoadChildren()
        {
        }
    }
}
