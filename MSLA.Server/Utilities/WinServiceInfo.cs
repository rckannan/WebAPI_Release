using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace MSLA.Server.Utilities
{
    public class WinServiceInfo
    {
        private static DataTable _ServiceInfo;
        private static WinServiceInfo _myInstance;

        private WinServiceInfo()
        { }

        public static WinServiceInfo GetInstance()
        {
            if (_myInstance == null)
                CreateInstance();

            return _myInstance;
        }

        private static void CreateInstance()
        {
            _myInstance = new WinServiceInfo();
        }

        public DataTable getServiceInfo(Security.IUser UserInfo)
        {
            CheckService(UserInfo);
            return _ServiceInfo;
        }

        private static void CheckService(Security.IUser UserInfo)
        {
            if (_ServiceInfo == null)
            { _ServiceInfo = new DataTable(); }

            ServiceController sc;
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;
            cmm.CommandText = "SELECT fldService_ID,fldName ,fldFriendlyName ,fldServiceUserName , fldNeedsExternalConnection, 0 as fldServiceStatus FROM System.tblWinServiceDetail";
            _ServiceInfo = MSLA.Server.Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.MainDB);

            foreach (DataRow dr in _ServiceInfo.Rows)
            {
                try
                {
                    sc = new ServiceController(dr["fldName"].ToString());
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        dr["fldServiceStatus"] = 1;
                    }
                    else if (sc != null)
                    {
                        dr["fldServiceStatus"] = 0;
                    }
                }
                catch (Exception ex)
                {
                    dr["fldServiceStatus"] = -1;
                }

            }
            _ServiceInfo.AcceptChanges();
        }

        public string GetServiceLog(string servicename, Security.IUser UserInfo)
        {
            string LogInfo = string.Empty;
            
            try
            {
                SqlCommand cmm = new SqlCommand();
                cmm.CommandType = CommandType.Text;
                cmm.CommandText = "SELECT fldName ,fldInstallPath FROM System.tblWinServiceDetail where fldName=@Name";
                cmm.Parameters.Add("@Name", SqlDbType.VarChar).Value = servicename;
                DataTable dtRes = new DataTable();
                dtRes = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.MainDB);
                if (dtRes.Rows.Count > 0)
                {
                    string LogPath = dtRes.Rows[0]["fldInstallPath"].ToString();
                    if (LogPath != string.Empty)
                    {
                        if (!Directory.Exists(LogPath + "\\Log"))
                        {
                            LogPath = LogPath.Substring(0, LogPath.LastIndexOf('\\'));
                        }
                        if (Directory.Exists(LogPath + "\\Log"))
                        {
                            DirectoryInfo di = new DirectoryInfo(LogPath + "\\Log");
                            FileInfo[] rgFiles = di.GetFiles("*.txt");
                            string resultFile = string.Empty;
                            DateTime refDate = DateTime.MinValue;
                            foreach (FileInfo fi in rgFiles)
                            {
                                if (fi.LastWriteTime > refDate)
                                {
                                    refDate = fi.LastWriteTime;
                                    resultFile = fi.FullName;
                                }
                            }

                            if (resultFile != string.Empty)
                            {
                                StreamReader myFile = new StreamReader(resultFile);
                                LogInfo = myFile.ReadToEnd();
                                myFile.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfo = "--Not able to retrieve log file contents.--";
            }
            return LogInfo;
        }

        public string GetServiceLog(Int64 service_ID, Security.IUser UserInfo)
        {
            string LogInfo = string.Empty;

            try
            {
                SqlCommand cmm = new SqlCommand();
                cmm.CommandType = CommandType.Text;
                cmm.CommandText = "SELECT fldService_ID,fldName ,fldInstallPath FROM System.tblWinServiceDetail where fldService_ID=@sid";
                cmm.Parameters.Add("@sid", SqlDbType.BigInt).Value = service_ID;
                DataTable dtRes = new DataTable();
                dtRes = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.MainDB);
                if (dtRes.Rows.Count > 0)
                {
                    string LogPath = dtRes.Rows[0]["fldInstallPath"].ToString();
                    if (LogPath != string.Empty)
                    {
                        if (!Directory.Exists(LogPath + "\\Log"))
                        {
                            LogPath = LogPath.Substring(0, LogPath.LastIndexOf('\\'));
                        }
                        if (Directory.Exists(LogPath + "\\Log"))
                        {
                            DirectoryInfo di = new DirectoryInfo(LogPath + "\\Log");
                            FileInfo[] rgFiles = di.GetFiles("*.txt");
                            string resultFile = string.Empty;
                            DateTime refDate = DateTime.MinValue;
                            foreach (FileInfo fi in rgFiles)
                            {
                                if (fi.LastWriteTime > refDate)
                                {
                                    refDate = fi.LastWriteTime;
                                    resultFile = fi.FullName;
                                }
                            }

                            if (resultFile != string.Empty)
                            {
                                StreamReader myFile = new StreamReader(resultFile);
                                LogInfo = myFile.ReadToEnd();
                                myFile.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfo = "--Not able to retrieve log file contents.--";
            }
            return LogInfo;
        }

    }
}
