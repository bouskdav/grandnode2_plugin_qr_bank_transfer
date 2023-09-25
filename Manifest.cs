using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Payments.BankTransfer;

[assembly: PluginInfo(
    FriendlyName = "Bank transfer with QR payment",
    Group = "Payment methods",
    SystemName = BankTransferPaymentDefaults.ProviderSystemName,
    Author = "Laguna Solutions",
    Version = "2.00"
)]