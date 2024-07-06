using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSLA.WebApi.ApiUtilities
{
    public class GenericBOResponse : IDisposable
    {
        public Server.Base.SimpleBOMaster data { get; set; }
        public int status { get; set; }
        public string statusText { get; set; }

        public GenericBOResponse()
        {
            data = null;
            status = -1;
            statusText = string.Empty;
        }

        public void Dispose()
        {
            data.Dispose();
        }
    }
}