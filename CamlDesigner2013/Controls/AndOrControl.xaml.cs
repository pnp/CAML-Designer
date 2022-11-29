using CamlDesigner2013.Helpers;
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

namespace CamlDesigner2013.Controls
{
    /// <summary>
    /// Interaction logic for AndOrControl.xaml
    /// </summary>
    public partial class AndOrControl : UserControl
    {
        public AndOrControl()
        {
            this.InitializeComponent();
        }

        //public event WhereFieldEventHandler WhereFieldSelectedEvent;
        public event FieldOperatorEventHandler FieldOperatorEvent;

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AndRectangle.Visibility == System.Windows.Visibility.Collapsed)
                SetAndOperator();
            else
                SetOrOperator();

            // fire the operator event handler
            if (this.FieldOperatorEvent != null)
                this.FieldOperatorEvent(AndOrTextBlock.Text);
        }

        private void SetOrOperator()
        {
            AndRectangle.Visibility = System.Windows.Visibility.Collapsed;
            OrRectangle.Visibility = System.Windows.Visibility.Visible;
            AndOrTextBlock.Text = "Or";
        }

        private void SetAndOperator()
        {
            AndRectangle.Visibility = System.Windows.Visibility.Visible;
            OrRectangle.Visibility = System.Windows.Visibility.Collapsed;
            AndOrTextBlock.Text = "And";
        }
    }
}
