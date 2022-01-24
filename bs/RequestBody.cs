using System.Text.Json;
using System.Text.Json.Serialization;

namespace bs
{
    public abstract class RequestBody
    {
        public string Serialize()
        {
            var options = new JsonSerializerOptions();
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            return JsonSerializer.Serialize(this, this.GetType(), options);
        }
        [JsonIgnore]
        public virtual int ProductGroup => 0;
        [JsonIgnore]
        public virtual string Extention => "";
        [JsonIgnore]
        public virtual string Url => "";
        [JsonIgnore]
        public virtual string PingUrl => "";
    }
}
