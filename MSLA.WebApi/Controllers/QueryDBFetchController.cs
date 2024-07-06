using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MSLA.WebApi.Controllers
{
    public class QueryDBFetchController : ApiController
    {
        MSLA.WebApi.ApiUtilities.GenericDBResponse _response;

        ~QueryDBFetchController()
        {
            this.Dispose();
        }
      
        public HttpResponseMessage Get()
        {
            //Reterive data for PSRE

            try
            {
                //MSLA.Server.Data.DataCommand cmm = new Server.Data.DataCommand();
                //cmm.CommandText = "select * from Eibor.tblTenor";
                //cmm.CommandType = Server.Data.EnDataCommandType.Text;
                ////cmm.Parameters = cmdParams;
                //cmm.ConnectionType= Server.Data.DBConnectionType.OLTPDB;
                //cmm.CommandTimeout = cmdTimeout; 


                MSLA.WebApi.ApiUtilities.GenericQyRequest reqob = new ApiUtilities.GenericQyRequest()
                {
                    RequestObject = "EiborFetch" ,
                    Params = new List<ApiUtilities.BOHelper.Params>(){
                        new  ApiUtilities.BOHelper.Params()
                        {
                            Name = "@Tenor_ID",
                            direction = Server.Data.DataParameter.EnParameterDirection.Input,
                            ParamType = Server.Data.DataParameter.EnDataParameterType.BigInt,
                            value = 100001
                        }
                    }
                };

                var cmm = MSLA.WebApi.ApiUtilities.DBQueryFactory.GenerateMSLAQueryOb(reqob);

                _response = new MSLA.WebApi.ApiUtilities.GenericDBResponse()
                {
                    status = 200,
                    data = MSLA.Server.Data.DataConnect.FillDt(MSLA.Server.Data.DataCommand.GetSQLCommand(cmm), cmm.ConnectionType)
                };
                var resp =  Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericDBResponse>(HttpStatusCode.OK, _response); 
                return resp;
            }

            catch (Exception ex)
            {
                _response = new MSLA.WebApi.ApiUtilities.GenericDBResponse()
                {
                    status = 500,
                    statusText = ex.Message.ToString()
                };

                return Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericDBResponse>(HttpStatusCode.InternalServerError, _response);
            }

        }

        //// GET api/fillds/5
        //public MSLA.Server.Data.SimpleTable Get(Int64 id)
        //{
        //    MSLA.Server.Data.DataCommand cmm = new Server.Data.DataCommand();
        //    cmm.CommandText = "select  * from Globus.tblPSREDisplay where fldCustomerLiability_ID = " + id;
        //    cmm.CommandType = Server.Data.EnDataCommandType.Text;
        //    cmm.ConnectionType = Server.Data.DBConnectionType.OLTPDB;
        //    return MSLA.Server.Data.DataConnect.FillDt(MSLA.Server.Data.DataCommand.GetSQLCommand(cmm), MSLA.Server.Data.DBConnectionType.OLTPDB);
        //}

        [HttpPost]
        public HttpResponseMessage Post(MSLA.WebApi.ApiUtilities.GenericQyRequest Reqobj)
        {
            try
            { 
                var cmm = MSLA.WebApi.ApiUtilities.DBQueryFactory.GenerateMSLAQueryOb(Reqobj);

               _response = new MSLA.WebApi.ApiUtilities.GenericDBResponse()
               {
                   status = 200,
                   data = MSLA.Server.Data.DataConnect.FillDt(MSLA.Server.Data.DataCommand.GetSQLCommand(cmm), cmm.ConnectionType)
               };
              
                return Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericDBResponse>(HttpStatusCode.OK, _response);
            }

            catch (Exception ex)
            {
                _response = new MSLA.WebApi.ApiUtilities.GenericDBResponse()
                {
                    status = 500,
                    statusText = ex.Message.ToString()
                };
               
                return Request.CreateResponse<MSLA.WebApi.ApiUtilities.GenericDBResponse>(HttpStatusCode.InternalServerError, _response);
            }
        }

        protected override void Dispose(bool disposing)
        {  
            _response.Dispose();
            base.Dispose(disposing); 
            //GC.Collect();
            //GC.SuppressFinalize(this);
            //EventLog.WriteEntry("Web API", "Disponse invoked.." + DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));
        }
    }
}
