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
using System.Windows.Data;
using System.Globalization;

namespace MSLA.Client_TestHarness
{
    public partial class MainPage : UserControl
    {
        private MSLA.Client.MSLAService.SimpleBOMaster _customer;
        private MSLA.Client.MSLAService.MasterCriteriaBase _BOCustomerCriteria;
        private OpenFileDialog odlg = new OpenFileDialog();
        MSLA.Client.Data.DataTable dtOptionType=new Client.Data.DataTable();

        public MainPage()
        {
            InitializeComponent();
            InitializeMe();           
        }

        private void InitializeMe()
        {
            //   Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy"
            //Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy"

            rdMyCustomers.IsChecked = true;
        }
        private void cmdLogin_Click(object sender, RoutedEventArgs e)
        {
            MSLA.Client.Login.LogonInfo.TryLoginUPA(txtUserName.Text);
            //MSLA.Client.Login.LogonInfo.TryLogin(txtUserName.Text);
            cmdLogin.IsEnabled = false;
            MSLA.Client.Login.LogonInfo.AuthenticationCompleted += new EventHandler(LogonInfo_AuthenticationCompleted);
        }

        
        private void cmbOptionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (c1DataGrid1.SelectedItem != null)
            {
                ComboBox cmb = (ComboBox)sender;
                //MSLA.Client.Data.DataRow dr = (MSLA.Client.Data.DataRow)c1DataGrid1.SelectedItem;
                //dr.RowValue["fldOptionType_ID"] = cmb.SelectedValue.ToString();
            }
        }

        private void FillGrid()
        {
            MSLA.Client.Data.DataTable dt = new Client.Data.DataTable();

            MSLA.Client.Data.DataRow dr = new Client.Data.DataRow(); 
            int i = 0;
            for (i = 0; i <= 20; i++)
            {
                dr = new MSLA.Client.Data.DataRow();

                dr.Add("fldOptionType_ID", -1);
                dr.Add("fldIsBarrier", 0);
                dt.Rows.Add(dr);
            }
            c1DataGrid1.ItemsSource = MSLA.Client.Data.C1DataTable.GetC1DataTable(dt).DefaultView;
        }

