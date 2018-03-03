using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SensorBean : RobotComponentBean {
    public List<SensorReadoutBean> readout { set; get; }
    public string type { set; get; }

    //CONSTRUCTOR
    public SensorBean(int id, string name, RobotComponentBean.LocalPlacement localPlacement, string type, List<SensorReadoutBean> readout)
        : base(id, name, localPlacement)
    {
        this.type = type;
        this.readout = readout;
    }

    public SensorBean(int id, string name, string type)
        : base(id, name)
    {
        this.type = type;
        this.readout = new List<SensorReadoutBean>();
    }

    public void updateSensor()
    {

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
        writer.WritePropertyName("identifier");
        writer.WriteValue(this.id);
        writer.WritePropertyName("name");
        writer.WriteValue(this.name);
        writer.WritePropertyName("readout");
        writer.WriteStartArray();
        for (int i = 0; i > readout.Count; i++)
        {
            readout[i].toJson();
        }
        writer.WriteEnd();
        writer.WriteEndObject();
        return stringWriter.ToString();
    }
}
