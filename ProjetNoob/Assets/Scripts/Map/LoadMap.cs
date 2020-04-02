using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMap : MonoBehaviour
{
    bool isLoaded = false;

    public bool IsLoaded
    {
        get
        {
            return isLoaded;
        }

        set
        {
            isLoaded = value;
        }
    }

    IEnumerator Start()
    {
        string mapToLoad = gameObject.scene.name;
 
        while (PlayerManager.GetPlayerManager.playerController == null)
        {
            yield return null;
        }

        MapManager.GetMapManager.LoadMap(mapToLoad);

        isLoaded = true;
    }
}
