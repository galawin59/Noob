using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    public delegate void DelegateHelpResources();
    public event DelegateHelpResources OnHelp = () => { };
    public event DelegateHelpResources OnDesactivate = () => { };

    public delegate void DelegateHarvesting(float _value);
    public event DelegateHarvesting onHarvesting = (float _value) => { };
    public event DelegateHarvesting onStopHarvesting = (float _value) => { };

    static PlayerController instance;
    [SerializeField] float speed = 2.0f;
    [SerializeField] GameObject dust;
    [SerializeField] GameObject stun;
    [SerializeField] Transform posPivot;
    [SerializeField] GameObject smourbiff;
    [SerializeField] GameObject smourbiffPrefabs;
    [SerializeField] Transform smourbiffSpawn;
    [SerializeField] GameObject reflect;
    [SerializeField] Vector2 reflectOffset;

    GameObject[] duckStun;
    float[] duckRotation;
    float radiusStun = 0.35f;

    SCamera sCam;
    Rigidbody2D rgbd;
    new BoxCollider2D collider;

    Vector2 velocity;
    Vector2 direction;

    public bool isHelpActive;
    bool isMoving;
    bool isMovingVerticaly;
    bool isMovingHorizontaly;
    float basicSpeed;

    bool isTeleporting = false;

    bool isStunned;

    bool isHarvesting;

    static bool machin = false;
    static int truc = 0;

    public bool IsHarvesting
    {
        get
        {
            return isHarvesting;
        }
    }

    public bool IsStunned
    {
        get
        {
            return isStunned;
        }
    }

#if UNITY_EDITOR
    //[Range(0, 50)]
    //public int segments = 50;
    //LineRenderer line;
