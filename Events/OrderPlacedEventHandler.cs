using Grand.Business.Core.Events.Checkout.Orders;
using Grand.Business.Core.Interfaces.Common.Directory;
using MediatR;
using Payments.BankTransfer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.BankTransfer.Events
{
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly IBankTransferService _bankTransferService;

        public OrderPlacedEventHandler(
            IBankTransferService bankTransferService)
        {
            _bankTransferService = bankTransferService;
        }

        public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Order.PaymentMethodSystemName == BankTransferPaymentDefaults.ProviderSystemName)
            {
                await _bankTransferService.SendPaymentNotificationWithQRCode(notification.Order.Id);
            }
        }
    }
}
