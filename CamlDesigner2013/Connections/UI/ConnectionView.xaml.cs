using CamlDesigner.Common;
using CamlDesigner2013.Alerts;
using CamlDesigner2013.Connections.Models;
using CamlDesigner2013.Controls;
using CamlDesigner2013.Helpers;
using MahApps.Metro.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace CamlDesigner2013.Connections.UI
{
    /// <summary>
    /// Interaction logic for ConnectionView.xaml
    /// </summary>
    public partial class ConnectionView : UserControl
    {

        public ConnectionView()
        {
            InitializeComponent();
            // Get Recent sites
            //TODO: only at start this recent list is filled, when entering a new site it's not being re-populated..
            this.GetRecents();
            try
            {
                bool foundAssembly = App.SharePointLocallyInstalled;


                // KBOS 1/1/2014: server OM button cannot be checked by default because of low performance
                //this.ServerOMButton.IsChecked = foundAssembly;
                //this.ClientOMButton.IsChecked = (foundAssembly == false);
                //this.O365Button.IsChecked = (foundAssembly == false);
                this.ClientOMButton.IsChecked = true;
                App.GeneralInformation.ConnectionType = Enumerations.ApiConnectionType.ClientObjectModel;

            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile("LoginScreen: An error has occured while looking for the server assembly", ex);
            }


        }

        public async void GetRecents()
        {
            var historyList = await Helpers.IO.GetRecent();
            this.DataContext = historyList;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder(App.mdiView.DebugBlock.Text);

            try
            {
                sb.AppendLine("Connect_Click Method, going to do some actions");
                sb.AppendLine("Connect_Click Method, checking if url is not empty");
                if (UrlTextBox.Text.Length == 0)
                {
                    sb.AppendLine("Connect_Click Method, Url field is empty, not allowed, stopping here");
                    CustomMessageBox.Show(Application.Current.Resources["ConnectionsWindowEmtpyUrlMsgBoxMessage"].ToString(), Application.Current.Resources["ConnectionsWindowEmtpyUrlMsgBoxTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                    Logger.WriteToLogFile("LoginScreen: Insufficient data, no URL", null);
                    return;
                }
                else
                {
                    sb.AppendLine("Connect_Click Method, url is not empty");
                }

                sb.AppendLine("Connect_Click Method, Going to see if the url is something that exists");
                if (!OnlineChecker.RemoteFileExists(UrlTextBox.Text, sb))
                {
                    sb.AppendLine("Connect_Click Method, Url is not OK, stopping here");
                    CustomMessageBox.Show(Application.Current.Resources["ConnectionsWindowInvalidUrlMsgBoxMessage"].ToString(), Application.Current.Resources["ConnectionsWindowEmtpyUrlMsgBoxTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                    Logger.WriteToLogFile("LoginScreen: URL link isn't valid", null);
                    return;
                }

                sb.AppendLine("Connect_Click Method, filling in the installation type and how to connect type, default, SP2013 and ServerOM");
                Recent.TargetInstallationType installationType = Recent.TargetInstallationType.SP2013;
                Recent.ApiConnectionType apiType = Recent.ApiConnectionType.ClientOM;

                if (O365Button.IsChecked.HasValue && O365Button.IsChecked.Value)
                {
                    sb.AppendLine("Connect_Click Method, going to O365 ");
                    installationType = Recent.TargetInstallationType.O365;
                }

               
                if (SP2013Button.IsChecked.HasValue && SP2013Button.IsChecked.Value)
                {
                    sb.AppendLine("Connect_Click Method, going to SP2013 ");
                    installationType = Recent.TargetInstallationType.SP2013;
                }

                if (SP2007Button.IsChecked.HasValue && SP2007Button.IsChecked.Value)
                {
                    sb.AppendLine("Connect_Click Method, going to SP2007 ");
                    installationType = Recent.TargetInstallationType.SP2007;
                }

                if (SP2016Button.IsChecked.HasValue && SP2016Button.IsChecked.Value)
                {
                    sb.AppendLine("Connect_Click Method, going to SP2016 ");
                    installationType = Recent.TargetInstallationType.SP2016;
                }

                if (SP2010Button.IsChecked.HasValue && SP2010Button.IsChecked.Value)
                {
                    sb.AppendLine("Connect_Click Method, going to SP2010 ");
                    installationType = Recent.TargetInstallationType.SP2010;
                }

                //sb.AppendLine(timer.Elapsed.ToString());
                if (ClientOMButton.IsChecked.HasValue && ClientOMButton.IsChecked.Value)
                {
                    sb.AppendLine("Connect_Click Method, using CSOM to connect ");
                    apiType = Recent.ApiConnectionType.ClientOM;
                }
                else if (WebServicesButton.IsChecked.HasValue && WebServicesButton.IsChecked.Value)
                {
                    sb.AppendLine("Connect_Click Method, using web services to connect ");
                    apiType = Recent.ApiConnectionType.WebServices;
                }

                string url = UrlTextBox.Text.ToLower().Trim();
                sb.AppendLine(string.Format("Connect_Click Method, new url after trim = {0} ", url));
                if (installationType == Recent.TargetInstallationType.O365)
                {
                    url = url.Replace("http://", "https://");
                    sb.AppendLine(string.Format("Connect_Click Method, new url in case of O365 (httpS) = {0} ", url));
                }

                // need to check if the server assembly is there, something is wrong when in client mode
                try
                {
                    sb.AppendLine("Connect_Click Method, Going to attempt to make a connection with all the data, setting current connection");

                    App.CurrentConnection = new Recent("Temp", url, installationType, apiType, (bool)CurrentCredentials.IsChecked, UserNameTextBox.Text.Trim().ToLower(), PasswordTextBox.Password, DomainTextBox.Text.Trim());
                    sb.AppendLine("Connect_Click Method, Going to attempt to make a connection with all the data, DONE setting current connection, attempting connection");
                    if (ValidateConnection(sb))
                    {
                        Helpers.IO.SetRecent(App.CurrentConnection);
                        sb.AppendLine("Connect Method, done setting all the values, informing main form to make the connection.");
                        Helpers.Connection.Connect(App.CurrentConnection);
                    }
                }
                catch (ApplicationException ex)
                {
                    sb.AppendLine(string.Format("Connect_Click Method, application error has occured: {0}, stacktrace is {1} ", ex.Message, ex.StackTrace));
                    Logger.WriteToLogFile("LoginScreen: Failed to login", ex);
                    CustomMessageBox.Show(string.Format("Failed to login: {0}", ex.Message), "No login possible", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    sb.AppendLine(string.Format("Connect_Click Method, General error has occured: {0}, stacktrace is {1} ", ex.Message, ex.StackTrace));
                    Logger.WriteToLogFile("LoginScreen: Failed to login", ex);
                    CustomMessageBox.Show("Failed to login", "No login possible", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Inform user
                }

                //sb.AppendLine(timer.Elapsed.ToString());
                //SimpleAlert simpleAlert = new SimpleAlert();
                //simpleAlert.Title = Application.Current.Resources["ConnectionsWindowConnectAlertTitle"].ToString();
                //simpleAlert.Message = Application.Current.Resources["ConnectionsWindowConnectAlertMessage"].ToString();
                this.GetRecents();
                //sb.AppendLine(timer.Elapsed.ToString());
                //timer.Stop();
                //CustomMessageBox.Show(sb.ToString());
            }
            catch (Exception ex)
            {
                sb.AppendLine(string.Format("Connect_Click Method, Big error has occured that is not captured before: {0}, stacktrace is {1} ", ex.Message, ex.StackTrace));
                Logger.WriteToLogFile("An error has occured in the creating the connection", ex);
                CustomMessageBox.Show("An error has occured, please send the log file to camlfeedback@biwug.be", "An error has occured", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (App.mdiView != null)
            {
                if (App.mdiView.Debug.Visibility == System.Windows.Visibility.Visible)
                {
                    App.mdiView.DebugBlock.Text = sb.ToString();
                }
            }
        }

        private void CurrentCredentials_Checked(object sender, RoutedEventArgs e)
        {
            EnableConnectionSelection(true);
        }

        private void CurrentCredentials_Unchecked(object sender, RoutedEventArgs e)
        {
            EnableConnectionSelection(false);
        }

        private void EnableConnectionSelection(bool currentCredentilsIsChecked)
        {
            if (CustomCredentials != null)
            {
                CustomCredentials.Visibility = currentCredentilsIsChecked ? Visibility.Collapsed : Visibility.Visible;
                try
                {
                    ConnectButton.ToolTip = currentCredentilsIsChecked ?
                        (Application.Current.Resources["ConnectionsWindowConnectButtonTooltipCCOn"] as string) + UrlTextBox.Text :
                        (Application.Current.Resources["ConnectionsWindowConnectButtonTooltipCCOff"] as string);
                }
                catch (Exception ex)
                {
                    Logger.WriteToLogFile("LoginScreen: An error has occured while looking for the server assembly", ex);
                }

                if (DomainTextBox != null)
                {
                    DomainTextBox.Visibility = (O365Button != null && O365Button.IsChecked.Value) ? Visibility.Hidden : Visibility.Visible;
                }
            }
        }

        private void URL_LostFocus(object sender, RoutedEventArgs e)
        {
            ConnectButton.ToolTip = Application.Current.Resources["ConnectionsWindowConnectButtonTooltipCCOn"].ToString() + UrlTextBox.Text;
        }

        private void SP2010_Checked(object sender, RoutedEventArgs e)
        {
            if (SP2007Button != null)
                SP2007Button.IsChecked = false;

            if (O365Button != null)
                O365Button.IsChecked = false;

            if (SP2013Button != null)
                SP2013Button.IsChecked = false;

            if (SP2016Button != null)
                SP2016Button.IsChecked = false;

            ClientOMButton.Visibility = Visibility.Visible;
            WebServicesButton.Visibility = Visibility.Visible;

            ClientOMButton.IsChecked = true;

            if (CurrentCredentials != null && CurrentCredentials.IsChecked.HasValue)
            {
                EnableConnectionSelection(CurrentCredentials.IsChecked.Value);
            }
        }

        private void SP2007_Checked(object sender, RoutedEventArgs e)
        {
            if (O365Button != null)
                O365Button.IsChecked = false;

            if (SP2010Button != null)
                SP2010Button.IsChecked = false;

            if (SP2013Button != null)
                SP2013Button.IsChecked = false;

            if (SP2016Button != null)
                SP2016Button.IsChecked = false;

            if (ClientOMButton != null)
                ClientOMButton.Visibility = Visibility.Hidden;
            if (WebServicesButton != null)
                WebServicesButton.Visibility = Visibility.Visible;

            WebServicesButton.IsChecked = true;

            if (CurrentCredentials != null && CurrentCredentials.IsChecked.HasValue)
            {
                EnableConnectionSelection(CurrentCredentials.IsChecked.Value);
            }
        }

        private void SP2013_Checked(object sender, RoutedEventArgs e)
        {
            if (O365Button != null)
                O365Button.IsChecked = false;

            if (SP2007Button != null)
                SP2007Button.IsChecked = false;

            if (SP2010Button != null)
                SP2010Button.IsChecked = false;

            if (SP2016Button != null)
                SP2016Button.IsChecked = false;

            // make the buttons visible

            if (ClientOMButton != null)
                ClientOMButton.Visibility = Visibility.Visible;
            if (WebServicesButton != null)
                WebServicesButton.Visibility = Visibility.Visible;

            // set the default button
            if (ClientOMButton != null)
                ClientOMButton.IsChecked = true;

            if (CurrentCredentials != null && CurrentCredentials.IsChecked.HasValue)
            {
                EnableConnectionSelection(CurrentCredentials.IsChecked.Value);
            }
        }

        /// <summary>
        /// for SP 2016 the connection for SP2013 can still be used. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SP2016_Checked(object sender, RoutedEventArgs e)
        {

            //SP2010Button.IsChecked = false;
            if (SP2007Button != null)
                SP2007Button.IsChecked = false;

            if (SP2010Button != null)
                SP2010Button.IsChecked = false;

            if (SP2013Button != null)
                SP2013Button.IsChecked = false;

            if (O365Button != null)
                O365Button.IsChecked = false;

            // make the buttons visible

            if (ClientOMButton != null)
                ClientOMButton.Visibility = Visibility.Visible;
            if (WebServicesButton != null)
                WebServicesButton.Visibility = Visibility.Hidden;

            // set the default button
            if (ClientOMButton != null)
                ClientOMButton.IsChecked = true;

            if (CurrentCredentials != null && CurrentCredentials.IsChecked.HasValue)
            {
                EnableConnectionSelection(CurrentCredentials.IsChecked.Value);
            }
        }

        private void O365_Checked(object sender, RoutedEventArgs e)
        {
            if (SP2007Button != null)
                SP2007Button.IsChecked = false;

            if (SP2010Button != null)
                SP2010Button.IsChecked = false;

            if (SP2013Button != null)
                SP2013Button.IsChecked = false;

            if (SP2016Button != null)
                SP2016Button.IsChecked = false;

            if (ClientOMButton != null)
                ClientOMButton.Visibility = Visibility.Visible;
            if (WebServicesButton != null)
                WebServicesButton.Visibility = Visibility.Hidden;

            ClientOMButton.IsChecked = true;

            if (CurrentCredentials != null)
            {
                if (CurrentCredentials.IsChecked == false)
                {
                    EnableConnectionSelection(false);
                }
                else
                {
                    CurrentCredentials.IsChecked = false; // forces EnableConnectionSelection via event
                }
            }
        }



        private void ClientOMButton_Checked(object sender, RoutedEventArgs e)
        {

            WebServicesButton.IsChecked = false;
        }

        private void WebServicesButton_Checked(object sender, RoutedEventArgs e)
        {

            ClientOMButton.IsChecked = false;
        }

        private bool ValidateConnection(StringBuilder sb)
        {
            sb.AppendLine("ValidateConnection Method, starting this method");
            bool isValid = true;

            //if (App.CurrentConnection.ConnectionType == Recent.ApiConnectionType.ServerOM
            //    && App.CurrentConnection.InstallationType == Recent.TargetInstallationType.SP2013)
            //{
            //    if (CAMLDesigner.BusinessObjects.Wrapper.GetSharePointVersion(App.CurrentConnection.SiteUrl) == Enumerations.SharePointVersions.SP2010)
            //    {
            //        sb.AppendLine("ValidateConnection Method, catching the SP version and checking if the url is really pointing to SP2013 instead of SP2010.. it is SP2010, not supported via ServerOM, Stopping");
            //        throw new ApplicationException("You try to connect to a SharePoint 2010 site using the SP2013 server object model. This is not supported by this version of the CAML Designer.");
            //    }
            //}

            //if (App.CurrentConnection.ConnectionType == Recent.ApiConnectionType.ServerOM
            //    && App.CurrentConnection.InstallationType == Recent.TargetInstallationType.SP2010)
            //{
            //    // you can't connect to SP2010 using the SP2013 server-side object model or this appliation
            //    sb.AppendLine("ValidateConnection Method, setting serverOM in combination with SP2010 to ClientOM");
            //    App.CurrentConnection.ConnectionType = Recent.ApiConnectionType.ClientOM;
            //}

            // you can connect to SP2O1O using the SP2010 client-side object model, but there are restrictions
            if (App.CurrentConnection.ConnectionType == Recent.ApiConnectionType.ClientOM
                && App.CurrentConnection.InstallationType == Recent.TargetInstallationType.SP2013)
            {
                Enumerations.SharePointVersions spVersion = CAMLDesigner.BusinessObjects.Wrapper.GetSharePointVersion(App.CurrentConnection.SiteUrl);
                if (spVersion == Enumerations.SharePointVersions.SP2010)
                {
                    sb.AppendLine("ValidateConnection Method, checking version and if SP2010, via CSOM is possible but with restrictions");
                    App.CurrentConnection.InstallationType = Recent.TargetInstallationType.SP2010;
                }
            }

            sb.AppendLine(string.Format("ValidateConnection Method, is the url valid? {0}", isValid.ToString()));
            return isValid;
        }
    }
}

