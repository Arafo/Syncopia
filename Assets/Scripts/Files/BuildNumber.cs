using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

public class BuildNumber : MonoBehaviour {

    [Serializable()]
    public class Build
    {
        [XmlAttribute("number")]
        public int Number;
    }

    // Use this for initialization
    void Start()
    {
        string path = "E:/Rafa/Mis documentos/Unity/ProjectSyncopia";
        string file = "build.xml";

        Build build = Load(Path.Combine(path, file));
        build.Number = build.Number + 1;
        Save(Path.Combine(path, file), build);
        //Debug.Log(build.Number);
    }

    public void Save(string path, Build build)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Build));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, build);
        }
    }

    public static Build Load(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Build));
        StreamReader reader = new StreamReader(path);
        Build build = (Build)serializer.Deserialize(reader);
        reader.Close();

        return build;
    }
}