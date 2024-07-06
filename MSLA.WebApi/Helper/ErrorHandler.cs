using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSLA.WebApi.ApiUtilities
{
    public class ErrorHandler
    {
        private static ErrorHandler _instance;
        public static ErrorHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ErrorHandler();
                }
                return _instance;
            }
        }

        public string FetchException(string _requestID)
        {
            return MSLA.Server.Exceptions.ServiceExceptionHandler.FetchException(_requestID);
        }

        public void SaveException(Guid Session_ID, Int64 User_ID,  Exception ex)
        {
            MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(Session_ID, User_ID, ex);
        }
    }
}