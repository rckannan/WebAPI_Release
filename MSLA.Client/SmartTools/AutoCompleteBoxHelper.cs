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
using MSLA.Client.MSLAService;

namespace MSLA.Client.SmartTools
{
    public class AutoCompleteBoxHelper
    {
        public static void GetResultSet(String collectionMember, String Filter, DBConnectionType cnType,
         MSLAService.SimpleUserInfo UserInfo, String valMember, String dispMember, String qText,
         MSLAService.MSLAServiceClient.GetRsltSetCompletedHandler GetRsltCompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.GetRsltSetCompleted += GetRsltCompletedAddress;
            wsClient.GetResultSetCompleted += new EventHandler<MSLAService.GetResultSetCompletedEventArgs>(wsClient_GetResultSetCompleted);
            wsClient.GetResultSetAsync(collectionMember, Filter, cnType, valMember, dispMember, qText, UserInfo, wsClient.Request_ID);
        }

        static void wsClient_GetResultSetCompleted(object sender, MSLAService.GetResultSetCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                System.Collections.Generic.Dictionary<Int64, String> result = new System.Collections.Generic.Dictionary<long, string>();
                result = e.Result;
                MSLAService.MSLAServiceClient.GetRsltSetCompletedEventArgs args = new MSLAService.MSLAServiceClient.GetRsltSetCompletedEventArgs(result);
                (sender as MSLAService.MSLAServiceClient).onGetResultSetCompleted(args);
            }
        }

        public static void GetSelectValue(String collectionMember, String Filter, DBConnectionType cnType,
            MSLAService.SimpleUserInfo UserInfo, String valMember, String dispMember, String qText,
            MSLAService.MSLAServiceClient.GetSelectValCompletedHandler GetSelectValCompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.GetSelectValCompleted += GetSelectValCompletedAddress;
            wsClient.GetSelectValueCompleted += new EventHandler<MSLAService.GetSelectValueCompletedEventArgs>(wsClient_GetSelectValueCompleted);
            wsClient.GetSelectValueAsync(collectionMember, Filter, cnType, valMember, dispMember, qText, UserInfo, wsClient.Request_ID);
        }

        static void wsClient_GetSelectValueCompleted(object sender, MSLAService.GetSelectValueCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                Int64 result = -1;
                result = e.Result;
                MSLAService.MSLAServiceClient.GetSelectValCompletedEventArgs args = new MSLAService.MSLAServiceClient.GetSelectValCompletedEventArgs(result);
                (sender as MSLAService.MSLAServiceClient).onGetSelectValCompleted(args);
            }
        }

        public static void GetSelectText(String collectionMember, String Filter, DBConnectionType cnType,
            MSLAService.SimpleUserInfo UserInfo, String valMember, String dispMember, Int64 qVal,
            MSLAService.MSLAServiceClient.GetSelectTxtCompletedHandler GetSelectTxtlCompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.GetSelectTxtCompleted += GetSelectTxtlCompletedAddress;
            wsClient.GetSelectTextCompleted += new EventHandler<MSLAService.GetSelectTextCompletedEventArgs>(wsClient_GetSelectTextCompleted);
            wsClient.GetSelectTextAsync(collectionMember, Filter, cnType, valMember, dispMember, qVal, UserInfo, wsClient.Request_ID);
        }

        static void wsClient_GetSelectTextCompleted(object sender, MSLAService.GetSelectTextCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                String result = String.Empty;
                result = e.Result;
                MSLAService.MSLAServiceClient.GetSelectTxtCompletedEventArgs args = new MSLAService.MSLAServiceClient.GetSelectTxtCompletedEventArgs(result);
                (sender as MSLAService.MSLAServiceClient).onGetSelectTxtCompleted(args);
            }
        }
    }
}
