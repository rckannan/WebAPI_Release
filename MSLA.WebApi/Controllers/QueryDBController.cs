using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

namespace MSLA.WebApi.Controllers
{
    public class QueryDBController : ApiController
    {
        // GET api/fillds
        public  MSLA.Server.Data.SimpleTable  Get()
        {
             //Reterive data for PSRE

         
               MSLA.Server.Data.DataCommand cmm = new Server.Data.DataCommand();
              cmm.CommandText = "select  * from Globus.tblPSREDisplay";
              cmm.CommandType = Server.Data.EnDataCommandType.Text;
              cmm.ConnectionType = Server.Data.DBConnectionType.OLTPDB;
              return MSLA.Server.Data.DataConnect.FillDt(MSLA.Server.Data.DataCommand.GetSQLCommand(cmm), MSLA.Server.Data.DBConnectionType.OLTPDB);
          
        
        }

        // GET api/fillds/5
        public MSLA.Server.Data.SimpleTable Get(Int64 id)
        {
            MSLA.Server.Data.DataCommand cmm = new Server.Data.DataCommand();
            cmm.CommandText = "select  * from Globus.tblPSREDisplay where fldCustomerLiability_ID = " + id;
            cmm.CommandType = Server.Data.EnDataCommandType.Text;
            cmm.ConnectionType = Server.Data.DBConnectionType.OLTPDB;
            return MSLA.Server.Data.DataConnect.FillDt(MSLA.Server.Data.DataCommand.GetSQLCommand(cmm), MSLA.Server.Data.DBConnectionType.OLTPDB);
        }

        // POST api/fillds
        public void Post([FromBody]string value)
        {
        }

        // PUT api/fillds/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/fillds/5
        public void Delete(int id)
        {
        }
    }
}
