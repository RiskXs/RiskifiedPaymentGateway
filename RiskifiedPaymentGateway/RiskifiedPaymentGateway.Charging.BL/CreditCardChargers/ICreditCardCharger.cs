using RiskifiedPaymentGateway.Charging.BL.Models;
using System;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.BL.CreditCardChargers
{
    public interface ICreditCardCharger
    {
        Task<TransactionResult> ChargeCreditCard(TransactionPayload payload);
    }
}
