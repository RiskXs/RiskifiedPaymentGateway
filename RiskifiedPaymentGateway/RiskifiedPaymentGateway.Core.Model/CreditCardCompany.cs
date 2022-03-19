using System;
using System.Collections.Generic;

namespace RiskifiedPaymentGateway.Core.Model
{
    public interface ICreditCardCompanyValidator
    {
        bool IsCreditCardCompanySupported(string creditCardCompany);
    }
    public class CreditCardCompany: ICreditCardCompanyValidator
    {
        public const string Visa = "Visa";
        public const string MasterCard = "MasterCard";

        private Dictionary<string, bool> _supportedCompanies;
        public CreditCardCompany()
        {
            _supportedCompanies = initializeSupportedCompanies();
        }
        public bool IsCreditCardCompanySupported(string creditCardCompany)
        {
            return _supportedCompanies.GetValueOrDefault(creditCardCompany.ToLower());
        }

        private Dictionary<string, bool> initializeSupportedCompanies()
        {
            return new Dictionary<string, bool>
            {
                {Visa.ToLower(), true },
                {MasterCard.ToLower(), true }
            };
        }
    }
}
