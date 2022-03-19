using RiskifiedPaymentGateway.Charging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.DAL
{
    public class InMemoryChargeStatusRepository : IChargeStatusRepository
    {
        Dictionary<string, List<ChargeStatus>> memory = new Dictionary<string, List<ChargeStatus>>();
        public Task Add(ChargeStatus chargeStatus)
        {
            return Task.Run(() =>
            {
                if (!memory.ContainsKey(chargeStatus.MerchentIdentifier))
                {
                    memory.Add(chargeStatus.MerchentIdentifier, new List<ChargeStatus>());
                }

                memory[chargeStatus.MerchentIdentifier].Add(chargeStatus);
            });
        }

        public Task<IEnumerable<ChargeStatusStatistics>> GetMerchentCharges(string merchentId)
        {
            return Task.Run(() =>
            {
                if (!memory.ContainsKey(merchentId))
                {
                    return new ChargeStatusStatistics[] { };
                }

                return memory[merchentId].GroupBy(cs => cs.ErrorMessage).Select(group => new ChargeStatusStatistics
                {
                    ErrorMessage = group.Key,
                    Count = group.Count()
                });
            });
        }
    }
}
