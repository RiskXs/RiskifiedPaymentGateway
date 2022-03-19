using RiskifiedPaymentGateway.API.DTOs;
using RiskifiedPaymentGateway.Core.Model;
using RiskifiedPaymentGateway.Utils.ExtentsionMethods.String;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.API.Validators
{
    public interface IChargeValidator
    {
        IEnumerable<ValidationResult> IsChargingRequestValid(ChargeRequest request);
    }
    public class ChargeValidator : IChargeValidator
    {
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
            return Enum.GetNames(typeof(CreditCardCompanies))
                .Any(company => company.ToLower() == creditCardCompany.ToLower());
        }

        private bool ExpirationDateIsValid(string expirationDateStr)
        {
            string expirationDateWithMonthDay = $"1/{expirationDateStr}";
            DateTime expirationDate;

            return DateTime.TryParse(expirationDateWithMonthDay, out expirationDate);

        }
    }
}
