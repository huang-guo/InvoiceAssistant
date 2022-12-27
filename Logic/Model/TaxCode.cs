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
    public class TaxCode
    {
        public int TaxCodeId { get; set; }
        [Required]
        public string Name { get; set; }=string.Empty;
        [StringLength(19)]
        [Required]
        public string Code { get; set; }=string.Empty;

       
    }
}
