using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MSLA.Server.Utilities
{
    public class RSSFeedWorker
    {
        private Int64 _fldCategory_ID = -1;
        private string _fldDescription = string.Empty;
        private string _fldTitle = string.Empty;
        private string _fldLink = string.Empty;
        private DateTime _fldPublishedDate = DateTime.Now;

        public Int64 fldCategory_ID
        {
            get { return _fldCategory_ID; }
            set { _fldCategory_ID = value; }
        }

        public String fldDescription
        {
            get { return _fldDescription; }
            set { _fldDescription = value; }
        }

        public string fldTitle
        {
            get { return _fldTitle; }
            set { _fldTitle = value; }
        }

        public string fldLink
        {
            get { return _fldLink; }
            set { _fldLink = value; }
        }

        public DateTime fldPublishedDate
        {
            get { return _fldPublishedDate; }
            set { _fldPublishedDate = value; }
        }

        public RSSFeedWorker(Int64 Category_ID, string Title, string Description, String Link)
        {
            _fldCategory_ID = Category_ID;
            _fldDescription = Description;
            _fldLink = Link;
            _fldTitle = Title;
        }

        public void SaveRSSFeed(Security.IUser UserInfo)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = System.Data.CommandType.StoredProcedure;
            cmm.CommandText = "System.spRSSFeedAdd";

            cmm.Parameters.Add("@Category_ID", SqlDbType.BigInt).Value = this.fldCategory_ID;
            cmm.Parameters.Add("@Description", SqlDbType.VarChar, 500).Value = this.fldDescription;
            cmm.Parameters.Add("@Title", SqlDbType.VarChar, 100).Value = this.fldTitle;
            cmm.Parameters.Add("@Link", SqlDbType.VarChar, 500).Value = this.fldLink;
            cmm.Parameters.Add("@PublishedDate", SqlDbType.DateTime).Value = this.fldPublishedDate;

            Data.DataConnect.ExecCMM(UserInfo, ref cmm, Data.DBConnectionType.MainDB);
        }
    }
}
