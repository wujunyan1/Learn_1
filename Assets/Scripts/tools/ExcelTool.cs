using System;
using System.Data;
using System.IO;

using UnityEngine;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace NPOIOprateExcel
{
    public class ExcelUtility
    {
        public static DataTable ExcelToDataTable(string filePath)
        {
            // 输出的数据
            DataTable dataTable = null;
            // 文件流
            FileStream fs = null;
            // 行列
            DataColumn column = null;
            DataRow dataRow = null;

            // 一个excel
            IWorkbook workbook = null;
            // 一个sheet
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(Application.dataPath + "/" + filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                // 第一行是给策划看的，第二行是程序，第三行开始是配置
                                IRow firstRow = sheet.GetRow(1);//第二行  表示配置对应的name
                                int cellCount = firstRow.LastCellNum;//列数  

                                startRow = 2;

                                //构建datatable的列  
                                for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                {
                                    cell = firstRow.GetCell(i);
                                    if (cell != null)
                                    {
                                        if (cell.StringCellValue != null)
                                        {
                                            // 设置列名
                                            column = new DataColumn(cell.StringCellValue);
                                            dataTable.Columns.Add(column);
                                        }
                                    }
                                }
                                
                                //填充行  
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;

                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                fs.Close();
                return dataTable;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }
    }
}