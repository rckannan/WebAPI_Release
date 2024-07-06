using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Collections.Concurrent;

namespace MSLA.WebApi.ApiUtilities
{
    public class DBQueryFactory
    { 
        //private Boolean _allowCache = true;
        internal static ConcurrentDictionary<string,QueryObject> _cachedItems;

        static DBQueryFactory() { _cachedItems = new ConcurrentDictionary<string, QueryObject>(); } 

        public static MSLA.Server.Data.DataCommand GenerateMSLAQueryOb(MSLA.WebApi.ApiUtilities.GenericQyRequest reqObj)
        {
            MSLA.Server.Data.DataCommand cmm = null;
            try
            {
                QueryObject qObj = new QueryObject();
                if (!(_cachedItems.TryGetValue(reqObj.RequestObject, out qObj))) 
                {
                    qObj = XMLHelper.ReadXMLQuery(reqObj.RequestObject);
                    _cachedItems.TryAdd(reqObj.RequestObject, qObj);
                } 

                //Load the MSLA Object
                cmm = new Server.Data.DataCommand();
                cmm.CommandText = qObj.Query;
                cmm.CommandType = qObj.EnDataCommandType;

                cmm.ConnectionType = qObj.DBConnectionType;
                cmm.CommandTimeout = qObj.timeOut; 

                //Load the params
                if (reqObj.Params.Count() > 0)
                {
                    foreach (var param in reqObj.Params)
                    {
                        var param1 = new Server.Data.DataParameter();
                        param1.ParameterName = param.Name;
                        param1.Value = param.value;
                        param1.Direction = param.direction;
                        param1.DBType = param.ParamType;

                        cmm.Parameters.Add(param1); 
                    }
                }  
            }
            catch (Exception ex)
            {
                MSLA.Server.Exceptions.ServiceExceptionHandler.HandleException(null, ex, "Web API Call");
            }
            return cmm;
        }
    }
     
}