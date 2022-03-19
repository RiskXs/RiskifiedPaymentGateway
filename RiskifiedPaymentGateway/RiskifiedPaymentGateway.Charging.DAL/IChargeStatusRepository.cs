using RiskifiedPaymentGateway.Charging.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.DAL
{
    public interface IChargeStatusRepository
    {
        Task Add(ChargeStatus chargeStatus);
        Task<IEnumerable<ChargeStatusStatistics>> GetMerchentCharges(string merchentId);
    }
}
