using FlexiForms.Data.Repositories;
using FlexiForms.Data.Tables;
using FlexiForms.Extensions;
using FlexiForms.Messages.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using NPoco.fastJSON;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
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


        [HttpGet]
        public async Task<IActionResult> ExportFormResponses(string formIdentifier)
        {
            var responses = await _responseRepository.GetResponsesByFormAlias(formIdentifier);
            var mapped = responses.MapToView<FlexiFormResponsesSchema, ResponseView>();

            var csv = new StringBuilder();

            var fieldHeaders = mapped.Select(x => x.Fields?.Select(x => x.Key)).FirstOrDefault() ?? new List<string>();
            var headers = string.Join(",", new List<string>()
            {
                "Id",
                "Name",
                "Email",
                string.Join(",", fieldHeaders),
                "Internal Email Sent",
                "External Email Sent",
                "Created Date"
            }
            .Where(x => !string.IsNullOrEmpty(x)));
            csv.AppendLine(headers);

            const string NoFields = "NoFields";

            foreach (var response in mapped)
            {
                var fieldValues = response.Fields?.Select(x => x.Key) ?? new List<string>();
                var row = string.Join(",", new List<string>()
                {
                    response.Id.ToString(),
                    response.Name,
                    response.Email,
                    fieldValues.Any() ? string.Join(",", fieldValues) : NoFields,
                    response.InternalSent ? "Yes" : "No",
                    response.ExternalSent ? "Yes" : "No",
                    response.CreateDate.ToString("G")
                }
                .Where(x => x != NoFields));
                csv.AppendLine(row);
            }

            var fileName = $"{formIdentifier}_Responses.csv";
            HttpContext.Response.Headers.Add("x-filename", fileName);

            return File(Encoding.UTF8.GetBytes(csv.ToString()), MediaTypeNames.Application.Octet, fileName);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllFormsWithReplies()
        {
            var responses = await _responseRepository.GetAliases();

            return Ok(responses.Where(x => x.Count > 0));
        }
    }
}
