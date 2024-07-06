using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace MSLA.Client.SmartTools
{
    public partial class ItemSelector 
        : ChildWindow
    {
        private String _DisplayMember = String.Empty;
        private String _ValueMember = String.Empty;
        private String _CollectionMember = String.Empty;
        private String _PrefetchFilter = String.Empty;
        private MSLAService.DBConnectionType _cnType = MSLAService.DBConnectionType.CompanyDB;
        private Dictionary<Int64, String> _resultSet = new Dictionary<long, string>();
        private Dictionary<Int64, String> _filteredSet = new Dictionary<long, string>();

        private KeyValuePair<Int64, String> _SelectedItem = new KeyValuePair<long, string>(-1, string.Empty);
        public event EventHandler Selected;

        private List<String> values = new List<string>();

        public KeyValuePair<Int64, string> selectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; }
        }

        public ItemSelector()
        {
            InitializeComponent();
            DataTemplate itm = new DataTemplate();
            itm = (DataTemplate)XamlReader.Load(@"<DataTemplate xmlns=""http://schemas.microsoft.com/client/2007""><StackPanel><TextBlock Text=""{Binding Path=Value}""/></StackPanel></DataTemplate>");
            lstOptions.ItemTemplate = itm;
            rdContains.IsChecked = true;
        }

        public void initiate(string displayMember, string valueMember, string collectionMember, string prefetchFilter, MSLAService.DBConnectionType cnType)
        {
            _DisplayMember = displayMember;
            _ValueMember = valueMember;
            _CollectionMember = collectionMember;
            _cnType = cnType;
            _PrefetchFilter = prefetchFilter;
            Client.SmartTools.AutoCompleteBoxHelper.GetResultSet(_CollectionMember, _PrefetchFilter, _cnType, MSLA.Client.Login.LogonInfo.myUserInfo, _ValueMember, _DisplayMember, string.Empty, new MSLAService.MSLAServiceClient.GetRsltSetCompletedHandler(GetRsltCompletedAdd));
        }

        private void GetRsltCompletedAdd(Object sender, MSLAService.MSLAServiceClient.GetRsltSetCompletedEventArgs e)
        {
            _resultSet = e.result;
            this.DataContext = null;
            this.DataContext = _resultSet;
            if (_SelectedItem.Key != -1)
            {
                lstOptions.SelectedItem = _SelectedItem;
                txtSearch.Text = _SelectedItem.Value;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            if (lstOptions.SelectedItem != null)
            {
                _SelectedItem = (KeyValuePair<Int64, string>)lstOptions.SelectedItem;
                Selected(this, new EventArgs());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            applyFilter();
        }

        private void rd_Checked(object sender, RoutedEventArgs e)
        {
            applyFilter();
        }

        private void applyFilter()
        {
            if (rdStarts.IsChecked == true)
            {
                var eles =
                from entry in _resultSet
                where (entry.Value.StartsWith(txtSearch.Text,StringComparison.OrdinalIgnoreCase))
                select entry;
                this.DataContext = null;
                this.DataContext = eles;
            }
            else if (rdEnds.IsChecked == true)
            {
                var eles =
                    from entry in _resultSet
                    where (entry.Value.EndsWith(txtSearch.Text, StringComparison.OrdinalIgnoreCase))
                    select entry;
                this.DataContext = null;
                this.DataContext = eles;
            }
            else
            {
                var eles =
                    from entry in _resultSet
                    where ((entry.Value.ToLower()).Contains(txtSearch.Text.ToLower()))
                    select entry;
                this.DataContext = null;
                this.DataContext = eles;
            }
        }

        


    }
}

