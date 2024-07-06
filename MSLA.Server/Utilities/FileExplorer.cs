using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Microsoft.Office.Interop.Excel;
using ImportFile.Base;
using OfficeOpenXml;

namespace MSLA.Server.Utilities
{
    public class FileExplorer
    {
        public static Dictionary<string, Data.SimpleTable> WriteFile(byte[] FileInfo, string FileExtension, string FilePassword,Security.IUser UserInfo)
        {
            string FilePathName = string.Empty;
            if (FileExtension == ".xls")
            {
                //FilePathName=@"C:\MashreqTemp\MashreqTempXls_"+UserInfo.Session_ID.ToString()+".xls";
                //FileStream fStreamXls = new FileStream(FilePathName, FileMode.Create, FileAccess.ReadWrite);
                //using (fStreamXls)
                //{
                //    fStreamXls.Position = fStreamXls.Length;
                //    fStreamXls.Write(FileInfo, 0, FileInfo.Length);
                //    fStreamXls.Flush();
                //    fStreamXls.Close();
                //}
                return GetExcelSheetNames(FilePathName,FilePassword);
            }
            else if (FileExtension == ".xlsx")
            {
                FilePathName = @"C:\MashreqTemp\MashreqTempXls_" + UserInfo.Session_ID.ToString() + ".xlsx";
                FileStream fStreamXls = new FileStream(FilePathName, FileMode.Create, FileAccess.ReadWrite);
                using (fStreamXls)
                {
                    fStreamXls.Position = fStreamXls.Length;
                    fStreamXls.Write(FileInfo, 0, FileInfo.Length);
                    fStreamXls.Flush();
                    fStreamXls.Close();
                }
                return GetExcelxSheetNames(FilePathName,FilePassword);
            }
            else if (FileExtension == ".csv")
            {
                FilePathName = @"C:\MashreqTemp\MashreqTempCsv" + UserInfo.Session_ID.ToString() + ".csv";
                FileStream fStreamCsv = new FileStream(FilePathName, FileMode.Create, FileAccess.ReadWrite);
                using (fStreamCsv)
                {
                    fStreamCsv.Position = fStreamCsv.Length;
                    fStreamCsv.Write(FileInfo, 0, FileInfo.Length);
                    fStreamCsv.Flush();
                    fStreamCsv.Close();
                }

                Dictionary<string, Data.SimpleTable> TempDict = new Dictionary<string, Data.SimpleTable>();
                OLEDBBase oleb = new OLEDBBase(FilePathName, OLEDBBase.EnImportType.CSV);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = oleb.GetSampleData(oleb.GetTable(0).TableName.ToString(), 1000);
                TempDict.Add(oleb.GetTable(0).TableName.ToString(), Data.DataConnect.ResolveToSimpleTable(dt));
                DeleteFile(FilePathName);
                return TempDict;
            }
            //else if (FileExtension == ".xml")
            //{
            //    FilePathName =@"C:\MashreqTemp\MashreqTempXML"+UserInfo.Session_ID.ToString()+".xml";
            //    FileStream fStreamCsv = new FileStream( FilePathName, FileMode.Create, FileAccess.ReadWrite);
            //    using (fStreamCsv)
            //    {
            //        fStreamCsv.Position = fStreamCsv.Length;
            //        fStreamCsv.Write(FileInfo, 0, FileInfo.Length);
            //        fStreamCsv.Flush();
            //        fStreamCsv.Close();
            //    }
            //    DataSet ds = new DataSet();
            //    Dictionary<string, Data.SimpleTable> TempDict = new Dictionary<string, Data.SimpleTable>();
            //    ds.ReadXml(@"C:\MashreqTemp\MashreqTempXML"+UserInfo.Session_ID.ToString()+".xml");
            //    foreach (System.Data.DataTable dt in ds.Tables)
            //    {
            //        TempDict.Add(dt.TableName.ToString(), Data.DataConnect.ResolveToSimpleTable(dt));
            //    }
            //    DeleteFile(FilePathName);
            //    return TempDict;
            //}
            return null;
        }

