using CamlDesigner.SharePoint.Common;
using CamlDesigner2013.CamlEventHandler;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


namespace CamlDesigner2013.Controls
{
    /// <summary>
    /// Interaction logic for SiteColumnControl.xaml
    /// </summary>
    public partial class SiteColumnControl : UserControl
    {

        private DataTypeTextControl dataTypeTextControl = null;

        public SiteColumnControl()
        {
            InitializeComponent();
            this.PopulateDataTypeComboBox();
            //this.PopulateSiteColumnGroups();
        }

        public event CAMLEventHandler CAMLEvent;

        private CAMLDesigner.BusinessObjects.Wrapper CamlWrapper
        {
            get
            {
                if (App.CamlWrapper == null)
                {
                    App.CamlWrapper = new CAMLDesigner.BusinessObjects.Wrapper(
                        App.GeneralInformation.SharePointUrl,
                        App.GeneralInformation.ConnectionType);
                }
                return App.CamlWrapper;
            }
        }

        private void DataTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataTypeComboBox.SelectedIndex > 0)
            {
                string selectedDataType = DataTypeComboBox.Items[DataTypeComboBox.SelectedIndex].ToString();
                switch (selectedDataType)
                {
                    case "Single line of text":
                        this.dataTypeTextControl = new DataTypeTextControl();
                        DataTypeStackPanel.Children.Clear();
                        DataTypeStackPanel.Children.Add(this.dataTypeTextControl);
                        break;
                    case "Multiple lines of text":
                        break;
                }
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataTypeComboBox.SelectedItem != null)
            {
                string selectedDataType = DataTypeComboBox.SelectedItem as string;
                string datatype =  UtilityFunctionsCAML.GetCAMLDataType(selectedDataType);
                string groupName = null;
                if ((bool)ExistingGroupRadioButton.IsChecked && GroupComboBox.SelectedIndex > -1)
                    groupName = GroupComboBox.SelectedItem as string;
                else
                    groupName = GroupTextBox.Text;

                string camlstring = null;
                switch (selectedDataType)
                {
                    case "Single line of text":
                        camlstring =  Builder.GenerateTextSiteColumn(
                            ColumnNameTextBlock.Text,
                            ColumnDisplayNameTextBlock.Text,
                            datatype,
                            DescriptionTextBlock.Text,
                            groupName,
                            (bool)RequiredRadioButton.IsChecked,
                            (bool)AllowDeletionCheckBox.IsChecked,
                            (bool)ShowInFileDlgCheckBox.IsChecked,
                            (bool)ShowInDisplayFormCheckBox.IsChecked,
                            (bool)ShowInNewFormCheckBox.IsChecked,
                            (bool)ShowInEditFormCheckBox.IsChecked,
                            (bool)ShowInViewsFormCheckBox.IsChecked,
                            (bool)ShowInListSettingsCheckBox.IsChecked,
                            (bool)ShowInVersionHistoryCheckBox.IsChecked,
                            this.dataTypeTextControl.MaximumNumberOfCharacters,
                            this.dataTypeTextControl.EnforceUniqueValues);
                        break;

                    case "Multiple lines of text":
                        break;
                }
                // TODO
                // if (CAMLEvent != null && !string.IsNullOrEmpty(camlstring))
                //    CAMLEvent(camlstring);
            }
        }

        private void PopulateDataTypeComboBox()
        {
            DataTypeComboBox.Items.Clear();
            DataTypeComboBox.Items.Add("-- select a data type --");
            DataTypeComboBox.Items.Add("Single line of text");
            DataTypeComboBox.Items.Add("Multiple lines of text");
        }

        //private void PopulateSiteColumnGroups()
        //{
        //    GroupComboBox.Items.Clear();

        //    List<string> groups = this.CamlWrapper.GetSiteColumnGroups();

        //    if (groups != null)
        //        GroupComboBox.ItemsSource = groups;
        //    else
        //    {
        //        // TODO: gray out the existing groups combobox
        //    }
        //}
    }
}
