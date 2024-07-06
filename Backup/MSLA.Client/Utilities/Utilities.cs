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
using MSLA.Client.MSLAService;

namespace MSLA.Client
{
    public class Utilities
    {
        public static void ResolveTablesFromFile(byte[] FileStreamInfo, string FileExtension, string FilePassword, SimpleUserInfo UserInfo, MSLAService.MSLAServiceClient.ResolveFileCompletedHandler ResolveFileCompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.ResolveFileCompleted += ResolveFileCompletedAddress;
            wsClient.ResolveTablesFromFileCompleted += new EventHandler<MSLAService.ResolveTablesFromFileCompletedEventArgs>(wsClient_ResolveTablesFromFileCompleted);
            wsClient.ResolveTablesFromFileAsync(FileStreamInfo, FileExtension, FilePassword, UserInfo, wsClient.Request_ID);
        }

        static void wsClient_ResolveTablesFromFileCompleted(object sender, MSLAService.ResolveTablesFromFileCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                Dictionary<string, SimpleTable> TableList = new Dictionary<string, SimpleTable>();

                TableList = e.Result;
                System.Collections.Generic.Dictionary<string, Data.DataTable> ResultTableList = new System.Collections.Generic.Dictionary<string, Data.DataTable>();

                foreach (KeyValuePair<string, SimpleTable> item in TableList)
                {
                    ResultTableList.Add(item.Key, Data.DataConnect.GetResolvedTable(item.Value));
                }

                MSLAService.MSLAServiceClient.ResolveFileCompletedEventArgs args = new MSLAService.MSLAServiceClient.ResolveFileCompletedEventArgs(ResultTableList);
                (sender as MSLAService.MSLAServiceClient).onResolveFileCompleted(args);
            }
        }

        ///// <summary> Used for notification message subscription handling </summary>
        ///// <param name="Category_ID"></param>
        ///// <param name="message"></param>
        ///// <param name="FileStreamInfo"></param>
        ///// <param name="FileName"></param>
        ///// <param name="testValue"></param>
        ///// <param name="UserInfo"></param>
        ///// <param name="ResolveFileCompletedAddress"></param>
        //public static void UpdateNotifications(long Category_ID, string message, byte[] FileStreamInfo, string FileName, Object testValue, SimpleUserInfo UserInfo, MSLAService.MSLAServiceClient.NotificationSubCompletedHandler ResolveFileCompletedAddress)
        //{
        //    MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
        //    wsClient.NotificationSubCompleted += ResolveFileCompletedAddress;
        //    wsClient.SubscriptionServiceCompleted += new EventHandler<SubscriptionServiceCompletedEventArgs>(wsClient_SubscriptionServiceCompleted);
        //    wsClient.SubscriptionServiceAsync(Category_ID, message, FileStreamInfo, FileName, testValue, UserInfo, wsClient.Request_ID);
        //}

        //static void wsClient_SubscriptionServiceCompleted(object sender, SubscriptionServiceCompletedEventArgs e)
        //{
        //    if (e.Error != null || e.Result == null)
        //    {
        //        Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
        //    }
        //    else
        //    {
        //        MSLAService.MSLAServiceClient.NotificationSubCompletedEventArgs args = new MSLAServiceClient.NotificationSubCompletedEventArgs(e.Result);
        //        (sender as MSLAService.MSLAServiceClient).onNotificationSubCompleted(args);
        //    }
        //}

        /// <summary> Used for notification message subscription contract handling </summary>
        /// <param name="Category_ID"></param>
        /// <param name="message"></param>
        /// <param name="FileStreamInfo"></param>
        /// <param name="FileName"></param>
        /// <param name="testValue"></param>
        /// <param name="UserInfo"></param>
        /// <param name="CompletedAddress"></param>
        public static void UpdateContractNotification(long Category_ID, string message, Boolean isHTML, String Subject, byte[] FileStreamInfo, string FileName, Data.DataTable testValue, SimpleUserInfo UserInfo, MSLAService.MSLAServiceClient.ContractSubCompletedHandler CompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.ContractSubCompleted += CompletedAddress;
            wsClient.ContractSubscriptionServiceCompleted += new EventHandler<ContractSubscriptionServiceCompletedEventArgs>(wsClient_ContractSubscriptionServiceCompleted);
            wsClient.ContractSubscriptionServiceAsync(Category_ID, message, isHTML, Subject, FileStreamInfo, FileName, Data.DataConnect.ResolveToSimpleTable(testValue), UserInfo, wsClient.Request_ID);
        }

