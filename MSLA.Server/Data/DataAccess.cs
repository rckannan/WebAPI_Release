using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Data
{
    /// <summary>The Server side Data Access Class. This is an anchored object and is not to be used directly.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class DataAccess
        : MarshalByRefObject
    {

        internal DataAccess()
            : base()
        {
        }

        #region "FetchDocMaster"

        internal Base.DocMaster FetchDocMaster(String DocMasterType, Security.IUser UserInfo)
        {
            if (DocMasterType == String.Empty)
            {
                throw new Exception("DocMasterType cannot be left blank. Please request a valid DocMasterType");
            }

            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spDocMasterFetch";

            //   ****    Set the Value
            cmm.Parameters.Add("@DocMasterType", SqlDbType.VarChar, 50).Value = DocMasterType;
            //   *****   Set Param Direction Only
            cmm.Parameters.Add("@DocObject_ID", SqlDbType.BigInt, 0).Direction = ParameterDirection.Output;
            cmm.Parameters.Add("@DocAssembly", SqlDbType.VarChar, 120).Direction = ParameterDirection.Output;
            cmm.Parameters.Add("@DocNameSpace", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
            cmm.Parameters.Add("@DocObject", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
            cmm.Parameters.Add("@DocDescription", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output;

            ExecCMM(cmm, UserInfo, MSLA.Server.Data.DBConnectionType.MainDB);

            Base.DocMaster myDocMaster = new Base.DocMaster();
            myDocMaster.DocObjectType = DocMasterType;
            myDocMaster.DocObject_ID = Convert.ToInt64(cmm.Parameters["@DocObject_ID"].Value);
            myDocMaster.DocAssembly = Convert.ToString(cmm.Parameters["@DocAssembly"].Value);
            myDocMaster.DocNameSpace = Convert.ToString(cmm.Parameters["@DocNameSpace"].Value);
            myDocMaster.DocObjectName = Convert.ToString(cmm.Parameters["@DocObject"].Value);
            myDocMaster.DocDescription = Convert.ToString(cmm.Parameters["@DocDescription"].Value);

            if (myDocMaster.DocObject_ID <= 0)
            {
                throw new Exception("Master Document '" + DocMasterType + "' not found in tblDocMasters. Failed to load the Business Object.");
            }
            else
            {
                return myDocMaster;
            }
        }

        internal Base.DocMaster FetchDocMaster(String DocMasterType )
        {
            if (DocMasterType == String.Empty)
            {
                throw new Exception("DocMasterType cannot be left blank. Please request a valid DocMasterType");
            }

            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spDocMasterFetch";

            //   ****    Set the Value
            cmm.Parameters.Add("@DocMasterType", SqlDbType.VarChar, 50).Value = DocMasterType;
            //   *****   Set Param Direction Only
            cmm.Parameters.Add("@DocObject_ID", SqlDbType.BigInt, 0).Direction = ParameterDirection.Output;
            cmm.Parameters.Add("@DocAssembly", SqlDbType.VarChar, 120).Direction = ParameterDirection.Output;
            cmm.Parameters.Add("@DocNameSpace", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
            cmm.Parameters.Add("@DocObject", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
            cmm.Parameters.Add("@DocDescription", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output;

            ExecCMM(cmm,  MSLA.Server.Data.DBConnectionType.MainDB);

            Base.DocMaster myDocMaster = new Base.DocMaster();
            myDocMaster.DocObjectType = DocMasterType;
            myDocMaster.DocObject_ID = Convert.ToInt64(cmm.Parameters["@DocObject_ID"].Value);
            myDocMaster.DocAssembly = Convert.ToString(cmm.Parameters["@DocAssembly"].Value);
            myDocMaster.DocNameSpace = Convert.ToString(cmm.Parameters["@DocNameSpace"].Value);
            myDocMaster.DocObjectName = Convert.ToString(cmm.Parameters["@DocObject"].Value);
            myDocMaster.DocDescription = Convert.ToString(cmm.Parameters["@DocDescription"].Value);

            if (myDocMaster.DocObject_ID <= 0)
            {
                throw new Exception("Master Document '" + DocMasterType + "' not found in tblDocMasters. Failed to load the Business Object.");
            }
            else
            {
                return myDocMaster;
            }
        }

        #endregion

        #region SQL Methods

        internal void ExecCMM(SqlCommand cmm, Security.IUser UserInfo, Data.DBConnectionType DBType)
        {

            try
            {
                using (SqlConnection cn = DataAccess.GetCn(DBType))
                {
                    cn.Open();
                    cmm.Connection = cn;
                    cmm.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
              throw new Data.DataAccessException(cmm.CommandText, DBType, ex);
            }
        }

        internal void ExecCMM(SqlCommand cmm,  Data.DBConnectionType DBType)
        {

            try
            {
                using (SqlConnection cn = DataAccess.GetCn(DBType))
                {
                    cn.Open();
                    cmm.Connection = cn;
                    cmm.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Data.DataAccessException(cmm.CommandText, DBType, ex);
            }
        }

        /// <summary>/This method can be used to execute Non Query commands on the server.</summary>
        ///<param name="cmm">The Command to Execute.</param>
        ///<param name="UserInfo">Logon User Info.</param>
        ///<param name="DBType">The Database to connect.</param>
        ///<returns>Returns the executed command.</returns>
        public Data.DataCommandResult ExecCMM(Data.DataCommand cmm, Security.IUser UserInfo, Data.DBConnectionType DBType)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            this.ExecCMM(CmmLocal, UserInfo, DBType);
            Data.DataCommandResult Result = new Data.DataCommandResult();
            foreach (SqlParameter Param in CmmLocal.Parameters)
            {
                if (Param.Direction != ParameterDirection.Input)
                {
                    Result.AddResult(Param.ParameterName, Param.Value);
                }
            }
            return Result;
        }

        /// <summary>/This method can be used to execute Non Query commands on the server.</summary>
        ///<param name="cmm">The Command to Execute.</param> 
        ///<param name="DBType">The Database to connect.</param>
        ///<returns>Returns the executed command.</returns>
        public Data.DataCommandResult ExecCMM(Data.DataCommand cmm, Data.DBConnectionType DBType)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            this.ExecCMM(CmmLocal, DBType);
            Data.DataCommandResult Result = new Data.DataCommandResult();
            foreach (SqlParameter Param in CmmLocal.Parameters)
            {
                if (Param.Direction != ParameterDirection.Input)
                {
                    Result.AddResult(Param.ParameterName, Param.Value);
                }
            }
            return Result;
        }

        /// <summary>This method fills the Smart Data Adapter</summary>
        /// <param name="SmartDA">The Smart Data Adapter</param>
        public Data.SmartDataAdapter FillDA(ref Data.SmartDataAdapter SmartDA)
        {
            SmartDA.Fetch();
            return SmartDA;
        }

        /// <summary>Fills a Dataset with the command. Can be used to fetch multiple results from the server.</summary>
        /// <param name="cmm">The command to execute</param>
        /// <param name="UserInfo"> The User Info</param>
        /// <param name="DBType">The Database Connection to use.</param>
        public DataSet FillDS(Data.DataCommand cmm, Security.IUser UserInfo, Data.DBConnectionType DBType)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            DataSet Ds = this.FillDS(CmmLocal, UserInfo, DBType);
            Ds.RemotingFormat = SerializationFormat.Binary;
            return Ds;
        }

        /// <summary>Fills a Dataset with the command. Can be used to fetch multiple results from the server. for API</summary>
        /// <param name="cmm">The command to execute</param>
        /// <param name="DBType">The Database Connection to use.</param>
        public DataSet FillDS(Data.DataCommand cmm,  Data.DBConnectionType DBType)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            DataSet Ds = this.FillDS(CmmLocal,DBType);
            Ds.RemotingFormat = SerializationFormat.Binary;
            return Ds;
        }

        internal DataSet FillDS(SqlCommand cmm, Security.IUser UserInfo, Data.DBConnectionType DBType)
        {
            try
            {
                using (SqlConnection cn = DataAccess.GetCn(DBType))
                {
                    cmm.Connection = cn;
                    SqlDataAdapter da = new SqlDataAdapter(cmm);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
               throw new Data.DataAccessException(cmm.CommandText, DBType, ex);
            }
            return null;
        }

        internal DataSet FillDS(SqlCommand cmm, Data.DBConnectionType DBType)
        {
            try
            {
                using (SqlConnection cn = DataAccess.GetCn(DBType))
                {
                    cmm.Connection = cn;
                    SqlDataAdapter da = new SqlDataAdapter(cmm);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw new Data.DataAccessException(cmm.CommandText, DBType, ex);
            }
            return null;
        }
        /// <summary>Fills a Datatable and returns it</summary>
        /// <param name="cmm">The Command to execute</param>
        /// <param name="UserInfo">The User Info</param>
        /// <param name="DBType">The Database Connection to use</param>
        public System.Data.DataTable FillDt(Data.DataCommand cmm, Security.IUser UserInfo, Data.DBConnectionType DBType)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = this.FillDtInternal(CmmLocal, UserInfo, DBType, ref dt);
            dt.RemotingFormat = SerializationFormat.Binary;
            return dt;
        }

        /// <summary>Fills a Datatable and returns it. for API</summary>
        /// <param name="cmm">The Command to execute</param> 
        /// <param name="DBType">The Database Connection to use</param>
        public System.Data.DataTable FillDt(Data.DataCommand cmm, Data.DBConnectionType DBType)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = this.FillDtInternal(CmmLocal, DBType, ref dt);
            dt.RemotingFormat = SerializationFormat.Binary;
            return dt;
        }

        /// <summary>Fills a Datatable and returns it</summary>
        /// <param name="cmm">The Command to execute</param>
        /// <param name="UserInfo">The User Info</param>
        /// <param name="DBType">The Database Connection to use</param>
        /// <param name="dt">The Datatable to be filled</param>
        public System.Data.DataTable FillDt(Data.DataCommand cmm, Security.IUser UserInfo, Data.DBConnectionType DBType, ref System.Data.DataTable dt)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            dt = this.FillDtInternal(CmmLocal, UserInfo, DBType, ref dt);
            dt.RemotingFormat = SerializationFormat.Binary;
            return dt;
        }

        /// <summary>Fills a Datatable and returns it</summary>
        /// <param name="cmm">The Command to execute</param> 
        /// <param name="DBType">The Database Connection to use</param>
        /// <param name="dt">The Datatable to be filled</param>
        public System.Data.DataTable FillDt(Data.DataCommand cmm, Data.DBConnectionType DBType, ref System.Data.DataTable dt)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            dt = this.FillDtInternal(CmmLocal,   DBType, ref dt);
            dt.RemotingFormat = SerializationFormat.Binary;
            return dt;
        }

        internal System.Data.DataTable FillDtInternal(SqlCommand cmm, Security.IUser UserInfo, Data.DBConnectionType DBType, ref System.Data.DataTable dt)
        {
            try
            {
                using (SqlConnection cn = DataAccess.GetCn(DBType))
                {
                    cmm.Connection = cn;
                    SqlDataAdapter da = new SqlDataAdapter(cmm);
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Data.DataAccessException(cmm.CommandText, DBType, ex);
            }
        }

        internal System.Data.DataTable FillDtInternal(SqlCommand cmm,  Data.DBConnectionType DBType, ref System.Data.DataTable dt)
        {
            try
            {
                using (SqlConnection cn = DataAccess.GetCn(DBType))
                {
                    cmm.Connection = cn;
                    SqlDataAdapter da = new SqlDataAdapter(cmm);
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Data.DataAccessException(cmm.CommandText, DBType, ex);
            }
        }

        /// <summary>Executes a Scalar command and returns the result</summary>
        /// <param name="cmm">The Command to Execute</param>
        /// <param name="UserInfo">The User Info</param>
        /// <param name="DBType">The Database Connection to use</param>
        public object ExecuteScalar(Data.DataCommand cmm, Security.IUser UserInfo, Data.DBConnectionType DBType)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            return this.ExecuteScalar(CmmLocal, UserInfo, DBType);
        }

        /// <summary>Executes a Scalar command and returns the result. for API</summary>
        /// <param name="cmm">The Command to Execute</param> 
        /// <param name="DBType">The Database Connection to use</param>
        public object ExecuteScalar(Data.DataCommand cmm, Data.DBConnectionType DBType)
        {
            SqlCommand CmmLocal = Data.DataCommand.GetSQLCommand(cmm);
            return this.ExecuteScalar(CmmLocal,   DBType);
        }

        internal object ExecuteScalar(SqlCommand cmm, Security.IUser UserInfo, Data.DBConnectionType DBType)
        {
            object obj;
            try
            {
                using (SqlConnection cn = DataAccess.GetCn(DBType))
                {
                    cn.Open();
                    cmm.Connection = cn;
                    obj = cmm.ExecuteScalar();
                    cn.Close();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw new Data.DataAccessException(cmm.CommandText, DBType, ex);
            }
        }

        internal object ExecuteScalar(SqlCommand cmm,  Data.DBConnectionType DBType)
        {
            object obj;
            try
            {
                using (SqlConnection cn = DataAccess.GetCn(DBType))
                {
                    cn.Open();
                    cmm.Connection = cn;
                    obj = cmm.ExecuteScalar();
                    cn.Close();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw new Data.DataAccessException(cmm.CommandText, DBType, ex);
            }
        }

        #endregion

        #region Helper Functions

        internal static SqlConnection GetCn(Data.DBConnectionType DBType)
        {
            SqlConnection cn = new SqlConnection();
            switch (DBType)
            {
                case DBConnectionType.CompanyDB:
                    cn.ConnectionString = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("CompanyDB").ToString());
                    break;

                case DBConnectionType.CompanyDMSDB:
                    cn.ConnectionString = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("CompanyDMSDB").ToString());
                    break;

                case DBConnectionType.MainDB:
                    cn.ConnectionString = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("MainDB").ToString());
                    break;

                case DBConnectionType.OLTPDB:
                    cn.ConnectionString = Security.Encryption64.DecryptFromBase64String(Utilities.AppConfig.Item("OLTPDB").ToString());
                    break;

                default:
                    break;
            }

            return cn;
        }

        internal static string GetDBName(Security.IUser UserInfo, Data.DBConnectionType DBType)
        {
            SqlConnection cn = GetCn(DBType);
            return cn.Database;
        }

        #endregion
    }
}
