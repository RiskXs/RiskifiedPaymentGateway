using Microsoft.AspNetCore.Mvc.ModelBinding;
using RiskifiedPaymentGateway.API.DTOs;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.API.Binders
{
    public class ChargeRequestBinder : IModelBinder
    {
        public const string MERCH_IDENTIFIER_KEY = "merchant-Identifier";
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ChargeRequest request = await GetDataFromRequestBody(bindingContext);

            request.MerchantIdentifier = GetMerchantIdentifier(bindingContext);
            
            bindingContext.Result = ModelBindingResult.Success(request);
        }

        private ValueTask<ChargeRequest> GetDataFromRequestBody(ModelBindingContext bindingContext)
        {
            return JsonSerializer.DeserializeAsync<ChargeRequest>(bindingContext.HttpContext.Request.Body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private string GetMerchantIdentifier(ModelBindingContext bindingContext)
        {
            return bindingContext.HttpContext.Request.Headers[MERCH_IDENTIFIER_KEY];
        }
    }
}
