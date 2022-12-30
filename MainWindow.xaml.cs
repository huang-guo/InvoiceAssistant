using InvoiceAssistant.Logic.Utils;
using InvoiceAssistant.Logic.ViewModel;
using InvoiceAssistant.Ui.MyWindows;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InvoiceAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var mainViewModel = App.Current.Services.GetService<MainViewModel>();
            if (mainViewModel!=null)
            {
                DataContext = mainViewModel;
                mainViewModel.IsActive = true;
            }
            Logic.Messages.Log.SetTextControl(myctrl);
           

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private  MainViewModel ViewModel => (MainViewModel)DataContext;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] paths = ExcelHelper.GetExcelFiles("获取发票文件");
            
            // 打开文件
            foreach (string item in paths)
            {

                ViewModel.Items.Add(Task.Run(() =>
                {
                    InvoiceItemViewModel invoiceItemViewModel = new()
                    {
                        Title = System.IO.Path.GetFileName(item),
                        Buyer = System.IO.Path.GetFileNameWithoutExtension(item)
                    };
                    invoiceItemViewModel.SetTable(item);
                    return invoiceItemViewModel;
                }).Result);
                //InvoiceItemViewModel invoiceItemViewModel = new()
                //{
                //    Title = System.IO.Path.GetFileName(item),
                //    Buyer = System.IO.Path.GetFileNameWithoutExtension(item)
                //};
                //invoiceItemViewModel.GetTable(item);
                //App.Current.Services.GetService<MainViewModel>()?.Items.Add(invoiceItemViewModel);
            }

        }

        

        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save(); //保存设置
        }


        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new Ui.MyControls.ImportTaxCodeControl()
            {
                DataContext = App.Current.Services.GetService<ImportTaxCodeViewModel>(),
            },"root");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ProductInfoWindow productInfoWindow= new();
            productInfoWindow.Show();
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid dataGrid=(DataGrid)sender;
            foreach (var item in dataGrid.SelectedItems)
            {
                Debug.WriteLine(item);
            }
        }
    }
}
