using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Entity
{ /// <summary>Class Used to manage Database Entities</summary>
    public partial class EntityManager
    {
        private static Dictionary<string, ActionScript> CachedTables = null;
        private static MSLA.Server.Utilities.XMLReader LoadedTemplate = null;
        private static MSLA.Server.Utilities.XMLReader ExcludedFields = null;


        static EntityManager()
        {
            CachedTables = new Dictionary<string, ActionScript>();

            Assembly a = Assembly.GetExecutingAssembly();
            XmlDocument Doc = new XmlDocument();
            Doc.Load(a.GetManifestResourceStream("MSLA.Server.Entity.ActionScriptTemplate.xml"));
            LoadedTemplate = new MSLA.Server.Utilities.XMLReader(Doc);

            XmlDocument exFieldList = new XmlDocument();
            exFieldList.Load(a.GetManifestResourceStream("MSLA.Server.Entity.ActionScriptFieldsExcluded.xml"));
            ExcludedFields = new MSLA.Server.Utilities.XMLReader(exFieldList);

        }

        /// <summary>Gets the script required to Add/Update Document control</summary>
        /// <param name="UserInfo">User Info</param>
        /// <param name="schema">Database schema</param>
        /// <param name="table">Database table</param>
        /// <param name="primaryKey">Primary key of the table</param>
        /// <param name="sequenceTable">Sequence table for generating primary key</param>
        public static SqlCommand Get_DocControlAddUpdate(Security.IUser UserInfo, string schema, string table, string primaryKey, string sequenceTable, Data.DBConnectionType DBType)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;

            // First lookup into the cache and generate if not exists
            if (!CachedTables.ContainsKey(schema + "." + table))
            {
                GenerateDocActionScripts(UserInfo, schema, table, primaryKey, sequenceTable, DBType);
            }

            // Object now exists in cache. hence return from cache
            cmm.CommandText = CachedTables[schema + "." + table].AddUpdateScript.Replace("?Database", Server.Data.DataAccess.GetDBName(UserInfo, DBType));
            foreach (ActionParam param in CachedTables[schema + "." + table].AddUpdateParams)
            {
                cmm.Parameters.Add(param.GetSQLParameter());
            }
            return cmm;

        }

        /// <summary>Returns a prepared SQL Command to Add/Update Maters</summary>
        /// <param name="UserInfo">UserInfo</param>
        /// <param name="schema">Database Schema</param>
        /// <param name="table">Database Table</param>
        /// <param name="primaryKey">primary key of the table</param>
        /// <param name="DBType">Connection Type (Company, Main, DMS)</param>
        public static SqlCommand Get_MasterAddUpdate(Security.IUser UserInfo, string schema, string table, string primaryKey, Data.DBConnectionType DBType)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;

            // First lookup into the cache and generate if not exists
            if (!CachedTables.ContainsKey(schema + "." + table))
            {
                GenerateMasterActionScripts(UserInfo, schema, table, primaryKey, DBType);
            }

            // Object now exists in cache. hence return from cache
            cmm.CommandText = CachedTables[schema + "." + table].AddUpdateScript.Replace("?Database", MSLA.Server.Data.DataAccess.GetDBName(UserInfo, DBType));
            foreach (ActionParam param in CachedTables[schema + "." + table].AddUpdateParams)
            {
                cmm.Parameters.Add(param.GetSQLParameter());
            }
            return cmm;
        }

        /// <summary>Gets the Script required to Add records in Tran Table</summary>
        /// <param name="UserInfo">User Info</param>
        /// <param name="schema">Database schema</param>
        /// <param name="table">Database Table</param>
        /// <param name="foreignKey">Foreign Key (referenced key of the control table)</param>
        /// <param name="DBType">Connection Type (Company, Main, DMS)</param>
        /// <returns></returns>
        public static SqlCommand Get_TranAdd(Security.IUser UserInfo, string schema, string table, string foreignKey, Data.DBConnectionType DBType)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;

            // First lookup into the cache and generate if not exists
            if (!CachedTables.ContainsKey(schema + "." + table))
            {
                GenerateTranActionScripts(UserInfo, schema, table, foreignKey, DBType);
            }

            // Object now exists in cache. hence return from cache
            cmm.CommandText = CachedTables[schema + "." + table].AddUpdateScript.Replace("?Database", Server.Data.DataAccess.GetDBName(UserInfo, DBType));
            foreach (ActionParam param in CachedTables[schema + "." + table].AddUpdateParams)
            {
                cmm.Parameters.Add(param.GetSQLParameter());
            }
            return cmm;
        }

        /// <summary>Gets the script required to Fetch Document control</summary>
        /// <param name="UserInfo">User Info</param>
        /// <param name="schema">Database schema</param>
        /// <param name="table">Database table</param>
        /// <param name="primaryKey">Primary key of the table</param>
        /// <param name="sequenceTable">Sequence table for generating primary key</param>
        public static SqlCommand Get_DocControlFetch(Security.IUser UserInfo, string schema, string table, string primaryKey, string sequenceTable, Data.DBConnectionType DBType)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;

            // First lookup into the cache and generate if not exists
            if (!CachedTables.ContainsKey(schema + "." + table))
            {
                GenerateDocActionScripts(UserInfo, schema, table, primaryKey, sequenceTable, DBType);
            }

            // Object now exists in cache. hence return from cache
            cmm.CommandText = CachedTables[schema + "." + table].FetchScript.Replace("?Database", Server.Data.DataAccess.GetDBName(UserInfo, DBType));
            foreach (ActionParam param in CachedTables[schema + "." + table].FetchParams)
            {
                cmm.Parameters.Add(param.GetSQLParameter());
            }
            return cmm;

        }

        /// <summary>Returns a prepared SQL Command to Fetch Masters</summary>
        /// <param name="UserInfo">UserInfo</param>
        /// <param name="schema">Database Schema</param>
        /// <param name="table">Database Table</param>
        /// <param name="primaryKey">primary key of the table</param>
        /// <param name="DBType">Connection Type (Company, Main, DMS)</param>
        public static SqlCommand Get_MasterFetch(Security.IUser UserInfo, string schema, string table, string primaryKey, Data.DBConnectionType DBType)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;

            // First lookup into the cache and generate if not exists
            if (!CachedTables.ContainsKey(schema + "." + table))
            {
                GenerateMasterActionScripts(UserInfo, schema, table, primaryKey, DBType);
            }

            // Object now exists in cache. hence return from cache
            cmm.CommandText = CachedTables[schema + "." + table].FetchScript.Replace("?Database", Server.Data.DataAccess.GetDBName(UserInfo, DBType));
            foreach (ActionParam param in CachedTables[schema + "." + table].FetchParams)
            {
                cmm.Parameters.Add(param.GetSQLParameter());
            }
            return cmm;
        }

        /// <summary>Gets the Script required to fetch Tran Table</summary>
        /// <param name="UserInfo">User Info</param>
        /// <param name="schema">Database schema</param>
        /// <param name="table">Database Table</param>
        /// <param name="foreignKey">Foreign Key (referenced key of the control table)</param>
        /// <param name="DBType">Connection Type (Company, Main, DMS)</param>
        /// <returns></returns>
        public static SqlCommand Get_TranFetch(Security.IUser UserInfo, string schema, string table, string foreignKey, Data.DBConnectionType DBType)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;

            // First lookup into the cache and generate if not exists
            if (!CachedTables.ContainsKey(schema + "." + table))
            {
                GenerateTranActionScripts(UserInfo, schema, table, foreignKey, DBType);
            }

            // Object now exists in cache. hence return from cache
            cmm.CommandText = CachedTables[schema + "." + table].FetchScript.Replace("?Database", Server.Data.DataAccess.GetDBName(UserInfo, DBType));
            foreach (ActionParam param in CachedTables[schema + "." + table].FetchParams)
            {
                cmm.Parameters.Add(param.GetSQLParameter());
            }
            return cmm;
        }


        /// <summary>Gets the Script required to delete Tran Table</summary>
        /// <param name="UserInfo">User Info</param>
        /// <param name="schema">Database schema</param>
        /// <param name="table">Database Table</param>
        /// <param name="foreignKey">Foreign Key (referenced key of the control table)</param>
        /// <param name="DBType">Connection Type (Company, Main, DMS)</param>
        /// <returns></returns>
        public static SqlCommand Get_TranDelete(Security.IUser UserInfo, string schema, string table, string foreignKey, Data.DBConnectionType DBType)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;

            // First lookup into the cache and generate if not exists
            if (!CachedTables.ContainsKey(schema + "." + table))
            {
                GenerateTranActionScripts(UserInfo, schema, table, foreignKey, DBType);
            }

            if (CachedTables[schema + "." + table].TableType != ActionScript.enTableType.TranTable)
            {
                throw new Exception("'" + schema + "." + table + "' is not a tran table. Delete scripts can be requested only for Tran tables.");
            }

            // Object now exists in cache. hence return from cache
            cmm.CommandText = CachedTables[schema + "." + table].DeleteScript.Replace("?Database", Server.Data.DataAccess.GetDBName(UserInfo, DBType));
            foreach (ActionParam param in CachedTables[schema + "." + table].DeleteParams)
            {
                cmm.Parameters.Add(param.GetSQLParameter());
            }
            return cmm;
        }

        /// <summary>Fill a Master BO specifying the command with command text and parameters. </summary>
        /// <param name="fetchObj">Master BO</param>
        /// <param name="schema">Database Schema</param>
        /// <param name="tableName"> Database Table Name</param>
        /// <param name="primaryKey">Primary Key Field</param>
        /// <param name="DocMaster_ID">ID of the master item</param>
        public static void FillMaster(IEntityUnit fetchObj, string schema, string tableName, string primaryKey, long DocMaster_ID)
        {
            System.Data.SqlClient.SqlCommand cmm = MSLA.Server.Entity.EntityManager.Get_MasterFetch(fetchObj.UserInfo, schema, tableName, primaryKey, fetchObj.DBType);
            cmm.Parameters[primaryKey.Replace("fld", "@")].Value = DocMaster_ID;

            FillMaster(fetchObj, schema, tableName, primaryKey, cmm);
        }

        /// <summary>Fill a Master BO specifying the command with command text and parameters. </summary>
        /// <param name="fetchObj">Master BO</param>
        /// <param name="schema">Database Schema</param>
        /// <param name="tableName"> Database Table Name</param>
        /// <param name="primaryKey">Primary Key Field</param>
        /// <param name="cmm">Command object that should return ony one row</param>
        public static void FillMaster(IEntityUnit fetchObj, string schema, string tableName, string primaryKey, SqlCommand cmm)
        {
            DataTable dtResult = new DataTable();
            dtResult = Data.DataConnect.FillDt(dtResult, cmm, fetchObj.UserInfo, fetchObj.DBType);
            if (dtResult.Rows.Count == 1)
            {
                foreach (DataColumn dtCol in dtResult.Columns)
                {
                    if (dtCol.ColumnName == primaryKey)
                    { Utilities.ReflectionHelper.CallMethod(fetchObj, "SetDocMaster_ID", dtResult.Rows[0][dtCol]); }
                    else
                    {
                        bool valueSet = Utilities.ReflectionHelper.SetPropertyValueSafe(fetchObj, dtCol.ColumnName, dtResult.Rows[0][dtCol]);
                        if (!valueSet)
                        {
                            FieldMapEventArgs args = new FieldMapEventArgs()
                            {
                                Schema = schema,
                                TableName = tableName,
                                FieldName = dtCol.ColumnName,
                                ResultValue = dtResult.Rows[0][dtCol]
                            };
                            fetchObj.FieldMapFetchFailed(args);
                            if (!args.Handled)
                            { throw new Exception("Failed to set the property value for Field '" + dtCol.ColumnName + "'.\nPlease provide required property in BO or override OnFieldMapFetchFailed to handle exception cases"); }
                        }
                    }
                }
            }
            else
            {
                if (dtResult.Rows.Count > 1)
                { throw new Exception("Multiple rows returned by command. Failed to fill Business Object"); }
                else
                { throw new Exception("Command did not return any rows from the database. Failed to fill Business Object"); }
            }
        }

        /// <summary>Fill a Document BO specifying the command with command text and parameters. </summary>
        /// <param name="fetchObj">Document BO</param>
        /// <param name="docObject">DocObjectInfo available for each Document that inherits from DocumentBase</param>
        /// <param name="Doc_ID">Usually the Voucher ID</param>
        public static void FillDocumentControl(IEntityUnit fetchObj, MSLA.Server.Base.DocObject docObject, string Doc_ID)
        {
            string schema = docObject.TableName.Split('.')[0];
            string tableName = docObject.TableName.Split('.')[1];

            FillDocumentControl(fetchObj, schema, tableName, docObject.VoucherField, docObject.SequenceTable, Doc_ID);
        }

        /// <summary>Fill a Document BO specifying the command with command text and parameters. </summary>
        /// <param name="fetchObj">Document BO</param>
        /// <param name="schema">Database Schema</param>
        /// <param name="tableName"> Database Table Name</param>
        /// <param name="primaryKey">Primary Key Field</param>
        /// <param name="sequenceTable">The Sequence Table used by this Document</param>
        /// <param name="Doc_ID">Usually the Voucher ID</param>
        public static void FillDocumentControl(IEntityUnit fetchObj, string schema, string tableName, string primaryKey, string sequenceTable, string Doc_ID)
        {
            System.Data.SqlClient.SqlCommand cmm = MSLA.Server.Entity.EntityManager.Get_DocControlFetch(fetchObj.UserInfo, schema, tableName, primaryKey, sequenceTable, fetchObj.DBType);
            cmm.Parameters[primaryKey.Replace("fld", "@")].Value = Doc_ID;

            FillDocumentControl(fetchObj, schema, tableName, primaryKey, cmm);
        }

        /// <summary>Fill a Document BO specifying the command with command text and parameters. </summary>
        /// <param name="fetchObj">Document BO</param>
        /// <param name="schema">Database Schema</param>
        /// <param name="tableName"> Database Table Name</param>
        /// <param name="primaryKey">Primary Key Field</param>
        /// <param name="cmm">Command object that should return ony one row</param>
        public static void FillDocumentControl(IEntityUnit fetchObj, string schema, string tableName, string primaryKey, SqlCommand cmm)
        {
            DataTable dtResult = new DataTable();
            dtResult = Data.DataConnect.FillDt(dtResult, cmm, fetchObj.UserInfo, fetchObj.DBType);
            if (dtResult.Rows.Count == 1)
            {
                foreach (DataColumn dtCol in dtResult.Columns)
                {
                    if (dtCol.ColumnName == primaryKey)
                    { Utilities.ReflectionHelper.CallMethod(fetchObj, "SetVoucher_ID", dtResult.Rows[0][dtCol]); }
                    else if (dtCol.ColumnName == "fldCompany_ID")
                    { Utilities.ReflectionHelper.CallMethod(fetchObj, "SetCompany_ID", dtResult.Rows[0][dtCol]); }
                    else if (dtCol.ColumnName == "fldBranch_ID")
                    { Utilities.ReflectionHelper.CallMethod(fetchObj, "SetBranch_ID", dtResult.Rows[0][dtCol]); }
                    else if (dtCol.ColumnName == "fldMonth")
                    { Utilities.ReflectionHelper.CallMethod(fetchObj, "SetMonth", dtResult.Rows[0][dtCol]); }
                    else if (dtCol.ColumnName == "fldYear")
                    { Utilities.ReflectionHelper.CallMethod(fetchObj, "SetYear", dtResult.Rows[0][dtCol]); }
                    //else if (dtCol.ColumnName == "fldStatus")
                    //{ Utilities.ReflectionHelper.CallMethod(fetchObj, "SetCurrentStatus", ((MSLA.Server.Base.EnDocStatus)Enum.ToObject(typeof(MSLA.Server.Base.EnDocStatus), dtResult.Rows[0][dtCol]))); }
                    else
                    {
                        bool valueSet = Utilities.ReflectionHelper.SetPropertyValueSafe(fetchObj, dtCol.ColumnName, dtResult.Rows[0][dtCol]);
                        if (!valueSet)
                        {
                            FieldMapEventArgs args = new FieldMapEventArgs()
                            {
                                Schema = schema,
                                TableName = tableName,
                                FieldName = dtCol.ColumnName,
                                ResultValue = dtResult.Rows[0][dtCol]
                            };
                            fetchObj.FieldMapFetchFailed(args);
                            if (!args.Handled)
                            { throw new Exception("Failed to set the property value for Field '" + dtCol.ColumnName + "'.\nPlease provide required property in BO or override OnFieldMapFetchFailed to handle exception cases"); }
                        }
                    }
                }
            }
            else
            {
                if (dtResult.Rows.Count > 1)
                { throw new Exception("Multiple rows returned by command. Failed to fill Business Object"); }
                else
                { throw new Exception("Command did not return any rows from the database. Failed to fill Business Object"); }
            }
        }
    }
}
