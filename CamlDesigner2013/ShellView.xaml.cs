using CamlDesigner2013.Alerts;
using CamlDesigner2013.Controls;
using CamlDesigner2013.Helpers;
using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CamlDesigner2013
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : MetroWindow
    {

        //protected override void OnInitialized(EventArgs e)
        //{
        //    base.OnInitialized(e);
        //    App.SetMainWindow(this);
        //}

        bool ignoreNextMouseMove;

        public ShellView()
        {
            App.SetMainWindow(this);
            
            InitializeComponent();

            //ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Red"), Theme.Dark);
            // add an event to the ConnectionsView
            switch (Properties.Settings.Default.State)
            {
                case "Maximized":
                    WindowState = System.Windows.WindowState.Maximized;
                    break;
                case "Minimized":
                    WindowState = System.Windows.WindowState.Minimized;
                    break;
                case "Normal":
                    WindowState = System.Windows.WindowState.Normal;
                    this.window.Width = Properties.Settings.Default.Width;
                    this.window.Height = Properties.Settings.Default.Height;
                    break;
                default:
                    WindowState = System.Windows.WindowState.Maximized;
                    break;
            }
            Helpers.Connection.ConnectionEvent += new ConnectionEventHandler(EstablishConnection);
            this.MUISetter();
            this.Closed += ShellView_Closed;
            this.SizeChanged += ShellView_SizeChanged;
        }

        void ShellView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Properties.Settings.Default.Height = this.window.Height;
            Properties.Settings.Default.Width = this.window.Width;
        }

        private void ShellView_Closed(object sender, EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.Height = this.window.Height;
                Properties.Settings.Default.Width = this.window.Width;
            }
            Properties.Settings.Default.Save();
        }

        void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {

            if (e.MiddleButton == MouseButtonState.Pressed) return;
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var header = this.FindName("header") as Grid;


            if (!header.IsMouseOver) return;

            if (WindowState == WindowState.Maximized && e.ClickCount != 2) return;

            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    Properties.Settings.Default.State = WindowState.Normal.ToString();
                }
                else
                {
                    WindowState = WindowState.Maximized;
                    Properties.Settings.Default.State = WindowState.Maximized.ToString();
                }
                ignoreNextMouseMove = true;
                return;
            }

            DragMove();
        }

        void MouseMoveWindow(object sender, MouseEventArgs e)
        {
            if (ignoreNextMouseMove)
            {
                ignoreNextMouseMove = false;
                return;
            }

            if (WindowState != WindowState.Maximized) return;

            if (e.MiddleButton == MouseButtonState.Pressed) return;
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!header.IsMouseOver) return;

            // Calculate correct left coordinate for multi-screen system
            var mouseX = PointToScreen(Mouse.GetPosition(this)).X;
            var width = RestoreBounds.Width;
            var left = mouseX - width / 2;
            if (left < 0) left = 0;

            // Align left edge to fit the screen
            var virtualScreenWidth = SystemParameters.VirtualScreenWidth;
            if (left + width > virtualScreenWidth) left = virtualScreenWidth - width;

            Top = 0;
            Left = left;

            WindowState = WindowState.Normal;
            Properties.Settings.Default.State = WindowState.Normal.ToString();
            DragMove();
        }

        void ToggleMaximized()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                Properties.Settings.Default.State = WindowState.Normal.ToString();
            }
            else
            {
                WindowState = WindowState.Maximized;
                Properties.Settings.Default.State = WindowState.Maximized.ToString();
            }
        }

        void ShellViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!header.IsMouseOver) return;
            ToggleMaximized();
        }

        void ButtonMinimiseOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            Properties.Settings.Default.State = WindowState.Minimized.ToString();
        }

        void ButtonMaxRestoreOnClick(object sender, RoutedEventArgs e)
        {
            ToggleMaximized();
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            Flyouts[0].IsOpen = !Flyouts[0].IsOpen;
        }

        private void ConnectionsClick(object sender, RoutedEventArgs e)
        {
            Flyouts[1].IsOpen = !Flyouts[1].IsOpen;
            CamlDesigner2013.Connections.UI.ConnectionView connections = Flyouts[1].Content as Connections.UI.ConnectionView;
            connections.GetRecents();
        }

        protected override void OnStateChanged(System.EventArgs e)
        {
            RefreshMaximiseIconState();

            switch (WindowState)
            {
                case WindowState.Maximized:
                    Properties.Settings.Default.State = WindowState.Maximized.ToString();
                    break;
                case WindowState.Minimized:
                    Properties.Settings.Default.State = WindowState.Minimized.ToString();
                    break;
                case WindowState.Normal:
                    Properties.Settings.Default.State = WindowState.Normal.ToString();
                    Properties.Settings.Default.Height = this.window.Height;
                    Properties.Settings.Default.Width = this.window.Width;
                    break;
                default:
                    Properties.Settings.Default.State = WindowState.Normal.ToString();
                    break;
            }
            base.OnStateChanged(e);
        }

        void RefreshMaximiseIconState()
        {

        }

        void WindowDragOver(object sender, DragEventArgs e)
        {
            var isFileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);
            e.Effects = isFileDrop ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        }

        private void HelpClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.camldesigner.com/?p=594");
        }

        public void MUISetter()
        {
            ResourceDictionary dict = new ResourceDictionary();
            // if user has selected a default language and will not rely on the OS language
            if (Properties.Settings.Default.LanguageOverride)
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.LanguageSet);
            }

            switch (Thread.CurrentThread.CurrentUICulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("..\\Languages\\MUI.en-US.xaml", UriKind.Relative);
                    break;
                case "nl-NL":
                    dict.Source = new Uri("..\\Languages\\MUI.nl-NL.xaml", UriKind.Relative);
                    break;
                case "nl-BE":
                    dict.Source = new Uri("..\\Languages\\MUI.nl-NL.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Languages\\MUI.en-US.xaml", UriKind.Relative);
                    break;
            }

            // first resourceDictionary is mahapps stuff, so the second is the language dictionary
            if (Application.Current.Resources.MergedDictionaries.Count() == 2)
            {
                Application.Current.Resources.MergedDictionaries.RemoveAt(Application.Current.Resources.MergedDictionaries.Count() - 1);
            }
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public void ChangeVisibilityOfExecuteButton(bool isVisible)
        {
            if (isVisible)
            {
                ExecuteGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ExecuteGrid.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private async void EstablishConnection()
        {
            try
            {
                //closes the connection view
                Flyouts[1].IsOpen = !Flyouts[1].IsOpen;

                // sets the treeviews busy indicator
                MdiView.BusyTreeview.Visibility = System.Windows.Visibility.Visible;

                // following statement solves a bug when switching between connections
                App.SelectedListViewModel = null;

                // Calling the populate treeview method
                await this.PopulateListTreeViewAsync();

                // makes the treeview indicator hidden
                MdiView.BusyTreeview.Visibility = System.Windows.Visibility.Hidden;
            }
            catch
            {
                CustomMessageBox.Show(Application.Current.Resources["ConnectionsWindowInvalidCredentialsMsgBoxMessage"].ToString(), Application.Current.Resources["ConnectionsWindowEmtpyUrlMsgBoxTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
             
                // makes the treeview indicator hidden
                MdiView.BusyTreeview.Visibility = System.Windows.Visibility.Hidden;

                Flyouts[1].IsOpen = !Flyouts[1].IsOpen;
            }
        }

        private System.Threading.Tasks.Task PopulateListTreeViewAsync()
        {
            return System.Threading.Tasks.Task.Run(() =>
                {
                    MdiView.PopulateListsTreeview();
                });
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            SimpleAlert alert = new SimpleAlert();
            alert.Title = "Action";
            alert.Message = "Checking for update information";
            CheckForUpdate.InstallUpdateSyncWithInfo();
            alert = new SimpleAlert();
            alert.Title = "Action";
            alert.Message = "DONE Checking for update information";
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.MUISetter();
            MdiView.InitializeQueryControls();
            SimpleAlert alert = new SimpleAlert();
            alert.Title = "Action";
            alert.Message = "Refreshing the list";
            //TODO: add refresh procedure
        }

        private void btnTwitter_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://twitter.com/CamlDesigner");
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            MdiView.ExecuteCAMLQuery();
        }
    }
}
