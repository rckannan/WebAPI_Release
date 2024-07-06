using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MSLA.Client.Data
{
    public class C1DataTable
    {
        public static C1.Silverlight.Data.DataTable GetC1DataTable(MSLA.Client.Data.DataTable dt)
        {
            C1.Silverlight.Data.DataTable dtC1 = new C1.Silverlight.Data.DataTable();

            C1.Silverlight.Data.DataColumn dcnew;  

            C1.Silverlight.Data.DataRow drnew;

            foreach (MSLA.Client.Data.DataColumn dc in dt.Columns)
            {
                dcnew = new C1.Silverlight.Data.DataColumn();
                dcnew.AllowDBNull = true;
                dcnew.ColumnName = dc.ColumnName;
                dcnew.DataType = dc.DataType;
                dtC1.Columns.Add(dcnew);
            }

            foreach (MSLA.Client.Data.DataRow dr1 in dt.Rows)
            {
                drnew = dtC1.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dr1[dt.Columns[i].ColumnName] != System.DBNull.Value)
                    {
                        drnew[i] = dr1[dt.Columns[i].ColumnName];
                    }
                }
                dtC1.Rows.Add(drnew);
            }

            dtC1.AcceptChanges();
            return dtC1;
        }

        public static MSLA.Client.Data.DataTable GetMSLADataTable(C1.Silverlight.Data.DataTable C1dt)
        {
            DataTable dt = new DataTable();

            DataColumn dcnew;

            DataRow drnew;

            foreach (C1.Silverlight.Data.DataColumn dc in C1dt.Columns)
            {
                dcnew = new DataColumn();
                dcnew.ColumnName = dc.ColumnName;
                dcnew.DataType = dc.DataType;
                dt.Columns.Add(dcnew);
            }

            foreach (C1.Silverlight.Data.DataRow dr1 in C1dt.Rows)
            {
                drnew = new DataRow();
                for (int i = 0; i < C1dt.Columns.Count; i++)
                {
                    if (dr1[C1dt.Columns[i].ColumnName] != System.DBNull.Value)
                    {
                        drnew.Add(C1dt.Columns[i].ColumnName, dr1[dt.Columns[i].ColumnName]);
                    }
                }
                dt.Rows.Add(drnew);
            }

            return dt;
        }
    }
}
