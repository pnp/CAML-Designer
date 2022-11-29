using CamlDesigner2013.Alerts;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace CamlDesigner2013.Settings.UI
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            ClearHistory.Click += new RoutedEventHandler(ClearHistory_Click);
            OpenLogFileLocation.Click += OpenLogFileLocation_Click;
            OverruleLanguage.IsChecked = Properties.Settings.Default.LanguageOverride;
            foreach (ComboBoxItem item in Languages.Items)
            {
                if (item.Tag == item.Tag)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            if (Properties.Settings.Default.Contrast == "Light")
            {
                Light.IsChecked = true;
            }
            else
            {
                Dark.IsChecked = true;
            }

            foreach (ComboBoxItem colorItem in Color.Items)
            {
                if (colorItem.Content.ToString() == Properties.Settings.Default.Color.ToString())
                {
                    colorItem.IsSelected = true;
                    break;
                }
            }

            this.SetColor();
        }

        void OpenLogFileLocation_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\BIWUG\\CamlDesigner");
        }

        private void Light_Checked(object sender, RoutedEventArgs e)
        {
            Dark.IsChecked = false;
            Properties.Settings.Default.Contrast = "Light";
            Properties.Settings.Default.Save();
        }

        private void MiLightRed()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Red"), Theme.Light);
        }

        private void MiDarkRed()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Red"), Theme.Dark);
        }

        private void MiLightGreen()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Green"), Theme.Light);
        }

        private void MiDarkGreen()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Green"), Theme.Dark);
        }

        private void MiLightBlue()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Light);
        }

        private void MiDarkBlue()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Dark);
        }

        private void MiLightPurple()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Purple"), Theme.Light);
        }

        private void MiDarkPurple()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Purple"), Theme.Dark);
        }

        private void MiDarkOrange()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Dark);
        }

        private void MiLightOrange()
        {
            ThemeManager.ChangeTheme(App.mainWindow, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Light);
        }

        private void Color_DropDownClosed(object sender, EventArgs e)
        {
            this.SetColor();
        }

        private void SetColor()
        {
            if (Light.IsChecked ?? true)
            {
                switch (Color.Text)
                {
                    case "Green":
                        Properties.Settings.Default.Color = "Green";
                        Properties.Settings.Default.Save();
                        this.MiLightGreen();
                        break;
                    case "Red":
                        Properties.Settings.Default.Color = "Red";
                        Properties.Settings.Default.Save();
                        this.MiLightRed();
                        break;
                    case "Blue":
                        Properties.Settings.Default.Color = "Blue";
                        Properties.Settings.Default.Save();
                        this.MiLightBlue();
                        break;
                    case "Orange":
                        Properties.Settings.Default.Color = "Orange";
                        Properties.Settings.Default.Save();
                        this.MiLightOrange();
                        break;
                    case "Purple":
                        Properties.Settings.Default.Color = "Purple";
                        Properties.Settings.Default.Save();
                        this.MiLightPurple();
                        break;
                    default:
                        break;
                }
            }
            else if (Dark.IsChecked ?? true)
            {
                switch (Color.Text)
                {
                    case "Green":
                        Properties.Settings.Default.Color = "Green";
                        Properties.Settings.Default.Save();
                        this.MiDarkGreen();
                        break;
                    case "Red":
                        Properties.Settings.Default.Color = "Red";
                        Properties.Settings.Default.Save();
                        this.MiDarkRed();
                        break;
                    case "Blue":
                        Properties.Settings.Default.Color = "Blue";
                        Properties.Settings.Default.Save();
                        this.MiDarkBlue();
                        break;
                    case "Orange":
                        Properties.Settings.Default.Color = "Orange";
                        Properties.Settings.Default.Save();
                        this.MiDarkOrange();
                        break;
                    case "Purple":
                        Properties.Settings.Default.Color = "Purple";
                        Properties.Settings.Default.Save();
                        this.MiDarkPurple();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Dark_Checked(object sender, RoutedEventArgs e)
        {
            Light.IsChecked = false;
            Properties.Settings.Default.Contrast = "Dark";
            Properties.Settings.Default.Save();
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            Helpers.IO.ClearRecent();
            SimpleAlert simpleAlert = new SimpleAlert();
            simpleAlert.Title = "Action";
            simpleAlert.Message = "Recent has been cleared";

        }


        private void OverruleLanguage_Checked(object sender, RoutedEventArgs e)
        {
            ChooseLanguage.Visibility = System.Windows.Visibility.Visible;
            Properties.Settings.Default.LanguageOverride = true;
        }

        private void OverruleLanguage_Unchecked(object sender, RoutedEventArgs e)
        {
            ChooseLanguage.Visibility = System.Windows.Visibility.Collapsed;
            Properties.Settings.Default.LanguageOverride = false;
        }

        private void Languages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = Languages.SelectedItem as ComboBoxItem;
            if (this.IsMouseCaptureWithin)
            {
                if (item != null)
                {
                    Properties.Settings.Default.LanguageSet = item.Tag.ToString();
                    Properties.Settings.Default.Save();
                    App.mainWindow.MUISetter();
                }
            }
        }

        private void Debug_Checked(object sender, RoutedEventArgs e)
        {
            App.mdiView.ShowDebug(System.Windows.Visibility.Visible);
        }

        private void Debug_Unchecked(object sender, RoutedEventArgs e)
        {
            App.mdiView.ShowDebug(System.Windows.Visibility.Hidden);
        }
    }
}
