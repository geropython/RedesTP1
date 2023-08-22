using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoad //: MonoBehaviour
{
    //public Info info;
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        //SerializationJSON(info, "Save", "Test");
    //        SerializationBIN(info, "Save", "Test");
    //    }
    //    else if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        //info = DeserializationJSON<Info>("Save", "Test");
    //        //DeserializationJSON(info, "Save", "Test");
    //        info = DeserializationBIN<Info>("Save", "Test");
    //    }
    //}

    public static void SerializationBIN<T>(T data, string path, string fileName)
    {
        var realPath = Path.Combine(Application.dataPath, path, fileName + ".bin");
        FileStream file = File.Create(realPath);
        var formatter = new BinaryFormatter();
        formatter.Serialize(file, data);
        file.Close();
    }
    public static T DeserializationBIN<T>(string path, string fileName)
    {
        var realPath = Path.Combine(Application.dataPath, path, fileName + ".bin");
        if (!File.Exists(realPath)) return default(T);
        FileStream file = File.OpenRead(realPath);
        var formatter = new BinaryFormatter();
        var data = (T)formatter.Deserialize(file);
        file.Close();
        return data;
    }

    public static T DeserializationJSON<T>(string path, string fileName)
    {
        var realPath = Path.Combine(Application.dataPath, path, fileName + ".json");
        if (!File.Exists(realPath)) return default(T);
        //StreamReader file = File.OpenText(path);
        //string json = file.ReadToEnd();
        //file.Close();

        string json = File.ReadAllText(realPath);

        return JsonUtility.FromJson<T>(json);
    }
    public static void DeserializationJSON<T>(T data, string path, string fileName)
    {
        var realPath = Path.Combine(Application.dataPath, path, fileName + ".json");
        if (!File.Exists(realPath)) return;
        string json = File.ReadAllText(realPath);
        JsonUtility.FromJsonOverwrite(json, data);
    }

    public static void SerializationJSON<T>(T data, string path, string fileName)
    {
        var json = JsonUtility.ToJson(data, true);
        var realPath = Path.Combine(Application.dataPath, path, fileName + ".json");

        //StreamWriter file = File.CreateText(path);
        //file.Write(json);
        //file.Close();

        File.WriteAllText(realPath, json);
        //if (Directory.Exists(path)) { }
        //if (File.Exists(path))
        // Assets/Save/test.json
        // myFolder/save/test.json
    }
}
