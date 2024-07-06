using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Tools
{
    public class FeedWorker
    {
        public static string generateFeed(Security.IUser UserInfo, Int64 Category_ID)
        {
            StringBuilder strFeed = new StringBuilder();
            strFeed.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            strFeed.AppendLine("<rss version=\"2.0\">");
            strFeed.AppendLine("<channel>");
            strFeed.AppendLine("<title>SiteName</title>");
            strFeed.AppendLine("<description> Site Description </description>");
            strFeed.AppendLine("<link></link>");

            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spRSSFeedFetch";
            cmm.Parameters.Add("@Category_ID", SqlDbType.BigInt).Value = Category_ID;
            DataTable dtFeed = new DataTable();
            dtFeed = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.CompanyDB);
            foreach (DataRow drFeed in dtFeed.Rows)
            {
                strFeed.AppendLine("<item>");
                strFeed.AppendLine("<title>" + drFeed["fldTitle"] + "</title>");
                strFeed.AppendLine("<date>" + drFeed["fldPublishedDate"] + "</date>");
                strFeed.AppendLine("<description>" + drFeed["fldDescription"] + "</description>");
                strFeed.AppendLine("<link>" + drFeed["fldLink"] + "</link>");
                strFeed.AppendLine("</item>");
            }
            strFeed.AppendLine("</channel>");
            strFeed.AppendLine("</rss>");

            //string xmlText = strFeed.ToString();
            //StringReader stringReader = new StringReader(xmlText);
            //XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(stringReader);

            return strFeed.ToString();
        }

        public static List<FeedItem> getFeedItems(Security.IUser UserInfo, Int64 Category_ID)
        {
            FeedItem myFeed;
            List<FeedItem> myFeeds = new List<FeedItem>();

            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spRSSFeedFetch";
            cmm.Parameters.Add("@Category_ID", SqlDbType.BigInt).Value = Category_ID;
            DataTable dtFeed = new DataTable();
            dtFeed = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.CompanyDB);
            foreach (DataRow drFeed in dtFeed.Rows)
            {
                myFeed = new FeedItem();
                myFeed.Title = drFeed["fldTitle"].ToString();
                myFeed.Description = drFeed["fldDescription"].ToString();
                myFeed.Link = drFeed["fldLink"].ToString();
                myFeed.PublishedDate = DateTime.Parse(drFeed["fldPublishedDate"].ToString());
                myFeeds.Add(myFeed);
            }
            return myFeeds;
        }
    }
    
    public class FeedItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime PublishedDate { get; set; }
    }

}
