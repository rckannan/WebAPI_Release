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

namespace MSLA.Client.Login
{
    public class LogonInfo
    {
        public static SimpleUserInfo myUserInfo;
        public static event EventHandler AuthenticationCompleted;
        public static Boolean ExportAllowed = false;

        //public static void TryLogin(String name, String pwd)
        //{
        //    MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
        //    wsClient.TryLoginCompleted += new EventHandler<MSLAService.TryLoginCompletedEventArgs>(wsClient_TryLoginCompleted);
        //    wsClient.TryLoginAsync(name, pwd);
        //}

        public static void TryLogin(String name)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.TryLoginCompleted += new EventHandler<MSLAService.TryLoginCompletedEventArgs>(wsClient_TryLoginCompleted);
            wsClient.TryLoginAsync(name, wsClient.Request_ID);
        }

        static void wsClient_TryLoginCompleted(object sender, MSLAService.TryLoginCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Login Failed.", "Login Error", MessageBoxButton.OK);
            }
            else
            {
                myUserInfo = e.Result;
                try
                {
                    CheckExportAccess();
                }
                catch (Exception ex)
                {
                    ExportAllowed = false;
                }
            }
        }

        public static void TryLogin(string name, string pwd)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.TryLoginMainCompleted += new EventHandler<TryLoginMainCompletedEventArgs>(wsClient_TryLoginMainCompleted);
            wsClient.TryLoginMainAsync(name, pwd, wsClient.Request_ID);
        }

        static void wsClient_TryLoginMainCompleted(object sender, MSLAService.TryLoginMainCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Login Failed.", "Login Error", MessageBoxButton.OK);
            }
            else
            {
                myUserInfo = e.Result;
                try
                {
                    CheckExportAccess();
                }
                catch (Exception ex)
                {
                    ExportAllowed = false;
                }
            }
        }

        public static void TryLoginUPA(string name)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.TryLoginUPACompleted += new EventHandler<TryLoginUPACompletedEventArgs>(wsClient_TryLoginUPACompleted);
            wsClient.TryLoginUPAAsync(name, wsClient.Request_ID);
        }

        static void wsClient_TryLoginUPACompleted(object sender, MSLAService.TryLoginUPACompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Login Failed.", "Login Error", MessageBoxButton.OK);
            }
            else
            {
                myUserInfo = e.Result;
                ExportAllowed = true;
                if (AuthenticationCompleted != null)
                {
                    AuthenticationCompleted.Invoke(null, new EventArgs());
                }
            }
        }

        public static void LogOut()
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.LogOutCompleted += new EventHandler<LogOutCompletedEventArgs>(wsClient_LogOutCompleted);
            wsClient.LogOutAsync(myUserInfo, wsClient.Request_ID);
        }

        static void wsClient_LogOutCompleted(object sender, MSLAService.LogOutCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "LogOut Error", MessageBoxButton.OK);
            }
            else
            {
                Boolean IfAllowed = e.Result;
                if (AuthenticationCompleted != null)
                {
                    AuthenticationCompleted.Invoke(null, new EventArgs());
                }
            }
        }

        static void CheckExportAccess()
        {
            Data.DataCommand cmm = new Data.DataCommand();
            cmm.CommandText = "select fldIsExportAllowed from Main.tblUser Where flduser_ID = @User_ID";
            DataParameter param = new DataParameter();
            param._ParameterName = "@User_ID";
            param._DBType = DataParameter.EnDataParameterType.BigInt;
            param._Size = 0;
            param._Value = myUserInfo.User_ID;
            param._Direction = DataParameter.EnParameterDirection.Input;
            cmm.Parameters.Add(param);
            cmm.ConnectionType = DBConnectionType.MainDB;
            cmm.CommandType = EnDataCommandType.Text;
            Data.DataConnect.ExecuteScalar(cmm, Login.LogonInfo.myUserInfo, new MSLAServiceClient.ExecScalarCompletedHandler(ExecScalarCompleted));
        }

        static void ExecScalarCompleted(object sender, MSLAServiceClient.ExecScalarCompletedEventArgs e)
        {
            ExportAllowed = Convert.ToBoolean(e.Result);
            if (AuthenticationCompleted != null)
            {
                AuthenticationCompleted.Invoke(null, new EventArgs());
            }
        }
    }
}
