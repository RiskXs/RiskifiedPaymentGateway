using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskifiedPaymentGateway.API.DTOs;
using RiskifiedPaymentGateway.API.Services;
using RiskifiedPaymentGateway.API.Validators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IChargingService _chargingService;

        public ChargeController(IChargeValidator chargeValidator, IChargingService chargingService)
        {
            _chargeValidator = chargeValidator;
            _chargingService = chargingService;
        }
        [HttpPost()]
        public async Task<IActionResult> Charge(ChargeRequest request)
        {
            // Note: Normally I'll have ChargeRequest to implement IValidableObject which integrates the custom validation into the request pipeline.
            // However since the demand was for a Bad Request response without a body this is the easiest way
            if (!IsChargeRequestValid(request))
            {
                // Note: Again, there is a more elegant way of doing this (invoking ControllerBase's BadRequest method) but that response will have a body...
                return new ObjectResult(null) { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            try
            {
                var result = await _chargingService.ChargeCreditCard(request);

                if (result.IsSuccess)
                {
                    return Ok();
                }

                return Ok(new { error = result.error });
            }

            // Note: Normally .NET returns 500 in case of uncaught exceptions but since the requirement says empty body I implmeneted this myself 
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new ObjectResult(null) { StatusCode = (int)HttpStatusCode.InternalServerError };

            }
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
