using Grand.Domain.Orders;

namespace Payments.BankTransfer.Infrastructure
{
    public interface IBankTransferService
    {
        Task<Order> SetNextAvailableNumberForOrder(Order order);

        Task<string> GetQrCodeString(Order order);

        Task<string> GetQrCodeStringByOrderId(string orderId);
    }
}
