namespace CamlDesigner2013.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Controls;

    public delegate void TreeViewItemViewModelDelegate(TreeViewItemViewModel model, EventArgs e);

    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        #region Data

        private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        private readonly ObservableCollection<TreeViewItemViewModel> children;
        private readonly TreeViewItemViewModel parent;
        private string sharePointEnvironment;
        private StackPanel detailsPanel;

        private bool isExpanded;
        private bool isSelected;

        #endregion // Data

        #region Constructors

        public TreeViewItemViewModel()
        {
        }

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            this.parent = parent;

            this.children = new ObservableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren)
            {
                this.children.Add(DummyChild);
            }
        }

        #endregion // Constructors

        public event TreeViewItemViewModelDelegate TreeViewItemViewModelSelected;

        #region Children

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return this.children; }
        }

        #endregion // Children

        #region HasLoadedChildren

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }

        #endregion // HasLoadedChildren

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set
            {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (this.isExpanded && this.parent != null)
                    this.parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged("IsSelected");

                    if (this.isSelected)
                    {
                        // raise an event that can be captured by the main window
                        if (this.TreeViewItemViewModelSelected != null)
                            this.TreeViewItemViewModelSelected(this, null);
                    }
                }
            }
        }

        #endregion // IsSelected

        #region LoadChildren



        #endregion // LoadChildren

        #region Parent

        public TreeViewItemViewModel Parent
        {
            get { return this.parent; }
        }

        #endregion // Parent

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
