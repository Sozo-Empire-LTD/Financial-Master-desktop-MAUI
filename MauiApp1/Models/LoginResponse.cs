using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    public class LoginResponse // Changed to public
    {
        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("data")]
        public ResponseData Data { get; set; }
    }
    public class ResponseData
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("businesses")]
        public List<Business> Businesses { get; set; } 
    }
}
