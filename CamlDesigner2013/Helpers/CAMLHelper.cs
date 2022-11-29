using CamlDesigner.SharePoint.Common;
using CamlDesigner.SharePoint.Objects;
using CamlDesigner2013.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamlDesigner2013.Helpers
{
    public delegate void ListSelectedEventHandler(string listName);
    public delegate void FieldEventHandler(Field field);
    public delegate void ViewFieldEventHandler(ViewField field);
    public delegate void OrderByFieldEventHandler(OrderByField field);
    public delegate void GroupByFieldEventHandler(GroupByField field);
    public delegate void WhereFieldEventHandler(WhereField field);
    public delegate void FieldOperatorEventHandler(string fieldOperator);
    public delegate void QueryOptionsEventHandler(QueryOptions queryOptions);

    public delegate void ConnectionEventHandler();

    public class CAMLHelper
    {
        public ObservableCollection<CamlDesigner2013.Controls.FieldControl> collection { get; set; }

        //private SortedList<int, ViewField> viewfieldList = null;
        //private SortedList<int, OrderByField> mainObject.OrderByFieldList = null;
        //private SortedList<int, WhereField> whereFieldList = null;

        public string CamlTextblock { get; set; }
        public string ServerOMTextblock { get; set; }
        public string CsomNetTextblock { get; set; }
        public string CsomRestTextblockJson { get; set; }
        public string CsomRestTextblockAtom { get; set; }
        public string WebServicesTextblock { get; set; }
        public string Powershellblock { get; set; }

        public CAMLHelper()
        {
        }

        public ObservableCollection<FieldControl> CreateFieldControlsList(List<Field> fields)
        {
            ObservableCollection<FieldControl> fieldControlsList = new ObservableCollection<FieldControl>();
            foreach (Field field in fields)
            {
                FieldControl fieldControl = new FieldControl(field);
                fieldControl.OrderByFieldSelectedEvent += new OrderByFieldEventHandler(OrderByFieldSelectedEvent);
                fieldControl.WhereFieldSelectedEvent += new WhereFieldEventHandler(WhereFieldSelectedEvent);
                fieldControlsList.Add(fieldControl);
            }
            return fieldControlsList;
        }

        public void ViewFieldSelectedEvent(ViewField field)
        {
            if (field != null)
            {
                if (App.mainObject.ViewFieldList == null)
                    App.mainObject.ViewFieldList = new SortedList<int, ViewField>();
                App.mainObject.ViewFieldList.Add(App.mainObject.ViewFieldList.Count, field);
            }

            this.GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType.ViewFields);
        }

        public void ViewFieldDeletedEvent(ViewField viewfield)
        {
            if (viewfield != null && App.mainObject.ViewFieldList != null)
            {
                for (int i = 0; i < App.mainObject.ViewFieldList.Count; i++)
                {
                    ViewField field = App.mainObject.ViewFieldList[i];
                    if (field.Field.ID == viewfield.Field.ID)
                    {
                        App.mainObject.ViewFieldList.RemoveAt(i);
                        break;
                    }
                }

                if (App.mainObject.ViewFieldList.Count > 0)
                {
                    SortedList<int, ViewField> tempList = new SortedList<int, ViewField>();

                    for (int i = 0; i < App.mainObject.ViewFieldList.Count; i++)
                    {
                        if (App.mainObject.ViewFieldList.ContainsKey(i))
                            tempList.Add(i, App.mainObject.ViewFieldList[i]);
                        else if (i + 1 <= App.mainObject.ViewFieldList.Count)
                        {
                            // this means that there is an empty space
                            tempList.Add(i, App.mainObject.ViewFieldList[i + 1]);
                        }
                    }
                    App.mainObject.ViewFieldList = tempList;
                }

                this.GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType.ViewFields);
            }
        }

        public void GroupByFieldSelectedEvent(GroupByField field)
        {
            if (field != null)
            {
                if (App.mainObject.GroupByFieldList == null)
                {
                    App.mainObject.GroupByFieldList = new SortedList<int, GroupByField>();
                }

                if (App.mainObject.GroupByFieldList.Values.Contains(field))
                {
                    int index = App.mainObject.GroupByFieldList.IndexOfValue(field);
                    App.mainObject.GroupByFieldList[index] = field;
                }
                else
                {
                    App.mainObject.GroupByFieldList.Add(App.mainObject.GroupByFieldList.Count, field);
                }

                // generate the CAML string
                this.GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType.GroupBy);
            }
        }

        public void GroupByFieldDeletedEvent(GroupByField groupbyField)
        {
            if (groupbyField != null && App.mainObject.GroupByFieldList != null)
            {
                for (int i = 0; i < App.mainObject.GroupByFieldList.Count; i++)
                {
                    GroupByField field = App.mainObject.GroupByFieldList[i];
                    if (field.Field.ID == groupbyField.Field.ID)
                    {
                        App.mainObject.GroupByFieldList.RemoveAt(i);
                        break;
                    }
                }

                if (App.mainObject.GroupByFieldList.Count > 0)
                {
                    SortedList<int, GroupByField> tempList = new SortedList<int, GroupByField>();

                    for (int i = 0; i < App.mainObject.GroupByFieldList.Count; i++)
                    {
                        if (App.mainObject.GroupByFieldList.ContainsKey(i))
                        {
                            tempList.Add(i, App.mainObject.GroupByFieldList[i]);
                        }
                        else if (i + 1 <= App.mainObject.GroupByFieldList.Count)
                        {
                            // this means that there is an empty space
                            tempList.Add(i, App.mainObject.GroupByFieldList[i + 1]);
                        }
                    }
                    App.mainObject.GroupByFieldList = tempList;
                }

                this.GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType.GroupBy);
            }
        }

        public void OrderByFieldSelectedEvent(OrderByField field)
        {
            if (field != null)
            {
                if (App.mainObject.OrderByFieldList == null)
                {
                    App.mainObject.OrderByFieldList = new SortedList<int, OrderByField>();
                }
                if (!App.mainObject.OrderByFieldList.ContainsValue(field))
                {
                    App.mainObject.OrderByFieldList.Add(App.mainObject.OrderByFieldList.Count, field);
                }
            }

            //    // generate the CAML string
            this.GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType.OrderBy);
        }

        public void OrderByFieldDeletedEvent(OrderByField orderByField)
        {
            if (orderByField != null && App.mainObject.OrderByFieldList != null)
            {
                for (int i = 0; i < App.mainObject.OrderByFieldList.Count; i++)
                {
                    OrderByField field = App.mainObject.OrderByFieldList[i];
                    if (field.Field.ID == orderByField.Field.ID)
                    {
                        App.mainObject.OrderByFieldList.RemoveAt(i);
                        break;
                    }
                }

                if (App.mainObject.OrderByFieldList.Count > 0)
                {
                    SortedList<int, OrderByField> tempList = new SortedList<int, OrderByField>();

                    for (int i = 0; i < App.mainObject.OrderByFieldList.Count; i++)
                    {
                        if (App.mainObject.OrderByFieldList.ContainsKey(i))
                            tempList.Add(i, App.mainObject.OrderByFieldList[i]);
                        else if (i + 1 <= App.mainObject.OrderByFieldList.Count)
                        {
                            // this means that there is an empty space
                            tempList.Add(i, App.mainObject.OrderByFieldList[i + 1]);
                        }
                    }
                    App.mainObject.OrderByFieldList = tempList;
                }

                this.GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType.OrderBy);
            }
        }

        public void WhereFieldSelectedEvent(WhereField field)
        {
            if (field != null)
            {
                if (App.mainObject.WhereFieldList == null)
                    App.mainObject.WhereFieldList = new SortedList<int, WhereField>();

                if (field.PositionInList == -1)
                {
                    // a new field is added to the list
                    field.PositionInList = App.mainObject.WhereFieldList.Count;
                    App.mainObject.WhereFieldList.Add(App.mainObject.WhereFieldList.Count, field);
                }
                else
                {
                    // the field was already added to the list of where field but one of the properties changed
                    for (int i = 0; i < App.mainObject.WhereFieldList.Count; i++)
                    {
                        WhereField wfield = App.mainObject.WhereFieldList[i];
                        if (field.PositionInList == i && wfield.Field.ID == field.Field.ID)
                        {
                            wfield.WhereOperator = field.WhereOperator;
                            wfield.Values = field.Values;
                            break;
                        }
                    }
                }

                this.GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType.Where);
            }
        }

        public void WhereFieldDeletedEvent(WhereField whereField)
        {
            if (whereField != null && App.mainObject.WhereFieldList != null)
            {
                // remove the where field
                for (int i = 0; i < App.mainObject.WhereFieldList.Count; i++)
                {
                    WhereField field = App.mainObject.WhereFieldList[i];
                    if (field.PositionInList == i && field.Field.ID == whereField.Field.ID)
                    {
                        App.mainObject.WhereFieldList.RemoveAt(i);
                        break;
                    }
                }

                if (App.mainObject.WhereFieldList.Count > 0)
                {
                    SortedList<int, WhereField> tempList = new SortedList<int, WhereField>();

                    for (int i = 0; i < App.mainObject.WhereFieldList.Count; i++)
                    {
                        if (App.mainObject.WhereFieldList.ContainsKey(i))
                            tempList.Add(i, App.mainObject.WhereFieldList[i]);
                        else if (i + 1 <= App.mainObject.WhereFieldList.Count)
                        {
                            // this means that there is an empty space
                            tempList.Add(i, App.mainObject.WhereFieldList[i + 1]);
                        }
                    }
                    App.mainObject.WhereFieldList = tempList;
                }

                this.GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType.Where);
                // InitializeWherePanel(ActualWidth, ActualHeight);
            }
        }

        public void QueryOptionsEvent(QueryOptions queryOptions)
        {
            if (queryOptions != null)
            {
                App.mainObject.QueryOptions = queryOptions;
                 Builder.GenerateQueryOptions(App.mainObject);

                if (queryOptions.QueryFilesAndFoldersAllFoldersDeep || queryOptions.QueryFilesAllFoldersDeep || queryOptions.QueryFoldersAllFoldersDeep
                    || queryOptions.QueryFilesAndFoldersInRootFolder || queryOptions.QueryFilesInRootFolder || queryOptions.QueryFoldersInRootFolder
                    || queryOptions.QueryFilesAndFoldersInSubFolder || queryOptions.QueryFilesInSubFolderDeep || queryOptions.QueryFilesAndFoldersInSubFolderDeep || queryOptions.ViewFieldsOnly)
                {
                     Builder.GenerateWhereFields(App.mainObject);
                }

                this.GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType.QueryOptions);
            }
        }

        private void GenerateCamlString( CamlDesigner.Common.Enumerations.QueryClauseType queryClauseType)
        {
            switch (queryClauseType)
            {
                case  CamlDesigner.Common.Enumerations.QueryClauseType.ViewFields:
                     Builder.GenerateViewFields(App.mainObject);
                    break;
                case  CamlDesigner.Common.Enumerations.QueryClauseType.OrderBy:
                     Builder.GenerateOrderByFields(App.mainObject);
                    break;
                case  CamlDesigner.Common.Enumerations.QueryClauseType.Where:
                     Builder.GenerateWhereFields(App.mainObject);
                    break;
                case  CamlDesigner.Common.Enumerations.QueryClauseType.GroupBy:
                     Builder.GenerateGroupByFields(App.mainObject);
                    break;

            }


            // call for the snippet
            if (App.mainObject.CamlDocument != null)
            {
                this.CamlTextblock = this.CamlWrapper.FormatCamlString(App.mainObject,
                    App.SelectedListViewModel.Title,
                     CamlDesigner.Common.Enumerations.SnippetType.CAML,
                    App.LanguageType,
                    App.QueryType, null);
                this.ServerOMTextblock = this.CamlWrapper.FormatCamlString(App.mainObject,
                    App.SelectedListViewModel.Title,
                     CamlDesigner.Common.Enumerations.SnippetType.ServerObjectModel,
                    App.LanguageType,
                    App.QueryType, null);
                this.CsomNetTextblock = this.CamlWrapper.FormatCamlString(App.mainObject,
                    App.SelectedListViewModel.Title,
                     CamlDesigner.Common.Enumerations.SnippetType.ClientObjectModelForDotnet,
                    App.LanguageType,
                    App.QueryType, null);
                this.WebServicesTextblock = this.CamlWrapper.FormatCamlString(App.mainObject,
                    App.SelectedListViewModel.Title,
                     CamlDesigner.Common.Enumerations.SnippetType.WebServices,
                    App.LanguageType,
                    App.QueryType, null);
                this.CsomRestTextblockAtom = this.CamlWrapper.FormatCamlString(App.mainObject,
                    App.SelectedListViewModel.Title,
                     CamlDesigner.Common.Enumerations.SnippetType.ClientObjectModelForRestWithAtom,
                    App.LanguageType,
                    App.QueryType, null);
                this.CsomRestTextblockJson = this.CamlWrapper.FormatCamlString(App.mainObject,
                    App.SelectedListViewModel.Title,
                     CamlDesigner.Common.Enumerations.SnippetType.ClientObjectModelForRestWithJson,
                    App.LanguageType,
                    App.QueryType, null);
                this.Powershellblock = this.CamlWrapper.FormatCamlString(App.mainObject,
                  App.SelectedListViewModel.Title,
                   CamlDesigner.Common.Enumerations.SnippetType.Powershell,
                  App.LanguageType,
                  App.QueryType, App.GeneralInformation);


                // populate the code blocks
                // App.mdiView.PopulateCodeBlocks(this);
                App.mdiView.CAMLTextBlock.Text = this.CamlTextblock;
                App.mdiView.ServerOmTextBlock.Text = this.ServerOMTextblock;
                App.mdiView.CsomNetTextBlock.Text = this.CsomNetTextblock;
                App.mdiView.WebServicesTextBlock.Text = this.WebServicesTextblock;
                App.mdiView.PowershellBlock.Text = this.Powershellblock;
                if ((bool)App.mdiView.AtomRadioButton.IsChecked)
                    App.mdiView.CsomRestTextBlock.Text = this.CsomRestTextblockAtom;
                else
                    App.mdiView.CsomRestTextBlock.Text = this.CsomRestTextblockJson;

                App.mdiView.CamlHelper = this;
            }

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
    }
}
