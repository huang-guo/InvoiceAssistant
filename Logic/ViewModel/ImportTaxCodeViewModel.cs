using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InvoiceAssistant.Logic.Model;
using InvoiceAssistant.Logic.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace InvoiceAssistant.Logic.ViewModel
{
    public partial class ImportTaxCodeViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> errorMessages= new();

        [ObservableProperty]
        private bool allowUpdate= true;
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UploadCommand))]
        private string? fileName;

        partial void OnFileNameChanged(string? value)
        {
            errorMessages.Clear();
        }

        /// <summary>
        /// 下载模板命令
        /// </summary>
        [RelayCommand]
        private void DownloadModel()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Excel 文件|*.xlsx",
                Title = "导出模板文件",
                FileName = "税收分类编码导入模板"
            };
            Stream myStream;
            if (saveFileDialog.ShowDialog() == true)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    
                    myStream.Write(Properties.Resources.导入发票信息模板文件, 0, Properties.Resources.导入发票信息模板文件.Length);
                    myStream.Close();

                    // 用系统默认程序打开文件
                    using Process p = new();
                    p.StartInfo = new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        UseShellExecute = true,
                        FileName = saveFileDialog.FileName
                    };
                    p.Start();

                    // 修改导入文件名
                    FileName= saveFileDialog.FileName;
                }
                
            }
        }
        /// <summary>
        /// 选择目的文件
        /// </summary>
        [RelayCommand]
        private void ChoiceFile() {

            var filename = ExcelHelper.GetExcelFile("选择文件");
            if (filename!=null)
            {
                FileName = filename;
            }
        }

        

        /// <summary>
        /// 上传数据
        /// </summary>
        [RelayCommand(CanExecute =nameof(CanUpload))]
        private void Upload()
        {
            if (fileName!=null)
            {
                DataTable data= ExcelHelper.GetTable(fileName);
                if (data.Rows.Count==0)
                {
                    errorMessages.Add($"{fileName}中数据列为空");
                    return;
                }else{
                    // 检查列名
                    foreach (var str in new string[] { "商品全名", "简名", "税收分类编码" })
                    {
                        if (!data.Columns.Contains(str))
                        {
                            errorMessages.Add($"{fileName}中不包含'{str}'列");
                            return;
                        }
                    }
                    using var context = new ProductContext();
                    foreach (DataRow row in data.Rows)
                    {
                        // 获取数据
                        string? name = row["商品全名"].ToString();
                        string? code = row["税收分类编码"].ToString()?.PadRight(19,'0');
                        string? breifName = row["简名"].ToString();

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(breifName))
                        {
                            // 添加数据
                            TaxCode taxCode = context.TaxCodes.FirstOrDefault(x => x.Name == breifName) ?? new TaxCode()
                            {
                                Name = breifName,
                                Code = code,
                            };
                            if (!context.Products.Any(x=>x.Name==name))
                            {
                                context.Products.Add(new Product()
                                {
                                    Name = name,
                                    TaxCode = taxCode
                                });
                            }
                            else if (allowUpdate)
                            {
                                taxCode.Code=code;
                                context.Products.Single(x=>x.Name== name).TaxCode=taxCode;
                                
                            }
                            try
                            {
                                context.SaveChanges();
                            }
                            catch (DbUpdateException ex)
                            {
                                Debug.WriteLine(ex.InnerException?.Message);
                            }
                        }
                    }
                   
                }
            }
            FileName = null;
        }

        private bool CanUpload =>!string.IsNullOrEmpty(fileName);
        
    }
}