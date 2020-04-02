using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Link : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    string nextRoom = "default";

    [SyncVar]
    [SerializeField]
    string sceneName = "default";

    [SyncVar]
    [SerializeField]
    string linkId = "";

    [SyncVar]
    [SerializeField]
    string nextId = "";

    public static string lastId = "";

    public static string lastRoomName = "";

    [SyncVar]
    [SerializeField]
    string idSmourb = "-1";

    [SyncVar]
    [SerializeField]
    float offset = 1.0f;

    [SyncVar]
    [SerializeField]
    string direction = "Vertical";

    [SyncVar]
    [SerializeField]
    string type = "Basic";

    #region assessors
    public string NextRoom
    {
        get
        {
            return nextRoom;
        }

        set
        {
            nextRoom = value;
        }
    }

    public string LinkId
    {
        get
        {
            return linkId;
        }

        set
        {
            linkId = value;
        }
    }

    public string NextId
    {
        get
        {
            return nextId;
        }

        set
        {
            nextId = value;
        }
    }

    public string IdSmourb
    {
        get
        {
            return idSmourb;
        }

        set
        {
            idSmourb = value;
        }
    }

    public string Direction
    {
        get
        {
            return direction;
        }

        set
        {
            direction = value;
        }
    }

    public string Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

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
    #endregion

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(ChangeSceneOnTrigger(collision));
        }
    }

    public IEnumerator ChangeSceneOnTrigger(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        bool posibleDir = false;

        if (player)
        {
            posibleDir = (Direction == "Vertical" && player.Direction.y != 0
                || Direction == "Horizontal" && player.Direction.x != 0
                || Direction == "Both"
                || Direction == "None");
        }

        if (idSmourb != "-1" && GameObject.Find("EncyclopediaSmourbiff").GetComponent<EncyclopediaSmourbiff>().dicoSmourbiff[int.Parse(idSmourb)])
        {
            gameObject.SetActive(false);
            StopCoroutine(ChangeSceneOnTrigger(collision));
        }
        else if (player && !player.IsTeleporting && posibleDir)
        {
            //fermeture du FurnitureManager sur changement de scène
            if (FurnitureManager.GetFurnitureManager != null && player.isLocalPlayer)
            {
                FurnitureManager.GetFurnitureManager.IsOpen = false;
            }

            /*if(!player.isLocalPlayer)
            {
                if (this is Teleporter)
                {
                    while (player.gameObject.GetComponent<TeleporterFX>().isTeleporting())
                    {
                        yield return null;
                    }
                }
            }*/

            if (player.isLocalPlayer)
            {
                Scene newScene = SceneManager.GetSceneByName(NextRoom);
                if (type == "Last")
                {
                    newScene = SceneManager.GetSceneByName(lastRoomName);
                }
                player.IsTeleporting = true;

                if (this is Teleporter)
                {
                    SoundManager.GetSoundManager.PlaySound("Teleport", 0.5f);
                    while (player.gameObject.GetComponent<TeleporterFX>().isTeleporting())
                    {
                        yield return null;
                    }
                }

                TransitionsManager.GetTransitionsManager.startTransitionInGame();

                yield return new WaitUntil(() => !TransitionsManager.GetTransitionsManager.InGame);

                string lastRoom = SceneManager.GetActiveScene().name;

                SetActiveLevel(false, player.isServer);
                SceneManager.SetActiveScene(newScene);
                SetActiveLevel(true, player.isServer);


                player.CmdChangeRoom(newScene.buildIndex);

                GameObject.FindObjectOfType<SCamera>().bounds = MapManager.GetMapManager.FindBound();
                if (newScene.name.Contains("Interrieur") || sceneName.Contains("Interrieur"))
                {
                    SoundManager.GetSoundManager.PlaySound("EnterDoor", 0.5f);
                }

                Link[] links = FindObjectsOfType<Link>();

                //Spawn player
                for (int i = 0; i < links.Length; i++)
                {
                    //Debug.Log(nextId + " == " + links[i].LinkId);
                    if (links[i] != this &&
                        (Type == "Basic" && nextRoom == links[i].sceneName && nextId == links[i].LinkId ||
                        Type == "Last" && lastRoomName == links[i].sceneName && lastId == links[i].LinkId))
                    {
                        lastId = linkId;
                        lastRoomName = lastRoom;
                        Vector3 newPos = links[i].gameObject.transform.position;
                        if (links[i].Direction == "Horizontal")
                        {
                            newPos.x = links[i].gameObject.transform.position.x + offset * player.Direction.x / Mathf.Abs(player.Direction.x);
                            newPos.y = links[i].gameObject.transform.position.y;
                        }
                        else if (links[i].Direction == "Vertical")
                        {
                            newPos.x = links[i].gameObject.transform.position.x;
                            newPos.y = links[i].gameObject.transform.position.y + offset * player.Direction.y / Mathf.Abs(player.Direction.y);
                        }
                        else if (links[i].Direction == "Both")
                        {
                            newPos = links[i].gameObject.transform.position + (Vector3)player.Direction * offset;
                        }
                        else if (links[i].Direction == "None")
                        {
                            newPos = links[i].gameObject.transform.position - (transform.position - player.transform.position);
                        }
                        player.gameObject.transform.position = newPos;
                        Camera.main.GetComponent<SCamera>().transform.position = newPos;

                        player.IsTeleporting = false;
                        yield return null;
                    }
                }
                SoundManager.GetSoundManager.ChangeMusicByScene(newScene.buildIndex);
            }
        }
    }

    public void SetActiveLevel(bool _enable, bool _isServer)
    {

        if (!_isServer)
        {
            Debug.Log("not server");
            GameObject[] gms = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < gms.Length; i++)
            {
                gms[i].SetActive(_enable);
                if (gms[i].GetComponent<SpawnerSmourbiff>())
                {
                    gms[i].GetComponent<SpawnerSmourbiff>().enabled = _enable;
                    if (_enable)
                    {
                        gms[i].GetComponent<SpawnerSmourbiff>().idSmourbiff = int.Parse(idSmourb);
                    }
                }

                if (gms[i].transform.GetComponent<Link>())
                {
                    Link l = gms[i].transform.GetComponent<Link>();
                    if (l is Teleporter)
                    {
                        Teleporter t = l as Teleporter;
                        t.Radius = 1.0f;
                    }
                }
            }

        }
        else
        {
            Debug.Log("is server");
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

                if (gms[i].GetComponent<SpawnerSmourbiff>())
                {
                    gms[i].GetComponent<SpawnerSmourbiff>().enabled = _enable;
                    if (_enable)
                    {
                        gms[i].GetComponent<SpawnerSmourbiff>().idSmourbiff = int.Parse(idSmourb);
                    }
                }
                //todo optmiser le cache misère serv
                SpriteRenderer[] spr;
                Collider2D[] colliders;
                Animator[] anims;
                foreach (Transform tr in gms[i].transform)
                {
                    if (tr.GetComponent<PathFish>())
                    {
                        tr.GetComponent<PathFish>().gameObject.SetActive(_enable);
                    }
                    if (tr.GetComponent<Link>())
                    {
                        tr.GetComponent<Link>().enabled = _enable;
                    }
                    if (tr.GetComponentInChildren<Link>())
                    {
                        Link l = tr.GetComponentInChildren<Link>();
                        l.enabled = _enable;
                        if (l is Teleporter)
                        {
                            Teleporter t = l as Teleporter;
                            t.Radius = 1.0f;
                        }
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
                    anims = tr.gameObject.GetComponentsInParent<Animator>();

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

                    if (anims.Length > 0)
                    {
                        foreach (Animator a in anims)
                        {
                            a.enabled = _enable;
                        }

                    }

                    spr = tr.gameObject.GetComponentsInChildren<SpriteRenderer>();
                    colliders = tr.gameObject.GetComponentsInChildren<Collider2D>();
                    anims = tr.gameObject.GetComponentsInChildren<Animator>();
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

                    if (anims.Length > 0)
                    {
                        foreach (Animator a in anims)
                        {
                            a.enabled = _enable;
                        }

                    }
                }

            }
        }
        bool canAddFurniture = ManagerScene.GetManagerScene.GetActiveScene().name.Contains("Interrieur");
        FurnitureManager.GetFurnitureManager.enabled = canAddFurniture;
    }
}
