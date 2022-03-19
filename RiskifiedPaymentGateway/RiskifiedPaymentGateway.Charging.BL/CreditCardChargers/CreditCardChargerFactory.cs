using RiskifiedPaymentGateway.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.BL.CreditCardChargers
{
    public interface ICreditCardChargerFactory
    {
        ICreditCardCharger Get(string company);
    }
    public class CreditCardChargerFactory: ICreditCardChargerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CreditCardChargerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ICreditCardCharger Get(string company)
        {
            string lowerCaseCompany = company.ToLower();
            if(lowerCaseCompany == CreditCardCompany.Visa.ToLower())
            {
                return GetCreditCardCharger<VisaCreditCardCharger>();
            }

            throw new NotSupportedException($"No charger for {company}");
        }

        private ICreditCardCharger GetCreditCardCharger<T>() where T: ICreditCardCharger
        {
            return (ICreditCardCharger)_serviceProvider.GetService(typeof(T));
        }
    }
}
