using InvoiceAssistant.Logic.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace InvoiceAssistant.Ui.MyWindows
{
    /// <summary>
    /// ProductInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProductInfoWindow : Window
    {
        private readonly ProductContext _context =
           new ProductContext();
        private readonly CollectionViewSource productViewSource;
        public ProductInfoWindow()
        {
            InitializeComponent();
            productViewSource = (CollectionViewSource)FindResource(nameof(productViewSource));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // load the entities into EF Core
            _context.Products.Load();

            // bind to the source
            productViewSource.Source =
                _context.Products.Local.ToObservableCollection();
        }
    }
}
