using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public delegate void OnTradeChange();
    public event OnTradeChange onValueChange = () => { };
    public event OnTradeChange onOpen = () => { };
    public event OnTradeChange onClose = () => { };
    public event OnTradeChange onValueChangeHis = () => { };
    public event OnTradeChange onValidate = () => { };
    public event OnTradeChange onValidateHis = () => { };
    private static TradeManager instance = null;
    [SerializeField] [Range(1, 10)] int nbLineInventoryTrade;
    [SerializeField] [Range(1, 15)] int nbColumnInventoryTrade;
    bool isOpen;
    bool haveValidate;
    bool haveValidateHis;

    public Anwser targetHavePlace;

    Inventory myTrade;
    Inventory hisTrade;

    #region assessors
    // Game Instance Singleton
    public static TradeManager GetTradeManager
    {
        get
        {
            return instance;
        }
    }

    public Inventory MyTrade
    {
        get
        {
            return myTrade;
        }

        private set
        {
            myTrade = value;
        }
    }

    public Inventory HisTrade
    {
        get
        {
            return hisTrade;
        }

        private set
        {
            hisTrade = value;
        }
    }

    public bool HaveValidate
    {
        get
        {
            return haveValidate;
        }

        set
        {
            haveValidate = value;
            onValidate();
        }
    }

    public bool HaveValidateHis
    {
        get
        {
            return haveValidateHis;
        }

        set
        {
            haveValidateHis = value;
            onValidateHis();
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

    public int NbColumnInventoryTrade
    {
        get
        {
            return nbColumnInventoryTrade;
        }
    }

    public int NbLineInventoryTrade
    {
        get
        {
            return nbLineInventoryTrade;
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

        isOpen = false;
        myTrade = new Inventory(nbColumnInventoryTrade, nbLineInventoryTrade);
        hisTrade = new Inventory(nbColumnInventoryTrade, nbLineInventoryTrade);


        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        GameManager.GetGameManager.OnReturnMenu += Destroy;
    }

    public void Open()
    {
        if (!isOpen)
        {
            targetHavePlace = Anwser.none;
            isOpen = true;
            onOpen();
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            if (!myTrade.IsEmpty())
            {
                AddTradeInventoryToPlayerInventory(myTrade, nbColumnInventoryTrade, nbLineInventoryTrade);
                myTrade.Clear();
                InventoryManager.GetInventoryManager.UpdateInventory();
            }
            hisTrade.Clear();
            HaveValidateHis = false;
            HaveValidate = false;
            onClose();
        }
    }

    void AddTradeInventoryToPlayerInventory(Inventory inventory, int nbColumn, int nbLine)
    {
        for (int i = 0; i < nbColumn; i++)
        {
            for (int j = 0; j < nbLine; j++)
            {
                Item item = inventory.GetItem(i, j);
                if (item != null)
                {
                    InventoryManager.GetInventoryManager.AddItem(item);
                    inventory.RemoveItem(i, j);
                }
            }
        }
        onValueChange();
        onValueChangeHis();
    }

    public void AddItemToMyTrade(Item item)
    {
        myTrade.AddItem(item);
        onValueChange();
    }

    public void AddItemToHisTrade(Item item)
    {
        hisTrade.AddItem(item);
        onValueChangeHis();
    }

    public void RemoveItemToMyTrade(Item item)
    {
        myTrade.RemoveItem(item);
        myTrade.Sort();
        onValueChange();
    }

    public void RemoveItemToHisTrade(Item item)
    {
        hisTrade.RemoveItem(item);
        hisTrade.Sort();
        onValueChangeHis();
    }

    public void RemoveItemToMyTrade(int column, int line)
    {
        myTrade.RemoveItem(column, line);
        myTrade.Sort();
        onValueChange();
    }

    public void RemoveItemToHisTrade(int column, int line)
    {
        hisTrade.RemoveItem(column, line);
        hisTrade.Sort();
        onValueChangeHis();
    }

    public void EndTrade()
    {
        StartCoroutine(IEndTrade());
    }

    IEnumerator IEndTrade()
    {
        bool havePlace = InventoryManager.GetInventoryManager.GetInventory().GetNbRemainingPlace() >= hisTrade.GetNbOccupiedPlace();
        InteractionManager.GetInteractionManager.HavePlaceForTrade(havePlace);
        while (targetHavePlace == Anwser.none)
        {
            yield return null;
        }

        if (targetHavePlace == Anwser.yes && havePlace)
        {
            AddTradeInventoryToPlayerInventory(hisTrade, nbColumnInventoryTrade, nbLineInventoryTrade);
            myTrade.Clear();
            hisTrade.Clear();
        }
        else if (!havePlace)
        {
            ChatManager.GetChatManager.SendInfoMsg("Vous n'avez pas la place dans votre inventaire", Message.MessageType.info);
        }
        else if (targetHavePlace == Anwser.no)
        {
            ChatManager.GetChatManager.SendInfoMsg(InteractionManager.GetInteractionManager.TargetName + " n'a pas la place dans son inventaire", Message.MessageType.info);
        }
        Close();
        targetHavePlace = Anwser.none;
        InventoryManager.GetInventoryManager.UpdateInventory();
    }

    public void Accept()
    {
        if (haveValidateHis)
        {
            EndTrade();
        }
        else
        {
            HaveValidate = true;
        }
        InteractionManager.GetInteractionManager.AcceptTrade();
    }

    public void HisAccept()
    {
        if (haveValidate)
        {
            EndTrade();
        }
        else
        {
            HaveValidateHis = true;
        }
    }

    public void Cancel()
    {
        if (haveValidate)
        {
            HaveValidate = false;
        }
        else
        {
            Close();
            ChatManager.GetChatManager.SendInfoMsg("Echange annulé", Message.MessageType.info);
        }
        InteractionManager.GetInteractionManager.CancelTrade();
    }

    public void HisCancel()
    {
        if (haveValidateHis)
        {
            HaveValidateHis = false;
        }
        else
        {
            Close();
            ChatManager.GetChatManager.SendInfoMsg("Echange annulé", Message.MessageType.info);
        }
    }

    public List<int> GetMyTradeId()
    {
        List<int> result = new List<int>();

        HaveValidate = false;
        HaveValidateHis = false;
        for (int i = 0; i < myTrade.NbColumns; i++)
        {
            for (int j = 0; j < myTrade.NbLines; j++)
            {
                Item item = myTrade.GetItem(i, j);
                if (item != null && item.ItemType != Item.TypeItem.unknown)
                {
                    result.Add((int)item.ItemType);
                }
            }
        }

        return result;
    }

    public void SetHisTrade(int[] idItem)
    {
        HaveValidate = false;
        HaveValidateHis = false;
        for (int i = 0; i < nbLineInventoryTrade * NbColumnInventoryTrade; i++)
        {
            int column = i % hisTrade.NbColumns;
            int line = i / hisTrade.NbColumns;
            if (i < idItem.Length)
            {
                hisTrade.AddItem(column, line, new Item((Item.TypeItem)idItem[i]));
            }
            else
            {
                hisTrade.RemoveItem(column, line);
            }
        }
        onValueChangeHis();
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
