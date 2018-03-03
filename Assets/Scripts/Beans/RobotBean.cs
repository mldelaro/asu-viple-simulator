using UnityEngine;
using System.Collections;
using System.Text;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class RobotBean
{
    public int id { set; get; }
    public string name { set; get; }
    public string actionProfile { set; get; }
    public List<SensorBean> sensorProfile { set; get; }

    //CONSTRUCTOR
    public RobotBean(int id, string name, string actionProfile, List<SensorBean> sensorProfile)
    {
        this.id = id;
        this.name = name;
        this.actionProfile = actionProfile;
        this.sensorProfile = sensorProfile;
    }

    public RobotBean(int id, string name, List<SensorBean> sensorProfile)
    {
        this.id = id;
        this.name = name;
        this.actionProfile = "";
        this.sensorProfile = sensorProfile;
    }

    public RobotBean(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.actionProfile = "";
        this.sensorProfile = new List<SensorBean>();
    }

    public void updateSensorProfile()
    {
        foreach (SensorBean sensor in sensorProfile)
        {
            sensor.updateSensor();
        }
    }

    public string toJSON()
    {
        StringBuilder stringBuilder = new StringBuilder();
        StringWriter stringWriter = new StringWriter(stringBuilder);
        JsonWriter jsonWriter = new JsonTextWriter(stringWriter);
        JsonWriter writer = new JsonTextWriter(stringWriter);

        writer.Formatting = Formatting.Indented;
        writer.WriteStartObject();
        /*writer.WritePropertyName("identifier");
        writer.WriteValue(this.id);
        writer.WritePropertyName("name");
        writer.WriteValue(this.name);
        writer.WritePropertyName("currentActionProfile");
        writer.WriteValue(this.actionProfile);*/
        writer.WritePropertyName("sensors");
        writer.WriteStartArray();

        //Start printing sensor profile
        for (int i = 0; i < sensorProfile.Count; i++)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("name");
            writer.WriteValue(sensorProfile[i].name);

            writer.WritePropertyName("id");
            writer.WriteValue(sensorProfile[i].id);

            /* Start printing sensor readout
            writer.WritePropertyName("readout");
            writer.WriteStartArray();
            for (int i2 = 0; i2 < sensorProfile[i].readout.Count; i2++)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("unit");
                writer.WriteValue(sensorProfile[i].readout[i2].unit);
                writer.WritePropertyName("label");
                writer.WriteValue(sensorProfile[i].readout[i2].label);
                writer.WritePropertyName("value");
                writer.WriteValue(sensorProfile[i].readout[i2].value);
                writer.WriteEndObject();
            }*/

            writer.WritePropertyName("value");
            writer.WriteValue(sensorProfile[i].readout[0].value);

            //writer.WriteEndArray();
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        /*writer.WriteStartArray();
        for (int i = 0; i < sensorProfile.Count; i++)
        {
            writer.WritePropertyName("identifier");
            writer.WriteValue(sensorProfile[i].id);
            writer.WritePropertyName("name");
            writer.WriteValue(sensorProfile[i].name);
            writer.WritePropertyName("readout");
            writer.WriteStartArray();
            for (int i2 = 0; i2 < sensorProfile[i].readout.Count; i2++)
            {
                writer.WritePropertyName("unit");
                writer.WriteValue(sensorProfile[i].readout[i2].unit);
                writer.WritePropertyName("label");
                writer.WriteValue(sensorProfile[i].readout[i2].label);
                writer.WritePropertyName("value");
                writer.WriteValue(sensorProfile[i].readout[i2].value);
            }
            writer.WriteEnd();
        }
        writer.WriteEnd();*/
        writer.WriteEndObject();
        String jsonOutput = stringWriter.ToString();
        jsonOutput = jsonOutput.Replace("\r", "");
        jsonOutput = jsonOutput.Replace("\n", "");
        return jsonOutput;
    }
}
