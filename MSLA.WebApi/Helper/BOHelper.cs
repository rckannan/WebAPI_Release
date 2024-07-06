using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSLA.WebApi.ApiUtilities
{
    public class BOHelper
    {
        private static BOHelper _instance;
        private GenericBOResponse _response;
        public static BOHelper Instance
        {
            get {
                if (_instance == null)
	            {
		            _instance = new BOHelper();
	            }
                return _instance;
            }
        }

        public GenericBOResponse FetchBOMaster(MasterCriteriaBase myBOCriteria, Guid Request_ID)
        {
            _response = new GenericBOResponse();
            try
            {
                
                MSLA.Server.BO.MasterBase myBo = MSLA.Server.BO.MasterBase.DataPortal_Fetch(myBOCriteria, Server.Login.LogonService.FetchLogonInfo(Request_ID));
                return new GenericBOResponse()
                {
                    data = myBo.ConstructSimpleBO(),
                    status = 200 
                };
                
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBOCriteria.UserInfo, ex, Request_ID.ToString());
                return new GenericBOResponse()
                {
                    data = null,
                    status = 500,
                    statusText = ex.Message
                };
            }
        }

        public GenericBOResponse SaveBOMaster(Server.Base.SimpleBOMaster myBO)
        {
            MSLA.Server.BO.MasterBase mySavedBO = null;
            try
            {

                mySavedBO = MSLA.Server.BO.MasterBase.ConstructMasterBO(myBO, Server.Login.LogonService.FetchLogonInfo(myBO.UserInfo.Session_ID));

                mySavedBO.DataPortal_Save();
                return new GenericBOResponse()
                {
                    data = mySavedBO.ConstructSimpleBO(),
                    status = 200
                };
                 
            }
            catch (MSLA.Server.Validations.ValidateException ex)
            {
                if (mySavedBO != null)
                {
                    if (mySavedBO.HasBrokenSaveRules)
                    {
                        myBO.BrokenSaveRules = mySavedBO.BrokenSaveRules;
                    }
                    return new GenericBOResponse()
                    {
                        data = mySavedBO.ConstructSimpleBO(),
                        status = 200
                    };
                }
                else
                {
                    MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBO.UserInfo, ex, myBO.UserInfo.Session_ID.ToString());
                    return new GenericBOResponse()
                    {
                        data = null,
                        status = 500,
                        statusText = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(myBO.UserInfo, ex, myBO.UserInfo.Session_ID.ToString());
                return new GenericBOResponse()
                {
                    data = null,
                    status = 500,
                    statusText = ex.Message
                };
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
                }
                return dtResult;
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(UserInfo, ex, Request_ID.ToString());
                throw ex;
            }
        } 

        public struct Params
        {
            public string Name { get; set; }
            public object value { get; set; }
            public MSLA.Server.Data.DataParameter.EnParameterDirection direction { get; set; }
            public MSLA.Server.Data.DataParameter.EnDataParameterType ParamType { get; set; } 
        }

        public class MasterCriteriaBase
        : MSLA.Server.BO.IMasterCriteria
        {
            long docMaster_ID = -1;
            string docMasterType = string.Empty;
            Server.Security.SimpleUserInfo _UserInfo;
            //Guid _Session_ID = new Guid();
            //long _User_ID = -1;
            Dictionary<string, object> _propertyCollection = new Dictionary<string, object>();

            //[DataMember]
            public long DocMaster_ID
            {
                get { return docMaster_ID; }
                set { docMaster_ID = value; }
            }

            //[DataMember]
            public string DocMasterType
            {
                get { return docMasterType; }
                set { docMasterType = value; }
            }

            //[DataMember]
            public Server.Security.SimpleUserInfo UserInfo
            {
                get { return _UserInfo; }
                set { _UserInfo = value; }
            }
            //public Guid Session_ID
            //{
            //    get { return _Session_ID; }
            //    set { _Session_ID = value; }
            //}

            //[DataMember]
            //public long User_ID
            //{
            //    get { return _User_ID; }
            //    set { _User_ID = value; }
            //}

            //[DataMember]
            public Dictionary<string, object> PropertyCollection
            {
                get { return _propertyCollection; }
                set { _propertyCollection = value; }
            }
        }
    }
}