        public static void UpdateContractNotification(long Category_ID, string message, String Subject, byte[] FileStreamInfo, string FileName, Data.DataTable testValue, SimpleUserInfo UserInfo, MSLAService.MSLAServiceClient.ContractSubCompletedHandler CompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.ContractSubCompleted += CompletedAddress;
            wsClient.ContractSubscriptionServiceCompleted += new EventHandler<ContractSubscriptionServiceCompletedEventArgs>(wsClient_ContractSubscriptionServiceCompleted);
            wsClient.ContractSubscriptionServiceAsync(Category_ID, message, false, Subject, FileStreamInfo, FileName, Data.DataConnect.ResolveToSimpleTable(testValue), UserInfo, wsClient.Request_ID);
        }

        static void wsClient_ContractSubscriptionServiceCompleted(object sender, ContractSubscriptionServiceCompletedEventArgs e)
        {
            MSLAService.MSLAServiceClient.ContractSubCompletedEventArgs args = new MSLAServiceClient.ContractSubCompletedEventArgs(e.Result);
            (sender as MSLAService.MSLAServiceClient).onContractSubCompleted(args);
        }

        /// <summary> Used for notification message subscription contract handling </summary>
        /// <param name="Category_ID"></param>
        /// <param name="message"></param>
        /// <param name="FileStreamInfo"></param>
        /// <param name="FileName"></param>
        /// <param name="testValue"></param>
        /// <param name="UserInfo"></param>
        /// <param name="CompletedAddress"></param>
        public static void GetNotificationContract(long Category_ID, SimpleUserInfo UserInfo, MSLAService.MSLAServiceClient.GetContractCompletedHandler CompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.GetContractCompleted += CompletedAddress;
            wsClient.GetCategoryContractCompleted += new EventHandler<GetCategoryContractCompletedEventArgs>(wsClient_ContractSubscriptionServiceCompleted);
            wsClient.GetCategoryContractAsync(Category_ID, UserInfo, wsClient.Request_ID);
        }

        static void wsClient_ContractSubscriptionServiceCompleted(object sender, GetCategoryContractCompletedEventArgs e)
        {
            Data.DataTable dtResult = new Data.DataTable();
            if (e.Result != null)
            {
                dtResult = Data.DataConnect.GetResolvedTable(e.Result);
            }
            MSLAService.MSLAServiceClient.GetContractCompletedEventArgs args = new MSLAServiceClient.GetContractCompletedEventArgs(dtResult);
            (sender as MSLAService.MSLAServiceClient).onGetContractCompleted(args);
        }

        public static void InvokeServerMethod(string assemblyName, string objectNamespace, string className, List<object> constructorArgs,
                string methodName, List<object> methodArgs,
                SimpleUserInfo UserInfo,
                MSLAService.MSLAServiceClient.DataFetchCompletedHandler DataFetchCompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.DataFetchCompleted += DataFetchCompletedAddress;
            wsClient.InvokeMethodCompleted += new EventHandler<InvokeMethodCompletedEventArgs>(wsClient_InvokeMethodCompleted);
            List<object> mtdArgs = new List<object>();
            List<object> ctdArgs = new List<object>();

            foreach (object obj in methodArgs)
            {
                if (obj is Data.DataTable)
                {
                    mtdArgs.Add(Data.DataConnect.ResolveToSimpleTable(obj as Data.DataTable));
                }
                else
                {
                    mtdArgs.Add(obj);
                }
            }

            foreach (object obj in constructorArgs)
            {
                if (obj is Data.DataTable)
                {
                    ctdArgs.Add(Data.DataConnect.ResolveToSimpleTable(obj as Data.DataTable));
                }
                else
                {
                    ctdArgs.Add(obj);
                }
            }

            wsClient.InvokeMethodAsync(assemblyName, objectNamespace, className, ctdArgs, methodName, mtdArgs, UserInfo, wsClient.Request_ID, null);
        }

