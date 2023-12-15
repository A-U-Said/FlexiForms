using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlexiForms.Models
{
    public class RecaptchaResponseModel
    {
        public bool Success { get; set; }
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
