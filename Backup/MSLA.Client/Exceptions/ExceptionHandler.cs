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

namespace MSLA.Client.Exceptions
{
    public class ExceptionHandler
    {
        public static Boolean result = false;
        public static void HandleException(Guid Request_ID)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.HandleExceptionCompleted+=new EventHandler<MSLAService.HandleExceptionCompletedEventArgs>(wsClient_HandleExceptionCompleted);
            wsClient.HandleExceptionAsync(Request_ID);
        }

        static void wsClient_HandleExceptionCompleted(object sender, MSLAService.HandleExceptionCompletedEventArgs e)
        {
            //throw new Exception(e.Result);
            result = true;
            MessageBox.Show(e.Result, "Service Exception", MessageBoxButton.OK);
        }

        public static void HandleException(Exception ex, string caption)
        {
            MessageBox.Show(ex.Message, caption, MessageBoxButton.OK);
        }
    }
}
