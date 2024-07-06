using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace MSLA.Server.Login
{
        /// <summary>The Logon Info Class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable]
    public class LogonInfo
    {
        /// <summary>Username for Logon</summary>
        public readonly String UserName = String.Empty;
        /// <summary>User Password for Logon</summary>
        public readonly String UserPass = String.Empty;
        /// <summary>The Client Machine Name</summary>
        public readonly string ClientMachineName = string.Empty;
        /// <summary>The Client Host Name</summary>
        public readonly string ClientHostName = string.Empty;
        /// <summary>The Client Domain</summary>
        public readonly string ClientDomain = string.Empty;
        /// <summary>The Client IP</summary>
        public readonly string ClientIP = string.Empty;
        /// <summary>The Client MAC address</summary>
        public readonly string ClientMAC = string.Empty;

        public readonly long User_ID = -1;

        public readonly Guid Session_ID = new Guid();

        private String _SQLServer = String.Empty;
        private String _SuperUser = String.Empty;
        private String _SuperPass = String.Empty;
        private String _MainDB = string.Empty;

        /// <summary>Constructor</summary>
        /// <param name="myUserName">Username for Logon</param>
        /// <param name="myUserPass">User Password for Logon</param>
        /// <param name="mySuperUser">The Super User</param>
        /// <param name="mySuperPass">The Super password</param>
        /// <param name="myMainDB">The Main DB</param>
        /// <param name="mySqlServer">The Server to Connect To</param>
        public LogonInfo(String myUserName, String myUserPass, String mySuperUser, String mySuperPass,
                                    String myMainDB, String mySqlServer)
        {
            UserName = myUserName;
            UserPass = myUserPass;
            _MainDB = myMainDB;
            _SQLServer = mySqlServer;
            _SuperUser = mySuperUser;
            _SuperPass = mySuperPass;
            ClientMachineName = Environment.MachineName;

            // Fetch IPs and MAC
            IPGlobalProperties compProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            ClientHostName = compProperties.HostName;
            ClientDomain = compProperties.DomainName;
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection Unics = properties.UnicastAddresses;

                foreach (UnicastIPAddressInformation Uni in Unics)
                {
                    if (Uni.IPv4Mask != null && Uni.IsDnsEligible)
                    {
                        ClientIP += Uni.Address.ToString();
                        ClientMAC += adapter.GetPhysicalAddress().ToString();
                        break;
                    }
                }
            }
        }


        /// <summary>Constructor</summary>
        /// <param name="myUserName">Username for Logon</param>
        /// <param name="myUserPass">User Password for Logon</param>
        /// <param name="mySuperUser">The Super User</param>
        /// <param name="mySuperPass">The Super password</param>
        /// <param name="myMainDB">The Main DB</param>
        /// <param name="mySqlServer">The Server to Connect To</param>
        /// <param name="macName">The client machine name</param>
        /// <param name="hostName">The client host name</param>
        /// <param name="clientDomain">The client domain name</param>
        /// <param name="clientIP">The client IP address</param>
        /// <param name="clientMAC">The client MAC address</param>
        public LogonInfo(String myUserName, String myUserPass, String mySuperUser, String mySuperPass,
                                    String myMainDB, String mySqlServer, String macName, String hostName, String clientDomain, String clientIP, String clientMAC)
        {
            UserName = myUserName;
            UserPass = myUserPass;
            _MainDB = myMainDB;
            _SQLServer = mySqlServer;
            _SuperUser = mySuperUser;
            _SuperPass = mySuperPass;
            ClientMachineName = macName;

            ClientHostName = hostName;
            ClientDomain = clientDomain;

            ClientIP = clientIP;
            ClientMAC = clientMAC;
        }

        /// <summary>Constructor for Application Server</summary>
        /// <param name="myUserName">Username for Logon</param>
        /// <param name="myUserPass">User Password for Logon</param>
        public LogonInfo(String myUserName, String myUserPass)
            : this(myUserName, myUserPass, string.Empty, string.Empty, string.Empty, string.Empty)
        {

        }

        /// <summary>Constructor for Application Server</summary>
        /// <param name="myUserName">Username for Logon</param>
        public LogonInfo(String myUserName)
            : this(myUserName, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {

        }

        /// <summary>Gets the MainDB Name</summary>
        public string MainDB
        {
            get { return _MainDB; }
        }

        internal void SetMainDB(string MainDBName)
        {
            _MainDB = MainDBName;
        }

        internal string SQLServer
        {
            get { return _SQLServer; }
            set { _SQLServer = value; }
        }

        internal string SuperUser
        {
            get { return _SuperUser; }
            set { _SuperUser = value; }
        }

        internal string SuperPass
        {
            get { return _SuperPass; }
            set { _SuperPass = value; }
        }

        public LogonInfo(long UserID, Guid SessionID)        
        {
            User_ID = UserID;
            Session_ID = SessionID;
        }

    }
}


        