using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MSLA.Server.Utilities
{
    /// <summary>A Table Helper Class</summary>
    public class TableHelper
    {
        /// <summary>Copies the Columns and Data only from the Source to the Target</summary>
        /// <param name="Source">The Source to copy from</param>
        /// <param name="Target">The Target to copy to</param>
        public static void CopyTableData(DataTable Source, DataTable Target)
        {
            CompareColumns(Source, Target);
            CopyValues(Source, Target);
        }

        private static void CompareColumns(DataTable Source, DataTable Target)
        {
            foreach (DataColumn Col in Source.Columns)
            {
                DataColumn TargetCol = null;
                if (!Target.Columns.Contains(Col.ColumnName))
                {
                    TargetCol = new DataColumn(Col.ColumnName, Col.DataType, Col.Expression);
                    TargetCol.DefaultValue = Col.DefaultValue;
                    Target.Columns.Add(TargetCol);
                }
            }
        }

        private static void CopyValues(DataTable Source, DataTable Target)
        {
            DataRow drNew = null;
            foreach (DataRow dr in Source.Rows)
            {
                drNew = Target.NewRow();
                foreach (DataColumn Col in Source.Columns)
                {
                    drNew[Col.ColumnName] = dr[Col];
                }
                Target.Rows.Add(drNew);
            }
            Target.AcceptChanges();
        }
    }
}
