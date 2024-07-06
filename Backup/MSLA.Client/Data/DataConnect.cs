using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MSLA.Client.MSLAService;

namespace MSLA.Client.Data
{
    public class DataConnect
    {
        public static void FillDt(MSLA.Client.Data.DataCommand cmm, MSLAService.SimpleUserInfo UserInfo, MSLAService.MSLAServiceClient.DataFetchCompletedHandler DataFetchCompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.DataFetchCompleted += DataFetchCompletedAddress;
            wsClient.FillDtCompleted += new EventHandler<MSLAService.FillDtCompletedEventArgs>(wsClient_FillDtCompleted);
            string eCMMtext = Security.EncryptionUtility.Encrypt(cmm.CommandText, wsClient.Request_ID.ToString());
            wsClient.FillDtAsync(eCMMtext, cmm.CommandType, cmm.Parameters, cmm.ConnectionType, cmm.CommandTimeout, UserInfo, wsClient.Request_ID);
        }

        static void wsClient_FillDtCompleted(object sender, MSLAService.FillDtCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                MSLAService.SimpleTable dtResult = e.Result;
                MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs args = new MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs();
                DataTable dt = new DataTable();

                foreach (KeyValuePair<string, string> item in dtResult.Columns)
                {
                    dt.Columns.Add(new DataColumn() { ColumnName = item.Key, DataType = System.Type.GetType(item.Value) });
                }

                foreach (Dictionary<string, object> item in dtResult.Rows)
                {
                    dt.Rows.Add(new DataRow(item));
                }

                args.setdtResult(dt);
                (sender as MSLAService.MSLAServiceClient).onDataFetchCompleted(args);
            }
        }

        public static void ExecCMM(MSLA.Client.Data.DataCommand cmm, MSLAService.SimpleUserInfo UserInfo, MSLAService.MSLAServiceClient.ExecCMMCompletedHandler ExecCMMCompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.ExecCMMCompleted += ExecCMMCompletedAddress;
            wsClient.ExecuteCMMCompleted += new EventHandler<MSLAService.ExecuteCMMCompletedEventArgs>(wsClient_ExececuteCMMCompleted);
            string eCMMtext = Security.EncryptionUtility.Encrypt(cmm.CommandText, wsClient.Request_ID.ToString());
            wsClient.ExecuteCMMAsync(eCMMtext, cmm.CommandType, cmm.Parameters, cmm.ConnectionType, cmm.CommandTimeout, UserInfo, wsClient.Request_ID);
        }

        static void wsClient_ExececuteCMMCompleted(object sender, MSLAService.ExecuteCMMCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                System.Collections.Generic.List<MSLAService.DataParameter> paramList = new List<MSLAService.DataParameter>();
                paramList = e.Result;
                MSLAService.MSLAServiceClient.ExecCMMCompletedEventArgs args = new MSLAService.MSLAServiceClient.ExecCMMCompletedEventArgs(paramList);
                (sender as MSLAService.MSLAServiceClient).onExecCMMCompleted(args);
            }
        }

        public static Client.Data.DataTable GetResolvedTable(MSLAService.SimpleTable dtResult)
        {
            DataTable dt = new DataTable();
            foreach (KeyValuePair<string, string> item in dtResult.Columns)
            {
                dt.Columns.Add(new DataColumn() { ColumnName = item.Key, DataType = System.Type.GetType(item.Value) });
            }
            foreach (Dictionary<string, object> item in dtResult.Rows)
            {
                dt.Rows.Add(new DataRow(item));
            }
            return dt;
        }

        public static SimpleTable ResolveToSimpleTable(DataTable dt)
        {
            SimpleTable dtResult = new SimpleTable();
            dtResult.Columns = new Dictionary<string, string>();
            dtResult.Rows = new List<Dictionary<string, object>>();

            foreach (DataColumn dc in dt.Columns)
            {
                dtResult.Columns.Add(dc.ColumnName, Convert.ToString(dc.DataType));
            }

            int ColCount = dt.Columns.Count;
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> drNew = new Dictionary<string, object>();
                for (int i = 0; i < ColCount; i++)
                {
                    drNew.Add(dt.Columns[i].ColumnName, dr[dt.Columns[i].ColumnName]);
                }
                dtResult.Rows.Add(drNew);
            }
            return dtResult;
        }

        public static SimpleTable ResolveToTable(DataTable dt)
        {

            SimpleTable dtResult = new SimpleTable();
            dtResult.Columns = new Dictionary<string, string>();
            dtResult.Rows = new List<Dictionary<string, object>>();

            foreach (DataColumn dc in dt.Columns)
            {
                dtResult.Columns.Add(dc.ColumnName, Convert.ToString(dc.DataType));
            }

            int ColCount = dt.Columns.Count;
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> drNew = new Dictionary<string, object>();
                for (int i = 0; i < ColCount; i++)
                {
                    drNew.Add(dt.Columns[i].ColumnName, dr[dt.Columns[i].ColumnName]);
                }
                dtResult.Rows.Add(drNew);
            }
            return dtResult;
        }

        public static void ExecuteScalar(MSLA.Client.Data.DataCommand cmm, MSLAService.SimpleUserInfo UserInfo, MSLAService.MSLAServiceClient.ExecScalarCompletedHandler ExecScalarCompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.ExecScalarCompleted += ExecScalarCompletedAddress;
            wsClient.ExecuteScalarCompleted += new EventHandler<MSLAService.ExecuteScalarCompletedEventArgs>(wsClient_ExececuteScalarCompleted);
            string eCMMtext = Security.EncryptionUtility.Encrypt(cmm.CommandText, wsClient.Request_ID.ToString());
            wsClient.ExecuteScalarAsync(eCMMtext, cmm.CommandType, cmm.Parameters, cmm.ConnectionType, cmm.CommandTimeout, UserInfo, wsClient.Request_ID);
        }

        static void wsClient_ExececuteScalarCompleted(object sender, MSLAService.ExecuteScalarCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                Object result;
                result = e.Result;
                MSLAService.MSLAServiceClient.ExecScalarCompletedEventArgs args = new MSLAService.MSLAServiceClient.ExecScalarCompletedEventArgs(result);
                (sender as MSLAService.MSLAServiceClient).onExecScalarCompleted(args);
            }
        }


    }
}
