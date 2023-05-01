using System.Text.Json;
using System.Text.Json.Serialization;

namespace usace.cc.plugin
{
    public class Payload
    {
    public Dictionary<string, Object> Attributes { get; set; }
    public DataStore[] Stores { get; set; }
    public DataSource[] Inputs { get; set; }
    public DataSource[] Outputs { get; set; }

    public static Payload FromJson(string json)
    {
      JsonSerializerOptions options = new JsonSerializerOptions();
      options.PropertyNameCaseInsensitive = true;
      options.Converters.Add(new StoreTypeConverter());

      Payload p = JsonSerializer.Deserialize<Payload>(json, options);

      return p;
    }
  }
  class StoreTypeConverter : JsonConverter<StoreType>
  {
    public override StoreType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (Enum.TryParse<StoreType>(reader.GetString(), out var value))
      {
        return value;
      }
      else
      {
        throw new JsonException($"Invalid enum value: {reader.GetString()}");
      }
    }

    public override void Write(Utf8JsonWriter writer, StoreType value, JsonSerializerOptions options)
    {
      writer.WriteStringValue(value.ToString());
    }
  }
}
