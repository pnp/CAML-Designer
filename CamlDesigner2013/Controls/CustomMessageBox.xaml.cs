using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace CamlDesigner2013.Controls
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : MetroWindow
    {
             private MessageBoxResult _result = MessageBoxResult.None;
        private Button _close;
        private FrameworkElement _title;

        #region Constructors
        public CustomMessageBox()
        {
            this.InitializeComponent();
            DataContext = this;
        }
        #endregion

        #region Properties
        public string Caption
        {
            get { return this.Title; }
            set { this.Title = value; }
        }

        public MessageBoxResult MessageBoxResult
        {
            get { return this._result; }
            private set
            {
                this._result = value;
                if (MessageBoxResult.Cancel == this._result)
                {
                    this.DialogResult = false;
                }
                else
                {
                    this.DialogResult = true;
                }
            }
        }
        #endregion

        #region Dependency Properties
        public MessageBoxResult DefaultResult
        {
            get { return (MessageBoxResult)GetValue(DefaultResultProperty); }
            set
            {
                SetValue(DefaultResultProperty, value);

                switch (value)
                {
                    case MessageBoxResult.Cancel:
                        this._cancel.IsDefault = true;
                        break;
                    case MessageBoxResult.No:
                        this._no.IsDefault = true;
                        break;
                    case MessageBoxResult.None:
                        break;
                    case MessageBoxResult.OK:
                        this._ok.IsDefault = true;
                        break;
                    case MessageBoxResult.Yes:
                        this._yes.IsDefault = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public static readonly DependencyProperty DefaultResultProperty =
            DependencyProperty.Register("DefaultResult", typeof(MessageBoxResult),
                typeof(CustomMessageBox), new UIPropertyMetadata(MessageBoxResult.None));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string),
                typeof(CustomMessageBox), new UIPropertyMetadata(string.Empty));

        public MessageBoxButton MessageBoxButton
        {
            get { return (MessageBoxButton)GetValue(MessageBoxButtonProperty); }
            set
            {
                SetValue(MessageBoxButtonProperty, value);

                switch (value)
                {
                    case MessageBoxButton.OK:
                        this._ok.Visibility = Visibility.Visible;
                        break;
                    case MessageBoxButton.OKCancel:
                        this._ok.Visibility = Visibility.Visible;
                        this._cancel.Visibility = Visibility.Visible;
                        break;
                    case MessageBoxButton.YesNo:
                        this._yes.Visibility = Visibility.Visible;
                        this._no.Visibility = Visibility.Visible;
                        break;
                    case MessageBoxButton.YesNoCancel:
                        this._yes.Visibility = Visibility.Visible;
                        this._no.Visibility = Visibility.Visible;
                        this._cancel.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }

        public static readonly DependencyProperty MessageBoxButtonProperty =
            DependencyProperty.Register("MessageBoxButton", typeof(MessageBoxButton),
                typeof(CustomMessageBox), new UIPropertyMetadata(MessageBoxButton.OK));

        public MessageBoxImage MessageBoxImage
        {
            get { return (MessageBoxImage)GetValue(MessageBoxImageProperty); }
            set { SetValue(MessageBoxImageProperty, value); }
        }

        public static readonly DependencyProperty MessageBoxImageProperty =
            DependencyProperty.Register("MessageBoxImage", typeof(MessageBoxImage),
                typeof(CustomMessageBox), new UIPropertyMetadata(MessageBoxImage.None));
        #endregion

        #region Static Methods
        public static MessageBoxResult Show(string messageBoxText)
        {
            return Show(null, messageBoxText, string.Empty,
                MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return Show(null, messageBoxText, caption, MessageBoxButton.OK,
                MessageBoxImage.None, MessageBoxResult.None);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText)
        {
            return Show(owner, messageBoxText, string.Empty, MessageBoxButton.OK,
                MessageBoxImage.None, MessageBoxResult.None);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            return Show(null, messageBoxText, caption, button, MessageBoxImage.None,
                MessageBoxResult.None);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
        {
            return Show(owner, messageBoxText, caption, MessageBoxButton.OK,
                MessageBoxImage.None, MessageBoxResult.None);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption,
            MessageBoxButton button, MessageBoxImage icon)
        {
            return Show(null, messageBoxText, caption, button, icon,
                MessageBoxResult.None);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption,
            MessageBoxButton button)
        {
            return Show(owner, messageBoxText, caption, button,
                MessageBoxImage.None, MessageBoxResult.None);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption,
            MessageBoxButton button, MessageBoxImage image, MessageBoxResult defaultResult)
        {
            return Show(null, messageBoxText, caption, button, image, defaultResult);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption,
            MessageBoxButton button, MessageBoxImage icon)
        {
            return Show(owner, messageBoxText, caption, button, icon, MessageBoxResult.None);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText,
            string caption, MessageBoxButton button, MessageBoxImage icon,
            MessageBoxResult defaultResult)
        {
            CustomMessageBox messageBox = new CustomMessageBox();
            messageBox.Caption = caption;
            messageBox.DefaultResult = defaultResult;
            messageBox.Owner = owner;
            messageBox.Message = messageBoxText;
            messageBox.MessageBoxButton = button;
            messageBox.MessageBoxImage = icon;
            messageBox._image.Source = GetImage(icon) as BitmapImage;
            if (false == messageBox.ShowDialog())
            {
                return MessageBoxResult.Cancel;
            }

            return messageBox.MessageBoxResult;
        }



        private static object GetImage(MessageBoxImage value)
        {
            switch (value)
            {
                case MessageBoxImage.Information: /* Asterisk has the same value. */
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/Information.png"));
                case MessageBoxImage.Error: /* Hand and Stop have the same value. */
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/Error.png"));
                case MessageBoxImage.Warning: /* Exclamation has the same value. */
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/Warning.png"));
                case MessageBoxImage.Question:
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/Question.png"));
                default:
                    return null;
            }
        }
        #endregion

        #region Event Handlers
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = MessageBoxResult.Cancel;
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = MessageBoxResult.No;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = MessageBoxResult.OK;
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            this._close = (Button)this.Template.FindName("PART_Close", this);
            if (null != this._close)
            {
                if (false == this._cancel.IsVisible)
                {
                    this._close.IsCancel = false;
                }
            }

            this._title = (FrameworkElement)this.Template.FindName("PART_Title", this);
            if (null != this._title)
            {
                this._title.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
            }
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = MessageBoxResult.Yes;
        }


        #endregion


    
    }
}
