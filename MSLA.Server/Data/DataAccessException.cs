using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MSLA.Server.Data
{
    /// <summary>This exception is raised whenever a command is executed and it fails</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class DataAccessException
        : Exception, Exceptions.ICustomException
    {
        DBConnectionType _DbType;
        /// <summary>Constructor</summary>
        /// <param name="mySQL">The SQL Command Text</param>
        /// <param name="ex">The actual exception</param>
        internal DataAccessException(string mySQL, Exception ex)
            : base(mySQL, ex)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="mySQL">The SQL Command Text</param>
        /// <param name="DBType">The Type of Connection To use</param>
        /// <param name="ex">The actual exception</param>
        internal DataAccessException(string mySQL, DBConnectionType DBType, Exception ex)
            : base(mySQL, ex)
        {
            _DbType = DBType;
        }

        /// <summary>Serialixation Constructor</summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected DataAccessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        string Exceptions.ICustomException.CustomMessage
        {
            get
            {
                string Msg = string.Empty;
                Msg = "Executed SQL: " + this.Message;
                switch (_DbType)
                {
                    case DBConnectionType.CompanyDB:
                        Msg += "\r\nDatabase: CompanyDB";
                        break;
                    case DBConnectionType.CompanyDMSDB:
                        Msg += "\r\nDatabase: CompanyDMSDB";
                        break;
                    case DBConnectionType.MainDB:
                        Msg += "\r\nDatabase: MainDB";
                        break;
                    case DBConnectionType.OLTPDB:
                        Msg += "\r\nDataBase: OLTPDB";
                        break;
                    default:
                        Msg += "\r\nDatabase: Failed to Resolve";
                        break;
                }
                if (this.InnerException != null)
                {
                    Msg += "\r\nError: " + this.InnerException.Message;
                }
                return Msg;
            }
        }
    }
}
