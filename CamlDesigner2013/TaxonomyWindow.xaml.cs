namespace CamlDesigner2013
{
    using CamlDesigner.SharePoint.Objects;
    using System.Collections.Generic;
    using System.Windows;
    using System.Linq;
    using System;

    /// <summary>
    /// Interaction logic for TaxonomyWindow.xaml
    /// </summary>
    public partial class TaxonomyWindow : Window
    {
        private string separator = ";";
        private string whereOperator;
        private bool multiSelect;
        private List<TaxonomyValue> taxonomyValues;

        private List<TaxonomyValue> selectedValues = null;
        private List<TaxonomyValue> newSelectedValues = null;

        public List<TaxonomyValue> SelectedValues
        {
            get { return this.selectedValues; }
            set { this.selectedValues = value; }
        }

        public List<TaxonomyValue> TaxonomyValues
        {
            get { return this.taxonomyValues; }
            set { this.taxonomyValues = value; }
        }

        public TaxonomyWindow()
        {
        }


        public TaxonomyWindow(List<TaxonomyValue> taxonomyValues, bool multiSelect, string whereOperator): this()
        {
            this.InitializeComponent();
            this.taxonomyValues = taxonomyValues;
            this.multiSelect = multiSelect;
            this.whereOperator = whereOperator;

            if (whereOperator == "In")
            {
                this.separator = "+";
            }

            TaxonomyTreeView.ItemsSource = GetTreeFromNodes(taxonomyValues);
        }

        private List<TaxonomyValue> GetTreeFromNodes(List<TaxonomyValue> taxonomyNodes)
        {
            List<TaxonomyValue> retVal = (from t in taxonomyNodes where t.ParentId.Equals(Guid.Empty) select t).ToList<TaxonomyValue>();

            foreach (TaxonomyValue tv in retVal)
            {
                ExpandChildren(tv, taxonomyNodes);
            }

            return new List<TaxonomyValue>(retVal);
        }

        private void ExpandChildren(TaxonomyValue tv, List<TaxonomyValue> taxonomyNodes)
        {
            if (tv != null)
            {
                if (tv.Terms == null || tv.Terms.Count <= 0)
                {
                    // Check if term has children
                    tv.Terms = (from t in taxonomyNodes where t.ParentId.Equals(tv.ID) select t).ToList<TaxonomyValue>();
                }

                if (tv.Terms != null)
                {
                    foreach (TaxonomyValue child in tv.Terms)
                    {
                        ExpandChildren(child, taxonomyNodes);
                    }
                }
            }
        }

        private void TaxonomyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // this must return the selected value and close the dialog
            if (TaxonomyTreeView.SelectedItem != null)
            {
                TaxonomyValue taxonomyValue = TaxonomyTreeView.SelectedItem as TaxonomyValue;
                if (taxonomyValue != null && !string.IsNullOrEmpty(taxonomyValue.Value))
                {
                    if (this.multiSelect)
                    {
                        if (SelectedValuesTextBox.Text.Length > 0)
                            SelectedValuesTextBox.Text += this.separator;
                        SelectedValuesTextBox.Text += taxonomyValue.Value;
                        if (newSelectedValues == null)
                            newSelectedValues = new List<TaxonomyValue>();
                        newSelectedValues.Add(taxonomyValue);
                    }
                    else
                    {
                        SelectedValuesTextBox.Text = taxonomyValue.Value;
                        newSelectedValues = new List<TaxonomyValue>();
                        newSelectedValues.Add(taxonomyValue);
                    }
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedValuesTextBox.Text.Length > 0)
            {
                this.DialogResult = true;
                this.selectedValues = newSelectedValues;
            }
            this.Close();
        }
    }
}
