using CamlDesigner.Common;
using CamlDesigner2013.Controls.Drag.ServiceProviders.UI;
using CamlDesigner2013.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CamlDesigner2013.Controls.Drag
{
    /// <summary>
    /// Interaction logic for DragAndDrop.xaml
    /// </summary>
    public partial class DragAndDrop : UserControl
    {
        public event ViewFieldEventHandler ViewFieldSelectedEvent;
        public event ViewFieldEventHandler ViewFieldDeletedEvent;
        public event OrderByFieldEventHandler OrderByFieldSelectedEvent;
        public event OrderByFieldEventHandler OrderByFieldDeletedEvent;
        public event WhereFieldEventHandler WhereFieldSelectedEvent;
        public event WhereFieldEventHandler WhereFieldDeletedEvent;

        public event GroupByFieldEventHandler GroupByFieldSelectedEvent;
        public event GroupByFieldEventHandler GroupByFieldDeletedEvent;

        ListViewDragDropManager<FieldControl> sourceDragManager;
        ListViewDragDropManager<FieldControl> targetDragManager;

        private CAMLHelper CamlHelper;

        private CamlDesigner.Common.Enumerations.QueryClauseType selectedQueryTab;

        public DragAndDrop()
        {
            InitializeComponent();
            App.SetDragAndDrop(this);

        }

        public DragAndDrop(ObservableCollection<FieldControl> fields)
            : this()
        {
        }

        //overloading constructor so that an Observablecollection can be passed along
        public DragAndDrop(ObservableCollection<FieldControl> fields,  CamlDesigner.Common.Enumerations.QueryClauseType selectedQueryTab)
            : this(fields)
        {
            this.selectedQueryTab = selectedQueryTab;

        }

        public DragAndDrop(ObservableCollection<FieldControl> fields, CamlDesigner.Common.Enumerations.QueryClauseType selectedQueryTab, CAMLHelper camlhelp)
            : this(fields, selectedQueryTab)
        {
            this.CamlHelper = camlhelp;
            if (fields != null)
            {
                this.LoadLists(fields);
            }
            else
            {
                this.LoadLists(new ObservableCollection<FieldControl>());
            }
        }

        #region Properties
        public CamlDesigner.Common.Enumerations.QueryClauseType SelectedQueryTab
        {
            get { return selectedQueryTab; }
            set { this.selectedQueryTab = value; }
        }
        #endregion

        private void LoadLists(ObservableCollection<FieldControl> fields)
        {
            // Give the ListView an ObservableCollection of FieldControl 
            // as a data source.  Note, the ListViewDragManager MUST
            // be bound to an ObservableCollection, where the collection's
            // type parameter matches the ListViewDragManager's type
            // parameter (in this case, both have a type parameter of Fieldcontrol
            // but when the item is dropped the correct control must be created instead).

            ObservableCollection<FieldControl> tasks = fields;
            this.SourceListView.ItemsSource = tasks;

            this.TargetListView.ItemsSource = new ObservableCollection<FieldControl>();

            // This is all that you need to do, in order to use the ListViewDragManager.
            this.sourceDragManager = new ListViewDragDropManager<FieldControl>(this.SourceListView, selectedQueryTab, this.CamlHelper, this);
            this.targetDragManager = new ListViewDragDropManager<FieldControl>(this.TargetListView, selectedQueryTab, this.CamlHelper, this);

            // Turn the ListViewDragManager on and off. 
            this.sourceDragManager.ListView = this.SourceListView;

            // Show and hide the drag adorner.
            this.sourceDragManager.ShowDragAdorner = true;

            // Apply or remove the item container style, which responds to changes
            // in the attached properties of ListViewItemDragState.
            this.SourceListView.ItemContainerStyle = this.FindResource("ItemContStyle") as Style;

            // Hook up events on both ListViews to that we can drag-drop
            // items between them.
            this.SourceListView.DragEnter += OnListViewDragEnter;
            this.TargetListView.DragEnter += OnListViewDragEnter;
            this.SourceListView.Drop += OnListViewDrop;
            this.TargetListView.Drop += OnListViewDrop;


        }


        #region sourceDragManager_ProcessDrop

        // Performs custom drop logic for the top ListView.
        void sourceDragManager_ProcessDrop(object sender, ProcessDropEventArgs<FieldControl> e)
        {
            // This shows how to customize the behavior of a drop.
            // Here we perform a swap, instead of just moving the dropped item.

            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            if (lowerIdx < 0)
            {
                // The item came from the lower ListView
                // so just insert it.
                e.ItemsSource.Insert(higherIdx, e.DataItem);
            }
            else
            {
                // null values will cause an error when calling Move.
                // It looks like a bug in ObservableCollection to me.
                if (e.ItemsSource[lowerIdx] == null ||
                    e.ItemsSource[higherIdx] == null)
                    return;

                // The item came from the ListView into which
                // it was dropped, so swap it with the item
                // at the target index.
                e.ItemsSource.Move(lowerIdx, higherIdx);
                e.ItemsSource.Move(higherIdx - 1, lowerIdx);
            }

            // Set this to 'Move' so that the OnListViewDrop knows to 
            // remove the item from the other ListView.
            e.Effects = DragDropEffects.Move;
        }

        #endregion // sourceDragManager_ProcessDrop

        #region OnListViewDragEnter

        // Handles the DragEnter event for both ListViews.
        void OnListViewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        #endregion // OnListViewDragEnter

        #region OnListViewDrop

        // Handles the Drop event for both ListViews.
        void OnListViewDrop(object sender, DragEventArgs e)
        {
            // check if the event is coming from the targetlistview and not from the source
            ListView listview = sender as ListView;
            if (listview.Name == "TargetListView")
            {
                FieldControl task = e.Data.GetData(typeof(FieldControl)) as FieldControl;

                if (e.Effects == DragDropEffects.None)
                    return;

                if (sender == this.SourceListView)
                {
                    if (this.sourceDragManager.IsDragInProgress)
                        return;

                    // An item was dragged from the target ListView into the source ListView
                    // so remove that item from the target ListView.
                    (this.TargetListView.ItemsSource as ObservableCollection<FieldControl>).Remove(task);

                    // reset the visibility of the FieldControl
                    task.SourceFieldStackPanel.Visibility = Visibility.Visible;
                    task.ViewFieldsFieldStackPanel.Visibility = Visibility.Collapsed;
                    task.OrderByFieldStackPanel.Visibility = Visibility.Collapsed;
                    task.GroupByFieldStackPanel.Visibility = Visibility.Collapsed;
                    task.WhereFieldStackPanel.Visibility = Visibility.Collapsed;
                    //task.AndOrStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                    // reset the height and the width of the FieldControl
                    task.Width = 195.0;
                    task.Height = 40.0;

                    // fire the event to remove the field from the CAML
                    switch (selectedQueryTab)
                    {
                        case Enumerations.QueryClauseType.ViewFields:
                            if (this.ViewFieldDeletedEvent != null)
                            {
                                this.ViewFieldDeletedEvent(task.ViewField);
                            }

                            break;
                        case Enumerations.QueryClauseType.OrderBy:
                            if (this.OrderByFieldDeletedEvent != null)
                            {
                                this.OrderByFieldDeletedEvent(task.OrderByField);
                            }

                            break;
                        case Enumerations.QueryClauseType.GroupBy:
                            if (this.GroupByFieldDeletedEvent != null)
                            {
                                this.GroupByFieldDeletedEvent(task.GroupByField);
                            }

                            break;
                        case Enumerations.QueryClauseType.Where:
                            if (this.WhereFieldDeletedEvent != null)
                            {
                                this.WhereFieldDeletedEvent(task.WhereField);
                            }

                            break;
                    }
                }
                else
                {
                    if (this.targetDragManager.IsDragInProgress)
                        return;

                    // An item was dragged from the source ListView into the target ListView
                    // so remove that item from the source ListView.
                    if (selectedQueryTab !=  CamlDesigner.Common.Enumerations.QueryClauseType.Where)
                    {
                        (this.SourceListView.ItemsSource as ObservableCollection<FieldControl>).Remove(task);
                    }
                }
            }
        }
        #endregion // OnListViewDrop

    }
}
