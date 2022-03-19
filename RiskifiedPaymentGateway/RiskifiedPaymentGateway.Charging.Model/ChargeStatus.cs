using System;

namespace RiskifiedPaymentGateway.Charging.Model
{
    public class ChargeStatus
    {
        public string MerchentIdentifier { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ChargeStatusStatistics
    {
        public string ErrorMessage { get; set; }
        public int Count { get; set; }
    }
}
