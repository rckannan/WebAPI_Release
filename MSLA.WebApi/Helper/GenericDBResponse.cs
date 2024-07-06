using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSLA.WebApi.ApiUtilities
{
    public class GenericDBResponse : IDisposable
    {
        public MSLA.Server.Data.SimpleTable data { get; set; }
        public int status { get; set; }
        public string statusText { get; set; }

        public GenericDBResponse()
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