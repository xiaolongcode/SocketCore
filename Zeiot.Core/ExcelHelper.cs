using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;

namespace Zeiot.Core
{
    /// <summary>
    /// 基于EPPlus实现的Excel操作
    /// </summary>
    public class ExcelHelper
    {

        public static string ExcelContentType
        {
            get
            {
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
        }

        /// <summary>
        /// List转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel(string filepath)
        {
            try
            {
                FileInfo file = new FileInfo(filepath);
                DataTable dataTable = new DataTable();
                if (file != null)
                {

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (ExcelPackage package = new ExcelPackage(file))
                    {

                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        //获取表格的列数和行数
                        int rowCount = worksheet.Dimension.Rows;
                        int ColCount = worksheet.Dimension.Columns;

                        string rowCounts = rowCount.ToString();
                        string ColCounts = ColCount.ToString();

                        for (int i = 1; i <= ColCount; i++)
                        {
                            string colname = "";
                            if (worksheet.Cells[1, i].Value != null)
                                colname = worksheet.Cells[1, i].Value.ToString();
                            dataTable.Columns.Add(colname);
                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            DataRow r = dataTable.NewRow();
                            for (int col = 1; col <= ColCount; col++)
                            {
                                if (worksheet.Cells[row, col].Value == null)
                                {
                                    r[col - 1] = "";
                                }
                                else
                                {
                                    r[col - 1] = worksheet.Cells[row, col].Value.ToString();
                                }
                            }
                            dataTable.Rows.Add(r);
                        }
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return null;
        }
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel(Stream stream)
        {
            try
            {
                DataTable dataTable = new DataTable();
                if (stream != null&& stream.Length>1)
                {

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {

                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        //获取表格的列数和行数
                        int rowCount = worksheet.Dimension.Rows;
                        int ColCount = worksheet.Dimension.Columns;

                        string rowCounts = rowCount.ToString();
                        string ColCounts = ColCount.ToString();

                        for (int i = 1; i <= ColCount; i++)
                        {
                            string colname = "";
                            if (worksheet.Cells[1, i].Value != null)
                                colname = worksheet.Cells[1, i].Value.ToString();
                            dataTable.Columns.Add(colname);
                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            DataRow r = dataTable.NewRow();
                            for (int col = 1; col <= ColCount; col++)
                            {
                                if (worksheet.Cells[row, col].Value == null)
                                {
                                    r[col - 1] = "";
                                }
                                else
                                {
                                    r[col - 1] = worksheet.Cells[row, col].Value.ToString();
                                }
                            }
                            dataTable.Rows.Add(r);
                        }
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dataTable">数据源</param>
        /// <param name="heading">工作簿Worksheet</param>
        /// <param name="showSrNo">//是否显示行编号</param>
        /// <param name="columnsToTake">要导出的列</param>
        /// <returns></returns>
        public static byte[] ExportExcel<T>(List<T> data,  string heading = "", bool showSrNo = false, params string[] columnsToTake)
        {
            DataTable dataTable = ListToDataTable<T>(data);
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(string.Format("{0}Data", heading));
                int startRowFrom = string.IsNullOrEmpty(heading) ? 1 : 3;  //开始的行
                //是否显示行编号
                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }

                //Add Content Into the Excel File
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
                // autofit width of cells with small content  
                int columnIndex = 1;
                foreach (DataColumn item in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                    if (maxLength < 150)
                    {
                        workSheet.Column(columnIndex).AutoFit();
                    }
                    columnIndex++;
                }
                // format header - bold, yellow on black  
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
                }

                // format cells - add borders  
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // removed ignored columns  
                for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                {
                    if (i == 0 && showSrNo)
                    {
                        continue;
                    }
                    if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                    {
                        workSheet.DeleteColumn(i + 1);
                    }
                }

                if (!String.IsNullOrEmpty(heading))
                {
                    workSheet.Cells["A1"].Value = heading;
                    workSheet.Cells["A1"].Style.Font.Size = 20;

                    workSheet.InsertColumn(1, 1);
                    workSheet.InsertRow(1, 1);
                    workSheet.Column(1).Width = 5;
                }

                result = package.GetAsByteArray();

            }
            return result;
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="Filepoth">要导出的文件路径</param>
        /// <param name="dataTable">数据源</param>
        /// <param name="heading">工作簿Worksheet</param>
        /// <param name="showSrNo">//是否显示行编号</param>
        /// <param name="columnsToTake">要导出的列</param>
        /// <returns></returns>
        public static bool ExportExcelUrl<T>(string Filepoth, List<T> data, string heading = "", bool showSrNo = false, params string[] columnsToTake)
        {
            try
            {
                DataTable dataTable = ListToDataTable<T>(data);
                FileInfo file = new FileInfo(Filepoth);
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Filepoth);
                }
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(string.Format("{0}Data", heading));
                    int startRowFrom = string.IsNullOrEmpty(heading) ? 1 : 3;  //开始的行
                                                                               //是否显示行编号
                    if (showSrNo)
                    {
                        DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                        dataColumn.SetOrdinal(0);
                        int index = 1;
                        foreach (DataRow item in dataTable.Rows)
                        {
                            item[0] = index;
                            index++;
                        }
                    }

                    //Add Content Into the Excel File
                    workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
                    // autofit width of cells with small content  
                    int columnIndex = 1;
                    foreach (DataColumn item in dataTable.Columns)
                    {
                        ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                        int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                        if (maxLength < 150)
                        {
                            workSheet.Column(columnIndex).AutoFit();
                        }
                        columnIndex++;
                    }
                    // format header - bold, yellow on black  
                    using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                    {
                        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        r.Style.Font.Bold = true;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
                    }

                    // format cells - add borders  
                    using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    }

                    // removed ignored columns  
                    for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                    {
                        if (i == 0 && showSrNo)
                        {
                            continue;
                        }
                        if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                        {
                            workSheet.DeleteColumn(i + 1);
                        }
                    }

                    if (!String.IsNullOrEmpty(heading))
                    {
                        workSheet.Cells["A1"].Value = heading;
                        workSheet.Cells["A1"].Style.Font.Size = 20;

                        workSheet.InsertColumn(1, 1);
                        workSheet.InsertRow(1, 1);
                        workSheet.Column(1).Width = 5;
                    }

                    package.Save();

                }

                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}
