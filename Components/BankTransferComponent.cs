using Microsoft.AspNetCore.Mvc;

namespace Payments.BankTransfer.Components
{
    [ViewComponent(Name = "BankTransfer")]
    public class BankTransferComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            return View();
        }
    }
}
