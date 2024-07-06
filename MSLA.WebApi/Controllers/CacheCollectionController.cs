using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MSLA.WebApi.Helper;
using System.Collections.Concurrent;
using MSLA.Server.ApiUtilities;

namespace MSLA.WebApi.Controllers
{
    public class CacheCollectionController : ApiController
    { 
           
        [HttpGet]
        public HttpResponseMessage Get()
        {
            try
            {
                var retobj =  CacheCollectionRepository.getInstance().GetCacheItem(); 
                return Request.CreateResponse(HttpStatusCode.OK, retobj);  
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, ex);
            }
        } 
        
        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                CacheCollectionRepository.getInstance().ForceClear(id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        
        [HttpGet]
        public HttpResponseMessage Post()
        {
            try
            {
                CacheCollectionRepository.getInstance().ResetAll();
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
