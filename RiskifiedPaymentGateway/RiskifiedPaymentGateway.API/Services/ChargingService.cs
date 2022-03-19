﻿using RiskifiedPaymentGateway.API.DTOs;
using RiskifiedPaymentGateway.API.Models;
using RiskifiedPaymentGateway.Charging.BL;
using RiskifiedPaymentGateway.Charging.BL.Models;
using RiskifiedPaymentGateway.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.API.Services
{
    public interface IChargingService
    {
        Task<ChargingResult> ChargeCreditCard(ChargeRequest request);
    }
    public class ChargingService : IChargingService
    {
        private readonly IChargingManager _chargingManager;

        public ChargingService(IChargingManager chargingManager)
        {
            _chargingManager = chargingManager;
        }
        
        public async Task<ChargingResult> ChargeCreditCard(ChargeRequest request)
        {
            var creditCardCompany = GetCreditCardCompany(request.CreditCardCompany);
            var transactionPayload = ConvertToTransactionPayload(request);
            var payloadResult = await _chargingManager.ChargeCreditCard(creditCardCompany, transactionPayload);

            return new ChargingResult
            {
                IsSuccess = payloadResult.IsSuccess,
                error = payloadResult.ReasonForFailure
            };
        }

        private CreditCardCompany GetCreditCardCompany(string company)
        {
            var companyName = Enum.GetNames(typeof(CreditCardCompany)).FirstOrDefault(supportedCompany => supportedCompany.ToLower() == company.ToLower());
            return (CreditCardCompany)Enum.Parse(typeof(CreditCardCompany), companyName);
        }

        private TransactionPayload ConvertToTransactionPayload(ChargeRequest request)
        {
            var splittedName = request.FullName.Trim().Split(" ");
            var firstName =  splittedName[0];
            var lastName = splittedName[1];
            DateTime expirationDate = DateTime.Parse($"1/{request.ExpirationDate}");

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