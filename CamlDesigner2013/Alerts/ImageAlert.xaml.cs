using DesktopAlert;
using System.ComponentModel;
using System.Windows;

namespace CamlDesigner2013.Alerts
{
    /// <summary>
    /// Interaction logic for ImageAlert.xaml
    /// </summary>
    public partial class ImageAlert : DesktopAlertBase
    {
        public static DependencyProperty UrlProperty = DependencyProperty.Register("Url", typeof(string), typeof(ImageAlert));

        [Bindable(true)]
        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public ImageAlert()
        {
            InitializeComponent();
        }
    }
}
