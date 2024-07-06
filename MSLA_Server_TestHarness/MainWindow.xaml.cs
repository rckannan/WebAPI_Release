using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSLA_Server_TestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void cmdFetch_Click(object sender, RoutedEventArgs e)
        {
            MSLAService.MSLAServiceClient conn = new MSLAService.MSLAServiceClient();
            
            MSLAService.SimpleTable dt = conn.GetData("Select top 10 fldAccount_ID, fldDate, fldVoucher_ID, fldDebitAmt, fldCreditAmt From Accounts.tblGeneralLedger");
            dataGrid1.ItemsSource = dt.Rows;
            //textBox1.Text = dt.Rows.Count.ToString();
        }
    }
}
