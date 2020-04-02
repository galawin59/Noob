using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DeleteSave : Editor {

    [MenuItem("Tools/DeleteSave")]
    static void DeleteSaves()
    {
        File.Delete(Application.persistentDataPath + "/" + "NoobSaves/SavePlayer.save");
    }
}
