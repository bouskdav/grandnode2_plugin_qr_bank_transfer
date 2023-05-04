using Grand.Domain.Customers;
using Grand.Domain.Orders;
using Grand.Domain.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.BankTransfer.Infrastructure
{
    public interface IBankTransferMessageProvider
    {
        Task<int> SendQrPaymentMessage(Order order, Customer customer, string languageId);
    }
}
