using DesktopAlert;
using System.ComponentModel;
using System.Windows;

namespace CamlDesigner2013.Alerts
{
    /// <summary>
    /// Interaction logic for SimpleAlert.xaml
    /// </summary>
    public partial class SimpleAlert : DesktopAlertBase
    {
        public static DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(SimpleAlert));

        [Bindable(true)]
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public SimpleAlert()
        {
            InitializeComponent();
        }
    }
}
