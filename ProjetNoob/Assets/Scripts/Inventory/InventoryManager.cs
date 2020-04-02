using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InventoryEvent();
public delegate void InventoryIsUpdate();
public class InventoryManager : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] [Range(1, 10)] int nbLines;
    [SerializeField] [Range(1, 15)] int nbColumns;
    Item selectedItem;
    int lineSelected;
    int columnSelected;


    public event InventoryEvent InventoryOpen = () => { };
    public event InventoryEvent InventoryClose = () =>
    {
    };
    public event InventoryIsUpdate InventoryIsUpdate = () => { };


    private static InventoryManager instance = null;

    // Game Instance Singleton
    public static InventoryManager GetInventoryManager
    {
        get
        {
            return instance;
        }
    }

    #region assessors
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

    public Inventory GetInventory()
    {
        return inventory;
    }
    #endregion

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        GameManager.GetGameManager.OnReturnMenu += Destroy;
        instance = this;
        inventory = new Inventory(nbColumns, nbLines);
        selectedItem = null;
        lineSelected = -1;
        columnSelected = -1;
        DontDestroyOnLoad(this.gameObject);
    }

    IEnumerator Start()
    {
        while (HUDManager.GetHUDManager == null ||
            ResourcesInventoryManager.GetResourcesInventoryManager == null ||
            CraftManager.GetCraftManager == null ||
            EncyclopediaSmourbiff.GetEncyclopediaSmourbiff == null)
        {
            yield return null;
        }
        ResourcesInventoryManager.GetResourcesInventoryManager.InventoryOpen += Close;
        CraftManager.GetCraftManager.CraftOpen += Close;
        HUDManager.GetHUDManager.HUDClose += Close;
        EncyclopediaSmourbiff.GetEncyclopediaSmourbiff.EncyclopediaOpen += Close;
    }

    private void Update()
    {
        //if (!ChatManager.chatIsactived && !ChatManager.chatIsFocused)
        //{
        //if (Input.GetKeyDown(KeyCode.I))
        if (InputManager.GetInputManager.GetButtonDown("Inventaire", false))
        {
            if (inventory.isOpen)
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
            inventory.Sort();
            InventoryIsUpdate();
        }
        //}
    }

    public void UpdateInventory()
    {
        InventoryIsUpdate();
    }

    public void Open()
    {
        if (!inventory.isOpen)
        {
            HUDManager.GetHUDManager.Open();
            inventory.isOpen = true;
            InventoryOpen();
        }
    }

    public void Close()
    {
        if (inventory.isOpen)
        {
            inventory.isOpen = false;
            InventoryClose();
        }
    }

    public bool IsFull()
    {
        for (int i = 0; i < inventory.NbColumns; i++)
        {
            for (int j = 0; j < inventory.NbLines; j++)
            {
                Item tmp = inventory.GetItem(i, j);
                if (tmp == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool AddItem(Item item)
    {
        return inventory.AddItem(item);
    }

    public bool RemoveItem(Item item)
    {
        if (inventory.Contain(item))
        {
            return inventory.RemoveItem(item);
        }
        return false;
    }

    public bool RemoveItem(int column, int line)
    {
        return inventory.RemoveItem(column, line);
    }

    public bool RemoveSelectedItem()
    {
        if (selectedItem == null)
        {
            return false;
        }
        selectedItem = null;
        return inventory.RemoveItem(columnSelected, lineSelected);
    }

    public bool SetSelectdItem(Item item)
    {
        if (selectedItem != null)
        {
            return false;
        }
        for (int j = 0; j < inventory.NbLines; j++)
        {
            for (int i = 0; i < inventory.NbColumns; i++)
            {
                if (inventory.GetItem(i, j) == null)
                {
                    inventory.AddItem(i, j, item);
                    SelectItem(i, j);
                    return true;
                }
            }
        }
        return false;
    }

    public Item GetFirstAvailableItem()
    {
        for (int j = 0; j < inventory.NbLines; j++)
        {
            for (int i = 0; i < inventory.NbColumns; i++)
            {
                Item tmp = inventory.GetItem(i, j);
                if (tmp != null)
                {
                    return tmp;
                }
            }
        }
        return null;
    }

    public Item GetItem(int column, int line)
    {
        return inventory.GetItem(column, line);
    }

    public bool HaveItemSelected()
    {
        return selectedItem != null;
    }

    public Item GetSelectedItem()
    {
        return selectedItem;
    }

    public bool MoveSelectedItemToNewCase(int column, int line)
    {
        if (selectedItem == null)
        {
            return false;
        }
        else if (column >= 0 && line >= 0 && column < nbColumns && line < nbLines && (column != columnSelected || line != lineSelected))
        {
            Item tmpItem = GetItem(column, line);
            inventory.AddItem(column, line, selectedItem);
            inventory.AddItem(columnSelected, lineSelected, tmpItem);
        }
        selectedItem.isMoving = false;
        selectedItem = null;
        InventoryIsUpdate();

        return true;
    }

    public void UnselectItem()
    {
        if (selectedItem != null)
        {
            selectedItem.isMoving = false;
        }
        selectedItem = null;
    }

    public void SelectItem(int column, int line)
    {
        lineSelected = line;
        columnSelected = column;
        selectedItem = inventory.GetItem(column, line);
        if (selectedItem != null)
        {
            selectedItem.isMoving = true;
        }
        InventoryIsUpdate();
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.GetGameManager.OnReturnMenu -= Destroy;
    }

}