#endif
    float radius = 0.5f;
    public NetworkInstanceId id { get; private set; }

    int resourcesLayer;
    Animator anim;
    SpriteRenderer spriteBody;
    SpriteRenderer spriteScar;
    SpriteRenderer spriteBeard;
    SpriteRenderer spriteRendererHair;
    SpriteRenderer spriteCape;
    SpriteRenderer spriteSpecial;
    SpriteRenderer mySpriteRendererHair;
    SpriteRenderer mySpriteBody;
    SpriteRenderer mySpriteScar;
    SpriteRenderer mySpriteBeard;
    SpriteRenderer mySpriteCape;
    SpriteRenderer mySpriteSpecial;
    Dictionary<PartBody, SpriteRenderer> dicSpriteBody;
    Dictionary<PartBody, SpriteRenderer> myDicSpriteBody;
    SpriteManager sm;
    Player player;
    IconsUI displayIcons;
    Fishing fishingActivity;
    Collider2D playerCollider;

    #region Sync

    [SyncVar]
    public int currentHairCut = 0;

    [SyncVar]
    public bool hasAHat = false;

    [SyncVar]
    public int currentScar = 0;

    [SyncVar]
    public int currentBeard = 0;

    [SyncVar]
    public int currentBody = 16;

    [SyncVar]
    public int currentTorse = 0;

    [SyncVar]
    public int currentPant = 0;

    [SyncVar]
    public int currentHead = 0;

    [SyncVar]
    public int currentCape = 0;

    [SyncVar]
    public int currentAnim = 0;

    [SyncVar]
    public ColorArmor currentColorTorse = 0;

    [SyncVar]
    public ColorArmor currentColorPant = 0;

    [SyncVar]
    public ColorArmor currentColorHead = 0;

    [SyncVar(hook = "OnChangeCursor")]
    public int currentCursor = 0;

    [SyncVar(hook = "OnChangeName")]
    public string namePlayer;

    [SyncVar]
    public Sex sexPlayer;

    [SyncVar]
    public Scars typeScar;

    [SyncVar]
    public TypeHair currentTypeHair = 0;

    [SyncVar]
    public TypeBeard currentTypeBeard = 0;

    [SyncVar]
    public ColorBeard currentColorBeard = 0;

    [SyncVar]
    public ColorHair colorHairPlayer;

    [SyncVar]
    public ColorBody colorBodyPlayer;

    [SyncVar]
    public Archetype archetype;

    [SyncVar]
    public Cape cape;

    [SyncVar]
    public ColorCape colorCape;

    [SyncVar]
    public bool isSpecialCharacter;

    [SyncVar]
    public SpecialsCharacters specialsCharacters;

    //to do starting scene id change
    //A CHANGER SUR LE PREFAB
    [SyncVar(hook = "OnChangeLevel")]
    public int sceneId = 3;

    public int lastsceneId = 0;
    #endregion
    // Use this for initialization

    public Vector2 Direction
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

    public bool IsTeleporting
    {
        get
        {
            return isTeleporting;
        }

        set
        {
            isTeleporting = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        lastsceneId = sceneId;
    }

    void OnChangeCursor(int _currentCursor)
    {
        currentCursor = _currentCursor;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = SpriteManager.GetSpriteManager.Cursors[_currentCursor];
    }

    void OnChangeName(string _name)
    {
        namePlayer = _name;
        GetComponentInChildren<Text>().text = _name;
        namePlayer = _name;
    }

    void HideObject(GameObject _player, bool _enable)
    {
        _player.GetComponent<SpriteRenderer>().enabled = _enable;
        _player.GetComponent<Animator>().enabled = _enable;
        _player.GetComponent<Collider2D>().enabled = _enable;
        SpriteRenderer[] sprites = _player.GetComponentsInChildren<SpriteRenderer>();
        Text[] texts = _player.GetComponentsInChildren<Text>();

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].enabled = _enable;
        }
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = _enable;
        }

    }

    public void OnChangeLevel(int _sceneId)
    {
        //PlayerManager.GetPlayerManager.listConnectedPlayers[sceneId].Remove(gameObject);
        //        Debug.Log("//////////////////////////////////////////////////////////////////////////////////");
        //        Debug.Log("lastsceneId " + lastsceneId + " _sceneId " + _sceneId);

        if (!isServer)
        {
            lastsceneId = sceneId;
            sceneId = _sceneId;
        }

        if (!isLocalPlayer)
        {
            if (_sceneId != PlayerManager.GetPlayerManager.playerController.sceneId)
            {
                HideObject(gameObject, false);
            }
            else
            {
                HideObject(gameObject, true);
            }
        }
        else
        {
            foreach (GameObject gmPlayer in PlayerManager.GetPlayerManager.listConnectedPlayers[lastsceneId])
            {
                if (!gmPlayer.GetComponent<PlayerController>().isLocalPlayer)
                {
                    HideObject(gmPlayer, false);
                }
            }

            foreach (GameObject gmPlayer in PlayerManager.GetPlayerManager.listConnectedPlayers[_sceneId])
            {
                if (!gmPlayer.GetComponent<PlayerController>().isLocalPlayer)
                {
                    HideObject(gmPlayer, true);
                }
            }
        }

        if (lastsceneId != _sceneId)
        {
            if (PlayerManager.GetPlayerManager.listConnectedPlayers[lastsceneId].Contains(gameObject))
            {
                PlayerManager.GetPlayerManager.listConnectedPlayers[lastsceneId].Remove(gameObject);
                //                Debug.Log("Remove player " + namePlayer + " in scene " + lastsceneId);
            }
        }

        if (!PlayerManager.GetPlayerManager.listConnectedPlayers[_sceneId].Contains(gameObject))
        {
            PlayerManager.GetPlayerManager.listConnectedPlayers[_sceneId].Add(gameObject);
            //            Debug.Log("Add palyer " + namePlayer + " in scene " + _sceneId);
        }

        /*foreach (GameObject gmPlayer in PlayerManager.GetPlayerManager.listConnectedPlayers[lastsceneId])
        {

            if (!gmPlayer.GetComponent<PlayerController>().isLocalPlayer)
            {
                //Debug.Log(gmPlayer.GetComponent<PlayerController>().namePlayer+" 1er");
                //cache misère serveur
                //  if (isServer)
                // {
                gmPlayer.GetComponent<SpriteRenderer>().enabled = false;
                gmPlayer.GetComponent<Collider2D>().enabled = false;
                SpriteRenderer[] sprites = gmPlayer.GetComponentsInChildren<SpriteRenderer>();
                Text[] texts = gmPlayer.GetComponentsInChildren<Text>();

                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].enabled = false;
                }
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i].enabled = false;
                }

                /*  }
                  else
                  {
                      Debug.Log("cli");
                      gmPlayer.SetActive(false);
                  }*/
        //        }
        //   }

        /*foreach (GameObject gmPlayer in PlayerManager.GetPlayerManager.listConnectedPlayers[_sceneId])
        {
            //cache misère serveur
            if (!gmPlayer.GetComponent<PlayerController>().isLocalPlayer)
            {
                // Debug.Log(gmPlayer.GetComponent<PlayerController>().namePlayer + " 2eme");
                //   Debug.Log(gmPlayer.name);
                // if (isServer)
                //  {
                gmPlayer.GetComponent<SpriteRenderer>().enabled = true;
                gmPlayer.GetComponent<Collider2D>().enabled = true;
                SpriteRenderer[] sprites = gmPlayer.GetComponentsInChildren<SpriteRenderer>();
                Text[] texts = gmPlayer.GetComponentsInChildren<Text>();

                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].enabled = true;
                }
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i].enabled = true;
                }

                /*  }
                  else
                  {*/

        //gmPlayer.SetActive(true);
        // }
        //   }
        // }


        /*       Debug.Log("last scene :");
              foreach (GameObject gmPlayer in PlayerManager.GetPlayerManager.listConnectedPlayers[lastsceneId])
               {
                   Debug.Log(gmPlayer.GetComponent<PlayerController>().namePlayer);
               }
               Debug.Log("new scene :");
               foreach (GameObject gmPlayer in PlayerManager.GetPlayerManager.listConnectedPlayers[_sceneId])
               {
                   Debug.Log(gmPlayer.GetComponent<PlayerController>().namePlayer);
               }
               Debug.Log("//////////////////////////////////////////////////////////////////////////////////");
       */
        /*foreach (KeyValuePair<int, List<GameObject>> entry in PlayerManager.GetPlayerManager.listConnectedPlayers)
        {
            Debug.Log("scene " + entry.Key);
            foreach (GameObject gmPlayer in entry.Value)
            {
                Debug.Log(gmPlayer.GetComponent<PlayerController>().namePlayer);
            }
        }*/
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdChangingAppearance(PlayerManager.GetPlayerManager.CurrentCursor, 16, PlayerManager.GetPlayerManager.Player.PlayerName,
            PlayerManager.GetPlayerManager.Player.Sexe, PlayerManager.GetPlayerManager.Player.HairColor, PlayerManager.GetPlayerManager.Player.BodyColor,
             PlayerManager.GetPlayerManager.Player.HairType, PlayerManager.GetPlayerManager.Player.Archetype, PlayerManager.GetPlayerManager.Player.Scars,
             PlayerManager.GetPlayerManager.Player.TypeBeard, PlayerManager.GetPlayerManager.Player.ColorBeard, PlayerManager.GetPlayerManager.Player.Cape,
             PlayerManager.GetPlayerManager.Player.ColorCape, PlayerManager.GetPlayerManager.Player.IsSpecialCharacter, PlayerManager.GetPlayerManager.Player.SpecialsCharacters);
        ColorArmor[] tmpColorArray = new ColorArmor[(int)PartBody.nbPart];
        for (int i = 0; i < (int)PartBody.nbPart; i++)
        {
            tmpColorArray[i] = PlayerManager.GetPlayerManager.Player.ArmorColor[(PartBody)i];
        }
        CmdSyncColorArmor(tmpColorArray);
        CmdSyncHasAHat(PlayerManager.GetPlayerManager.Player.HasAHat);
        truc = 0;
        machin = false;
    }

    [Command]
    void CmdSyncHasAHat(bool _hasAHat)
    {
        hasAHat = _hasAHat;
    }

    [Command]
    void CmdChangingAppearance(int cursor, int hairCut, string playerName, Sex _sex, ColorHair _colorHair, ColorBody _colorBody, TypeHair _typeHair, Archetype _archetype,
        Scars _scar, TypeBeard _typeBeard, ColorBeard _colorBeard, Cape _cape, ColorCape _colorCape, bool isSpecial, SpecialsCharacters _specialsCharacters)
    {
        currentHairCut = hairCut;
        currentCursor = cursor;
        namePlayer = playerName;
        sexPlayer = _sex;
        colorHairPlayer = _colorHair;
        colorBodyPlayer = _colorBody;
        currentTypeHair = _typeHair;
        currentBody = 16;
        typeScar = _scar;
        currentColorBeard = _colorBeard;
        currentTypeBeard = _typeBeard;
        archetype = _archetype;
        cape = _cape;
        colorCape = _colorCape;
        isSpecialCharacter = isSpecial;
        specialsCharacters = _specialsCharacters;
    }

    [Command]
    void CmdSyncSprite(int hairCut)
    {
        currentHairCut = hairCut;
    }

    [Command]
    public void CmdChangeRoom(int _id)
    {
        lastsceneId = sceneId;
        sceneId = _id;
    }

    [Command]
    public void CmdDisplayBubble(string _sprite, int _sceneId)
    {
        foreach (GameObject g in PlayerManager.GetPlayerManager.listConnectedPlayers[_sceneId])
        {
            TargetDisplayBubble(g.GetComponent<NetworkIdentity>().connectionToClient, _sprite, _sceneId);
        }
    }

    [TargetRpc]
    public void TargetDisplayBubble(NetworkConnection networkConnection, string _sprite, int _sceneId)
    {
        displayIcons.DisplayBubbleClient(_sprite, _sceneId);
    }

    [Command]
    void CmdSyncArmor(int[] armor)
    {
        currentTorse = armor[(int)PartBody.Torse];
        currentPant = armor[(int)PartBody.Pant];
        currentHead = armor[(int)PartBody.Head];
    }

    [Command]
    void CmdSyncColorArmor(ColorArmor[] colorArmor)
    {
        currentColorTorse = colorArmor[(int)PartBody.Torse];
        currentColorPant = colorArmor[(int)PartBody.Pant];
        currentColorHead = colorArmor[(int)PartBody.Head];
    }

    [Command]
    void CmdSyncBody(int body)
    {
        currentBody = body;
    }

    [Command]
    void CmdSyncCape(int _cape)
    {
        currentCape = _cape;
    }

    [Command]
    void CmdSyncScar(int scar)
    {
        currentScar = scar;
    }

    [Command]
    void CmdSyncBeard(int beard)
    {
        currentBeard = beard;
    }

    [Command]
    void CmdSyncSpecialCharacter(int _currentAnim)
    {
        currentAnim = _currentAnim;
    }

    //bande de pd
