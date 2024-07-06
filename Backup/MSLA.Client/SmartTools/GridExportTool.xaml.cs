using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using C1.Silverlight;
using C1.Silverlight.DataGrid;
using C1.Silverlight.Pdf;

namespace MSLA.Client.SmartTools
{
    public partial class GridExportTool : UserControl
    {
        private C1DataGrid _dgv;
        private String _title;
        private Font _titleFont = new Font("Tahoma", 15, PdfFontStyle.Bold);
        private Font _bodyFont = new Font("Tahoma", 12, PdfFontStyle.Regular);

        public GridExportTool()
        {
            InitializeComponent();
            if (!MSLA.Client.Login.LogonInfo.ExportAllowed)
            {
                this.IsEnabled = false;
            }
        }

        public C1DataGrid C1DataGrid
        {
            set
            {
                _dgv = value;
                if (!MSLA.Client.Login.LogonInfo.ExportAllowed)
                {
                    _dgv.SelectionMode = C1.Silverlight.DataGrid.DataGridSelectionMode.None;
                    _dgv.IsReadOnly = true;
                }
            }
            get { return _dgv; }
        }

        public String Title
        {
            set { _title = value; }
            get { return _title; }
        }

        public Font titleFont
        {
            get { return _titleFont; }
            set { _titleFont = value; }
        }

        public Font bodyFont
        {
            get { return _bodyFont; }
            set { _bodyFont = value; }
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();

                dialog.DefaultExt = "*.xml";
                dialog.Filter = "Excel Xml (*.xml)|*.xml|All files (*.*)|*.*";

                bool? DailogResult = dialog.ShowDialog();

                using (var sw = new StreamWriter(dialog.OpenFile()))
                {
                    var excelContent = GetExcelFile(true, false, true);
                    excelContent.Save(sw);
                }
            }
            catch (Exception ex)
            {
                Exceptions.ExceptionHandler.HandleException(ex, "Excel Click Error");
            }
        }


        private XDocument GetExcelFile(bool generateColumnTitles, bool setRowHeight, bool setColumnWidth)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string excelTemplateResourcestring = assembly.GetManifestResourceNames().First((resourceName) => resourceName.Contains(".Resources.ExcelTemplate.xml"));
            XDocument xml = XDocument.Load(assembly.GetManifestResourceStream(excelTemplateResourcestring));
            var table = (from element in xml.Descendants("{urn:schemas-microsoft-com:office:spreadsheet}Worksheet")
                         where element.Attribute("{urn:schemas-microsoft-com:office:spreadsheet}Name").Value == "Sheet1"
                         select element.Element("{urn:schemas-microsoft-com:office:spreadsheet}Table")).First();

            //table settings
            table.SetAttributeValue("{urn:schemas-microsoft-com:office:spreadsheet}ExpandedColumnCount", _dgv.VisibleColumns.Count);
            table.SetAttributeValue("{urn:schemas-microsoft-com:office:spreadsheet}ExpandedRowCount", _dgv.VisibleRows.Count + (generateColumnTitles ? 1 : 0));
            if (_dgv.RowHeight.IsAbsolute && setRowHeight)
            {
                table.SetAttributeValue("{urn:schemas-microsoft-com:office:spreadsheet}DefaultRowHeight", _dgv.RowHeight.Value);
            }

