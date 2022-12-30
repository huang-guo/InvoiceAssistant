using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InvoiceAssistant.Logic.Messages;
using InvoiceAssistant.Logic.Model;
using InvoiceAssistant.Logic.Utils;
using InvoiceAssistant.Ui.MyControls;
using InvoiceAssistant.Ui.MyWindows;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeAssitant;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace InvoiceAssistant.Logic.ViewModel
{
    public partial class MainViewModel : ObservableRecipient,IRecipient<AddNewInvoiceMessage>
    {
        [ObservableProperty]
        private ObservableCollection<InvoiceItemViewModel> _items=new();

        [ObservableProperty]
        private bool isOpenDialog;

        [ObservableProperty]
        private InvoiceItemViewModel? _selecteItem;

        [RelayCommand]
        public  void GennerateInvoicFile()
        {
            List<KpFpxxFpsjFP> invoices = new(); // 发票数据列表
            List<InvoiceItemViewModel> remove_list = new();
            foreach (InvoiceItemViewModel item in _items)
            {
                var fP = item.GetInvoice();
                
                if (fP!=null)
                {
                    fP.Djh = (_items.IndexOf(item) + 1).ToString();
                    invoices.Add(fP);
                    remove_list.Add(item);
                }
            }
            if (invoices.Count>0)
            {
                Kp kp = new()
                {
                    Fpxx = new KpFpxx()
                    {
                       
                        Zsl = invoices.Count,
                        Fpsj = new KpFpxxFpsj() { Fp = invoices.ToArray() },
                        
                    }
                };
                SaveFileDialog saveFileDialog1 = new()
                {
                    Filter = "Xml 文件|*.xml",
                    Title = "保存发票文件",
                    RestoreDirectory = true,
                    FileName=Path.GetRandomFileName(),
                };
                Stream myStream;
                if (saveFileDialog1.ShowDialog() ==true)
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        // Code to write the stream goes here.
                        XmlSerializer xs = new(typeof(Kp));
                        using (StreamWriter sw = new(myStream, Encoding.GetEncoding("GBK")))
                        {
                            xs.Serialize(sw, kp);
                        }
                        myStream.Close();

                        foreach (var item in remove_list)
                        {
                            _items.Remove(item);
                        }
                    }
                }
            }
        }

        public void Receive(AddNewInvoiceMessage message)
        {
            Items.Add(message.Value);
            SelecteItem = message.Value;
        }

        /// <summary>
        /// 获取发票文件
        /// </summary>
        [RelayCommand]
        private async void GetFiles()
        {
            string[] paths = ExcelHelper.GetExcelFiles("获取发票文件");

            // 打开文件
            foreach (string item in paths)
            {

                var result = await Task.Run(
                    () =>
                        {
                        InvoiceItemViewModel invoiceItemViewModel = new()
                        {
                            Title = Path.GetFileName(item),
                            Buyer = Path.GetFileNameWithoutExtension(item)
                        };
                        invoiceItemViewModel.SetTable(item);
                        return invoiceItemViewModel;
                        }
                        );
                _items.Add(result);
                SelecteItem = result;
            }
        }
       
    }
}