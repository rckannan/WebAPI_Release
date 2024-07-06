using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Entity
{
   
    /// <summary>Class Used to Save an Entity that represents a Database Table</summary>
    public class EntityUnit
    {
        /// <summary>Table Type Enum</summary>
        public enum enTableType
        {
            /// <summary>Default: Unknown type</summary>
            Unknown = -1,
            /// <summary>Document Control (e.g. BPV Control, BRV Control)</summary>
            DocumentControl = 0,
            /// <summary>Tran Table (e.g. BPV Tran, BRV Tran)</summary>
            TranTable = 1,
            /// <summary>Master Control (e.g. Account Head, Customer)</summary>
            Master = 2
        }

        #region Private Variables

        private SqlConnection _SQLConnect;
        private SqlTransaction _SQLTran;
        private enTableType _TableType = enTableType.Unknown;
        private string _Schema;
        private string _TableName = string.Empty;
        private string _PrimaryKey;
        private string _SequenceTable;
        private string _ForeignKey;

        private SqlCommand _cmmControlAddUpdate = null;
        private SqlCommand _cmmTranAdd = null;

        #endregion

        #region delegates and events

        /// <summary>Delegate to raise Event for Entity Unit</summary>
        /// <param name="sender">Entity Unit</param>
        /// <param name="e">EntityUnitEventArgs</param>
        public delegate void EntityUnitSaveDelegate(object sender, EntityUnitEventArgs e);
        /// <summary>Event raised before Command is Executed but after the entity is mapped to the command</summary>
        public event EntityUnitSaveDelegate BeforeSaveEntity;
        /// <summary>Event Raised after successful execution of the command</summary>
        public event EntityUnitSaveDelegate AfterSaveEntity;
        /// <summary>Event raised before Delete Command is Executed on Tran Table but after entity is mapped to command</summary>
        public event EntityUnitSaveDelegate BeforeDeleteEntity;
        /// <summary>Event raised after delete command is executed</summary>
        public event EntityUnitSaveDelegate AfterDeleteEntity;


        #endregion

        /// <summary>Constructor</summary>
        public EntityUnit(string schema, string tableName, enTableType tableType)
        {
            this._Schema = schema;
            this._TableName = tableName;
            this._TableType = tableType;
        }

        /// <summary>Constructor</summary>
        /// <param name="docObjectInfo">DocumentBase.DocObjectInfo</param>
        /// <param name="tableType">Table Type</param>
        public EntityUnit(Base.DocObject docObjectInfo, enTableType tableType)
            :this(docObjectInfo.TableName.Split('.')[0], docObjectInfo.TableName.Split('.')[0], tableType)
        {
            this._PrimaryKey = docObjectInfo.VoucherField;
            this._SequenceTable = docObjectInfo.SequenceTable;
        }

        #region public properties

        /// <summary>Sets the Open SQL Connection</summary>
        public SqlConnection SQLConnect
        { set { _SQLConnect = value; } }

        /// <summary>Sets the Open SQL Transaction</summary>
        public SqlTransaction SQLTran
        { set { _SQLTran = value; } }

        /// <summary>Sets the Primary Key of the Table</summary>
        public string PrimaryKey
        { set { _PrimaryKey = value; } }

        /// <summary>Sets the sequence table. Relevant only for Document Control</summary>
        public string SequenceTable
        { set { _SequenceTable = value; } }

        /// <summary>Sets the Foreign Key of the Tran Table. Ideally this is the primary key of the control table</summary>
        public string ForeignKey
        { set { _ForeignKey = value; } }

        /// <summary>Gets the Parameter collection of Executed Command</summary>
        public SqlParameterCollection Parameters
        { get { return _cmmControlAddUpdate.Parameters; } }
        #endregion

        #region public Delete/Save methods

        /// <summary>Saves Document control or Master</summary>
        /// <param name="saveObj">BO</param>
        public void SaveControl(IEntityUnit saveObj)
        {

            ValidateSelf();

            _cmmControlAddUpdate = null;

            // Create Command Object
            if (_TableType == enTableType.Master)
            {
                _cmmControlAddUpdate = EntityManager.Get_MasterAddUpdate(saveObj.UserInfo, _Schema, _TableName, _PrimaryKey, saveObj.DBType);

                // Map the parameters
                MapEntity(saveObj, _cmmControlAddUpdate);

                // Raise Before Save Event
                if (BeforeSaveEntity != null)
                { BeforeSaveEntity.Invoke(this, new EntityUnitEventArgs(_cmmControlAddUpdate.Parameters) { Schema = _Schema, TableName = _TableName }); }

                // Set Connection, Transaction and execute command
                _cmmControlAddUpdate.Connection = _SQLConnect;
                _cmmControlAddUpdate.Transaction = _SQLTran;
                _cmmControlAddUpdate.ExecuteNonQuery();

                // Raise After Save Event
                if (AfterSaveEntity != null)
                { AfterSaveEntity.Invoke(this, new EntityUnitEventArgs(_cmmControlAddUpdate.Parameters) { Schema = _Schema, TableName = _TableName }); }

            }
            else if (_TableType == enTableType.DocumentControl)
            {
                _cmmControlAddUpdate = EntityManager.Get_DocControlAddUpdate(saveObj.UserInfo, _Schema, _TableName, _PrimaryKey, _SequenceTable, saveObj.DBType);

                // Map the parameters
                MapEntity(saveObj, _cmmControlAddUpdate);

                // Raise Before Save Event
                if (BeforeSaveEntity != null)
                { BeforeSaveEntity.Invoke(this, new EntityUnitEventArgs(_cmmControlAddUpdate.Parameters) { Schema = _Schema, TableName = _TableName }); }

                // Set Connection, Transaction and execute command
                _cmmControlAddUpdate.Connection = _SQLConnect;
                _cmmControlAddUpdate.Transaction = _SQLTran;
                _cmmControlAddUpdate.ExecuteNonQuery();

                // Raise After Save Event
                if (AfterSaveEntity != null)
                { AfterSaveEntity.Invoke(this, new EntityUnitEventArgs(_cmmControlAddUpdate.Parameters) { Schema = _Schema, TableName = _TableName }); }
            
            }
            else if (_TableType == enTableType.TranTable)
            {
                throw new Exception("Call DeleteTran(), SaveTranItem() to save Tran Tables. SaveControl() method is only for control Add/Update.");
            }
        }

        /// <summary>Deletes records in a Tran Table based on the foreign key</summary>
        /// <param name="saveObj">BO</param>
        /// <param name="tranObj">Table containing tran data</param>
        public void DeleteTran(IEntityUnit saveObj, DataTable tranObj)
        {
            SqlCommand cmmDelete = null;
            cmmDelete = EntityManager.Get_TranDelete(saveObj.UserInfo, _Schema, _TableName, _ForeignKey, saveObj.DBType);

            // Map Parameters
            MapEntity(saveObj, cmmDelete);

            // Raise Before Delete Event
            if (BeforeDeleteEntity != null)
            { BeforeSaveEntity.Invoke(this, new EntityUnitEventArgs(cmmDelete.Parameters) { Schema = _Schema, TableName = _TableName }); }

            // Set Connection, Transaction and execute command
            cmmDelete.Connection = _SQLConnect;
            cmmDelete.Transaction = _SQLTran;
            cmmDelete.ExecuteNonQuery();

            // Raise After Delete Event
            if (AfterDeleteEntity != null)
            { AfterSaveEntity.Invoke(this, new EntityUnitEventArgs(cmmDelete.Parameters) { Schema = _Schema, TableName = _TableName }); }
        }

        private SqlCommand InitialiseTranSave(IEntityUnit saveObj, DataTable tranObj)
        {
            if (_cmmTranAdd == null)
            {
                
                _cmmTranAdd = EntityManager.Get_TranAdd(saveObj.UserInfo, _Schema, _TableName, _ForeignKey, saveObj.DBType);
                _cmmTranAdd.Connection = _SQLConnect;
                _cmmTranAdd.Transaction = _SQLTran;
            }
            return _cmmTranAdd;
        }

        /// <summary>Saves a line item of a Tran Table</summary>
        /// <param name="saveObj">BO</param>
        /// <param name="tranObj">Table containing Tran Data</param>
        /// <param name="currentRow">The current row in tran table to save. Use a for loop and call this method for each row.</param>
        public void SaveTranItem(IEntityUnit saveObj, DataTable tranObj, DataRow currentRow)
        {
            SqlCommand cmmAdd = InitialiseTranSave(saveObj, tranObj);

            MapEntity(saveObj, currentRow, cmmAdd);

            // Raise Before Save Event
            if (BeforeSaveEntity != null)
            { BeforeSaveEntity.Invoke(this, new EntityUnitEventArgs(cmmAdd.Parameters) { Schema = _Schema, TableName = _TableName, CurrentRow = currentRow }); }

            // execute command
            cmmAdd.ExecuteNonQuery();

            // Raise After Save Event
            if (AfterSaveEntity != null)
            { AfterSaveEntity.Invoke(this, new EntityUnitEventArgs(cmmAdd.Parameters) { Schema = _Schema, TableName = _TableName, CurrentRow = currentRow }); }
        }

        #endregion

        #region Public Fetch/Fill Methods


        #endregion

        #region helper methods

        private void MapEntity(IEntityUnit saveObj, SqlCommand cmm)
        {
            string fieldName;
            object fieldValue;
            foreach (SqlParameter param in cmm.Parameters)
            {   // Loop and get the value for each parameter. The enity may have a lot more fields. 
                // We are concerned with the fields in the table and hence we loop through the command parameters
                fieldValue = null;
                fieldName = param.ParameterName.Replace("@", "fld");
                // First: Do a specific find
                switch (fieldName)
                {
                    //case "fldDocType": // This is the DocType as per DocObjectInfo
                    //    if (saveObj is BO.DocumentBase)
                    //    { fieldValue = (saveObj as BO.DocumentBase).DocObjectInfo.DocObjectType; }
                    //    break;
                    //case "fldDocObject_ID": // This is the DocObject_ID as per DocObjectInfo
                    //    if (saveObj is BO.DocumentBase)
                    //    { fieldValue = (saveObj as BO.DocumentBase).DocObjectInfo.DocObject_ID; }
                    //    break;
                    default:
                        fieldValue = Utilities.ReflectionHelper.GetPropertyValueSafe(saveObj, fieldName);
                        break;
                }
                if (fieldValue != null)
                {
                    switch (param.SqlDbType)
                    {
                        case SqlDbType.DateTime:
                            param.Value = GetDateTimeFieldMapValue(saveObj, fieldName, fieldValue); 
                            break;
                        default:
                            param.Value = fieldValue;
                            break;
                    }
                }
                else
                {
                   param.Value = GetFieldMapFailedValue(saveObj, fieldName, null);
                }
            }
        }

        private void MapEntity(IEntityUnit saveObj, DataRow dr, SqlCommand cmm)
        {
            string fieldName;
            object fieldValue;
            foreach (SqlParameter param in cmm.Parameters)
            {   // Loop and get the value for each parameter. The enity may have a lot more fields. 
                // We are concerned with the fields in the table and hence we loop through the command parameters
                fieldName = param.ParameterName.Replace("@", "fld");
                fieldValue = dr[fieldName];
                if (fieldValue != null)
                {
                    switch (param.SqlDbType)
                    {
                        case SqlDbType.DateTime:
                            param.Value = GetDateTimeFieldMapValue(dr, fieldName, fieldValue);
                            break;
                        default:
                            param.Value = fieldValue;
                            break;
                    }
                }
                else
                {
                    param.Value = GetFieldMapFailedValue(saveObj, fieldName, null);
                }
                    
            }
        }

        private object GetDateTimeFieldMapValue(object saveObj, string fieldName, object fieldValue)
        {
            // check for DBNull
            if (fieldValue is System.DBNull)
            { return System.DBNull.Value; }

            // Check for dateformat
            object[] attribs = Utilities.ReflectionHelper.GetPropertyAttributesSafe(saveObj, fieldName);
            if (attribs != null)
            {
                foreach (object attrib in attribs)
                {
                    if (attrib is DateWithTimeAttribute)
                    { return Convert.ToDateTime(fieldValue).ToString("yyyy-MM-dd HH:mm:ss"); }
                }
            }
            // No custom attributes found. Hence return normal date format
            return Convert.ToDateTime(fieldValue).ToString(MSLA.Server.Constants.SQLDateFormat);
        }

        private object GetFieldMapFailedValue(IEntityUnit saveObj, string fieldName, DataRow currentRow)
        {
            FieldMapEventArgs MFArgs = new FieldMapEventArgs() { Schema = this._Schema, TableName = _TableName, FieldName = fieldName, CurrentRow = currentRow };
            saveObj.FieldMapSaveFailed(MFArgs);
            if (MFArgs.ResultValue == null)
            { throw new Exception("Field mapping for '" + fieldName + "' failed. \nPlease return a valid value by overriding method FieldMapFailed and provide a valid ResultValue in the Business Object."); }
            else
            { return MFArgs.ResultValue; }
        }

        private void ValidateSelf()
        {
            string msg = string.Empty;
            if (this._TableType == enTableType.Unknown)
            { msg = "Entity Unit Error: Table Type not set"; }
            else if (this._SQLConnect == null)
            { msg = "Entity Unit Error: SQL Connection not set"; }
            else if (this._SQLTran == null)
            { msg = "Entity Unit Error: SQL Transaction not set"; }
            else if (this._TableName == string.Empty)
            { msg = "Entity Unit Error: Table Name not set"; }

            if (msg != string.Empty)
            {throw new Exception(msg);}

        }


        #endregion


    } 

    /// <summary>Interface required by Entity Unit</summary>
    public interface IEntityUnit
    {
        /// <summary>Return the UserInfo</summary>
        Security.IUser UserInfo { get; }
        /// <summary>Returns the Connection type used by the BO</summary>
        Data.DBConnectionType DBType { get; }

        /// <summary>Implement this method to handled failed mapping. ResultValue must contain a proper value upon exit of this function.</summary>
        /// <param name="e">Event Args</param>
        void FieldMapSaveFailed(FieldMapEventArgs e);

        /// <summary>Implement this method to handle Fetch/fill failed field mapping</summary>
        /// <param name="e">Event Args. ResultValue contains the value to be loaded</param>
        void FieldMapFetchFailed(FieldMapEventArgs e);
    }
}

