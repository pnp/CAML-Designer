using CamlDesigner.SharePoint.Objects;
using CamlDesigner2013.Alerts;
using CamlDesigner2013.Controls;
using CamlDesigner2013.Helpers;
using CamlDesigner2013.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Linq; 

namespace CamlDesigner2013.MdiView.UI
{
    /// <summary>
    /// Interaction logic for MdiView.xaml
    /// </summary>
    public partial class MdiView : UserControl
    {
        private TreeViewState treeViewState;
        public StringBuilder sb = new StringBuilder();

        private List<Field> fieldsList = null;

        // treeview models
        private RootViewModel rootViewModel;

        // control variables
        private Controls.Drag.DragAndDrop availableFieldsViewFieldListControl = null;
        private Controls.Drag.DragAndDrop availableFieldsOrderByListControl = null;
        private Controls.Drag.DragAndDrop availableFieldsWhereListControl = null;
        private Controls.Drag.DragAndDrop availableFieldsGroupByListControl = null;

        private DataTable resultTable = null;

        private CAMLHelper camlHelper;

        public MdiView()
        {
            InitializeComponent();
            App.SetMDIView(this);
        }

        private CAMLDesigner.BusinessObjects.Wrapper CamlWrapper
        {
            get
            {
                if (App.CamlWrapper == null)
                {
                    if (App.GeneralInformation != null)
                    {
                        if (!string.IsNullOrEmpty(App.GeneralInformation.SharePointUrl))
                        {
                            App.CamlWrapper = new CAMLDesigner.BusinessObjects.Wrapper(
                                App.GeneralInformation.SharePointUrl,
                                App.GeneralInformation.ConnectionType);

                        }
                    }
                }
                return App.CamlWrapper;
            }
        }

        public CAMLHelper CamlHelper
        {
            get
            {
                if (camlHelper == null)
                {
                    camlHelper = new CAMLHelper();
                }
                return camlHelper;
            }

            set
            {
                camlHelper = value;
            }
        }

