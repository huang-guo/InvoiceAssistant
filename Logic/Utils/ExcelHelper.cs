using ExcelDataReader;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceAssistant.Logic.Utils
{
    static internal class ExcelHelper
    {
        public static DataTable GetTable(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
           
            // Auto-detect format, supports:
            //  - Binary Excel files (2.0-2003 format; *.xls)
            //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
            using var reader = ExcelReaderFactory.CreateReader(stream);
            Debug.WriteLine(reader.RowCount);
            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                // Gets or sets a value indicating whether to set the DataColumn.DataType 
                // property in a second pass.
                UseColumnDataType = true,


                // Gets or sets a callback to obtain configuration options for a DataTable. 
                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                {
                    // Gets or sets a value indicating the prefix of generated column names.
                    EmptyColumnNamePrefix = "列",

                    // Gets or sets a value indicating whether to use a row from the 
                    // data as column names.
                    UseHeaderRow = true,
                    // Gets or sets a callback to determine which row is the header row. 
                    // Only called when UseHeaderRow = true.
                    ReadHeaderRow = (rowReader) => {
                        for (int i = 0; i < rowReader.RowCount; i++)
                        {
                            
                            int nullCount = 0;
                            for (int j = 0; j < rowReader.FieldCount; j++)
                            {
                                if (rowReader.IsDBNull(j)) { nullCount++; }
                            }
                            if (nullCount > rowReader.FieldCount-3)
                            {
                                rowReader.Read();
                            }
                            else
                            {
                                break;
                            }
                        }
                    },

                }
            });
            // The result of each spreadsheet is in result.Tables
            DataTable table=result.Tables[0];
            DataTableHelper.DeleteEmptyColumns(table);
            return table;

        }
        /// <summary>
        /// 获取多个excel文件路径
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <returns>文件路径列表</returns>
        public static string[] GetExcelFiles(string title = "获取execl文件")
        {
            OpenFileDialog dlg = new()
            {
                Title = title,
                Multiselect = true,
                DefaultExt = ".xlsx", // Default file extension
                Filter = "Excel文件 (*.xls,*.xlsx)|*.xls;*.xlsx" // Filter files by extension
            };
            // Show open file dialog box
            dlg.ShowDialog();
            return dlg.FileNames;
        }
        /// <summary>
        /// 获取单个excel文件路径
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <returns>文件路径列表</returns>
        public static string GetExcelFile(string title = "获取execl文件")
        {
            OpenFileDialog dlg = new()
            {
                Title = title,
                Multiselect = false,
                DefaultExt = ".xlsx", // Default file extension
                Filter = "Excel文件 (*.xls,*.xlsx)|*.xls;*.xlsx" // Filter files by extension
            };
            // Show open file dialog box
            dlg.ShowDialog();
            return dlg.FileName;
        }

    }
}