        private static Dictionary<string, Data.SimpleTable> GetExcelSheetNames(string FilePathname,string FilePassword)
        {
            Dictionary<string, Data.SimpleTable> SheetInfo = new Dictionary<string, Data.SimpleTable>();
            DataSet ds = new DataSet();

            //Microsoft.Office.Interop.Excel.Application oXL = null;
            //Workbook oWB = null;
            //Worksheet oSheet = null;
            //Range oRng = null;
            //int SheetIndex = 1;
            //try
            //{
            //    //Create a Application object
            //    oXL = new Microsoft.Office.Interop.Excel.Application();
            //    //Getting WorkBook object
            //    oWB = oXL.Workbooks.Open(FilePathname, 0, false, 5, FilePassword, "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            //    //Getting WorkSheet object
               
            //    while (SheetIndex <= oWB.Sheets.Count)
            //    {
            //        oSheet = (Microsoft.Office.Interop.Excel.Worksheet)oWB.Sheets[SheetIndex];
            //        System.Data.DataTable dt = new System.Data.DataTable(oSheet.Name);

            //        ds.Tables.Add(dt);
            //        DataRow dr;
            //        int jValue = oSheet.UsedRange.Cells.Columns.Count;
            //        int iValue = oSheet.UsedRange.Cells.Rows.Count;

            //        //Getting Data Columns
            //        for (int j = 1; j <= jValue; j++)
            //        {
            //            dt.Columns.Add(CreateExcelColumnName(j), System.Type.GetType("System.String"));
            //        }

            //        //Getting Data in Cell
            //        for (int i = 1; i <= iValue; i++)
            //        {
            //            dr = ds.Tables[oSheet.Name].NewRow();
            //            for (int j = 1; j <= ds.Tables[oSheet.Name].Columns.Count; j++)
            //            {
            //                oRng = (Microsoft.Office.Interop.Excel.Range)oSheet.Cells[i, j];
            //                string strValue = string.Empty;
            //                if (oRng.Value == null)
            //                {
            //                    strValue = string.Empty;
            //                }
            //                else
            //                {
            //                    strValue = oRng.Value.ToString();
            //                }
            //                dr[CreateExcelColumnName(j)] = strValue;
            //            }
            //            ds.Tables[oSheet.Name].Rows.Add(dr);
            //        }
            //        SheetIndex = SheetIndex + 1;
            //    }
            //    oWB.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
            //}
            //catch (Exception ex)
            //{
            //    Exceptions.ServiceExceptionHandler.HandleException(-1, "", ex);
            //}
            //finally
            //{
            //    //Release the Excel objects
            //    oXL.Workbooks.Close();
            //    oXL.Quit();
            //    oXL = null;
            //    oWB = null;
            //    oSheet = null;
            //    oRng = null;
            //    DeleteFile(FilePathname);
            //    //GC.GetTotalMemory(false);
            //    //GC.Collect();
            //    //GC.WaitForPendingFinalizers();
            //    //GC.Collect();
            //    //GC.GetTotalMemory(true);
            //}

            foreach (System.Data.DataTable dt in ds.Tables)
            {
                SheetInfo.Add(dt.ToString(), Data.DataConnect.ResolveToSimpleTable(dt));
            }
            return SheetInfo;
        }

        private static string CreateExcelColumnName(int num)
        {
            const int A = 65; //ASCII value for capital A
            string sCol = string.Empty;
            int iRemain = 0;
            // THIS ALGORITHM ONLY WORKS UP TO ZZ. It fails on AAA
            if (num > 701)
            {
                return string.Empty;
            }
            if (num <= 26)
            {
                if (num == 0)
                {
                    sCol = Convert.ToChar((A + 26) - 1).ToString(); ;
                }
                else
                {
                    sCol = Convert.ToChar((A + num) - 1).ToString();
                }
            }
            else
            {
                iRemain = ((num / 26)) - 1;
                if ((num % 26) == 0)
                {
                    sCol = CreateExcelColumnName(iRemain) + CreateExcelColumnName(num % 26);
                }
                else
                {
                    sCol = Convert.ToChar(A + iRemain) + CreateExcelColumnName(num % 26);
                }
            }
            return sCol;

        }

