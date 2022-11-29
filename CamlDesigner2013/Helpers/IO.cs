using CamlDesigner2013.Connections.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CamlDesigner2013.Helpers
{
    public static class IO
    {

        public static bool IsDirectoryExisting(string fullPath)
        {
            DirectoryInfo dir = new DirectoryInfo(fullPath);
            return dir.Exists;
        }

        public static bool DoesDirectoryExist(string fullPath,string basicPath, string path1,string path2)
        {
            DirectoryInfo dir = new DirectoryInfo(fullPath);
            if (!dir.Exists)
            {
                //create path 
                CreatePath(basicPath, path1, path2);
            }

            dir = new DirectoryInfo(fullPath);
            return dir.Exists;
        }

        public static void CreatePath(string path, string path1, string path2)
        {
            if (IsDirectoryExisting(path + @"\" + path1))
            {
                if (!IsDirectoryExisting(path + @"\" + path1 + @"\" + path2))
                {
                    Directory.CreateDirectory(path + @"\" + path1 + @"\" + path2);
                }
            }
            else
            {
                Directory.CreateDirectory(path + @"\" + path1);
                Directory.CreateDirectory(path + @"\" + path1 + @"\" + path2);
            }
        }

        public static Task<ViewModel> GetRecent()
        {
            return Task.Run(() =>
                {
                    var recents = new ViewModel { RecentList = new ObservableCollection<Recent>() };
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BIWUG\CamlDesigner";
                    if (!IO.DoesDirectoryExist(path, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"BIWUG","CamlDesigner"))
                    {
                        return new ViewModel { RecentList = new ObservableCollection<Recent>() };
                    }
                    string filepath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\BIWUG\\CamlDesigner\Recent.xml";
                    if (File.Exists(filepath))
                    {
                        XDocument xdoc = XDocument.Load(filepath);
                        foreach (XElement item in xdoc.Descendants("Site"))
                        {
                            Recent.TargetInstallationType installationType = GetInstallationType(item, Recent.TargetInstallationType.SP2013);
                            Recent.ApiConnectionType connectionType = GetConnectionType(item, Recent.ApiConnectionType.ClientOM);

                            recents.RecentList.Add(new Recent()
                            {
                                UserName = item.Element("UserName").Value,
                                SiteName = item.Element("Name").Value,
                                SiteUrl = item.Element("URL").Value,
                                Password = item.Element("Password").Value,
                                InstallationType = installationType,
                                ConnectionType = connectionType,
                                CurrentCredentials = bool.Parse(item.Element("CurrentCredentials").Value)
                            });
                        }

                        return recents;
                    }
                    else
                    {
                        return new ViewModel { RecentList = new ObservableCollection<Recent>() };
                    }
                });
        }

        private static Recent.ApiConnectionType GetConnectionType(XElement item, Recent.ApiConnectionType defaultConnectionType)
        {
            Recent.ApiConnectionType retVal = defaultConnectionType;
            string apiConnectionType = (item.Element("ApiConnectionType") == null) ? string.Empty : item.Element("ApiConnectionType").Value;

            if (string.IsNullOrEmpty(apiConnectionType) == false)
            {
                Enum.TryParse<Recent.ApiConnectionType>(apiConnectionType, out retVal);
            }
            else
            {
                // Handle old type of Recent
                //bool isServerOM = false, isClientOM = false, isWebService = false;
                bool isClientOM = false, isWebService = false; 
                //bool.TryParse(item.Element("ServerObjectModel").Value, out isServerOM);
                bool.TryParse(item.Element("ClientObjectModel").Value, out isClientOM);
                bool.TryParse(item.Element("WebServices").Value, out isWebService);

                //if (isServerOM)
                //{
                //    retVal = Recent.ApiConnectionType.ServerOM;
                //} else
                if (isClientOM)
                {
                    retVal = Recent.ApiConnectionType.ClientOM;
                }
                else if (isWebService)
                {
                    retVal = Recent.ApiConnectionType.WebServices;
                }
            }

            return retVal;
        }

        private static Recent.TargetInstallationType GetInstallationType(XElement item, Recent.TargetInstallationType defaultInstallationType)
        {
            Recent.TargetInstallationType retVal = defaultInstallationType;
            string targetInstallationType = (item.Element("TargetInstallationType") == null) ? string.Empty : item.Element("TargetInstallationType").Value;

            if (string.IsNullOrEmpty(targetInstallationType) == false)
            {
                Enum.TryParse<Recent.TargetInstallationType>(targetInstallationType, out retVal);
            }

            return retVal;
        }

        public static void SetRecent(Recent currentConnection)
        {
            string filepath = GetFilePath();
            XDocument xdoc = GetRecentDocument(filepath);

            bool nodeExists = CheckIfNodeExists(currentConnection, xdoc);

            if (xdoc != null && !nodeExists)
            {
                XElement parentNode = xdoc.Element("Sites");
                parentNode.Add(new XElement(
                       "Site",
                       new XElement("Name", currentConnection.SiteName),
                       new XElement("URL", currentConnection.SiteUrl),
                       new XElement("CurrentCredentials", currentConnection.CurrentCredentials.ToString()),
                       new XElement("UserName", currentConnection.UserName),
                       new XElement("Password", currentConnection.Password),
                       new XElement("Domain", currentConnection.Domain),
                       new XElement("ApiConnectionType", currentConnection.ConnectionType.ToString()),
                       new XElement("TargetInstallationType", currentConnection.InstallationType.ToString())));

                xdoc.Save(filepath, SaveOptions.None);
            }
        }

        private static bool CheckIfNodeExists(Recent currentConnection, XDocument xdoc)
        {
            var nodes = (from c in xdoc.Descendants("Site")
                         where (c.Element("URL").Value == currentConnection.SiteUrl)
                         select c).ToList();

            if (nodes.Count > 0)
            {
                bool sameNode = false;
                foreach (var item in nodes)
                {
                    if (item.Element("CurrentCredentials").Value == currentConnection.CurrentCredentials.ToString())
                    {
                        if (item.Element("UserName").Value == currentConnection.UserName)
                        {
                            if (item.Element("Password").Value == currentConnection.Password)
                            {
                                if (item.Element("Domain").Value == currentConnection.Domain)
                                {
                                    if (item.Element("ApiConnectionType").Value == currentConnection.ConnectionType.ToString())
                                    {
                                        if (item.Element("TargetInstallationType").Value == currentConnection.InstallationType.ToString())
                                        {
                                            sameNode = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return sameNode;
            }
            else
            {
                return false;
            }
        }

        public static void UpdateRecent(Recent currentConnection)
        {
            string filepath = GetFilePath();
            XDocument xdoc = GetRecentDocument(filepath);

            if (xdoc != null)
            {
                XElement item = (from xml2 in xdoc.Elements("Sites").Elements("Site")
                                 where (xml2.Element("URL").Value == App.CurrentConnection.SiteUrl)
                                    && (xml2.Element("TargetInstallationType").Value == App.CurrentConnection.InstallationType.ToString())
                                    && (xml2.Element("ApiConnectionType").Value == App.CurrentConnection.ConnectionType.ToString())
                                 select xml2).FirstOrDefault();

                if (item != null)
                {
                    item.Element("Name").Value = App.CurrentConnection.SiteName;
                    xdoc.Save(filepath, SaveOptions.None);
                }
            }
        }

        public static bool ClearRecent()
        {
            string filepath = GetFilePath();
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            return File.Exists(filepath);
        }

        private static string GetFilePath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\BIWUG\\CamlDesigner";
            if (!IO.IsDirectoryExisting(path))
            {
                IO.CreatePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BIWUG", "CamlDesigner");
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\BIWUG\\CamlDesigner\Recent.xml";
        }

        private static XDocument GetRecentDocument(string filepath)
        {
            XDocument xdoc = null;
            if (File.Exists(filepath))
            {
                xdoc = XDocument.Load(filepath);

            }
            else
            {
                xdoc = new XDocument();
                xdoc.Add(new XElement("Sites"));
            }

            return xdoc;
        }
    }
}
