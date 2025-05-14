using System;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Collections;

namespace FaceAttendance.Classes
{
    class ExcelFunc
    {
        //public static bool Write_Excel(DataTable td)
        //{
        //    try
        //    {
        //        // creating Excel Application  
        //        Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
        //        // creating new WorkBook within Excel application  
        //        Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
        //        // creating new Excelsheet in workbook  
        //        Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
        //        // see the excel sheet behind the program  
        //        app.Visible = true;
        //        // get the reference of first sheet. By default its name is Sheet1.  
        //        // store its reference to worksheet  
        //        worksheet = workbook.Sheets["Sheet1"];
        //        worksheet = workbook.ActiveSheet;
        //        // changing the name of active sheet  
        //        worksheet.Name = "Exported from gridview";
        //        // storing header part in Excel  
        //        for (int i = 1; i < td.Columns.Count + 1; i++)
        //        {
        //            worksheet.Cells[1, i] = td.Columns[i - 1].ColumnName;
        //            if(i == 3)
        //            {
        //                worksheet.Columns[i].ColumnWidth = 17;

        //            }
        //        }
        //        // storing Each row and column value to excel sheet  
        //        for (int i = 0; i <= td.Rows.Count - 1; i++)
        //        {
        //            for (int j = 0; j < td.Columns.Count; j++)
        //            {
        //                worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
        //            }
        //        }
        //        // save the application  
        //        workbook.SaveAs(Directory.GetCurrentDirectory() + "\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //        // Exit from the application  
        //        //app.Quit();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception has occured while exporting data to Excel :\t" + ex.Message);
        //    }
        //    return false;

        //}
        public static bool Write_Excel(DataTable td)
        {
            Microsoft.Office.Interop.Excel._Application app = null;
            Microsoft.Office.Interop.Excel._Workbook workbook = null;
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                // creating Excel Application  
                app = new Microsoft.Office.Interop.Excel.Application();
                // creating new WorkBook within Excel application  
                workbook = app.Workbooks.Add(Type.Missing);
                // creating new Excelsheet in workbook  
                worksheet = workbook.Sheets[1]; // Access the first sheet by index

                // changing the name of active sheet  
                worksheet.Name = "Exported from gridview";

                // storing header part in Excel  
                for (int i = 1; i < td.Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = td.Columns[i - 1].ColumnName;
                    if (i == 3)
                    {
                        worksheet.Columns[i].ColumnWidth = 17;
                    }
                }

                // storing Each row and column value to excel sheet  
                for (int i = 0; i <= td.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < td.Columns.Count; j++)
                    {
                        if (j == 0)
                        {
                            worksheet.Cells[i + 2, j + 1] = (i + 1).ToString();
                            continue;
                        }
                        worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
                    }
                }

