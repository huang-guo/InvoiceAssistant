using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceAssistant.Logic.Validator
{
    internal class Either: ValidationAttribute
    {
        public Either(string propertyName) { 
            PropertyName= propertyName;
        }
        public string PropertyName { get; }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            object? otherValue = instance.GetType().GetProperty(PropertyName)?.GetValue(instance);
            if (otherValue != null || value!=null) {
                return ValidationResult.Success;
            }
            else
            {
                return new(ErrorMessage ?? $"该属性不能和{PropertyName}同时为空");
            }
        }
    }
}
