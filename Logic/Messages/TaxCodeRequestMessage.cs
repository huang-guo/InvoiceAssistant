using CommunityToolkit.Mvvm.Messaging.Messages;
using InvoiceAssistant.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceAssistant.Logic.Messages
{
    class TaxCodeRequestMessage: AsyncRequestMessage<TaxCode?>
    {
        public string? ProductName { get; set; }
    }
}
