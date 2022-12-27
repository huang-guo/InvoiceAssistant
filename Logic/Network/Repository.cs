using InvoiceAssistant.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceAssistant.Logic.Network
{
    public static class Repository
    {
        public static void AddTaxCode(string name,string code,string shortName)
        {

            using var context = new ProductContext();
            TaxCode? taxCode = context.TaxCodes.FirstOrDefault((item) => item.Name == shortName);
            if (taxCode == null)
            {
                taxCode = new TaxCode { Name = shortName, Code = code.PadRight(19, '0') };
                context.TaxCodes.Add(taxCode);
            }
            context.Products.Add(new Product
            {
                Name = name,
                TaxCode = taxCode,
            });
            context.SaveChanges();
        }
    }
}
