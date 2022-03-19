using RiskifiedPaymentGateway.API.DTOs;
using RiskifiedPaymentGateway.Core.Model;
using RiskifiedPaymentGateway.Utils;
using RiskifiedPaymentGateway.Utils.ExtentsionMethods.String;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.API.Validators
{
    public interface IChargeValidator
    {
        IEnumerable<ValidationResult> IsChargingRequestValid(ChargeRequest request);
    }
    public class ChargeValidator : IChargeValidator
    {
        private readonly ICreditCardCompanyValidator _creditCardCompanyValidator;

        public ChargeValidator(ICreditCardCompanyValidator creditCardCompanyValidator)
        {
            _creditCardCompanyValidator = creditCardCompanyValidator;
        }
        public IEnumerable<ValidationResult> IsChargingRequestValid(ChargeRequest request)
        {
            if (RequestFieldsAreInvalid(request))
            {
                yield return new ValidationResult("Invalid field was detected");
            }

            if (!IsCreditCardCompanySupported(request.CreditCardCompany))
            {
                yield return new ValidationResult($"Credit card company {request.CreditCardCompany} is not supported");
            }

            var regex = "(0[1-9]|1[0-2])/?(([0-9]{4})|[0-9]{2}$)";
            if (!Regex.IsMatch(request.ExpirationDate, regex)){
                yield return new ValidationResult("Invalid date format");
            }

            if (!ExpirationDateIsValid(request.ExpirationDate))
            {
                yield return new ValidationResult("Invalid date");
            }
        }

        private bool RequestFieldsAreInvalid(ChargeRequest request)
        {
            return request.MerchantIdentifier.IsNullOrEmpty() ||
                            request.FullName.IsNullOrEmpty() ||
                            request.CreditCardNumber.IsNullOrEmpty() ||
                            request.CreditCardCompany.IsNullOrEmpty() ||
                            request.ExpirationDate.IsNullOrEmpty() ||
                            request.CVV.IsNullOrEmpty() ||
                            request.Amount == null;
        }

        private bool IsCreditCardCompanySupported(string creditCardCompany)
        {
            return _creditCardCompanyValidator.IsCreditCardCompanySupported(creditCardCompany);
        }

        private bool ExpirationDateIsValid(string expirationDateStr)
        {
            DateTime expirationDate;
            return ExpirationDateUtility.TryToConvertCVVDate(expirationDateStr, out expirationDate);

        }
    }
}
