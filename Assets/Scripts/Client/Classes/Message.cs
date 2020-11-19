using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public string Key { get; set; }
    public string Value { get; set; }
    public Message(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public static Message[] Parse(string msg)
    {
        string[] paramArr = msg.Split('&');
        Message[] messages = new Message[1];
        if (paramArr.Length > 1)
            messages = new Message[paramArr.Length];

        for (int i = 0; i < paramArr.Length; i++)
        {
            string[] split = paramArr[i].Split('=');
            messages[i] = new Message(split[0], split[1]);
        }
        return messages;
    }

    public static Func<Message[], string, int> IntValueOfKey = (Message[] m, string k) => Int32.Parse(Array.Find(m, element => element.Key == k).Value);

    public static Func<Message[], string, string> StringValueOfKey = (Message[] m, string k) => Array.Find(m, element => element.Key == k).Value;

    public static Func<Message[], string, float> FloatValueOfKey = (Message[] m, string k) => float.Parse(Array.Find(m, element => element.Key == k).Value);

    public static Func<Message[], string, bool> BoolValueOfKey = (Message[] m, string k) => bool.Parse(Array.Find(m, element => element.Key == k).Value);

    public static Func<Message[], string, Vector3> Vector3ValueOfKey = (Message[] m, string k) =>
    {
        string[] raw = StringValueOfKey(m, k).Split(',');
        if (raw == null || raw.Length == 0 || String.IsNullOrEmpty(raw[0]) || String.IsNullOrWhiteSpace(raw[0]))
            return Vector3.zero;

        return new Vector3(
            float.Parse(raw[0]),
            float.Parse(raw[1]),
            float.Parse(raw[2]));
    };

    public static Func<Vector3, string> ParseVector3 = (Vector3 v) => v.x.ToString("F3") + "," + v.y.ToString("F3") + "," + v.z.ToString("F3");
}
