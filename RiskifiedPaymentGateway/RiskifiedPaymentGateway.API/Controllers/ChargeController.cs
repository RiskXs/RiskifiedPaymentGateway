using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskifiedPaymentGateway.API.DTOs;
using RiskifiedPaymentGateway.API.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeController : ControllerBase
    {
        private readonly IChargeValidator _chargeValidator;

        public ChargeController(IChargeValidator chargeValidator)
        {
            _chargeValidator = chargeValidator;
        }
        [HttpPost()]
        public IActionResult Charge(ChargeRequest request)
        {
            // Note: Normally I'll have ChargeRequest to implement IValidableObject which integrates the custom validation into the request pipeline.
            // However since the demand was for a Bad Request response without a body this is the easiest way
            if (!IsChargeRequestValid(request))
            {
                // Note: Again, there is a more elegant way of doing this (invoking ControllerBase's BadRequest method) but that response will have a body...
                return new ObjectResult(null) { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            return Ok();
        }

        private bool IsChargeRequestValid(ChargeRequest request)
        {
            var validationResults = _chargeValidator.IsChargingRequestValid(request);

            // Valid if validationResults is empty
            var isRequestValid = validationResults == null || validationResults.Count() == 0;

            return isRequestValid;
        }
    }
}