        private void QueryTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QueryTypeComboBox.SelectedItem.ToString().ToLower().Contains("query"))
            {
                App.QueryType =  CamlDesigner.Common.Enumerations.QueryType.SiteDataQuery;
                this.Caml.IsSelected = true;
                this.CSOM.Visibility = System.Windows.Visibility.Hidden;
                this.Powershell.Visibility = System.Windows.Visibility.Hidden;
                this.CSOMRest.Visibility = System.Windows.Visibility.Hidden;
                this.WebServices.Visibility = System.Windows.Visibility.Hidden;
                this.QueryOptionsControl.Clear();
            }
            else
            {
                App.QueryType =  CamlDesigner.Common.Enumerations.QueryType.CAMLQuery;
                if (this.CSOM != null && this.CSOMRest != null && this.WebServices != null)
                {
                    this.CSOM.Visibility = System.Windows.Visibility.Visible;
                    this.CSOMRest.Visibility = System.Windows.Visibility.Visible;
                    this.Powershell.Visibility = System.Windows.Visibility.Visible;
                    this.WebServices.Visibility = System.Windows.Visibility.Visible;
                    this.QueryOptionsControl.Clear();
                }
            }
        }

        public void ShowDebug(System.Windows.Visibility visibility)
        {
            Debug.Visibility = visibility;
        }

        public void ExecuteCAMLQuery()
        {
            if (!string.IsNullOrEmpty(CAMLTextBlock.Text))
            {
                QueryClauseTabControl.SelectedIndex = 4;
            }
            string caml = "<CAML>" + CAMLTextBlock.Text + "</CAML>";
            this.GetResults(caml);
        }

        private void QueryClauseTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabItem selectedTab = QueryClauseTabControl.SelectedItem as TabItem;
                if (selectedTab != null)
                {
                    switch (selectedTab.Header.ToString())
                    {
                        case "Test":
                            SimpleAlert simpleAlert = new SimpleAlert();
                            simpleAlert.Title = "Action";
                            simpleAlert.Message = "Initiating query, trying to get results";
                            this.GetResults(null);
                            break;
                        case "QueryOptions":
                            QueryOptionsControl.Reset();
                            break;
                    }
                }
            }
        }

        private async void GetResults(string caml)
        {
            BusyTreeview.Visibility = System.Windows.Visibility.Visible;

            //Populate the lists with the fields of the selected list
            await this.GetResultsForTest(caml);

            BusyTreeview.Visibility = System.Windows.Visibility.Hidden;
        }

        private System.Threading.Tasks.Task GetResultsForTest(string caml)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                // This must come from the treeview
                if (this.CamlWrapper != null)
                {
                    if (string.IsNullOrEmpty(caml))
                        resultTable = this.CamlWrapper.ExecuteQuery(App.SelectedListViewModel.Title,App.mainObject, App.QueryType);
                    else
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(caml);
                        App.mainObject.CamlDocument = doc;
                        resultTable = this.CamlWrapper.ExecuteQuery(App.SelectedListViewModel.Title, App.mainObject, App.QueryType);
                    }

                    //Message: The calling thread must be STA, because many UI components require this

                    Thread thread = new Thread(SetResultsToGrid);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();

                }
            });
        }

        private void SetResultsToGrid()
        {
            TestResultsGrid.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
               delegate()
               {
                   if (resultTable != null)
                   {
                       TestResultsGrid.ItemsSource = resultTable.DefaultView;
                       RowCountTextBlock.Text = resultTable.Rows.Count.ToString();
                       //Message: The calling thread must be STA, because many UI components require this

                       //Thread thread = new Thread(PopulateFieldsListsExec);
                       //thread.SetApartmentState(ApartmentState.STA);
                       //thread.Start();
                       //thread.Join();

                   }
                   else
                   {
                       TestResultsGrid.ItemsSource = null;
                       RowCountTextBlock.Text = "0";
                   }
                   TestResultsGrid.DataContext = null;
               }));
        }




        public void PopulateListsTreeview()
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    sb.AppendLine(this.DebugBlock.Text);
                }));

                sb.AppendLine("PopulateListsTreeview Method, starting");
                try
                {
                    this.rootViewModel = new RootViewModel("list");
                }
                catch (Exception ex)
                {
                    sb.AppendLine(string.Format("PopulateListsTreeview Method, an error has occured, message: {0}, stacktrace: {1}", ex.Message, ex.StackTrace));
                    // most likely invalid credentials 
                    throw;
                }

                // Let the UI bind to the view-model.
                if (this.rootViewModel != null)
                {
                    this.treeViewState = TreeViewState.ShowLists;
                    
                    this.tree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(this.Tree_SelectedItemChanged);
                    this.tree.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                this.tree.DataContext = this.rootViewModel;
                                InitializeQueryControls();
                            }));
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    this.DebugBlock.Text = sb.ToString();
                }));
            }
            catch (Exception ex)
            {
                //TODO: catch exception
                throw ex;
            }
        }

        bool isExecuting = false;

        private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!isExecuting)
            {
                SimpleAlert alert = new SimpleAlert();
                alert.Title = "Action";
                alert.Message = "Getting the fields from the selected list.";
                this.InitializeQueryControls();
            }
            else
            {
                isExecuting = false;
            }
        }

        private void InitializeControlsForSharePointVersion()
        {
            // REST snippets are only available for SharePoint 2013
            if (App.GeneralInformation.SharePointVersion ==  CamlDesigner.Common.Enumerations.SharePointVersions.SP2013)
                CSOMRest.Visibility = System.Windows.Visibility.Visible;
            else
                CSOMRest.Visibility = System.Windows.Visibility.Collapsed;
        }

        public async void InitializeQueryControls()
        {
            BusyTreeview.Visibility = System.Windows.Visibility.Visible;

            // Check which version of SharePoint is selected
            InitializeControlsForSharePointVersion();
            App.ShowHiddenFields = !(bool)ShowHiddenField.IsChecked;
            // Populate the lists with the fields of the selected list
            await this.PopulateFieldsLists();
            QueryOptionsControl.Clear();

            // clear CAML controls
            CAMLTextBlock.Text = string.Empty;
            ServerOmTextBlock.Text = string.Empty;
            CsomNetTextBlock.Text = string.Empty;
            CsomRestTextBlock.Text = string.Empty;
            WebServicesTextBlock.Text = string.Empty;
            BusyTreeview.Visibility = System.Windows.Visibility.Hidden;

            // empty the CAML document
            App.mainObject.CamlDocument = null;

            // empty the sorted lists
            App.mainObject.ViewFieldList = null;
            App.mainObject.OrderByFieldList = null;
            App.mainObject.WhereFieldList = null;
            App.mainObject.GroupByFieldList = null; 

            // empty the results datagrid
            TestResultsGrid.ItemsSource = null;
        }

        private System.Threading.Tasks.Task PopulateFieldsLists()
        {
            return System.Threading.Tasks.Task.Run(() =>
                {
                    // This must come from the treeview
                    if (App.SelectedListViewModel != null)
                    {
                        if (App.SelectedListViewModel.TemplateID == 106)
                        {
                            this.fieldsList = this.CamlWrapper.GetFields(App.SelectedListViewModel.Title, App.ShowHiddenFields, true);
                        }
                        else
                        {
                            this.fieldsList = this.CamlWrapper.GetFields(App.SelectedListViewModel.Title, App.ShowHiddenFields, false);
                        }
                        
                        // sort alfabetically 
                        List<Field> SortedList = this.fieldsList.OrderBy(o => o.DisplayName).ToList();

                        this.fieldsList = SortedList;

                        if (this.fieldsList != null && this.fieldsList.Count > 0)
                        {
                            //Message: The calling thread must be STA, because many UI components require this

                            Thread thread = new Thread(PopulateFieldsListsExec);
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            thread.Join();

                        }
                    }
                });
        }

     

        private void PopulateFieldsListsExec()
        {
            PopulateDragAndDropList(availableFieldsViewFieldListControl, ViewfieldsGrid,  CamlDesigner.Common.Enumerations.QueryClauseType.ViewFields);
            PopulateDragAndDropList(availableFieldsOrderByListControl, OrderByGrid,  CamlDesigner.Common.Enumerations.QueryClauseType.OrderBy);
            PopulateDragAndDropList(availableFieldsWhereListControl, WhereGrid,  CamlDesigner.Common.Enumerations.QueryClauseType.Where);

            PopulateDragAndDropList(availableFieldsGroupByListControl, GroupByGrid,  CamlDesigner.Common.Enumerations.QueryClauseType.GroupBy);
            // set the QueryOptions event
            QueryOptionsControl.QueryOptionsEvent += CamlHelper.QueryOptionsEvent;
        }

        private void PopulateDragAndDropList(Controls.Drag.DragAndDrop dragAndDropListControl, Grid grid,  CamlDesigner.Common.Enumerations.QueryClauseType queryClauseType)
        {
            grid.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    // get the fieldcontrolslist at the instantiated camlhelper class and pass this class to the correct draganddrop window
                    CAMLHelper camlhelper = new CAMLHelper();
                    dragAndDropListControl = new Controls.Drag.DragAndDrop(camlhelper.CreateFieldControlsList(fieldsList), queryClauseType, camlhelper);
                    switch (queryClauseType)
                    {
                        case  CamlDesigner.Common.Enumerations.QueryClauseType.ViewFields:
                            dragAndDropListControl.ViewFieldSelectedEvent += new ViewFieldEventHandler(camlhelper.ViewFieldSelectedEvent);
                            dragAndDropListControl.ViewFieldDeletedEvent += new ViewFieldEventHandler(camlhelper.ViewFieldDeletedEvent);
                            break;
                        case  CamlDesigner.Common.Enumerations.QueryClauseType.OrderBy:
                            dragAndDropListControl.OrderByFieldSelectedEvent += new OrderByFieldEventHandler(camlhelper.OrderByFieldSelectedEvent);
                            dragAndDropListControl.OrderByFieldDeletedEvent += new OrderByFieldEventHandler(camlhelper.OrderByFieldDeletedEvent);
                            break;
                        case  CamlDesigner.Common.Enumerations.QueryClauseType.Where:
                            dragAndDropListControl.WhereFieldSelectedEvent += new WhereFieldEventHandler(camlhelper.WhereFieldSelectedEvent);
                            dragAndDropListControl.WhereFieldDeletedEvent += new WhereFieldEventHandler(camlhelper.WhereFieldDeletedEvent);
                            break;
                        case  CamlDesigner.Common.Enumerations.QueryClauseType.GroupBy:
                            dragAndDropListControl.GroupByFieldSelectedEvent += new GroupByFieldEventHandler(camlhelper.GroupByFieldSelectedEvent);
                            dragAndDropListControl.GroupByFieldDeletedEvent += new GroupByFieldEventHandler(camlhelper.GroupByFieldDeletedEvent);
                            
                            break;
                    }
                    grid.Children.Add(dragAndDropListControl);
                }));
        }


        private void JsonRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)JsonRadioButton.IsChecked && App.mainObject.CamlDocument != null)
            {
                if (this.CamlHelper != null)
                {
                    CsomRestTextBlock.Text = this.CamlHelper.CsomRestTextblockJson;
                }
            }
        }

        private void AtomRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)AtomRadioButton.IsChecked && App.mainObject.CamlDocument != null)
            {
                if (this.CamlHelper != null)
                {
                    CsomRestTextBlock.Text = this.CamlHelper.CsomRestTextblockAtom;
                }
            }
        }

        private void MUISetter()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("..\\..\\..\\Languages\\MUI.en-US.xaml", UriKind.Relative);
                    break;
                case "nl-NL":
                    dict.Source = new Uri("..\\..\\..\\Languages\\MUI.nl-NL.xaml", UriKind.Relative);
                    break;
                case "nl-BE":
                    dict.Source = new Uri("..\\..\\..\\Languages\\MUI.nl-BE.xaml", UriKind.Relative);
                    break;
                default:
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Caml.IsSelected)
            {
                App.mainWindow.ChangeVisibilityOfExecuteButton(false);
            }
            else
            {
                App.mainWindow.ChangeVisibilityOfExecuteButton(true);
            }
        }

        private void WithLambaExpression_Checked(object sender, RoutedEventArgs e)
        {

        }


    }
}
