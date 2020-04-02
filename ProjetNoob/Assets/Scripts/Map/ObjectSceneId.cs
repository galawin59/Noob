using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ObjectSceneId : NetworkBehaviour
{
    [SerializeField]
    bool isFurniture = false;

    [SyncVar]
    string sceneName = "";

    public string SceneName
    {
        get
        {
            return sceneName;
        }

        set
        {

            sceneName = value;
        }
    }

    IEnumerator Start()
    {
        if (!isServer && !isFurniture)
        {
            while (PlayerManager.GetPlayerManager.currentPlayer == null)
            {
                yield return null;
            }
            Scene s = SceneManager.GetSceneByName(sceneName);
            SceneManager.MoveGameObjectToScene(gameObject, s);
            string currentScene = SceneManager.GetSceneByBuildIndex(PlayerManager.GetPlayerManager.currentPlayer.GetComponent<PlayerController>().sceneId).name;
            if (currentScene != sceneName)
            {
                gameObject.SetActive(false);
            }
        }

        //Sorting
        Collider2D collider = GetComponent<Collider2D>();
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        SpriteRenderer spr = transform.GetComponent<SpriteRenderer>();

        if (spr)
        {
            if (collider)
            {
                spr.sortingOrder = (int)(((transform.position.y + collider.offset.y) * 100.0f) * -1 + 1000);
            }
            else if (poly)
            {
                spr.sortingOrder = (int)(((transform.position.y + poly.offset.y) * 100.0f) * -1 + 1000);

            }
            else
            {
                spr.sortingOrder = (int)((transform.position.y * 100.0f) * -1 + 1000);
            }
        }
    }
}
