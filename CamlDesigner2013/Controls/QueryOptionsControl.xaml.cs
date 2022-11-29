using CamlDesigner.SharePoint.Objects;
using CamlDesigner2013.Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CamlDesigner2013.Controls
{
    /// <summary>
    /// Interaction logic for QueryOptionsControl.xaml
    /// </summary>
    public partial class QueryOptionsControl : UserControl
    {
        private QueryOptions queryOptions;

        public QueryOptionsControl()
        {
            InitializeComponent();

            this.queryOptions = new QueryOptions();

            // some query options are not available in the Client Object Model(s)
            if (App.ConnectionType ==  CamlDesigner.Common.Enumerations.ApiConnectionType.ClientObjectModel)
            {
                ExpandUserCheckBox.IsEnabled = false;
                IncludeMandatoryColumnsCheckBox.IsEnabled = false;
            }
            else
            {
                ExpandUserCheckBox.IsEnabled = true;
                IncludeMandatoryColumnsCheckBox.IsEnabled = true;
            }

            // initialize certain query options
            PopulateScopeComboBox();
        }

        public event QueryOptionsEventHandler QueryOptionsEvent;

        public void Clear()
        {
            IncludeMandatoryColumnsCheckBox.IsChecked = false;
            UTCDateCheckBox.IsChecked = false;
            FilesAndFoldersCheckBox.IsChecked = false;
            SubFoldersCheckBox.IsChecked = false;
            RowLimitCheckBox.IsChecked = false;
            ExpandUserCheckBox.IsChecked = false;
            IncludeAttachmentUrlsCheckBox.IsChecked = false;

            SubFolderTextBox.Text = string.Empty;
            RowLimitTextBox.Text = string.Empty;
            RowLimitTextBox.Visibility = System.Windows.Visibility.Collapsed;

            FilesAndFoldersPanel.Visibility = System.Windows.Visibility.Collapsed;

            this.queryOptions.ViewFieldsOnly = false;
            this.queryOptions.ExpandUserField = false;
            this.queryOptions.IncludeAttachmentUrls = false;
            this.queryOptions.IncludeAttachmentVersion = false;
            this.queryOptions.IncludeMandatoryColumns = false;
            this.queryOptions.WorkWithFilesAndFolders = false;
            this.queryOptions.QueryFilesAllFoldersDeep = false;
            this.queryOptions.QueryFilesAndFoldersAllFoldersDeep = false;
            this.queryOptions.QueryFilesAndFoldersInRootFolder = false;
            this.queryOptions.QueryFilesAndFoldersInSubFolder = false;
            this.queryOptions.QueryFilesAndFoldersInSubFolderDeep = false;
            this.queryOptions.QueryFilesInRootFolder = false;
            this.queryOptions.QueryFilesInSubFolder = false;
            this.queryOptions.QueryFilesInSubFolderDeep = false;
            this.queryOptions.QueryFoldersAllFoldersDeep = false;
            this.queryOptions.QueryFoldersInRootFolder = false;
            this.queryOptions.RowLimit = 0;
            this.queryOptions.SubFolder = string.Empty;
            this.queryOptions.UtcDate = false;

            this.Reset();
        }

        public void Reset()
        {
            // depending on the selected query type, a different StackPanel needs to be shown
            switch (App.QueryType)
            {
                case  CamlDesigner.Common.Enumerations.QueryType.CAMLQuery:
                    QueryOptionsPanel.Visibility = System.Windows.Visibility.Visible;
                    SiteDataQueryOptionsPanel.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case  CamlDesigner.Common.Enumerations.QueryType.SiteDataQuery:
                    QueryOptionsPanel.Visibility = System.Windows.Visibility.Collapsed;
                    SiteDataQueryOptionsPanel.Visibility = System.Windows.Visibility.Visible;
                    if (App.SelectedListViewModel != null)
                    {
                        ListTemplateTextBox.Text = App.SelectedListViewModel.TemplateID.ToString();
                    }
                    break;
            }
        }

        private void PopulateScopeComboBox()
        {
            ScopeComboBox.Items.Clear();

            List< CamlDesigner.Common.Enumerations.SiteDataQueryScope> comboList = new List< CamlDesigner.Common.Enumerations.SiteDataQueryScope>();

            comboList.Add(new  CamlDesigner.Common.Enumerations.SiteDataQueryScope("Site collection", "SiteCollection"));
            comboList.Add(new  CamlDesigner.Common.Enumerations.SiteDataQueryScope("Site", "Site"));
            comboList.Add(new  CamlDesigner.Common.Enumerations.SiteDataQueryScope("Site and sub sites", "Recursive"));

            ScopeComboBox.DisplayMemberPath = "ScopeName";
            ScopeComboBox.SelectedValuePath = "ScopeValue";
            ScopeComboBox.ItemsSource = comboList;
        }

        private void ViewFieldsOnly_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
            {
                App.mainObject.ViewFieldsOnly = (bool)((CheckBox)sender).IsChecked;
                this.queryOptions.ViewFieldsOnly = (bool)((CheckBox)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
                this.QueryOptionsEvent(this.queryOptions);
        }

        #region CAML Query event handlers

        private void IncludeMandatoryColumnsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.IncludeMandatoryColumns = false;
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
                this.queryOptions.IncludeMandatoryColumns = (bool)((CheckBox)sender).IsChecked;

            if (this.QueryOptionsEvent != null)
                this.QueryOptionsEvent(this.queryOptions);
        }

        private void UTCDateCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.UtcDate = false;
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
                this.queryOptions.UtcDate = (bool)((CheckBox)sender).IsChecked;

            if (this.QueryOptionsEvent != null)
                this.QueryOptionsEvent(this.queryOptions);
        }

        private void RowLimitCheckBox_Click(object sender, RoutedEventArgs e)
        {
            RowLimitTextBox.Visibility = System.Windows.Visibility.Collapsed;
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null && (bool)((CheckBox)sender).IsChecked)
            {
                RowLimitTextBox.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.queryOptions.RowLimit = 0;

                if (this.QueryOptionsEvent != null)
                    this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void RowLimitTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                int rowLimit = 0;
                int.TryParse(((TextBox)sender).Text, out rowLimit);

                if (rowLimit > 0)
                {
                    this.queryOptions.RowLimit = rowLimit;

                    if (this.QueryOptionsEvent != null)
                        this.QueryOptionsEvent(this.queryOptions);
                }
            }
        }

        private void IncludeAttachmentUrlsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.IncludeAttachmentUrls = false;
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
                this.queryOptions.IncludeAttachmentUrls = (bool)((CheckBox)sender).IsChecked;

            if (this.QueryOptionsEvent != null)
                this.QueryOptionsEvent(this.queryOptions);
        }

        private void IncludeAttachmentVersionCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.IncludeAttachmentVersion = false;
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
                this.queryOptions.IncludeAttachmentVersion = (bool)((CheckBox)sender).IsChecked;

            if (this.QueryOptionsEvent != null)
                this.QueryOptionsEvent(this.queryOptions);
        }

        private void ExpandUserCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.ExpandUserField = false;
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
            {
                this.queryOptions.ExpandUserField = (bool)((CheckBox)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void FilesAndFoldersCheckBox_Click(object sender, RoutedEventArgs e)
        {
            // this event handler should only make the stackpanel visible or not
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
            {
                if ((bool)((CheckBox)sender).IsChecked)
                {
                    queryOptions.WorkWithFilesAndFolders = true;
                    FilesAndFoldersPanel.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    queryOptions.WorkWithFilesAndFolders = false;
                    FilesAndFoldersPanel.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void AllFilesAndFoldersRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResetQueryOptions(false);
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFilesAndFoldersAllFoldersDeep = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void AllFoldersRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResetQueryOptions(false);
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFoldersAllFoldersDeep = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void AllFilesRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResetQueryOptions(false);
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFilesAllFoldersDeep = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void FilesAndFoldersRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResetQueryOptions(false);
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFilesAndFoldersInRootFolder = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void FilesOnlyRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResetQueryOptions(false);
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFilesInRootFolder = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void FoldersOnlyRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResetQueryOptions(false);
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFoldersInRootFolder = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void SubFoldersCheckBox_Click(object sender, RoutedEventArgs e)
        {
            SubFolderStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null && (bool)((CheckBox)sender).IsChecked)
            {
                // queryOptions.QueryFilesInFolder = true;
                this.ResetQueryOptions(true);
                SubFolderStackPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.ResetQueryOptions(false);
                SubFolderTextBox.Text = string.Empty;
            }
        }

        private void SubFolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                this.queryOptions.SubFolder = ((TextBox)sender).Text;

                // if (this.QueryOptionsEvent != null)
                // this.QueryOptionsEvent(queryOptions);
            }
        }

        private void SubFolderOnlyRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.QueryFilesAndFoldersInSubFolder = false;
            this.queryOptions.QueryFilesInSubFolderDeep = false;
            this.queryOptions.QueryFilesAndFoldersInSubFolderDeep = false;
            this.queryOptions.QueryFilesInSubFolder = false;
            this.queryOptions.QueryFoldersInSubFolder = false;

            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFilesAndFoldersInSubFolder = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void SubFolderFilesOnlyRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.QueryFilesAndFoldersInSubFolder = false;
            this.queryOptions.QueryFilesInSubFolderDeep = false;
            this.queryOptions.QueryFilesAndFoldersInSubFolderDeep = false;
            this.queryOptions.QueryFilesInSubFolder = false;
            this.queryOptions.QueryFoldersInSubFolder = false;

            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFilesInSubFolder = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void SubFolderFoldersOnlyRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.QueryFilesAndFoldersInSubFolder = false;
            this.queryOptions.QueryFilesInSubFolderDeep = false;
            this.queryOptions.QueryFilesAndFoldersInSubFolderDeep = false;
            this.queryOptions.QueryFilesInSubFolder = false;
            this.queryOptions.QueryFoldersInSubFolder = false;

            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFoldersInSubFolder = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void SubFolderDeepRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.QueryFilesAndFoldersInSubFolder = false;
            this.queryOptions.QueryFilesInSubFolderDeep = false;
            this.queryOptions.QueryFilesAndFoldersInSubFolderDeep = false;
            this.queryOptions.QueryFilesInSubFolder = false;
            this.queryOptions.QueryFoldersInSubFolder = false;

            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFilesInSubFolderDeep = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void SubFolderDeepFilesAndFoldersRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.queryOptions.QueryFilesAndFoldersInSubFolder = false;
            this.queryOptions.QueryFilesInSubFolderDeep = false;
            this.queryOptions.QueryFilesAndFoldersInSubFolderDeep = false;
            this.queryOptions.QueryFilesInSubFolder = false;
            this.queryOptions.QueryFoldersInSubFolder = false;

            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                this.queryOptions.QueryFilesAndFoldersInSubFolderDeep = (bool)((RadioButton)sender).IsChecked;
            }

            if (this.QueryOptionsEvent != null)
            {
                this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void ResetQueryOptions(bool isSubFolderCheckBoxChecked)
        {
            this.queryOptions.QueryFilesAndFoldersAllFoldersDeep = false;
            this.queryOptions.QueryFilesAllFoldersDeep = false;
            this.queryOptions.QueryFoldersAllFoldersDeep = false;
            this.queryOptions.QueryFilesAndFoldersInRootFolder = false;
            this.queryOptions.QueryFilesInRootFolder = false;
            this.queryOptions.QueryFoldersInRootFolder = false;
            this.queryOptions.QueryFilesAndFoldersInSubFolder = false;
            this.queryOptions.QueryFilesInSubFolder = false;
            this.queryOptions.QueryFilesInSubFolderDeep = false;
            this.queryOptions.QueryFilesAndFoldersInSubFolderDeep = false;
            this.queryOptions.SubFolder = string.Empty;
            SubFoldersCheckBox.IsChecked = isSubFolderCheckBoxChecked;
        }

        #endregion

        #region SiteDataQuery event handlers
        private void ListTemplateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListTemplateTextBox.Text.Length > 0)
            {
                this.queryOptions.ListTemplate = ListTemplateTextBox.Text;

                if (this.QueryOptionsEvent != null)
                    this.QueryOptionsEvent(this.queryOptions);
            }
        }

        private void ScopeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScopeComboBox.SelectedIndex > -1)
            {
                this.queryOptions.WebScope = ScopeComboBox.SelectedValue.ToString();

                if (this.QueryOptionsEvent != null)
                    this.QueryOptionsEvent(this.queryOptions);
            }
        }
        #endregion
    }
}
