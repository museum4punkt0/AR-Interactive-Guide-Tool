using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class XmlOperation
{
    public static bool TrySerialize(object item, string path)
    {
        try 
        {
            var dir = Path.GetDirectoryName(path);
            Directory.CreateDirectory(dir);
            
            XmlSerializer serializer = new XmlSerializer(item.GetType());
            using (FileStream file = System.IO.File.Create(path))
            {
                serializer.Serialize(file, item);
                file.Close();
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Serialized Failed with Exception:" + e.Message);
            return false;
        }
    }

    public static string Serialize<T>(this T toSerialize)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
    
        using(StringWriter stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, toSerialize);
            return stringWriter.ToString();
        }
    }

    public static T TryDeserialize<T>(string path)
    {
        try 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(path))
            {
                T deserialized = (T)serializer.Deserialize(reader.BaseStream);
                reader.Close();
                return deserialized;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Deserialized Failed with Exception:" + e.Message);
            return default(T);
        }
    }

    public static T TryDeserializeFromString<T>(string data)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(data))
            {
                T deserialized = (T)serializer.Deserialize(reader);
                reader.Close();
                return deserialized;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Deserialized Failed with Exception:" + e.Message);
            return default(T);
        }
    }
}