using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class ManagerScene : MonoBehaviour
{

    public static ManagerScene GetManagerScene { get; private set; }
    // Use this for initialization

    private void Awake()
    {
        if (GetManagerScene == null)
        {
            GetManagerScene = this;
        }
        else if (GetManagerScene != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeCurrentScene(int sceneID)
    {
   
        SceneManager.LoadScene(sceneID);

        if (sceneID == 0)
        {
            GameManager.GetGameManager.ActivateDelegateMenu();
        }
    }

    public void ChangeCurrentScene(string sceneName)
    {
      
        SceneManager.LoadScene(sceneName);
   
        if (sceneName == "MainScreen")
        {
            GameManager.GetGameManager.ActivateDelegateMenu();
        }
    }

    public int GetActiveSceneID()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public Scene GetActiveScene()
    {
        return SceneManager.GetActiveScene();
    }

    public void Quit()
    {
        Application.Quit();
    }

}
