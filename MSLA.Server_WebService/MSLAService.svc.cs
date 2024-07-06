using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Data;
using Microsoft.Office.Interop.Excel;

namespace MSLA.Server_WebService
{
    public class MSLAService : IMSLAService
    {
        public Server.Security.SimpleUserInfo TryLogin(String name, Guid Request_ID)
        {
            Server.Security.SimpleUserInfo myUser = new Server.Security.SimpleUserInfo();

            try
            {
                MSLA.Server.Login.LogonInfo myLogonInfo = new Server.Login.LogonInfo(name);
                MSLA.Server.Login.Logon myLogon = new Server.Login.Logon();
                MSLA.Server.Login.LogonResult myLogonResult;
                myLogonResult = myLogon.TryLogin(myLogonInfo);

                if (myLogonResult.Status == MSLA.Server.Login.Logon.LogonState.Succeeded)
                {
                    myUser = Server.Login.LogonService.SaveLogonInfo(myLogonResult);
                    myUser.MainDBName = myLogonResult.MainDBName;
                }
                else
                {
                    myUser.User_ID = -1;
                }
                return myUser;
            }

            catch (Exception ex)
            {
                myUser.User_ID = -1;
                //MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myUser, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Server.Security.SimpleUserInfo TryLoginMain(String name, String pwd, Guid Request_ID)
        {
            Server.Security.SimpleUserInfo myUser = new Server.Security.SimpleUserInfo();

            try
            {
                MSLA.Server.Login.LogonInfo myLogonInfo = new Server.Login.LogonInfo(name, pwd);
                MSLA.Server.Login.Logon myLogon = new Server.Login.Logon();
                MSLA.Server.Login.LogonResult myLogonResult;
                myLogonResult = myLogon.TryLoginMain(myLogonInfo);

                if (myLogonResult.Status == MSLA.Server.Login.Logon.LogonState.Succeeded)
                {
                    myUser = Server.Login.LogonService.SaveLogonInfo(myLogonResult);
                    myUser.MainDBName = myLogonResult.MainDBName;
                }
                else
                {
                    myUser.User_ID = -1;
                }
                return myUser;
            }
            catch (Exception ex)
            {
                myUser.User_ID = -1;
                //MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myUser, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Boolean LogOut(Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                Server.Login.LogonService.LogOut(UserInfo.Session_ID);
                return true;
            }
            catch (Exception ex)
            {
                // MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Server.Data.SimpleTable FillDt(String CmdText, MSLA.Server.Data.EnDataCommandType cmdType, List<MSLA.Server.Data.DataParameter> cmdParams,
            MSLA.Server.Data.DBConnectionType cnType, int cmdTimeout, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                MSLA.Server.Data.DataCommand cmm = new Server.Data.DataCommand();
                cmm.CommandText = MSLA.Server.Security.EncryptionUtility.Decrypt(CmdText, Request_ID.ToString());
                cmm.CommandType = cmdType;
                cmm.Parameters = cmdParams;
                cmm.ConnectionType = cnType;
                cmm.CommandTimeout = cmdTimeout;
                return MSLA.Server.Data.DataConnect.FillDt(MSLA.Server.Data.DataCommand.GetSQLCommand(cmm), Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID), cmm.ConnectionType);
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public List<MSLA.Server.Data.DataParameter> ExecuteCMM(String CmdText, MSLA.Server.Data.EnDataCommandType cmdType,
            List<MSLA.Server.Data.DataParameter> cmdParams, MSLA.Server.Data.DBConnectionType cnType, int cmdTimeout, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                MSLA.Server.Data.DataCommand cmm = new Server.Data.DataCommand();
                cmm.CommandText = MSLA.Server.Security.EncryptionUtility.Decrypt(CmdText, Request_ID.ToString());
                cmm.CommandType = cmdType;
                cmm.Parameters = cmdParams;
                cmm.ConnectionType = cnType;
                cmm.CommandTimeout = cmdTimeout;

                System.Data.SqlClient.SqlCommand sqlcmm = new System.Data.SqlClient.SqlCommand();
                sqlcmm = MSLA.Server.Data.DataCommand.GetSQLCommand(cmm);
                return MSLA.Server.Data.DataConnect.ExecCMM(sqlcmm, Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID), cmm.ConnectionType);
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Object ExecuteScalar(String CmdText, MSLA.Server.Data.EnDataCommandType cmdType, List<MSLA.Server.Data.DataParameter> cmdParams,
            MSLA.Server.Data.DBConnectionType cnType, int cmdTimeout, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                MSLA.Server.Data.DataCommand cmm = new Server.Data.DataCommand();
                cmm.CommandText = MSLA.Server.Security.EncryptionUtility.Decrypt(CmdText, Request_ID.ToString());
                cmm.CommandType = cmdType;
                cmm.Parameters = cmdParams;
                cmm.ConnectionType = cnType;
                cmm.CommandTimeout = cmdTimeout;

                System.Data.SqlClient.SqlCommand sqlcmm = new System.Data.SqlClient.SqlCommand();
                sqlcmm = MSLA.Server.Data.DataCommand.GetSQLCommand(cmm);
                return MSLA.Server.Data.DataConnect.ExecuteScalar(sqlcmm, Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID), cmm.ConnectionType);
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Server.Base.SimpleBOMaster FetchBOMaster(MasterCriteriaBase myBOCriteria, Guid Request_ID)
        {
            try
            {
                MSLA.Server.BO.MasterBase myBo = MSLA.Server.BO.MasterBase.DataPortal_Fetch(myBOCriteria, Server.Login.LogonService.FetchLogonInfo(myBOCriteria.UserInfo.Session_ID));
                return myBo.ConstructSimpleBO();
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBOCriteria.UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Server.Base.SimpleBOMaster SaveBOMaster(Server.Base.SimpleBOMaster myBO, Guid Request_ID)
        {
            MSLA.Server.BO.MasterBase mySavedBO = null;
            try
            {
                mySavedBO = MSLA.Server.BO.MasterBase.ConstructMasterBO(myBO, Server.Login.LogonService.FetchLogonInfo(myBO.UserInfo.Session_ID));

                mySavedBO.DataPortal_Save();

                return mySavedBO.ConstructSimpleBO();
            }
            catch (MSLA.Server.Validations.ValidateException ex)
            {
                if (mySavedBO != null)
                {
                    if (mySavedBO.HasBrokenSaveRules)
                    {
                        myBO.BrokenSaveRules = mySavedBO.BrokenSaveRules;
                    }
                    MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBO.UserInfo, ex, Request_ID.ToString());
                    return myBO;
                }
                else
                {
                    MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBO.UserInfo, ex, Request_ID.ToString());
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBO.UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Server.Base.SimpleBOMaster DeleteBOMaster(MSLA.Server.Base.SimpleBOMaster myBO, Guid Request_ID)
        {
            MSLA.Server.BO.MasterBase mySavedBO = null;
            try
            {
                mySavedBO = MSLA.Server.BO.MasterBase.ConstructMasterBO(myBO, Server.Login.LogonService.FetchLogonInfo(myBO.UserInfo.Session_ID));

                mySavedBO.DataPortal_Delete();

                return new Server.Base.SimpleBOMaster();
            }
            catch (MSLA.Server.Validations.ValidateException ex)
            {
                if (mySavedBO != null)
                {
                    if (mySavedBO.HasBrokenDeleteRules)
                    {
                        myBO.BrokenDeleteRules = mySavedBO.BrokenDeleteRules;
                    }
                    MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBO.UserInfo, ex, Request_ID.ToString());
                    return myBO;
                }
                else
                {
                    MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBO.UserInfo, ex, Request_ID.ToString());
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBO.UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Dictionary<string, Server.Data.SimpleTable> ResolveTablesFromFile(byte[] FileStreamInfo, string FileExtension,
            string FilePassword, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                return Server.Utilities.FileExplorer.WriteFile(FileStreamInfo, FileExtension, FilePassword, (Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID)) as MSLA.Server.Security.IUser);
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public string HandleException(Guid Request_ID)
        {
            return MSLA.Server.Exceptions.ServiceExceptionHandler.FetchException(Request_ID.ToString());
        }

        public Dictionary<long, string> GetResultSet(String collectionMember, String Filter, Server.Data.DBConnectionType cnType,
                string valueMember, string displayMember, string queryText, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Tools.AutoCompleteBoxWorker.getResultSet(collectionMember, Filter, cnType, valueMember, displayMember, queryText,
                    Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID));
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public long GetSelectValue(String collectionMember, String Filter, Server.Data.DBConnectionType cnType, string valueMember,
                string displayMember, string queryText, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Tools.AutoCompleteBoxWorker.getValue(collectionMember, Filter, cnType, valueMember, displayMember, queryText,
                    Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID));
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public string GetSelectText(String collectionMember, String Filter, Server.Data.DBConnectionType cnType, string valueMember,
                string displayMember, long selectedVal, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Tools.AutoCompleteBoxWorker.getSelectedText(collectionMember, Filter, cnType, valueMember, displayMember, selectedVal,
                    Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID));
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public string GetCategoryFeed(Int64 Category_ID, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Tools.FeedWorker.generateFeed(Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID), Category_ID);
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public List<MSLA.Server.Tools.FeedItem> GetCategoryFeedItems(Int64 Category_ID, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Tools.FeedWorker.getFeedItems(Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID), Category_ID);
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        //public Boolean SubscriptionService(Int64 Category_ID, String message, byte[] FileStreamInfo, string FileName, Object testValue,
        //Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        //{
        //    try
        //    {
        //        return MSLA.Server.Utilities.SubscriptionHelper.SubscriptionService(Category_ID, message, FileName, FileStreamInfo,
        //            testValue, Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID));
        //    }
        //    catch (Exception ex)
        //    {
        //        MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
        //        throw ex;
        //    }
        //}

        public Server.Data.SimpleTable GetCategoryContract(Int64 Category_ID, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Utilities.SubscriptionHelper.getContracts(Category_ID, Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID));
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Boolean ContractSubscriptionService(Int64 Category_ID, String message, Boolean isHTML, String Subject, byte[] FileStreamInfo, string FileName, MSLA.Server.Data.SimpleTable testValue,
                Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Utilities.SubscriptionHelper.ContractSubscriptionService(Category_ID, message, isHTML, Subject, FileName, FileStreamInfo,
                    testValue, Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID));
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public MSLA.Server.Data.SimpleTable InvokeMethod(string assemblyName, string objectNamespace, string className, object[] constructorArgs,
                string methodName, object[] methodArgs,
                Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                if (constructorArgs != null)
                {   // Convert the UserInfo to server side User Info
                    for (int i = 0; i < constructorArgs.Length; i++)
                    {
                        if (constructorArgs[i].ToString() == "Server.Security.SimpleUserInfo")
                        {
                            constructorArgs[i] = Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID);
                        }
                        else if (constructorArgs[i] is Server.Data.SimpleTable)
                        {
                            constructorArgs[i] = Server.Data.DataConnect.ResolveToSystemTable(constructorArgs[i] as MSLA.Server.Data.SimpleTable);
                        }
                    }
                }

                if (methodArgs != null)
                {   // Convert the UserInfo to server side User Info
                    for (int i = 0; i < methodArgs.Length; i++)
                    {
                        if (methodArgs[i].ToString() == "Server.Security.SimpleUserInfo")
                        {
                            methodArgs[i] = Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID);
                        }
                        if (methodArgs[i] is Server.Data.SimpleTable)
                        {
                            methodArgs[i] = Server.Data.DataConnect.ResolveToSystemTable(methodArgs[i] as MSLA.Server.Data.SimpleTable);
                        }
                    }
                }

                MSLA.Server.Data.SimpleTable dtResult = new Server.Data.SimpleTable();
                object cInstance = MSLA.Server.Utilities.ReflectionHelper.CreateObject(assemblyName, objectNamespace, className, constructorArgs);
                if (cInstance != null)
                {
                    object res = MSLA.Server.Utilities.ReflectionHelper.CallMethod(cInstance, methodName, methodArgs);
                    if (res is MSLA.Server.Data.SimpleTable)
                    {
                        dtResult = res as MSLA.Server.Data.SimpleTable;
                    }
                    if (res is System.Data.DataTable)
                    {
                        dtResult = MSLA.Server.Data.DataConnect.ResolveToSimpleTable(res as System.Data.DataTable);
                    }

                    //System.Data.DataTable dt = (System.Data.DataTable)MSLA.Server.Utilities.ReflectionHelper.CallMethod(cInstance, methodName, methodArgs);
                    //foreach(System.Data.DataColumn dtCol in dt.Columns)
                    //{
                    //    dtResult.AddColumn(dtCol.ColumnName, dtCol.DataType.ToString());
                    //}

                    //foreach(DataRow dr in dt.Rows)
                    //{
                    //    Dictionary<string, object> drNew = new Dictionary<string, object>();
                    //    for (int i = 0; i < dt.Columns.Count; i++)
                    //    {
                    //        drNew.Add(dt.Columns[i].ColumnName, dr[i]);
                    //    }
                    //    dtResult.AddRow(drNew);
                    //}
                }
                return dtResult;
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public Server.Security.SimpleUserInfo TryLoginUPA(String name, Guid Request_ID)
        {
            Server.Security.SimpleUserInfo myUser = new Server.Security.SimpleUserInfo();

            try
            {
                MSLA.Server.Login.LogonInfo myLogonInfo = new Server.Login.LogonInfo(name);
                MSLA.Server.Login.Logon myLogon = new Server.Login.Logon();
                MSLA.Server.Login.LogonResult myLogonResult;
                myLogonResult = myLogon.TryLoginUPA(myLogonInfo);

                if (myLogonResult.Status == MSLA.Server.Login.Logon.LogonState.Succeeded)
                {
                    myUser = Server.Login.LogonService.SaveLogonInfo(myLogonResult);
                    myUser.MainDBName = myLogonResult.MainDBName;
                }
                else
                {
                    myUser.User_ID = -1;
                }
                return myUser;
            }
            catch (Exception ex)
            {
                myUser.User_ID = -1;
                throw ex;
            }
        }

        public Server.Data.SimpleTable GetSerivceInfo(Server.Security.SimpleUserInfo UserInfo, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Data.DataConnect.ResolveToSimpleTable
                    (MSLA.Server.Utilities.WinServiceInfo.GetInstance().getServiceInfo(Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID)));
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public String GetSerivceLog(Server.Security.SimpleUserInfo UserInfo, string ServiceName, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Utilities.WinServiceInfo.GetInstance().GetServiceLog(ServiceName, Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID));
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

        public String GetSerivceLogFromID(Server.Security.SimpleUserInfo UserInfo, Int64 Service_ID, Guid Request_ID)
        {
            try
            {
                return MSLA.Server.Utilities.WinServiceInfo.GetInstance().GetServiceLog(Service_ID, Server.Login.LogonService.FetchLogonInfo(UserInfo.Session_ID));
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        }

    }
}
