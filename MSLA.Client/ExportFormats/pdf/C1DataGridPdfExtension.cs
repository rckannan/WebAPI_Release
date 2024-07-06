using C1.Silverlight.DataGrid;
using C1.Silverlight.Pdf;
using System.Windows;
using PdfCreator;
using System;
using System.Linq;
using System.Windows.Media;

namespace MSLA.Client
{
    public static class C1DataGridPdfExtension
    {
        public static C1PdfDocument ToPdf(this C1DataGrid grid)
        {
            return grid.ToPdf(null);
        }

        public static C1PdfDocument ToPdf(this C1DataGrid grid, C1PdfDocument pdf)
        {
            return grid.ToPdf(pdf, new Thickness(75), 2);
        }

        public static C1PdfDocument ToPdf(this C1DataGrid grid, C1PdfDocument pdf, Font fontTitle, Font fontBody)
        {
            return grid.ToPdf(pdf, new Thickness(75), 2, fontTitle, fontBody);
        }

        public static C1PdfDocument ToPdf(this C1DataGrid grid, C1PdfDocument pdf, Thickness pageMargins, double cellsMargins)
        {
            if (pdf == null)
            {
                pdf = new C1PdfDocument();
            }
            Rect rcPage = PdfUtils.PageRectangle(pdf, pageMargins);
            if (rcPage.Width > 0 && rcPage.Height > 0)
            {
                Rect rc = rcPage;

                // title
                if (!string.IsNullOrEmpty(pdf.DocumentInfo.Title))
                {
                    // add title
                    Font titleFont = new Font("Tahoma", 15, PdfFontStyle.Bold);
                    rc = PdfUtils.RenderParagraph(pdf, pdf.DocumentInfo.Title, titleFont, rcPage, rc, false);
                }

                Font cellFont = new Font("Tahoma", 12);
                // body
                double[] columnsWidth, rowsHeight;
                double indentWidth = 8;
                MeasureCells(grid, pdf, ref cellFont, rcPage.Width - (grid.GroupedColumns.Length * indentWidth), cellsMargins, out rowsHeight, out columnsWidth);

                double rowOffset = rcPage.Top + rc.Height + cellsMargins;
                for (int i = 0; i <= grid.VisibleRows.Count; i++)
                {
                    var row = i > 0 ? grid.VisibleRows[i - 1] : null;

                    if (rowOffset + rowsHeight[i] > rcPage.Bottom)
                    {
                        pdf.NewPage();
                        rowOffset = rcPage.Top;
                        if (i > 0)
                        {
                            rcPage = RenderRow(grid, cellsMargins, pdf, rcPage, cellFont, columnsWidth, rowsHeight[0], rowOffset, indentWidth, null);
                            rowOffset += rowsHeight[0];
                        }
                    }
                    // Apply indentation

                    rcPage = RenderRow(grid, cellsMargins, pdf, rcPage, cellFont, columnsWidth, rowsHeight[i], rowOffset, indentWidth, row);
                    rowOffset += rowsHeight[i];
                }
            }
            return pdf;
        }

        public static C1PdfDocument ToPdf(this C1DataGrid grid, C1PdfDocument pdf, Thickness pageMargins, double cellsMargins,Font fontTitle,Font fontBody)
        {
            if (pdf == null)
            {
                pdf = new C1PdfDocument();
            }
            Rect rcPage = PdfUtils.PageRectangle(pdf, pageMargins);
            if (rcPage.Width > 0 && rcPage.Height > 0)
            {
                Rect rc = rcPage;

                // title
                if (!string.IsNullOrEmpty(pdf.DocumentInfo.Title))
                {
                    // add title
                    Font titleFont = fontTitle;
                    rc = PdfUtils.RenderParagraph(pdf, pdf.DocumentInfo.Title, titleFont, rcPage, rc, false);
                }

                Font cellFont = fontBody;
                // body
                double[] columnsWidth, rowsHeight;
                double indentWidth = 8;
                MeasureCells(grid, pdf, ref cellFont, rcPage.Width - (grid.GroupedColumns.Length * indentWidth), cellsMargins, out rowsHeight, out columnsWidth);

                double rowOffset = rcPage.Top + rc.Height + cellsMargins;
                for (int i = 0; i <= grid.VisibleRows.Count; i++)
                {
                    var row = i > 0 ? grid.VisibleRows[i - 1] : null;

                    if (rowOffset + rowsHeight[i] > rcPage.Bottom)
                    {
                        pdf.NewPage();
                        rowOffset = rcPage.Top;
                        if (i > 0)
                        {
                            rcPage = RenderRow(grid, cellsMargins, pdf, rcPage, cellFont, columnsWidth, rowsHeight[0], rowOffset, indentWidth, null);
                            rowOffset += rowsHeight[0];
                        }
                    }
                    // Apply indentation

                    rcPage = RenderRow(grid, cellsMargins, pdf, rcPage, cellFont, columnsWidth, rowsHeight[i], rowOffset, indentWidth, row);
                    rowOffset += rowsHeight[i];
                }
            }
            return pdf;
        }

