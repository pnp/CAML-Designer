using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using CamlDesigner2013.Helpers;
using  CamlDesigner.Common;
using CamlDesigner.SharePoint.Objects;
using CamlDesigner.SharePoint.Common;

namespace CamlDesigner2013.Controls
{
    /// <summary>
    /// Interaction logic for FieldControl.xaml
    /// </summary>
    public partial class FieldControl : UserControl
    {
        //public event ViewFieldEventHandler ViewFieldSelectedEvent;
        public event OrderByFieldEventHandler OrderByFieldSelectedEvent;
        public event WhereFieldEventHandler WhereFieldSelectedEvent;
        public event GroupByFieldEventHandler GroupByFieldSelectedEvent;
        // these controls must be class level variables because they need to be made visible later on
        private CheckBox offsetCheckBox;
        private StackPanel offsetStackPanel;
        private Control valueControl;
        private TextBlock messageControl;

        private TextBox userTextBox = null;
        private ComboBox membershipComboBox = null;
        private TextBlock membershipTextBlock = null;
        private ComboBox groupComboBox = null;

        // variables for the Where controls
        private bool isTimeControlsAdded = false;
        private double dateTimeControlHeight = 95.0;
        private string selectedOperator;
        private TaxonomyWindow taxonomyWindow = null;

        public FieldControl(Field field)
        {
            InitializeComponent();
            this.SelectedField = field;
            InitializeFieldControl();

        }

        #region Public Properties
        public Field SelectedField { get; set; }



        private ViewField viewField;

        public ViewField ViewField
        {
            get { return this.viewField; }
            set { this.viewField = value; }
        }

        private OrderByField orderByField;

        public OrderByField OrderByField
        {
            get { return this.orderByField; }
            set { this.orderByField = value; }
        }

        private GroupByField groupByField;

        public GroupByField GroupByField
        {
            get { return this.groupByField; }
            set { this.groupByField = value; }
        }

        private WhereField whereField;

        public WhereField WhereField
        {
            get { return this.whereField; }
            set { this.whereField = value; }
        }

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

        #endregion

        private void InitializeFieldControl()
        {
            if (string.IsNullOrEmpty(SelectedField.AuthoringInfo))
            {
                //DisplayNameTextBlock.Text = field.DisplayName;
                FieldTile.Title = SelectedField.DisplayName;
                FieldTile.ToolTip = SelectedField.DisplayName;
            }
            else
            {
                //DisplayNameTextBlock.Text = field.DisplayName;
                FieldTile.Title = SelectedField.DisplayName;
                FieldTile.ToolTip = SelectedField.DisplayName + " " + SelectedField.AuthoringInfo;
            }

            // Fill the other controls
            ViewFieldTile.Title = FieldTile.Title;
            ViewFieldTile.ToolTip = FieldTile.ToolTip;
            OrderByFieldTile.Title = FieldTile.Title;
            OrderByFieldTile.ToolTip = FieldTile.ToolTip;
            GroupByTile.Title = FieldTile.Title;
            GroupByTile.ToolTip = FieldTile.ToolTip;
            if (SelectedField.DisplayName.Length <= 15)
                WhereFieldTile.Title = FieldTile.Title;
            else
                WhereFieldTile.Title = SelectedField.DisplayName.Substring(0, 15);
            WhereFieldTile.ToolTip = FieldTile.ToolTip;

        }

        private void SortorderImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.orderByField.SortOrder ==  CamlDesigner.Common.Enumerations.SortOrder.Ascending)
            {
                this.orderByField.SortOrder =  CamlDesigner.Common.Enumerations.SortOrder.Descending;
                this.LoadImage(SortOrderImage, "../Images/descending_32x32.png");
            }
            else
            {
                this.orderByField.SortOrder =  CamlDesigner.Common.Enumerations.SortOrder.Ascending;
                this.LoadImage(SortOrderImage, "../Images/ascending_32x32.png");
            }