        private static Dictionary<string, Data.SimpleTable> GetExcelxSheetNames(string FilePathname, string FilePassword)
        {
            Dictionary<string, Data.SimpleTable> SheetInfo = new Dictionary<string, Data.SimpleTable>();
            FileInfo FI;
            ExcelPackage xlPackage;
            DataSet dsMain;
            DataSet ds;
            System.IO.TextReader TextReader;
            try
            {
                FI = new FileInfo(FilePathname);
               FI.SetAccessControl( new System.Security.AccessControl.FileSecurity(FilePathname, System.Security.AccessControl.AccessControlSections.All));
                using (xlPackage = new ExcelPackage(FI))
                {                    
                    dsMain = new DataSet();
                    ds = new DataSet();
                    int SheetIndex = 1;
                    while (SheetIndex <= xlPackage.Workbook.Worksheets.Count)
                    {
                        // get the worksheet in the workbook
                        ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[SheetIndex];
                        TextReader = new System.IO.StringReader(worksheet.WorksheetXml.InnerXml);
                        dsMain.ReadXml(TextReader);

                        System.Data.DataTable dt = new System.Data.DataTable(worksheet.Name);
                        ds.Tables.Add(dt);

                        //Getting No of Rows And Columns
                        System.Data.DataTable dtColumns = dsMain.Tables["c"].Clone();
                        foreach (DataColumn dc in dtColumns.Columns)
                        {
                            if (dc.ColumnName == "colNumber")
                            {
                                if (dc.DataType == typeof(System.String))
                                {
                                    dc.DataType = typeof(System.Int64);
                                }
                            }

                        }
                        foreach (DataRow drTemp in dsMain.Tables["c"].Rows)
                        {
                            dtColumns.ImportRow(drTemp);
                        }

                        System.Data.DataTable dtRows = dsMain.Tables["row"].Clone();
                        foreach (DataColumn dc in dtRows.Columns)
                        {
                            if (dc.ColumnName == "r")
                            {
                                if (dc.DataType == typeof(System.String))
                                {
                                    dc.DataType = typeof(System.Int64);
                                }
                            }

                        }
                        foreach (DataRow drTemp in dsMain.Tables["row"].Rows)
                        {
                            dtRows.ImportRow(drTemp);
                        }

                        Int64 NoOfColumns = (Int64)dtColumns.Compute("MAX(colNumber)", "");
                        Int64 NoOfRows = (Int64)dtRows.Compute("MAX(r)", "");

                        //Create Columns
                        for (int i = 1; i <= NoOfColumns; i++)
                        {
                            dt.Columns.Add(CreateExcelColumnName(i), System.Type.GetType("System.String"));
                        }

                        //Read each cell value and insert into the system table
                        DataRow dr;
                        for (int iValue = 1; iValue <= NoOfRows; iValue++)
                        {
                            //Getting Data in Cell
                            dr = ds.Tables[worksheet.Name].NewRow();
                            for (int jValue = 1; jValue <= ds.Tables[worksheet.Name].Columns.Count; jValue++)
                            {
                                string strValue = string.Empty;
                                if (worksheet.Cell(iValue, jValue).Value == null)
                                {
                                    strValue = string.Empty;
                                }
                                else
                                {
                                    strValue = worksheet.Cell(iValue, jValue).Value;
                                }

                                dr[CreateExcelColumnName(jValue)] = strValue;
                            }
                            ds.Tables[worksheet.Name].Rows.Add(dr);
                        }
                        SheetIndex = SheetIndex + 1;
                    }
                }

                foreach (System.Data.DataTable dt in ds.Tables)
                {
                    SheetInfo.Add(dt.ToString(), Data.DataConnect.ResolveToSimpleTable(dt));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error" + ex.Message);
            }
            finally
            {
                FI = null;
                xlPackage = null;
                dsMain = null;
                ds = null;
                TextReader = null;
            }
            return SheetInfo;
        }

        private static bool DeleteFile(string filepathname)
        {
            File.Delete(filepathname);
            return true;
        }

    }
}
