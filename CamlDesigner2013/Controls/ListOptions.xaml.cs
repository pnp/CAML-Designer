using CamlDesigner.SharePoint.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CamlDesigner2013.Controls
{
    /// <summary>
    /// Interaction logic for ListOptions.xaml
    /// </summary>
    public partial class ListOptions : UserControl
    {

        private ListQueryOptions listOptions;

        public ListOptions()
        {
            InitializeComponent();

            listOptions = new ListQueryOptions();
        }

        private void DocumentSets_Click(object sender, RoutedEventArgs e)
        {

            // fill document set list: CurrentListDocumentSets




        }

        private void CurrentListDocumentSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentListDocumentSets.SelectedItem != null)
            {
                listOptions.DocumentSetUrl = string.Empty;

                //below is the query... url must be set to folder
                //SPQuery itemDocSetQuery = new SPQuery();
                //itemDocSetQuery.Folder = docsetitem.Folder;
            }
        }

        private void IncludeProjectedFields_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
