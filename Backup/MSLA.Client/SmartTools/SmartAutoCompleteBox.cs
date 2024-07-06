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
using System.Windows.Markup;
using System.IO;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;


namespace MSLA.Client.SmartTools
{
    [TemplatePart(Name = "OpenPopup", Type = typeof(Button))]
    [TemplatePart(Name = "PopupBorder", Type = typeof(Border))]
    public class SmartAutoCompleteBox
          :AutoCompleteBox
    {

        public SmartAutoCompleteBox()
        {
            this.FilterMode = AutoCompleteFilterMode.StartsWith;
            this.Resources.Add("conv", new ValueConverter());
            Binding bnd = new Binding();
            bnd.Converter = new ValueConverter();
            this.ValueMemberBinding = bnd;
            Binding bnd2 = new Binding();
            DataTemplate itm = new DataTemplate();
            itm = (DataTemplate)XamlReader.Load(@"<DataTemplate xmlns=""http://schemas.microsoft.com/client/2007""><StackPanel><TextBlock Text=""{Binding Path=Value}""/></StackPanel></DataTemplate>");
            this.ItemTemplate = itm;
        }
        
        private String _DisplayMember = String.Empty;
        private String _ValueMember = String.Empty;
        private String _CollectionMember = String.Empty;
        private String _PrefetchFilter = String.Empty;
        private MSLAService.DBConnectionType _cnType = MSLAService.DBConnectionType.CompanyDB;
        private Int64 _SelectedValue = -1;
        private String _SelectedText = String.Empty;
        private Dictionary<Int64, String> _resultSet = new Dictionary<long, string>();
        private Boolean _IsManualPopup = false;
        private Popup popup = new Popup();
        private Border border = new Border();
        private ItemSelector w;
        private Boolean _AllowF8 = true;
        private Boolean _IsInitialised = false;

        public Boolean ArrowClicked = false;

        public event EventHandler SelectedItemChanged;
        public event EventHandler ExplicitInitCompleted;
        public event EventHandler SelectedTextRefreshed;

        public Boolean IsManualPopup
        {
            get { return _IsManualPopup; }
            set { _IsManualPopup = value; }
        }

        public String DisplayMember
        {
            get { return _DisplayMember; }
            set { _DisplayMember = value; }
        }

        public String ValueMember
        {
            get { return _ValueMember; }
            set { _ValueMember = value; }
        }

        internal void ResetItemSource()
        {
            Client.Repository.getInstance().ForceRefresh(CollectionMember);
            _resultSet.Clear();
            _IsInitialised = false;
            this.SelectedItem = null;
            _SelectedValue = -1;
            _SelectedText = String.Empty;
            this.DataContext = null;
        }

        public String CollectionMember
        {
            get { return _CollectionMember; }
            set { _CollectionMember = value; }
        }

        public Dictionary<Int64, String> SourceData
        {
            get { return _resultSet; }
        }

        public String PrefetchFilter
        {
            get { return _PrefetchFilter; }
            set { _PrefetchFilter = value; }
        }

        public MSLAService.DBConnectionType cnType
        {
            get { return _cnType; }
            set { _cnType = value; }
        }

        public Boolean AllowF8
        {
            get { return _AllowF8; }
            set { _AllowF8 = value; }
        }

        public Int64 SelectedValue
        {
            get 
            {
                if (this.SelectedItem != null)
                {
                    if (this.SelectedItem is KeyValuePair<Int64, String>)
                    {
                        _SelectedValue = ((KeyValuePair<Int64, String>)this.SelectedItem).Key; 
                    }
                }
                return _SelectedValue; 
            }
            set 
            {              
                _SelectedValue = value;
                if (SelectedItemChanged != null)
                {
                    SelectedItemChanged.Invoke(this, new EventArgs());
                }
                PreInit();
            }
        }

        public String SelectedText
        {
            get { return this.Text; }
        }

        public void ExplicitInit()
        {
            if (ValueMember != string.Empty && DisplayMember != string.Empty && this.CollectionMember!=string.Empty)
            {
                Client.SmartTools.AutoCompleteBoxHelper.GetResultSet(CollectionMember, PrefetchFilter, cnType, MSLA.Client.Login.LogonInfo.myUserInfo, 
                    ValueMember, DisplayMember, string.Empty, new MSLAService.MSLAServiceClient.GetRsltSetCompletedHandler(ExplicitRsltCompletedAdd));
            }
        }

        private void PreInit()
        {
            if (ValueMember != string.Empty && DisplayMember != string.Empty)
            {
                Client.SmartTools.AutoCompleteBoxHelper.GetSelectText(CollectionMember, PrefetchFilter, cnType, 
                    MSLA.Client.Login.LogonInfo.myUserInfo, ValueMember, DisplayMember, _SelectedValue, 
                    new MSLAService.MSLAServiceClient.GetSelectTxtCompletedHandler(GetRsltText));
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (this.Text != String.Empty && _resultSet.Count!=0 && !_resultSet.ContainsValue(this.Text))
            {
                this.Background = new SolidColorBrush(Color.FromArgb(100,255,0,0));
                this.Text = string.Empty;
                this.SelectedValue = -1;
            }
            else
            {
                this.Background = new SolidColorBrush(Colors.White);
            }
            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            this.Background = new SolidColorBrush(Colors.White);
            base.OnGotFocus(e);
        }

        public void ShowOptions()
        {
            if (!_IsInitialised)
            {
                this.ArrowClicked = true;
                ExplicitInit();
            }
                this.IsDropDownOpen = true;
        }

        private void GetRsltText(Object sender, MSLAService.MSLAServiceClient.GetSelectTxtCompletedEventArgs e)
        {
            this.Text = e.result;
            if (this.SelectedTextRefreshed != null)
            {
                SelectedTextRefreshed.Invoke(this, new EventArgs());
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
             if (ValueMember != string.Empty && DisplayMember != string.Empty && _AllowF8==true &&  e.Key == Key.F8 )
            {               
                w = new ItemSelector();
                w.initiate(DisplayMember, ValueMember, CollectionMember, PrefetchFilter, cnType);
                if (this.SelectedItem != null)
                    w.selectedItem = (KeyValuePair<long,string>)this.SelectedItem;
                w.Show();
                w.Selected += new EventHandler(w_Selected);               
            }
             else if (ValueMember != string.Empty && DisplayMember != string.Empty && this.Text != string.Empty)
             {
                 if (this.Text.Length == 1)
                 {
                     if (!_IsInitialised)
                     {
                         ArrowClicked = true;
                         ExplicitInit();
                     }
                 }
             }

            if (this.Text == string.Empty)
            {
                this.SelectedValue = -1;
            }

            if (e.Key == Key.Enter && this.Text != String.Empty && this.SelectedValue == -1)
            {
                this.Background = new SolidColorBrush(Color.FromArgb(100,255,0,0));
                this.Text = string.Empty;
            }
            else
            {
                this.Background = new SolidColorBrush(Colors.White);
            }
            base.OnKeyUp(e);            
        }

        private void w_Selected(object sender, EventArgs e)
        {
            if (!_IsInitialised)
            {
                ExplicitInit();
            }

            this.SelectedItem = w.selectedItem;
            this.Background = new SolidColorBrush(Colors.White);

            if (SelectedItemChanged != null)
            {
                SelectedItemChanged.Invoke(this, e);
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged.Invoke(this, e);
            }
        }

        private void ExplicitRsltCompletedAdd(Object sender, MSLAService.MSLAServiceClient.GetRsltSetCompletedEventArgs e)
        {
            _resultSet = e.result;
            _IsInitialised = true;

            this.DataContext = null;            
            this.DataContext = _resultSet;
            this.PopulateComplete();

            if (!ArrowClicked)
            {
                this.IsDropDownOpen = true;
                this.IsDropDownOpen = false;
            }
            else
            { ArrowClicked = false; }

            if (ExplicitInitCompleted != null)
            {
                ExplicitInitCompleted.Invoke(this, new EventArgs());
            }
        }
    }

    public class ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return ((KeyValuePair<Int64, String>)value).Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
