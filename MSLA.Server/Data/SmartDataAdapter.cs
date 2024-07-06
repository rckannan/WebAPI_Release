using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Data
{
    /// <summary>The Smart Data Adapter Class. Can be used to fetch multiple results from the server. </summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class SmartDataAdapter
        : ISerializable, IDisposable
    {
        private Security.IUser _UserInfo;
        private DBConnectionType _DBType;
        private Dictionary<string, SDAInfo> _SQLCommands = new Dictionary<string, SDAInfo>();

        /// <summary>Constructor</summary>
        /// <param name="UserInfo">user Info</param>
        /// <param name="DBtype">Connection Type</param>
        public SmartDataAdapter(Security.IUser UserInfo, DBConnectionType DBtype)
        {
            _UserInfo = UserInfo;
            _DBType = DBtype;
        }

        ///<summary>Adds a Table and Command to the collection.</summary>
        ///<param name="TableName">Provide a unique Table Name.</param>
        ///<param name="cmm">The Command required to fill the Table.</param>
        public void AddCommand(String TableName, SqlCommand cmm)
        {
            if (_SQLCommands.ContainsKey(TableName))
            {
                throw new Exception(TableName + " already in the collection of SmartDataAdapter. Duplicates Prohibited.");
            }
            else
            {
                _SQLCommands.Add(TableName, new SDAInfo(null, cmm));
            }
        }

        ///<summary>Adds a Table and Command to the collection.</summary>
        ///<param name="Table">Provide an instance of strongly typed datatable. Ensure that the variable is reset to the table after you call the fill method.</param>
        ///<param name="cmm">The Command required to fill the Table.</param>
        public void AddCommand(System.Data.DataTable Table, SqlCommand cmm)
        {
            if (_SQLCommands.ContainsKey(Table.TableName))
            {
                throw new Exception(Table.TableName + " already in the collection of SmartDataAdapter. Duplicates Prohibited.");
            }
            else
            {
                _SQLCommands.Add(Table.TableName, new SDAInfo(Table, cmm));
            }
        }

        internal void Fetch()
        {
            SqlDataAdapter da = new SqlDataAdapter();
            using (SqlConnection cn = Data.DataAccess.GetCn( _DBType))
            {
                cn.Open();
                foreach (KeyValuePair<String, SDAInfo> Item in _SQLCommands)
                {
                    Item.Value.SQLCmm.Connection = cn;
                    da.SelectCommand = Item.Value.SQLCmm;
                    try
                    {
                        if (Item.Value.Table != null)
                        {
                            da.Fill(Item.Value.Table);
                        }
                        else
                        {
                            System.Data.DataTable dt = new System.Data.DataTable(Item.Key);
                            da.Fill(dt);
                            Item.Value.Table = dt;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Data.DataAccessException(Item.Value.SQLCmm.CommandText, _DBType, ex);
                    }
                }
                cn.Close();
            }
        }

        internal void Merge(SmartDataAdapter SourceSDA)
        {
            System.Data.DataTable TargetDt = null;
            foreach (KeyValuePair<String, SDAInfo> SourceItem in SourceSDA._SQLCommands)
            {
                TargetDt = this._SQLCommands[SourceItem.Key].Table;
                if (TargetDt == null)
                { TargetDt = new System.Data.DataTable(); }
                Utilities.TableHelper.CopyTableData(SourceItem.Value.Table, TargetDt);
                this._SQLCommands[SourceItem.Key].Table = TargetDt;
            }
        }

        internal SmartDataAdapter Copy()
        {
            SmartDataAdapter sda = new SmartDataAdapter(this._UserInfo, this._DBType);
            foreach (KeyValuePair<String, SDAInfo> Item in this._SQLCommands)
            {
                if (Item.Value.Table == null)
                { sda.AddCommand(Item.Key, Item.Value.SQLCmm); }
                else
                { sda.AddCommand(Item.Value.Table, Item.Value.SQLCmm); }
            }
            return sda;
        }

        /// <summary>Call this only when you have supplied tablenames and not table instances. </summary>
        /// <param name="TableName">Mention the Table Name.</param>
        public System.Data.DataTable GetTable(String TableName)
        {
            return _SQLCommands[TableName].Table;
        }

        internal void DisposeTables()
        {
            foreach (KeyValuePair<String, SDAInfo> Item in _SQLCommands)
            {
                if (Item.Value.Table != null)
                { Item.Value.Table.Dispose(); };
            }
        }


        #region "SDA Info Class"
        private class SDAInfo
        {
            public System.Data.DataTable Table;
            public SqlCommand SQLCmm;

            public SDAInfo(System.Data.DataTable dt, SqlCommand Cmm)
            {
                this.Table = dt;
                this.SQLCmm = Cmm;
            }
        }
        #endregion

        #region "ISerializable Implementation"

        /// <summary>Deserialization Constructor</summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected SmartDataAdapter(SerializationInfo info, StreamingContext context)
        {
            _UserInfo = (Security.IUser)info.GetValue("_UserInfo", typeof(Security.IUser));
            _DBType = (DBConnectionType)info.GetValue("_DBType", typeof(DBConnectionType));
            int Count;
            Count = (System.Int32)info.GetValue("CommandCount", typeof(System.Int32));
            System.Data.DataTable dt;
            DataCommand DataCmm;
            SqlCommand SQLCmm;
            String TableName;
            for (int i = 0; i < Count; i++)
            {
                TableName = (System.String)info.GetValue("TableName_" + i.ToString(), typeof(System.String));
                dt = (System.Data.DataTable)info.GetValue("DataTable_" + i.ToString(), typeof(DataTable));
                if (dt != null)
                { dt.RemotingFormat = SerializationFormat.Xml; }
                DataCmm = (Data.DataCommand)info.GetValue("SQLCommand_" + i.ToString(), typeof(Data.DataCommand));
                SQLCmm = Data.DataCommand.GetSQLCommand(DataCmm);
                _SQLCommands.Add(TableName, new SDAInfo(dt, SQLCmm));
            }
        }

        /// <summary>Serialization Implementation</summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_UserInfo", _UserInfo, typeof(Security.IUser));
            info.AddValue("_DBType", _DBType, typeof(DBConnectionType));
            int i = 0;
            foreach (KeyValuePair<String, SDAInfo> Item in _SQLCommands)
            {
                if (Item.Value.Table != null)
                { Item.Value.Table.RemotingFormat = SerializationFormat.Binary; }
                info.AddValue("TableName_" + i.ToString(), Item.Key, typeof(System.String));
                info.AddValue("DataTable_" + i.ToString(), Item.Value.Table, typeof(DataTable));
                info.AddValue("SQLCommand_" + i.ToString(), DataCommand.GetDataCommand(Item.Value.SQLCmm), typeof(Data.DataCommand));
                i++;
            }
            info.AddValue("CommandCount", i, typeof(System.Int32));
        }

        #endregion

        #region "IDisposable Interface Implementation"
        private bool disposedValue = false;

        /// <summary>Dispose</summary>
        /// <param name="disposing">boolean</param>
        protected void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    _UserInfo = null;
                    _SQLCommands.Clear();
                }
            }
            this.disposedValue = true;
        }

        /// <summary>Dispose</summary>
        public void Dispose()
        {
            //Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
