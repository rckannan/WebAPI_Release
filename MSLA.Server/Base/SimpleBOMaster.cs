using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Base
{
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(System.DBNull))]
    [System.Runtime.Serialization.KnownType(typeof(MSLA.Server.Data.SimpleTable))]
    public class SimpleBOMaster : IDisposable
    {
        Int64 _fldMasterItem_ID = -1;
        string _masterType = string.Empty;
        Dictionary<string, object> _propertyBag = new Dictionary<string, object>();
        Dictionary<string, bool> _propertyIsReadOnly = new Dictionary<string, bool>();
        Guid _fldSerializedMaster_ID = Guid.NewGuid();
        //long _fldUser_ID = -1;
        //Guid _fldUserSession_ID = Guid.NewGuid();

        Security.SimpleUserInfo _UserInfo = new Security.SimpleUserInfo();
        Security.EnAccessLevelMaster _AccessLevel = Security.EnAccessLevelMaster.No_Access;
        VersionBase.EnAuditMode _AuditMode = VersionBase.EnAuditMode.NoTrail;
        Rules.BrokenRuleCollection _BrokenDeleteRules = new Rules.BrokenRuleCollection();
        Rules.BrokenRuleCollection _BrokenSaveRules = new Rules.BrokenRuleCollection();
        DocMaster _DocObjectInfo = new DocMaster();
        Int64 _DocObjectIC_ID = -1;
        Guid _VersionInfo = Guid.NewGuid();
        Boolean _IsDeleteAllowed = false;

        string _ErrorMsg = string.Empty;


        public Guid fldSerializedMaster_ID
        {
            get { return _fldSerializedMaster_ID; }
            set { _fldSerializedMaster_ID = value; }
        }

        public Int64 fldMasterItem_ID
        {
            get { return _fldMasterItem_ID; }
            set { _fldMasterItem_ID = value; }
        }

        public string MasterType
        {
            get { return _masterType; }
            set { _masterType = value; }
        }

        public Dictionary<string, bool> PropertyIsReadOnly
        {
            get { return _propertyIsReadOnly; }
            set { _propertyIsReadOnly = value; }
        }

        //public long fldUser_ID
        //{
        //    get { return _fldUser_ID; }
        //    set { _fldUser_ID = value; }
        //}

        //public Guid fldUserSession_ID
        //{
        //    get { return _fldUserSession_ID; }
        //    set { _fldUserSession_ID = value; }
        //}

        public Server.Security.SimpleUserInfo UserInfo
        {
            get { return _UserInfo; }
            set { _UserInfo = value; }
        }

        public Dictionary<string, object> PropertyBag
        {
            get { return _propertyBag; }
            set { _propertyBag = value; }
        }

        internal Security.EnAccessLevelMaster AccessLevel
        {
            set { _AccessLevel = value; }
        }

        internal VersionBase.EnAuditMode AuditMode
        {
            set { _AuditMode = value; }
        }

        public Rules.BrokenRuleCollection BrokenDeleteRules
        {
            get { return _BrokenDeleteRules; }
            set { _BrokenDeleteRules = value; }
        }

        public Rules.BrokenRuleCollection BrokenSaveRules
        {
            get { return _BrokenSaveRules; }
            set { _BrokenSaveRules = value; }
        }

        internal DocMaster DocObjectInfo
        {
            set { _DocObjectInfo = value; }
        }

        internal Int64 DocObjectIC_ID
        {
            set { _DocObjectIC_ID = value; }
        }

        internal Guid VersionInfo
        {
            set { _VersionInfo = value; }
        }

        internal Boolean IsDeleteAllowed
        {
            set { _IsDeleteAllowed = value; }
        }

        public Security.EnAccessLevelMaster GetAccessLevel
        {
            get { return _AccessLevel; }
        }

        public VersionBase.EnAuditMode GetAuditMode
        {
            get { return _AuditMode; }
        }

        public DocMaster GetDocObjectInfo
        {
            get { return _DocObjectInfo; }
        }

        public Int64 GetDocObjectIC_ID
        {
            get { return _DocObjectIC_ID; }
        }

        public Guid GetVersionInfo
        {
            get { return _VersionInfo; }
        }

        public Boolean GetIsDeleteAllowed
        {
            get { return _IsDeleteAllowed; }
        }

        public string ErrorMsg
        {
            get { return _ErrorMsg; }
            set { _ErrorMsg = value; }
        }

        public void Dispose()
        {
            _propertyBag.Clear();
            _propertyBag = null;
        }
    }
}
