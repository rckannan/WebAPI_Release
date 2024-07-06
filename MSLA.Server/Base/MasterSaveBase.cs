using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MSLA.Server.Base
{
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public abstract class MasterSaveBase
        : Base.VersionBase, Security.IARMaster, Entity.IEntityUnit
    {
        private Rules.BrokenRuleCollection _BrokenSaveRules = new Rules.BrokenRuleCollection();
        private Rules.BrokenRuleCollection _BrokenDeleteRules = new Rules.BrokenRuleCollection();

        private Base.DocMaster _ObjectInfo;
        //private Data.DBConnectionType _ConnectionType;
        private Security.EnAccessLevelMaster _AccessLevel = MSLA.Server.Security.EnAccessLevelMaster.No_Access;
        private bool _IsDeleteAllowed = false;
        private XMLDataTypes.MasterItem _MasterItemQueEntry = null;
        private String _ErrorMsg = string.Empty;


        /// <summary>Set the Client Machine Name to be saved into the Log</summary>
        public String ClientMachineName = string.Empty;

        ///<summary>Use this constructor to connect to the requested Database</summary>
        ///<param name="User">The User Info</param>
        ///<param name="DbType">The DB Type</param>
        protected MasterSaveBase(Security.IUser User, MSLA.Server.Data.DBConnectionType DbType)
            : base(User, DbType)
        {
            //_ConnectionType = DbType;
        }

        #region "Interface Implementation - IARMaster"

        void Security.IARMaster.SetAccessLevel(Security.EnAccessLevelMaster AccessLevel)
        { _AccessLevel = AccessLevel; }

        void Security.IARMaster.SetIsDeleteAllowed(bool IsDelAllow)
        { _IsDeleteAllowed = IsDelAllow; }

        #endregion

        #region "Public Properties"

        /// <summary>Object Info</summary>
        public Base.DocMaster DocObjectInfo
        {
            get { return _ObjectInfo; }
        }

        internal void SetObjectInfo(Base.DocMaster ObjInfo)
        {
            _ObjectInfo = ObjInfo;
        }

        /// <summary>Gets the Access Level</summary>
        public Security.EnAccessLevelMaster AccessLevel
        { get { return _AccessLevel; } }

        /// <summary>Gets if the user has rights to delete the master item</summary>
        public bool IsDeleteAllowed
        { get { return _IsDeleteAllowed; } }

        /// <summary>Returns 'True' if the Master Object has broken any Save Rules.</summary>
        public bool HasBrokenSaveRules
        {
            get
            {
                if (this._BrokenSaveRules.Count > 0)
                { return true; }
                else
                { return false; }
            }
        }

        ///<summary>Returns 'True' if the Master Object has broken any Delete Rules.</summary>
        public Boolean HasBrokenDeleteRules
        {
            get
            {
                if (this._BrokenDeleteRules.Count > 0)
                { return true; }
                else
                { return false; }
            }
        }

        ///<summary>Returns the Collection of Broken Save Rules.</summary>
        public override Rules.BrokenRuleCollection BrokenSaveRules
        {
            get { return _BrokenSaveRules; }
        }

        ///<summary>Returns the Collection of Broken Delete Rules.</summary>
        public override Rules.BrokenRuleCollection BrokenDeleteRules
        {
            get { return _BrokenDeleteRules; }
        }

        #endregion

        #region "Protected Abstract Methods"
        /// <summary>Override this method to set the Master Iten ID on Fetch or Save</summary>
        /// <param name="Item_ID">The ID of the Master Item</param>
        protected abstract void SetDocMaster_ID(long Item_ID);

        /// <summary>Override this to return the Master Table name that would be used to write into the Que</summary>
        protected abstract string MasterTableName
        { get; }

        #endregion

        #region "Save Code"

        internal void DoSaveValidations()
        {
            //   ****    Clear all Broken Rules
            this.BrokenSaveRules.Clear();
            this.BrokenDeleteRules.Clear();

            //  ****    Now Call Client Validations
            ValidateBeforeSave();

            //  ****    Proceed only if there are no Broken Rules
            if (this.HasBrokenSaveRules)
            {
                throw new Validations.ValidateException("Master Document has broken business rules. Please rectify before saving.");
            }
        }

        /// <summary>Override this method to validate before save</summary>
        protected abstract void ValidateBeforeSave();

        internal void SaveDocument()
        {
            SqlTransaction cnTran = null;
            //LogMaster myLogDetails;

            using (SqlConnection cn = Data.DataAccess.GetCn(ConnectionType))
            {
                MasterSaveResult LocalSaveResult;
                try
                {
                    //  First Open the Connection and create a transaction
                    cn.Open();
                    cnTran = cn.BeginTransaction(IsolationLevel.ReadCommitted);

                    //  ****    Save Vch Control And Tran
                    LocalSaveResult = this.SaveControlTran(cn, cnTran);

                    //   ****    Commit if there are no errors
                    cnTran.Commit();
                    cnTran = null;

                    //   ****    Always call aftersave
                    AfterSave(LocalSaveResult);

                    //  ****    Clear Old Version and Create present data version
                    this.ClearVersion();
                    this.CreateVersion();
                }
                catch (Exception ex)
                {
                    if (cnTran != null)
                    {
                        try
                        { cnTran.Rollback(); }
                        catch { }
                    }
                    throw ex;
                }
                finally
                {
                    if (cn.State != ConnectionState.Closed)
                    { cn.Close(); }
                }
            }
        }

        /// <summary>Override this method to save the Master Document and return Master Save Result</summary>
        /// <param name="cn">The Open Connection</param>
        /// <param name="cnTran">The Open Transaction</param>
        protected abstract MasterSaveResult SaveControlTran(SqlConnection cn, SqlTransaction cnTran);

        ///<summary>This method is called after the document is saved successfully. Write any After Save Code here.</summary>
        protected abstract void AfterSave(MasterSaveResult LocalSaveResult);

        #endregion

        #region "Delete Code"
        internal void DoDeleteValidations()
        {
            //  ****    Clear all Broken Rules
            this.BrokenSaveRules.Clear();
            this.BrokenDeleteRules.Clear();

            //   ****    Now Call Client Validations
            ValidateBeforeDelete();

            //   ****    Do Not Proceed it Document has Broken Rules
            if (this.HasBrokenDeleteRules)
            {
               throw new Validations.ValidateException("Master Document has broken business rules. Please rectify before delete.");
            }
        }

        /// <summary>Override this method to validate before delete.</summary>
        protected abstract void ValidateBeforeDelete();

        internal void DeleteDocument()
        {

            SqlTransaction cnTran = null;
            MasterSaveResult LocalSaveResult;
            //LogMaster myLogDetails;

            using (SqlConnection cn = Data.DataAccess.GetCn(ConnectionType))
            {
                try
                {
                    //****    First Open the Connection and create a transaction
                    cn.Open();
                    cnTran = cn.BeginTransaction(IsolationLevel.ReadCommitted);

                    // ****    Save Vch Control And Tran
                    LocalSaveResult = this.DeleteControlTran(cn, cnTran);

                    // ****    Commit if there are no errors
                    cnTran.Commit();
                    cnTran = null;

                    // ****    Always call aftersave
                    AfterDelete();

                    //  Clear Version
                    this.ClearVersion();
                }
                catch (Exception ex)
                {
                    if (cnTran != null)
                    {
                        try
                        { cnTran.Rollback(); }
                        catch { }
                    }
                    throw ex;
                }
                finally
                {
                    if (cn.State != ConnectionState.Closed)
                    { cn.Close(); }
                }
            }
        }

        /// <summary>Override this method to delete the document and return Master Save Result</summary>
        /// <param name="cn">The Open Connection</param>
        /// <param name="cnTran">The open Transaction</param>
        protected abstract MasterSaveResult DeleteControlTran(SqlConnection cn, SqlTransaction cnTran);

        ///<summary>This method is called after the document is deleted successfully. Write any After Delete Code here.</summary>
        protected virtual void AfterDelete()
        {
        }

        #endregion

        /// <summary>The master save result class</summary>
        public class MasterSaveResult
        {
            /// <summary>Master ID</summary>
            public long fldMasterDoc_ID;
            /// <summary>The Last updated date time returned from the server</summary>
            public DateTime fldLastUpdated;

            /// <summary>Constructor</summary>
            public MasterSaveResult()
                : this(-1)
            {
            }

            /// <summary>Constructor</summary>
            /// <param name="Doc_ID">The master Doc ID</param>
            public MasterSaveResult(long Doc_ID)
            {
                this.fldMasterDoc_ID = Doc_ID;
                this.fldLastUpdated = DateTime.Now;
            }
        }

        #region "IEntityUnit Implementation"

        Security.IUser Entity.IEntityUnit.UserInfo
        {
            get { return this.UserInfo; }
        }

        Data.DBConnectionType Entity.IEntityUnit.DBType
        {
            get { return this.ConnectionType; }
        }

        void Entity.IEntityUnit.FieldMapSaveFailed(Entity.FieldMapEventArgs e)
        {
            this.OnFieldMapSaveFailed(e);
        }

        void Entity.IEntityUnit.FieldMapFetchFailed(Entity.FieldMapEventArgs e)
        {
            this.OnFieldMapFetchFailed(e);
        }

        /// <summary>Override this method to handle Entity Field Map Failed</summary>
        /// <param name="e">Field Map Event Args</param>
        protected virtual void OnFieldMapSaveFailed(Entity.FieldMapEventArgs e)
        {

        }

        /// <summary>Override this method to handle Entity Field Map Failed</summary>
        /// <param name="e">Field Map Event Args</param>
        protected virtual void OnFieldMapFetchFailed(Entity.FieldMapEventArgs e)
        {

        }

        #endregion
    }
}