            // Fire CAML event
            if (this.OrderByFieldSelectedEvent != null)
                this.OrderByFieldSelectedEvent(this.orderByField);
        }

        private void LoadImage(Image image, string uristring)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(uristring, UriKind.Relative);
            img.EndInit();

            SortOrderImage.Source = img;
        }

        #region helper methods to build up the Where field UX

        public void SetValueStackPanel(FieldControl fieldControl)
        {
            // set the width and height of the parent control
            this.Width = 400.0;
            this.Height = 50.0;

            // the controls in the ValueStackPanel differ based on the data type of the selected field
            fieldControl.ValueStackPanel.Children.Clear();
            switch (fieldControl.WhereField.Field.DataType)
            {
                case "Boolean":
                    CreateControlsForBooleanField();
                    break;
                case "DateTime":
                    CreateControlsForDateTimeField();
                    break;
                case "Lookup":
                    CreateControlsForLookupField();
                    break;
                case "LookupMulti":
                    CreateControlsForLookupField();
                    break;
                case "Choice":
                    CreateControlsForChoiceField(false);
                    break;
                case "MultiChoice":
                    CreateControlsForChoiceField(true);
                    break;
                case "User":
                case "UserMulti":
                    CreateControlsForUserField();
                    break;
                case "TaxonomyFieldType":
                case "TaxonomyFieldTypeMulti":
                    CreateControlsForTaxonomyField();
                    break;
                default:
                    CreateControlsForDefaultBehavior();
                    break;
            }
        }

        #region Text field methods
        private void CreateControlsForDefaultBehavior()
        {
            // set the child controls
            TextBox textbox = new TextBox();
            textbox.Width = 180.0;
            textbox.Margin = new Thickness(0, 3, 0, 0);
            textbox.TextChanged += new TextChangedEventHandler(this.textbox_TextChanged);
            // set the value
            if (WhereField.Values != null && WhereField.Values[0] != null)
                textbox.Text = (string)WhereField.Values[0];
            ValueStackPanel.Children.Add(textbox);
            TextBlock messageTextBlock = new TextBlock();
            messageTextBlock.Margin = new Thickness(0, 2, 0, 0);
            messageTextBlock.Text = Application.Current.Resources["FieldControlMessageTextBlock"].ToString();
            messageTextBlock.TextWrapping = TextWrapping.Wrap;
            messageTextBlock.Foreground = new SolidColorBrush(UI.GetColorFromHexString("#FF293955"));
            messageTextBlock.Width = 180.0;
            messageTextBlock.Visibility = Visibility.Collapsed;
            ValueStackPanel.Children.Add(messageTextBlock);
            this.valueControl = textbox;
            this.messageControl = messageTextBlock;
        }
        #endregion

        #region Boolean field methods
        private void CreateControlsForBooleanField()
        {
            // set the parent control
            this.Height = 50.0;
            this.Width = 400.0;
            // set the child controls
            CheckBox checkbox = new CheckBox();
            checkbox.Margin = new Thickness(0, 8, 0, 0);
            checkbox.Content = "True";
            //checkbox.Foreground = new SolidColorBrush(Colors.Ivory);
            //checkbox.Background = new SolidColorBrush(Colors.SlateGray);
            checkbox.Click += new RoutedEventHandler(this.checkbox_Click);
            ValueStackPanel.Children.Add(checkbox);
            this.valueControl = checkbox;

            // set the value
            if (WhereField.Values != null)
                checkbox.IsChecked = (bool)WhereField.Values[0];
            else
                this.AddWhereFieldValue("False");

            // fire the event to modify the CAML query
            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(WhereField);
        }
        #endregion

        #region DateTime field methods
        private void CreateControlsForDateTimeField()
        {
            // Today checkbox
            RadioButton todayRadioButton = new RadioButton();
            todayRadioButton.Margin = new Thickness(0, 3, 0, 0);
            todayRadioButton.Content = Application.Current.Resources["FieldControlDateFieldToday"].ToString();
            todayRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            todayRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            todayRadioButton.Click += new RoutedEventHandler(this.todayRadioButton_Click);
            ValueStackPanel.Children.Add(todayRadioButton);
            // Generate offset panel
            GenerateOffsetPanel();
            // Specific date RadioButton
            RadioButton dateRadioButton = new RadioButton();
            dateRadioButton.Margin = new Thickness(0, 10, 0, 0);
            dateRadioButton.Content = Application.Current.Resources["FieldControlDateFieldSpecificDate"].ToString();
            dateRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            dateRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            dateRadioButton.Click += new RoutedEventHandler(this.dateRadioButton_Click);
            ValueStackPanel.Children.Add(dateRadioButton);
            //resize control
            this.ResizeControl(this.dateTimeControlHeight);
            // set the value
            if (WhereField.Values != null)
            {
                if (WhereField.OptionalDateParameter ==  CamlDesigner.Common.Enumerations.OptionalDateParameters.Today)
                    todayRadioButton.IsChecked = true;
                else if (WhereField.OptionalDateParameter ==  CamlDesigner.Common.Enumerations.OptionalDateParameters.SpecificDate)
                {
                    dateRadioButton.IsChecked = true;
                    CreateCalendarControl();
                }
            }
        }

        private void GenerateOffsetPanel()
        {
            this.offsetCheckBox = new CheckBox();
            this.offsetCheckBox.Margin = new Thickness(20, 10, 0, 0);
            this.offsetCheckBox.Content = Application.Current.Resources["FieldControlDateFieldOffSet"].ToString();
            this.offsetCheckBox.Foreground = new SolidColorBrush(Colors.Ivory);
            this.offsetCheckBox.Background = new SolidColorBrush(Colors.SlateGray);
            this.offsetCheckBox.Click += new RoutedEventHandler(this.offsetCheckBox_Click);
            this.offsetCheckBox.Visibility = System.Windows.Visibility.Collapsed;
            ValueStackPanel.Children.Add(this.offsetCheckBox);

            this.offsetStackPanel = new StackPanel();
            this.offsetStackPanel.Margin = new Thickness(30, 10, 0, 0);
            this.offsetStackPanel.Orientation = Orientation.Horizontal;
            this.offsetStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            ValueStackPanel.Children.Add(this.offsetStackPanel);

            ComboBox offsetComboBox = new ComboBox();
            //Populate the combobox
            ComboBoxItem item1 = new ComboBoxItem();
            item1.Content = "+";
            offsetComboBox.Items.Add(item1);
            ComboBoxItem item2 = new ComboBoxItem();
            item2.Content = "-";
            offsetComboBox.Items.Add(item2);
            offsetComboBox.SelectionChanged += new SelectionChangedEventHandler(this.offsetComboBox_SelectionChanged);
            this.offsetStackPanel.Children.Add(offsetComboBox);

            TextBox offsetTextBox = new TextBox();
            offsetTextBox.Margin = new Thickness(10, 10, 0, 0);
            offsetTextBox.Width = 50.0;
            offsetTextBox.TextChanged += new TextChangedEventHandler(this.offsetTextBox_TextChanged);
            this.offsetStackPanel.Children.Add(offsetTextBox);

            TextBlock offsetTextBlock = new TextBlock();
            offsetTextBlock.Margin = new Thickness(0, 10, 0, 0);
            offsetTextBlock.Text = Application.Current.Resources["FieldControlDateFieldOffSetTextBlock"].ToString();
            offsetTextBlock.Foreground = new SolidColorBrush(Colors.Ivory);
            this.offsetStackPanel.Children.Add(offsetTextBlock);

            if (WhereField.IncludeOffset)
            {
                offsetCheckBox.IsChecked = true;
                offsetStackPanel.Visibility = System.Windows.Visibility.Visible;
                this.dateTimeControlHeight += 50.0;
                this.ResizeControl(this.dateTimeControlHeight);

                if (WhereField.OffsetValue.Sign == null || WhereField.OffsetValue.Sign == "+")
                    offsetComboBox.SelectedIndex = 0;
                else
                    offsetComboBox.SelectedIndex = 1;
                offsetTextBox.Text = WhereField.OffsetValue.Value.ToString();
            }
        }

        void CreateCalendarControl()
        {
            Calendar calendar = new Calendar();
            calendar.Margin = new Thickness(0, 5, 0, 0);
            calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(this.calendar_SelectedDatesChanged);
            ValueStackPanel.Children.Add(calendar);
            this.valueControl = calendar;

            if (WhereField.Values != null && WhereField.Values[0] is DateTime)
                calendar.SelectedDate = WhereField.Values[0] as DateTime?;
            else
                calendar.SelectedDate = DateTime.Today;

            CheckBox includeTimeCheckBox = new CheckBox();
            includeTimeCheckBox.Margin = new Thickness(0, 10, 0, 0);
            includeTimeCheckBox.Content = Application.Current.Resources["FieldControlDateFieldIncludeTime"].ToString();
            includeTimeCheckBox.Foreground = new SolidColorBrush(Colors.SlateGray);
            includeTimeCheckBox.Background = new SolidColorBrush(Colors.SlateGray);
            includeTimeCheckBox.Click += new RoutedEventHandler(this.includeTimeCheckBox_Click);
            ValueStackPanel.Children.Add(includeTimeCheckBox);
            this.dateTimeControlHeight += 190.0;
            this.ResizeControl(this.dateTimeControlHeight);

            if (WhereField.IncludeTimeValue)
            {
                includeTimeCheckBox.IsChecked = true;
                CreateIncludeTimeControls();
            }
        }

        void CreateIncludeTimeControls()
        {
            isTimeControlsAdded = true;
            TextBox timeTextBox = new TextBox();
            timeTextBox.Width = 100.0;
            timeTextBox.Margin = new Thickness(15, 5, 0, 0);
            timeTextBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            timeTextBox.TextChanged += new TextChangedEventHandler(this.timeTextBox_TextChanged);
            ValueStackPanel.Children.Add(timeTextBox);

            TextBlock timeLabel = new TextBlock();
            timeLabel.Text = Application.Current.Resources["FieldControlDateFieldTimeLabelFormat"].ToString();
            timeLabel.Foreground = new SolidColorBrush(Colors.SlateGray);
            timeLabel.Margin = new Thickness(15, 0, 0, 0);
            ValueStackPanel.Children.Add(timeLabel);
            this.dateTimeControlHeight += 40.0;
            this.ResizeControl(this.dateTimeControlHeight);

            if (WhereField.IncludeTimeValue && !string.IsNullOrEmpty(WhereField.TimeValue))
                timeTextBox.Text = WhereField.TimeValue;
        }

        void todayRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                WhereField.AddValue("Today");
                WhereField.OptionalDateParameter =  CamlDesigner.Common.Enumerations.OptionalDateParameters.Today;

                // display Offset controls
                this.offsetCheckBox.Visibility = System.Windows.Visibility.Visible;
                this.dateTimeControlHeight += 25.0;
                this.ResizeControl(this.dateTimeControlHeight);

                // TODO: hide the specific date controls

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(WhereField);
            }
        }

        void offsetCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
            {
                WhereField.IncludeOffset = (bool)((CheckBox)sender).IsChecked;

                // display Offset panel
                this.offsetStackPanel.Visibility = System.Windows.Visibility.Visible;

                this.dateTimeControlHeight += 50.0;
                this.ResizeControl(this.dateTimeControlHeight);

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(WhereField);
            }
        }

        void offsetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                if (WhereField.OffsetValue == null)
                    WhereField.OffsetValue = new OffsetValue();
                string sign = ((ComboBoxItem)((ComboBox)sender).SelectedValue).Content.ToString();
                if (!string.IsNullOrEmpty(sign))
                    WhereField.OffsetValue.Sign = sign;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(WhereField);
            }
        }

        void offsetTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                string offsetString = ((TextBox)sender).Text;
                int offsetValue = 0;
                int.TryParse(offsetString, out offsetValue);
                if (offsetValue > 0)
                {
                    if (WhereField.OffsetValue == null)
                        WhereField.OffsetValue = new OffsetValue();
                    WhereField.OffsetValue.Value = offsetValue;

                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent(WhereField);
                }
            }
        }

        void dateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            // Calendar control
            CreateCalendarControl();

            // TODO: hide the controls below the Today checkbox

        }


        void includeTimeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
            {
                WhereField.IncludeTimeValue = (bool)((CheckBox)sender).IsChecked;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(WhereField);

                if ((bool)((CheckBox)sender).IsChecked && !this.isTimeControlsAdded)
                {
                    CreateIncludeTimeControls();
                }
            }
        }

        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Calendar)
            {
                this.AddWhereFieldValue(((Calendar)sender).SelectedDate);
                WhereField.OptionalDateParameter =  CamlDesigner.Common.Enumerations.OptionalDateParameters.SpecificDate;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(WhereField);
            }
        }
        void timeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
                WhereField.TimeValue = ((TextBox)sender).Text;

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(WhereField);
        }

        #endregion

        #region Lookup field methods
        private void CreateControlsForLookupField()
        {
            ListBox lookupListBox = new ListBox();
            if (WhereField.WhereOperator == "In")
                lookupListBox.SelectionMode = SelectionMode.Multiple;
            lookupListBox.Margin = new Thickness(0, 5, 0, 0);
            lookupListBox.Height = 80.0;
            this.PopulateLookupListControl(lookupListBox);
            lookupListBox.SelectionChanged += new SelectionChangedEventHandler(this.listbox_SelectionChanged);
            ValueStackPanel.Children.Add(lookupListBox);
            this.valueControl = lookupListBox;

            if (WhereField.Field.InternalName == "FileRef")
            {
                this.ResizeControl(100.0);
            }
            else
            {
                CheckBox lookupCheckBox = new CheckBox();
                lookupCheckBox.Margin = new Thickness(0, 10, 0, 0);
                lookupCheckBox.Content = Application.Current.Resources["FieldControlLookupCheckBox"].ToString();
                lookupCheckBox.Foreground = new SolidColorBrush(Colors.Ivory);
                lookupCheckBox.Background = new SolidColorBrush(Colors.SlateGray);
                lookupCheckBox.Click += new RoutedEventHandler(this.lookupCheckBox_Click);
                if (WhereField.Values != null)
                    lookupCheckBox.IsChecked = WhereField.ByLookupId;
                ValueStackPanel.Children.Add(lookupCheckBox);
                this.ResizeControl(120.0);
            }
        }

        void lookupCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
                WhereField.ByLookupId = (bool)((CheckBox)sender).IsChecked;

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(WhereField);
        }

        private void PopulateLookupListControl(Control listcontrol)
        {
            List<LookupValue> lookupValues = GetLookupValues(
                ((LookupField)WhereField.Field).LookupListName,
                ((LookupField)WhereField.Field).ShowField,
                ((LookupField)WhereField.Field).LookupWebId);
            if (lookupValues != null)
            {
                if (listcontrol is ComboBox)
                {
                    ComboBox combobox = listcontrol as ComboBox;
                    combobox.Items.Clear();
                    combobox.ItemsSource = lookupValues;
                    combobox.DisplayMemberPath = "Value";
                    combobox.SelectedValuePath = "ID";

                }
                else if (listcontrol is ListBox)
                {
                    ListBox listbox = listcontrol as ListBox;
                    listbox.Items.Clear();
                    listbox.ItemsSource = lookupValues;
                    listbox.DisplayMemberPath = "Value";
                    listbox.SelectedValuePath = "ID";
                }

                SetValuesForLookupListControl(listcontrol);
            }
        }

        private void SetValuesForLookupListControl(Control listcontrol)
        {
            // set the value
            // if operator differs from in, it will contain only one value
            if (WhereField.Values != null)
            {
                foreach (object value in WhereField.Values)
                {
                    string valuestring = null;
                    if (value is LookupValue)
                        valuestring = ((LookupValue)value).Value;
                    else
                        valuestring = value.ToString();

                    foreach (var item in ((ListBox)listcontrol).Items)
                    {
                        if (((LookupValue)item).Value == valuestring)
                        {
                            if (listcontrol is ListBox)
                            {
                                if (((ListBox)listcontrol).SelectionMode == SelectionMode.Multiple)
                                    ((ListBox)listcontrol).SelectedItems.Add(item);
                                else
                                    ((ListBox)listcontrol).SelectedItem = item;
                            }
                            else if (listcontrol is ComboBox)
                            {
                                ((ComboBox)listcontrol).SelectedItem = item;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Choice field methods
        private void CreateControlsForChoiceField(bool isMultiSelect)
        {
            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(0, 5, 0, 0);
            listbox.Height = 60.0;
            if (isMultiSelect && WhereField.WhereOperator == "In")
                listbox.SelectionMode = SelectionMode.Multiple;
            this.PopulateChoiceListControl(listbox);
            this.ResizeControl(75.0);
            listbox.SelectionChanged += new SelectionChangedEventHandler(this.multiChoiceListBox_SelectionChanged);
            ValueStackPanel.Children.Add(listbox);
            this.valueControl = listbox;
        }

        void choiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                if (((ListBox)sender).SelectionMode == SelectionMode.Single || ((ListBox)sender).SelectedItems == null)
                {
                    this.AddWhereFieldValue(((ListBox)sender).SelectedItem.ToString());
                }
                else
                {
                    WhereField.Values = null;
                    foreach (object value in ((ListBox)sender).SelectedItems)
                    {
                        this.AddWhereFieldValue(value.ToString());
                    }
                }
            }

            if (this.WhereFieldSelectedEvent != null)
                this.WhereFieldSelectedEvent(WhereField);
        }

        void multiChoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                if (((ListBox)sender).SelectionMode == SelectionMode.Single || ((ListBox)sender).SelectedItems == null)
                {
                    this.AddWhereFieldValue(((ListBox)sender).SelectedItem.ToString());
                }
                else
                {
                    WhereField.Values = null;
                    foreach (object value in ((ListBox)sender).SelectedItems)
                    {
                        this.AddWhereFieldValue(value.ToString());
                    }
                }
            }

            if (this.WhereFieldSelectedEvent != null)
                this.WhereFieldSelectedEvent(WhereField);
        }

        private void PopulateChoiceListControl(Control listcontrol)
        {
            if (WhereField.Field is ChoiceField)
            {
                if (listcontrol is ComboBox)
                {
                    ComboBox combobox = listcontrol as ComboBox;
                    combobox.Items.Clear();
                    foreach (string choice in ((ChoiceField)WhereField.Field).Choices)
                    {
                        combobox.Items.Add(choice);
                    }
                }
                else if (listcontrol is ListBox)
                {
                    ListBox listbox = listcontrol as ListBox;
                    listbox.Items.Clear();
                    foreach (string choice in ((ChoiceField)WhereField.Field).Choices)
                    {
                        listbox.Items.Add(choice);
                    }
                }

                SetValuesForChoiceListControl(listcontrol);
            }
        }

        private void SetValuesForChoiceListControl(Control listcontrol)
        {
            // set the value
            // if operator differs from in, it will contain only one value
            if (WhereField.Values != null)
            {
                foreach (string value in WhereField.Values)
                {
                    foreach (var item in ((ListBox)listcontrol).Items)
                    {
                        if (item.ToString() == value)
                        {
                            if (listcontrol is ListBox)
                            {
                                if (((ListBox)listcontrol).SelectionMode == SelectionMode.Multiple)
                                    ((ListBox)listcontrol).SelectedItems.Add(item);
                                else
                                    ((ListBox)listcontrol).SelectedItem = item;
                            }
                            else if (listcontrol is ComboBox)
                            {
                                ((ComboBox)listcontrol).SelectedItem = item;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region User field methods
        private void CreateControlsForUserField()
        {
            // Radio button for current user
            RadioButton currentUserRadioButton = new RadioButton();
            currentUserRadioButton.Margin = new Thickness(0, 3, 0, 0);
            currentUserRadioButton.Name = "CurrentUserRadioButton";
            currentUserRadioButton.Content = Application.Current.Resources["FieldControlCurrentUser"].ToString();
            currentUserRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            currentUserRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            currentUserRadioButton.Click += new RoutedEventHandler(this.userRadioButton_Click);
            ValueStackPanel.Children.Add(currentUserRadioButton);
            // Radio button for specific user
            RadioButton userRadioButton = new RadioButton();
            userRadioButton.Name = "userRadioButton";
            userRadioButton.Margin = new Thickness(0, 10, 0, 0);
            userRadioButton.Content = Application.Current.Resources["FieldControlSpecificUser"].ToString();
            userRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            userRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            userRadioButton.Click += new RoutedEventHandler(this.userRadioButton_Click);
            ValueStackPanel.Children.Add(userRadioButton);
            // create the user text box but make it hidden
            CreateUserTextBox();
            // Radio button for membership
            RadioButton mshipRadioButton = new RadioButton();
            mshipRadioButton.Name = "membershipRadioButton";
            mshipRadioButton.Margin = new Thickness(0, 10, 0, 0);
            mshipRadioButton.Content = Application.Current.Resources["FieldControlMembership"].ToString();
            mshipRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            mshipRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            mshipRadioButton.Click += new RoutedEventHandler(this.userRadioButton_Click);
            ValueStackPanel.Children.Add(mshipRadioButton);
            // create the membership combobox but make it hidden
            CreateMembershipControls();
            this.ResizeControl(90.0);

            if (WhereField.Values != null && WhereField.Values[0] != null)
            {
                if (WhereField.Values[0].ToString() == "UserID")
                    currentUserRadioButton.IsChecked = true;
                else
                {
                    userRadioButton.IsChecked = true;
                    CreateUserTextBox();
                }
            }
        }

        void userRadioButton_Click(object sender, RoutedEventArgs e)
        {
            double andOrHeight = 0.0;

            if (whereField != null && whereField.PositionInList > 0)
                andOrHeight = 40.0;

            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                RadioButton radio = (RadioButton)sender;
                if (radio.Name == "CurrentUserRadioButton")
                {
                    if (userTextBox != null)
                        userTextBox.Visibility = Visibility.Collapsed;
                    if (membershipComboBox != null)
                        membershipComboBox.Visibility = Visibility.Collapsed;
                    if (membershipTextBlock != null)
                        membershipTextBlock.Visibility = Visibility.Collapsed;
                    if (groupComboBox != null)
                        groupComboBox.Visibility = Visibility.Collapsed;

                    this.ResizeControl(90.0 + andOrHeight);

                    this.AddWhereFieldValue("UserID");

                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent(WhereField);
                }
                else if (radio.Name == "membershipRadioButton")
                {
                    if (userTextBox != null)
                        userTextBox.Visibility = Visibility.Collapsed;
                    if (membershipComboBox != null)
                        membershipComboBox.Visibility = Visibility.Visible;
                    if (membershipTextBlock != null)
                        membershipTextBlock.Visibility = Visibility.Visible;

                    this.ResizeControl(115.0 + andOrHeight);

                    if (membershipComboBox.SelectedIndex >= 0)
                    {
                        ComboBoxItem selectedItem = membershipComboBox.SelectedItem as ComboBoxItem;

                        if (selectedItem.Content.ToString() == "SPGroup")
                        {
                            groupComboBox.Visibility = Visibility.Visible;

                            this.ResizeControl(175.0 + andOrHeight);

                            string groupvalue = string.Empty;
                            if (groupComboBox.SelectedIndex >= 0)
                            {
                                ComboBoxItem selectedGroupItem = groupComboBox.SelectedItem as ComboBoxItem;
                                if (selectedGroupItem.Name.StartsWith("ID"))
                                    groupvalue = selectedGroupItem.Name.Substring(2);
                            }
                            this.AddWhereFieldValue(selectedItem.Content.ToString() + groupvalue);
                        }
                        else
                        {
                            groupComboBox.Visibility = Visibility.Collapsed;

                            this.ResizeControl(155.0 + andOrHeight);

                            this.AddWhereFieldValue(selectedItem.Content.ToString());
                        }

                        if (WhereFieldSelectedEvent != null)
                            WhereFieldSelectedEvent(WhereField);
                    }
                }
                else
                {
                    if (userTextBox != null)
                        userTextBox.Visibility = Visibility.Visible;
                    if (membershipComboBox != null)
                        membershipComboBox.Visibility = Visibility.Collapsed;
                    if (membershipTextBlock != null)
                        membershipTextBlock.Visibility = Visibility.Collapsed;
                    if (groupComboBox != null)
                        groupComboBox.Visibility = Visibility.Collapsed;

                    this.ResizeControl(115.0 + andOrHeight);

                    if (userTextBox.Text.Length > 0)
                    {
                        this.AddWhereFieldValue(userTextBox.Text);
                        if (WhereFieldSelectedEvent != null)
                            WhereFieldSelectedEvent(WhereField);
                    }
                }
            }
        }

        private void CreateUserTextBox()
        {
            userTextBox = new TextBox();
            userTextBox.Width = 180.0;
            userTextBox.Margin = new Thickness(15, 5, 0, 0);
            userTextBox.Visibility = System.Windows.Visibility.Collapsed;
            userTextBox.TextChanged += new TextChangedEventHandler(this.userTextBox_TextChanged);
            ValueStackPanel.Children.Add(userTextBox);

            if (WhereField.Values != null && WhereField.Values[0] != null)
                userTextBox.Text = WhereField.Values[0].ToString();
        }

        private void CreateMembershipControls()
        {
            membershipComboBox = new ComboBox();
            membershipComboBox.Width = 180.0;
            membershipComboBox.Margin = new Thickness(15, 5, 0, 0);
            PopulateMembershipComboBox();
            membershipComboBox.Visibility = System.Windows.Visibility.Collapsed;
            membershipComboBox.SelectionChanged += new SelectionChangedEventHandler(this.membershipComboBox_SelectionChanged);
            ValueStackPanel.Children.Add(membershipComboBox);

            groupComboBox = new ComboBox();
            groupComboBox.Width = 180.0;
            groupComboBox.Margin = new Thickness(15, 5, 0, 0);
            PopulateGroupsComboBox();
            groupComboBox.Visibility = System.Windows.Visibility.Collapsed;
            groupComboBox.SelectionChanged += new SelectionChangedEventHandler(this.groupComboBox_SelectionChanged);
            ValueStackPanel.Children.Add(groupComboBox);

            membershipTextBlock = new TextBlock();
            membershipTextBlock.Width = 180.0;
            membershipTextBlock.Margin = new Thickness(15, 5, 0, 0);
            membershipTextBlock.TextWrapping = TextWrapping.Wrap;
            membershipTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            ValueStackPanel.Children.Add(membershipTextBlock);
        }

        private void PopulateMembershipComboBox()
        {
            if (membershipComboBox != null)
            {
                ComboBoxItem item1 = new ComboBoxItem();
                item1.Content = "CurrentUserGroups";    // Assigned to groups to which current user belongs
                item1.Name = "CurrentUserGroups";
                membershipComboBox.Items.Add(item1);
                ComboBoxItem item2 = new ComboBoxItem();
                item2.Content = "SPGroup";              // Assigned to a specific group
                item2.Name = "SPGroup";
                membershipComboBox.Items.Add(item2);
                ComboBoxItem item3 = new ComboBoxItem();
                item3.Content = "SPWeb.Groups";         // Assigned to groups
                item3.Name = "SPWebGroups";
                membershipComboBox.Items.Add(item3);
                ComboBoxItem item4 = new ComboBoxItem();
                item4.Content = "SPWeb.AllUsers";
                item4.Name = "SPWebAllUsers";
                membershipComboBox.Items.Add(item4);
                ComboBoxItem item5 = new ComboBoxItem();
                item5.Content = "SPWeb.Users";
                item5.Name = "SPWebUsers";
                membershipComboBox.Items.Add(item5);
            }
        }

        private void PopulateGroupsComboBox()
        {
            List<GroupValue> groups = this.CamlWrapper.GetGroups();

            if (groups != null)
            {
                foreach (GroupValue group in groups)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = group.Name;
                    item.Name = "ID" + group.ID.ToString();
                    groupComboBox.Items.Add(item);
                }
            }
        }

        void userTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                if (WhereField.Values == null)
                    WhereField.AddValue(((TextBox)sender).Text);
                else
                    WhereField.Values[0] = ((TextBox)sender).Text;
            }

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(WhereField);
        }

        void membershipComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox && ((ComboBox)sender).SelectedIndex > -1)
            {
                ComboBoxItem selectedItem = ((ComboBox)sender).SelectedItem as ComboBoxItem;
                string value = selectedItem.Content.ToString();

                switch (value)
                {
                    case "CurrentUserGroups":
                        membershipTextBlock.Text = "Tasks assigned to current user or to groups to which current user belongs";
                        break;
                    case "SPGroup":
                        membershipTextBlock.Text = "Tasks assigned to members of this group";
                        break;
                    case "SPWeb.Groups":
                        membershipTextBlock.Text = "Tasks assigned to groups";
                        break;
                    case "SPWeb.AllUsers":
                        membershipTextBlock.Text = "Tasks assigned directly to users";
                        break;
                    case "SPWeb.Users":
                        membershipTextBlock.Text = "Tasks assigned to users who have received rights to the site directly (not through a group).";
                        break;
                    default:
                        membershipTextBlock.Text = string.Empty;
                        break;
                }

                double andOrHeight = 0.0;

                if (whereField != null && whereField.PositionInList > 0)
                    andOrHeight = 40.0;

                if (value == "SPGroup")
                {
                    groupComboBox.Visibility = System.Windows.Visibility.Visible;
                    this.ResizeControl(175.0 + andOrHeight);
                }
                else
                {
                    groupComboBox.Visibility = System.Windows.Visibility.Collapsed;
                    this.ResizeControl(155.0 + andOrHeight);
                }

                if (WhereField.Values == null)
                    WhereField.AddValue(value);
                else
                    WhereField.Values[0] = value;
            }

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(WhereField);
        }

        void groupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox && ((ComboBox)sender).SelectedIndex > -1)
            {
                ComboBoxItem selectedItem = ((ComboBox)sender).SelectedItem as ComboBoxItem;
                string value = string.Empty;
                if (selectedItem.Name.StartsWith("ID"))
                    value = selectedItem.Name.Substring(2);

                if (WhereField.Values == null)
                    WhereField.AddValue(selectedItem.Name);
                else
                    WhereField.Values[0] = "SPGroup" + value;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(WhereField);
            }
        }

        #endregion

        #region Taxonomy field methods
        private void CreateControlsForTaxonomyField()
        {
            StackPanel taxonomyStackPanel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            TextBox taxonomyTextBox = new TextBox() { Width = 140.0, Height = 24.0, Margin = new Thickness(0, 10, 0, 0) };
            taxonomyTextBox.TextChanged += new TextChangedEventHandler(textbox_TextChanged);
            taxonomyStackPanel.Children.Add(taxonomyTextBox);

            if (App.GeneralInformation.ConnectionType ==  CamlDesigner.Common.Enumerations.ApiConnectionType.WebServices
                || (App.GeneralInformation.ConnectionType ==  CamlDesigner.Common.Enumerations.ApiConnectionType.ClientObjectModel
                    && App.GeneralInformation.SharePointVersion ==  CamlDesigner.Common.Enumerations.SharePointVersions.SP2010))
            {
                // Do nothing
            }
            else
            {
                // initialize a button to show a popup with a term set

                Button taxonomyButton = new Button()
                {
                    Width = 35.0,
                    Height = 30.0,
                    Margin = new Thickness(5, 10, 0, 0),
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(Colors.SlateGray),
                    Content = "..."
                };

                taxonomyButton.Click += new RoutedEventHandler(taxonomyButton_Click);
                taxonomyStackPanel.Children.Add(taxonomyButton);
            }
            ValueStackPanel.Children.Add(taxonomyStackPanel);

            CheckBox lookupCheckBox = new CheckBox();
            lookupCheckBox.Margin = new Thickness(0, 10, 0, 0);
            lookupCheckBox.Content = Application.Current.Resources["FieldControlLookupCheckBox"].ToString();
            lookupCheckBox.Foreground = new SolidColorBrush(Colors.Ivory);
            lookupCheckBox.Background = new SolidColorBrush(Colors.SlateGray);
            lookupCheckBox.Click += new RoutedEventHandler(wssIdCheckBox_Click);
            if (WhereField.Values != null)
                lookupCheckBox.IsChecked = WhereField.ByLookupId;
            ValueStackPanel.Children.Add(lookupCheckBox);

            TextBlock tmessageTextBlock = new TextBlock()
            {
                Margin = new Thickness(0, 10, 0, 0),
                Text = Application.Current.Resources["FieldControlMessageTextBlock"].ToString(),
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(UI.GetColorFromHexString("#FF293955")),
                Width = 180.0,
                Visibility = Visibility.Collapsed
            };

            ValueStackPanel.Children.Add(tmessageTextBlock);
            valueControl = taxonomyTextBox;
            messageControl = tmessageTextBlock;

            this.ResizeControl(75.0);
        }

        void wssIdCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
            {
                WhereField.ByLookupId = (bool)((CheckBox)sender).IsChecked;

                // change the value in the textbox
                TextBox textbox = valueControl as TextBox;
                string convertedValue = string.Empty;

                if (textbox != null)
                {
                    if (taxonomyWindow != null && taxonomyWindow.TaxonomyValues != null)
                    {
                        // in this case the taxonomy window was used or a term has already been retrieved from the server
                        foreach (TaxonomyValue value in taxonomyWindow.TaxonomyValues)
                        {
                            if (WhereField.ByLookupId)
                            {
                                // the value in the text box should be a term
                                if (value.Value == textbox.Text)
                                {
                                    convertedValue = UtilityFunctions.FormatIntegerArrayToString(value.WssIds);
                                }
                            }
                            else
                            {
                                // the value in the text box should be an integer representing a term
                                string wssIdsString = UtilityFunctions.FormatIntegerArrayToString(value.WssIds);
                                if (wssIdsString == textbox.Text)
                                {
                                    convertedValue = value.Value;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(convertedValue))
                    {
                        convertedValue = GetTaxonomyValue(WhereField, textbox.Text);
                    }

                    // TODO: to be complete, check if value is string or integer
                    textbox.Text = convertedValue;
                }
            }
        }

        void taxonomyButton_Click(object sender, RoutedEventArgs e)
        {
            // get the taxonomy
            List<TaxonomyValue> values = GetTaxonomyValues(((TaxonomyField)WhereField.Field).TermStoreId, ((TaxonomyField)WhereField.Field).TermSetId);

            if (values != null)
            {
                bool multiSelect = WhereField.Field.DataType == "TaxonomyFieldTypeMulti";
                taxonomyWindow = new TaxonomyWindow(values, multiSelect, WhereField.WhereOperator);
                Nullable<bool> dialogResult = taxonomyWindow.ShowDialog();
                ProcessTaxonomyValues();
            }
            else
            {
                System.Windows.MessageBox.Show(string.Format(Application.Current.Resources["FieldControlManagedMetadataWarning"].ToString(),
                     WhereField.Field.DisplayName, ((TaxonomyField)WhereField.Field).TermStoreId, ((TaxonomyField)WhereField.Field).TermSetId));
            }
        }

        private string GetTaxonomyValue(WhereField whereField, string input)
        {
            // get the taxonomy
            TaxonomyValue value = this.CamlWrapper.GetTaxonomyValue(((TaxonomyField)WhereField.Field).TermStoreId, ((TaxonomyField)WhereField.Field).TermSetId, input);


            if (value != null && value.ID != Guid.Empty)
            {
                if (WhereField.ByLookupId)
                {
                    // retrieve the WssId
                    if (value.WssIds == null || value.WssIds.Length == 0)
                    {
                        CustomMessageBox.Show("No WSS Ids have been found... Term is not yet used in a list or library.  Check '/lists/taxonomyhiddenlist' ", "An error has occured", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    return UtilityFunctions.FormatIntegerArrayToString(value.WssIds);
                }
                else
                {
                    // retrieve the label
                    return value.Value;
                }
            }
            else
            {
                CustomMessageBox.Show(value.Value, "An error has occured", MessageBoxButton.OK, MessageBoxImage.Error);
                return value.Value;
            }
        }

        private void ProcessTaxonomyValues()
        {
            if (taxonomyWindow.SelectedValues != null && taxonomyWindow.SelectedValues.Count > 0)
            {
                TextBox textbox = valueControl as TextBox;

                if (((TaxonomyField)WhereField.Field).MultiSelect)
                {
                    string valuestring = string.Empty;
                    // KBOS 4/1/2015: not the right place to empty this collection, but it solves the bug of switching between Eq and In in case of already selected value
                    WhereField.Values = null;
                    foreach (TaxonomyValue value in taxonomyWindow.SelectedValues)
                    {
                        if (valuestring.Length > 0)
                            valuestring += ";";

                        if (WhereField.ByLookupId)
                            valuestring += value.WssIds;
                        else
                            valuestring += value.Value;

                        AddWhereFieldValue(value);
                    }

                    if (textbox != null)
                        textbox.Text = valuestring;
                }
                else
                {
                    // in that case only take the first selected value
                    AddWhereFieldValue(taxonomyWindow.SelectedValues[0]);

                    if (textbox != null)
                    {
                        if (WhereField.ByLookupId)
                        {
                            textbox.Text = UtilityFunctions.FormatIntegerArrayToString(taxonomyWindow.SelectedValues[0].WssIds);
                        }
                        else
                            textbox.Text = taxonomyWindow.SelectedValues[0].Value;
                    }
                }

                // this event was already triggered by the change event of the text box
                //if (WhereFieldSelectedEvent != null)
                //{
                //    WhereFieldSelectedEvent(WhereField);
                //}
            }
        }

        #endregion

        #region Events that cause the CAML Query to fire
        void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text) && WhereField.Field.DataType != "TaxonomyFieldTypeMulti")
            {
                if (WhereField.Values == null)
                {
                    WhereField.AddValue(((TextBox)sender).Text);
                }
                else
                {
                    WhereField.Values[0] = ((TextBox)sender).Text;
                }
            }

            if (this.WhereFieldSelectedEvent != null)
            {
                this.WhereFieldSelectedEvent(WhereField);
            }
        }

        void checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                this.AddWhereFieldValue(((CheckBox)sender).IsChecked);
            }

            if (this.WhereFieldSelectedEvent != null)
            {
                this.WhereFieldSelectedEvent(WhereField);
            }
        }

        void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // first check which operator is used
            object selectedValue = null;

            if (sender is ListBox)
            {
                if (((ListBox)sender).SelectionMode == SelectionMode.Single || ((ListBox)sender).SelectedItems == null)
                {
                    selectedValue = (LookupValue)((ListBox)sender).SelectedItem;
                    this.AddWhereFieldValue(selectedValue);
                }
                else
                {
                    WhereField.Values = null;
                    foreach (object value in ((ListBox)sender).SelectedItems)
                    {
                        this.AddWhereFieldValue(value);
                    }
                }
            }
            else if (sender is ComboBox)
            {
                selectedValue = (LookupValue)((ComboBox)sender).SelectedItem;
                this.AddWhereFieldValue(selectedValue);
            }

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent(WhereField);
        }

        // TODO: deze methods horen misschien ergens anders: helper methods???
        internal List<LookupValue> GetLookupValues(string listName, string showField, Guid lookupWebId)
        {
            return this.CamlWrapper.GetLookupValues(listName, showField, lookupWebId);
        }

        internal List<TaxonomyValue> GetTaxonomyValues(Guid termStoreId, Guid termSetId)
        {
            return this.CamlWrapper.GetTaxonomyValues(termStoreId, termSetId);
        }

        #endregion

        private void AndOrControl_FieldOperatorEvent(string andOrOperator)
        {
            if (string.IsNullOrEmpty(andOrOperator) == false)
            {
                this.WhereField.AndOrOperator = andOrOperator;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent(whereField);
            }
        }

        private void OperatorControl_FieldOperatorEvent(string fieldOperator)
        {
            if (string.IsNullOrEmpty(fieldOperator) == false)
            {
                whereField.WhereOperator = fieldOperator;

                // set the Visibility of the Value StackPanel
                if (fieldOperator == "IsNull" || fieldOperator == "IsNotNull")
                    ValueStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                else
                    ValueStackPanel.Visibility = System.Windows.Visibility.Visible;

                // set the SelectionMode for list boxes
                if (fieldOperator == "In")
                {
                    // TODO: what with other controls?
                    if (valueControl != null)
                    {
                        if (valueControl is ListBox)
                            ((ListBox)valueControl).SelectionMode = SelectionMode.Multiple;

                        // if a ; separated string can be found in the value control, change it into a + sign
                        if (valueControl is TextBox)
                        {
                            ((TextBox)valueControl).Text = ((TextBox)valueControl).Text.Replace(';', '+');
                        }
                    }

                    if (this.messageControl != null)
                    {
                        this.messageControl.Visibility = Visibility.Visible;
                        this.ResizeControl(this.Height + 30.0);
                    }
                }
                else
                {
                    // TODO: what with other controls?
                    if (this.valueControl is ListBox)
                        ((ListBox)this.valueControl).SelectionMode = SelectionMode.Single;
                    if (this.messageControl != null && whereField.WhereOperator == "In")
                    {
                        // otherwise the box is made less heigh, even if the previous where operator was not "In"
                        (this.messageControl).Visibility = Visibility.Collapsed;
                        this.ResizeControl(this.Height - 20.0);
                    }
                }

                if (WhereField.Values != null || fieldOperator == "IsNull" || fieldOperator == "IsNotNull")
                {
                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent(whereField);
                }
            }
        }

        private void AddWhereFieldValue(object selectedValue)
        {
            if (selectedValue != null)
            {
                if (WhereField.WhereOperator == "In")
                {
                    WhereField.AddValue(selectedValue);
                }
                else
                {
                    if (WhereField.Values == null)
                        WhereField.AddValue(selectedValue);
                    else
                        WhereField.Values[0] = selectedValue;
                }
            }
        }

        public void ResizeControl(double height)
        {
            if (height >= 20)
            {
                this.Height = height;
                WhereFieldTile.Height = height;
            }
        }

        #endregion

        private void CollapseExpandGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Expand.Visibility == System.Windows.Visibility.Visible)
            {
                Expand.Visibility = System.Windows.Visibility.Hidden;
                Collapse.Visibility = System.Windows.Visibility.Visible;
                this.groupByField.Collapse = Enumerations.Collapse.True;
            }
            else
            {
                Expand.Visibility = System.Windows.Visibility.Visible;
                Collapse.Visibility = System.Windows.Visibility.Hidden;
                this.groupByField.Collapse = Enumerations.Collapse.False;
            }

            // Fire CAML event
            if (this.GroupByFieldSelectedEvent != null)
            {
                this.GroupByFieldSelectedEvent(this.groupByField);
            }
        }


    }
}
