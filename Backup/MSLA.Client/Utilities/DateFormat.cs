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
using System.Threading;
using System.Globalization;



namespace MSLA.Client
{
    public class DateFormat
    {
        //private static String _DateFormat = string.Empty;

        public static String SQLDateFormat
        {
            get { return "dd MMM, yyyy"; }
            //set { _DateFormat = value; }
        }

        private static String _BranchDateFormat = string.Empty;

        public static String BranchDateFormat
        {
            get { return _BranchDateFormat; }
            set { _BranchDateFormat = value; }
        }

        public DateFormat()
        {
            //SetDateFormat();
            SetBranchDateFormat();
        }

        //private void SetDateFormat()
        //{
        //    Data.DataTable dtDateFormat = new Data.DataTable();

        //    Data.DataCommand cmm = new Data.DataCommand();
        //    cmm.CommandText = "Select fldValue from System.tblSettings where fldCode='CN' and fldKey='SQLDateFormat'";
        //    cmm.CommandType = MSLAService.EnDataCommandType.Text;
        //    cmm.ConnectionType = MSLAService.DBConnectionType.CompanyDB;

        //    Client.Data.DataConnect.FillDt(cmm, MSLA.Client.Login.LogonInfo.myUserInfo, new Client.MSLAService.MSLAServiceClient.DataFetchCompletedHandler(DataConnect_DataFetchCompleted));
        //}

        //void DataConnect_DataFetchCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs e)
        //{
        //    MSLA.Client.Data.DataTable dt = e.dtResult;

        //    if (dt.Rows.Count > 0)
        //    {
        //        _DateFormat = (string)dt[0]["fldValue"];
        //    }
        //}

        private void SetBranchDateFormat()
        {
            Data.DataTable dtDateFormat = new Data.DataTable();

            Data.DataCommand cmm = new Data.DataCommand();
            cmm.CommandText = "Select fldValue from System.tblSettings where fldCode='RW' and fldKey='BranchDateFormat'";
            cmm.CommandType = MSLAService.EnDataCommandType.Text;
            cmm.ConnectionType = MSLAService.DBConnectionType.CompanyDB;

            Client.Data.DataConnect.FillDt(cmm, MSLA.Client.Login.LogonInfo.myUserInfo, new Client.MSLAService.MSLAServiceClient.DataFetchCompletedHandler(DataConnect_BranchDateFormatFetchCompleted));
        }

        void DataConnect_BranchDateFormatFetchCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs e)
        {
            MSLA.Client.Data.DataTable dt = e.dtResult;

            if (dt.Rows.Count > 0)
            {
                _BranchDateFormat = (string)dt[0]["fldValue"];

                Thread.CurrentThread.CurrentCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern =_BranchDateFormat; 
            }
        }
    }
}
