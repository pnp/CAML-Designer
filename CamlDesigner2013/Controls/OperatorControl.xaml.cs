using System;
using System.Collections.Generic;
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
using CamlDesigner.SharePoint.Objects;
using CamlDesigner2013.Helpers;

namespace CamlDesigner2013.Controls
{
    /// <summary>
    /// Interaction logic for OperatorControl.xaml
    /// </summary>
    public partial class OperatorControl : UserControl
    {
        private string whereOperator = null;

        public OperatorControl()
        {
            InitializeComponent();
        }

        public event FieldOperatorEventHandler FieldOperatorEvent;

        public void AdaptContent(string dataType)
        {
            // In operator
            if (dataType == "Note" || dataType == "DateTime" || dataType == "MultiChoice")
            {
                InMenuItem.Visibility = Visibility.Collapsed;
            }
            else
            {
                InMenuItem.Visibility = Visibility.Visible;
            }

            // Includes and NotIncludes operators only work with LookupMulti and UserMulti
            if (dataType == "LookupMulti" || dataType == "UserMulti")
            {
                IncludesMenuItem.Visibility = Visibility.Visible;
                NotIncludesMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                IncludesMenuItem.Visibility = Visibility.Collapsed;
                NotIncludesMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string header = menuItem.Header.ToString();
            if (menuItem != null)
            {
                //TODO: must look at this via a MUI way, than a switch isn't an option?
                switch (header)
                {
                    case "Equal":
                        this.OperatorTextBlock.Text = "=";
                        this.whereOperator = "Eq";
                        break;
                    case "Not equal":
                        this.OperatorTextBlock.Text = "!=";
                        this.whereOperator = "Neq";
                        break;
                    case "Greater than or equal":
                        this.OperatorTextBlock.Text = ">=";
                        this.whereOperator = "Geq";
                        break;
                    case "Greater than":
                        this.OperatorTextBlock.Text = ">";
                        this.whereOperator = "Gt";
                        break;
                    case "Less than":
                        this.OperatorTextBlock.Text = "<";
                        this.whereOperator = "Lt";
                        break;
                    case "Less than or equal":
                        this.OperatorTextBlock.Text = "<=";
                        this.whereOperator = "Leq";
                        break;
                    case "Is null":
                        this.OperatorTextBlock.Text = "IsNull";
                        this.whereOperator = "IsNull";
                        break;
                    case "Is not null":
                        this.OperatorTextBlock.Text = "IsNotNull";
                        this.whereOperator = "IsNotNull";
                        break;
                    case "Begins with":
                        this.OperatorTextBlock.Text = "begins";
                        this.whereOperator = "BeginsWith";
                        break;
                    case "Contains":
                        this.OperatorTextBlock.Text = "contains";
                        this.whereOperator = "Contains";
                        break;
                    case "In":
                        this.OperatorTextBlock.Text = "in";
                        this.whereOperator = "In";
                        break;
                    case "Includes":
                        this.OperatorTextBlock.Text = "includes";
                        this.whereOperator = "Includes";
                        break;
                    case "Not includes":
                        this.OperatorTextBlock.Text = "Not includes";
                        this.whereOperator = "NotIncludes";
                        break;
                }
                // fire the operator event handler
                if (this.FieldOperatorEvent != null)
                    this.FieldOperatorEvent(this.whereOperator);
            }
        }

        public void SetOperator(string whereOperator)
        {
            switch (whereOperator)
            {
                case "Eq":
                    this.OperatorTextBlock.Text = "=";
                    break;
                case "Neq":
                    this.OperatorTextBlock.Text = "!=";
                    break;
                case "Geq":
                    this.OperatorTextBlock.Text = ">=";
                    break;
                case "Gt":
                    this.OperatorTextBlock.Text = ">";
                    break;
                case "Lt":
                    this.OperatorTextBlock.Text = "<";
                    break;
                case "Leq":
                    this.OperatorTextBlock.Text = "<=";
                    break;
                case "IsNull":
                    this.OperatorTextBlock.Text = "IsNull";
                    break;
                case "IsNotNull":
                    this.OperatorTextBlock.Text = "IsNotNull";
                    break;
                case "BeginsWith":
                    this.OperatorTextBlock.Text = "begins";
                    break;
                case "Contains":
                    this.OperatorTextBlock.Text = "contains";
                    break;
                case "In":
                    this.OperatorTextBlock.Text = "in";
                    break;
                case "Includes":
                    this.OperatorTextBlock.Text = "includes";
                    break;
                case "NotIncludes":
                    this.OperatorTextBlock.Text = "NotIncludes";
                    break;
            }
        }
    }
}
