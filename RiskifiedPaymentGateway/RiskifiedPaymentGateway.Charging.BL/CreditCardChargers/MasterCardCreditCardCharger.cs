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
    public class MasterCardCreditCardCharger : ICreditCardCharger
    {
        private readonly IOptions<MasterCardChargingSettings> _masterCardChargingSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public MasterCardCreditCardCharger(IOptions<MasterCardChargingSettings> masterCardChargingSettings, IHttpClientFactory httpClientFactory)
        {
            _masterCardChargingSettings = masterCardChargingSettings;
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
            requestMessage.RequestUri = new Uri(_masterCardChargingSettings.Value.CharingEndPointUri);
            requestMessage.Method = HttpMethod.Post;

            // Sets the transaction details
            requestMessage.Headers.Add(MasterCardConsts.Identifier, payload.FirstName);
            requestMessage.Content = GenerateRequestContent(payload);

            return requestMessage;
        }

        private Task<HttpResponseMessage> Charge(HttpRequestMessage request)
        {
            return _httpClientFactory.CreateClient().SendAsync(request);
        }

        private async Task<TransactionResult> ConvertToTransactionResult(HttpResponseMessage response)
        {
            var failedRequestButNotBadRequest = !response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.BadRequest;
            var badRequest = response.StatusCode == System.Net.HttpStatusCode.BadRequest;
            
            if (failedRequestButNotBadRequest)
            {
                return new TransactionResult { IsSuccess = false, ReasonForFailure = MasterCardConsts.UnknownFailureReasons };
            }

            if(badRequest)
            {
                var responseParams = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var error = responseParams.GetValueOrDefault(MasterCardConsts.DeclineReason);

                return new TransactionResult { IsSuccess = false, ReasonForFailure = error };
            }

            return new TransactionResult { IsSuccess = true};
        }

        private JsonContent GenerateRequestContent(TransactionPayload payload)
        {
            var requestBody = new
            {
                first_name = payload.FirstName,
                last_name = payload.LastName,
                card_number = payload.CreditCardNumber,
                expiration = payload.ExpirationDate.ToString(_masterCardChargingSettings.Value.ExpirationDateFormat),
                cvv = payload.CVV,
                charge_amount = payload.ChargingAmount
            };

            return JsonContent.Create(requestBody);
        }

        class MasterCardConsts
        {
            public const string Identifier = "identifier";
            public const string UnknownFailureReasons = "Uknown";
            public const string DeclineReason = "decline_reason";
        }
    }

    public class MasterCardChargingSettings
    {
        public string CharingEndPointUri { get; set; }
        public string ExpirationDateFormat { get; set; }
    }
}
