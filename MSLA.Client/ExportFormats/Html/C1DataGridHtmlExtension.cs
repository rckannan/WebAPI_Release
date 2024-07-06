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
using C1.Silverlight.DataGrid;
using System.Text;
using System.Windows.Media.Imaging;
namespace MSLA.Client
{
    /// <summary>
    /// Extension class that provides HTML export capabilities to the C1DataGrid.
    /// </summary>
    public static class C1DataGridHtmlExtension
    {
        #region public methods

        public static string ToHtml(this C1DataGrid grid)
        {
            return ToHtml(grid, string.Empty, string.Empty);
        }

        public static string ToHtml(this C1DataGrid grid, string title)
        {
            return ToHtml(grid, title, string.Empty);
        }

        public static string ToHtml(this C1DataGrid grid, string title, string stylesheet)
        {
            string stylesheetToUse = (stylesheet == string.Empty)
                                    ? stylesheetToUse = GenerateDefaultStylesheet(grid)
                                    : stylesheet;

            return Export(grid, title, stylesheetToUse);
        }

        #endregion

        #region Implementation

        const string TITLE_GROUP_BY = "Group By:";
        const string TITLE_GROUP_BY_SEPARATOR = "/";

        private static string Export(C1DataGrid grid, string title, string stylesheetToUse)
        {
            StringBuilder html = new StringBuilder();

            html.AppendLine("<html>");

            // define stylesheet
            html.AppendLine("<head>");
            html.AppendLine("<style type='text/css'>");
            html.AppendLine(stylesheetToUse);
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            // title
            if (!string.IsNullOrEmpty(title))
            {
                html.AppendLine(string.Format("<h3>{0}</h3>", title));
            }

            // create grouping description table
            if (grid.GroupedColumns.Length > 0)
            {
                html.AppendLine("<table>");
                html.AppendLine("<tr>");
                html.AppendLine(string.Format("<td class='{0}'>{1}</td>", GetHeaderGroupingLabelCSSClassName(), TITLE_GROUP_BY));
                for (int i = 0; i < grid.GroupedColumns.Length; i++)
                {
                    var groupCol = grid.GroupedColumns[i];

                    // get column header
                    var colHeader = groupCol.Header.ToString() ?? string.Empty;
                    html.AppendLine(string.Format("<td class='{0}'>{1}</td>", GetHeaderGroupingTextCSSClassName(), colHeader));

                    // separator
                    if (i < grid.GroupedColumns.Length - 1)
                    {
                        html.AppendLine(string.Format("<td class='{0}'>{1}</td>", GetHeaderGroupingSeparatorCSSClassName(), TITLE_GROUP_BY_SEPARATOR));
                    }
                }
                html.AppendLine("</tr>");
                html.AppendLine("</table>");
            }

            // create table for the DataGrid content
            html.AppendLine("<table>");

            // header
            html.AppendLine("<tr>");
            // if it's grouped (add an indent space column before)
            for (int i = 0; i < grid.GroupedColumns.Length; i++)
            {
                html.AppendLine(string.Format("<th class='{0}' />", GetIndentHeaderSpaceCSSClassName()));
            }
            foreach (var column in grid.VisibleColumns)
            {
                html.AppendLine(string.Format("<th class='{0}'>", column.GetHeaderCSSClassName()));
                html.AppendLine(column.GetColumnText());
                html.AppendLine("</th>");
            }
            html.AppendLine("</tr>");

            // body
            html.AppendLine("<tbody>");
            foreach (var row in grid.Rows)
            {
                html.AppendLine("<tr>");

                // Apply indentation
                for (int i = 0; i < row.Level; i++)
                {
                    html.AppendLine("<td></td>");
                }

                var groupRow = row as DataGridGroupRow;
                if (groupRow != null)
                {
                    // append HTML group header
                    html.AppendLine(string.Format("<td colspan='{0}' class='{1}'>", grid.VisibleColumns.Count + grid.GroupedColumns.Length - groupRow.Level, groupRow.Column.GetGroupedCellCSSClassName()));
                    html.AppendLine(groupRow.GetGroupText());
                    html.AppendLine("</td>");
                }
                else
                {
                    foreach (var column in grid.VisibleColumns)
                    {
                        // append HTML portion for the cell
                        html.AppendLine(string.Format("<td class='{0}' >", column.GetCellCSSClassName()));
                        html.AppendLine(column.GetCellText(row));
                        html.AppendLine("</td>");
                    }
                }
                html.AppendLine("</tr>");
            }
            html.AppendLine("</tbody>");

            // close table
            html.AppendLine("</table>");
            html.AppendLine("</body>");

            html.AppendLine("</html>");

            System.Diagnostics.Debug.WriteLine(html.ToString());

            return html.ToString();
        }


        #endregion

        #region Helpers