        private void cmbOptionType_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cmbOptionType = sender as ComboBox;
            cmbOptionType.ItemsSource = MSLA.Client.Data.C1DataTable.GetC1DataTable(dtOptionType).DefaultView;
        }

        void DataConnect_DataFetchCompletedForOptionType(object sender, MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs e)
        {
            dtOptionType = e.dtResult;
        }


        private void FillOptionType()
        {
            MSLA.Client.Data.DataCommand cmmOptionType = new MSLA.Client.Data.DataCommand();
            cmmOptionType.CommandText = "select fldOptionType_ID,fldOptionType from RWA.tblOptionType";
            cmmOptionType.ConnectionType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
            cmmOptionType.CommandType = MSLA.Client.MSLAService.EnDataCommandType.Text;
            MSLA.Client.Data.DataConnect.FillDt(cmmOptionType, MSLA.Client.Login.LogonInfo.myUserInfo, new MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedHandler(DataConnect_DataFetchCompletedForOptionType));
        }

        void LogonInfo_AuthenticationCompleted(object sender, EventArgs e)
        {
            if (MSLA.Client.Login.LogonInfo.myUserInfo.User_ID != -1)
            {
                FillGrid();
                MSLA.Client.Data.DataTable dt = new Client.Data.DataTable();

                MSLA.Client.DateFormat df = new Client.DateFormat();

                //smartDatePicker1.DateFormat = MSLA.Client.DateFormat.BranchDateFormat;

                //string str= MSLA.Client.Constants.SQLDateFormat;

                //MSLA.Client.Data.DataCommand cmm = new Client.Data.DataCommand();
                //cmm.CommandText = "select * from dbo.tblCustomerDetail";
                //cmm.ConnectionType = Client.MSLAService.DBConnectionType.CompanyDB;
                //cmm.CommandType = Client.MSLAService.EnDataCommandType.Text;

                //param._ParameterName = "@Name";
                //param._DBType = Client.MSLAService.DataParameter.EnDataParameterType.VarChar;
                //param._Size = 50;
                //param._Value = string.Empty;
                //param._Direction = Client.MSLAService.DataParameter.EnParameterDirection.InputOutput;
                //cmm.Parameters.Add(param);

                //param = new Client.MSLAService.DataParameter();

                //param._ParameterName = "@Account_ID";
                //param._DBType = Client.MSLAService.DataParameter.EnDataParameterType.BigInt;
                //param._Size = 0;
                //param._Value = 1;
                //param._Direction = Client.MSLAService.DataParameter.EnParameterDirection.Input;
                //cmm.Parameters.Add(param);


                //Client.Data.DataConnect.FillDt(cmm, MSLA.Client.Login.LogonInfo.myUserInfo, new Client.MSLAService.MSLAServiceClient.DataFetchCompletedHandler(DataConnect_DataFetchCompleted));

                FillOptionType();
                MSLA.Client.Data.DataCommand cmm = new MSLA.Client.Data.DataCommand();
                cmm.CommandText = "select fldCounterParty_ID,fldCounterPartyName from Mapping.tblCounterPartyInfo";
                cmm.ConnectionType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
                cmm.CommandType = MSLA.Client.MSLAService.EnDataCommandType.Text;


                //Client.Data.DataConnect.ExecCMM(cmm, MSLA.Client.Login.LogonInfo.myUserInfo, new Client.MSLAService.MSLAServiceClient.ExecCMMCompletedHandler(DataConnect_ExcCMMFetchCompleted));


                Client.Data.DataConnect.FillDt(cmm, MSLA.Client.Login.LogonInfo.myUserInfo, new MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedHandler(DataConnect_DataFetchCompleted));

                FillCustomers();
                //smtCmbCustomer.CollectionMember = "RWA.vewPrefferedCustomer";
                //smtCmbCustomer.cnType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
                //smtCmbCustomer.ValueMember = "fldCustomerLiability_ID";
                //smtCmbCustomer.DisplayMember = "fldCustomerLiabilityName";

                smartAutoCompleteBox1.CollectionMember = "Mapping.tblCounterPartyInfo";
                smartAutoCompleteBox1.cnType = Client.MSLAService.DBConnectionType.CompanyDB;
                smartAutoCompleteBox1.ValueMember = "fldCounterParty_ID";
                smartAutoCompleteBox1.DisplayMember = "fldCounterPartyName";


                smartComboCumAutoCompleteBox1.CollectionMember = "RWA.tblCurrency";
                smartComboCumAutoCompleteBox1.cnType = Client.MSLAService.DBConnectionType.CompanyDB;
                smartComboCumAutoCompleteBox1.ValueMember = "fldCurrency_ID";
                smartComboCumAutoCompleteBox1.DisplayMember = "fldCurrencyName";

                smartComboCumAutoCompleteBox1.SelectedValue = 1000001;
                //smartComboCumAutoCompleteBox3.CollectionMember = "RWA.vewPrefferedCustomer";
                //smartComboCumAutoCompleteBox3.cnType = Client.MSLAService.DBConnectionType.CompanyDB;
                //smartComboCumAutoCompleteBox3.ValueMember = "fldCustomerLiability_ID";
                //smartComboCumAutoCompleteBox3.DisplayMember = "fldCustomerLiabilityName";

                //smartComboCumAutoCompleteBox2.CollectionMember = "RWA.vewCustomer";
                //smartComboCumAutoCompleteBox2.cnType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
                //smartComboCumAutoCompleteBox2.ValueMember = "fldCustomer_ID";
                //smartComboCumAutoCompleteBox2.DisplayMember = "fldCustomerFullNameMnemonic";

                //smtCmbEquityName.CollectionMember = "RWA.tblEquityList";
                //smtCmbEquityName.cnType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
                //smtCmbEquityName.ValueMember = "fldEquity_ID";
                //smtCmbEquityName.DisplayMember = "fldEquityName";

                //smtCmbCustomer.CollectionMember = "RWA.vewCustomer";
                //smtCmbCustomer.cnType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
                //smtCmbCustomer.ValueMember = "fldCustomer_ID";
                //smtCmbCustomer.DisplayMember = "fldCustomerFullNameMnemonic";

                //label3.Content = MSLA.Client.DateFormat.SQLDateFormat;
                //Dictionary<string, object> PropCollection = new Dictionary<string, object>();
                ////PropCollection.Add("RefCust_ID", 1);
                //Test1 tst1 = new Test1("Customer", -1, PropCollection);
                //LayoutRoot.Children.Add(tst1);
                //MSLA.Client.RSSWorker.GetFeedItems(1, MSLA.Client.Login.LogonInfo.myUserInfo, GetFeedItemsHandler);

                //cmm = new MSLA.Client.Data.DataCommand();
                //cmm.CommandText = "select fldCounterPartyName from Mapping.tblCounterPartyInfo where fldCounterParty_ID=1";
                //cmm.ConnectionType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
                //cmm.CommandType = MSLA.Client.MSLAService.EnDataCommandType.Text;
                //Client.Data.DataConnect.ExecuteScalar(cmm, MSLA.Client.Login.LogonInfo.myUserInfo, new MSLA.Client.MSLAService.MSLAServiceClient.ExecScalarCompletedHandler(ExecScalarCompleted));

                MSLA.Client.Utilities.GetServiceInfo(MSLA.Client.Login.LogonInfo.myUserInfo,
                    new Client.MSLAService.MSLAServiceClient.GetServiceInformationCompletedHandler(GetServiceInfoCompleted));

            }
            else
            {
                txtpassword.Text = "";
            }
            cmdLogin.IsEnabled = true;
        }

        private void GetServiceInfoCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.GetServiceInformationCompletedEventArgs e)
        {
            c1DataGrid1.ItemsSource = MSLA.Client.Data.C1DataTable.GetC1DataTable(e.result).DefaultView;
            gridExportTool1.C1DataGrid = c1DataGrid1;
            gridExportTool1.Title = "Service Information";
        }

        private void ExecScalarCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.ExecScalarCompletedEventArgs e)
        {

        }

        private void rdMyCustomers_Checked(object sender, RoutedEventArgs e)
        {
            FillCustomers();           
        }

        private void rdAllCustomers_Checked(object sender, RoutedEventArgs e)
        {
            FillCustomers();
        }

        private void GetFeedItemsHandler(object sender,MSLA.Client.MSLAService.MSLAServiceClient.GetFeedItemsCompletedEventArgs e)
        {
            c1DataGrid1.ItemsSource = e.result;
        }

        private void FillCustomers()
        {
            if (rdAllCustomers.IsChecked == true)
            {
                smtcmbCustomer.ResetItemSource();
                smtcmbCustomer.CollectionMember = "RWA.tblCustomerLiability";
                smtcmbCustomer.cnType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
                smtcmbCustomer.ValueMember = "fldCustomerLiability_ID";
                smtcmbCustomer.DisplayMember = "fldCustomerLiabilityName";
            }
            else
            {
                smtcmbCustomer.ResetItemSource();
                smtcmbCustomer.CollectionMember = "RWA.vewPrefferedCustomer";
                smtcmbCustomer.cnType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
                smtcmbCustomer.ValueMember = "fldCustomerLiability_ID";
                smtcmbCustomer.DisplayMember = "fldCustomerLiabilityName";
            }
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //MSLA.Client.Data.DataCommand cmm = new MSLA.Client.Data.DataCommand();
            //cmm.CommandType = MSLA.Client.MSLAService.EnDataCommandType.Text;
            //cmm.CommandText = "exec Mapping.spGetDataImportDetails 5,'FX Forward Trades','08/23/2010'  ";
            //cmm.ConnectionType = Client.MSLAService.DBConnectionType.CompanyDB;

            //MSLA.Client.MSLAService.DataParameter param = new Client.MSLAService.DataParameter();
            //param._DBType = MSLA.Client.MSLAService.DataParameter.EnDataParameterType.BigInt;
            //param._Value = 1;
            //param._ParameterName = "@Account_ID";
            //param._Direction = Client.MSLAService.DataParameter.EnParameterDirection.Input;


            //cmm.Parameters.Add(param);

            //Client.Data.DataConnect.FillDt(cmm, MSLA.Client.Login.LogonInfo.myUserInfo, new Client.MSLAService.MSLAServiceClient.DataFetchCompletedHandler(DataConnect_DataFetchCompleted));

            //this.IsEnabled = false;
        }

        //private void cmdGet_Click(object sender, RoutedEventArgs e)
        //{

        //    MSLA.Client.MSLAService.MasterCriteriaBase clCriteria=new MSLA.Client.MSLAService.MasterCriteriaBase(1,"Customer",MSLA.Client.Login.LogonInfo.myUserInfo.User_ID,MSLA.Client.Login.LogonInfo.myUserInfo.Session_ID);
        //    clCriteria.ProprtyCollection.Add("Customer_Name", "default name.... ");
        //    MSLA.Client.MSLAService.SimpleBOMaster.FetchBOMasterCompleted+=new EventHandler(BOMaster_FetchBOMasterCompleted);
        //    MSLA.Client.MSLAService.SimpleBOMaster.FetchBOMaster(clCriteria);

        //    //MSLA.Client.Data.DataCommand cmm = new MSLA.Client.Data.DataCommand();
        //    //cmm.DataCommandType = MSLA.Client.MSLAService.DataCommand.EnDataCommandType.StoredProcedure;
        //    //cmm.DataCommandText = "dbo.spCustomerFetch";
        //    //cmm.ConnectionType = Client.MSLAService.DBConnectionType.CompanyDB;



        //    //MSLA.Client.MSLAService.DataParameter param = new Client.MSLAService.DataParameter();
        //    //param._DBType = MSLA.Client.MSLAService.DataParameter.EnDataParameterType.BigInt;
        //    //param._Value = 1;
        //    //param._ParameterName = "@Account_ID";
        //    //param._Direction = Client.MSLAService.DataParameter.EnParameterDirection.Input;


        //    //cmm.DataParameters.Add(param);
           
        //    //Client.Data.DataConnect.FillDt(cmm, MSLA.Client.Login.LogonInfo.myUserInfo, new Client.MSLAService.MSLAServiceClient.DataFetchCompletedHandler(DataConnect_DataFetchCompleted));

        //    //this.IsEnabled = false;

        //}

        //void BOMaster_FetchBOMasterCompleted(object sender, EventArgs e)
        //{
        //    _customer = MSLA.Client.MSLAService.SimpleBOMaster.resultBO;            
        //    txtName.Text = _customer.PropertyValue["fldName"].ToString();
        //    txtAccID.Text=_customer.PropertyValue["fldAccount_ID"].ToString();
        //    //cmbEnType.SelectedValue
        //    dgvCustomer.ItemsSource = MSLA.Client.Data.DataConnect.GetResolvedTable((_customer.PropertyValue["TableOf_tblCustomerDetail"]) as MSLA.Client.MSLAService.SimpleTable);          
        //    //dgvCustomer.set
        //}


        void DataConnect_ExcCMMFetchCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.ExecCMMCompletedEventArgs e)
        {
        }


        void DataConnect_DataFetchCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs e)
        {
            MSLA.Client.Data.DataTable dt = e.dtResult;

            //C1.Silverlight.Data.DataTable dt1 = new C1.Silverlight.Data.DataTable();

            //C1.Silverlight.Data.DataRow drnew;

            //dt1.Columns.Add("fldCounterPartyName", Type.GetType("String"));

            //foreach (MSLA.Client.Data.DataRow dr1 in dt.Rows)
            //{
            //    drnew = dt1.NewRow();
            //    drnew["fldCounterPartyName"] = dr1["fldCounterPartyName"];
            //    dt1.Rows.Add(drnew);
            //}

            //dt1.AcceptChanges();
            //c1DataGrid1.ItemsSource = MSLA.Client.Data.C1DataTable.GetC1DataTable(dt).DefaultView;


            //dataGrid1.ItemsSource = dt;

            // Define the query expression.
            //IEnumerable<MSLA.Client.Data.DataRow> scoreQuery =
            //    from score in dt.Rows
            //    where (Int64)score.RowValue["fldAccount_ID"] == 1
            //    select score;
            //MSLA.Client.Data.DataTable dtnew = new Client.Data.DataTable();
            //foreach (MSLA.Client.Data.DataRow dr in scoreQuery)
            //{               
            //    dtnew.Rows.Add(dr);
            //}
            //dgvTest.ItemsSource = dtnew;
            //listBox1.ItemsSource = scoreQuery;
            //listBox1.DisplayMemberPath = "fldEmail";
            // Execute the query.
            //foreach (int i in scoreQuery)
            //{
            //    Console.Write(i + " ");
            //}


            //if (dt.Rows.Count > 0)
            //{
            //    txtName.Text = (String)dt.Rows[0]["fldName"];
            //    txtAccID.Text = dt.Rows[0]["fldAccount_ID"].ToString();
            //}
            //dgvCustomer.UpdateLayout();
            this.IsEnabled = true;
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            //smartComboCumAutoCompleteBox1.SelectedValue = -1;
            Dictionary<string, object> PropCollection = new Dictionary<string, object>();
            //PropCollection.Add("RefCust_ID", 1);
            Test1 tst1 = new Test1("Customer1", -1, PropCollection);
            LayoutRoot.Children.Add(tst1);
            //odlg.Filter = "Excel File (*.xls)|*.xls|CSV files (*.csv)|*.csv|XML files (*.xml)|*.xml";

            //odlg.Multiselect = false;
            //if ((bool)odlg.ShowDialog())
            //{
            //    System.IO.Stream fileStream = odlg.File.OpenRead();
            //    byte[] byteArr = new byte[fileStream.Length];
            //    fileStream.Read(byteArr, 0, Convert.ToInt32(fileStream.Length - 1));
            //    //lstFileName.Items.Add(str.Name);
            //    fileStream.Flush();
            //    fileStream.Close();
            //    Client.Utilities.ResolveTablesFromFile(byteArr, odlg.File.Extension, "", Client.Login.LogonInfo.myUserInfo, Utilities_ResolveFileCompleted);
            //}
            //button1.IsEnabled = false;

            //MessageBox.Show(smartAutoCompleteBox1.SelectedText);
        }

        void Utilities_ResolveFileCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.ResolveFileCompletedEventArgs e)
        {
            Dictionary<string, MSLA.Client.Data.DataTable> TableList = new Dictionary<string, Client.Data.DataTable>();
            TableList = e.TableCollection;
            MSLA.Client.Data.DataTable dt = new Client.Data.DataTable();
            foreach (System.Collections.Generic.KeyValuePair<string, MSLA.Client.Data.DataTable> kv in TableList)
            {
                dt = kv.Value;
            }
            MSLA.Client.Data.DataTable dtCPDetails = dt;
            dataGrid1.Columns.Clear();
            dataGrid1.ItemsSource = null;
            if (dtCPDetails.Rows.Count == 0)
            { MessageBox.Show("No data found", "CounterParty Data", MessageBoxButton.OK); }
            else
            {
                for (int i = 0; i < dtCPDetails.Columns.Count; i++)
                {
                    DataGridTextColumn TextColumn = new DataGridTextColumn();
                    TextColumn.Header = dtCPDetails.Columns[i].ColumnName.ToString();
                    TextColumn.Binding = new Binding("RowValue[" + dtCPDetails.Columns[i].ColumnName.ToString() + "]");
                    dataGrid1.Columns.Add(TextColumn);
                }
                dataGrid1.ItemsSource = dtCPDetails.Rows;
            }

            button1.IsEnabled = true;
        }


        void DataConnect_ExecCMMCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.ExecCMMCompletedEventArgs e)
        {
            System.Collections.Generic.List<MSLA.Client.MSLAService.DataParameter> paramList = new List<Client.MSLAService.DataParameter>();

            paramList = e.ParamCollection;

            //txtName.Text = ((MSLA.Client.MSLAService.DataParameter)paramList[0])._Value.ToString();
        }

        private void smartComboCumAutoCompleteBox1_SelectedItemChanged(object sender, EventArgs e)
        {
          
        }

        
        //private void cmdSave_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_customer != null)
        //    {
        //        _customer.PropertyValue["fldName"] = txtName.Text;
        //        _customer.PropertyValue["fldAccount_ID"] = txtAccID.Text;
        //        _customer.PropertyValue["fldEnType"] = 1;
        //        _customer.PropertyValue["TableOf_tblCustomerDetail"] = Client.Data.DataConnect.ResolveToSimpleTable((Client.Data.DataTable)(dgvCustomer.ItemsSource));
        //        MSLA.Client.MSLAService.SimpleBOMaster.SaveBOMasterCompleted += new EventHandler(BOMaster_SaveBOMasterCompleted);
        //        MSLA.Client.MSLAService.SimpleBOMaster.SaveBOMaster(_customer);
        //    }
        //}

        //void BOMaster_SaveBOMasterCompleted(object sender, EventArgs e)
        //{
        //    _customer = MSLA.Client.MSLAService.SimpleBOMaster.SaveBO;

        //    txtName.Text = _customer.PropertyValue["fldName"].ToString();
        //    txtAccID.Text = _customer.PropertyValue["fldAccount_ID"].ToString();

        //    if (_customer.BrokenSaveRules.Count == 0)
        //    {
        //        lblStatus.Content = "Saved";
        //    }
        //    else
        //    {
        //        foreach (MSLA.Client.MSLAService.BrokenRule brRule in _customer.BrokenSaveRules)
        //        {
        //            lblStatus.Content += brRule._RuleDesc;
        //        }
        //    }
        //}       
    }
}
