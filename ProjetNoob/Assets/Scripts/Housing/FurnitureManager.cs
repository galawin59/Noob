

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Globalization;//pour parse float
using UnityEngine.Networking;

public class FurnitureManager : NetworkBehaviour
{


    public enum E_FurnitureDirection
    {
        DIR_TOP,
        DIR_RIGHT,
        DIR_BOT,
        DIR_LEFT,
        NB_DIR
    };

    struct NetworkObjectInfos
    {
        public int idItem;
        public int direction;
        public int indexSprite;
        public float posX;
        public float posY;
    }

    private static FurnitureManager instance = null;

    NetworkIdentity netID;

    PlayerController pc;

    private Inventory inventory;

    private int inventoryIndex;

    [SerializeField]
    public GameObject housingCanvas;

    [SerializeField]
    string pathInResources = "Prefabs/Furnitures/";

    [SerializeField]
    string pathData = "Data/Furnitures";

    [SerializeField]
    string pathSave = "../SaveHousing/";

    new Camera camera = null;

    Grid grid = null;

    InventoryManager im;

    bool isOpen = false;

    int indexSprite = 0;

    GameObject selectedItem = null;

    GameObject previewGO = null;
    Sprite previewSprite;

    bool isVisiting = true;

    public delegate void FurnitureEvent();

    public event FurnitureEvent furnitureOpen = () => { };
    public event FurnitureEvent furnitureClose = () => { };
    public event FurnitureEvent furnitureUpdate = () => { };
    public event FurnitureEvent furnitureUpdateSelector = () => { };

    List<GameObject[]> prefabsProps;

    public static Dictionary<string, string> ownersHouse;

    ///<summary>liste des GameObject présents dans la scène</summary>
    private List<GameObject> props;
    ///<summary>liste de Furniture de tous les meubles existants</summary>
    public List<GameObject> Props
    {
        get
        {
            return props;
        }

        set
        {
            props = value;
        }
    }

    ///<summary>liste de Furniture de tous les meubles existants</summary>
    private Dictionary<Item.TypeItem, Furniture> furnitures;
    ///<summary>liste de Furniture de tous les meubles existants</summary>
    public Dictionary<Item.TypeItem, Furniture> Furnitures
    {
        get
        {
            return furnitures;
        }

        set
        {
            furnitures = value;
        }
    }

    ///</summary>dictionaire des objets de chaques interieur <Nom de la scene, List de NetworkObjectInfos></summary>
    Dictionary<int, List<NetworkObjectInfos>> listFunituresPerScenes;

