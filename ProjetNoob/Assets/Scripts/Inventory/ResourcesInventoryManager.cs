using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResourcesInventoryManager : MonoBehaviour
{
    [SerializeField] List<CraftResources> resources;
    [SerializeField] bool isOpen;


    private static ResourcesInventoryManager instance = null;

    public event InventoryEvent InventoryOpen = () => { };
    public event InventoryEvent InventoryClose = () => { };
    public event InventoryIsUpdate InventoryIsUpdate;



    #region assessors
    // Game Instance Singleton
    public static ResourcesInventoryManager GetResourcesInventoryManager
    {
        get
        {
            return instance;
        }
    }

    public List<CraftResources> GetResources()
    {
        return resources;
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

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        resources = new List<CraftResources>();
        instance = this;
        isOpen = false;
        InventoryIsUpdate += SortResources;
        DontDestroyOnLoad(this.gameObject);

        for (int i = 0; i < (int)CraftResources.TypeResources.nbResources; i++)
        {
            AddRessource(new CraftResources((CraftResources.TypeResources)i, 100));
        }
    }

    IEnumerator Start()
    {
        while (HUDManager.GetHUDManager == null ||
            InventoryManager.GetInventoryManager == null ||
            CraftManager.GetCraftManager == null ||
            EncyclopediaSmourbiff.GetEncyclopediaSmourbiff == null)
        {
            yield return null;
        }
        CraftManager.GetCraftManager.CraftOpen += Open;
        InventoryManager.GetInventoryManager.InventoryOpen += Close;
        HUDManager.GetHUDManager.HUDClose += Close;
        EncyclopediaSmourbiff.GetEncyclopediaSmourbiff.EncyclopediaOpen += Close;
    }

    private void Update()
    {
        if (InputManager.GetInputManager.GetKeyDown(KeyCode.G))
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
            for (int i = 0; i < (int)CraftResources.TypeResources.nbResources; i++)
            {
                AddRessource(new CraftResources((CraftResources.TypeResources)i, 100));
            }
            InventoryIsUpdate();
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            HUDManager.GetHUDManager.Open();
            isOpen = true;
            InventoryOpen();
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            InventoryClose();
        }
    }

    public void UpdateInvetory()
    {
        InventoryIsUpdate();
    }

    public void AddRessource(CraftResources resource)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].Type == resource.Type)
            {
                resources[i].amount += resource.amount;
                return;
            }
        }
        resources.Add(resource);
    }

    public void AddRessource(CraftResources.TypeResources type, int amount)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].Type == type)
            {
                resources[i].amount += amount;
                return;
            }
        }
        resources.Add(new CraftResources(type, amount));
    }

    public bool RemoveResources(CraftResources.TypeResources type)
    {
        foreach (CraftResources resource in resources)
        {
            if (resource.Type == type)
            {
                resources.Remove(resource);
                return true;
            }
        }
        return false;
    }

    public bool RemoveResources(CraftResources.TypeResources type, int amount)
    {
        foreach (CraftResources resource in resources)
        {
            if (resource.Type == type && resource.amount >= amount)
            {
                resource.amount -= amount;
                if (resource.amount <= 0)
                {
                    resources.Remove(resource);
                }
                return true;
            }
        }
        return false;
    }

    public bool HaveResources(CraftResources.TypeResources type)
    {
        foreach (CraftResources resource in resources)
        {
            if (resource.Type == type)
            {
                return true;
            }
        }
        return false;
    }

    public bool HaveResources(CraftResources.TypeResources type, int amount)
    {
        foreach (CraftResources resource in resources)
        {
            if (resource.Type == type && resource.amount >= amount)
            {
                return true;
            }
        }
        return false;
    }

    public int GetNbResources(CraftResources.TypeResources type)
    {
        foreach (CraftResources resource in resources)
        {
            if (resource.Type == type)
            {
                return resource.amount;
            }
        }
        return 0;
    }

    public CraftResources GetResource(CraftResources.TypeResources type)
    {
        foreach (CraftResources resource in resources)
        {
            if (resource.Type == type)
            {
                return resource;
            }
        }
        return null;
    }

    void SortResources()
    {
        resources = resources.OrderBy(x => x.Type).ToList();
    }
}
