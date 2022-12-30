using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InvoiceAssistant.Logic.Messages;
using InvoiceAssistant.Logic.Model;
using InvoiceAssistant.Logic.Utils;
using InvoiceAssistant.Logic.Validator;
using InvoiceAssistant.Ui.MyControls;
using InvoiceAssistant.Ui.MyWindows;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using OfficeAssitant;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InvoiceAssistant.Logic.ViewModel
{
    public partial class InvoiceItemViewModel : ObservableValidator
    {
        [ObservableProperty]
        private string? _title;

        [RelayCommand]
        private void DeleteItem()
        {
            App.Current.Services.GetService<MainViewModel>()?.Items.Remove(this);
        }

        [ObservableProperty]
        private DataTable excelData=new();

        [ObservableProperty]
        private ReadOnlyCollection<DataColumn>? strColumns;
        [ObservableProperty]
        private ReadOnlyCollection<DataColumn>? doubleColumns;


        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage ="商品全名字段不能为空")]
        private DataColumn? productNameColumn;
        partial void OnProductNameColumnChanged(DataColumn? value)
        {
            if (value != null)
            {
                DataTableHelper.DeleteEmptyRows(ExcelData, value);
                Properties.Settings.Default.ProductColumnName = value.ColumnName;
                value.AllowDBNull= false;
            }
        }
        
        [ObservableProperty]
        private DataColumn? modelColumn;
        partial void OnModelColumnChanged(DataColumn? value)
        {
            Properties.Settings.Default.ModelColumnName= value?.ColumnName;
        }

        [ObservableProperty]
        private DataColumn? unitNameColumn;
        partial void OnUnitNameColumnChanged(DataColumn? value)
        {
            
            Properties.Settings.Default.UnitColumnName = value?.ColumnName;
            
        }
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Either(nameof(AmountColumn),ErrorMessage = "单价与金额字段不能同时为空")]
        private DataColumn? priceColumn;
        partial void OnPriceColumnChanged(DataColumn? value)
        {
            Properties.Settings.Default.PriceColumnName  = value?.ColumnName;
            

        }
        [ObservableProperty]
        private DataColumn? amountColumn;
        partial void OnAmountColumnChanged(DataColumn? value)
        {
                Properties.Settings.Default.AmountColumnName = value?.ColumnName;
          
        }
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage ="数量字段不能为空")]
        private DataColumn? countColumn;

        partial void OnCountColumnChanged(DataColumn? value)
        {
            Properties.Settings.Default.CountColumnName = value?.ColumnName;
            if (value !=null)
            {
                foreach (DataRow row in excelData.Select($"{value.ColumnName} = 0"))
                {
                    excelData.Rows.Remove(row);
                }
                
            }
        }
        [ObservableProperty]
        private string? buyer;
        [ObservableProperty]
        private string? taxID;

        [ObservableProperty]
        private string? addressPhone;

        [ObservableProperty]
        private string? bankAccount;

        [ObservableProperty]
        private string? remarks;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsZeroTaxRate))]
        [NotifyDataErrorInfo]
        [Range(0,0.17,ErrorMessage ="税率范围为0-0.17之间")]
        private double taxRate = Properties.Settings.Default.InvoiceTaxRate;

        partial void OnTaxRateChanged(double value)
        {
            Properties.Settings.Default.InvoiceTaxRate = value;
        }

        public bool IsZeroTaxRate=>taxRate==0.0;
        [ObservableProperty]
        private ReadOnlyDictionary<int,string> zeroTaxRates= new(new Dictionary<int, string>()
        {

            {0,"出口退税" },
            {1,"免税" },
            {2,"不征收" },
            {3,"普通零税率" },
        });


        [ObservableProperty]
        private bool isFullName = Properties.Settings.Default.InvoiceFullName;
        partial void OnIsFullNameChanged(bool value)
        {
           
            Properties.Settings.Default.InvoiceFullName = value;
        }
        /// <summary>
        /// 初始设置数据列名
        /// </summary>
        private void InitColumns()
        {
            ProductNameColumn = strColumns?.FirstOrDefault((item) => item.ColumnName == Properties.Settings.Default.ProductColumnName);
            ModelColumn = strColumns?.FirstOrDefault((item) => item.ColumnName == Properties.Settings.Default.ModelColumnName);
            UnitNameColumn = strColumns?.FirstOrDefault((item) => item.ColumnName == Properties.Settings.Default.UnitColumnName);
            PriceColumn = doubleColumns?.FirstOrDefault((item) => item.ColumnName == Properties.Settings.Default.PriceColumnName);
            AmountColumn = doubleColumns?.FirstOrDefault((item) => item.ColumnName == Properties.Settings.Default.AmountColumnName);
            CountColumn = doubleColumns?.FirstOrDefault((item) => item.ColumnName == Properties.Settings.Default.CountColumnName);
        }
        [ObservableProperty]
        private ObservableCollection<DataRowView> selectedRows=new();
        [RelayCommand]
        private void NewInvoice()
        {
            var list = selectedRows.ToArray();
            InvoiceItemViewModel invoiceItemViewModel = Clone();
            foreach (var item in list)
            {
                invoiceItemViewModel.ExcelData.Rows.Add(item.Row.ItemArray);
                excelData.Rows.Remove(item.Row);
            }
            WeakReferenceMessenger.Default.Send(new AddNewInvoiceMessage(invoiceItemViewModel));
        }
        /// <summary>
        /// 复制一个新的InvoiceItemViewModel
        /// </summary>
        /// <returns></returns>
        public InvoiceItemViewModel Clone()
        {
            return new InvoiceItemViewModel
            {
                ExcelData = excelData.Clone(),
                Title = _title,
                StrColumns=strColumns,
                DoubleColumns=doubleColumns,
                ProductNameColumn = productNameColumn,
                UnitNameColumn = unitNameColumn,
                ModelColumn = modelColumn,
                PriceColumn = priceColumn,
                AmountColumn = amountColumn,
                TaxRate=taxRate,
                CountColumn=countColumn
            };
        }

        /// <summary>
        /// 从excel文件中获取数据表
        /// </summary>
        /// <param name="filename">excel文件路径</param>
        /// <returns></returns>
        public void SetTable(string filename)
        {
            ExcelData = ExcelHelper.GetTable(filename);

            // 设置数据选择列
            List<DataColumn> columns1 = new();
            List<DataColumn> columns2 = new();
            foreach (DataColumn column in excelData.Columns)
            {
                if (column.DataType == typeof(string))
                {
                    columns1.Add(column);
                }
                else if (column.DataType == typeof(double))
                {
                    columns2.Add(column);
                }
            }
            StrColumns = new ReadOnlyCollection<DataColumn>(columns1);
            DoubleColumns = new ReadOnlyCollection<DataColumn>(columns2);
            InitColumns();
        }

        public KpFpxxFpsjFP? GetInvoice()
        {
            ValidateAllProperties();
            if (!HasErrors)
            {
                if (productNameColumn != null && countColumn != null)
                {
                    List<KpFpxxFpsjFPSpxxSph> sphs = new(); //商品行
                    List<KpFpxxFpsjFPSpxxSph> discountLine = new(); //折扣行
                    foreach (DataRow row in excelData.Rows)
                    {
                        // 从DaTaTable中获取商品行数据
                        var name = row[productNameColumn].ToString();

                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }

                        double count = (double)row[countColumn];
                        double price;
                        double amount;
                        if (priceColumn == null && amountColumn != null)
                        {
                            amount = (double)row[amountColumn];
                            price = amount / count;
                        }
                        else if (priceColumn != null)
                        {
                            price = (double)row[priceColumn];
                            amount = price * count;
                        }
                        else
                        {
                            return null;
                        }

                        if (count != 0)
                        {
                            TaxCode? tax = GetTaxCode(name);
                            if (tax is null)
                            {

                                InputTaxCodeViewModel inputTaxCodeViewModel = new(name);
                                new InputTaxCodeWindow() { DataContext = inputTaxCodeViewModel }.ShowDialog();
                                if (inputTaxCodeViewModel.Result is null)
                                {
                                    Log.Warning($"文件'{Title}'中第{excelData.Rows.IndexOf(row) + 1}行没有税收分类编码");
                                    continue;
                                }
                                else
                                {
                                    tax = inputTaxCodeViewModel.Result;
                                }
                            }
                            KpFpxxFpsjFPSpxxSph spxxSph = new()
                            {
                                Xh = excelData.Rows.IndexOf(row) + 1,
                                Spmc = isFullName ? name : tax.Name,
                                Spbm = tax.Code,
                                Ggxh = modelColumn != null ? row[modelColumn].ToString() : "",
                                Slv = taxRate,
                                Jldw = unitNameColumn != null ? row[unitNameColumn].ToString() : "",
                                Dj = price / (1 + taxRate),
                                Sl = count,
                                Je = amount / (1 + taxRate),
                                Se = amount * taxRate / (1 + taxRate),

                                Lslbz = IsZeroTaxRate ? Properties.Settings.Default.InvoiceZeroTaxRateID.ToString() : "",
                            };
                            if (count < 0)
                            {
                                discountLine.Add(spxxSph);
                            }
                            else
                            {
                                sphs.Add(spxxSph);
                            }
                        }


                    }
                    // 添加折扣行
                    foreach (KpFpxxFpsjFPSpxxSph item in discountLine)
                    {
                        int index = sphs.FindIndex(x => x.Spmc == item.Spmc && x.Je + item.Je >= 0);
                        if (index < 0)
                        {
                            Log.Error($"找不到'{_title}'中第{item.Xh}行抵扣行对应的商品行");
                            return null;
                        }
                        else
                        {
                            sphs.Insert(index + 1, item);
                        }
                    }
                    return new KpFpxxFpsjFP()
                    {
                        Bz = remarks ?? "",
                        Gfyhzh = bankAccount ?? "",
                        Gfdzdh = addressPhone ?? "",
                        Gfsh = taxID ?? "",
                        Gfmc = buyer ?? "",
                        Skr = Properties.Settings.Default.InvoicePayee,
                        Fhr = Properties.Settings.Default.InvoiceReviewer,
                        Spbmbbh = Properties.Settings.Default.InvoiceVersion,
                        Spxx = new KpFpxxFpsjFPSpxx() { Sph = sphs.ToArray() },

                    };
                }
            }
            return null;
        }
        /// <summary>
        /// 通过<paramref name="name"/>获取税收分类编码
        /// </summary>
        /// <param name="name">商品名称</param>
        /// <returns></returns>
        private static TaxCode? GetTaxCode(string name)
        {
            using var context = new ProductContext();
            var product = context.Products.SingleOrDefault((item) => item.Name == name);
            if (product!=null)
            {
                return product.TaxCode;
            }
            else
            {
                TaxCode? taxCode = context.TaxCodes.Where(x => name.Contains(x.Name)).OrderBy(x => x.Name.Length).FirstOrDefault();
                if (taxCode!=null)
                {
                    context.Products.Add(new Product { Name = name,TaxCode = taxCode});
                    context.SaveChanges();
                    return taxCode;
                }
                else
                {
                    return null;
                }
            }
        }
       
    }
   
}