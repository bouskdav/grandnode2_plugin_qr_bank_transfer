using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Payments.BankTransfer
{
    public partial class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Plugin.PaymentBankTransfer",
                 "Plugins/PaymentBankTransfer/PaymentInfo",
                 new { controller = "PaymentBankTransfer", action = "PaymentInfo", area = "" }
            );

            endpointRouteBuilder.MapControllerRoute("Plugin.PaymentBankTransferInstructions",
                 $"{BankTransferPaymentDefaults.PaymentInstructionsUrl}/{{orderId}}",
                 new { controller = "PaymentBankTransfer", action = "PaymentInstructions", area = "" }
            );
        }

        public int Priority => 0;

    }
}