                // Save the workbook
                workbook.SaveAs(Directory.GetCurrentDirectory() + "\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception has occurred while exporting data to Excel :\t" + ex.Message);
                return false;
            }
            finally
            {
                // Release each COM object
                if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                if (workbook != null) workbook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                if (app != null) app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            }
        }
        public static bool Write_Excel(DataTable td, string filePath, out string error)
        {
            error = string.Empty;
            Microsoft.Office.Interop.Excel._Application app = null;
            Microsoft.Office.Interop.Excel._Workbook workbook = null;
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                // creating Excel Application  
                app = new Microsoft.Office.Interop.Excel.Application();
                // creating new WorkBook within Excel application  
                workbook = app.Workbooks.Add(Type.Missing);
                // creating new Excelsheet in workbook  
                worksheet = workbook.Sheets[1]; // Access the first sheet by index

                // changing the name of active sheet  
                worksheet.Name = "Exported from gridview";

                // storing header part in Excel  
                for (int i = 1; i < td.Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = td.Columns[i - 1].ColumnName;
                    if (i == 3)
                    {
                        worksheet.Columns[i].ColumnWidth = 17;
                    }
                }

                // storing Each row and column value to excel sheet  
                for (int i = 0; i <= td.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < td.Columns.Count; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                worksheet.Cells[i + 2, j + 1] = (i + 1).ToString();
                                break;
                            case 7:
                                switch (string.IsNullOrEmpty(td.Rows[i].ItemArray[j].ToString()))
                                {
                                    case true:
                                        worksheet.Cells[i + 2, j + 1] = 0.ToString();
                                        break;
                                    case false:
                                        worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
                                        break;
                                }
                                break;
                            case 8:
                                switch (string.IsNullOrEmpty(td.Rows[i].ItemArray[j].ToString()))
                                {
                                    case true:
                                        worksheet.Cells[i + 2, j + 1] = 0.ToString();
                                        break;
                                    case false:
                                        worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
                                        break;
                                }
                                break;
                            default:
                                worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
                                break;
                        }
                        //if (j == 0)
                        //{
                        //    worksheet.Cells[i + 2, j + 1] = (i + 1).ToString();
                        //    continue;
                        //}
                        //worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
                    }
                }

                // Save the workbook
                //workbook.SaveAs(Directory.GetCurrentDirectory() + "\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workbook.SaveAs(filePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Console.WriteLine("Exception has occurred while exporting data to Excel :\t" + ex.Message);
                return false;
            }
            finally
            {
                // Release each COM object
                if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                if (workbook != null) workbook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                if (app != null) app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            }
        }
        //public static bool Write_ExcelWithImages(DataTable td, string filePath, out string error)
        //{
        //    error = string.Empty;
        //    Microsoft.Office.Interop.Excel._Application app = null;
        //    Microsoft.Office.Interop.Excel._Workbook workbook = null;
        //    Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

        //    try
        //    {
        //        app = new Microsoft.Office.Interop.Excel.Application();
        //        workbook = app.Workbooks.Add(Type.Missing);
        //        worksheet = workbook.Sheets[1];
        //        worksheet.Name = "Exported Data";

        //        // Write column headers
        //        for (int i = 1; i < td.Columns.Count + 1; i++)
        //        {
        //            worksheet.Cells[1, i] = td.Columns[i - 1].ColumnName;

        //            // Adjust column width for clarity
        //            worksheet.Columns[i].ColumnWidth = 15;
        //        }

        //        // Add data to cells
        //        for (int i = 0; i < td.Rows.Count; i++)
        //        {
        //            for (int j = 0; j < td.Columns.Count; j++)
        //            {
        //                if (td.Columns[j].ColumnName.Equals("Photo", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    // Decode the byte array to an image and insert into Excel
        //                    if (td.Rows[i][j] is byte[] photoBytes && photoBytes.Length > 0)
        //                    {
        //                        string tempImagePath = Path.GetTempFileName() + ".jpg";
        //                        File.WriteAllBytes(tempImagePath, photoBytes);

        //                        // Add image to Excel
        //                        var range = worksheet.Cells[i + 2, j + 1];
        //                        Microsoft.Office.Interop.Excel.Range cell = (Microsoft.Office.Interop.Excel.Range)range;
        //                        float left = (float)((double)cell.Left);
        //                        float top = (float)((double)cell.Top);
        //                        worksheet.Shapes.AddPicture(tempImagePath,
        //                            Microsoft.Office.Core.MsoTriState.msoFalse,
        //                            Microsoft.Office.Core.MsoTriState.msoCTrue,
        //                            left, top, 40, 40); // Width and height of the image

        //                        // Optionally delete the temporary image file
        //                        File.Delete(tempImagePath);
        //                    }
        //                }
        //                else
        //                {
        //                    worksheet.Cells[i + 2, j + 1] = td.Rows[i][j]?.ToString();
        //                }
        //            }
        //        }

        //        // Save the Excel file
        //        workbook.SaveAs(filePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //            Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing,
        //            Type.Missing, Type.Missing);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.Message;
        //        return false;
        //    }
        //    finally
        //    {
        //        // Release COM objects
        //        if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
        //        if (workbook != null) workbook.Close(false, Type.Missing, Type.Missing);
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
        //        if (app != null) app.Quit();
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
        //    }
        //}
        public static bool Write_ExcelWithImages(DataTable td, string filePath, out string error)
        {
            error = string.Empty;
            Microsoft.Office.Interop.Excel._Application app = null;
            Microsoft.Office.Interop.Excel._Workbook workbook = null;
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                app = new Microsoft.Office.Interop.Excel.Application();
                workbook = app.Workbooks.Add(Type.Missing);
                worksheet = workbook.Sheets[1];
                worksheet.Name = "Exported Data";

                // Write column headers
                for (int i = 1; i < td.Columns.Count + 1; i++)
                {
                    string colName = td.Columns[i - 1].ColumnName;
                    //worksheet.Cells[1, i] = td.Columns[i - 1].ColumnName;
                    worksheet.Cells[1, i] = colName;
                    switch (colName)
                    {
                        case "SrNo":
                            worksheet.Columns[i].ColumnWidth = 7;
                            break;
                        case "EnrollNumber":
                            worksheet.Columns[i].ColumnWidth = 15;
                            break;
                        case "Employee Name":
                            worksheet.Columns[i].ColumnWidth = 50;
                            break;
                        case "Photo":
                            worksheet.Columns[i].ColumnWidth = 15;
                            break;
                    }
                }

                // Apply center alignment to headers
                Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Rows[1];
                headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                headerRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                // Add data to cells
                for (int i = 0; i < td.Rows.Count; i++)
                {
                    int rowIndex = i + 2; // Row index in Excel (2 because 1 is for headers)
                    for (int j = 0; j < td.Columns.Count; j++)
                    {
                        var cell = worksheet.Cells[rowIndex, j + 1];
                        if (td.Columns[j].ColumnName.Equals("Photo", StringComparison.OrdinalIgnoreCase))
                        {
                            // Decode the byte array to an image and insert into Excel
                            if (td.Rows[i][j] is byte[] photoBytes && photoBytes.Length > 0)
                            {
                                string tempImagePath = Path.GetTempFileName() + ".jpg";
                                File.WriteAllBytes(tempImagePath, photoBytes);

                                // Add image to Excel
                                Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)cell;
                                float left = (float)((double)range.Left);
                                float top = (float)((double)range.Top);

                                // Get cell dimensions
                                float cellWidth = (float)((double)range.Width);
                                float cellHeight = (float)((double)range.Height);

                                float imageWidth = 40; // Width of the image
                                float imageHeight = 40; // Height of the image

                                // Adjust top position to center vertically
                                float imageLeft = left + (cellWidth - imageWidth) / 2;
                                float imageTop = top + (cellHeight - imageHeight) / 2;

                                worksheet.Shapes.AddPicture(tempImagePath,
                                    Microsoft.Office.Core.MsoTriState.msoFalse,
                                    Microsoft.Office.Core.MsoTriState.msoCTrue,
                                    imageLeft, top+3, imageWidth, imageHeight);

                                // Adjust the row height to fit the image
                                worksheet.Rows[rowIndex].RowHeight = Math.Max(imageHeight + 5, 45); // Slight padding for aesthetics

                                // Optionally delete the temporary image file
                                File.Delete(tempImagePath);
                            }
                        }
                        else
                        {
                            cell.Value = td.Rows[i][j]?.ToString();
                        }

                        // Apply center alignment to the cell
                        cell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        cell.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    }
                }

                // Save the Excel file
                workbook.SaveAs(filePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
            finally
            {
                // Release COM objects
                if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                if (workbook != null) workbook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                if (app != null) app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            }
        }
        public static bool Write_Excel_Without_Images(DataTable td, string filePath, out string error)
        {
            error = string.Empty;
            Microsoft.Office.Interop.Excel._Application app = null;
            Microsoft.Office.Interop.Excel._Workbook workbook = null;
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                // creating Excel Application  
                app = new Microsoft.Office.Interop.Excel.Application();
                // creating new WorkBook within Excel application  
                workbook = app.Workbooks.Add(Type.Missing);
                // creating new Excelsheet in workbook  
                worksheet = workbook.Sheets[1]; // Access the first sheet by index

                // changing the name of active sheet  
                worksheet.Name = "Exported from gridview";

                // storing header part in Excel  
                for (int i = 1; i < td.Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = td.Columns[i - 1].ColumnName;
                    if (i == 3)
                    {
                        worksheet.Columns[i].ColumnWidth = 17;
                    }
                }

                // storing Each row and column value to excel sheet  
                for (int i = 0; i <= td.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < td.Columns.Count; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                worksheet.Cells[i + 2, j + 1] = (i + 1).ToString();
                                break;
                            case 7:
                                switch (string.IsNullOrEmpty(td.Rows[i].ItemArray[j].ToString()))
                                {
                                    case true:
                                        worksheet.Cells[i + 2, j + 1] = 0.ToString();
                                        break;
                                    case false:
                                        worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
                                        break;
                                }
                                break;
                            case 8:
                                switch (string.IsNullOrEmpty(td.Rows[i].ItemArray[j].ToString()))
                                {
                                    case true:
                                        worksheet.Cells[i + 2, j + 1] = 0.ToString();
                                        break;
                                    case false:
                                        worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
                                        break;
                                }
                                break;
                            default:
                                worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
                                break;
                        }
                        //if (j == 0)
                        //{
                        //    worksheet.Cells[i + 2, j + 1] = (i + 1).ToString();
                        //    continue;
                        //}
                        //worksheet.Cells[i + 2, j + 1] = td.Rows[i].ItemArray[j].ToString();
                    }
                }

                // Save the workbook
                //workbook.SaveAs(Directory.GetCurrentDirectory() + "\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workbook.SaveAs(filePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Console.WriteLine("Exception has occurred while exporting data to Excel :\t" + ex.Message);
                return false;
            }
            finally
            {
                // Release each COM object
                if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                if (workbook != null) workbook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                if (app != null) app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            }
        }

        public static DataTable ReadExcel(string fileName, string fileExt)
        {
            string conn = string.Empty;
            DataTable dtexcel = new DataTable();
            if (fileExt.CompareTo(".xls") == 0)
            {
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for below excel 2007  
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=Excel 12.0;";
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=NO;\"";
            }
            else
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=NO';"; //for above excel 2007  
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    con.Open();
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Sheet1$]", con); //here we read data from sheet1  
                    oleAdpt.Fill(dtexcel); //fill excel data into dataTable  
                }
                catch (Exception e)
                {

                }
            }
            return dtexcel;
        }



        public static DataTable getExcelFile(string path)
        {
            try
            {
                DataTable dt = new DataTable();

                //Create COM Objects. Create a COM object for everything that is referenced
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(path);
                Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                //DataRow workRow;

                //for (int i = 0; i <= 9; i++)
                //{
                //    workRow = workTable.NewRow();
                //    workRow[0] = i;
                //    workRow[1] = "CustName" + i.ToString();
                //    workTable.Rows.Add(workRow);
                //}
                //iterate over the rows and columns and print to the console as it appears in the file
                //excel is not zero based!!
                //for (int i = 1; i <= rowCount; i++)
                //{
                //    for (int j = 1; j <= colCount; j++)
                //    {
                //        //new line
                //        if (j == 1)
                //            Console.Write("\r\n");

                //        //write the value to the console
                //        if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                //            Console.Write(xlRange.Cells[i, j].Value2.ToString() + "\t");
                //    }
                //}

                for (int i = 1; i < colCount + 1; i++)
                {
                    string col = xlRange.Cells[1, i].Value2.ToString();
                    if(i==6 || i==8)
                    {
                        dt.Columns.Add(col,typeof(string));
                    }
                    else
                    {
                      dt.Columns.Add(col,typeof(int));// = xlRange.Cells[1, i];

                    }
                    //worksheet.Cells[1, i] = td.Columns[i - 1].ColumnName;
                }
                // storing Each row and column value to excel sheet  
                for (int i = 0; i < rowCount - 1; i++)
                {
                    var arlist1 = new ArrayList();

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (xlRange.Cells[i + 2, j + 1].Value2 == null)
                        {
                            //arlist1.Add(0);
                            arlist1.Add("Null");
                        }
                        else
                        {
                            arlist1.Add(xlRange.Cells[i + 2, j + 1].Value);
                        }

                        //xlRange.Cells[i + 2, j + 1] = dt.Rows[i].ItemArray[j].ToString();
                    }
                    dt.Rows.Add(arlist1[0], arlist1[1], arlist1[2], arlist1[3], arlist1[4], arlist1[5], arlist1[6], arlist1[7]);
                }

                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //rule of thumb for releasing com objects:
                //  never use two dots, all COM objects must be referenced and released individually
                //  ex: [somthing].[something].[something] is bad

                //release com objects to fully kill excel process from running in the background
                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(xlWorksheet);

                //close and release
                xlWorkbook.Close();
                Marshal.ReleaseComObject(xlWorkbook);

                //quit and release
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
                
                return dt;

            }
            catch (Exception ex)
            {
                string exPath = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( ImportUsers  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(exPath, message);                
                return null;
            }

        }

    }
}
