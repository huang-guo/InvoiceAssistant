using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InvoiceAssistant.Logic.Model;
using MaterialDesignThemes.Wpf;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace InvoiceAssistant.Logic.ViewModel
{
    public partial class InputTaxCodeViewModel : ObservableValidator
    {
        public InputTaxCodeViewModel(string productName) {
            ProductName= productName;
        }
        public string ProductName { get; }
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage ="税收分类编码不能为空")]
        private string? code;

        partial void OnCodeChanging(string? value)
        {
            value?.PadRight(19, '0');
        }
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage ="简名不能为空")]
        private string? briefName;
        partial void OnBriefNameChanged(string? value)
        {
            if (value != null)
            {
                using var context = new ProductContext();
                TaxCode? taxCode = context.TaxCodes.FirstOrDefault((item) => item.Name == value);
                if (taxCode != null)
                {
                    Code= taxCode.Code;
                }
            }
        }
        [ObservableProperty]
        private bool? dialogResult;
        public TaxCode? Result { get;private set; }
        [RelayCommand]
        private void Enter()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                return;
            }
            using var context = new ProductContext();
            TaxCode? taxCode = context.TaxCodes.FirstOrDefault((item) => item.Name == briefName);
            Product? product = context.Products.SingleOrDefault(x => x.Name == ProductName);
            
            taxCode ??= new TaxCode { Code = Code?.PadRight(19,'0') ?? string.Empty, Name = briefName ?? string.Empty };
            if (product!=null)
            {
                product.TaxCode = taxCode;
                context.SaveChanges();
            }
            else
            {
                context.Products.Add(new Product
                {
                    Name = ProductName,
                    TaxCode = taxCode
                });
                context.SaveChanges();
            }
            Result = taxCode;
            DialogResult = true;
        }
        [RelayCommand]
        private void Cancle()
        {
            DialogResult = false;
        }

    }
}