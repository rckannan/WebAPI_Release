using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using MSLA.Server.Login.API;

namespace MSLA.Server.Login
{
    /// <summary>The Logon Class</summary>
    //[System.Diagnostics.DebuggerStepThrough()]
    public partial class Logon
        : MarshalByRefObject
    {
        public Logon()
            : base()
        {
        }

        /// <summary>Enum For Logon State</summary>
        public enum LogonState
        {
            /// <summary>Logon Failed</summary>
            Failed = 0,
            /// <summary>Logon Succeeded</summary>
            Succeeded = 1
        }

        /// <summary>Use this method to Login. Returns Logon Result</summary>
        /// <param name="myLogonInfo">The Logon Info</param>
        public LogonResult TryLogin(LogonInfo myLogonInfo)
        {
            LogonResult myLogonResult = null;
            SqlConnection cnLogin = null;
            try
            {
                //  *****   Now that the connection succeeded, we verify
                //   *****   whether the UserName and password
                SqlCommand cmm = new SqlCommand();
                System.Int64 myUserID;
                Boolean UserIsSuperUser = false;
                cnLogin = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB);
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "Main.spUserValidateWithoutPwd";
                cmm.Connection = cnLogin;
                if (cmm.Connection.State == ConnectionState.Closed)
                {
                    cmm.Connection.Open();
                }
                cmm.Parameters.Add("@SuperUser", SqlDbType.VarChar, 20).Value = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("Superuser").ToString());
                //cmm.Parameters.Add("@SuperPass", SqlDbType.VarChar, 16).Value = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("Superpwd").ToString());
                cmm.Parameters.Add("@UserName", SqlDbType.VarChar, 20).Value = myLogonInfo.UserName;
                //cmm.Parameters.Add("@UserPass", SqlDbType.VarChar, 16).Value = myLogonInfo.UserPass;
                cmm.Parameters.Add("@UserIsSuperUser", SqlDbType.Bit, 0).Value = 0;
                cmm.Parameters["@UserIsSuperUser"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@FullUserName", SqlDbType.VarChar, 50).Value = "";
                cmm.Parameters["@FullUserName"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@User_ID", SqlDbType.BigInt, 0).Value = -1;
                cmm.Parameters["@User_ID"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@Message", SqlDbType.VarChar, 250).Value = "";
                cmm.Parameters["@Message"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@Machine_Name", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientMachineName;
                cmm.Parameters.Add("@ClientIP", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientIP;
                cmm.Parameters.Add("@ClientMAC", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientMAC;
                cmm.Parameters.Add("@SessionMessage", SqlDbType.VarChar, 250).Value = "";
                cmm.Parameters["@SessionMessage"].Direction = ParameterDirection.InputOutput;
                cmm.ExecuteNonQuery();

                if (Convert.ToString(cmm.Parameters["@Message"].Value) != "OK")
                {
                    if (Convert.ToString(cmm.Parameters["@SessionMessage"].Value) == "")
                    {
                        // ****    Logon Failed due to invalid username/password
                        //Utilities.LogWriter.LogFailedSession(myLogonInfo, cnLogin);
                    }
                    //  Exceptions.ServiceExceptionHandler.HandleException(-1, string.Empty, new Exception(Convert.ToString(cmm.Parameters["@Message"].Value))); 

                    throw new Exception(Convert.ToString(cmm.Parameters["@Message"].Value));

                }
                else //'   *****   Logon Succeeded
                {
                    if (Convert.ToBoolean(cmm.Parameters["@UserIsSuperUser"].Value))
                    { UserIsSuperUser = true; }

                    myUserID = Convert.ToInt64(cmm.Parameters["@User_ID"].Value);
                    myLogonResult = new LogonResult(LogonState.Succeeded, Convert.ToString(cmm.Parameters["@FullUserName"].Value), cmm.Connection.ToString(), UserIsSuperUser,
                                                    myUserID, myLogonInfo.UserName, myLogonInfo.SQLServer, String.Empty, cnLogin.Database);
                    myLogonResult.LogonInfoUsed = myLogonInfo;
                    //  ****    Open User Session
                    UserSessionOpen(myLogonResult);
                }
            }
            catch (Exception exLogon)
            {
                // Exceptions.ServiceExceptionHandler.HandleException(-1, string.Empty, exLogon); 
                throw new LogonException(exLogon);
            }
            finally
            {
                if (cnLogin.State != ConnectionState.Closed)
                { cnLogin.Close(); }
            }
            myLogonResult.SetMainDBName(cnLogin.Database);
            return myLogonResult;
        }

        public LogonResult TryLoginWeb(LogonInfo myLogonInfo)
        {
            LogonResult myLogonResult = null;
            SqlConnection cnLogin = null;
            try
            {
                //  *****   Now that the connection succeeded, we verify
                //   *****   whether the UserName and password
                SqlCommand cmm = new SqlCommand();
                System.Int64 myUserID;
                Boolean UserIsSuperUser = false;
                cnLogin = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB);
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "Main.spUserValidateWithoutPwdWeb";
                cmm.Connection = cnLogin;
                if (cmm.Connection.State == ConnectionState.Closed)
                {
                    cmm.Connection.Open();
                }
                cmm.Parameters.Add("@SuperUser", SqlDbType.VarChar, 20).Value = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("Superuser").ToString());
                //cmm.Parameters.Add("@SuperPass", SqlDbType.VarChar, 16).Value = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("Superpwd").ToString());
                cmm.Parameters.Add("@UserName", SqlDbType.VarChar, 20).Value = myLogonInfo.UserName;
                //cmm.Parameters.Add("@UserPass", SqlDbType.VarChar, 16).Value = myLogonInfo.UserPass;
                cmm.Parameters.Add("@UserIsSuperUser", SqlDbType.Bit, 0).Value = 0;
                cmm.Parameters["@UserIsSuperUser"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@FullUserName", SqlDbType.VarChar, 50).Value = "";
                cmm.Parameters["@FullUserName"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@User_ID", SqlDbType.BigInt, 0).Value = -1;
                cmm.Parameters["@User_ID"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@Message", SqlDbType.VarChar, 250).Value = "";
                cmm.Parameters["@Message"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@EmailAddress", SqlDbType.VarChar, 250).Value = "";
                cmm.Parameters["@EmailAddress"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@Machine_Name", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientMachineName;
                cmm.Parameters.Add("@ClientIP", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientIP;
                cmm.Parameters.Add("@ClientMAC", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientMAC;
                cmm.Parameters.Add("@SessionMessage", SqlDbType.VarChar, 250).Value = "";
                cmm.Parameters["@SessionMessage"].Direction = ParameterDirection.InputOutput;
                cmm.ExecuteNonQuery();

                if (Convert.ToString(cmm.Parameters["@Message"].Value) != "OK")
                {
                    if (Convert.ToString(cmm.Parameters["@SessionMessage"].Value) == "")
                    {
                        // ****    Logon Failed due to invalid username/password
                        //Utilities.LogWriter.LogFailedSession(myLogonInfo, cnLogin);
                    }
                    //  Exceptions.ServiceExceptionHandler.HandleException(-1, string.Empty, new Exception(Convert.ToString(cmm.Parameters["@Message"].Value))); 

                    throw new Exception(Convert.ToString(cmm.Parameters["@Message"].Value));

                }
                else //'   *****   Logon Succeeded
                {
                    if (Convert.ToBoolean(cmm.Parameters["@UserIsSuperUser"].Value))
                    { UserIsSuperUser = true; }

                    myUserID = Convert.ToInt64(cmm.Parameters["@User_ID"].Value);
                    myLogonResult = new LogonResult(LogonState.Succeeded, Convert.ToString(cmm.Parameters["@FullUserName"].Value), cmm.Connection.ToString(), UserIsSuperUser,
                                                    myUserID, myLogonInfo.UserName, myLogonInfo.SQLServer, String.Empty, cnLogin.Database);
                    myLogonResult.LogonInfoUsed = myLogonInfo;
                    //  ****    Open User Session
                    UserSessionOpen(myLogonResult);
                }
            }
            catch (Exception exLogon)
            {
                // Exceptions.ServiceExceptionHandler.HandleException(-1, string.Empty, exLogon); 
                throw new LogonException(exLogon);
            }
            finally
            {
                if (cnLogin.State != ConnectionState.Closed)
                { cnLogin.Close(); }
            }
            myLogonResult.SetMainDBName(cnLogin.Database);
            return myLogonResult;
        }

        public LogonResult TryLoginMain(LogonInfo myLogonInfo)
        {
            LogonResult myLogonResult = null;
            SqlConnection cnLogin = null;
            try
            {
                //  *****   Now that the connection succeeded, we verify
                //   *****   whether the UserName and password
                SqlCommand cmm = new SqlCommand();
                System.Int64 myUserID;
                Boolean UserIsSuperUser = false;
                cnLogin = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB);
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "Main.spUserValidate";
                cmm.Connection = cnLogin;
                if (cmm.Connection.State == ConnectionState.Closed)
                {
                    cmm.Connection.Open();
                }
                cmm.Parameters.Add("@SuperUser", SqlDbType.VarChar, 20).Value = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("Superuser").ToString());
                cmm.Parameters.Add("@SuperPass", SqlDbType.VarChar, 16).Value = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("Superpwd").ToString());
                cmm.Parameters.Add("@UserName", SqlDbType.VarChar, 20).Value = myLogonInfo.UserName;
                cmm.Parameters.Add("@UserPass", SqlDbType.VarChar, 16).Value = myLogonInfo.UserPass;
                cmm.Parameters.Add("@UserIsSuperUser", SqlDbType.Bit, 0).Value = 0;
                cmm.Parameters["@UserIsSuperUser"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@FullUserName", SqlDbType.VarChar, 50).Value = "";
                cmm.Parameters["@FullUserName"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@User_ID", SqlDbType.BigInt, 0).Value = -1;
                cmm.Parameters["@User_ID"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@Message", SqlDbType.VarChar, 250).Value = "";
                cmm.Parameters["@Message"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@Machine_Name", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientMachineName;
                cmm.Parameters.Add("@ClientIP", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientIP;
                cmm.Parameters.Add("@ClientMAC", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientMAC;
                cmm.Parameters.Add("@SessionMessage", SqlDbType.VarChar, 250).Value = "";
                cmm.Parameters["@SessionMessage"].Direction = ParameterDirection.InputOutput;
                cmm.ExecuteNonQuery();

                if (Convert.ToString(cmm.Parameters["@Message"].Value) != "OK")
                {
                    if (Convert.ToString(cmm.Parameters["@SessionMessage"].Value) == "")
                    {
                        // ****    Logon Failed due to invalid username/password
                        //Utilities.LogWriter.LogFailedSession(myLogonInfo, cnLogin);
                    }
                    //  Exceptions.ServiceExceptionHandler.HandleException(-1, string.Empty, new Exception(Convert.ToString(cmm.Parameters["@Message"].Value))); 

                    throw new Exception(Convert.ToString(cmm.Parameters["@Message"].Value));

                }
                else //'   *****   Logon Succeeded
                {
                    if (Convert.ToBoolean(cmm.Parameters["@UserIsSuperUser"].Value))
                    { UserIsSuperUser = true; }

                    myUserID = Convert.ToInt64(cmm.Parameters["@User_ID"].Value);
                    myLogonResult = new LogonResult(LogonState.Succeeded, Convert.ToString(cmm.Parameters["@FullUserName"].Value), cmm.Connection.ToString(), UserIsSuperUser,
                                                    myUserID, myLogonInfo.UserName, myLogonInfo.SQLServer, String.Empty, cnLogin.Database);
                    myLogonResult.LogonInfoUsed = myLogonInfo;
                    //  ****    Open User Session
                    UserSessionOpen(myLogonResult);
                }
            }
            catch (Exception exLogon)
            {
                // Exceptions.ServiceExceptionHandler.HandleException(-1, string.Empty, exLogon); 
                throw new LogonException(exLogon);
            }
            finally
            {
                if (cnLogin.State != ConnectionState.Closed)
                { cnLogin.Close(); }
            }
            myLogonResult.SetMainDBName(cnLogin.Database);
            return myLogonResult;
        }

        public LogonResult TryLoginUPA(LogonInfo myLogonInfo)
        {
            LogonResult myLogonResult = null;
            SqlConnection cnLogin = null;
            try
            {
                //  *****   Now that the connection succeeded, we verify
                //   *****   whether the UserName and password
                SqlCommand cmm = new SqlCommand();
                System.Int64 myUserID;
                Boolean UserIsSuperUser = false;
                cnLogin = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB);
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "Main.spUPAUserValidateWOPwd";
                cmm.Connection = cnLogin;
                if (cmm.Connection.State == ConnectionState.Closed)
                {
                    cmm.Connection.Open();
                }
                cmm.Parameters.Add("@SuperUser", SqlDbType.VarChar, 20).Value = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("Superuser").ToString());
                //cmm.Parameters.Add("@SuperPass", SqlDbType.VarChar, 16).Value = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("Superpwd").ToString());
                cmm.Parameters.Add("@UserName", SqlDbType.VarChar, 20).Value = myLogonInfo.UserName;
                //cmm.Parameters.Add("@UserPass", SqlDbType.VarChar, 16).Value = myLogonInfo.UserPass;
                cmm.Parameters.Add("@UserIsSuperUser", SqlDbType.Bit, 0).Value = 0;
                cmm.Parameters["@UserIsSuperUser"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@FullUserName", SqlDbType.VarChar, 50).Value = "";
                cmm.Parameters["@FullUserName"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@User_ID", SqlDbType.BigInt, 0).Value = -1;
                cmm.Parameters["@User_ID"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@Message", SqlDbType.VarChar, 250).Value = "";
                cmm.Parameters["@Message"].Direction = ParameterDirection.InputOutput;
                cmm.Parameters.Add("@Machine_Name", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientMachineName;
                cmm.Parameters.Add("@ClientIP", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientIP;
                cmm.Parameters.Add("@ClientMAC", SqlDbType.VarChar, 250).Value = myLogonInfo.ClientMAC;
                cmm.Parameters.Add("@SessionMessage", SqlDbType.VarChar, 250).Value = "";
                cmm.Parameters["@SessionMessage"].Direction = ParameterDirection.InputOutput;
                cmm.ExecuteNonQuery();

                if (Convert.ToString(cmm.Parameters["@Message"].Value) != "OK")
                {
                    if (Convert.ToString(cmm.Parameters["@SessionMessage"].Value) == "")
                        throw new Exception(Convert.ToString(cmm.Parameters["@Message"].Value));
                }
                else //'   *****   Logon Succeeded
                {
                    if (Convert.ToBoolean(cmm.Parameters["@UserIsSuperUser"].Value))
                    { UserIsSuperUser = true; }

                    myUserID = Convert.ToInt64(cmm.Parameters["@User_ID"].Value);
                    myLogonResult = new LogonResult(LogonState.Succeeded, Convert.ToString(cmm.Parameters["@FullUserName"].Value), cmm.Connection.ToString(), UserIsSuperUser,
                                                    myUserID, myLogonInfo.UserName, myLogonInfo.SQLServer, String.Empty, cnLogin.Database);
                    myLogonResult.LogonInfoUsed = myLogonInfo;
                    //  ****    Open User Session
                    UserSessionOpen(myLogonResult);
                }
            }
            catch (Exception exLogon)
            {
                // Exceptions.ServiceExceptionHandler.HandleException(-1, string.Empty, exLogon); 
                throw new LogonException(exLogon);
            }
            finally
            {
                if (cnLogin.State != ConnectionState.Closed)
                { cnLogin.Close(); }
            }
            myLogonResult.SetMainDBName(cnLogin.Database);
            return myLogonResult;
        }

        public static Boolean IsValidUser(String UserName)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandText = "select COUNT(*) from Main.tblUser where fldUserName=@UserName and fldActiveUser=1";
            cmm.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;

            using (SqlConnection cn = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB))
            {
                try
                {
                    cn.Open();
                    cmm.Connection = cn;
                    int result = (int)cmm.ExecuteScalar();

                    if (result == 1)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    cn.Close();
                }
            }
            return false;
        }

        public static Boolean IsValidUPAUser(String UserName)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandText = "select COUNT(*) from Main.tblUPAUser where fldUserName=@UserName and fldActiveUser=1";
            cmm.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;

            using (SqlConnection cn = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB))
            {
                try
                {
                    cn.Open();
                    cmm.Connection = cn;
                    int result = (int)cmm.ExecuteScalar();

                    if (result == 1)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    cn.Close();
                }
            }
            return false;
        }

        private void UserSessionOpen(LogonResult LogResult)
        {

            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "Main.spUserSessionAdd";

            Cmm.Parameters.Add("@Session_ID", SqlDbType.UniqueIdentifier).Value = LogResult.Session_ID;
            Cmm.Parameters.Add("@User_ID", SqlDbType.BigInt, 0).Value = LogResult.User_ID;
            Cmm.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = LogResult.LogonName;
            Cmm.Parameters.Add("@MachineName", SqlDbType.VarChar, 250).Value = LogResult.LogonInfoUsed.ClientMachineName;
            Cmm.Parameters.Add("@ClientHostName", SqlDbType.VarChar, 250).Value = LogResult.LogonInfoUsed.ClientHostName;
            Cmm.Parameters.Add("@ClientDomain", SqlDbType.VarChar, 250).Value = LogResult.LogonInfoUsed.ClientDomain;
            Cmm.Parameters.Add("@ClientIP", SqlDbType.VarChar, 250).Value = LogResult.LogonInfoUsed.ClientIP;
            Cmm.Parameters.Add("@ClientMAC", SqlDbType.VarChar, 250).Value = LogResult.LogonInfoUsed.ClientMAC;

            Data.DataConnect.ExecCMM(LogResult, ref Cmm, Data.DBConnectionType.MainDB);

        }

        /// <summary>Closes a User Session</summary>
        /// <param name="UserInfo">The User Info</param>
        public static void UserSessionClose(Security.IUser UserInfo)
        {
            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "Main.spUserSessionClose";

            Cmm.Parameters.Add("@Session_ID", SqlDbType.UniqueIdentifier).Value = UserInfo.Session_ID;

            Data.DataConnect.ExecCMM(UserInfo, ref Cmm, Data.DBConnectionType.MainDB);

        }

        /// <summary>Keeps a User Session alive. Returns true if the session is kept alive</summary>
        /// <param name="UserInfo">The User Info</param>
        public static bool UserSessionKeepAlive(Security.IUser UserInfo)
        {
            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "Main.spUserSessionUpdate";

            Cmm.Parameters.Add("@Session_ID", SqlDbType.UniqueIdentifier).Value = UserInfo.Session_ID;
            Cmm.Parameters.Add("@IsAlive", SqlDbType.Bit).Value = false;
            Cmm.Parameters["@IsAlive"].Direction = ParameterDirection.InputOutput;

            Data.DataConnect.ExecCMM(UserInfo, ref Cmm, Data.DBConnectionType.MainDB);
            return Convert.ToBoolean(Cmm.Parameters["@IsAlive"].Value);
        }

        public static API.WebClient IsValidWebClient(string webClientId)
        {
            var cmm = new SqlCommand
            {
                CommandText = "select * from Main.tblWebClients where fldWebClient_ID = '" + webClientId + "'",
                CommandType = CommandType.Text
            };

            using (SqlConnection cn = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB))
            {
                try
                {
                   
                    cmm.Connection = cn;
                    var dtResult = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmm);
                    da.Fill(dtResult);

                    if (dtResult.Rows.Count <= 0) return null;
                   
                    return new WebClient()
                    {
                        fldWebClientId = Convert.ToString(dtResult.Rows[0]["fldWebClient_Id"]),
                        fldSecretCode = Convert.ToString(dtResult.Rows[0]["fldSecretCode"]),
                        fldAllowedOrigin = Convert.ToString(dtResult.Rows[0]["fldAllowedOrigin"]),
                        fldName = Convert.ToString(dtResult.Rows[0]["fldName"]),
                        fldActive = Convert.ToBoolean(dtResult.Rows[0]["fldActive"]),
                        fldApplicationType = Convert.ToInt32(dtResult.Rows[0]["fldApplicationType"]),
                        fldRefreshTokenLifeTime = Convert.ToInt32(dtResult.Rows[0]["fldRefreshTokenLifeTime"])
                    };
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    cn.Close();
                }
            }  
            
        }

        
    }
}
