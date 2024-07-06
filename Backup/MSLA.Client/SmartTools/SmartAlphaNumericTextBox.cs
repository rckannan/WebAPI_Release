using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;

namespace MSLA.Client.SmartTools
{
    public class SmartAlphaNumericTextBox
        : TextBox
    {
        #region "Private Variables"
        private bool _IsFocused = false;
        private bool _NegativeAllowed = false;
        private bool _comma = false;
        private Int32 _DecimalPlaces;
        private string _FormatType = string.Empty;
        private List<Key> LegalKeys = new List<Key> { Key.B, Key.M, Key.T };
        private decimal _MaxValue = decimal.MaxValue;
        #endregion

        #region "Constructor"
        public SmartAlphaNumericTextBox(bool ShowComma, Int32 NoOfDecPlaces, bool AllowNegative)
            : base()
        {
            InitialiseControl(ShowComma, NoOfDecPlaces, AllowNegative);
        }

        public SmartAlphaNumericTextBox()
            : this(false, 0, false)
        {

        }

        protected void InitialiseControl(bool AllowComma, Int32 NoOfDecPlaces, bool AllowNegative)
        {
            this.TextAlignment = System.Windows.TextAlignment.Right;
            _comma = AllowComma;
            if (NoOfDecPlaces > 8 | NoOfDecPlaces < 0)
            {
                throw new Exception("ERROR. Maximum number of decimal places is restricted to 8 digits.");
            }
            _DecimalPlaces = NoOfDecPlaces;

            //   ****    Set the maxlength
            _FormatType = SetFormatType();
            _NegativeAllowed = AllowNegative;
        }
        #endregion

        #region "Text Box Properites / Methods"
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

        public bool Comma
        {
            get { return _comma; }
            set
            {
                _comma = value;
                _FormatType = SetFormatType();
            }
        }

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

            // New code
            decimal result = 0;
            if (decimal.TryParse(this.Text, out result))
            {
                this.Text = String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0:" + _FormatType + "}", result);
            }
            return FormatType;
        }

        [TypeConverter(typeof(DecimalTypeConverter))]
        public decimal MaxValue
        {
            get { return _MaxValue; }
            set { _MaxValue = value; }

        }

        public bool DisplayZeroAsEmpty
        {
            get { return (bool)GetValue(DisplayZeroAsEmptyProperty); }
            set { SetValue(DisplayZeroAsEmptyProperty, value); }
        }

        public static readonly DependencyProperty DisplayZeroAsEmptyProperty = DependencyProperty.Register
           (
                "DisplayZeroAsEmpty",
                typeof(bool),
                typeof(SmartAlphaNumericTextBox),
                new PropertyMetadata(false)
           );

        public static readonly DependencyProperty TextAsDecimalProperty = DependencyProperty.Register
           (
                "TextAsDecimal",
                typeof(decimal),
                typeof(SmartAlphaNumericTextBox),
                new PropertyMetadata(decimal.Zero, onTextAsDecimalChanged)
           );


        private static void onTextAsDecimalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SmartAlphaNumericTextBox obj = d as SmartAlphaNumericTextBox;
            obj.TextAsDecimalChangedInternal(Convert.ToDecimal(e.NewValue));
        }

        private void TextAsDecimalChangedInternal(decimal NewValue)
        {
            string cValue = String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0:" + this._FormatType + "}", NewValue);

            if (Convert.ToDecimal(NewValue) == 0)
            {
                if (this.DisplayZeroAsEmpty)
                {
                    cValue = string.Empty;
                }
            }
            this.SetValue(TextProperty, cValue);
        }

        public decimal TextAsDecimal
        {
            get { return (decimal)GetValue(TextAsDecimalProperty); }
            set { SetValue(TextAsDecimalProperty, value); }
        }
        #endregion

        #region events
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            _IsFocused = true;
            this.SelectAll();
            base.OnGotFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            int keyAscii = -1;
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
                    if (this.Text.Contains("m") | this.Text.Contains("b") | this.Text.Contains("t") | this.Text.Contains("M") | this.Text.Contains("B") | this.Text.Contains("T"))
                    {
                        if (this.SelectionStart == this.Text.Length)
                        {
                            keyAscii = 0;
                        }
                    }
                    break;
                case Key.M:
                case Key.B:
                case Key.T:
                    if (this.SelectionStart == 0 | this.Text.Contains("m") | this.Text.Contains("b") | this.Text.Contains("t") | this.Text.Contains("M") | this.Text.Contains("B") | this.Text.Contains("T"))
                    {
                        keyAscii = 0;
                    }
                    break;
                case Key.Subtract:

                    //   ****    If in first position and Negative does not already exist then allow
                    if (_NegativeAllowed)
                    {
                        if (this.SelectionLength != this.Text.Length & (this.SelectionStart != 0 | this.Text.Contains("-")))
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
                    if (this.Text.Contains(".") & this.SelectionLength != this.Text.Length)
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
                            if (this.SelectionLength != this.Text.Length & (this.SelectionStart != 0 | this.Text.Contains("-")))
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
                        if (this.Text.Contains(".") & this.SelectionLength != this.Text.Length)
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
                    if (this.SelectionStart == 0 | this.Text.Contains("m") | this.Text.Contains("b") | this.Text.Contains("t") | this.Text.Contains("M") | this.Text.Contains("B") | this.Text.Contains("T"))
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
            base.OnKeyDown(e);
        }

        protected override void OnTextInputUpdate(TextCompositionEventArgs e)
        {
            base.OnTextInputUpdate(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            Boolean HasChar = true;
            String str = string.Empty;
            decimal result = 0;
            decimal Multiplier = 1;

            if (this.Text.Contains("T") || this.Text.Contains("t"))
            {
                Multiplier = 1000;
            }
            else if (this.Text.Contains("M") || this.Text.Contains("m"))
            {
                Multiplier = 1000000;
            }
            else if (this.Text.Contains("B") || this.Text.Contains("b"))
            {
                Multiplier = 1000000000;
            }
            else
            {
                HasChar = false;
            }

            if (HasChar)
            {
                str = this.Text.Substring(0, this.Text.Length - 1);
            }
            else
            {
                str = this.Text;
            }

            decimal.TryParse(str, out result);
            result = result * Multiplier;

            if (result >= MaxValue)
            {
                MessageBox.Show("Value should be less than " + String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0:" + _FormatType + "}", MaxValue));
                result = 0;
            }
            if (this.TextAsDecimal == result)
            {
                TextAsDecimalChangedInternal(result);
            }
            this.TextAsDecimal = result;
            base.OnLostFocus(e);
        }
        #endregion
    }
}
