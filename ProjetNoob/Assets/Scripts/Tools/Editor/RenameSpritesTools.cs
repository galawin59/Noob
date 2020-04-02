using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RenameSpritesTools : EditorWindow
{
    public List<Sprite> listSprite;
    Sprite sprite;
    SerializedProperty test;
    static RenameSpritesTools window;
    string nameSprite;

    [MenuItem("Tools/RenameSprite")]
    static void OpenWindow()
    {
        window = GetWindow<RenameSpritesTools>();
        window.titleContent = new GUIContent("Rename");
    }

    private void OnEnable()
    {
        listSprite = new List<Sprite>();
        name = "";
    }

    /*static void RenameSprite()
    {

    }*/

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        nameSprite = EditorGUILayout.TextField("Nouveau nom", nameSprite, GUILayout.Width(350));
        GUILayout.Label("Sprite", EditorStyles.boldLabel);
        sprite = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), false);



        if (sprite)
        {
            listSprite.Add(sprite);
            sprite = null;
        }
        GUILayout.Label("List of Sprites", EditorStyles.boldLabel);
        for (int i = 0; i < listSprite.Count; i++)
        {
            listSprite[i] = (Sprite)EditorGUILayout.ObjectField(listSprite[i], typeof(Sprite), true);

        }
        if (GUILayout.Button("Confirm", GUILayout.Width(120)))
        {
            for (int i = 0; i < listSprite.Count; i++)
            {
                listSprite[i].name = nameSprite;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(listSprite[i]), name + "_" + i);
            }
        }
        //GUILayout.Label("List of sprite in spriteSheet", EditorStyles.boldLabel);
        /* for (int i = 0; i < spriteSheet.Count; i++)
         {
             spriteSheet[i] = (Sprite)EditorGUILayout.ObjectField(spriteSheet[i], typeof(Sprite), true);

         }*/
        //for (int i = 0; i < spriteSheet.Length; i++)
        //{
        //    EditorGUILayout.ObjectField(spriteSheet[i], typeof(Object), true);
        //}

        //nameHand[0] = EditorGUILayout.TextField("Nouveau nom hand", nameHand[0], GUILayout.Width(350));
        //if (GUILayout.Button("Confirm", GUILayout.Width(120)))
        //{
        //    for (int i = 1; i < 5; i++)
        //    {
        //        spriteSheet[i].name = nameHand[0];
        //        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(spriteSheet[i]), nameHand[0] + i);
        //    }
        //}
        //nameHand[1] = EditorGUILayout.TextField("Nouveau nom Pant", nameHand[1], GUILayout.Width(350));
        //if (GUILayout.Button("Confirm", GUILayout.Width(120)))
        //{

        //}
        //nameHand[2] = EditorGUILayout.TextField("Nouveau nom Shoes", nameHand[2], GUILayout.Width(350));
        //if (GUILayout.Button("Confirm", GUILayout.Width(120)))
        //{

        //}
        //nameHand[3] = EditorGUILayout.TextField("Nouveau nom Under", nameHand[3], GUILayout.Width(350));
        //if (GUILayout.Button("Confirm", GUILayout.Width(120)))
        //{

        //}
        //nameHand[4] = EditorGUILayout.TextField("Nouveau nom Torse", nameHand[4], GUILayout.Width(350));
        //if (GUILayout.Button("Confirm", GUILayout.Width(120)))
        //{

        //}
        /* if (GUILayout.Button("Confirm", GUILayout.Width(120)))
         {
             for (int i = 0; i < listSprite.Count; i++)
             {
                 listSprite[i].name = nameSprite;
                 AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(listSprite[i]), name + "_" + i);
             }
         }*/

        if (GUILayout.Button("Close", GUILayout.Width(120)))
        {
            Close();
        }
        GUILayout.EndVertical();
        // contextualAdd = (string)EditorGUILayout.Foldout(true, "String");

        /* if (GUILayout.Button("Build"))
         {
             Selection.activeGameObject = GameObject.CreatePrimitive(selectedPrimitive);
             EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
         }*/
        GUIUtility.ExitGUI();
    }

}
