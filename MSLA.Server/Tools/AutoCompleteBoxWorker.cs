using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MSLA.Server.Tools
{
    public class AutoCompleteBoxWorker
    {
        public static Dictionary<Int64, String> getResultSet(String collectionMember, String Filter, Data.DBConnectionType cnType,
                  String valueMember, String displayMember, String queryText, Security.IUser UserInfo)
        {
            Dictionary<Int64, String> result = new Dictionary<long, string>();
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;

            if (Filter == String.Empty)
            {
                Filter = displayMember + " like '" + queryText + "%'";
            }
            else
            {
                Filter = "(" + Filter + ") And " + displayMember + " like '" + queryText + "%'";
            }
            cmm.CommandText = cmdBuilder(collectionMember, Filter, valueMember, displayMember);

            DataTable dt = Data.DataConnect.FillDt(UserInfo, cmm, cnType);
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(Convert.ToInt64(dr[valueMember]), dr[displayMember] as String);
            }

            return result;
        }

        public static List<AutoCompleteCollection> getResultSetApi(String collectionMember, String Filter, Data.DBConnectionType cnType,
                  String valueMember, String displayMember, String queryText, Security.IUser UserInfo)
        {
            List<AutoCompleteCollection> result = new List<AutoCompleteCollection>();
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;

            if (Filter == String.Empty)
            {
                Filter = displayMember + " like '" + queryText + "%'";
            }
            else
            {
                Filter = "(" + Filter + ") And " + displayMember + " like '" + queryText + "%'";
            }
            cmm.CommandText = cmdBuilder(collectionMember, Filter, valueMember, displayMember);

            DataTable dt = Data.DataConnect.FillDt(UserInfo, cmm, cnType);
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new AutoCompleteCollection() { fldKey = Convert.ToInt64(dr[valueMember]), fldValue = Convert.ToString(dr[displayMember]) });

                //result.Add(Convert.ToInt64(dr[valueMember]), dr[displayMember] as String);
            }

            return result;
        }

        public static Int64 getValue(String collectionMember, String Filter, Data.DBConnectionType cnType,
                String valueMember, String displayMember, String queryText, Security.IUser UserInfo)
        {
            Int64 result = -1;
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;
            if (Filter == String.Empty)
            {
                Filter = displayMember + " ='" + queryText + "'";
            }
            else
            {
                Filter = "(" + Filter + ") And " + displayMember + " ='" + queryText + "' ";
            }
            cmm.CommandText = cmdBuilder(collectionMember, Filter, valueMember, displayMember);

            DataTable dt = Data.DataConnect.FillDt(UserInfo, cmm, cnType);
            if (dt.Rows.Count > 0)
            {
                result = Convert.ToInt64(dt.Rows[0][valueMember]);
            }
            return result;
        }

        public static String getSelectedText(String collectionMember, String Filter, Data.DBConnectionType cnType,
                    String valueMember, String displayMember, Int64 val, Security.IUser UserInfo)
        {
            String result = String.Empty;
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.Text;
            if (Filter == String.Empty)
            {
                Filter = valueMember + " =" + val.ToString();
            }
            else
            {
                Filter = "(" + Filter + ") And " + valueMember + " =" + val.ToString();
            }
            cmm.CommandText = cmdBuilder(collectionMember, Filter, valueMember, displayMember);

            DataTable dt = Data.DataConnect.FillDt(UserInfo, cmm, cnType);
            if (dt.Rows.Count > 0)
            {
                result = Convert.ToString(dt.Rows[0][displayMember]);
            }
            return result;
        }

        private static String cmdBuilder(String collectionMember, String Filter, String valueMember, String displayMember)
        {
            System.Text.StringBuilder strb = new StringBuilder();
            strb.Append("Select ");
            strb.Append(valueMember);
            strb.Append(" , ");
            strb.Append(displayMember);
            strb.Append(" From ");
            strb.Append(collectionMember);
            strb.Append(" Where ");
            strb.Append(Filter);
            strb.Append(" Order By ");
            strb.Append(displayMember);
            return strb.ToString();
        }

    }

    public class AutoCompleteCollection
    {
        public Int64 fldKey { get; set; }
        public string fldValue { get; set; }
    }
}
