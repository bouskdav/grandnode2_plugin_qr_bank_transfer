using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Stores;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Payments.BankTransfer.Domain;
using System.Text.RegularExpressions;

namespace Payments.BankTransfer.Infrastructure
{
    public class BankTransferService : IBankTransferService
    {
        private readonly BankTransferPaymentSettings _bankTransferPaymentSettings;
        private readonly IUserFieldService _userFieldService;
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;

        public BankTransferService(
            BankTransferPaymentSettings bankTransferPaymentSettings,
            IUserFieldService userFieldService,
            IOrderService orderService,
            IStoreService storeService) 
        {
            _bankTransferPaymentSettings = bankTransferPaymentSettings;
            _userFieldService = userFieldService;
            _orderService = orderService;
            _storeService = storeService;
        }

        public async Task<string> GetQrCodeString(Order order)
        {
            string qrCodeString = _bankTransferPaymentSettings.QrCodeStringPattern;

            string iban = _bankTransferPaymentSettings.IBAN;
            string swift = _bankTransferPaymentSettings.SWIFT;
            string variableSymbol = await _userFieldService.GetFieldsForEntity<string>(order, InvoiceConstants.INVOICE_VARIABLE_SYMBOL_FIELD_KEY);

            if (String.IsNullOrEmpty(variableSymbol))
            {
                order = await SetNextAvailableNumberForOrder(order);

                variableSymbol = await _userFieldService.GetFieldsForEntity<string>(order, InvoiceConstants.INVOICE_VARIABLE_SYMBOL_FIELD_KEY);
            }

            var store = await _storeService.GetStoreById(order.StoreId);

            qrCodeString = qrCodeString
                .Replace("{ID}", order.Code)
                .Replace("{SHOP_NAME}", store.Name)
                .Replace("{IBAN}", iban.Replace(" ", ""))
                .Replace("{SWIFT}", swift)
                .Replace("{CURRENCY}", order.CustomerCurrencyCode)
                .Replace("{PRICE}", order.OrderTotal.ToString("0.00").Replace(",", "."))
                .Replace("{VS}", variableSymbol);

            return qrCodeString;
        }

        public async Task<string> GetQrCodeStringByOrderId(string orderId)
        {
            var order = await _orderService.GetOrderById(orderId);

            if (order == null)
            {
                return null;
            }

            return await GetQrCodeString(order);
        }

        public async Task<Order> SetNextAvailableNumberForOrder(Order order)
        {
            string variableSymbol = _bankTransferPaymentSettings.VariableSymbolPattern;

            variableSymbol = variableSymbol.Replace("{yyyy}", order.CreatedOnUtc.ToString("yyyy"));
            variableSymbol = variableSymbol.Replace("{yy}", order.CreatedOnUtc.ToString("yy"));
            variableSymbol = variableSymbol.Replace("{dd}", order.CreatedOnUtc.ToString("dd"));
            variableSymbol = variableSymbol.Replace("{MM}", order.CreatedOnUtc.ToString("MM"));

            Regex orderNumberPatternRegex = new Regex("{(#+)}", RegexOptions.Compiled);
            if (orderNumberPatternRegex.IsMatch(variableSymbol))
            {
                int orderNumber = order.OrderNumber;

                Match match = orderNumberPatternRegex.Match(variableSymbol);
                int length = match.Groups[1].Length;

                if (orderNumber >= Math.Pow(10, length))
                    orderNumber -= (int)Math.Pow(10, length);

                string orderNumberFormatted = orderNumber.ToString(new String('0', length));
                variableSymbol = orderNumberPatternRegex.Replace(variableSymbol, orderNumberFormatted);
            }

            await _userFieldService.SaveField(order, InvoiceConstants.INVOICE_VARIABLE_SYMBOL_FIELD_KEY, variableSymbol);

            return order;
        }


    }
}
