using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HUDEvent();

public class HUDManager : MonoBehaviour
{
    [SerializeField] bool isOpen;
    private static HUDManager instance = null;

    public event HUDEvent HUDOpen = () => { };
    public event HUDEvent HUDClose = () =>
    {
        InteractionManager.GetInteractionManager.CloseTrade();
        TradeManager.GetTradeManager.Close();
        EncyclopediaSmourbiff.GetEncyclopediaSmourbiff.CloseEncyclopedia();
    };

    public static HUDManager GetHUDManager
    {
        get
        {
            return instance;
        }
    }

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }

        private set
        {
            isOpen = value;
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
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        GameManager.GetGameManager.OnReturnMenu += Destroy;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen && InputManager.GetInputManager.GetButtonDown("Cancel", true))
        {
            Close();
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            HUDOpen();
            if (FurnitureManager.GetFurnitureManager != null)
            {
                FurnitureManager.GetFurnitureManager.Close();
            }
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            InventoryManager.GetInventoryManager.Close();
            CraftManager.GetCraftManager.Close();
            ResourcesInventoryManager.GetResourcesInventoryManager.Close();
            EncyclopediaSmourbiff.GetEncyclopediaSmourbiff.CloseEncyclopedia();
            HUDClose();
        }
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
