using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MSLA.Server.Data
{
    /// <summary>The serializable Data Command</summary>
    //[System.Runtime.Serialization.DataContract]
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class DataCommand
    {
        private EnDataCommandType _CommandType;
        private string _CommandText;
        private List<DataParameter> _Parameters;
        private int _CommandTimeout = 30;
        private DBConnectionType _ConnectionType;


        /// <summary>Constructor</summary>
        public DataCommand()
        {
            _Parameters = new List<DataParameter>();
        }


        #region "Public Properties"
        /// <summary>The Command Type</summary>
        public EnDataCommandType CommandType
        {
            get { return _CommandType; }
            set { _CommandType = value; }
        }

        /// <summary>Command Text</summary>
        public String CommandText
        {
            get { return _CommandText; }
            set { _CommandText = value; }
        }

        /// <summary>Command Timeout. Default is 30 secs</summary>
        public int CommandTimeout
        {
            get { return _CommandTimeout; }
            set { _CommandTimeout = value; }
        }

        /// <summary>Command Parameter Collection</summary>
        public List<DataParameter> Parameters
        {
            get { return _Parameters; }
            set { _Parameters = value; }
        }


        /// <summary>Database Connection Type</summary>
        public DBConnectionType ConnectionType
        {
            get { return _ConnectionType; }
            set { _ConnectionType = value; }
        }

        #endregion

        #region "Shared Methods"

        /// <summary>Gets the SQLClient.SQLCommand from the DataCommand</summary>
        /// <param name="DataCmm">The DataCommand</param>
        public static SqlCommand GetSQLCommand(DataCommand DataCmm)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = (CommandType)DataCmm.CommandType;
            cmm.CommandText = DataCmm.CommandText;
            cmm.CommandTimeout = DataCmm.CommandTimeout;

            SqlParameter Param;
            foreach (DataParameter DbParam in DataCmm.Parameters)
            {
                Param = new SqlParameter();
                Param.ParameterName = DbParam.ParameterName;
                Param.SqlDbType = (SqlDbType)DbParam.DBType;
                Param.Size = DbParam.Size;
                Param.Value = DbParam.Value;
                Param.Direction = (ParameterDirection)DbParam.Direction;

                Param.Precision = DbParam.Precision;
                Param.Scale = DbParam.Scale;

                cmm.Parameters.Add(Param);
            }
            return cmm;
        }

        /// <summary>Gets the DataCommand from the SQLClient.SQLCommand</summary>
        /// <param name="SQLCmm">The SQL Command</param>
        public static DataCommand GetDataCommand(SqlCommand SQLCmm)
        {
            DataCommand DataCmm = new DataCommand();
            DataCmm.CommandType = (EnDataCommandType)SQLCmm.CommandType;
            DataCmm.CommandText = SQLCmm.CommandText;
            DataCmm.CommandTimeout = SQLCmm.CommandTimeout;

            DataParameter DbParam;
            foreach (SqlParameter Param in SQLCmm.Parameters)
            {
                DbParam = new DataParameter();
                DbParam.ParameterName = Param.ParameterName;
                DbParam.DBType = (DataParameter.EnDataParameterType)Param.SqlDbType;
                DbParam.Size = Param.Size;
                DbParam.Value = Param.Value;
                DbParam.Direction = (DataParameter.EnParameterDirection)Param.Direction;

                DbParam.Precision = Param.Precision;
                DbParam.Scale = Param.Scale;

                DataCmm.Parameters.Add(DbParam);
            }
            return DataCmm;
        }
        #endregion

    }
}
