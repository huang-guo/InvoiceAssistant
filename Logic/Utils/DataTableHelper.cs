using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceAssistant.Logic.Utils
{
    internal static class DataTableHelper
    {
        /// <summary>
        /// 删除空列
        /// </summary>
        /// <param name="dataTable"></param>
        public static void DeleteEmptyColumns(DataTable dataTable)
        {
            List<DataColumn> columns = new(); //需要删除的列
            foreach (DataColumn column in dataTable.Columns)
            {
                if (dataTable.Select($"[{column.ColumnName}] is NULL").Length == dataTable.Rows.Count)
                {
                    columns.Add(column);
                }
            }
            foreach (DataColumn column1 in columns)
            {
                dataTable.Columns.Remove(column1);
            }
        }

        /// <summary>
        /// 删除Datable<paramref name="column"/>列下所有为空值的行
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <param name="column">数据列</param>
        public static void DeleteEmptyRows(DataTable dataTable,DataColumn column)
        {
            DataRow[] rows = dataTable.Select($"[{column.ColumnName}] is NULL"); //需要删除的行
            foreach (DataRow row in rows)
            {
                dataTable.Rows.Remove(row);
            }
        }


    }
   
}
