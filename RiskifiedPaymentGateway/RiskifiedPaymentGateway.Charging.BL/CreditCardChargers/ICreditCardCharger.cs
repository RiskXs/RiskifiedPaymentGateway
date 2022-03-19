using System;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.BL.CreditCardChargers
{
    public interface ICreditCardCharger
    {
        Task<TransactionResult> ChargeCreditCard(TransactionPayload payload);
    }

    public class TransactionPayload
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CreditCardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CVV { get; set; }
        public double ChargingAmount { get; set; }
    }

    public class TransactionResult
    {
        public bool IsSuccess { get; set; }
        public string ReasonForFailure { get; set; }
    }
}
