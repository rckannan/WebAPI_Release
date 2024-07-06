using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MSLA.Server.Entity
{
    internal class ActionScript
    {
        public enum enTableType
        {
            DocumentControl = 0,
            TranTable = 1,
            Master = 2
        }

        private List<ActionParam> _AddUpdateParams = new List<ActionParam>();
        private List<ActionParam> _FetchParams = new List<ActionParam>();
        private List<ActionParam> _DeleteParams = new List<ActionParam>();

        public string TableName { get; set; }

        public string AddUpdateScript { get; set; }
        public List<ActionParam> AddUpdateParams
        {
            get { return _AddUpdateParams; }
        }

        public string FetchScript { get; set; }
        public List<ActionParam> FetchParams
        {
            get { return _FetchParams; }
        }

        public string DeleteScript { get; set; }
        public List<ActionParam> DeleteParams
        {
            get { return _DeleteParams; }
        }

        public enTableType TableType { get; set; }
    }

    internal static class SQLParamCopy
    {
        public static SqlParameter Copy(SqlParameter param)
        {
            return new SqlParameter();
        }
    }

    internal class ActionParam
    {
        public string ParameterName { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public int Size { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public ParameterDirection Direction { get; set; }

        public SqlParameter GetSQLParameter()
        {
            return new SqlParameter() { ParameterName = ParameterName, SqlDbType = SqlDbType, Size = Size, Precision = Precision, Scale = Scale, Direction = Direction };
        }
    }
}
