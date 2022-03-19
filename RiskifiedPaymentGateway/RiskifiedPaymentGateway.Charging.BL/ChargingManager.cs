using RiskifiedPaymentGateway.Charging.BL.CreditCardChargers;
using RiskifiedPaymentGateway.Charging.BL.Models;
using RiskifiedPaymentGateway.Charging.DAL;
using RiskifiedPaymentGateway.Charging.Model;
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
        Task<TransactionResult> ChargeCreditCard(string merchentIdentifier, string creditCardCompany, TransactionPayload payload);
        Task<IEnumerable<ChargeStatusStatistics>> GetChargeStatuses(string merchantId);
    }
    public class ChargingManager : IChargingManager
    {
        private ICreditCardChargerFactory _creditCardChargerFactory;
        private readonly IChargeStatusRepository _chargeStatusRepository;

        public ChargingManager(ICreditCardChargerFactory creditCardChargerFactory, IChargeStatusRepository chargeStatusRepository)
        {
            _creditCardChargerFactory = creditCardChargerFactory;
            _chargeStatusRepository = chargeStatusRepository;
        }

        public async Task<TransactionResult> ChargeCreditCard(string merchentIdentifier, string creditCardCompany, TransactionPayload payload)
        {
            var creditCardCharger = _creditCardChargerFactory.Get(creditCardCompany);

            var transactionResult = await creditCardCharger.ChargeCreditCard(payload);

            if (transactionResult.IsSuccess)
            {
                return transactionResult;
            }

            await AddFailedTransactionToRepository(merchentIdentifier, transactionResult);

            return transactionResult;
        }

        private async Task AddFailedTransactionToRepository(string merchentIdentifier, TransactionResult transactionResult)
        {
            ChargeStatus chargeStatus = new ChargeStatus
            {
                MerchentIdentifier = merchentIdentifier,
                ErrorMessage = transactionResult.ReasonForFailure
            };

            await _chargeStatusRepository.Add(chargeStatus);
        }

        public Task<IEnumerable<ChargeStatusStatistics>> GetChargeStatuses(string merchantId)
        {
            return _chargeStatusRepository.GetMerchentCharges(merchantId);
        }
    }
}
