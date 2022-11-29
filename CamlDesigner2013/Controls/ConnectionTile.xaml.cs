using CamlDesigner2013.Connections.Models;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace CamlDesigner2013.Controls
{
    /// <summary>
    /// Interaction logic for ConnectionTile.xaml
    /// </summary>
    public partial class ConnectionTile : UserControl
    {
        public ConnectionTile()
        {
            InitializeComponent();
        }

        void child_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.CurrentConnection = this.DataContext as Connections.Models.Recent;
            Helpers.Connection.Connect(App.CurrentConnection);
        }

        private void Tile_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void RemoveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
