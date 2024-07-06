using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization;

namespace MSLA.Server.Base
{
    /// <summary>Abstract Version Base. The Versioning logic is written in this class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public abstract class VersionBase
        : Rules.IBrokenRules
    {
        private Security.IUser _UserInfo;
        private Data.DBConnectionType _ConnectionType;
        private Guid _MyVersion = System.Guid.NewGuid();
        private EnAuditMode _AuditMode = EnAuditMode.Complete;

        private System.Int64 _docObjectIC_ID = -1;

        /// <summary>Constructor</summary>
        /// <param name="User"></param>
        /// <param name="DbType"></param>
        protected VersionBase(Security.IUser User, Data.DBConnectionType DbType)
        {
            _UserInfo = User;
            _ConnectionType = DbType;
        }


        /// <summary>The Audit Mode Enum</summary>
        public enum EnAuditMode
        {
            /// <summary>Maintains Complete Audit Trail</summary>
            Complete = 0,
            /// <summary>Does not maintain any trail</summary>
            NoTrail = 1,
            /// <summary>Maintains user info trail only</summary>
            UserInfoOnly = 2
        }

        ///<summary>Returns the Collection of Broken Save Rules.</summary>
        public abstract Rules.BrokenRuleCollection BrokenSaveRules
        { get; }
        ///<summary>Returns the Collection of Broken Delete Rules.</summary>
        public abstract Rules.BrokenRuleCollection BrokenDeleteRules
        { get; }

        /// <summary>The Login User Info</summary>
        protected internal Security.IUser UserInfo
        {
            get { return _UserInfo; }
        }

        /// <summary>Connection Type</summary>
        protected Data.DBConnectionType ConnectionType
        {
            get { return _ConnectionType; }
        }

        ///<summary>Creates a new version of Self.</summary>
        protected internal void CreateVersion()
        {
            if (_AuditMode == EnAuditMode.Complete)
            {
                Utilities.XMLFormatter XFormatter = new Utilities.XMLFormatter();
                SqlCommand cmm = new SqlCommand();
                cmm.CommandText = "AuditTrail.spCreateVersionCache";
                cmm.CommandType = CommandType.StoredProcedure;
                _MyVersion = Guid.NewGuid();
                cmm.Parameters.Add("@Version_ID", SqlDbType.UniqueIdentifier).Value = _MyVersion;
                cmm.Parameters.Add("@VersionInfo", SqlDbType.Xml).Value = System.Text.Encoding.ASCII.GetString(XFormatter.Serialize(this));
                Data.DataConnect.ExecCMM(UserInfo, ref cmm, _ConnectionType);
            }
        }

        /// <summary>Restores the already existing version upon itself. Will work only if version exists.</summary>
        public void RestoreVersion()
        {
            throw new VersionException("This Method is not implemented.");
        }

        /// <summary>Clears already existing version, if any. Call this before calling CreateVersion.</summary>
        public void ClearVersion()
        {
            if (_AuditMode == EnAuditMode.Complete)
            {
                SqlCommand cmm = new SqlCommand();
                cmm.CommandText = "AuditTrail.spClearVersionCache";
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.Parameters.Add("@Version_ID", SqlDbType.UniqueIdentifier).Value = _MyVersion;
                Data.DataConnect.ExecCMM(UserInfo, ref cmm, _ConnectionType);
            }
        }

        /// <summary>Gets if a version has already been created.</summary>
        [VersionNotRequired]
        public bool HasVersion
        {
            get
            {
                if (_AuditMode == EnAuditMode.Complete)
                { return true; }
                else
                { return false; }
            }
        }

        /// <summary>Gets the mode of AuditTrail.</summary>
        [VersionNotRequired]
        public EnAuditMode AuditMode
        { get { return _AuditMode; } }

        /// <summary>Sets the Audit Trail Mode</summary>
        /// <param name="audMode">the Audit Mode Enum</param>
        protected internal void SetAuditMode(EnAuditMode audMode)
        { _AuditMode = audMode; }

        /// <summary>Returns the GUID represented in AuditTrail.tblVersionCache.</summary>
        [VersionNotRequired]
        public Guid GetVersionInfo
        {
            get
            {
                return _MyVersion;
            }
        }

        /// <summary>Gets the ID set at the time of creating an instance. Valid only for new documents.</summary>
        public Int64 GetDocObjectIC_ID
        {
            get { return _docObjectIC_ID; }
        }

        /// <summary>Sets the IC ID</summary>
        /// <param name="ic_ID">The IC ID</param>
        internal void SetDocObjectIC_ID(Int64 ic_ID)
        { _docObjectIC_ID = ic_ID; }


        #region "Custom Attributes and Exception"

        /// <summary>Use this attribute to mark properties as VersionNotRequired.</summary>
        public class VersionNotRequiredAttribute
            : Attribute
        {
        }

        /// <summary>The Version Exception Class. This can be used to generate VersionExceptions from VersionBase</summary>
        [Serializable()]
        public class VersionException
            : Exception
        {

            /// <summary>Creates a new instance of VersionException</summary>
            /// <param name="Msg">The message to be sent as part of exception.</param>
            public VersionException(string Msg)
                : base(Msg)
            {
            }

            /// <summary>The version exception is raised while doing some activity in the Version Base</summary>
            /// <param name="info">SerializationInfo</param>
            /// <param name="context">StreamingContext</param>
            public VersionException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }

        }

        #endregion

    }
}
