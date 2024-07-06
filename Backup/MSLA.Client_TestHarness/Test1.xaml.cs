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
using C1.Silverlight.DataGrid;

namespace MSLA.Client_TestHarness
{
    public partial class Test1 : MSLA.Client.Templates.MasterTemplate
    {
        DataGridComboBoxColumn _colOption = new DataGridComboBoxColumn();

        MSLA.Client.Data.DataTable dtOptionType = new Client.Data.DataTable();
        private MSLA.Client.Data.DataTable _dt = new Client.Data.DataTable();

        protected Test1()            
        {
            InitializeComponent();
        }

        protected override void InitializeMe()
        {

            label1.Content = MSLA.Client.DateFormat.SQLDateFormat;
            base.SaveButton = cmdSave;
            base.DeleteButton = cmdDelete;
            base.BrokenRuleGrid = dgvBrokenRule;
            base.Status = lblStatus;

            base.InitializeMe();
            //smartDatePicker1.DateFormat = MSLA.Client.DateFormat.BranchDateFormat;            

            FillOptionType();
        }

        void DataConnect_DataFetchCompletedForOptionType(object sender, MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs e)
        {
            dtOptionType = e.dtResult;
            this.RemoveFromSync("FillOptionType");
        }

        protected override string SaveSucceededMessage()
        {
            return "Saved";
        }
        private void FillOptionType()
        {

            this.AddToSync("FillOptionType");

            MSLA.Client.Data.DataCommand cmmOptionType = new MSLA.Client.Data.DataCommand();
            cmmOptionType.CommandText = "select fldOptionType_ID,fldOptionType from RWA.tblOptionType";
            cmmOptionType.ConnectionType = MSLA.Client.MSLAService.DBConnectionType.CompanyDB;
            cmmOptionType.CommandType = MSLA.Client.MSLAService.EnDataCommandType.Text;
            MSLA.Client.Data.DataConnect.FillDt(cmmOptionType, MSLA.Client.Login.LogonInfo.myUserInfo, new MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedHandler(DataConnect_DataFetchCompletedForOptionType));
        }

        void DataConnect_DataFetchCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs e)
        {
            _dt = e.dtResult;
            c1ComboBox1.DataContext = _dt;
            //c1ComboBox1.ItemsSource = _dt;
            //dgvCustomer.ItemsSource = dt;

            //if (dt.Rows.Count > 0)
            //{
            //    txtName.Text = (String)dt.Rows[0]["fldName"];
            //    txtAccID.Text = dt.Rows[0]["fldAccount_ID"].ToString();
            //}
            //dgvCustomer.UpdateLayout();
            //this.IsEnabled = true;
        }

        public Test1(string MasterType, Int64 MasterID,Dictionary<string, object> PropertyCollection)
            : base(MasterType, MasterID, PropertyCollection)
        {
            InitializeComponent();
            InitializeMe();
            //CreateDefaultRow();
        }
        private void CreateDefaultRow()
        {
            MSLA.Client.Data.DataTable dt = new MSLA.Client.Data.DataTable();
            //MSLA.Client.Data.DataTable dtTemp = (MSLA.Client.Data.DataTable)base.BOMaster.PropertyValue["TableOf_tblCustomerDetail"];
            //foreach (MSLA.Client.Data.DataColumn dc in dtTemp.Columns)
            //{
            //    dt.Columns.Add(dc);
            //}
            
            MSLA.Client.Data.DataRow dr = new MSLA.Client.Data.DataRow();
            //dr.Add("fldCounterPartyDetail_ID", string.Empty);
            //dr.Add("fldAccount_ID", -1);
            //dr.Add("fldAge", 0);

            //dr.Add("fldEmail", string.Empty);
            //dr.Add("fldSheet_ID", -1);
            //dr.Add("fldDataType_ID", -1);
            //dr.Add("fldLastupdated", System.DateTime.Now);
            dt.Rows.Add(dr);


            c1DataGrid1.ItemsSource = dt.Rows;
        }

        protected override void DoForwardBinding()
        {

            this.LayoutRoot.DataContext = null;
            this.LayoutRoot.DataContext = base.BOMaster;
            //this.dgvCustomer.ItemsSource = ((MSLA.Client.Data.DataTable)(base.BOMaster.PropertyBag["TableOf_tblCustomerDetail"])).Rows;
            
            this.LayoutRoot.UpdateLayout();
            base.DoForwardBinding();
            //c1DataGrid1.ItemsSource = ((MSLA.Client.Data.DataTable)(base.BOMaster.PropertyBag["TableOf_tblCustomerDetail"])).Rows;
        }

        protected override void OnBeforeSave(Client.Base.TemplateEventArgs e)
        {
            //if (smartNumericTextBox1.Text == string.Empty)
            //{
            //    base.BOMaster.PropertyValue["fldAccount_ID"] = 3;
            //}
            base.OnBeforeSave(e);
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            MSLA.Client.Data.DataColumn dc1;
            foreach (MSLA.Client.Data.DataColumn dc in _dt.Columns)
            {
                dc1 = new MSLA.Client.Data.DataColumn();
                dc1.ColumnName = dc.ColumnName;


                dc1.DataType = dc.DataType;
                (base.BOMaster.PropertyValue["tblCustomer"] as MSLA.Client.Data.DataTable).Columns.Add(dc1);
            }

            foreach (MSLA.Client.Data.DataRow dr in _dt.Rows)
            {
                MSLA.Client.Data.DataRow dr1 = new MSLA.Client.Data.DataRow();
                //foreach (KeyValuePair<string, object> KV in dr)
                //{
                //   // dr1.Add(KV.Key, KV.Value);

                //}
                (base.BOMaster.PropertyValue["tblCustomer"] as MSLA.Client.Data.DataTable).Rows.Add(dr1);
            }
        }

        private void c1DataGrid1_AutoGeneratingColumn(object sender, C1.Silverlight.DataGrid.DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = true;
        }

        private void c1DataGrid1_BeganEdit(object sender, DataGridBeganEditEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MSLA.Client.Data.DataTable dt = new MSLA.Client.Data.DataTable();
            //MSLA.Client.Data.DataTable dtTemp = (MSLA.Client.Data.DataTable)base.BOMaster.PropertyValue["TableOf_tblCustomerDetail"];
            //foreach (MSLA.Client.Data.DataColumn dc in dtTemp.Columns)
            //{
            //    dt.Columns.Add(dc);
            //}

            MSLA.Client.Data.DataRow dr = new MSLA.Client.Data.DataRow();
            //dr.Add("fldCounterPartyDetail_ID", string.Empty);
            //dr.Add("fldAccount_ID", -1);
            //dr.Add("fldAge", 0);

            //dr.Add("fldEmail", string.Empty);
            //dr.Add("fldSheet_ID", -1);
            //dr.Add("fldDataType_ID", -1);
            //dr.Add("fldLastupdated", System.DateTime.Now);
            dt.Rows.Add(dr);


            c1DataGrid1.ItemsSource = dt.Rows;
        }

        private void smartDatePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