        private static Rect RenderRow(C1DataGrid grid, double margin, C1PdfDocument pdf, Rect rcPage, Font cellFont, double[] columnsWidth, double rowHeight, double rowOffset, double indentWidth, DataGridRow row)
        {
            var groupRow = row as DataGridGroupRow;
            if (groupRow != null)
            {
                // render group header
                double width = rcPage.Width - (indentWidth * groupRow.Level);
                double left = rcPage.Left + (indentWidth * groupRow.Level);
                Rect rcGroup = new Rect(left, rowOffset, width, rowHeight);
                rcGroup = PdfUtils.Inflate(rcGroup, -margin, -margin);
                pdf.DrawString(groupRow.GetGroupText(), cellFont, Colors.Black, rcGroup);
                pdf.DrawLine(new Pen(Colors.Black, 1), left, rowOffset + rowHeight - 1, rcPage.Right, rowOffset + rowHeight - 1);
            }
            else
            {
                double columnOffset = rcPage.Left;
                for (int j = -1; j < grid.VisibleColumns.Count; j++)
                {
                    if (j >= 0)
                    {
                        // render cell
                        Rect rcCell = new Rect(columnOffset, rowOffset, columnsWidth[j], rowHeight);
                        var column = grid.VisibleColumns[j];
                        string text;
                        if (row == null)
                        {
                            pdf.FillRectangle(Colors.LightGray, rcCell);
                            text = column.GetColumnText();
                        }
                        else
                        {
                            pdf.DrawRectangle(Colors.LightGray, rcCell);
                            text = column.GetCellText(row);
                        }
                        rcCell = PdfUtils.Inflate(rcCell, -margin, -margin);
                        pdf.DrawString(text, cellFont, Colors.Black, rcCell, new StringFormat() { Alignment = column.HorizontalAlignment });
                        columnOffset += columnsWidth[j];
                    }
                    else
                    {
                        //render indent column
                        double indentColumnWidth = (indentWidth * grid.GroupedColumns.Length);
                        Rect rcCell = new Rect(columnOffset, rowOffset, indentColumnWidth, rowHeight);
                        if (row == null)
                        {
                            pdf.FillRectangle(Colors.LightGray, rcCell);
                        }
                        columnOffset += indentColumnWidth;
                    }
                }
            }
            return rcPage;
        }

        private static void MeasureCells(C1DataGrid grid, C1PdfDocument pdf, ref Font cellFont, double availableWidth, double margin, out double[] rowsHeight, out double[] colsWidth)
        {
            rowsHeight = new double[grid.VisibleRows.Count + 1];
            colsWidth = new double[grid.VisibleColumns.Count];
            rowsHeight.Initialize();
            colsWidth.Initialize();
            for (int i = 0; i <= grid.VisibleRows.Count; i++)
            {
                var row = i > 0 ? grid.VisibleRows[i - 1] : null;
                var groupRow = row as DataGridGroupRow;
                if (groupRow != null)
                {
                    var groupSize = pdf.MeasureString(groupRow.GetGroupText(), cellFont);
                    rowsHeight[i] = groupSize.Height + (margin * 2) + (0.4 * cellFont.Size);
                }
                else
                {
                    for (int j = 0; j < grid.VisibleColumns.Count; j++)
                    {
                        var column = grid.VisibleColumns[j];
                        //if is the column header row
                        string text;
                        if (row == null)
                        {
                            text = column.GetColumnText();
                        }
                        else
                        {
                            // gets and measure cell text
                            text = column.GetCellText(row);
                        }
                        var textSize = pdf.MeasureString(text, cellFont);
                        colsWidth[j] = Math.Max(textSize.Width + margin * 2, colsWidth[j]);
                        rowsHeight[i] = Math.Max(textSize.Height + (margin * 2) + (0.4 * cellFont.Size), rowsHeight[i]);
                        if (colsWidth.Sum() > availableWidth)
                        {
                            var newCellFont = ReduceSize(cellFont);
                            if (newCellFont.Size > 0)
                            {
                                cellFont = newCellFont;
                                MeasureCells(grid, pdf, ref cellFont, availableWidth, margin, out rowsHeight, out colsWidth);
                                goto a;
                            }
                        }
                    }
                }
            }
        a: ;
            //shares the remaining available width
            double remainingWidth = availableWidth - colsWidth.Sum();
            if (remainingWidth > 0)
            {
                double additionalWidth = remainingWidth / colsWidth.Length;
                for (int i = 0; i < colsWidth.Length; i++)
                {
                    colsWidth[i] += additionalWidth;
                }
            }
        }

        private static Font ReduceSize(Font cellFont)
        {
            return new Font(cellFont.Name, cellFont.Size - 1);
        }
    }
}
