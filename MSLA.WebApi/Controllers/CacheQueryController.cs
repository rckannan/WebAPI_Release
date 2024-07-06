using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MSLA.WebApi.ApiUtilities;
using System.Collections.Concurrent;
using MSLA.Server.ApiUtilities;

namespace MSLA.WebApi.Controllers
{
    public class CacheQueryController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            try
            {
                 return Request.CreateResponse(HttpStatusCode.OK,  DBQueryFactory._cachedItems); 
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(string id)
        { 
            try
            {
                QueryObject  _cachedItems=new  QueryObject ();
                if (DBQueryFactory._cachedItems.TryGetValue(id, out  _cachedItems))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _cachedItems); 
                }

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Requested Object is not available in the cache");
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,ex);
            } 
        }

       
        [HttpPost]
        public HttpResponseMessage Post()
        {
            try
            {
                DBQueryFactory._cachedItems.Clear(); 
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }  
    }
}