        static void wsClient_InvokeMethodCompleted(object sender, InvokeMethodCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                MSLAService.SimpleTable dtResult = e.Result;
                Data.DataTable dt = new Data.DataTable();

                foreach (KeyValuePair<string, string> item in dtResult.Columns)
                {
                    dt.Columns.Add(new Data.DataColumn() { ColumnName = item.Key, DataType = System.Type.GetType(item.Value) });
                }

                foreach (Dictionary<string, object> item in dtResult.Rows)
                {
                    dt.Rows.Add(new Data.DataRow(item));
                }

                MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs args = new MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs(dt);
                (sender as MSLAService.MSLAServiceClient).onDataFetchCompleted(args);
            }
        }

        public static void GetServiceInfo(SimpleUserInfo UserInfo, MSLAService.MSLAServiceClient.GetServiceInformationCompletedHandler CompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.GetServiceInformationCompleted += CompletedAddress;
            wsClient.GetSerivceInfoCompleted += new EventHandler<GetSerivceInfoCompletedEventArgs>(wsClient_SerivceInfoServiceCompleted);
            wsClient.GetSerivceInfoAsync(UserInfo, wsClient.Request_ID);
        }

        static void wsClient_SerivceInfoServiceCompleted(object sender, GetSerivceInfoCompletedEventArgs e)
        {
            Data.DataTable dtResult = new Data.DataTable();
            if (e.Result != null)
            {
                dtResult = Data.DataConnect.GetResolvedTable(e.Result);
            }
            MSLAService.MSLAServiceClient.GetServiceInformationCompletedEventArgs args = new MSLAServiceClient.GetServiceInformationCompletedEventArgs(dtResult);
            (sender as MSLAService.MSLAServiceClient).onGetServiceInformationCompleted(args);
        }

        public static void GetServiceLog(SimpleUserInfo UserInfo, string serviceName, MSLAService.MSLAServiceClient.GetWinServiceLogCompletedHandler CompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.GetWinServiceLogCompleted += CompletedAddress;
            wsClient.GetSerivceLogCompleted += new EventHandler<GetSerivceLogCompletedEventArgs>(wsClient_SerivceLogServiceCompleted);
            wsClient.GetSerivceLogAsync(UserInfo, serviceName, wsClient.Request_ID);
        }

        static void wsClient_SerivceLogServiceCompleted(object sender, GetSerivceLogCompletedEventArgs e)
        {
            string Result = string.Empty;
            if (e.Result != null)
            {
                Result = e.Result;
            }
            MSLAService.MSLAServiceClient.GetWinServiceLogCompletedEventArgs args = new MSLAServiceClient.GetWinServiceLogCompletedEventArgs(Result);
            (sender as MSLAService.MSLAServiceClient).onGetWinServiceLogCompleted(args);
        }

        public static void GetServiceLogFromID(SimpleUserInfo UserInfo, Int64 service_ID, MSLAService.MSLAServiceClient.GetWinServiceLogFromIDCompletedHandler CompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.GetWinServiceLogFromIDCompleted += CompletedAddress;
            wsClient.GetSerivceLogFromIDCompleted += new EventHandler<GetSerivceLogFromIDCompletedEventArgs>(wsClient_GetSerivceLogFromIDCompleted);
            wsClient.GetSerivceLogFromIDAsync(UserInfo, service_ID, wsClient.Request_ID);
        }

        static void wsClient_GetSerivceLogFromIDCompleted(object sender, GetSerivceLogFromIDCompletedEventArgs e)
        {
            string Result = string.Empty;
            if (e.Result != null)
            {
                Result = e.Result;
            }
            MSLAService.MSLAServiceClient.GetWinServiceLogFromIDCompletedEventArgs args = new MSLAServiceClient.GetWinServiceLogFromIDCompletedEventArgs(Result);
            (sender as MSLAService.MSLAServiceClient).onGetWinServiceLogFromIDCompleted(args);
        }

    }
}
