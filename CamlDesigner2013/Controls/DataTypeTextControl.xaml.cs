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
    /// Interaction logic for DataTypeTextControl.xaml
    /// </summary>
    public partial class DataTypeTextControl : UserControl
    {
        public DataTypeTextControl()
        {
            InitializeComponent();
        }

        public bool EnforceUniqueValues
        {
            get
            {
                return (bool)EnforceUniqueValuesRadioButton.IsChecked;
            }
            set
            {
                if (value)
                {
                    EnforceUniqueValuesRadioButton.IsChecked = true;
                }
                else
                {
                    NotEnforceUniqueValuesRadioButton.IsChecked = false;
                }
            }
        }

        public int MaximumNumberOfCharacters
        {
            get
            {
                return System.Convert.ToInt32(NumberOfCharactersTextBox.Text);
            }
            set
            {
                NumberOfCharactersTextBox.Text = value.ToString();
            }
        }
    }
}
