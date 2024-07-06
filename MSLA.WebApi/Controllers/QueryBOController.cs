using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MSLA.WebApi.Helper;
using MSLA.WebApi.ApiUtilities;

namespace MSLA.WebApi.Controllers
{
    public class QueryBOController : ApiController, IDisposable
    {
        MSLA.Server.Login.LogonResult myLogonResult;
        MSLA.WebApi.ApiUtilities.GenericBOResponse _result;
        public QueryBOController()
        {
            Server.Security.SimpleUserInfo myUser = new Server.Security.SimpleUserInfo(); 

            MSLA.Server.Login.LogonInfo myLogonInfo = new Server.Login.LogonInfo("chinnakannanr");
            MSLA.Server.Login.Logon myLogon = new Server.Login.Logon();
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
        }

        // GET api/qureybo
        public HttpResponseMessage Get()
        { 
            try
            { 
                MSLA.WebApi.ApiUtilities.BOHelper.MasterCriteriaBase bse = new ApiUtilities.BOHelper.MasterCriteriaBase()
                {
                    DocMaster_ID = -1,
                    DocMasterType = "EIBORDataSource",
                    PropertyCollection = new Dictionary<string, object>()
                };

                return Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericBOResponse>(HttpStatusCode.OK, BOHelper.Instance.FetchBOMaster(bse, myLogonResult.Session_ID));
            }

            catch (Exception ex)
            {
                var resp = new MSLA.WebApi.ApiUtilities.GenericBOResponse()
                {
                    status = 500,
                    statusText = ex.Message.ToString()
                };

                MSLA.WebApi.ApiUtilities.ErrorHandler.Instance.SaveException(myLogonResult.Session_ID, myLogonResult.User_ID, ex);
                return Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericBOResponse>(HttpStatusCode.InternalServerError, resp);
            } 
        }

        public HttpResponseMessage Get(string DocType, string Doc_ID)
        {
            try
            { 
                MSLA.WebApi.ApiUtilities.BOHelper.MasterCriteriaBase bse = new ApiUtilities.BOHelper.MasterCriteriaBase()
                {
                    DocMaster_ID = Convert.ToInt64(Doc_ID),
                    DocMasterType = DocType,
                    PropertyCollection = new Dictionary<string, object>()
                };

                _result = BOHelper.Instance.FetchBOMaster(bse, myLogonResult.Session_ID); 
                return Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericBOResponse>(HttpStatusCode.OK, _result);

            }

            catch (Exception ex)
            {
                var resp = new MSLA.WebApi.ApiUtilities.GenericBOResponse()
                {
                    status = 500,
                    statusText = ex.Message.ToString()
                };
                MSLA.WebApi.ApiUtilities.ErrorHandler.Instance.SaveException(myLogonResult.Session_ID, myLogonResult.User_ID, ex);
                return Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericBOResponse>(HttpStatusCode.InternalServerError, resp);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post(Server.Base.SimpleBOMaster myBO)
        {
            try
            {
                _result = BOHelper.Instance.SaveBOMaster(myBO);
                return Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericBOResponse>(HttpStatusCode.OK, _result); 
            }

            catch (Exception ex)
            {
                var resp = new MSLA.WebApi.ApiUtilities.GenericBOResponse()
                {
                    status = 500,
                    statusText = ex.Message.ToString()
                };
                MSLA.WebApi.ApiUtilities.ErrorHandler.Instance.SaveException(myLogonResult.Session_ID, myLogonResult.User_ID, ex);
                return Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericBOResponse>(HttpStatusCode.InternalServerError, resp);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _result.Dispose();
            base.Dispose(disposing); 
        }
    }
}
