using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public delegate void DelegateReturnMenu();
    public event DelegateReturnMenu OnReturnMenu = () => { };
    public static GameManager GetGameManager { get; private set; }

    [SerializeField] Canvas prefabCanvasMenu;

    [SerializeField] Camera mainCam;

    bool gameIsPaused;

    Button continueButton;

    Button backMenuButton;

    public Canvas menuGo;

    bool isMenuCreated;

    bool hasAlreadySeeMenu;

    public bool GameIsPaused
    {
        get
        {
            return gameIsPaused;
        }
    }

    public bool HasAlreadySeeMenu
    {
        get
        {
            return hasAlreadySeeMenu;
        }

        set
        {
            hasAlreadySeeMenu = value;
        }
    }

    private void Awake()
    {
        if (GetGameManager == null)
        {
            GetGameManager = this;
        }
        else if (GetGameManager != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameIsPaused = false;
        isMenuCreated = false;
        HasAlreadySeeMenu = false;
        //CreateCanvasMenuInGame();
    }

    public void CreateCanvasMenuInGame()
    {
        menuGo = Instantiate(prefabCanvasMenu);
        menuGo.gameObject.AddComponent<DontDestroy>();
        menuGo.gameObject.SetActive(false);
        menuGo.transform.Find("Continue").GetComponent<Button>().onClick.AddListener(ContinueGame);
        isMenuCreated = true;
    }

    public void ActivateDelegateMenu()
    {
        gameIsPaused = false;
        OnReturnMenu();
        PlayerManager.GetPlayerManager.ClearListConnectedPlayers();
        isMenuCreated = false;
        if (mainCam != null)
            mainCam.enabled = false;
    }

    private void Update()
    {
        bool allIsClose = ChatManager.GetChatManager != null && !ChatManager.GetChatManager.ChatIsOpen &&
            HUDManager.GetHUDManager != null && !HUDManager.GetHUDManager.IsOpen;
        if (allIsClose && InputManager.GetInputManager.GetButtonDown("Cancel", true)
            && ManagerScene.GetManagerScene.GetActiveSceneID() != 0 && ManagerScene.GetManagerScene.GetActiveSceneID() != 1 &&
            ManagerScene.GetManagerScene.GetActiveSceneID() != 2 &&
            ManagerScene.GetManagerScene.GetActiveSceneID() != 3)
        {
            if (!isMenuCreated)
                CreateCanvasMenuInGame();
            menuGo.gameObject.SetActive(!menuGo.gameObject.activeSelf);
            if (!gameIsPaused)
            {
                PauseGame();
            }
            else
            {
                ContinueGame();
            }
        }
    }

    public void ActivateCamera()
    {
        if (mainCam == null)
            mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        mainCam.enabled = true;
        GameObject.Find("CameraMenu").gameObject.SetActive(false);
        StartCoroutine(CenterCamera());
    }

    IEnumerator CenterCamera()
    {
        while (PlayerManager.GetPlayerManager.currentPlayer == null)
        {
            yield return null;
        }
        mainCam.transform.position = PlayerManager.GetPlayerManager.currentPlayer.transform.position;
    }

    public void PauseGame()
    {
        gameIsPaused = true;
    }

    public void ContinueGame()
    {
        menuGo.gameObject.SetActive(false);

        gameIsPaused = false;
    }
}