#if UNITY_EDITOR
    //void CreateRadius()
    //{
    //    float x;
    //    float y;

    //    float angle = 20f;

    //    for (int i = 0; i < (segments + 1); i++)
    //    {
    //        x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
    //        y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

    //        line.SetPosition(i, new Vector2(x, y));

    //        angle += (380f / segments);
    //    }
    //}

#endif

    IEnumerator Start()
    {
        if (!PlayerManager.GetPlayerManager.listConnectedPlayers[sceneId].Contains(gameObject))
            PlayerManager.GetPlayerManager.listConnectedPlayers[sceneId].Add(gameObject);

        sm = SpriteManager.GetSpriteManager;
        sCam = GameObject.Find("Main Camera").GetComponent<SCamera>();
        collider = GetComponent<BoxCollider2D>();
        rgbd = GetComponent<Rigidbody2D>();
        Direction = -Vector2.up;
        isHelpActive = false;
        basicSpeed = speed;
        resourcesLayer = 1 << LayerMask.NameToLayer("InteractableObjects") | 1 << LayerMask.NameToLayer("Pnj");

        transform.Find("Cursor").GetComponent<SpriteRenderer>().sprite = sm.Cursors[currentCursor];
        GetComponentInChildren<Text>().text = namePlayer;
        anim = GetComponent<Animator>();
        //uncomment to show radius
#if UNITY_EDITOR
        //line = gameObject.AddComponent<LineRenderer>();

        //Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));

        //line.materials[0].mainTexture = whiteDiffuseMat.mainTexture;
        //line.materials[0].color = Color.blue;


        //line.positionCount = segments + 1;
        //line.useWorldSpace = false;
        //CreateRadius();
#endif
        spriteBody = GetComponent<SpriteRenderer>();
        spriteRendererHair = transform.Find("Hair").GetComponent<SpriteRenderer>();
        spriteScar = transform.Find("Scar").GetComponent<SpriteRenderer>();
        spriteBeard = transform.Find("Beard").GetComponent<SpriteRenderer>();
        spriteCape = transform.Find("Cape").GetComponent<SpriteRenderer>();
        spriteSpecial = transform.Find("Special").GetComponent<SpriteRenderer>();
        dicSpriteBody = new Dictionary<PartBody, SpriteRenderer>();
        dicSpriteBody[PartBody.Pant] = transform.Find("Pant").GetComponent<SpriteRenderer>();
        dicSpriteBody[PartBody.Torse] = transform.Find("Torse").GetComponent<SpriteRenderer>();
        dicSpriteBody[PartBody.Head] = transform.Find("Head").GetComponent<SpriteRenderer>();
        player = PlayerManager.GetPlayerManager.Player;
        fishingActivity = GetComponent<Fishing>();
        displayIcons = transform.GetComponentInChildren<IconsUI>();

        if (isLocalPlayer)
        {
            duckStun = new GameObject[6];
            duckRotation = new float[6];
            PlayerManager.GetPlayerManager.currentPlayer = gameObject;
            PlayerManager.GetPlayerManager.currentPlayer.name = PlayerManager.GetPlayerManager.Player.PlayerName;
            PlayerManager.GetPlayerManager.playerController = this;
            PlayerManager.GetPlayerManager.playerIdentity = GetComponent<NetworkIdentity>();
            isStunned = false;
            isHarvesting = false;
            mySpriteBody = GetComponent<SpriteRenderer>();
            myDicSpriteBody = new Dictionary<PartBody, SpriteRenderer>();
            myDicSpriteBody[PartBody.Pant] = transform.Find("Pant").GetComponent<SpriteRenderer>();
            myDicSpriteBody[PartBody.Torse] = transform.Find("Torse").GetComponent<SpriteRenderer>();
            myDicSpriteBody[PartBody.Head] = transform.Find("Head").GetComponent<SpriteRenderer>();
            mySpriteRendererHair = transform.Find("Hair").GetComponent<SpriteRenderer>();
            mySpriteScar = transform.Find("Scar").GetComponent<SpriteRenderer>();
            mySpriteBeard = transform.Find("Beard").GetComponent<SpriteRenderer>();
            mySpriteCape = transform.Find("Cape").GetComponent<SpriteRenderer>();
            mySpriteSpecial = transform.Find("Special").GetComponent<SpriteRenderer>();
            currentHairCut = 16;
            currentCursor = player.CurrentCursor;
            namePlayer = player.PlayerName;
            ReskinAnimation();
            Smourbiff.OnTryCatch += OnTryCatch;
            GameManager.GetGameManager.OnReturnMenu += Destroy;

        }
        else
        {
            while (PlayerManager.GetPlayerManager == null || PlayerManager.GetPlayerManager.currentPlayer == null)
            {
                yield return null;
            }

            if (isServer)
            {
                OnChangeLevel(sceneId);
                foreach (KeyValuePair<int, List<GameObject>> entry in PlayerManager.GetPlayerManager.listConnectedPlayers)
                {
                    foreach (GameObject go in entry.Value)
                    {
                        //add gameobject into clients dictionary
                        RpcSendDic(go, entry.Key);
                    }
                }

                /* foreach (KeyValuePair<int, List<GameObject>> entry in PlayerManager.GetPlayerManager.listConnectedPlayers)
                 {
                     Debug.Log("scene " + entry.Key);
                     foreach (GameObject gmPlayer in entry.Value)
                     {
                         Debug.Log(gmPlayer.GetComponent<PlayerController>().namePlayer);
                     }
                 }*/
            }
        }
        Collider2D[] collider2Ds = GetComponents<Collider2D>();
        foreach (Collider2D item in collider2Ds)
        {
            if (!item.isTrigger)
            {
                playerCollider = item;
            }
        }

        reflectOffset = reflect.transform.localPosition;

    }

    [ClientRpc]
    void RpcSendDic(GameObject go, int sceneID)
    {
        StartCoroutine(RefreshPlayers(go, sceneID));
    }

    IEnumerator RefreshPlayers(GameObject go, int sceneID)
    {
        while (PlayerManager.GetPlayerManager == null || PlayerManager.GetPlayerManager.currentPlayer == null)
        {
            yield return null;
        }

        if (!isServer)
        {

            if (!PlayerManager.GetPlayerManager.listConnectedPlayers[sceneId].Contains(go))
            {
                PlayerManager.GetPlayerManager.listConnectedPlayers[sceneID].Add(go);
            }

            //Refreshs player at the first connection on local player client
            foreach (KeyValuePair<int, List<GameObject>> entry in PlayerManager.GetPlayerManager.listConnectedPlayers)
            {
                if (entry.Key == PlayerManager.GetPlayerManager.currentPlayer.GetComponent<PlayerController>().sceneId)
                {
                    foreach (GameObject gmPlayer in entry.Value)
                    {
                        if (!gmPlayer.GetComponent<PlayerController>().isLocalPlayer)
                        {
                            //Debug.Log("true " + gmPlayer.GetComponent<PlayerController>().namePlayer);
                            HideObject(gmPlayer, true);
                        }
                    }
                }
                else
                {
                    foreach (GameObject gmPlayer in entry.Value)
                    {
                        if (!gmPlayer.GetComponent<PlayerController>().isLocalPlayer)
                        {
                            //Debug.Log("false " + gmPlayer.GetComponent<PlayerController>().namePlayer);
                            HideObject(gmPlayer, false);
                        }
                    }
                }
            }


            /* Debug.Log("RPC sceneID " + sceneID);

             foreach (KeyValuePair<int, List<GameObject>> entry in PlayerManager.GetPlayerManager.listConnectedPlayers)
             {
                 Debug.Log("scene " + entry.Key);
                 foreach (GameObject gmPlayer in entry.Value)
                 {
                     Debug.Log(gmPlayer.GetComponent<PlayerController>().namePlayer);
                 }
             }*/
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //pour le spawn car unity c'est de la merde
        if (!machin)
        {
            transform.position = new Vector3(15.0f, -87.0f);
            truc++;
            if (truc > 2)
                machin = true;
        }
        if (!ChatManager.chatIsFocused)
        {
            if (!isTeleporting)
            {
                if (!fishingActivity.IsLaunched)
                    fishingActivity.LaunchHook(direction, transform.position);

                if (InputManager.GetInputManager.GetButtonDown("Interact") && !isHarvesting)
                {
                    StartCoroutine(Interact());
                }

                if (InputManager.GetInputManager.GetButton("ResourcesHelp"))
                {
                    if (!isHelpActive)
                    {
                        OnHelp();
                        isHelpActive = true;
                    }
                }
                else if (isHelpActive)
                {

                    OnDesactivate();
                    isHelpActive = false;
                }

                if (InputManager.GetInputManager.GetButtonDown("DisplayIconsUI"))
                {
                    displayIcons.EnableIcon();
                }
            }

            if (fishingActivity.IsFishing)
            {
                fishingActivity.GetFish();
            }

        }

        if (!isStunned &&
            !isTeleporting &&
            !GameManager.GetGameManager.GameIsPaused &&
            !fishingActivity.IsLaunched &&
            !isHarvesting)
            Movement();
        else
        {
            rgbd.velocity = Vector2.zero;
            anim.SetBool("isMoving", false);
            if (smourbiff != null && isTeleporting)
            {
                smourbiff.transform.position = transform.position;
            }
        }


        int[] tmpArray = new int[(int)PartBody.nbPart];
        ColorArmor[] tmpColorArray = new ColorArmor[(int)PartBody.nbPart];

        if (!isServer)
        {
            if (player.IsSpecialCharacter)
            {
                CmdSyncSpecialCharacter(sm.specialsCharacters[(int)specialsCharacters].FindIndex(item => item.name == mySpriteSpecial.sprite.name));
            }
            else
            {
                if (player.HairType != TypeHair.HairLess)
                    CmdSyncSprite(sm.hairList[(int)sexPlayer, (int)currentTypeHair, (int)colorHairPlayer].FindIndex(item => item.name == mySpriteRendererHair.sprite.name));

                if (player.Cape != Cape.None)
                    CmdSyncCape(sm.accessoriesList[(int)cape, (int)colorCape].FindIndex(item => item.name == mySpriteCape.sprite.name));

                for (int i = 0; i < (int)PartBody.nbPart; i++)
                {
                    if (player.ArmorColor[(PartBody)i] != ColorArmor.None)
                    {
                        if (myDicSpriteBody[(PartBody)i].sprite != null)
                        {
                            tmpArray[i] = sm.armorList[(int)sexPlayer, (int)archetype, (int)player.ArmorColor[(PartBody)i], i].FindIndex(item => item.name == myDicSpriteBody[(PartBody)i].sprite.name);
                        }
                    }
                    tmpColorArray[i] = player.ArmorColor[(PartBody)i];
                }
                CmdSyncColorArmor(tmpColorArray);
                CmdSyncArmor(tmpArray);
                CmdSyncBody(sm.bodyList[(int)colorBodyPlayer].FindIndex(item => item.name == mySpriteBody.sprite.name));

                if (mySpriteScar.sprite != null)
                {
                    if (player.Scars != Scars.None)
                        CmdSyncScar(sm.scarsList[(int)typeScar].FindIndex(item => item.name == mySpriteScar.sprite.name));
                }
                else
                {
                    CmdSyncScar(-1);
                }

                if (mySpriteBeard.sprite != null && player.ArmorColor[PartBody.Head] == ColorArmor.None)
                {
                    if (player.TypeBeard != TypeBeard.None)
                        CmdSyncBeard(sm.beardList[(int)player.TypeBeard, (int)player.ColorBeard].FindIndex(item => item.name == mySpriteBeard.sprite.name));
                }
                else
                {
                    CmdSyncBeard(-1);
                }
            }
        }
        else
        {
            if (player.IsSpecialCharacter)
            {
                currentAnim = sm.specialsCharacters[(int)specialsCharacters].FindIndex(item => item.name == mySpriteSpecial.sprite.name);
            }
            else
            {
                if (player.HairType != TypeHair.HairLess)
                {
                    currentHairCut = sm.hairList[(int)player.Sexe, (int)player.HairType, (int)player.HairColor].FindIndex(item => item.name == mySpriteRendererHair.sprite.name);
                }

                if (player.Cape != Cape.None)
                {
                    currentCape = sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].FindIndex(item => item.name == mySpriteCape.sprite.name);
                }

                if (myDicSpriteBody[PartBody.Head].sprite != null)
                {
                    if (player.ArmorColor[PartBody.Head] != ColorArmor.None)
                        currentHead = sm.armorList[(int)sexPlayer, (int)player.Archetype, (int)player.ArmorColor[PartBody.Head], (int)PartBody.Head].FindIndex(item => item.name == myDicSpriteBody[PartBody.Head].sprite.name);
                }

                if (myDicSpriteBody[PartBody.Torse].sprite != null)
                {
                    if (player.ArmorColor[PartBody.Torse] != ColorArmor.None)
                        currentTorse = sm.armorList[(int)sexPlayer, (int)player.Archetype, (int)player.ArmorColor[PartBody.Torse], (int)PartBody.Torse].FindIndex(item => item.name == myDicSpriteBody[PartBody.Torse].sprite.name);
                }

                if (myDicSpriteBody[PartBody.Pant].sprite != null)
                {
                    if (player.ArmorColor[PartBody.Pant] != ColorArmor.None)
                        currentPant = sm.armorList[(int)sexPlayer, (int)player.Archetype, (int)player.ArmorColor[PartBody.Pant], (int)PartBody.Pant].FindIndex(item => item.name == myDicSpriteBody[PartBody.Pant].sprite.name);
                }

                currentColorHead = player.ArmorColor[PartBody.Head];

                currentColorPant = player.ArmorColor[PartBody.Pant];

                currentColorTorse = player.ArmorColor[PartBody.Torse];

                currentBody = sm.bodyList[(int)colorBodyPlayer].FindIndex(item => item.name == mySpriteBody.sprite.name);

                if (mySpriteScar.sprite != null)
                {
                    if (player.Scars != Scars.None)
                        currentScar = sm.scarsList[(int)player.Scars].FindIndex(item => item.name == mySpriteScar.sprite.name);
                }
                else
                {
                    currentScar = -1;
                }
                if (mySpriteBeard.sprite != null && player.ArmorColor[PartBody.Head] == ColorArmor.None)
                {
                    if (player.TypeBeard != TypeBeard.None)
                        currentBeard = sm.beardList[(int)player.TypeBeard, (int)player.ColorBeard].FindIndex(item => item.name == mySpriteBeard.sprite.name);
                }
                else
                {
                    currentBeard = -1;
                }
            }
        }

        WalkWithPet();
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
        {
            if (!PersonalNetworkManager.isSERVER)
            {
                Debug.Log("non serveur");
                NetworkManager.singleton.StopClient();
                ManagerScene.GetManagerScene.ChangeCurrentScene(0);
            }
            else
            {
                Debug.Log("serveur");
                NetworkManager.singleton.StopHost();
            }
        }
        Smourbiff.OnTryCatch -= OnTryCatch;
        GameManager.GetGameManager.OnReturnMenu -= Destroy;
        PlayerManager.GetPlayerManager.listConnectedPlayers[sceneId].Remove(gameObject);
    }

    public void ReskinAnimation()
    {
        if (!isLocalPlayer)
        {
            if (isSpecialCharacter)
            {
                spriteSpecial.sprite = sm.specialsCharacters[(int)specialsCharacters][currentAnim];
                spriteSpecial.color = Color.white;
                spriteRendererHair.color = sm.WhiteTransparentColor;
                spriteCape.color = sm.WhiteTransparentColor;
                spriteBeard.color = sm.WhiteTransparentColor;
                spriteScar.color = sm.WhiteTransparentColor;
                spriteBody.color = sm.WhiteTransparentColor;
                for (int i = 0; i < (int)PartBody.nbPart; i++)
                {
                    dicSpriteBody[PartBody.Head].color = sm.WhiteTransparentColor;
                }
            }
            else
            {
                if (currentTypeHair != TypeHair.HairLess && !hasAHat && currentHairCut != -1)
                {
                    spriteRendererHair.sprite = sm.hairList[(int)sexPlayer, (int)currentTypeHair, (int)colorHairPlayer][currentHairCut];
                    spriteRendererHair.color = Color.white;
                }
                else
                {
                    spriteRendererHair.color = sm.WhiteTransparentColor;
                }

                if (cape != Cape.None && currentCape != -1)
                {
                    spriteCape.sprite = sm.accessoriesList[(int)cape, (int)colorCape][currentCape];
                    spriteCape.color = Color.white;
                }
                else
                {
                    spriteCape.color = sm.WhiteTransparentColor;
                }


                if (currentTypeBeard != TypeBeard.None && currentBeard != -1)
                {
                    spriteBeard.sprite = sm.beardList[(int)currentTypeBeard, (int)currentColorBeard][currentBeard];
                    spriteBeard.color = Color.white;
                }
                else
                {
                    spriteBeard.color = sm.WhiteTransparentColor;
                }


                if (typeScar != Scars.None && currentScar != -1)
                {
                    spriteScar.sprite = sm.scarsList[(int)typeScar][currentScar];
                    spriteScar.color = Color.white;
                }
                else
                {
                    spriteScar.color = sm.WhiteTransparentColor;
                }

                //Head
                if (currentColorHead != ColorArmor.None)
                {
                    dicSpriteBody[PartBody.Head].sprite = sm.armorList[(int)sexPlayer, (int)archetype, (int)currentColorHead, (int)PartBody.Head][currentHead];
                    dicSpriteBody[PartBody.Head].color = Color.white;
                }
                else
                {
                    dicSpriteBody[PartBody.Head].color = sm.WhiteTransparentColor;
                }

                //Torse
                if (currentColorTorse != ColorArmor.None)
                {
                    dicSpriteBody[PartBody.Torse].sprite = sm.armorList[(int)sexPlayer, (int)archetype, (int)currentColorTorse, (int)PartBody.Torse][currentTorse];
                    dicSpriteBody[PartBody.Torse].color = Color.white;
                }
                else
                {
                    dicSpriteBody[PartBody.Torse].color = sm.WhiteTransparentColor;
                }
                //Pants
                if (currentColorPant != ColorArmor.None)
                {
                    dicSpriteBody[PartBody.Pant].sprite = sm.armorList[(int)sexPlayer, (int)archetype, (int)currentColorPant, (int)PartBody.Pant][currentPant];
                    dicSpriteBody[PartBody.Pant].color = Color.white;
                }
                else
                {
                    dicSpriteBody[PartBody.Pant].color = sm.WhiteTransparentColor;
                }

                spriteBody.sprite = sm.bodyList[(int)colorBodyPlayer][currentBody];
                spriteSpecial.color = sm.WhiteTransparentColor;
            }
        }
        else
        {
            if (player.IsSpecialCharacter)
            {
                Sprite newSprite = sm.specialsCharacters[(int)player.SpecialsCharacters].Find(item => item.name == mySpriteSpecial.sprite.name);
                if (newSprite)
                    mySpriteSpecial.sprite = newSprite;
                else
                    mySpriteSpecial.sprite = null;

                mySpriteSpecial.color = Color.white;
                mySpriteRendererHair.color = sm.WhiteTransparentColor;
                mySpriteCape.color = sm.WhiteTransparentColor;
                mySpriteBeard.color = sm.WhiteTransparentColor;
                mySpriteScar.color = sm.WhiteTransparentColor;
                mySpriteBody.color = sm.WhiteTransparentColor;
                for (int i = 0; i < (int)PartBody.nbPart; i++)
                {
                    myDicSpriteBody[(PartBody)i].color = sm.WhiteTransparentColor;
                }
            }
            else
            {
                mySpriteSpecial.color = sm.WhiteTransparentColor;
                for (int i = 0; i < (int)PartBody.nbPart; i++)
                {
                    if (player.ArmorColor[(PartBody)i] != ColorArmor.None && dicSpriteBody[(PartBody)i].sprite != null)
                    {
                        string spriteName = dicSpriteBody[(PartBody)i].sprite.name;
                        myDicSpriteBody[(PartBody)i].sprite = sm.armorList[(int)sexPlayer, (int)archetype, (int)player.ArmorColor[(PartBody)i], i].Find(item => item.name == spriteName);
                        myDicSpriteBody[(PartBody)i].color = Color.white;
                    }
                    else
                    {
                        myDicSpriteBody[(PartBody)i].color = sm.WhiteTransparentColor;
                    }
                }
                if (player.HairType != TypeHair.HairLess && !player.HasAHat)
                {
                    Sprite newSprite = sm.hairList[(int)sexPlayer, (int)currentTypeHair, (int)colorHairPlayer].Find(item => item.name == mySpriteRendererHair.sprite.name);
                    if (newSprite)
                        mySpriteRendererHair.sprite = newSprite;
                    mySpriteRendererHair.color = Color.white;
                }
                else
                {
                    mySpriteRendererHair.color = sm.WhiteTransparentColor;
                }

                if (player.Scars != Scars.None && mySpriteScar.sprite != null)
                {
                    Sprite newSprite = sm.scarsList[(int)typeScar].Find(item => item.name == mySpriteScar.sprite.name);
                    if (newSprite)
                        mySpriteScar.sprite = newSprite;
                    else
                        mySpriteScar.sprite = null;
                    mySpriteScar.color = Color.white;
                }
                else
                {
                    mySpriteScar.color = sm.WhiteTransparentColor;
                }

                if (player.TypeBeard != TypeBeard.None && mySpriteBeard.sprite != null && player.ArmorColor[PartBody.Head] == ColorArmor.None)
                {
                    Sprite newSprite = sm.beardList[(int)player.TypeBeard, (int)player.ColorBeard].Find(item => item.name == mySpriteBeard.sprite.name);
                    if (newSprite)
                        mySpriteBeard.sprite = newSprite;
                    else
                        mySpriteBeard.sprite = null;
                    mySpriteBeard.color = Color.white;
                }
                else
                {
                    mySpriteBeard.color = sm.WhiteTransparentColor;
                }

                Sprite newSprite3 = sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].Find(item => item.name == mySpriteCape.sprite.name);
                if (newSprite3)
                    mySpriteCape.sprite = newSprite3;
                else
                    mySpriteCape.sprite = null;


                Sprite newSprite1 = sm.bodyList[(int)player.BodyColor].Find(item => item.name == mySpriteBody.sprite.name);
                if (newSprite1)
                    mySpriteBody.sprite = newSprite1;
                else
                    mySpriteBody.sprite = null;
            }
        }
    }

    private void LateUpdate()
    {
        ReskinAnimation();
    }

    void Movement()
    {
        velocity = rgbd.velocity;
        isMoving = false;
        isMovingHorizontaly = false;
        isMovingVerticaly = false;
        if (ChatManager.chatIsFocused && isLocalPlayer)
        {
            rgbd.velocity = Vector2.zero;
            anim.SetFloat("velocityX", rgbd.velocity.x);
            anim.SetFloat("velocityY", rgbd.velocity.y);
            anim.SetFloat("directionX", Direction.x);
            anim.SetFloat("directionY", Direction.y);
            anim.SetBool("isMovingVerticaly", isMovingVerticaly);
            anim.SetBool("isMovingHorizontaly", isMovingHorizontaly);
            anim.SetBool("isMoving", isMoving);
            return;
        }

        if (InputManager.GetInputManager.GetButton("Horizontal") || InputManager.GetInputManager.GetButton("Vertical"))
        {
            isMoving = true;
        }

        velocity.x = InputManager.GetInputManager.GetAxisRaw("Horizontal");
        velocity.y = InputManager.GetInputManager.GetAxisRaw("Vertical");

        if (velocity.x == 0.0f && velocity.y == 0.0f)
        {
            isMoving = false;
        }

        if (isMoving && (velocity.x != 0.0f || velocity.y != 0.0f))
        {
            Direction = velocity;
        }

        if (Direction.x != 0)
        {
            isMovingHorizontaly = true;
        }
        if (Direction.y != 0)
        {
            isMovingVerticaly = true;
        }

        Direction.Normalize();

        ChangeFacingDirection();

        if (InputManager.GetInputManager.GetButton("Run"))
        {
            speed = 2.0f * basicSpeed;
            anim.SetFloat("isRunning", 2.0f);
            if (smourbiff != null)
            {
                smourbiff.GetComponent<PetFollowing>().isRunning = true;
            }
        }
        else
        {
            speed = basicSpeed;
            anim.SetFloat("isRunning", 1.0f);
            if (smourbiff != null)
            {
                smourbiff.GetComponent<PetFollowing>().isRunning = false;
            }
        }

        velocity.Normalize();
        rgbd.velocity = velocity * speed;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, sCam.MinSizeMap.x + collider.size.x / 2.0f, sCam.MaxSizeMap.x - collider.size.x / 2.0f);
        pos.y = Mathf.Clamp(transform.position.y, sCam.MinSizeMap.y + collider.size.y / 2.0f, sCam.MaxSizeMap.y - collider.size.y / 2.0f);
        transform.position = pos;
        //anim
        anim.SetFloat("velocityX", rgbd.velocity.x);
        anim.SetFloat("velocityY", rgbd.velocity.y);
        anim.SetFloat("directionX", Direction.x);
        anim.SetFloat("directionY", Direction.y);
        anim.SetBool("isMovingVerticaly", isMovingVerticaly);
        anim.SetBool("isMovingHorizontaly", isMovingHorizontaly);
        anim.SetBool("isMoving", isMoving);

    }

    public void CreateDust()
    {
        if (anim.GetFloat("isRunning") > 1.0f)
        {
            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                GameObject gm = Instantiate(dust, (Vector2)transform.position + (Vector2)transform.GetChild(9).transform.localPosition * 0.5f + Random.insideUnitCircle * Random.Range(0.1f, 0.4f), Quaternion.identity);
                float scale = Random.Range(0.4f, 0.7f);
                gm.transform.localScale = new Vector3(scale, scale, 1);
            }
        }


    }

    void Stun()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector2 pos = new Vector2(posPivot.position.x + Mathf.Cos(Mathf.Deg2Rad * 60.0f * i) * radiusStun, posPivot.position.y + Mathf.Sin(Mathf.Deg2Rad * 60.0f * i) * radiusStun);
            if (i % 2 == 0)
            {
                stun.GetComponent<SpriteRenderer>().sprite = sm.stunSprite[0];
            }
            else
            {
                stun.GetComponent<SpriteRenderer>().sprite = sm.stunSprite[1];
            }
            duckRotation[i] = Mathf.Deg2Rad * 60.0f * i;
            duckStun[i] = Instantiate(stun, pos, Quaternion.identity);
        }
        StartCoroutine(AnimStun());
    }

    void WalkWithPet()
    {
        if (InputManager.GetInputManager.GetButtonDown("PetWalk") && EncyclopediaSmourbiff.GetEncyclopediaSmourbiff.dicoSmourbiff[12])
        {
            if (smourbiff == null)
            {
                smourbiff = Instantiate(smourbiffPrefabs, transform.position, Quaternion.identity);
                smourbiff.transform.SetParent(gameObject.transform);
            }
            else
            {
                Destroy(smourbiff);
                smourbiff = null;
            }
        }
    }

    IEnumerator AnimStun()
    {
        while (isStunned)
        {
            for (int i = 0; i < 6; i++)
            {
                duckStun[i].transform.position = new Vector2(posPivot.position.x + radiusStun * Mathf.Cos(duckRotation[i]), posPivot.position.y + radiusStun * Mathf.Sin(duckRotation[i]));
                duckRotation[i] += Mathf.Deg2Rad * 250.0f * Time.deltaTime;
            }
            yield return null;
        }
        for (int i = 0; i < 6; i++)
        {
            Destroy(duckStun[i]);
        }
    }

    [Command]
    public void CmdSpawn(GameObject objectToSpawn)
    {
        NetworkServer.Spawn(objectToSpawn);
    }

    [Command]
    public void CmdDestroy(GameObject objectToDestroy)
    {
        NetworkServer.Destroy(objectToDestroy);
    }

    [Command]
    public void CmdSetAuthority(NetworkIdentity identity)
    {
        var currentOwner = identity.clientAuthorityOwner;
        if (currentOwner == connectionToClient)
        {
            return;
        }
        else
        {
            if (currentOwner != null)
            {
                identity.RemoveClientAuthority(currentOwner);
            }
            identity.AssignClientAuthority(connectionToClient);
        }
    }
    [Command]
    void CmdRemoveAuthority(NetworkIdentity identity)
    {
        var currentOwner = identity.clientAuthorityOwner;
        if (currentOwner != null)
        {
            identity.RemoveClientAuthority(currentOwner);
        }
    }

    void ChangeFacingDirection()
    {
        int direc = 0;
        if (Direction.x > 0.0f && Direction.y > 0.0f)
        {
            direc = 1;
        }
        else if (Direction.x > 0.0f && Direction.y < 0.0f)
        {
            direc = 3;
        }
        else if (Direction.x < 0.0f && Direction.y > 0.0f)
        {
            direc = 7;
        }
        else if (Direction.x < 0.0f && Direction.y < 0.0f)
        {
            direc = 5;
        }
        else if (Direction.x == 0.0f && Direction.y > 0.0f)
        {
            direc = 0;
        }
        else if (Direction.x == 0.0f && Direction.y < 0.0f)
        {
            direc = 4;
        }
        else if (Direction.x > 0.0f && Direction.y == 0.0f)
        {
            direc = 2;
        }
        else if (Direction.x < 0.0f && Direction.y == 0.0f)
        {
            direc = 6;
        }

        PlayerManager.GetPlayerManager.Player.Direction = (FacingDirection)direc;
    }

    IEnumerator Interact()
    {
        //get every colliders in a certain area
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, resourcesLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector2 test = ((Vector2)colliders[i].transform.position + colliders[i].offset) - ((Vector2)transform.position + playerCollider.offset);
            test.Normalize();

            float dot = Vector2.Dot(Direction, test);

            //player look in the direction of the object
            if (dot > 0.5f)
            {
                IInteractable interactable = colliders[i].transform.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    NetworkIdentity identity = colliders[i].GetComponent<NetworkIdentity>();
                    CraftStation craftStation = colliders[i].GetComponent<CraftStation>();
                    bool isBeingHarvest = false;
                    if (colliders[i].GetComponent<HarvestableResources>() != null)
                    {
                        isBeingHarvest = colliders[i].GetComponent<HarvestableResources>().IsAlreadyHarvesting;
                    }
                    if (!isBeingHarvest)
                    {
                        if (identity != null && craftStation == null)
                        {
                            CmdSetAuthority(identity);
                            while (!identity.hasAuthority)
                            {
                                yield return null;
                            }
                        }
                        if (interactable.Interact())
                        {
                            StartCoroutine(OnHarvesting());
                        }
                        yield return new WaitForSeconds(2.2f);
                        if (identity != null && craftStation == null)
                        {
                            CmdRemoveAuthority(identity);
                        }
                        i = colliders.Length;
                    }
                }
                yield return null;
            }
        }
    }

    IEnumerator OnHarvesting()
    {
        isHarvesting = true;
        float timerHarvest = 0.0f;
        while (timerHarvest < 2.0f)
        {
            onHarvesting(timerHarvest);
            timerHarvest += Time.deltaTime;
            yield return null;
        }
        isHarvesting = false;
        onStopHarvesting(0.0f);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    IEnumerator DecreaseTimerStun()
    {
        yield return new WaitForSeconds(1.5f);
        isStunned = false;
    }

    void OnTryCatch(int capture)
    {
        if (capture != 0)
        {
            isStunned = true;
            StartCoroutine(DecreaseTimerStun());
            Stun();
        }
    }
    public void SetReflectOffset(Vector2 newOffset)
    {
        reflect.transform.localPosition = newOffset + reflectOffset;
    }

}
