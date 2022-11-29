using CamlDesigner2013.Connections.Models;
using CamlDesigner2013.Connections.UI;
using MahApps.Metro.Controls;
using System.Windows.Controls;

namespace CamlDesigner2013.Controls
{
    /// <summary>
    /// Interaction logic for ConnectionTile.xaml
    /// </summary>
    public partial class SecureConnectionTile : UserControl
    {
        public SecureConnectionTile()
        {
            InitializeComponent();
        }

        void child_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.CurrentConnection = this.DataContext as Connections.Models.Recent;
            Helpers.Connection.Connect(App.CurrentConnection);
        }
    }
}
