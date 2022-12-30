using CommunityToolkit.Mvvm.Messaging.Messages;
using InvoiceAssistant.Logic.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceAssistant.Logic.Messages
{
    public class AddNewInvoiceMessage : ValueChangedMessage<InvoiceItemViewModel>
    {
        public AddNewInvoiceMessage(InvoiceItemViewModel value) : base(value)
        {
        }
    }
}