            //add the rows
            for (int i = generateColumnTitles ? -1 : 0; i < _dgv.VisibleRows.Count; i++)
            {
                C1.Silverlight.DataGrid.DataGridRow row = null;
                if (i >= 0)
                {
                    row = _dgv.VisibleRows[i];
                }
                XElement rowElement = new XElement("{urn:schemas-microsoft-com:office:spreadsheet}Row");
                bool isFirstColumn = true;
                //add the columns
                for (int j = 0; j < _dgv.VisibleColumns.Count; j++)
                {
                    C1.Silverlight.DataGrid.DataGridColumn column = _dgv.VisibleColumns[j];
                    //If is the row that contains the column titles
                    if (i == -1)
                    {
                        string columnTitle = column.GetColumnText();
                        XElement columnElement = new XElement("{urn:schemas-microsoft-com:office:spreadsheet}Column");
                        columnElement.SetAttributeValue("{urn:schemas-microsoft-com:office:spreadsheet}AutoFitWidth", column.Width.IsAbsolute ? 0 : 1);
                        if (column.Width.IsAbsolute && setColumnWidth)
                        {
                            columnElement.SetAttributeValue("{urn:schemas-microsoft-com:office:spreadsheet}Width", column.Width.Value);
                        }
                        table.Add(columnElement);
                        XElement titleElement = new XElement("{urn:schemas-microsoft-com:office:spreadsheet}Cell");
                        XElement titleDataElement = new XElement("{urn:schemas-microsoft-com:office:spreadsheet}Data", columnTitle);
                        titleDataElement.SetAttributeValue("{urn:schemas-microsoft-com:office:spreadsheet}Type", "String");
                        titleElement.Add(titleDataElement);
                        rowElement.Add(titleElement);
                    }
                    else
                    {
                        if (row.Type == DataGridRowType.Group)
                        {
                            if (isFirstColumn)
                            {
                                //output.Append(GetGroupText((DataGridGroupRow)row));
                            }
                        }
                        else
                        {
                            XElement cellElement = new XElement("{urn:schemas-microsoft-com:office:spreadsheet}Cell");
                            var cell = _dgv[row, column];
                            if (cell != null)
                            {
                                XElement cellDataElement = new XElement("{urn:schemas-microsoft-com:office:spreadsheet}Data", cell.Text);
                                cellDataElement.SetAttributeValue("{urn:schemas-microsoft-com:office:spreadsheet}Type", GetColumnExcelType(column));
                                cellElement.Add(cellDataElement);
                            }
                            rowElement.Add(cellElement);
                        }
                    }
                    isFirstColumn = false;
                }
                table.Add(rowElement);
            }
            return xml;
        }

        private static string GetColumnExcelType(C1.Silverlight.DataGrid.DataGridColumn column)
        {
            //if (column is DataGridNumericColumn)
            //{
            //    return "Number";
            //}
            return "String";
        }

        private void btnPdf_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var dialog = new SaveFileDialog();

                dialog.DefaultExt = "*.pdf";
                dialog.Filter = "Portable document format (*.pdf)|*.pdf|All files (*.*)|*.*";

                if (dialog.ShowDialog() == false) return;

                using (var s = dialog.OpenFile())
                {
                    var pdfDoc = new C1PdfDocument();
                    pdfDoc.DocumentInfo.Title = _title;
                    //pdfDoc.PageSize = new Size(100, 100);
                    //pdfDoc.PageRectangle = new Rect(10, 10, 80, 80);
                    _dgv.ToPdf(pdfDoc,titleFont,bodyFont);
                    pdfDoc.Save(s);
                }
            }
            catch (Exception ex)
            {
                Exceptions.ExceptionHandler.HandleException(ex, "Pdf Click Error");
            }
        }

        private void btnHtml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog();

                dialog.DefaultExt = "*.html";
                dialog.Filter = "Web Page (*.html)|*.html|All files (*.*)|*.*";

                if (dialog.ShowDialog() == false) return;

                using (var sw = new StreamWriter(dialog.OpenFile()))
                {
                    var htmlContent = _dgv.ToHtml(_title);
                    sw.Write(htmlContent);
                }
            }
            catch (Exception ex)
            {
                Exceptions.ExceptionHandler.HandleException(ex, "Html Click Error");
            }
        }

        private void btnHtmlFile_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // uses extension methods declared in C1DataGridHtmlExtension 
                // to convert the C1DataGrid to html
                var html = _dgv.ToHtml(_title);
                PrintHelper.Print("Preview", html, true);
            }
            catch (Exception ex)
            {
                Exceptions.ExceptionHandler.HandleException(ex, "Print Click Error");
            }
        }
    }
}
