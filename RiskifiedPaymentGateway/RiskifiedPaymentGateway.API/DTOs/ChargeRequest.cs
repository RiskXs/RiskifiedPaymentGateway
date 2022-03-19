using Microsoft.AspNetCore.Mvc;
using RiskifiedPaymentGateway.API.Binders;
using RiskifiedPaymentGateway.Utils.ExtentsionMethods.String;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.API.DTOs
{
    [ModelBinder(typeof(ChargeRequestBinder))]
    public class ChargeRequest
    {
        public string MerchantIdentifier { get; set; }
        public string FullName { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardCompany { get; set; }
        public string ExpirationDate { get; set; }
        public string CVV { get; set; }
        public double? Amount { get; set; }
    }
}
