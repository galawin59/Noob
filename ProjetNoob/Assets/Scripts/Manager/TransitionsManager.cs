using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionsManager : MonoBehaviour
{
    public static TransitionsManager GetTransitionsManager { get; private set; }
    public Animator anim;

    private int sceneId = -1;
    private string sceneName = "";
    private bool connection = false;
    private bool isServer = false;
    public bool inGame = false;

    public bool InGame
    {
        get
        {
            return inGame;
        }

        set
        {
            inGame = value;
        }
    }
    private void Awake()
    {
        if (GetTransitionsManager == null)
        {
            GetTransitionsManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (GetTransitionsManager != this)
        {
            Destroy(gameObject);
        }
    }

    public void transitionEndEvent()
    {
        if (!inGame)
        {
            if (sceneId != -1)
            {
                ManagerScene.GetManagerScene.ChangeCurrentScene(sceneId);
            }
            else
            {
                ManagerScene.GetManagerScene.ChangeCurrentScene(sceneName);
            }

            if (connection)
            {
                LevelManager.GetLevelManager.GenerateLevels();
                if (isServer)
                {
                    PersonalNetworkManager serv = FindObjectOfType<PersonalNetworkManager>();
                    serv.CreateServer();
                }
                else
                {
                    PersonalNetworkManager cli = FindObjectOfType<PersonalNetworkManager>();
                    cli.JoinServer();
                }
            }

            sceneId = -1;
            sceneName = "";
            connection = false;
            isServer = false;
        }
        else
        {
            inGame = false;
        }

    }

    public void startTransitionInGame()
    {
        GetTransitionsManager.anim.SetTrigger("FadeIn");
        GetTransitionsManager.inGame = true;
    }

    public void startTransition(string _sceneName)
    {
        if (GameManager.GetGameManager.menuGo)
            GameManager.GetGameManager.ContinueGame();
        GetTransitionsManager.anim.SetTrigger("FadeIn");
        GetTransitionsManager.sceneName = _sceneName;
    }

    public void startTransition(int _sceneId)
    {
        if (GameManager.GetGameManager.menuGo)
            GameManager.GetGameManager.ContinueGame();
        GetTransitionsManager.anim.SetTrigger("FadeIn");
        GetTransitionsManager.sceneId = _sceneId;
    }

    public void createServer(string _sceneName)
    {
        if (!GetTransitionsManager.connection)
        {
            startTransition(_sceneName);
            GetTransitionsManager.connection = true;
            GetTransitionsManager.isServer = true;
            GameManager.GetGameManager.ActivateCamera();
        }
    }

    public void connectToServer(string _sceneName)
    {
        if (!GetTransitionsManager.connection)
        {
            startTransition(_sceneName);
            GetTransitionsManager.connection = true;
            GameManager.GetGameManager.ActivateCamera();
        }
    }

    public void createServer(int _sceneId)
    {
        if (!GetTransitionsManager.connection)
        {
            GameManager.GetGameManager.ActivateCamera();
            startTransition(_sceneId);
            GetTransitionsManager.connection = true;
            GetTransitionsManager.isServer = true;
        }
    }

    public void connectToServer(int _sceneId)
    {
        if (!GetTransitionsManager.connection)
        {
            GameManager.GetGameManager.ActivateCamera();
            startTransition(_sceneId);
            GetTransitionsManager.connection = true;
        }
    }
}
