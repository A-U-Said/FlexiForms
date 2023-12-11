using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlexForms.Models
{
    public class RecaptchaResponseModel
    {
        public bool Success { get; set; }
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