    public Inventory Inventory
    {
        get
        {
            return inventory;
        }

        private set
        {
            inventory = value;
        }
    }

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }

        set
        {
            isOpen = value;
            //if (!IsOwner())
            //{
            //    isOpen = false;
            //}
            if (isOpen)
            {
                InventoryIndex = 0;
                furnitureOpen();
                UpdateInventory();
            }
            else
            {
                furnitureClose();
                inventoryIndex = -1;
            }
        }
    }

    public int InventoryIndex
    {
        get
        {
            return inventoryIndex;
        }

        private set
        {
            furDir = E_FurnitureDirection.DIR_BOT;
            inventoryIndex = value;
            furnitureUpdateSelector();
            IndexSprite = 0;
        }
    }

    public E_FurnitureDirection FurDir
    {
        get
        {
            return furDir;
        }

        private set
        {
            if (value >= E_FurnitureDirection.NB_DIR)
            {
                value = 0;
            }
            else if (value < 0)
            {
                value = E_FurnitureDirection.NB_DIR - 1;
            }
            Item item = inventory.GetItem(inventoryIndex, 0);
            if (item != null)
            {
                switch (value)
                {
                    case E_FurnitureDirection.DIR_TOP:
                        if ((furnitures[item.ItemType].nbrDirections / 1000) % 2 > 0)
                        {
                            furDir = value;
                            UpdatePreview();
                        }
                        break;
                    case E_FurnitureDirection.DIR_BOT:
                        if ((furnitures[item.ItemType].nbrDirections / 100) % 2 > 0)
                        {
                            furDir = value;
                            UpdatePreview();
                        }
                        break;
                    case E_FurnitureDirection.DIR_RIGHT:
                        if ((furnitures[item.ItemType].nbrDirections / 10) % 2 > 0)
                        {
                            furDir = value;
                            UpdatePreview();
                        }
                        break;
                    case E_FurnitureDirection.DIR_LEFT:
                        if (furnitures[item.ItemType].nbrDirections % 2 > 0)
                        {
                            furDir = value;
                            UpdatePreview();
                        }
                        break;
                    default:
                        Debug.LogWarning("FurnitureManager : Direction (left right top bottom) of item not found");
                        break;
                }
            }
        }
    }

    public int IndexSprite
    {
        get
        {
            return indexSprite;
        }

        private set
        {
            Item item = inventory.GetItem(inventoryIndex, 0);
            if (item != null)
            {
                if (value < 0)
                {
                    value = furnitures[item.ItemType].nbrSprites - 1;
                }
                else if (value >= furnitures[item.ItemType].nbrSprites)
                {
                    value = 0;
                }
                indexSprite = value;
                previewSprite = SpriteManager.GetSpriteManager.GetFurnitureSprite(previewGO.name.Substring(0, previewGO.name.IndexOf("_")), indexSprite, furDir);
                previewGO.GetComponent<SpriteRenderer>().sprite = previewSprite;
            }
        }
    }

    public static FurnitureManager GetFurnitureManager
    {
        get
        {
            return instance;
        }
    }

    E_FurnitureDirection furDir = E_FurnitureDirection.DIR_BOT;

    public void Close()
    {
        IsOpen = false;
    }

    void DestroyPreview()
    {
        if (previewGO != null)
        {
            Destroy(previewGO);
            previewGO = null;
        }
    }

    #region mainMethods

    //////////////////////////////////////////////////////////////////////////// AWAKE
    private void Awake()
    {

        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            //Destroy(this.gameObject);
            return;
        }
        instance = this;
        isOpen = false;
        DontDestroyOnLoad(this.gameObject);
        GameManager.GetGameManager.OnReturnMenu += Destroy;
        netID = GetComponent<NetworkIdentity>();


        furnitures = InitFurnituresData(pathData);

        Inventory = new Inventory(10, 1);
    }

    //////////////////////////////////////////////////////////////////////////// START (IEnumerator)
    IEnumerator Start()
    {
        Directory.CreateDirectory(pathSave);

        camera = GameObject.Find("Main Camera").GetComponent<Camera>();//Camera.main;

        props = new List<GameObject>();
        while (im == null)
        {
            yield return null;
            im = InventoryManager.GetInventoryManager;//dans addItem
        }
        this.enabled = false;
        InventoryManager.GetInventoryManager.InventoryIsUpdate += UpdateInventory;

        while (pc == null)
        {
            yield return null;
            pc = PlayerManager.GetPlayerManager.playerController;
        }

        Instantiate(housingCanvas).GetComponent<FurnitureCanvas>().fm = this;//canvas

        inventoryIndex = -1;

        furnitureClose += DestroyPreview;
        furnitureUpdateSelector += UpdatePreview;
        furnitureUpdate += UpdatePreview;

        if (PersonalNetworkManager.isSERVER)
        {
            listFunituresPerScenes = new Dictionary<int, List<NetworkObjectInfos>>();
        }
        prefabsProps = new List<GameObject[]>();
        for (Item.TypeItem i = 0; i < Item.TypeItem.nbItem; i++)
        {
            prefabsProps.Add(new GameObject[4]);
            if ((furnitures[i].nbrDirections / 1000) % 2 > 0)
            {
                string prefabPath = pathInResources + furnitures[i].prefab + "_top";
                prefabsProps[(int)i][(int)E_FurnitureDirection.DIR_TOP] = Resources.Load<GameObject>(prefabPath);
            }
            if ((furnitures[i].nbrDirections / 100) % 2 > 0)
            {
                string prefabPath = pathInResources + furnitures[i].prefab + "_bot";
                prefabsProps[(int)i][(int)E_FurnitureDirection.DIR_BOT] = Resources.Load<GameObject>(prefabPath);
            }
            if ((furnitures[i].nbrDirections / 10) % 2 > 0)
            {
                string prefabPath = pathInResources + furnitures[i].prefab + "_right";
                prefabsProps[(int)i][(int)E_FurnitureDirection.DIR_RIGHT] = Resources.Load<GameObject>(prefabPath);
            }
            if (furnitures[i].nbrDirections % 2 > 0)
            {
                string prefabPath = pathInResources + furnitures[i].prefab + "_left";
                prefabsProps[(int)i][(int)E_FurnitureDirection.DIR_LEFT] = Resources.Load<GameObject>(prefabPath);
            }
        }
        string saveFile = Application.persistentDataPath + "/NoobSaves/OwnersHouse.save";
        LoadOwnerHouse(saveFile);
    }

    //////////////////////////////////////////////////////////////////////////// UPDATE
    private void Update()
    {

        if (grid == null && SceneManager.GetActiveScene().name != "Network")
        {
            string gridName = SceneManager.GetActiveScene().name;

            GameObject go = GameObject.Find("Grid" + gridName);
            if (go != null)
            {
                grid = go.GetComponent<Grid>();
            }
            else
            {
                return;
            }
        }

        //if (!ChatManager.chatIsFocused)
        //{
        if (InputManager.GetInputManager.GetButtonDown("Housing", false))
        {
            IsOpen = !isOpen;
        }
        //}
        if (InputManager.GetInputManager.GetKeyDown(KeyCode.V, false))
        {
            isVisiting = !isVisiting;
        }
        if (InputManager.GetInputManager.GetKeyDown(KeyCode.P, false))
        {
            AddOwnerHouse();
        }
        if (InputManager.GetInputManager.GetKeyDown(KeyCode.O, false))
        {
            string saveFile = Application.persistentDataPath + "/NoobSaves/OwnersHouse.save";
            SaveOwnerHouse(saveFile);
        }


        //Poser le meuble
        if (IsOpen)
        {

            if (InputManager.GetInputManager.GetAxisRaw("Mouse ScrollWheel") != 0)
            {
                if (InputManager.GetInputManager.GetButton("Run", false))
                {
                    IndexSprite += (int)(InputManager.GetInputManager.GetAxisRaw("Mouse ScrollWheel", false) * 10f);
                }
                else
                {
                    FurDir += (int)(InputManager.GetInputManager.GetAxisRaw("Mouse ScrollWheel", false) * 10f);
                }
            }

            if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha1, false))
            {
                InventoryIndex = 0;
            }
            else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha2))
            {
                InventoryIndex = 1;
            }
            else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha3))
            {
                InventoryIndex = 2;
            }
            else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha4))
            {
                InventoryIndex = 3;
            }
            else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha5))
            {
                InventoryIndex = 4;
            }
            else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha6))
            {
                InventoryIndex = 5;
            }
            else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha7))
            {
                InventoryIndex = 6;
            }
            else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha8))
            {
                InventoryIndex = 7;
            }
            else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha9))
            {
                InventoryIndex = 8;
            }
            else if (InputManager.GetInputManager.GetKeyDown(KeyCode.Alpha0))
            {
                InventoryIndex = 9;
            }
            if (inventory.GetItem(inventoryIndex, 0) != null)
            {
                if (InputManager.GetInputManager.GetMouseButtonDown(0))
                {
                    AddFurniture();
                }
                //SetPreview();
            }
            if (InputManager.GetInputManager.GetMouseButtonDown(1))
            {
                RemoveFurniture();
            }
        }


        //sauvegarde et chargement
        if (InputManager.GetInputManager.GetKeyDown(KeyCode.PageDown))
        {
            string saveFile = SceneManager.GetActiveScene().name;
            SavePropData(pathSave + saveFile);
        }

        if (InputManager.GetInputManager.GetKeyDown(KeyCode.PageUp))
        {
            string loadFile = SceneManager.GetActiveScene().name;
            LoadPropData(pathSave + loadFile);
        }

        if (isOpen && previewGO != null)
        {
            previewGO.transform.position = GetPositionFromFurnitureSnap(GetFurnitureFromType(inventory.GetItem(inventoryIndex, 0).ItemType));
        }


    }

    void UpdateInventory()
    {
        Inventory.Clear();
        for (int i = 0; i < Inventory.NbColumns; i++)
        {
            Item item = im.GetItem(i, 0);
            if (item != null)
            {
                Inventory.AddItem(i, 0, item);
            }
        }
        furnitureUpdate();
    }

    #endregion

    public Dictionary<Item.TypeItem, Furniture> InitFurnituresData(string _path)
    {
        //definition des temporaires
        Dictionary<Item.TypeItem, Furniture> tempFurnitures = new Dictionary<Item.TypeItem, Furniture>();

        string filePath;

        TextAsset[] dataFiles;

        dataFiles = Resources.LoadAll<TextAsset>(_path);

        for (int i = 0; i < dataFiles.Length; i++)
        {
            Furniture tempFurniture = new Furniture();

            filePath = _path + "/" + dataFiles[i].name;

            StringReader strR = new StringReader(dataFiles[i].text);

            string line;
            string dataType;
            string dataStr;

            while ((line = strR.ReadLine()) != null)
            {
                dataType = line.Substring(0, line.IndexOf("="));

                dataStr = line.Substring(5);

                switch (dataType)
                {
                    case "name":
                        tempFurniture.name = dataStr;
                        break;
                    case "type":
                        tempFurniture.type = GetTypeItemFromName(dataStr);
                        break;
                    case "pref":
                        tempFurniture.prefab = dataStr;//Resources.Load<GameObject>(pathInResources);
                        break;
                    case "nbrD":
                        tempFurniture.nbrDirections = int.Parse(dataStr);
                        break;
                    case "nbrP":
                        tempFurniture.nbrParts = int.Parse(dataStr);
                        break;
                    case "snap":
                        tempFurniture.isSnap = bool.Parse(dataStr);
                        break;
                    case "inte":
                        tempFurniture.isInteract = bool.Parse(dataStr);
                        break;
                    case "cbpo":
                        tempFurniture.canBePutOn = dataStr;
                        break;
                    case "nbrS":
                        tempFurniture.nbrSprites = int.Parse(dataStr);
                        break;
                    default:
                        //Debug.LogWarning("The type of the data in " + dataFiles[i].name + " has not been found");
                        break;
                }
            }
            tempFurnitures.Add(tempFurniture.type, tempFurniture);
        }

        return tempFurnitures;
    }

    IEnumerator IAddFuniture(NetworkObjectInfos networkObjectInfos)
    {
        if (!hasAuthority)
        {
            pc.CmdSetAuthority(netID);
        }
        while (!hasAuthority)
        {
            yield return null;
        }
        CmdAddFurniture(SceneManager.GetActiveScene().buildIndex, networkObjectInfos);
    }

    [Command]
    void CmdAddFurniture(int sceneId, NetworkObjectInfos networkObjectInfos)
    {
        if (!listFunituresPerScenes.ContainsKey(sceneId))
        {
            listFunituresPerScenes.Add(sceneId, new List<NetworkObjectInfos>());
        }
        listFunituresPerScenes[sceneId].Add(networkObjectInfos);
        foreach (GameObject g in PlayerManager.GetPlayerManager.listConnectedPlayers[sceneId])
        {
            TargetAddFurniture(g.GetComponent<NetworkIdentity>().connectionToClient, networkObjectInfos);
        }
    }

    [TargetRpc]
    void TargetAddFurniture(NetworkConnection networkConnection, NetworkObjectInfos objectInfos)
    {
        Scene actualScene = SceneManager.GetActiveScene();
        GameObject[] sceneObjects = actualScene.GetRootGameObjects();
        Transform parent = null;
        foreach (GameObject go in sceneObjects)
        {
            if (go.name == "prefabs group")
            {
                parent = go.transform;
            }
        }

        Item.TypeItem itemToSpawnType = (Item.TypeItem)objectInfos.idItem;

        Furniture furniture = GetFurnitureFromType(itemToSpawnType);

        if (furniture != null)
        {
            Vector3 position = Vector3.zero;
            position.x = objectInfos.posX;
            position.y = objectInfos.posY;

            GameObject prefab = GetPrefabFromFurniture(furniture);

            string saveFile = actualScene.name;
            GameObject furnitureGO = Instantiate(prefabsProps[(int)itemToSpawnType][objectInfos.direction], position, Quaternion.identity);
            furnitureGO.GetComponent<SpriteRenderer>().sprite = SpriteManager.GetSpriteManager.GetFurnitureSprite(furnitureGO.name.Substring(0, furnitureGO.name.IndexOf("_")), objectInfos.indexSprite, (E_FurnitureDirection)objectInfos.direction);
            props.Add(furnitureGO);
            furnitureGO.transform.SetParent(parent);
        }

    }

    /// <summary>
    /// Generate path from the item type and it's direction
    /// and searching for the prefab to Instaciate in the Resources/Prefabs/Furniture
    /// </summary>
    /// <param name="_direction">the top/bot/right/left direction to spawn the item</param>
    /// <param name="_position">the position in world where spawns the item</param>
    /// <returns></returns>
    public bool AddFurniture()
    {
        if (inventory.GetItem(inventoryIndex, 0) != null)
        {
            Item.TypeItem itemToSpawnType = inventory.GetItem(inventoryIndex, 0).ItemType;

            Furniture furniture = GetFurnitureFromType(itemToSpawnType);

            if (furniture != null)
            {

                Vector3 position = GetPositionFromFurnitureSnap(furniture);

                GameObject prefab = GetPrefabFromFurniture(furniture);

                if (!CheckPropsAtPos(position, prefab, furniture))
                {

                    Scene actualScene = SceneManager.GetActiveScene();
                    if (furniture == null)
                    {
                        Debug.LogWarning("FurnitureManager : Item to spawn not found in Resources ");
                        //ne pas retirer de l'inventaire
                        im.UpdateInventory();
                    }
                    else
                    {
                        im.RemoveItem(inventory.GetItem(inventoryIndex, 0));
                        im.UpdateInventory();
                    }
                    NetworkObjectInfos objectInfos;
                    objectInfos.direction = (int)furDir;
                    objectInfos.idItem = (int)itemToSpawnType;
                    objectInfos.indexSprite = indexSprite;
                    objectInfos.posX = position.x;
                    objectInfos.posY = position.y;
                    StartCoroutine(IAddFuniture(objectInfos));
                }
            }
        }
        //StartCoroutine(SpawnFurniture());
        return true;
    }

    public bool IsOwner()
    {
        string ownerHouseName = "";
        string sceneName = SceneManager.GetSceneByBuildIndex(PlayerManager.GetPlayerManager.playerController.sceneId).name;
        if (ownersHouse.ContainsKey(sceneName))
        {
            ownerHouseName = ownersHouse[sceneName];
        }
        string playerName = PlayerManager.GetPlayerManager.playerController.name;
        return ownerHouseName == playerName;
    }

    public Vector3 GetPositionFromFurnitureSnap(Furniture _furniture)
    {
        Vector3 position;

        Vector3 mouseTempPos = Input.mousePosition;

        if (_furniture.isSnap)
        {
            position = grid.WorldToCell(camera.ScreenToWorldPoint(mouseTempPos)) + new Vector3(0.5f, 0.5f, 0.0f);
        }
        else
        {
            Vector3 offset = Vector3.zero;
            position = camera.ScreenToWorldPoint(mouseTempPos - offset);
        }
        position.z = 0.0f;

        return position;
    }

    public GameObject GetPrefabFromFurniture(Furniture _furniture)
    {
        string prefabPath = pathInResources + _furniture.prefab;

        prefabPath = AddDirectionToPrefabStr(_furniture, prefabPath);

        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        return prefab;
    }

    public Furniture GetFurnitureFromType(Item.TypeItem _type)
    {
        return furnitures[_type];
    }

    public string AddDirectionToPrefabStr(Furniture furniture, string prefabPath)
    {
        E_FurnitureDirection _direction = FurDir;

        switch (_direction)
        {
            case E_FurnitureDirection.DIR_TOP:
                if ((furniture.nbrDirections / 1000) % 2 > 0)
                {
                    prefabPath += "_top";
                }
                break;
            case E_FurnitureDirection.DIR_BOT:
                if ((furniture.nbrDirections / 100) % 2 > 0)
                {
                    prefabPath += "_bot";
                }
                break;
            case E_FurnitureDirection.DIR_RIGHT:
                if ((furniture.nbrDirections / 10) % 2 > 0)
                {
                    prefabPath += "_right";
                }
                break;
            case E_FurnitureDirection.DIR_LEFT:
                if (furniture.nbrDirections % 2 > 0)
                {
                    prefabPath += "_left";
                }
                break;
            default:
                Debug.LogWarning("FurnitureManager : Direction (left right top bottom) of item not found");
                break;
        }
        return prefabPath;
    }

    public bool RemoveFurniture()
    {
        Vector3 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);

        Collider2D collider = Physics2D.OverlapCircle(mousePos, 0.2f);

        if (collider != null)
        {
            GameObject itemGO = collider.gameObject;

            string itemName = GetRootItemNameFromGO(itemGO);

            Item.TypeItem typeItem = GetTypeItemFromName(itemName);

            if (typeItem != Item.TypeItem.unknown)
            {
                Item itemI = new Item(typeItem);

                im.AddItem(itemI);
                indexSprite = 0;
                StartCoroutine(DestroyFurniture(itemGO.transform.position));
                return true;
            }
            else
            {
                Debug.LogWarning("Furniture Manager : TypeItem " + itemName + " Not Found, Can't Remove it to Inventory");
            }
        }
        return false;
    }


    [Command]
    void CmdRemoveFurniture(int sceneId, Vector3 position)
    {
        listFunituresPerScenes[sceneId].Remove(listFunituresPerScenes[sceneId].Where(x => x.posX == position.x
        && x.posY == position.y).ToArray()[0]);
        foreach (GameObject g in PlayerManager.GetPlayerManager.listConnectedPlayers[sceneId])
        {
            TargetRemoveFurniture(g.GetComponent<NetworkIdentity>().connectionToClient, position);
        }
    }

    [TargetRpc]
    void TargetRemoveFurniture(NetworkConnection networkConnection, Vector3 position)
    {
        GameObject go = props.Where(x => x.transform.position == position).ToArray()[0];
        props.Remove(go);
        Destroy(go);
    }

    IEnumerator DestroyFurniture(Vector3 position)
    {
        if (!hasAuthority)
        {
            pc.CmdSetAuthority(netID);
        }
        while (!hasAuthority)
        {
            yield return null;
        }
        CmdRemoveFurniture(SceneManager.GetActiveScene().buildIndex, position);
        im.UpdateInventory();
    }

    /// <summary>
    /// Removes "(clone)" and "_direction" from the gameObject's name
    /// </summary>
    /// <param name="itemGO"></param>
    /// <returns></returns>
    public string GetRootItemNameFromGO(GameObject itemGO)
    {
        //remove (clone)
        int cloneNameIndex = itemGO.name.IndexOf("(Clone)");
        string itemName = cloneNameIndex != -1 ? itemGO.name.Substring(0, cloneNameIndex) : itemGO.name;

        //remove de la direction
        int endNameIndex = itemName.IndexOf("_");
        itemName = endNameIndex != -1 ? itemName.Substring(0, endNameIndex) : itemName;

        return itemName;
    }

    bool CheckPropsAtPos(Vector3 _position, GameObject _prop, Furniture _fur)
    {
        Vector2 colliderSize = _prop.GetComponent<BoxCollider2D>().size;

        Vector3 colliderOffset = _prop.GetComponent<BoxCollider2D>().offset;

        Collider2D collider = Physics2D.OverlapBox(camera.ScreenToWorldPoint(Input.mousePosition) + colliderOffset, colliderSize, 0.0f);

        if (collider != null)
        {
            string colliderName = GetRootItemNameFromGO(collider.gameObject);

            if (_fur.canBePutOn.IndexOf(colliderName) > -1)
            {
                return false;
            }
            else
            {
                return collider != null ? true : false;
            }
        }
        else
        {
            return false;
        }


    }

    public GameObject GetGOFromItem(Item _item)
    {
        GameObject propGO;

        Furniture propFur = GetFurnitureFromType(_item.ItemType);

        propGO = GetPrefabFromFurniture(propFur);

        return propGO;
    }

    private void UpdatePreview()
    {
        if (previewGO != null)
        {
            Destroy(previewGO);
            previewGO = null;
        }

        if (inventory.GetItem(inventoryIndex, 0) != null)
        {

            previewGO = Instantiate(GetGOFromItem(inventory.GetItem(inventoryIndex, 0)),
                                        GetPositionFromFurnitureSnap(GetFurnitureFromType(inventory.GetItem(inventoryIndex, 0).ItemType)),
                                        Quaternion.identity);

            previewGO.GetComponent<BoxCollider2D>().enabled = false;

            Color tmpColor = previewGO.GetComponent<SpriteRenderer>().color;
            tmpColor.a = 0.5f;
            SpriteRenderer sr = previewGO.GetComponent<SpriteRenderer>();
            sr.color = tmpColor;
            previewSprite = sr.sprite;
        }

    }

    public void SavePropData(string _path)
    {
        //System.IO.StreamWriter wFile = new System.IO.StreamWriter(_path);

        //for (int i = 0; i < props.Count; i++)
        //{

        //    string propFullName = props[i].name;
        //    int endPos = propFullName.IndexOf("(Clone)");
        //    string propName = propFullName.Substring(0, endPos);

        //    string propPos = props[i].transform.position.x.ToString() + props[i].transform.position.y.ToString();

        //    wFile.WriteLine(propName + " " + propPos);
        //}

        //wFile.Close();
    }

    public void LoadPropData(string _path)
    {
        //System.IO.StreamReader rFile = new System.IO.StreamReader(_path);

        //string line;

        //while (!rFile.EndOfStream)
        //{
        //    line = rFile.ReadLine();

        //    int iseparation = line.IndexOf(" ");
        //    int iseparaPos = line.IndexOf("-");

        //    string propName = line.Substring(0, iseparation);

        //    string sposX = line.Substring(iseparation + 1, iseparaPos - iseparation - 1);
        //    string sposY = line.Substring(iseparaPos + 1);

        //    float fposX;
        //    float fposY;

        //    if (!float.TryParse(sposX, out fposX))
        //    {
        //        fposX = float.Parse(sposX + ".0", CultureInfo.InvariantCulture);
        //    }

        //    if (!float.TryParse(sposY, out fposY))
        //    {
        //        fposY = float.Parse(sposY + ".0", CultureInfo.InvariantCulture);
        //    }

        //    Vector3 position = new Vector3(fposX, -fposY, 0.0f);

        //    GameObject furniture = Instantiate(Resources.Load<GameObject>("Prefabs/Furnitures/" + propName), position, Quaternion.identity);

        //    props.Add(furniture);

        //    //on ajoute au groupe de préfabs
        //    furniture.transform.SetParent(GameObject.Find("prefabs group").transform);
        //}
        //rFile.Close();
    }

    void SaveOwnerHouse(string _path)
    {
        StreamWriter wFile = new StreamWriter(_path);

        foreach (KeyValuePair<string, string> ownerHouse in ownersHouse)
        {
            wFile.WriteLine(ownerHouse.Key + "," + ownerHouse.Value);
        }

        wFile.Close();
    }

    IEnumerator IAddOwnerHouse()
    {
        if (!hasAuthority)
        {
            pc.CmdSetAuthority(netID);
        }
        while (!hasAuthority)
        {
            yield return null;
        }
        string nameScene = SceneManager.GetSceneByBuildIndex(PlayerManager.GetPlayerManager.playerController.sceneId).name;
        string nameOwner = PlayerManager.GetPlayerManager.playerController.name;
        CmdAddOwner(nameScene, nameOwner);
    }

    [Command]
    void CmdAddOwner(string nameScene, string nameOwner)
    {
        if (!ownersHouse.ContainsKey(nameScene))
        {
            ownersHouse.Add(nameScene, nameOwner);
        }
        else
        {
            ownersHouse[nameScene] = nameOwner;
        }
        NetworkOwnerHouseDictionary networkOwner;
        networkOwner.nameScene = ownersHouse.Keys.ToArray<string>();
        networkOwner.nameOwner = ownersHouse.Values.ToArray<string>();

        RpcGetOwnerHouse(networkOwner);
    }

    void AddOwnerHouse()
    {
        StartCoroutine(IAddOwnerHouse());
    }

    struct NetworkOwnerHouseDictionary
    {
        public string[] nameScene;
        public string[] nameOwner;
    }

    IEnumerator IGetOwnerHouse()
    {
        if (!hasAuthority)
        {
            pc.CmdSetAuthority(netID);
        }
        while (!hasAuthority)
        {
            yield return null;
        }
        CmdGetOwnerHouse();
    }

    [Command]
    void CmdGetOwnerHouse()
    {
        NetworkOwnerHouseDictionary networkOwner;
        networkOwner.nameScene = ownersHouse.Keys.ToArray<string>();
        networkOwner.nameOwner = ownersHouse.Values.ToArray<string>();

        RpcGetOwnerHouse(networkOwner);
    }

    [ClientRpc]
    void RpcGetOwnerHouse(NetworkOwnerHouseDictionary networkOwnerHouseDictionary)
    {
        if (ownersHouse == null)
        {
            ownersHouse = new Dictionary<string, string>();
        }
        for (int i = 0; i < networkOwnerHouseDictionary.nameScene.Length; i++)
        {
            string nameOwner = networkOwnerHouseDictionary.nameOwner[i];
            string nameScene = networkOwnerHouseDictionary.nameScene[i];
            if (!ownersHouse.ContainsKey(nameScene))
            {
                ownersHouse.Add(nameScene, nameOwner);
            }
            else
            {
                ownersHouse[nameScene] = nameOwner;
            }
        }
    }

    void LoadOwnerHouse(string _path)
    {
        if (PersonalNetworkManager.isSERVER)
        {
            using (FileStream stream = File.Open(_path, FileMode.OpenOrCreate))
            {
                if (ownersHouse == null)
                {
                    ownersHouse = new Dictionary<string, string>();
                }
                StreamReader rFile = new StreamReader(stream);

                string line;

                while (!rFile.EndOfStream)
                {
                    line = rFile.ReadLine();
                    string nameOwner = line.Substring(line.IndexOf(",") + 1);
                    string nameScene = line.Substring(0, line.IndexOf(","));
                    if (!ownersHouse.ContainsKey(nameScene))
                    {
                        ownersHouse.Add(nameScene, nameOwner);
                    }
                    else
                    {
                        ownersHouse[nameScene] = nameOwner;
                    }

                }
                rFile.Close();
                stream.Close();
            }
        }
        else
        {
            StartCoroutine(IGetOwnerHouse());
        }
    }

    /// <summary>
    /// returns the TypeItem of an object from his name
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public Item.TypeItem GetTypeItemFromName(string _name)
    {
        Item.TypeItem tempTypeItem;
        tempTypeItem = Item.TypeItem.unknown;

        for (int i = 0; i < (int)Item.TypeItem.nbItem; i++)
        {
            if (((Item.TypeItem)i).ToString() == _name)
            {
                tempTypeItem = (Item.TypeItem)i;
                i = (int)Item.TypeItem.nbItem;
            }
        }

        return tempTypeItem;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    IEnumerator IGetNetworkObecjtInfos()
    {
        if (!hasAuthority)
        {
            pc.CmdSetAuthority(netID);
        }
        while (!hasAuthority)
        {
            yield return null;
        }
        CmdGetNetworkObjectInfos(PlayerManager.GetPlayerManager.playerIdentity, SceneManager.GetActiveScene().buildIndex);
    }

    [Command]
    void CmdGetNetworkObjectInfos(NetworkIdentity playerIdentity, int sceneId)
    {
        if (listFunituresPerScenes.ContainsKey(sceneId))
        {
            TargetGetNetworkObjectInfos(playerIdentity.connectionToClient, listFunituresPerScenes[sceneId].ToArray());
        }
    }

    [TargetRpc]
    void TargetGetNetworkObjectInfos(NetworkConnection networkConnection, NetworkObjectInfos[] objectsInfos)
    {
        GameObject[] sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        Transform parent = null;
        foreach (GameObject go in sceneObjects)
        {
            if (go.name == "prefabs group")
            {
                parent = go.transform;
            }
        }

        foreach (NetworkObjectInfos objectInfos in objectsInfos)
        {
            Item.TypeItem itemToSpawnType = (Item.TypeItem)objectInfos.idItem;

            Furniture furniture = GetFurnitureFromType(itemToSpawnType);

            if (furniture != null)
            {
                Vector3 position = Vector3.zero;
                position.x = objectInfos.posX;
                position.y = objectInfos.posY;

                GameObject prefab = GetPrefabFromFurniture(furniture);

                Scene actualScene = SceneManager.GetActiveScene();
                string saveFile = actualScene.name;

                GameObject furnitureGO = Instantiate(prefabsProps[(int)itemToSpawnType][objectInfos.direction], position, Quaternion.identity);
                furnitureGO.GetComponent<SpriteRenderer>().sprite = SpriteManager.GetSpriteManager.GetFurnitureSprite(furnitureGO.name.Substring(0, furnitureGO.name.IndexOf("_")), objectInfos.indexSprite, (E_FurnitureDirection)objectInfos.direction);
                props.Add(furnitureGO);
                furnitureGO.transform.SetParent(parent);
            }

        }
    }

    private void OnEnable()
    {
        if (pc != null && netId != null)
        {
            StartCoroutine(IGetNetworkObecjtInfos());
        }
    }

    private void OnDisable()
    {
        foreach (GameObject go in props)
        {
            Destroy(go);
        }
        props.Clear();
    }

    private void OnDestroy()
    {
        GameManager.GetGameManager.OnReturnMenu -= Destroy;
    }


}