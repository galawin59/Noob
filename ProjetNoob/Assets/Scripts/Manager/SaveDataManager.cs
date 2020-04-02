using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    static string fileName = "SavePlayer.save";
    static string directoryName = /*Application.dataPath*/Application.persistentDataPath + "/" + "NoobSaves";

    public static void Save(object entity)
    {
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = File.Open( directoryName + "/" + fileName, FileMode.OpenOrCreate))
        {
            formatter.Serialize(stream, entity);
            stream.Close();
        }
    }

    public static object Load()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = File.Open(directoryName + "/" + fileName, FileMode.OpenOrCreate))
        {
            var entity = formatter.Deserialize(stream);
            stream.Close();
            return entity;
        }
    }

    public static bool IsSaveExists()
    {
        return File.Exists(directoryName + "/" + fileName);
    }
}
