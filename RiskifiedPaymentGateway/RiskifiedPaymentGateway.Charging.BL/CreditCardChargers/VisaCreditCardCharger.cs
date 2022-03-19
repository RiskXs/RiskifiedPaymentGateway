using Microsoft.Extensions.Options;
using RiskifiedPaymentGateway.Charging.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.BL.CreditCardChargers
{
    public class VisaCreditCardCharger : ICreditCardCharger
    {
        private readonly IOptions<VisaChargingSettings> _visaChargingSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public VisaCreditCardCharger(IOptions<VisaChargingSettings> visaChargingSettings, IHttpClientFactory httpClientFactory)
        {
            _visaChargingSettings = visaChargingSettings;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<TransactionResult> ChargeCreditCard(TransactionPayload payload)
        {
            var request = GenerateHttpRequest(payload);
            HttpResponseMessage response = await Charge(request);
            var transactionResult = await ConvertToTransactionResult(response);

            return transactionResult;
        }

        private HttpRequestMessage GenerateHttpRequest(TransactionPayload payload)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri(_visaChargingSettings.Value.CharingEndPointUri);
            requestMessage.Method = HttpMethod.Post;
            
            // Sets the transaction details
            requestMessage.Headers.Add(VisaConsts.Identifier, payload.FirstName);
            requestMessage.Content = GenerateRequestContent(payload);

            return requestMessage;
        }
        private Task<HttpResponseMessage> Charge(HttpRequestMessage request)
        {
            return _httpClientFactory.CreateClient().SendAsync(request);
        }


        private JsonContent GenerateRequestContent(TransactionPayload payload)
        {
            var requestBody = new
            {
                fullName = $"{payload.FirstName} {payload.LastName}",
                number = payload.CreditCardNumber,
                expiration = payload.ExpirationDate.ToString(_visaChargingSettings.Value.ExpirationDateFormat),
                cvv = payload.CVV,
                totalAmount = payload.ChargingAmount
            };

            return JsonContent.Create(requestBody);
        }

        private async Task<TransactionResult> ConvertToTransactionResult(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return new TransactionResult { IsSuccess = false, ReasonForFailure = VisaConsts.UnknownFailureReasons };
            }

            var responseParams = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            var isSuccess = responseParams.GetValueOrDefault(VisaConsts.ChargeResult) != null &&
                responseParams[VisaConsts.ChargeResult].ToLower() == VisaConsts.SuccessValue.ToLower();
            
            return new TransactionResult
            {
                IsSuccess = isSuccess,
                ReasonForFailure = responseParams.GetValueOrDefault(VisaConsts.ResultReason)
            };
        }

        class VisaConsts{

            public const string Identifier = "identifier";
            public const string ChargeResult = "chargeResult";
            public const string ResultReason = "resultReason";
            public const string SuccessValue = "Success";
            public const string UnknownFailureReasons = "Uknown";
        }

    }

    public class VisaChargingSettings
    {
        public string CharingEndPointUri { get; set; }
        public string ExpirationDateFormat { get; set; }
    }
}