        // create css class for each column
        private static string GenerateDefaultStylesheet(C1DataGrid grid)
        {
            StringBuilder defStylesheet = new StringBuilder();

            // general styles
            defStylesheet.Append(DefaultCssGeneral);

            // header styles
            foreach (var column in grid.VisibleColumns)
            {
                defStylesheet.AppendLine("." + column.GetHeaderCSSClassName() + "{" + DefaultCssHeader + "}");
            }

            // cell styles
            foreach (var column in grid.VisibleColumns)
            {
                // special handling just for numeric columns
                var cssCell = DefaultCssCellTxt;
                if (column is DataGridNumericColumn) cssCell = DefaultCssCellNum;

                defStylesheet.AppendLine("." + column.GetCellCSSClassName() + "{" + cssCell + "}");
            }

            // grouping styles
            foreach (var column in grid.VisibleColumns)
            {
                defStylesheet.AppendLine("." + column.GetGroupedCellCSSClassName() + "{" + DefaultCssGroup + "}");
            }
            defStylesheet.AppendLine("." + GetIndentHeaderSpaceCSSClassName() + "{" + DefaultCssIndentHeader + "}");
            defStylesheet.AppendLine("." + GetIndentSpaceCSSClassName() + "{" + DefaultCssIndent + "}");
            defStylesheet.AppendLine("." + GetHeaderGroupingLabelCSSClassName() + "{" + DefaultCssGroupInfoLabel + "}");
            defStylesheet.AppendLine("." + GetHeaderGroupingTextCSSClassName() + "{" + DefaultCssGroupInfoText + "}");
            defStylesheet.AppendLine("." + GetHeaderGroupingSeparatorCSSClassName() + "{" + DefaultCssGroupInfoSeparator + "}");


            return defStylesheet.ToString();
        }

        private static string GetHeaderGroupingLabelCSSClassName()
        {
            return "classGroupByLabel";
        }
        private static string GetHeaderGroupingTextCSSClassName()
        {
            return "classGroupByData";
        }
        private static string GetHeaderGroupingSeparatorCSSClassName()
        {
            return "classGroupBySeparator";
        }

        private static string GetIndentHeaderSpaceCSSClassName()
        {
            return "classIndentSpaceHeader";
        }
        private static string GetIndentSpaceCSSClassName()
        {
            return "classIndentSpace";
        }

        private static string GetHeaderCSSClassName(this C1.Silverlight.DataGrid.DataGridColumn column)
        {
            return "classColumnHeader" + column.DisplayIndex.ToString();
        }

        private static string GetCellCSSClassName(this C1.Silverlight.DataGrid.DataGridColumn column)
        {
            return "classColumn" + column.DisplayIndex.ToString();
        }

        private static string GetGroupedCellCSSClassName(this C1.Silverlight.DataGrid.DataGridColumn column)
        {
            return "classColumnGruped" + column.DisplayIndex.ToString();
        }

        #endregion

        #region default styles

        private static string DefaultCssIndentHeader = @"
width: 10px;
background:#D5D5D5;";

        private static string DefaultCssIndent = @"
width: 10px;";

        private static string DefaultCssGroup = @"
font-weight:bold; padding:3px 6px 3px 6px; 
border-bottom: solid 2px #000000;";

        private static string DefaultCssCellNum = @"
text-align:right; 
padding:2px 6px 2px 6px; 
border-right: solid 1px #000000; 
border-left: solid 1px #000000; 
border-bottom: solid 1px #000000;";

        private static string DefaultCssCellTxt = @"
padding:2px 6px 2px 6px; 
border-right: solid 1px #000000; 
border-left: solid 1px #000000; 
border-bottom: solid 1px #000000;";

        private static string DefaultCssHeader = @"
font-size:12px;
font-weight:bold; 
background:#D5D5D5; 
padding:2px 6px 2px 6px;";

        private static string DefaultCssGroupInfoLabel = @"
font-weight:bold; padding: 4px 2px 4px 6px;";

        private static string DefaultCssGroupInfoText = @"
padding:4px 2px 4px 6px
";
        private static string DefaultCssGroupInfoSeparator = @"
padding:4px 2px 4px 6px
";

        private static string DefaultCssGeneral = @"
body{ 
    margin:0; 
    font-family:Arial, Helvetica, sans-serif; 
    font-size:11px; 
    color:#000000; 
    text-align:left; 
}
table{
    border-collapse: collapse; 
    margin:5px;
    font-family:Arial, Helvetica, sans-serif; 
    font-size:11px; 
    color:#000000; 
    text-align:left;
}
table td, table th {
    padding : 0;
}
h1{
    padding:4px 2px 4px 6px
    font-family:Arial, Helvetica, sans-serif; 
    font-size:15px; 
}
";

        #endregion
    }
}
