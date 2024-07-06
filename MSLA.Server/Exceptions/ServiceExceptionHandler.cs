using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MSLA.Server.Exceptions
{
    public class ServiceExceptionHandler
    {

        public static void HandleException(Security.SimpleUserInfo UserInfo, Exception ex, String Request_ID)
        {
            StringBuilder strbInnerException = new StringBuilder();

            strbInnerException = ResolveInnerException(ex.InnerException, strbInnerException);

            SqlCommand cmm = new SqlCommand();
            SqlConnection cn = null;
            cn = Data.DataAccess.GetCn(Data.DBConnectionType.CompanyDB);
            cn.Open();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spExceptionLogAdd";
            cmm.Connection = cn;
            cmm.Parameters.Add("@User_ID", SqlDbType.BigInt).Value = UserInfo.User_ID;
            cmm.Parameters.Add("@UserSession_ID", SqlDbType.VarChar, 200).Value = UserInfo.Session_ID.ToString();
            cmm.Parameters.Add("@Desc", SqlDbType.VarChar, 2000).Value = ex.Message;
            cmm.Parameters.Add("@Sender", SqlDbType.VarChar, 50).Value = ex.Source;
            cmm.Parameters.Add("@LastUpdated", SqlDbType.DateTime).Value = DateTime.Now;
            cmm.Parameters.Add("@Request_ID", SqlDbType.VarChar, 200).Value = Request_ID;
            cmm.Parameters.Add("@InnerException", SqlDbType.VarChar).Value = strbInnerException.ToString();
            cmm.ExecuteNonQuery();
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        public static void HandleException(Guid Session_ID, Int64 User_ID, Exception ex)
        {
            StringBuilder strbInnerException = new StringBuilder();

            strbInnerException = ResolveInnerException(ex.InnerException, strbInnerException);

            SqlCommand cmm = new SqlCommand();
            SqlConnection cn = null;
            cn = Data.DataAccess.GetCn(Data.DBConnectionType.CompanyDB);
            cn.Open();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spExceptionLogAdd";
            cmm.Connection = cn;
            cmm.Parameters.Add("@User_ID", SqlDbType.BigInt).Value = User_ID;
            cmm.Parameters.Add("@UserSession_ID", SqlDbType.VarChar, 200).Value = Session_ID.ToString();
            cmm.Parameters.Add("@Desc", SqlDbType.VarChar, 2000).Value = ex.Message;
            cmm.Parameters.Add("@Sender", SqlDbType.VarChar, 50).Value = ex.Source;
            cmm.Parameters.Add("@LastUpdated", SqlDbType.DateTime).Value = DateTime.Now;
            cmm.Parameters.Add("@Request_ID", SqlDbType.VarChar, 200).Value = Session_ID.ToString();
            cmm.Parameters.Add("@InnerException", SqlDbType.VarChar).Value = strbInnerException.ToString();
            cmm.ExecuteNonQuery();
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        private static StringBuilder ResolveInnerException(Exception ex, StringBuilder strb)
        {
            if (ex != null && ex.Message != String.Empty)
            {
                strb.AppendLine(ex.ToString());
                ResolveInnerException(ex.InnerException, strb);
            }
            return strb;
        }

        public static string FetchException(String Request_ID)
        {
            SqlCommand cmm = new SqlCommand();
            SqlConnection cn = null;
            cn = Data.DataAccess.GetCn(Data.DBConnectionType.CompanyDB);
            if (cn.State != ConnectionState.Open)
            {
                cn.Open();
            }
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spExceptionLogFetch";
            cmm.Connection = cn;
            cmm.Parameters.Add("@Request_ID", SqlDbType.VarChar, 200).Value = Request_ID;
            cmm.Parameters.Add("@Desc", SqlDbType.VarChar, 2000).Value = string.Empty;
            cmm.Parameters["@Desc"].Direction = ParameterDirection.InputOutput;
            cmm.ExecuteNonQuery();
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
            return cmm.Parameters["@Desc"].Value.ToString();
        }

        public static string FetchExceptionDetail(String Request_ID)
        {
            SqlCommand cmm = new SqlCommand();
            SqlConnection cn = null;
            cn = Data.DataAccess.GetCn(Data.DBConnectionType.CompanyDB);
            if (cn.State != ConnectionState.Open)
            {
                cn.Open();
            }
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spExceptionDetailFetch";
            cmm.Connection = cn;
            cmm.Parameters.Add("@Request_ID", SqlDbType.VarChar, 200).Value = Request_ID;
            cmm.Parameters.Add("@Desc", SqlDbType.VarChar).Value = string.Empty;
            cmm.Parameters["@Desc"].Direction = ParameterDirection.InputOutput;
            cmm.ExecuteNonQuery();
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
            return cmm.Parameters["@Desc"].Value.ToString();
        }

    }
}
