using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.BL.HttpPolicies
{
    public class MasterCardHttpRetryPolicy
    {
        public const string Name = "MasterCardRetryPolicy";

        public const string masterCardErrorFieldName = "decline_reason";
        public const string CardDecliened = "Card declined";
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg =>
                {
                    if (msg.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string bodyContent = new StreamReader(msg.Content.ReadAsStream()).ReadToEnd();
                        var responseParams = JsonSerializer.Deserialize<Dictionary<string, string>>(bodyContent);
                        return responseParams.GetValueOrDefault(masterCardErrorFieldName) == CardDecliened;
                    }

                    return false;
                })
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }
    }
}
