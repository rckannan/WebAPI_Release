using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MSLA.WebApi.Controllers
{
    public class AutoCompleteController : ApiController
    {
        List<MSLA.Server.Tools.AutoCompleteCollection> _response = null;

        ~AutoCompleteController()
        {
        }

        public HttpResponseMessage Get()
        {
            try
            {
                MSLA.Server.ApiUtilities.CachedItem Reqobj = new Server.ApiUtilities.CachedItem()
                {
                    cnType = MSLA.Server.Data.DBConnectionType.OLTPDB,
                    collectionMember = "DealReg.tblCustomer",
                    displayMember = "fldCustomer",
                    Filter  = string.Empty,
                    valueMember = "fldCustomer_ID"
                };
                _response = MSLA.Server.ApiUtilities.CacheCollectionRepository.getInstance().GetResultSet(Reqobj.collectionMember, Reqobj.Filter,
                        Reqobj.cnType, Reqobj.valueMember, Reqobj.displayMember, string.Empty, new Guid());

                return Request.CreateResponse<List<MSLA.Server.Tools.AutoCompleteCollection>>(HttpStatusCode.OK, _response);
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post(MSLA.Server.ApiUtilities.CachedItem Reqobj)
        {
            try
            {
                _response = MSLA.Server.ApiUtilities.CacheCollectionRepository.getInstance().GetResultSet(Reqobj.collectionMember, Reqobj.Filter,
                        Reqobj.cnType, Reqobj.valueMember, Reqobj.displayMember, string.Empty, new Guid());

                return Request.CreateResponse<List<MSLA.Server.Tools.AutoCompleteCollection>>(HttpStatusCode.OK, _response);
            }

            catch (Exception ex)
            { 
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            //_response.Clear();
            //_response = null;
            base.Dispose(disposing); 
        }
    }
}
