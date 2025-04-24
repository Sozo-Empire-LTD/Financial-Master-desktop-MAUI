using Newtonsoft.Json;
using System;

namespace MauiApp1.Models
{
   public class Business
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("phone2")]
        public string Phone2 { get; set; }

        [JsonProperty("image_logo")]
        public string ImageLogo { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("registration_number")]
        public string RegistrationNumber { get; set; }

        [JsonProperty("tax_payers_no")]
        public string TaxPayersNo { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("town")]
        public string Town { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("post_box")]
        public string PostBox { get; set; }

        [JsonProperty("business_type")]
        public int BusinessType { get; set; }

        [JsonProperty("logged_by")]
        public int LoggedBy { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("members_limit")]
        public int? MembersLimit { get; set; }

        [JsonProperty("max_product")]
        public int MaxProduct { get; set; }

        [JsonProperty("startWeek")]
        public int StartWeek { get; set; }

        [JsonProperty("endWeek")]
        public int EndWeek { get; set; }

        [JsonProperty("business_currency")]
        public string BusinessCurrency { get; set; }

        [JsonProperty("previous_currency")]
        public string PreviousCurrency { get; set; }

        [JsonProperty("invoice_frequency")]
        public string InvoiceFrequency { get; set; }

        [JsonProperty("deleted_at")]
        public string DeletedAt { get; set; }

        [JsonProperty("pivot")]
        public BusinessPivot Pivot { get; set; }
    }

    public class BusinessPivot
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("business_id")]
        public int BusinessId { get; set; }
    }
}
