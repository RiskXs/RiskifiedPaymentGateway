using RiskifiedPaymentGateway.Charging.BL.CreditCardChargers;
using RiskifiedPaymentGateway.Charging.BL.Models;
using RiskifiedPaymentGateway.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.BL
{
    public interface IChargingManager
    {
        Task<TransactionResult> ChargeCreditCard(CreditCardCompany creditCardCompany, TransactionPayload payload);
    }
    public class ChargingManager : IChargingManager
    {
        private ICreditCardChargerFactory _creditCardChargerFactory;
        public ChargingManager(ICreditCardChargerFactory creditCardChargerFactory)
        {
            _creditCardChargerFactory = creditCardChargerFactory;
        }

        public async Task<TransactionResult> ChargeCreditCard(CreditCardCompany creditCardCompany, TransactionPayload payload)
        {
            var creditCardCharger = _creditCardChargerFactory.Get(creditCardCompany);

            return await creditCardCharger.ChargeCreditCard(payload);
        }
    }
}
