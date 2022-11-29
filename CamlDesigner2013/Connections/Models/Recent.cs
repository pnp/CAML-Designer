using System.Collections.ObjectModel;
using System.ComponentModel;
namespace CamlDesigner2013.Connections.Models
{
    public class ViewModel
    {
        public ObservableCollection<Recent> RecentList { get; set; }
    }

    /// <summary>
    /// This class holds an object called Recent
    /// </summary>
    public class Recent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }


        public enum TargetInstallationType { SP2007, SP2010, SP2013, SP2016, O365 };
        public enum ApiConnectionType { ClientOM, WebServices }

        public Recent()
        {
        }

        public Recent(string sitename, string siteurl, TargetInstallationType installationType, ApiConnectionType connectionType, bool currentcredentials, string username, string password, string domain)
        {
            this.SiteName = sitename;
            this.SiteUrl = siteurl;
            this.InstallationType = installationType;
            this.ConnectionType = connectionType;
            this.CurrentCredentials = currentcredentials;
            this.UserName = username;
            this.Password = password;
            this.Domain = domain;
        }

        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public TargetInstallationType InstallationType { get; set; }
        public ApiConnectionType ConnectionType { get; set; }
        public bool CurrentCredentials { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
    }
}
