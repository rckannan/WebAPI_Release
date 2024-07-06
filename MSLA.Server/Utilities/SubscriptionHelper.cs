using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Utilities
{
    public class SubscriptionHelper
    {
        private static List<Int64> _validSubs;
        private static DataTable _dtSubAddresses;

        private static bool RuleValidator(String ruleDataType, String ruleOperand, String ruleValue1, String ruleValue2, Object tstValue)
        {
            Boolean result = false;
            Int64 rVal1, rVal2, tVal = 0;
            DateTime rDt1, rDt2, tDt;
            String testValue = tstValue.ToString();

            switch (ruleOperand)
            {
                case ">":
                    switch (ruleDataType)
                    {
                        case "Numeric":
                            if (Int64.TryParse(ruleValue1, out rVal1) && Int64.TryParse(testValue, out tVal))
                            {
                                if (tVal > rVal1)
                                { result = true; }
                                else
                                { result = false; }
                            }
                            break;

                        case "DateTime":
                            if (DateTime.TryParse(ruleValue1, out rDt1) && DateTime.TryParse(testValue, out tDt))
                            {
                                if (tDt > rDt1)
                                { result = true; }
                                else
                                { result = false; }
                            }
                            break;
                    }
                    break;
                case "<":
                    switch (ruleDataType)
                    {
                        case "Numeric":
                            if (Int64.TryParse(ruleValue1, out rVal1) && Int64.TryParse(testValue, out tVal))
                            {
                                if (tVal < rVal1)
                                { result = true; }
                            }
                            break;

                        case "DateTime":
                            if (DateTime.TryParse(ruleValue1, out rDt1) && DateTime.TryParse(testValue, out tDt))
                            {
                                if (tDt < rDt1)
                                { result = true; }
                            }
                            break;
                    }
                    break;
                case "=":
                    switch (ruleDataType)
                    {
                        case "Numeric":
                            if (Int64.TryParse(ruleValue1, out rVal1) && Int64.TryParse(testValue, out tVal))
                            {
                                if (tVal == rVal1)
                                { result = true; }
                            }
                            break;

                        case "DateTime":
                            if (DateTime.TryParse(ruleValue1, out rDt1) && DateTime.TryParse(testValue, out tDt))
                            {
                                if (tDt == rDt1)
                                { result = true; }
                            }
                            break;

                        case "String":
                            if (testValue == ruleValue1)
                            { result = true; }
                            break;
                    }
                    break;

                case "between":
                    switch (ruleDataType)
                    {
                        case "Numeric":
                            if (Int64.TryParse(ruleValue1, out rVal1) && Int64.TryParse(testValue, out tVal) && Int64.TryParse(ruleValue2, out rVal2))
                            {
                                if ((rVal1 <= tVal && rVal2 >= tVal) || (rVal1 >= tVal && rVal2 <= tVal))
                                { result = true; }
                            }
                            break;

                        case "DateTime":
                            if (DateTime.TryParse(ruleValue1, out rDt1) && DateTime.TryParse(testValue, out tDt) && DateTime.TryParse(ruleValue2, out rDt2))
                            {
                                if ((rDt1 <= tDt && rDt2 >= tDt) || (rDt1 >= tDt && rDt2 <= tDt))
                                { result = true; }
                            }
                            break;
                    }
                    break;
            }
            return result;
        }

        private static void SetAddresses(Int64 Sub_ID, Security.IUser UserInfo)
        {
            _dtSubAddresses = null;
            DataTable dtSubs = new DataTable();
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;
            cmm.CommandText = "SELECT fldEmail_ID, fldEmailAddress, fldSubscription_ID, fldCc FROM Email.tblMailAddress";
            cmm.CommandText += " Where fldSubscription_ID = @Subscription_ID";
            cmm.Parameters.Add("@Subscription_ID", SqlDbType.BigInt).Value = Sub_ID;

            //foreach (Int64 sub_ID in _validSubs)
            dtSubs = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.CompanyDB);
            if (_dtSubAddresses == null)
            {
                _dtSubAddresses = new DataTable();
                _dtSubAddresses = dtSubs.Copy();
            }
            else
            {
                foreach (DataRow drSub in dtSubs.Rows)
                {
                    if ((_dtSubAddresses.Select("fldEmailAddress='" + drSub["fldEmailAddress"].ToString() + "'")).Length == 0)
                    {
                        DataRow drSubNew;
                        drSubNew = _dtSubAddresses.NewRow();
                        drSubNew["fldEmail_ID"] = drSub["fldEmail_ID"];
                        drSubNew["fldEmailAddress"] = drSub["fldEmailAddress"];
                        drSubNew["fldSubscription_ID"] = drSub["fldSubscription_ID"];
                        drSubNew["fldCc"] = drSub["fldCc"];
                        _dtSubAddresses.Rows.Add(drSubNew);
                    }
                }
            }
            //}
        }

        private static string SetFromAddress(Int64 Sub_ID, Security.IUser UserInfo)
        {
            DataTable dt = new DataTable();
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;
            cmm.CommandText = "select fldMailFrom from Email.tblMailFrom where fldmailFrom_ID =(select fldMailFrom_ID from Email.tblSubscription where fldSubscription_ID=@Subscription_ID)";
            cmm.Parameters.Add("@Subscription_ID", SqlDbType.BigInt).Value = Sub_ID;
            dt = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.CompanyDB);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["fldMailFrom"].ToString();
            }
            return string.Empty;
        }

        private static void generateMails(Int64 Category_ID, String Message, Boolean isHTML, String Subject, string MailFrom, string FileName, byte[] FileStreamInfo, Security.IUser UserInfo)
        {
            StringBuilder sbMailTo = new StringBuilder();
            StringBuilder sbCC = new StringBuilder();

            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandText = "System.spNotificationMailAdd";
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.Parameters.Add("@Category_ID", SqlDbType.BigInt, 0).Value = Category_ID;
            Cmm.Parameters.Add("@MailTo", SqlDbType.VarChar);
            Cmm.Parameters.Add("@MailFrom", SqlDbType.VarChar, 100).Value = MailFrom;
            Cmm.Parameters.Add("@Body", SqlDbType.VarChar).Value = Message;
            Cmm.Parameters.Add("@Subject", SqlDbType.VarChar, 250).Value = Subject;
            Cmm.Parameters.Add("@Cc", SqlDbType.VarChar);
            Cmm.Parameters.Add("@BCc", SqlDbType.VarChar).Value = string.Empty;
            Cmm.Parameters.Add("@Attachment", SqlDbType.VarBinary, -1).Value = FileStreamInfo;
            Cmm.Parameters.Add("@FileName", SqlDbType.VarChar, 50).Value = FileName;
            Cmm.Parameters.Add("@IsBodyHtml", SqlDbType.Bit).Value = isHTML;

            foreach (DataRow drSub in _dtSubAddresses.Rows)
            {
                if (Convert.ToBoolean(drSub["fldCc"]) == false)
                {
                    if (sbMailTo.Length != 0)
                    { sbMailTo.Append(","); }
                    sbMailTo.Append(drSub["fldEmailAddress"].ToString());
                }
                else
                {
                    if (sbCC.Length != 0)
                    { sbCC.Append(","); }
                    sbCC.Append(drSub["fldEmailAddress"].ToString());
                }
            }

            Cmm.Parameters["@MailTo"].Value = sbMailTo.ToString();
            Cmm.Parameters["@Cc"].Value = sbCC.ToString();
            MSLA.Server.Data.DataConnect.ExecCMM(UserInfo, ref Cmm, Data.DBConnectionType.CompanyDB);
        }

        public static bool ContractSubscriptionService(Int64 Category_ID, String Message, Boolean isHTML, String Subject, string FileName, byte[] FileStreamInfo, Data.SimpleTable testValueS, Security.IUser UserInfo)
        {
            DataTable testValue = Data.DataConnect.ResolveToSystemTable(testValueS);
            return ContractSubscriptionService(Category_ID, Message, isHTML, Subject, string.Empty, FileName, FileStreamInfo, testValue, UserInfo);
        }

        public static bool ContractSubscriptionService(Int64 Category_ID, String Message, Boolean isHTML, String Subject, string MailFrom, string FileName, byte[] FileStreamInfo, DataTable testValueS, Security.IUser UserInfo)
        {
            DataTable testValue = testValueS;
            if (_validSubs == null)
            { _validSubs = new System.Collections.Generic.List<long>(); }
            else
            { _validSubs.Clear(); }
            DataTable dtContract = new DataTable();
            DataTable dtUserValue = new DataTable();
            DataTable dtCondition = new DataTable();
            DataTable dtSubs = new DataTable();
            SqlCommand cmm = new SqlCommand();
            Dictionary<Int64, decider> rslt = new System.Collections.Generic.Dictionary<Int64, decider>();

            cmm.CommandType = CommandType.Text;
            cmm.CommandText = "Select fldSubscription_ID, fldSubscriptionName From Email.tblSubscription";
            cmm.CommandText += " Where fldCategory_ID=@Category_ID";
            cmm.Parameters.Add("@Category_ID", SqlDbType.BigInt).Value = Category_ID;
            dtSubs = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.CompanyDB);

            cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;
            cmm.CommandText = "SELECT a.fldField_ID,a.fldCategory_ID,a.fldFieldName,b.fldDataType as fldFieldDataType,a.fldFieldFriendlyName " +
                                " FROM Email.tblCategoryFieldContract a LEFT JOIN Email.tblDataType b ON a.fldDataType_ID = b.fldDataType_ID " +
                                " Where a.fldCategory_ID=@Category_ID";
            cmm.Parameters.Add("@Category_ID", SqlDbType.BigInt).Value = Category_ID;
            dtContract = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.CompanyDB);

            foreach (DataRow drSub in dtSubs.Rows)
            {
                cmm = new SqlCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "Email.spGetFormedCondition";
                cmm.Parameters.Add("@Subscription_ID", SqlDbType.BigInt).Value = Convert.ToInt64(drSub["fldSubscription_ID"]);
                dtCondition = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.CompanyDB);

                if (dtCondition.Rows.Count == 0)
                {
                    _validSubs.Add(Convert.ToInt64(drSub["fldSubscription_ID"]));
                }
                else
                {
                    foreach (DataRow drVal in testValue.Rows)
                    {
                        rslt = new System.Collections.Generic.Dictionary<Int64, decider>();
                        decider temp;
                        long cnt = 1;
                        foreach (DataRow drCond in dtCondition.Rows)
                        {
                            temp = new decider();
                            temp.opr8r = drCond["fldOperator"].ToString();
                            temp.result = RuleValidator(drCond["fldFieldDataType"].ToString(), drCond["fldOperand"].ToString(),
                                drCond["fldValue1"].ToString(), drCond["fldValue2"].ToString(), drVal[drCond["fldFieldFriendlyName"].ToString()].ToString());
                            rslt.Add(cnt++, temp);
                        }
                        if (ResolveCondition(rslt))
                        {
                            _validSubs.Add(Convert.ToInt64(drSub["fldSubscription_ID"]));
                        }
                    }
                }
            }

            foreach (Int64 sub_ID in _validSubs)
            {
                SetAddresses(sub_ID, UserInfo);
                generateMails(Category_ID, Message, isHTML, Subject, SetFromAddress(sub_ID, UserInfo), FileName, FileStreamInfo, UserInfo);
            }
            return true;
        }

        public static bool ContractSubscriptionService(Int64 Category_ID, String Message, Boolean isHTML, String Subject, string MailFrom, DataTable testValueS, Security.IUser UserInfo)
        {
            ContractSubscriptionService(Category_ID, Message, isHTML, Subject, MailFrom, string.Empty, null, testValueS, UserInfo);
            return true;
        }

        public static bool ContractSubscriptionService(Int64 Category_ID, String Message, String Subject, string MailFrom, DataTable testValueS, Security.IUser UserInfo)
        {
            ContractSubscriptionService(Category_ID, Message, false, Subject, MailFrom, string.Empty, null, testValueS, UserInfo);
            return true;
        }

        private static Boolean ResolveCondition(Dictionary<Int64, decider> rslt)
        {
            bool result;
            result = (rslt.Values.First<decider>()).result;
            foreach (decider keyval in rslt.Values)
            {
                if (keyval.opr8r == "AND")
                {
                    result = result && keyval.result;
                }
                else if (keyval.opr8r == "OR")
                {
                    result = result || keyval.result;
                }
            }
            return result;
        }

        public static Data.SimpleTable getContracts(long Category_ID, Security.IUser UserInfo)
        {
            return Data.DataConnect.ResolveToSimpleTable(getContractInfo(Category_ID, UserInfo));
        }

        public static DataTable getContractInfo(long Category_ID, Security.IUser UserInfo)
        {
            DataTable dtContractValue = new DataTable();
            DataTable dtContract = new DataTable();
            SqlCommand cmm = new SqlCommand();
            cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;
            cmm.CommandText = "SELECT a.fldField_ID,a.fldCategory_ID,a.fldFieldName,b.fldDataType as fldFieldDataType,a.fldFieldFriendlyName " +
                                " FROM Email.tblCategoryFieldContract a LEFT JOIN Email.tblDataType b ON a.fldDataType_ID = b.fldDataType_ID " +
                                " Where a.fldCategory_ID=@Category_ID";
            cmm.Parameters.Add("@Category_ID", SqlDbType.BigInt).Value = Category_ID;
            dtContract = Data.DataConnect.FillDt(UserInfo, cmm, Data.DBConnectionType.CompanyDB);

            foreach (DataRow drContract in dtContract.Rows)
            {
                switch (drContract["fldFieldDataType"].ToString())
                {
                    case "String":
                        dtContractValue.Columns.Add(drContract["fldFieldFriendlyName"].ToString(), Type.GetType("System.String"));
                        dtContractValue.Columns[drContract["fldFieldFriendlyName"].ToString()].DefaultValue = String.Empty;
                        break;
                    case "Numeric":
                        dtContractValue.Columns.Add(drContract["fldFieldFriendlyName"].ToString(), Type.GetType("System.Int64"));
                        dtContractValue.Columns[drContract["fldFieldFriendlyName"].ToString()].DefaultValue = -1;
                        break;
                    case "DateTime":
                        dtContractValue.Columns.Add(drContract["fldFieldFriendlyName"].ToString(), Type.GetType("System.DateTime"));
                        dtContractValue.Columns[drContract["fldFieldFriendlyName"].ToString()].DefaultValue = DateTime.Today;
                        break;
                }
            }
            dtContract.AcceptChanges();
            return dtContractValue;
        }

        private struct decider
        {
            public string opr8r;
            public bool result;
        }
    }
}
