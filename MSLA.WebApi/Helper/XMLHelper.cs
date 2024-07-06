using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Configuration;

namespace MSLA.WebApi.ApiUtilities
{
    /// <summary>
    /// Class with static method to reterive queries from XML Config
    /// </summary>
    public class XMLHelper
    {
        static String templatepath = ConfigurationManager.AppSettings["FileTemplatePath"];
        static XDocument xdoc;
        static QueryObject  _QueryObject;
        static XMLHelper()
        {
            xdoc = XDocument.Load(templatepath);
        }

        internal static QueryObject ReadXMLQuery(string ObjectName)
        { 
            var selectedTemplate = xdoc.Descendants("query").Where(x => (string)x.Attribute("name") == ObjectName);

            if (selectedTemplate == null) return _QueryObject;

            _QueryObject = new QueryObject();
            _QueryObject.ObjectName = ObjectName;
            _QueryObject.Query = selectedTemplate.Select(x => x.Element("BaseQuery").Value).FirstOrDefault();
            _QueryObject.DBConnectionType = (MSLA.Server.Data.DBConnectionType)Convert.ToInt32(selectedTemplate.Select(x => x.Attribute("DBType").Value).FirstOrDefault());
            _QueryObject.EnDataCommandType = (MSLA.Server.Data.EnDataCommandType)Convert.ToInt32(selectedTemplate.Select(x => x.Attribute("QueryType").Value).FirstOrDefault()); 

            return _QueryObject;
        }
    }

   
    public class  QueryObject
    {
        public string  ObjectName { get; set; }
        public string Query { get; set; }
        public MSLA.Server.Data.DBConnectionType DBConnectionType { get; set; }
        public MSLA.Server.Data.EnDataCommandType EnDataCommandType { get; set; }
        public int timeOut { get; set; } 
    }
     
}