using FlexiForms.Data.Repositories;
using FlexiForms.Data.Tables;
using FlexiForms.Extensions;
using FlexiForms.Messages.Views;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;


namespace FlexiForms.Controllers
{
    [PluginController("FlexiForms")]
    public class ResponsesController : UmbracoApiController
    {
        private readonly IFlexiFormsResponsesRepository _responseRepository;
        private readonly ILogger<ResponsesController> _logger;

        public ResponsesController(
            IFlexiFormsResponsesRepository responseRepository,
            ILogger<ResponsesController> logger)
        {
            _responseRepository = responseRepository;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetResponses(string formIdentifier)
        {
            var responses = await _responseRepository.GetResponsesByFormAlias(formIdentifier);
            var mapped = responses.MapToView<FlexiFormResponsesSchema, ResponseView>();

            return Ok(mapped);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteResponse(int responseId)
        {
            var response = await _responseRepository.DeleteFormById(responseId);

            return Ok(responseId);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteAllByIdentifier(string formIdentifier)
        {
            var response = await _responseRepository.DeleteAllByIdentifier(formIdentifier);

            return Ok();
        }
    }
}
