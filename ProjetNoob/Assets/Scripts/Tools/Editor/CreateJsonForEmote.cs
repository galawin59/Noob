using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class CreateJsonForEmote : EditorWindow
{
    TextAsset jsonFile;
    
    static CreateJsonForEmote window;
   
    [MenuItem("Tools/CreateJson")]
    static void OpenWindow()
    {
        window = GetWindow<CreateJsonForEmote>();
        window.titleContent = new GUIContent("CreateJSon");
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Json", EditorStyles.boldLabel);
        jsonFile = (TextAsset)EditorGUILayout.ObjectField(jsonFile, typeof(TextAsset), false);
        GUILayout.EndVertical();
    }
}
