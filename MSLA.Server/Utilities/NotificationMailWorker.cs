using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MSLA.Server.Utilities
{
    public class NotificationMailWorker
    {
        private Int64 _fldCategory_ID = -1;
        private string _fldMailTo = string.Empty;
        private string _fldMailFrom = string.Empty;
        private string _fldBody = string.Empty;
        private string _fldSubject = string.Empty;
        private string _fldCc = string.Empty;
        private string _fldBCc = string.Empty;
        private byte [] _fldAttachment;
        private string _fldFileName = string.Empty;

        public Int64 fldCategory_ID
        {
            get { return _fldCategory_ID; }
            set { _fldCategory_ID = value; }
        }

        public String fldBody
        {
            get { return _fldBody; }
            set { _fldBody = value; }
        }

        public string fldMailTo
        {
            get { return _fldMailTo; }
            set { _fldMailTo = value; }
        }
        
        public string fldMailFrom
        {
            get { return _fldMailFrom; }
            set { _fldMailFrom = value; }
        }

        public String fldSubject
        {
            get { return _fldSubject; }
            set { _fldSubject = value; }
        }

        public string fldCc
        {
            get { return _fldCc; }
            set { _fldCc = value; }
        }

        public string fldBCc
        {
            get { return _fldBCc; }
            set { _fldBCc = value; }
        }

        public byte[] fldAttachment
        {
            get { return _fldAttachment; }
            set { _fldAttachment = value; }
        }

        public String fldFileName
        {
            get { return _fldFileName; }
            set { _fldFileName = value; }
        }

        public NotificationMailWorker()
        {

        }

        public void SaveNotificationMail(Security.IUser UserInfo)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = System.Data.CommandType.StoredProcedure;
            cmm.CommandText = "System.spNotificationMailAdd";

            cmm.Parameters.Add("@Category_ID", SqlDbType.BigInt).Value = this.fldCategory_ID;
            cmm.Parameters.Add("@MailTo", SqlDbType.VarChar, 250).Value = this.fldMailTo;
            cmm.Parameters.Add("@MailFrom", SqlDbType.VarChar, 100).Value = this.fldMailFrom;
            cmm.Parameters.Add("@Body", SqlDbType.VarChar, 1000).Value = this.fldBody;
            cmm.Parameters.Add("@Subject", SqlDbType.VarChar, 250).Value = this.fldSubject;
            cmm.Parameters.Add("@Cc", SqlDbType.VarChar, 250).Value = this.fldCc;
            cmm.Parameters.Add("@BCc", SqlDbType.VarChar, 250).Value = this.fldBCc;
            cmm.Parameters.Add("@FileName", SqlDbType.VarChar, 250).Value = this.fldFileName;
            cmm.Parameters.Add("@Attachment", SqlDbType.VarBinary, this.fldAttachment.Length).Value = this.fldAttachment;

            Data.DataConnect.ExecCMM(UserInfo, ref cmm, Data.DBConnectionType.CompanyDB);
        }

    }
}
