using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Collections.Concurrent;

namespace MSLA.WebApi.ApiUtilities
{
    public class GenericQyRequest
    {
        public string RequestObject { get; set; }
        public List<MSLA.WebApi.ApiUtilities.BOHelper.Params> Params { get; set; }
        public int TimeOut { get; set; }

        public GenericQyRequest()
        {
            RequestObject = string.Empty;
            Params = new List<BOHelper.Params>();
            TimeOut = 30;
        }
    }
     
}