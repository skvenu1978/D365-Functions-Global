using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.PowerPlatform.Dataverse.Client;
using Newtonsoft.Json;

using McGlobalAzureFunctions.Models.Requests;
using McGlobalAzureFunctions.Gate;
using McGlobalAzureFunctions.FunctionsCommon;
using McGlobalAzureFunctions.Converters;
using McGlobalAzureFunctions.Models.Common;

namespace McGlobalAzureFunctions
{
    public class FnD365ContactWebActivationInfoToAsb
    {
        private readonly ILogger<FnD365ContactWebActivationInfoToAsb> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;

        public FnD365ContactWebActivationInfoToAsb(ILogger<FnD365ContactWebActivationInfoToAsb> logger, IOrganizationServiceAsync2 crmServiceClient)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            _logger = logger;
            _crmServiceClient = crmServiceClient;
        }

        [Function(nameof(FnD365ContactWebActivationInfoToAsb))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("FnD365ContactWebActivationInfoToAsb trigger started");
            bool isSuccess = false;
            string errMsg = string.Empty;
            ContactWebActivationInfoRequest contactWebActRequest = new ContactWebActivationInfoRequest();

            try
            {
                if (req.Body != null)
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    Entity contextEntity = FnCommon.GetEntityFromRequest(requestBody);
                    bool isProd = FnCommon.GetIsProd();

                    _logger.LogInformation("Preparing Request Data Start");
                    contactWebActRequest = ContactWebActivationConversion.GetContactWebActivationtRequest(contextEntity, _crmServiceClient, out errMsg);
                    _logger.LogInformation("Preparing Request Data End");

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        var reqData = JsonConvert.SerializeObject(contactWebActRequest);
                        _logger.LogInformation(reqData);

                        DynTaskResponse asbTask = FnCommon.SendMessageASB(reqData, nameof(ContactWebActivationInfoRequest));
                        if (asbTask.IsSuccess)
                        {
                            isSuccess = true;
                            _logger.LogInformation("ContactWebActivationInfoRequest trigger complete");
                        }
                        else
                        {
                            _logger.LogError(asbTask.Exception, "Error in sending to ASB: " + asbTask.ErrorMessage);
                        }
                    }
                    else
                    {
                        _logger.LogError("Request is not valid to be sent to OMNI or error preparing request: " + errMsg);
                    }
                }

                _logger.LogInformation("FnD365ContactWebActivationInfoToAsb trigger complete");
            }
            catch (Exception ex)
            {
                errMsg = "Error in function: " + nameof(FnD365ContactWebActivationInfoToAsb) + "Error:" + ex.Message;
                _logger.LogError(ex, errMsg);
            }

            if (isSuccess)
                return new OkObjectResult("Completed successfully");
            else
                return new BadRequestObjectResult(errMsg);
        }
    }
}