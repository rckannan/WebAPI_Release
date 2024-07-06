using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MSLA.Server_WebService
{
    [ServiceContract]
    public interface IMSLAService
    {

        #region Authentication Services

        [OperationContract]
        Server.Security.SimpleUserInfo TryLogin(String name, Guid Request_ID);

        [OperationContract]
        Boolean LogOut(Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        [OperationContract]
        Server.Security.SimpleUserInfo TryLoginMain(String name, String pwd, Guid Request_ID);

        [OperationContract]
        Server.Security.SimpleUserInfo TryLoginUPA(String name, Guid Request_ID);

        #endregion

        #region Database Services

        [OperationContract]
        MSLA.Server.Data.SimpleTable FillDt(String CmdText, MSLA.Server.Data.EnDataCommandType cmdType,
            List<MSLA.Server.Data.DataParameter> cmdParams, MSLA.Server.Data.DBConnectionType cnType, int cmdTimeout, Server.Security.SimpleUserInfo myUser, Guid Request_ID);

        [OperationContract]
        List<MSLA.Server.Data.DataParameter> ExecuteCMM(String CmdText, MSLA.Server.Data.EnDataCommandType cmdType,
            List<MSLA.Server.Data.DataParameter> cmdParams, MSLA.Server.Data.DBConnectionType cnType, int cmdTimeout, Server.Security.SimpleUserInfo myUser, Guid Request_ID);

        [OperationContract]
        Object ExecuteScalar(String CmdText, MSLA.Server.Data.EnDataCommandType cmdType,
            List<MSLA.Server.Data.DataParameter> cmdParams, MSLA.Server.Data.DBConnectionType cnType, int cmdTimeout, Server.Security.SimpleUserInfo myUser, Guid Request_ID);

        #endregion

        #region Entity Related Services

        [OperationContract]
        [FaultContract(typeof(Exception))]
        MSLA.Server.Base.SimpleBOMaster FetchBOMaster(MasterCriteriaBase myBOCriteria, Guid Request_ID);

        [OperationContract]
        MSLA.Server.Base.SimpleBOMaster SaveBOMaster(MSLA.Server.Base.SimpleBOMaster myBO, Guid Request_ID);

        [OperationContract]
        MSLA.Server.Base.SimpleBOMaster DeleteBOMaster(MSLA.Server.Base.SimpleBOMaster myBO, Guid Request_ID);

        #endregion

        #region Misc Services

        [OperationContract]
        Dictionary<String, MSLA.Server.Data.SimpleTable> ResolveTablesFromFile(Byte[] FileStreamInfo, String FileExtension,
            String FilePassword, Server.Security.SimpleUserInfo UserInfo, Guid REquest_ID);

        [OperationContract]
        String HandleException(Guid Request_ID);

        [OperationContract]
        [ServiceKnownType(typeof(MSLA.Server.Data.SimpleTable))]
        MSLA.Server.Data.SimpleTable InvokeMethod(string assemblyName, string objectNamespace, string className, object[] constructorArgs,
                string methodName, object[] methodArgs,
                Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        [OperationContract]
        MSLA.Server.Data.SimpleTable GetSerivceInfo(Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        [OperationContract]
        String GetSerivceLog(Server.Security.SimpleUserInfo UserInfo, string ServiceName, Guid Request_ID);

        [OperationContract]
        String GetSerivceLogFromID(Server.Security.SimpleUserInfo UserInfo, Int64 Service_ID, Guid Request_ID);

        #endregion

        #region ComboBox services

        [OperationContract]
        Dictionary<Int64, String> GetResultSet(String collectionMember, String Filter, MSLA.Server.Data.DBConnectionType cnType,
            String valueMember, String displayMember, String queryText, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        [OperationContract]
        Int64 GetSelectValue(String collectionMember, String Filter, MSLA.Server.Data.DBConnectionType cnType, String valueMember,
            String displayMember, String queryText, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        [OperationContract]
        String GetSelectText(String collectionMember, String Filter, MSLA.Server.Data.DBConnectionType cnType, String valueMember,
           String displayMember, Int64 selectedVal, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        #endregion

        #region Feed Services

        [OperationContract]
        string GetCategoryFeed(Int64 Category_ID, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        [OperationContract]
        List<MSLA.Server.Tools.FeedItem> GetCategoryFeedItems(Int64 Category_ID, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        #endregion

        #region Subscription Services

        [OperationContract]
        MSLA.Server.Data.SimpleTable GetCategoryContract(Int64 Category_ID, Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        [OperationContract]
        Boolean ContractSubscriptionService(Int64 Category_ID, String message, Boolean IsHTML, String Subject, byte[] FileStreamInfo, string FileName, MSLA.Server.Data.SimpleTable testValue,
                Server.Security.SimpleUserInfo UserInfo, Guid Request_ID);

        #endregion
    }

    [DataContract]
    public class MasterCriteriaBase
        : MSLA.Server.BO.IMasterCriteria
    {
        long docMaster_ID = -1;
        string docMasterType = string.Empty;
        Server.Security.SimpleUserInfo _UserInfo;
        //Guid _Session_ID = new Guid();
        //long _User_ID = -1;
        Dictionary<string, object> _propertyCollection = new Dictionary<string, object>();

        [DataMember]
        public long DocMaster_ID
        {
            get { return docMaster_ID; }
            set { docMaster_ID = value; }
        }

        [DataMember]
        public string DocMasterType
        {
            get { return docMasterType; }
            set { docMasterType = value; }
        }

        [DataMember]
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

        [DataMember]
        public Dictionary<string, object> PropertyCollection
        {
            get { return _propertyCollection; }
            set { _propertyCollection = value; }
        }
    }
}
