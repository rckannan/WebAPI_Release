using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MSLA.Server.Entity
{
    /// <summary>Event Args used by EntityUnitEventDelegate</summary>
    public class EntityUnitEventArgs
        : EventArgs
    {
        /// <summary>Gets the Parameter collection relating to the executable command</summary>
        public readonly SqlParameterCollection Parameters;
        /// <summary>Database Schema</summary>
        public string Schema;
        /// <summary>Database Table Name</summary>
        public string TableName;
        /// <summary>Current Row of Tran Table. If control table is used, this would return null</summary>
        public DataRow CurrentRow;

        internal EntityUnitEventArgs(SqlParameterCollection paramColl)
        {
            this.Parameters = paramColl;
        }

    }

    /// <summary>Used by the Interface IEntity Unit. Provided as Args when a field mapping fails.</summary>
    public class FieldMapEventArgs
        : EventArgs
    {
        /// <summary>SQL Server Schema of the object being mapped</summary>
        public string Schema;
        /// <summary>SQL Server Table being mapped</summary>
        public string TableName;
        /// <summary>Field/column for which mapping failed. Primarily, this field was not found in the BO. However, this column was available in the table.</summary>
        public string FieldName;
        /// <summary>The Current Row being processed for the Tran Table.</summary>
        public DataRow CurrentRow;
        /// <summary>The value that is required to be returned. This would be mapped to the Parameter Value for insert/update to Table in database.
        /// If this value is set to null/nothing, Enitiy Unit would raise an exception. This result value is compulsory.
        /// </summary>
        public object ResultValue;
        /// <summary>Return True if this field is handled and the error is to be ignored</summary>
        public bool Handled;

    }


    /// <summary>Decorate the property with this attribute to save Time along with the date.</summary>
    [System.AttributeUsage(AttributeTargets.Property)]
    public class DateWithTimeAttribute
        : Attribute
    {

    }
}