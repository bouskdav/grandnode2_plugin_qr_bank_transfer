using Grand.Business.Core.Extensions;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Messages;
using Grand.Domain.Messages;
using Grand.Infrastructure.Plugins;

namespace Payments.BankTransfer
{
    /// <summary>
    /// BankTransfer payment processor
    /// </summary>
    public class BankTransferPaymentPlugin : BasePlugin, IPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly EmailAccountSettings _emailAccountSettings;

        #endregion

        #region Ctor

        public BankTransferPaymentPlugin(
            ISettingService settingService,
            ITranslationService translationService,
            ILanguageService languageService,
            IMessageTemplateService messageTemplateService,
            EmailAccountSettings emailAccountSettings)
        {
            _settingService = settingService;
            _translationService = translationService;
            _languageService = languageService;
            _messageTemplateService = messageTemplateService;
            _emailAccountSettings = emailAccountSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string ConfigurationUrl()
        {
            return BankTransferPaymentDefaults.ConfigurationUrl;
        }

        public override async Task Install()
        {
            var settings = new BankTransferPaymentSettings {
                DescriptionText = "<p>In cases where an order is placed, an authorized representative will contact you, personally or over telephone, to confirm the order.<br />After the order is confirmed, it will be processed.<br />Orders once confirmed, cannot be cancelled.</p><p>P.S. You can edit this text from admin panel.</p>",
                QrCodeStringPattern = "SPD*1.0*ACC:{IBAN}+{SWIFT}*AM:{PRICE}*CC:{CURRENCY}*MSG:{SHOP_NAME} ORDER {ID}*X-VS:{VS}",
                VariableSymbolPattern = "{yy}{######}",
                SkipPaymentInfo = true,
            };

            await _settingService.SaveSetting(settings);

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Payments.BankTransfer.FriendlyName", "Bank transfer");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.IBAN", "IBAN");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.SWIFT", "SWIFT");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AccountNumber", "Account number");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.VariableSymbol", "Variable symbol");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.Amount", "Amount");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.DescriptionText", "Description");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.DescriptionText.Hint", "Enter info that will be shown to customers during checkout");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.PaymentMethodDescription", "Bank transfer (with QR code)");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AdditionalFee", "Additional fee");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AdditionalFee.Hint", "The additional fee.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AdditionalFeePercentage", "Additional fee. Use percentage");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.ShippableProductRequired", "Shippable product required");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.ShippableProductRequired.Hint", "An option indicating whether shippable products are required in order to display this payment method during checkout.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.SkipPaymentInfo", "Skip payment info");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.DisplayOrder", "Display order");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.CurrencyWarning", "When scaning QR code, please always check in which currency the payment is processed.");

            MessageTemplate messageTemplate = new MessageTemplate() 
            {
                Name = BankTransferPaymentDefaults.EmailPaymentQrCode,
                Subject = "{{Store.Name}}. Payment instructions with QR code for order #{{Order.OrderNumber}}",
                Body = "<p><a href=\"{{Store.URL}}\">{{Store.Name}}</a> <br />\r\n<br />\r\nDobr� den {{Order.CustomerFullName}}, <br />\r\nD�kujeme za V� n�kup na na�em e-shopu <a href=\"{{Store.URL}}\">{{Store.Name}}</a>. Jeliko� jste zvolil(a) platbu p�evodem, n�e zas�l�me platebn� QR k�d:<br />\r\n<br />\r\n<img style=\"width: 200px;\" src=\"{{Store.URL}}Plugins/PaymentBankTransfer/PaymentCodeByNumber/{{Order.OrderNumber}}.png?size=2\" /><br />\r\n<br />\r\nOstatn� �daje pos�l�me v dal��m emailu.<br />\r\nS pozdravem, t�m {{Store.Name}}",
                IsActive = true,
                LimitedToStores = false,
                EmailAccountId = _emailAccountSettings.DefaultEmailAccountId
            };

            await _messageTemplateService.InsertMessageTemplate(messageTemplate);

            await base.Install();
        }

        public override async Task Uninstall()
        {
            //settings
            await _settingService.DeleteSetting<BankTransferPaymentSettings>();

            //locales
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.CurrencyWarning");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.DescriptionText");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.DescriptionText.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.PaymentMethodDescription");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AdditionalFee");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AdditionalFee.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AdditionalFeePercentage");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AdditionalFeePercentage.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.ShippableProductRequired");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.ShippableProductRequired.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.SkipPaymentInfo");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.Amount");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.VariableSymbol");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.AccountNumber");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.IBAN");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.BankTransfer.SWIFT");

            await base.Uninstall();
        }

        #endregion


    }
}