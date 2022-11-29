using  CamlDesigner.Common;
using CamlDesigner.SharePoint.Objects;
using CamlDesigner2013.Alerts;
using CamlDesigner2013.Connections.Models;
using System;

namespace CamlDesigner2013.Helpers
{
    public static class Connection
    {
        public static event ConnectionEventHandler ConnectionEvent;

        public static void Connect(Recent connection)
        {
            try
            {

                SimpleAlert alert = new SimpleAlert()
                {
                    Title = "Action",
                    Message = "Trying to connect to: " + connection.SiteUrl
                };

                Enumerations.ApiConnectionType connectionType = Enumerations.ApiConnectionType.ServerObjectModel;

                if (connection.ConnectionType == Recent.ApiConnectionType.ClientOM)
                {
                    connectionType = Enumerations.ApiConnectionType.ClientObjectModel;
                }
                else if (connection.ConnectionType == Recent.ApiConnectionType.WebServices)
                {
                    connectionType = Enumerations.ApiConnectionType.WebServices;
                }

                Enumerations.SharePointVersions sharePointVersion = Enumerations.SharePointVersions.SP2013;
                //TODO: expand with other versions
                if (connection.InstallationType == Recent.TargetInstallationType.SP2010)
                {
                    sharePointVersion = Enumerations.SharePointVersions.SP2010;
                }

                if (connection.InstallationType == Recent.TargetInstallationType.O365)
                {
                    sharePointVersion = Enumerations.SharePointVersions.SP2013;
                }

                App.GeneralInformation = new CAMLDesigner.BusinessObjects.GeneralInfo(connection.SiteUrl, connectionType, sharePointVersion);

                if (string.IsNullOrEmpty(connection.UserName) == false)
                {
                    App.GeneralInformation.Username = connection.UserName;
                }

                if (string.IsNullOrEmpty(connection.Password) == false)
                {
                    App.GeneralInformation.Password = connection.Password;
                }

                if (string.IsNullOrEmpty(connection.Domain) == false)
                {
                    App.GeneralInformation.Domain = connection.Domain;
                }
            }
            catch (Exception ex)
            {
                // TODO: log error
                Logger.WriteToLogFile("LoginScreen: An error has occured while filling in all the data", ex);
            }

            // set the CAML wrapper to null so that all parameters are correctly set
            App.CamlWrapper = null;

            // inform the main form that a connection needs to be made
            if (ConnectionEvent != null)
            {
                ConnectionEvent();
            }
        }
    }
}
