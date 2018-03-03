using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using Newtonsoft.Json;

public class SensorReadoutBean
{
    public string unit { get; set; }
    public string label { get; set; }
    public string value { get; set; }

    public SensorReadoutBean(string unit, string label, string value)
    {
        this.unit = unit;
        this.label = label;
        this.value = value;
    }

    public string toJson()
    {
        StringBuilder stringBuilder = new StringBuilder();
        StringWriter stringWriter = new StringWriter(stringBuilder);
        JsonWriter jsonWriter = new JsonTextWriter(stringWriter);
        JsonWriter writer = new JsonTextWriter(stringWriter);

        writer.Formatting = Formatting.Indented;
        writer.WriteStartObject();
        writer.Formatting = Formatting.Indented;
        writer.WriteStartObject();
        writer.WritePropertyName("unit");
        writer.WriteValue(this.unit);
        writer.WritePropertyName("label");
        writer.WriteValue(this.label);
        writer.WritePropertyName("value");
        writer.WriteValue(this.value);
        writer.WriteEndObject();
        return stringWriter.ToString();
    }
}
