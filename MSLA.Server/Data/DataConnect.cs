using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Data
{
    public class DataConnect
    {

        private DataConnect()
        {}
       
        private static void GetResolvedTable(SqlDataReader reader, Data.SimpleTable dtResult)
        {
            System.Data.DataTable dtSchema = reader.GetSchemaTable();
            if (dtSchema != null)
            {
                foreach (System.Data.DataRow dr in dtSchema.Rows)
                {
                    dtResult.AddColumn(dr["ColumnName"].ToString(), dr["DataType"].ToString());
                }

                int ColCount = dtSchema.Rows.Count;
                while (reader.Read())
                {
                    Dictionary<string, object> drNew = new Dictionary<string, object>();
                    for (int i = 0; i < ColCount; i++)
                    {
                        drNew.Add(dtSchema.Rows[i][0].ToString(), reader.GetValue(i));
                    }
                    dtResult.AddRow(drNew);
                }
            }
        }

        public static System.Data.DataTable ResolveToSystemTable(Data.SimpleTable dt)
        {
            System.Data.DataTable dtResult = new System.Data.DataTable();
            foreach (KeyValuePair<string, string> dc in dt.Columns)
            {
                dtResult.Columns.Add(dc.Key, Type.GetType(dc.Value));
            }
            System.Data.DataRow drNew;

            foreach (Dictionary<string, object> dr in dt.Rows)
            {
                drNew = dtResult.NewRow();
                foreach (KeyValuePair<string, string> dc in dt.Columns)
                {
                    drNew[dc.Key] = dr[dc.Key];
                }
                dtResult.Rows.Add(drNew);
            }
            dtResult.AcceptChanges();
            return dtResult;
        }

        public static Data.SimpleTable ResolveToSimpleTable(System.Data.DataTable dt)
        {
            Data.SimpleTable dtResult = new SimpleTable();
            foreach (System.Data.DataColumn dc in dt.Columns)
            {
                dtResult.AddColumn(dc.ColumnName, Convert.ToString(dc.DataType));
            }

            int ColCount = dt.Columns.Count;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                Dictionary<string, object> drNew = new Dictionary<string, object>();
                for (int i = 0; i < ColCount; i++)
                {
                    drNew.Add(dt.Columns[i].ColumnName, dr[i]);
                }
                dtResult.AddRow(drNew);
            }
            return dtResult;


        }               

        private static void GetParamCollection(SqlCommand cmm,List<DataParameter> ParamList)
        {
            DataParameter tempParam;
            foreach (SqlParameter param in cmm.Parameters)
            {
                tempParam = new DataParameter();
                tempParam.DBType = (DataParameter.EnDataParameterType)param.DbType;
                tempParam.Direction = (DataParameter.EnParameterDirection)param.Direction;
                tempParam.ParameterName = param.ParameterName;
                tempParam.Precision = param.Precision;
                tempParam.Scale = param.Scale;
                tempParam.Size = param.Size;
                tempParam.Value = param.Value;
                ParamList.Add(tempParam);
            }
        }

        public static MSLA.Server.Data.SimpleTable FillDt(SqlCommand cmm, Security.IUser UserInfo, DBConnectionType cnType)
        {
            Data.SimpleTable dt = new Data.SimpleTable();
            using (SqlConnection cn = DataAccess.GetCn(cnType))
            {
                cn.Open();
                cmm.Connection = cn;
                using (SqlDataReader reader = cmm.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                {
                    GetResolvedTable(reader, dt);
                    reader.Close();
                }
                cn.Close();
            }
            return dt;
        }

        public static MSLA.Server.Data.SimpleTable FillDt(SqlCommand cmm, DBConnectionType cnType)
        {
            Data.SimpleTable dt = new Data.SimpleTable();
             
                using (SqlConnection cn = DataAccess.GetCn(cnType))
                {
                    cn.Open();
                    cmm.Connection = cn;
                    using (SqlDataReader reader = cmm.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                    {
                        GetResolvedTable(reader, dt);
                        reader.Close();
                    }
                    cn.Close();
                }
                return dt;
     
        }

        public static System.Data.DataTable FillDt(Security.IUser UserInfo, SqlCommand cmm, DBConnectionType cnType)
        {
            Data.DataAccess ServerDataAccess = new DataAccess();
            System.Data.DataTable dt = new System.Data.DataTable();
            return ServerDataAccess.FillDtInternal(cmm, UserInfo, cnType, ref dt);
        } 
      

        /// <summary>Fills A DataTable Instance already created. Can be used for strongly typed tables</summary>
        /// <param name="dtResult">Instance of the DataTable</param>
        /// <param name="cmm">The Command to execute</param>
        /// <param name="User">The User Info</param>
        /// <param name="DBType">The Connection Type</param>
        public static System.Data.DataTable FillDt(System.Data.DataTable dtResult, SqlCommand cmm, Security.IUser User, DBConnectionType DBType)
        {
            Data.DataAccess ServerDataAccess = new Data.DataAccess();
            ServerDataAccess.FillDtInternal(cmm, User, DBType, ref dtResult);
            return dtResult;
        }

        /// <summary>Executes a resultset(reader) Command and returns the Dataset</summary>
        /// <param name="cmm">The Command to execute</param>
        /// <param name="User">The User Info</param>
        /// <param name="DBType">The Connection Type</param>
        public static DataSet FillDS(SqlCommand cmm, Security.IUser User, DBConnectionType DBType)
        {
            Data.DataAccess ServerDataAccess = new Data.DataAccess();
            return ServerDataAccess.FillDS(cmm, User, DBType);
        }

        public static void ExecCMM(Security.IUser User, ref SqlCommand cmm, DBConnectionType DBType)
        {
            Data.DataAccess ServerDataAccess = new DataAccess();
            ServerDataAccess.ExecCMM(cmm, User, DBType);
        }
 
        /// <summary>Executes a non Query Command and returns it.</summary>
        /// <param name="cmm">The Command to execute</param>
        /// <param name="User">The User Info</param>
        /// <param name="DBType">The Connection Type</param>
        public static List<DataParameter> ExecCMM(SqlCommand cmm, Security.IUser User, DBConnectionType DBType)
        {
            List<Data.DataParameter> ParamList=new List<DataParameter>();
            Data.DataAccess ServerDataAccess = new DataAccess();
            ServerDataAccess.ExecCMM(cmm, User, DBType);
            GetParamCollection(cmm, ParamList);
            return ParamList;
        }

        /// <summary>Esecutes a scalar command and returns the result</summary>
        /// <param name="cmm">The Command to execute</param>
        /// <param name="User">The user Info</param>
        /// <param name="DBType">The Connection Type</param>
        public static object ExecuteScalar(SqlCommand cmm, Security.IUser User, DBConnectionType DBType)
        {
            DataAccess ServerDataAccess = new DataAccess();
            return ServerDataAccess.ExecuteScalar(cmm, User, DBType);
        }

        public static AttachedDoc getAttachment(Int64 Doc_ID)
        {
            AttachedDoc res = new AttachedDoc();
            res.Hasaccess = false;
            System.Data.DataTable dt = new System.Data.DataTable();
            using (SqlConnection con =DataAccess.GetCn(DBConnectionType.CompanyDMSDB))
            {

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                con.Open();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Select a.fldDocument_ID, a.fldDocumentName, b.fldDocumentImage From CP_DMS.DMS.tblDocument a " +
                                    " Inner Join CP_DMS.DMS.tblDocumentImage b On a.fldDocument_ID=b.fldDocument_ID" +
                                    " Where a.fldDocument_ID=@Document_ID";
                cmd.Parameters.Add("@Document_ID", SqlDbType.BigInt).Value = Doc_ID;
                SqlDataAdapter da = new SqlDataAdapter(cmd);                
                da.Fill(dt);
                con.Close();
            }
            if (dt.Rows.Count == 1)
            {
               res.FileData= (byte[])dt.Rows[0]["fldDocumentImage"];
               res.FileName = dt.Rows[0]["fldDocumentName"].ToString();
               res.Hasaccess = true;
            }
            return res;
        }

        public static AttachedDoc getAttachment(Int64 Doc_ID, Guid session_ID)
        {
            //Check for valid session ID
            System.Data.DataTable Sessiondetails = new System.Data.DataTable();
            AttachedDoc res = new AttachedDoc();
            res.Hasaccess = false;
            System.Data.DataTable dt = new System.Data.DataTable();

            using (SqlConnection con = DataAccess.GetCn(DBConnectionType.MainDB))
            { 
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                con.Open();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select  * from MSLA_Main.Main.tblUserSessionOpen where fldSession_ID=@Document_ID";
                cmd.Parameters.Add("@Document_ID", SqlDbType.UniqueIdentifier).Value = session_ID;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(Sessiondetails);
                con.Close();
            }

            if (Sessiondetails.Rows.Count > 0)
            {
                using (SqlConnection con = DataAccess.GetCn(DBConnectionType.CompanyDMSDB))
                { 
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    con.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Select a.fldDocument_ID, a.fldDocumentName, b.fldDocumentImage From CP_DMS.DMS.tblDocument a " +
                                        " Inner Join CP_DMS.DMS.tblDocumentImage b On a.fldDocument_ID=b.fldDocument_ID" +
                                        " Where a.fldDocument_ID=@Document_ID";
                    cmd.Parameters.Add("@Document_ID", SqlDbType.BigInt).Value = Doc_ID;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();
                }
                if (dt.Rows.Count == 1)
                {
                    res.FileData = (byte[])dt.Rows[0]["fldDocumentImage"];
                    res.FileName = dt.Rows[0]["fldDocumentName"].ToString();
                    res.Hasaccess = true;
                }
                return res;
            }
            return res; 
        }
        
    }

    public class AttachedDoc
    {
        public string FileName { get; set; }
        public Byte[] FileData { get; set; }
        public bool Hasaccess { get; set; }
    }
}
