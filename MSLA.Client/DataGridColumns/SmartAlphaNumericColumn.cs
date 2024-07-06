using System.Reflection;
using System.Windows.Controls;
using C1.Silverlight.DataGrid;
using C1.Silverlight;
using System.Windows;
using System;
using System.Windows.Input;
using System.ComponentModel;

namespace MSLA.Client.SmartTools
{
    public class SmartAlphaNumericColumn
        : C1.Silverlight.DataGrid.DataGridTextColumn
    {

        #region Private Variables

        private bool _NegativeAllowed = false;
        private bool _comma = false;
        private Int32 _DecimalPlaces = 0;
        private string _FormatType = string.Empty;
        private decimal _MaxValue = decimal.MaxValue;

        #endregion

        #region Public Constructor

        public SmartAlphaNumericColumn()
            : base()
        {
        }

        public SmartAlphaNumericColumn(PropertyInfo property)
            : base(property)
        {

        }

        #endregion

        #region Helper Methods
        private string SetFormatType()
        {
            string FormatType = string.Empty;
            if (_DecimalPlaces > 0)
            {
                FormatType = "." + FormatType.PadRight(DecimalPlaces, '#');
            }
            if (_comma == true)
            {
                FormatType = "#,##0" + FormatType;
            }
            else
            {
                FormatType = "###0" + FormatType;
            }
            return FormatType;
        }
        #endregion

        #region Public Properties

        public bool NegativeAllowed
        {
            get { return _NegativeAllowed; }
            set { _NegativeAllowed = value; }
        }

        public Int32 DecimalPlaces
        {
            get { return _DecimalPlaces; }
            set
            {
                _DecimalPlaces = value;
                _FormatType = SetFormatType();
            }
        }

        public decimal MaxValue
        {
            get { return _MaxValue; }
            set
            {
                _MaxValue = value;
            }
        }

        public bool Comma
        {
            get { return _comma; }
            set
            {
                _comma = value;
                _FormatType = SetFormatType();
            }
        }

        #endregion

        #region Public Override Methods

        public override bool IsEditable
        {
            get
            {
                return true;
            }
        }

        public override void EndEdit(FrameworkElement editingElement)
        {
            C1TextBoxBase textBox = (C1TextBoxBase)editingElement;
            LostFocus(textBox);
        }

        public override FrameworkElement GetCellEditingContent(C1.Silverlight.DataGrid.DataGridRow row)
        {
            C1TextBoxBase textBox = (C1TextBoxBase)base.GetCellEditingContent(row);

            textBox.GotFocus += new RoutedEventHandler(textBox_GotFocus);
            textBox.LostFocus += new RoutedEventHandler(textBox_LostFocus);
            textBox.KeyDown += new KeyEventHandler(textBox_KeyDown);
            this.Format = _FormatType;
            return textBox;
        }

        #endregion

        #region TextBox Events

        void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            C1TextBoxBase textBox = (C1TextBoxBase)sender;
            decimal result = 0;
            if (decimal.TryParse(textBox.Text, out result))
            {
                textBox.Text = String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0:" + _FormatType + "}", result);
            }
            else
            {
                textBox.Text = "0";
            }
            textBox.SelectAll();
        }

        void textBox_KeyDown(Object sender, KeyEventArgs e)
        {
            int keyAscii = -1;
            C1TextBoxBase textBox = (C1TextBoxBase)sender;

            switch (e.Key)
            {
                //   ****    Digits 0-9, backspace, Enter key
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                    if (textBox.Text.Contains("m") | textBox.Text.Contains("b") | textBox.Text.Contains("t") | textBox.Text.Contains("M") | textBox.Text.Contains("B") | textBox.Text.Contains("T"))
                    {
                        if (textBox.SelectionStart == textBox.Text.Length)
                        {
                            keyAscii = 0;
                        }
                    }
                    break;
                case Key.M:
                case Key.B:
                case Key.T:
                    if (textBox.SelectionStart == 0 | textBox.Text.Contains("m") | textBox.Text.Contains("b") | textBox.Text.Contains("t") | textBox.Text.Contains("M") | textBox.Text.Contains("B") | textBox.Text.Contains("T"))
                    {
                        keyAscii = 0;
                    }
                    break;
                case Key.Subtract:

                    //   ****    If in first position and Negative does not already exist then allow
                    if (_NegativeAllowed)
                    {
                        if (textBox.SelectionLength != textBox.Text.Length & (textBox.SelectionStart != 0 | textBox.Text.Contains("-")))
                        {
                            keyAscii = 0;
                        }
                    }
                    else
                    {
                        keyAscii = 0;
                    }
                    break;
                case Key.Decimal:
                    //   ****    If decimal point already exists, then trow away the new
                    if (textBox.Text.Contains(".") & textBox.SelectionLength != textBox.Text.Length)
                    {
                        keyAscii = 0;
                    }

                    if (_DecimalPlaces == 0)
                    {
                        keyAscii = 0;
                    }
                    break;
                case Key.Unknown:
                    if (e.PlatformKeyCode == 189)
                    {
                        if (_NegativeAllowed)
                        {
                            if (textBox.SelectionLength != textBox.Text.Length & (textBox.SelectionStart != 0 | textBox.Text.Contains("-")))
                            {
                                keyAscii = 0;
                            }
                        }
                        else
                        {
                            keyAscii = 0;
                        }
                    }
                    else if (e.PlatformKeyCode == 190)
                    {
                        if (textBox.Text.Contains(".") & textBox.SelectionLength != textBox.Text.Length)
                        {
                            keyAscii = 0;
                        }

                        if (_DecimalPlaces == 0)
                        {
                            keyAscii = 0;
                        }
                    }
                    else
                    {
                        keyAscii = 0;
                    }
                    break;
                // Do nothing  
                case Key.Left:
                case Key.Right:
                case Key.Delete:
                case Key.Tab:
                case Key.Back:
                case Key.Enter:
                    break;
                default:
                    keyAscii = 0;
                    break;
            }
            if (keyAscii == 0)
            {
                e.Handled = true;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                if (e.Key == Key.Tab)
                {
                    e.Handled = false;
                }
                else if (e.Key == Key.M || e.Key == Key.B || e.Key == Key.T)
                {
                    if (textBox.SelectionStart == 0 | textBox.Text.Contains("m") | textBox.Text.Contains("b") | textBox.Text.Contains("t") | textBox.Text.Contains("M") | textBox.Text.Contains("B") | textBox.Text.Contains("T"))
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            C1TextBoxBase textBox = (C1TextBoxBase)sender;
            LostFocus(textBox);
        }

        private void LostFocus(C1TextBoxBase textBox)
        {
            this.Format = _FormatType;
            Boolean HasChar = true;
            String str = string.Empty;
            decimal result = 0;
            decimal Multiplier = 1;

            if (textBox.Text.Contains("T") || textBox.Text.Contains("t"))
            {
                Multiplier = 1000;
            }
            else if (textBox.Text.Contains("M") || textBox.Text.Contains("m"))
            {
                Multiplier = 1000000;
            }
            else if (textBox.Text.Contains("B") || textBox.Text.Contains("b"))
            {
                Multiplier = 1000000000;
            }
            else
            {
                HasChar = false;
            }

            if (HasChar)
            {
                str = textBox.Text.Substring(0, textBox.Text.Length - 1);
            }
            else
            {
                str = textBox.Text;
            }

            if (decimal.TryParse(str, out result))
            {
                //textBox.Text = "0";
            }
            if (result == 0)
            {
                textBox.Text = string.Empty;
            }
            else
            {
                result = result * Multiplier;
                textBox.Text = String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0:" + _FormatType + "}", result);

                decimal tmp = 0;
                if (decimal.TryParse(textBox.Text, out tmp))
                {
                    if (Math.Abs(tmp) >= MaxValue)
                    {
                        MessageBox.Show("Value should be less than " + String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0:" + _FormatType + "}", MaxValue));
                        textBox.Text = string.Empty;
                    }

                }
            }

        }
        #endregion
    }
}