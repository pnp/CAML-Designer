using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Windows;
using CamlDesigner2013.Controls;

namespace CamlDesigner2013.Helpers
{
    public static class CheckForUpdate
    {

        public static void InstallUpdateSyncWithInfo()
        {
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();

                }
                catch (DeploymentDownloadException dde)
                {

                    CustomMessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    CustomMessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    CustomMessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    return;
                }

                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    if (!info.IsUpdateRequired)
                    {
                        MessageBoxResult dr = CustomMessageBox.Show("An update is available. Would you like to update the application now?", string.Format("Update Available - Version: {0}", info.AvailableVersion), MessageBoxButton.OKCancel);
                        if (!(MessageBoxResult.OK == dr))
                        {
                            doUpdate = false;
                        }
                    }
                    else
                    {
                        // Display a message that the app MUST reboot. Display the minimum required version.
                        MessageBox.Show("This application has detected a mandatory update from your current " +
                            "version to version " + info.MinimumRequiredVersion.ToString() +
                            ". The application will now install the update and restart.",
                            "Update Available", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }

                    if (doUpdate)
                    {
                        try
                        {
                            ad.Update();
                            CustomMessageBox.Show("The application has been upgraded, you must restart the application.");
                            //Process.Start(Application.ResourceAssembly.Location);
                            Application.Current.Shutdown();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            CustomMessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                            return;
                        }
                    }
                }
                else
                {
                    CustomMessageBox.Show("No update available at this time. You have the latest version.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                CustomMessageBox.Show("This application cannot be updated at this point.","Information",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
        }

    }
}
