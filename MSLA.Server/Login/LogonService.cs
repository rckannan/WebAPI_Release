using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace MSLA.Server.Login
{
    public class LogonService
    {
        public static Security.SimpleUserInfo SaveLogonInfo(LogonResult myLogonResult)
        {
            Security.SimpleUserInfo mySimpleUserInfo = new Security.SimpleUserInfo();
            mySimpleUserInfo.Session_ID = myLogonResult.Session_ID;
            mySimpleUserInfo.User_ID = myLogonResult.User_ID;
            mySimpleUserInfo.UserName = myLogonResult.FullUserName;

            //persist user info to database

            Serialize(myLogonResult);

            return mySimpleUserInfo;
        }

        public static LogonResult FetchLogonInfo(Guid Session_ID)
        {
            Byte[] obj_bytes;
            SqlCommand cmm = new SqlCommand();
            SqlDataReader rdr;

            using (SqlConnection cn = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB))
            {
                cmm.Connection = cn;
                if (cn.State == System.Data.ConnectionState.Closed)
                {
                    cn.Open();
                }
                cmm.CommandType = System.Data.CommandType.StoredProcedure;
                cmm.CommandText = "System.spSerializedLogonInfoFetch";
                cmm.Parameters.Add("@Session_ID", System.Data.SqlDbType.UniqueIdentifier).Value = Session_ID;
                rdr = cmm.ExecuteReader();

                LogonResult myLogonResult;

                while (rdr.Read())
                {
                    if (rdr[0] != System.DBNull.Value)
                    {
                        obj_bytes = new Byte[((Byte[])rdr[0]).Length];
                        obj_bytes = (Byte[])rdr[0];
                        System.IO.MemoryStream MStream = new System.IO.MemoryStream();
                        MStream.Write(obj_bytes, 0, obj_bytes.Length);
                        MStream.Flush();
                        MStream.Position = 0;
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter sbf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        object obj = sbf.Deserialize(MStream);
                        myLogonResult = (LogonResult)obj;
                        return myLogonResult;
                    }
                }
            }
            return null;
        }

        private static void Serialize(LogonResult myLogonResult)
        {
            System.IO.MemoryStream MStream = new System.IO.MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter sbf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            sbf.Serialize(MStream, myLogonResult);
            Byte[] obj_bytes;
            obj_bytes = new Byte[MStream.Length];
            MStream.Position = 0;
            MStream.Read(obj_bytes, 0, (int)MStream.Length);
            MStream.Flush();
            MStream.Close();

            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = System.Data.CommandType.StoredProcedure;
            cmm.CommandText = "System.spSerializedLogonInfoAdd";

            cmm.Parameters.Add("@Session_ID", System.Data.SqlDbType.UniqueIdentifier).Value = myLogonResult.Session_ID;
            cmm.Parameters.Add("@S_Value", System.Data.SqlDbType.VarBinary, obj_bytes.Length).Value = obj_bytes;
            Data.DataConnect.ExecCMM(myLogonResult, ref cmm, Data.DBConnectionType.MainDB);
        }

        public static void LogOut(Guid Session_ID)
        {
            using (SqlConnection cn = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB))
            {
                SqlCommand cmm = new SqlCommand();
                cmm.Connection = cn;
                if (cn.State == System.Data.ConnectionState.Closed)
                {
                    cn.Open();
                }
                cmm.CommandType = System.Data.CommandType.StoredProcedure;
                cmm.CommandText = "System.spSerializedLogonInfoDelete";

                cmm.Parameters.Add("@Session_ID", System.Data.SqlDbType.UniqueIdentifier).Value = Session_ID;
                cmm.ExecuteNonQuery();
            }
        }
    }
}
