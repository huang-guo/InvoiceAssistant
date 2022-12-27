using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceAssistant.Logic.Model
{

    [Index(nameof(Name),IsUnique =true)]
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int TaxCodeID { get; set; }
        [Required]
        public virtual TaxCode TaxCode { get; set; } = new();
    }
}
