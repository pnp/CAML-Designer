using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using BPoint.SharePoint.Objects;
using CamlDesigner2013.Helpers;

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

        // these controls must be class level variables because they need to be made visible later on
        private CheckBox offsetCheckBox;
        private StackPanel offsetStackPanel;
        private Control valueControl;
        private TextBlock messageControl;

        // variables for the Where controls
        private bool isTimeControlsAdded;
        private double dateTimeControlHeight = 95.0;
        private string selectedOperator;

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
                //DisplayNameTextBlock.Text = field.DisplayName + " " + field.AuthoringInfo;
                FieldTile.Title = SelectedField.DisplayName + " " + SelectedField.AuthoringInfo;
                FieldTile.ToolTip = SelectedField.DisplayName + " " + SelectedField.AuthoringInfo;
            }

            // Fill the other controls
            ViewFieldTile.Title = FieldTile.Title;
            ViewFieldTile.ToolTip = FieldTile.ToolTip;
            OrderByFieldTile.Title = FieldTile.Title;
            OrderByFieldTile.ToolTip = FieldTile.ToolTip;
            WhereFieldTile.Title = FieldTile.Title;
            WhereFieldTile.ToolTip = FieldTile.ToolTip;
        }

        private void SortorderImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.orderByField.SortOrder == BPoint.SharePoint.Common.Enumerations.SortOrder.Ascending)
            {
                this.orderByField.SortOrder = BPoint.SharePoint.Common.Enumerations.SortOrder.Descending;
                this.LoadImage(SortOrderImage, "../Images/descending_32x32.png");
            }
            else
            {
                this.orderByField.SortOrder = BPoint.SharePoint.Common.Enumerations.SortOrder.Ascending;
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
            messageTextBlock.Text = "Separate values with a + sign, ex: titel 1+title 2+titel 3";
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
            todayRadioButton.Content = "Today";
            todayRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            todayRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            todayRadioButton.Click += new RoutedEventHandler(this.todayRadioButton_Click);
            ValueStackPanel.Children.Add(todayRadioButton);
            // Generate offset panel
            GenerateOffsetPanel();
            // Specific date RadioButton
            RadioButton dateRadioButton = new RadioButton();
            dateRadioButton.Margin = new Thickness(0, 10, 0, 0);
            dateRadioButton.Content = "Specific date";
            dateRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            dateRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            dateRadioButton.Click += new RoutedEventHandler(this.dateRadioButton_Click);
            ValueStackPanel.Children.Add(dateRadioButton);
            //resize control
            this.ResizeControl(this.dateTimeControlHeight);
            // set the value
            if (WhereField.Values != null)
            {
                if (WhereField.OptionalDateParameter == BPoint.SharePoint.Common.Enumerations.OptionalDateParameters.Today)
                    todayRadioButton.IsChecked = true;
                else if (WhereField.OptionalDateParameter == BPoint.SharePoint.Common.Enumerations.OptionalDateParameters.SpecificDate)
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
            this.offsetCheckBox.Content = " Add an offset to today";
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
            offsetTextBlock.Text = " days";
            offsetTextBlock.Foreground = new SolidColorBrush(Colors.Ivory);
            this.offsetStackPanel.Children.Add(offsetTextBlock);

            if ( WhereField.IncludeOffset)
            {
                offsetCheckBox.IsChecked = true;
                offsetStackPanel.Visibility = System.Windows.Visibility.Visible;
                this.dateTimeControlHeight += 50.0;
                this.ResizeControl(this.dateTimeControlHeight);

                if ( WhereField.OffsetValue.Sign == null ||  WhereField.OffsetValue.Sign == "+")
                    offsetComboBox.SelectedIndex = 0;
                else
                    offsetComboBox.SelectedIndex = 1;
                offsetTextBox.Text =  WhereField.OffsetValue.Value.ToString();
            }
        }

        void CreateCalendarControl()
        {
            Calendar calendar = new Calendar();
            calendar.Margin = new Thickness(0, 5, 0, 0);
            calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(this.calendar_SelectedDatesChanged);
             ValueStackPanel.Children.Add(calendar);
            this.valueControl = calendar;

            if ( WhereField.Values != null &&  WhereField.Values[0] is DateTime)
                calendar.SelectedDate =  WhereField.Values[0] as DateTime?;
            else
                calendar.SelectedDate = DateTime.Today;

            CheckBox includeTimeCheckBox = new CheckBox();
            includeTimeCheckBox.Margin = new Thickness(0, 10, 0, 0);
            includeTimeCheckBox.Content = "Include Time Value";
            includeTimeCheckBox.Foreground = new SolidColorBrush(Colors.SlateGray);
            includeTimeCheckBox.Background = new SolidColorBrush(Colors.SlateGray);
            includeTimeCheckBox.Click += new RoutedEventHandler(this.includeTimeCheckBox_Click);
             ValueStackPanel.Children.Add(includeTimeCheckBox);
            this.dateTimeControlHeight += 190.0;
            this.ResizeControl(this.dateTimeControlHeight);

            if ( WhereField.IncludeTimeValue)
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
            timeLabel.Text = "Format: xx:xx:xx";
            timeLabel.Foreground = new SolidColorBrush(Colors.SlateGray);
            timeLabel.Margin = new Thickness(15, 0, 0, 0);
             ValueStackPanel.Children.Add(timeLabel);
            this.dateTimeControlHeight += 40.0;
            this.ResizeControl(this.dateTimeControlHeight);

            if ( WhereField.IncludeTimeValue && !string.IsNullOrEmpty( WhereField.TimeValue))
                timeTextBox.Text =  WhereField.TimeValue;
        }

        void todayRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                 WhereField.AddValue("Today");
                 WhereField.OptionalDateParameter = BPoint.SharePoint.Common.Enumerations.OptionalDateParameters.Today;

                // display Offset controls
                this.offsetCheckBox.Visibility = System.Windows.Visibility.Visible;
                this.dateTimeControlHeight += 25.0;
                this.ResizeControl(this.dateTimeControlHeight);

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent( WhereField);
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
                    WhereFieldSelectedEvent( WhereField);
            }
        }

        void offsetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                if ( WhereField.OffsetValue == null)
                     WhereField.OffsetValue = new OffsetValue();
                string sign = ((ComboBoxItem)((ComboBox)sender).SelectedValue).Content.ToString();
                if (!string.IsNullOrEmpty(sign))
                     WhereField.OffsetValue.Sign = sign;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent( WhereField);
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
                    if ( WhereField.OffsetValue == null)
                         WhereField.OffsetValue = new OffsetValue();
                     WhereField.OffsetValue.Value = offsetValue;

                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent( WhereField);
                }
            }
        }

        void dateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            // Calendar control
            CreateCalendarControl();
        }


        void includeTimeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
            {
                 WhereField.IncludeTimeValue = (bool)((CheckBox)sender).IsChecked;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent( WhereField);

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
                 WhereField.OptionalDateParameter = BPoint.SharePoint.Common.Enumerations.OptionalDateParameters.SpecificDate;

                if (WhereFieldSelectedEvent != null)
                    WhereFieldSelectedEvent( WhereField);
            }
        }
        void timeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
                 WhereField.TimeValue = ((TextBox)sender).Text;

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent( WhereField);
        }

        #endregion

        #region Lookup field methods
        private void CreateControlsForLookupField()
        {
            ListBox lookupListBox = new ListBox();
            if ( WhereField.WhereOperator == "In")
                lookupListBox.SelectionMode = SelectionMode.Multiple;
            lookupListBox.Margin = new Thickness(0, 5, 0, 0);
            lookupListBox.Height = 60.0;
            this.PopulateLookupListControl(lookupListBox);
            lookupListBox.SelectionChanged += new SelectionChangedEventHandler(this.listbox_SelectionChanged);
             ValueStackPanel.Children.Add(lookupListBox);
            this.valueControl = lookupListBox;

            CheckBox lookupCheckBox = new CheckBox();
            lookupCheckBox.Margin = new Thickness(0, 10, 0, 0);
            lookupCheckBox.Content = "Query by ID";
            lookupCheckBox.Foreground = new SolidColorBrush(Colors.Ivory);
            lookupCheckBox.Background = new SolidColorBrush(Colors.SlateGray);
            lookupCheckBox.Click += new RoutedEventHandler(this.lookupCheckBox_Click);
            if ( WhereField.Values != null)
                lookupCheckBox.IsChecked =  WhereField.ByLookupId;
             ValueStackPanel.Children.Add(lookupCheckBox);
            this.ResizeControl(100.0);
        }

        void lookupCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox && ((CheckBox)sender).IsChecked != null)
                 WhereField.ByLookupId = (bool)((CheckBox)sender).IsChecked;

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent( WhereField);
        }

        private void PopulateLookupListControl(Control listcontrol)
        {
                List<LookupValue> lookupValues = GetLookupValues(
                    ((LookupField) WhereField.Field).LookupList, 
                    ((LookupField) WhereField.Field).ShowField,
                    ((LookupField) WhereField.Field).LookupWebId);
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
            if ( WhereField.Values != null)
            {
                foreach (object value in  WhereField.Values)
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
            if (isMultiSelect &&  WhereField.WhereOperator == "In")
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
                this.WhereFieldSelectedEvent( WhereField);
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
                this.WhereFieldSelectedEvent( WhereField);
        }

        private void PopulateChoiceListControl(Control listcontrol)
        {
            if ( WhereField.Field is ChoiceField)
            {
                if (listcontrol is ComboBox)
                {
                    ComboBox combobox = listcontrol as ComboBox;
                    combobox.Items.Clear();
                    foreach (string choice in ((ChoiceField) WhereField.Field).Choices)
                    {
                        combobox.Items.Add(choice);
                    }
                }
                else if (listcontrol is ListBox)
                {
                    ListBox listbox = listcontrol as ListBox;
                    listbox.Items.Clear();
                    foreach (string choice in ((ChoiceField) WhereField.Field).Choices)
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
            if ( WhereField.Values != null)
            {
                foreach (string value in  WhereField.Values)
                {
                    foreach (var item in ((ListBox)listcontrol).Items)
                    {
                        if (item == value)
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
            currentUserRadioButton.Content = "Current user";
            currentUserRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            currentUserRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            currentUserRadioButton.Click += new RoutedEventHandler(this.userRadioButton_Click);
             ValueStackPanel.Children.Add(currentUserRadioButton);
            // Radio button for specific user
            RadioButton userRadioButton = new RadioButton();
            userRadioButton.Margin = new Thickness(0, 10, 0, 0);
            userRadioButton.Content = "Specific user";
            userRadioButton.Foreground = new SolidColorBrush(Colors.Ivory);
            userRadioButton.Background = new SolidColorBrush(Colors.SlateGray);
            userRadioButton.Click += new RoutedEventHandler(this.userRadioButton_Click);
             ValueStackPanel.Children.Add(userRadioButton);
            this.ResizeControl(70.0);

            if ( WhereField.Values != null &&  WhereField.Values[0] != null)
            {
                if ( WhereField.Values[0].ToString() == "UserID")
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
            if (sender is RadioButton && ((RadioButton)sender).IsChecked != null)
            {
                RadioButton radio = (RadioButton)sender;
                if (radio.Name == "CurrentUserRadioButton")
                {
                    this.AddWhereFieldValue("UserID");

                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent( WhereField);
                }
                else
                {
                    CreateUserTextBox();
                }
            }
        }

        private void CreateUserTextBox()
        {
            TextBox userTextBox = new TextBox();
            userTextBox.Width = 180.0;
            userTextBox.Margin = new Thickness(0, 10, 0, 0);
            userTextBox.TextChanged += new TextChangedEventHandler(this.userTextBox_TextChanged);
             ValueStackPanel.Children.Add(userTextBox);
            this.ResizeControl(100.0);

            if ( WhereField.Values != null &&  WhereField.Values[0] != null)
            {
                userTextBox.Text =  WhereField.Values[0].ToString();
            }
        }

        void userTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                if ( WhereField.Values == null)
                     WhereField.AddValue(((TextBox)sender).Text);
                else
                     WhereField.Values[0] = ((TextBox)sender).Text;
            }

            if (WhereFieldSelectedEvent != null)
                WhereFieldSelectedEvent( WhereField);
        }

        #endregion

        #region Taxonomy field methods
        private void CreateControlsForTaxonomyField()
        {
            StackPanel taxonomyStackPanel = new StackPanel();
            taxonomyStackPanel.Orientation = Orientation.Horizontal;
            taxonomyStackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            TextBox taxonomyTextBox = new TextBox();
            taxonomyTextBox.Width = 140.0;
            taxonomyTextBox.Height = 24.0;
            taxonomyTextBox.Margin = new Thickness(0, 10, 0, 0);
            taxonomyTextBox.TextChanged += new TextChangedEventHandler(textbox_TextChanged);
            taxonomyStackPanel.Children.Add(taxonomyTextBox);
            if (App.GeneralInformation.ConnectionType != BPoint.SharePoint.Common.Enumerations.ConnectionType.WebServices)
            {
                Button taxonomyButton = new Button();
                taxonomyButton.Width = 30.0;
                taxonomyButton.Height = 24.0;
                taxonomyButton.Content = "...";
                taxonomyButton.Background = new SolidColorBrush(UI.GetColorFromHexString("#FF4D6082"));
                taxonomyButton.Foreground = new SolidColorBrush(Colors.Ivory);
                taxonomyButton.Background = new SolidColorBrush(Colors.SlateGray);
                taxonomyButton.Margin = new Thickness(5, 10, 0, 0);
                taxonomyButton.Click += new RoutedEventHandler(taxonomyButton_Click);
                taxonomyStackPanel.Children.Add(taxonomyButton);
            }
             ValueStackPanel.Children.Add(taxonomyStackPanel);
            TextBlock tmessageTextBlock = new TextBlock();
            tmessageTextBlock.Margin = new Thickness(0, 10, 0, 0);
            tmessageTextBlock.Text = "Separate values with a + sign, ex: titel 1+title 2+titel 3";
            tmessageTextBlock.TextWrapping = TextWrapping.Wrap;
            tmessageTextBlock.Foreground = new SolidColorBrush(UI.GetColorFromHexString("#FF293955"));
            tmessageTextBlock.Width = 180.0;
            tmessageTextBlock.Visibility = Visibility.Collapsed;
             ValueStackPanel.Children.Add(tmessageTextBlock);
            valueControl = taxonomyTextBox;
            messageControl = tmessageTextBlock;
        }

        void taxonomyButton_Click(object sender, RoutedEventArgs e)
        {
            // get the taxonomy
            List<TaxonomyValue> values = GetTaxonomyValues(
                ((TaxonomyField) WhereField.Field).TermStoreId, ((TaxonomyField) WhereField.Field).TermSetId);
            if (values != null)
            {
                bool multiSelect = false;
                if ( WhereField.Field.DataType == "TaxonomyFieldTypeMulti")
                    multiSelect = true;
                TaxonomyWindow taxonomyWindow = new TaxonomyWindow(values, multiSelect,  WhereField.WhereOperator);
                Nullable<bool> dialogResult = taxonomyWindow.ShowDialog();
                if (!string.IsNullOrEmpty(taxonomyWindow.SelectedValues))
                {
                    TextBox textbox = valueControl as TextBox;

                    if (((TaxonomyField) WhereField.Field).MultiSelect)
                    {
                        if (textbox != null)
                            textbox.Text = taxonomyWindow.SelectedValues;
                        AddWhereFieldValue(taxonomyWindow.SelectedValues);
                    }
                    else
                    {
                        if (taxonomyWindow.SelectedValues.Contains(";"))
                        {
                            // in that case only take the first selected value
                            AddWhereFieldValue(taxonomyWindow.SelectedValues.Substring(0, taxonomyWindow.SelectedValues.IndexOf(';')));
                            if (textbox != null)
                                textbox.Text = taxonomyWindow.SelectedValues.Substring(0, taxonomyWindow.SelectedValues.IndexOf(';'));
                        }
                        else
                        {
                            if (textbox != null)
                                textbox.Text = taxonomyWindow.SelectedValues;
                            AddWhereFieldValue(taxonomyWindow.SelectedValues);
                        }
                    }

                    if (WhereFieldSelectedEvent != null)
                        WhereFieldSelectedEvent( WhereField);
                }
            }
            else
            {
                System.Windows.MessageBox.Show(string.Format("No terms found for managed metadata field {0} with group {1} and term set id {2}",
                     WhereField.Field.DisplayName, ((TaxonomyField) WhereField.Field).TermStoreId, ((TaxonomyField) WhereField.Field).TermSetId));
            }
        }
        #endregion

        #region Events that cause the CAML Query to fire
        void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                if ( WhereField.Values == null)
                     WhereField.AddValue(((TextBox)sender).Text);
                else
                     WhereField.Values[0] = ((TextBox)sender).Text;
            }

            if (this.WhereFieldSelectedEvent != null)
                this.WhereFieldSelectedEvent( WhereField);
        }

        void checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
                this.AddWhereFieldValue(((CheckBox)sender).IsChecked);

            if (this.WhereFieldSelectedEvent != null)
                this.WhereFieldSelectedEvent( WhereField);
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
                WhereFieldSelectedEvent( WhereField);
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

        private void OperatorControl_FieldOperatorEvent(string fieldOperator)
        {
            if (!string.IsNullOrEmpty(fieldOperator))
            {
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
                        this.ResizeControl(this.Height + 20.0);
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

                // Set the And or Or operator
                if (fieldOperator == "And" || fieldOperator == "Or")
                    whereField.AndOrOperator = fieldOperator;
                else
                    whereField.WhereOperator = fieldOperator;

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
                if ( WhereField.WhereOperator == "In")
                     WhereField.AddValue(selectedValue);
                else
                {
                    if ( WhereField.Values == null)
                         WhereField.AddValue(selectedValue);
                    else
                         WhereField.Values[0] = selectedValue;
                }
            }
        }

        private void ResizeControl(double height)
        {
            if (height >= 20)
            {
                this.Height = height;
                WhereFieldTile.Height = height;
            }
        }



        #endregion
}
}
