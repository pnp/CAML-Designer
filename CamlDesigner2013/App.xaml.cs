using CamlDesigner.Common;
using CamlDesigner.Common.Objects;
using CamlDesigner2013.Connections.Models;
using CamlDesigner2013.Helpers;
using CamlDesigner2013.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Xml;

namespace CamlDesigner2013
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // CamlDesigner workings
        public static bool ClearControls = true;
        public static bool ShowHiddenFields;
        public static ShellView mainWindow;
        public static MdiView.UI.MdiView mdiView;
        public static CamlDesigner2013.Controls.Drag.DragAndDrop dragAndDrop;
        
        public static ListViewModel SelectedListViewModel = null;

        //objects 
        public static MainObject mainObject = null; 
        public static CAMLDesigner.BusinessObjects.GeneralInfo GeneralInformation = null;
        public static CAMLDesigner.BusinessObjects.Wrapper CamlWrapper = null;
        
        
        //public static QueryOptions QueryOptions = null;
        //public static XmlDocument CamlDocument = null;
        //public static bool mainObject.mainObject.ViewFieldsOnly;
        //public static SortedList<int, ViewField> ViewfieldList = null;
        //public static SortedList<int, OrderByField> mainObject.OrderByFieldList = null;
        //public static SortedList<int, WhereField> WhereFieldList = null;

        public static Recent CurrentConnection { get; set; }

        public static bool SharePointLocallyInstalled { get; private set; }

        // set the default to Server OM
        public static Enumerations.ApiConnectionType ConnectionType = Enumerations.ApiConnectionType.ServerObjectModel;
        // set the default to CAML
        public static Enumerations.SnippetType SnippetType = Enumerations.SnippetType.CAML;
        // set the default to C#
        public static Enumerations.LanguageType LanguageType = Enumerations.LanguageType.CSharp;
        // set the default to CAML Query
        public static Enumerations.QueryType QueryType = Enumerations.QueryType.CAMLQuery;
        
        private const string Unique = "There can be only one!!!";

        public App()
        {
            InitializeComponent();
            SharePointLocallyInstalled = CheckClass.AssemblyExist("Microsoft.SharePoint");
            mainObject = new MainObject(); 
        }

        public static void SetMainWindow(ShellView mainwin)
        {
            mainWindow = mainwin;
        }

        public static void SetMDIView(MdiView.UI.MdiView mdiview)
        {
            mdiView = mdiview;
        }

        public static void SetDragAndDrop(CamlDesigner2013.Controls.Drag.DragAndDrop dd)
        {
            dragAndDrop = dd;
        }
    }
}
