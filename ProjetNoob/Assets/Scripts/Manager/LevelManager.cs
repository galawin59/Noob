using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    string[] roomName;

    public static LevelManager GetLevelManager { get; private set; }

    // Use this for initialization
    private void Awake()
    {
        if (GetLevelManager == null)
        {
            GetLevelManager = this;
        }
        else if (GetLevelManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    /*private void Start()
    {
        GameManager.GetGameManager.OnReturnMenu += Destroy;
    }*/

    public void GenerateLevels()
    {
        for (int i = 0; i < roomName.Length; i++)
        {

            SceneManager.LoadScene(roomName[i], LoadSceneMode.Additive);
            int id = SceneManager.GetSceneByName(roomName[i]).buildIndex;
            if(!PlayerManager.GetPlayerManager.listConnectedPlayers.ContainsKey(id))
                PlayerManager.GetPlayerManager.listConnectedPlayers.Add(id, new List<GameObject>());
        }
    }

    /*private void Destroy()
    {
        Destroy(gameObject);
    }*/
}
