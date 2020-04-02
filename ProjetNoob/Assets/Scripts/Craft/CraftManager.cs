using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CraftManager : MonoBehaviour
{

    [SerializeField] bool isOpen;
    Inventory inventory;

    [SerializeField] [Range(1, 100)] int nbLines;
    [SerializeField] [Range(1, 15)] int nbColumns;

    [SerializeField]
    List<CraftInfo> craftInfos;


    private static CraftManager instance = null;

    public event InventoryEvent CraftOpen = () => { };
    public event InventoryEvent CraftClose = () => { };
    public event InventoryIsUpdate CraftIsUpdate = () => { };

    #region assessors
    // Game Instance Singleton
    public static CraftManager GetCraftManager
    {
        get
        {
            return instance;
        }
    }

    public int NbLines
    {
        get
        {
            return nbLines;
        }
    }

    public int NbColumns
    {
        get
        {
            return nbColumns;
        }
    }

    public bool IsOpen
    {
        private set
        {
            isOpen = value;
            UpdateInvetory();
        }
        get
        {
            return isOpen;
        }
    }
    #endregion

    public void Open()
    {
        if (!isOpen)
        {
            HUDManager.GetHUDManager.Open();
            isOpen = true;
            CraftOpen();
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            CraftClose();
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        isOpen = false;
        if (craftInfos == null)
        {
            craftInfos = new List<CraftInfo>();
        }
        inventory = new Inventory(nbColumns, nbLines);
        DontDestroyOnLoad(this.gameObject);
    }

    IEnumerator Start()
    {
        while (HUDManager.GetHUDManager == null ||
            ResourcesInventoryManager.GetResourcesInventoryManager == null ||
            InventoryManager.GetInventoryManager == null ||
            EncyclopediaSmourbiff.GetEncyclopediaSmourbiff == null)
        {
            yield return null;
        }
        HUDManager.GetHUDManager.HUDClose += Close;
        ResourcesInventoryManager.GetResourcesInventoryManager.InventoryOpen += Open;
        InventoryManager.GetInventoryManager.InventoryOpen += Close;
        EncyclopediaSmourbiff.GetEncyclopediaSmourbiff.EncyclopediaOpen += Close;
        InitListOfCraft();
        ResourcesInventoryManager.GetResourcesInventoryManager.InventoryIsUpdate += UpdateInvetory;
    }

    void Update()
    {
        if (InputManager.GetInputManager.GetButtonDown("Craft", false))
        {
            if (isOpen)
            {
                HUDManager.GetHUDManager.Close();
            }
            else
            {
                Open();
            }
        }
        if (InputManager.GetInputManager.GetKeyDown(KeyCode.U))
        {
            CraftIsUpdate();
        }
    }

    public void AddCraftToList(List<CraftInfo> craftsToAdd)
    {
        foreach (CraftInfo craftToAdd in craftsToAdd)
        {
            craftInfos.Add(craftToAdd);
        }
        InitListOfCraft();
    }

    public bool RemoveCraftToList(List<CraftInfo> craftsToAdd)
    {
        foreach (CraftInfo craftToAdd in craftsToAdd)
        {
            if (!craftInfos.Remove(craftToAdd))
            {
                return false;
            }

        }
        InitListOfCraft();
        return true;
    }

    public bool CanCraftItem(int id)
    {
        return CanCraftItem(craftInfos[id]);
    }

    public bool CanCraftItem(CraftInfo craftInfo)
    {
        foreach (CraftResources craftResource in craftInfo.resourcesToCraft)
        {
            if (!ResourcesInventoryManager.GetResourcesInventoryManager.HaveResources(craftResource.Type, craftResource.amount))
            {
                return false;
            }
        }
        return true;
    }

    public bool CraftItem(int id)
    {
        return CraftItem(craftInfos[id]);
    }

    public bool CraftItem(CraftInfo craftInfo)
    {
        if (craftInfo == null)
        {
            return false;
        }
        if (!InventoryManager.GetInventoryManager.IsFull() && CanCraftItem(craftInfo))
        {
            foreach (CraftResources craftResource in craftInfo.resourcesToCraft)
            {
                if (!ResourcesInventoryManager.GetResourcesInventoryManager.RemoveResources(craftResource.Type, craftResource.amount))
                {
                    return false;
                }
            }
            InventoryManager.GetInventoryManager.AddItem(new Item(craftInfo.itemCraft));
            InventoryManager.GetInventoryManager.UpdateInventory();
            ResourcesInventoryManager.GetResourcesInventoryManager.UpdateInvetory();
            SoundManager.GetSoundManager.PlaySound("Craft", 0.45f);
            return true;
        }
        return false;
    }

    public int GetRealNbLines()
    {
        int occupiedPlace = inventory.GetNbOccupiedPlace();
        int actualNbLines = occupiedPlace / nbColumns;
        if (occupiedPlace % nbColumns != 0)
        {
            actualNbLines++;
        }
        return actualNbLines;
    }

    public CraftInfo GetCraftInfo(int id)
    {
        if (id < 0 || id >= craftInfos.Count)
        {
            return null;
        }
        return craftInfos[id];
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public Item GetItem(int column, int line)
    {
        return inventory.GetItem(column, line);
    }

    public bool AddItem(Item item)
    {
        return inventory.AddItem(item);
    }

    void InitListOfCraft()
    {
        inventory.Clear();
        foreach (CraftInfo craft in craftInfos)
        {
            AddItem(new Item(craft.itemCraft));
        }
        UpdateInvetory();
    }

    public void UpdateInvetory()
    {
        CraftIsUpdate();
    }
}
