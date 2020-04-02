using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour
{
    enum dir
    {
        bot,
        top,
        left,
        right
    }
    public bool useCheat = false;

    private static CheatManager instance = null;
    public static CheatManager GetCheatManager
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (!useCheat)
        {
            Destroy(gameObject);
            return;
        }
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            //Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        GameManager.GetGameManager.OnReturnMenu += Destroy;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.GetGameManager.OnReturnMenu -= Destroy;
    }

    // Use this for initialization
    void Start()
    {

    }

    IEnumerator ChangeScene(string sceneName, string idLink, dir dir)
    {
        PlayerController player = PlayerManager.GetPlayerManager.playerController;

        if (player && !player.IsTeleporting)
        {

            if (player.isLocalPlayer)
            {
                Scene newScene = SceneManager.GetSceneByName(sceneName);
                player.IsTeleporting = true;
                TransitionsManager.GetTransitionsManager.startTransitionInGame();

                yield return new WaitUntil(() => !TransitionsManager.GetTransitionsManager.InGame);

                string lastRoom = SceneManager.GetActiveScene().name;

                SetActiveLevel(false);
                SceneManager.SetActiveScene(newScene);
                SetActiveLevel(true);
                player.sceneId = newScene.buildIndex;

                if (!PersonalNetworkManager.isSERVER)
                {
                    player.CmdChangeRoom(player.sceneId);
                }

                GameObject.FindObjectOfType<SCamera>().bounds = MapManager.GetMapManager.FindBound();

                Link[] links = FindObjectsOfType<Link>();

                //Spawn player
                for (int i = 0; i < links.Length; i++)
                {
                    if (idLink == links[i].LinkId && links[i].SceneName == sceneName)
                    {
                        Vector3 newPos = links[i].gameObject.transform.position;
                        if (dir == dir.right)
                        {
                            newPos.x = links[i].gameObject.transform.position.x + 0.7f;
                            newPos.y = links[i].gameObject.transform.position.y;
                        }
                        else if (dir == dir.left)
                        {
                            newPos.x = links[i].gameObject.transform.position.x - 0.7f;
                            newPos.y = links[i].gameObject.transform.position.y;
                        }
                        else if (dir == dir.bot)
                        {
                            newPos.x = links[i].gameObject.transform.position.x;
                            newPos.y = links[i].gameObject.transform.position.y - 0.7f;
                        }
                        else if (dir == dir.top)
                        {
                            newPos.x = links[i].gameObject.transform.position.x;
                            newPos.y = links[i].gameObject.transform.position.y + 0.7f;
                        }
                        player.gameObject.transform.position = newPos;
                        Camera.main.GetComponent<SCamera>().transform.position = newPos;
                        player.IsTeleporting = false;
                        yield return null;
                    }
                }
            }
        }

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!useCheat)
        {
            return;
        }
        if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad0))
        {
            StartCoroutine(ChangeScene("LevelOne", "f3", dir.top));
        }
        else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad1))
        {
            StartCoroutine(ChangeScene("Neige", "n1", dir.right));
        }
        else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad2))
        {
            StartCoroutine(ChangeScene("Montagne", "m1", dir.right));
        }
        else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad3))
        {
            StartCoroutine(ChangeScene("Interrieur6", "l14", dir.top));
        }
        else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad4))
        {
            StartCoroutine(ChangeScene("Grotte", "g1", dir.bot));
        }
        else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad5))
        {
            StartCoroutine(ChangeScene("Grotte2", "g3", dir.top));
        }
        else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad6))
        {
            StartCoroutine(ChangeScene("Grotte3", "g5", dir.right));
        }
        else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad7))
        {
            StartCoroutine(ChangeScene("Grotte4", "g9", dir.top));
        }
        else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad8))
        {
            StartCoroutine(ChangeScene("Grotte5", "g12", dir.top));
        }
        else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Keypad9))
        {
            StartCoroutine(ChangeScene("Interrieur6", "l14", dir.top));
        }
    }

    public void SetActiveLevel(bool _enable)
    {
        if (!PersonalNetworkManager.isSERVER)
        {
            GameObject[] gms = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < gms.Length; i++)
            {
                gms[i].SetActive(_enable);
            }

        }
        else
        {
            //Change scene for the server
            GameObject[] gms = SceneManager.GetActiveScene().GetRootGameObjects();

            for (int i = 0; i < gms.Length; i++)
            {

                if (gms[i].name == "colliders group")
                {
                    gms[i].SetActive(_enable);
                    continue;
                }

                if (gms[i].GetComponent<Grid>())
                {
                    gms[i].SetActive(_enable);
                    continue;
                }

                //todo optmiser le cache misère serv
                SpriteRenderer[] spr;
                Collider2D[] colliders;
                foreach (Transform tr in gms[i].transform)
                {
                    if (tr.GetComponent<Link>())
                    {
                        tr.GetComponent<Link>().enabled = _enable;
                    }
                    if (tr.GetComponent<SpawnBirdTree>())
                    {
                        tr.GetComponent<SpawnBirdTree>().enabled = _enable;
                    }
                    if (tr.GetComponent<ParticleSystem>())
                    {
                        if (_enable)
                        {
                            tr.GetComponent<ParticleSystem>().Play();
                        }
                        else
                        {
                            tr.GetComponent<ParticleSystem>().Stop();
                            tr.GetComponent<ParticleSystem>().Clear();
                        }
                    }

                    spr = tr.gameObject.GetComponentsInParent<SpriteRenderer>();
                    colliders = tr.gameObject.GetComponentsInParent<Collider2D>();

                    if (spr.Length > 0)
                    {
                        foreach (SpriteRenderer s in spr)
                        {
                            s.enabled = _enable;
                        }
                    }

                    if (colliders.Length > 0)
                    {
                        foreach (Collider2D c in colliders)
                        {
                            c.enabled = _enable;
                        }

                    }

                    spr = tr.gameObject.GetComponentsInChildren<SpriteRenderer>();
                    colliders = tr.gameObject.GetComponentsInChildren<Collider2D>();
                    if (spr.Length > 0)
                    {
                        foreach (SpriteRenderer s in spr)
                        {
                            s.enabled = _enable;
                        }
                    }

                    if (colliders.Length > 0)
                    {
                        foreach (Collider2D c in colliders)
                        {
                            c.enabled = _enable;
                        }

                    }
                }

            }
        }
        bool canAddFurniture = ManagerScene.GetManagerScene.GetActiveScene().name.Contains("Interrieur");
        FurnitureManager.GetFurnitureManager.enabled = canAddFurniture;
    }
}

