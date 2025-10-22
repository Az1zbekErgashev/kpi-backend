using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kpi.Service.Extencions
{
    public class JsonTimeZoneConverter : JsonConverter<DateTime>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JsonTimeZoneConverter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDateTime(); 
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            string? timeZoneHeader = _httpContextAccessor.HttpContext?.Request.Headers["TimeZone"];

            try
            {
                TimeZoneInfo tz = !string.IsNullOrEmpty(timeZoneHeader)
                    ? TimeZoneInfo.FindSystemTimeZoneById(timeZoneHeader)
                    : TimeZoneInfo.FindSystemTimeZoneById("Asia/Tashkent"); 

                var localTime = TimeZoneInfo.ConvertTimeFromUtc(value.ToUniversalTime(), tz);
                writer.WriteStringValue(localTime.ToString());
            }
            catch
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
