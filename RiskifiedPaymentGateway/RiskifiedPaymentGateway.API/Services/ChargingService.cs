using RiskifiedPaymentGateway.API.DTOs;
using RiskifiedPaymentGateway.API.Models;
using RiskifiedPaymentGateway.Charging.BL;
using RiskifiedPaymentGateway.Charging.BL.Models;
using RiskifiedPaymentGateway.Charging.Model;
using RiskifiedPaymentGateway.Core.Model;
using RiskifiedPaymentGateway.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.API.Services
{
    public interface IChargingService
    {
        Task<ChargingResult> ChargeCreditCard(ChargeRequest request);
        Task<IEnumerable<ChargeStatusStatistics>> GetChargeStatuses(string merchantId);
    }
    public class ChargingService : IChargingService
    {
        private readonly IChargingManager _chargingManager;
        private readonly string CardDeclinedMessage = "Card declined";

        public ChargingService(IChargingManager chargingManager)
        {
            _chargingManager = chargingManager;
        }
        
        public async Task<ChargingResult> ChargeCreditCard(ChargeRequest request)
        {
            var creditCardCompany = request.CreditCardCompany;
            var transactionPayload = ConvertToTransactionPayload(request);
            var payloadResult = await _chargingManager.ChargeCreditCard(request.MerchantIdentifier, creditCardCompany, transactionPayload);

            return new ChargingResult
            {
                IsSuccess = payloadResult.IsSuccess,
                error = payloadResult.IsSuccess ? String.Empty : CardDeclinedMessage
            };
        }

        public Task<IEnumerable<ChargeStatusStatistics>> GetChargeStatuses(string merchantId)
        {
            return _chargingManager.GetChargeStatuses(merchantId);
        }

        private TransactionPayload ConvertToTransactionPayload(ChargeRequest request)
        {
            var splittedName = request.FullName.Trim().Split(" ");
            var firstName =  splittedName[0];
            var lastName = splittedName[1];
            DateTime expirationDate;
            ExpirationDateUtility.TryToConvertCVVDate(request.ExpirationDate, out expirationDate);

            return new TransactionPayload
            {
                FirstName = firstName,
                LastName = lastName,
                CVV = request.CVV,
                ChargingAmount = request.Amount.Value,
                CreditCardNumber = request.CreditCardNumber,
                ExpirationDate = expirationDate
            };
        }
    }
}
