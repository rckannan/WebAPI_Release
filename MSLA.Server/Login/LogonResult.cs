using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Login
{
    /// <summary>The Logon Result returned by the Server</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class LogonResult
        : Security.IUser
    {

        /// <summary>The Logon Status</summary>
        public readonly Logon.LogonState Status = Logon.LogonState.Failed;
        /// <summary>Logon Exception if any</summary>
        public readonly LogonException LogonEx;
        ///
        public readonly Logon.tblMainDB dtMainDB = null;

        //'   ****    Private Variables
        private long _User_ID = -1;
        private String _LogonName = String.Empty;
        private String _FullUserName = String.Empty;
        private Boolean _IsSuperUser = false;
        private String _ServerName = String.Empty;
        private String _DatabaseName = String.Empty;
        private String _MainDBName = string.Empty;
        private String _SuperUser = string.Empty;
        private String _SuperPass = string.Empty;
        private System.Guid _Session_ID = System.Guid.NewGuid();
        private string _emailAddress = string.Empty;

        //   ****    Friend Variables
        internal LogonInfo LogonInfoUsed;

        #region "Interface Implementation and Public Properties"

        /// <summary>Gets the User ID</summary>
        public long User_ID
        {
            get { return _User_ID; }
        }

        /// <summary>Gets the Logon Name of the User</summary>
        public string LogonName
        {
            get { return _LogonName; }
        }

        /// <summary>Gets the Full user Name</summary>
        public String FullUserName
        {
            get { return (_FullUserName); }
        }

        /// <summary>Gets if User is Superuser</summary>
        public Boolean IsSuperUser
        {
            get { return _IsSuperUser; }
        }

        /// <summary>Gets the ServerName</summary>
        public string ServerName
        {
            get { return _ServerName; }
        }

        /// <summary>Gets the Company Database Name</summary>
        public string DatabaseName
        {
            get { return _DatabaseName; }
        }

        /// <summary>Gets the MainDB Name</summary>
        public string MainDBName
        {
            get { return _MainDBName; }
        }

        /// <summary>Gets the SuperUser</summary>
        public string SuperUser
        {
            get { return _SuperUser; }
        }

        /// <summary>Gets the SuperPass</summary>
        public string SuperPass
        {
            get { return _SuperPass; }
        }

        /// <summary>The Session ID</summary>
        public System.Guid Session_ID
        {
            get { return _Session_ID; }
        }

        #endregion

        internal LogonResult(Logon.LogonState myStatus, String myFullUserName, String myMainConnectString, Boolean myUserIsSuperUser,
                                long myUserID, string logonName, string myServerName, string myDatabaseName, string myMainDBName)
        {
            if (myStatus == Logon.LogonState.Succeeded)
            {
                _FullUserName = myFullUserName;
                _IsSuperUser = myUserIsSuperUser;
                _ServerName = myServerName;
                _DatabaseName = myDatabaseName;
                _MainDBName = myMainDBName;
                Status = myStatus;
                _LogonName = logonName;
                _User_ID = myUserID;
            }
        }

        internal LogonResult(Logon.LogonState myStatus, String myFullUserName, String myMainConnectString, Boolean myUserIsSuperUser,
                                long myUserID, string logonName, string myServerName, string myDatabaseName, string myMainDBName, string emailAddress)
        {
            if (myStatus == Logon.LogonState.Succeeded)
            {
                _FullUserName = myFullUserName;
                _IsSuperUser = myUserIsSuperUser;
                _ServerName = myServerName;
                _DatabaseName = myDatabaseName;
                _MainDBName = myMainDBName;
                Status = myStatus;
                _LogonName = logonName;
                _User_ID = myUserID;
                _emailAddress = emailAddress;
            }
        }

        internal LogonResult(LogonException myException)
        {
            LogonEx = myException;
        }

        internal LogonResult(Logon.tblMainDB dtMDB)
        {
            this.dtMainDB = dtMDB;
        }

        internal void SetDatabaseName(string DBName)
        {
            _DatabaseName = DBName;
        }

        internal void SetMainDBName(string maindb)
        {
            _MainDBName = maindb;
        }

        #region "Methods for GetXML And ReadXML"

        /// <summary>Gets the Logon Info in an array of Bytes</summary>
        //public Byte[] GetXML()
        //{

        //    Utilities.XMLWriter XMLw = new Utilities.XMLWriter("MessageTag", "LogonResult");

        //    XMLw.SetValue("User_ID", _User_ID.ToString());
        //    XMLw.SetValue("FullUserName", _FullUserName);
        //    XMLw.SetValue("IsSuperUser", _IsSuperUser.ToString());
        //    XMLw.SetValue("ServerName", _ServerName);
        //    XMLw.SetValue("DatabaseName", _DatabaseName);
        //    XMLw.SetValue("MainConnectString", _MainConnectString);
        //    XMLw.SetValue("CompanyConnectString", _CompanyConnectString);
        //    XMLw.SetValue("DMSConnectString", _DMSConnectString);
        //    XMLw.SetValue("Session_ID", _Session_ID.ToString());
        //    XMLw.SetValue("IV", System.Text.Encoding.ASCII.GetString(_IV));

        //    return XMLw.GetBytes;

        //}
        #endregion

    }
}
