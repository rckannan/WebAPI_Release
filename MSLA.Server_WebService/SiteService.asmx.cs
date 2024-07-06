using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace MSLA.Server_WebService
{
    /// <summary>
    /// Summary description for SiteService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class SiteService : System.Web.Services.WebService
    {
        [WebMethod]
        public bool IsValidUser(string UserName)
        {
            return MSLA.Server.Login.Logon.IsValidUser(UserName);
        }

        [WebMethod]
        public bool IsValidUPAUser(string UserName)
        {
            return MSLA.Server.Login.Logon.IsValidUPAUser(UserName);
        }

    }
}
