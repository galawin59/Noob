using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeCanvas : MonoBehaviour
{
    [SerializeField] Vector2 sizeCase;
    [SerializeField] Vector2 offset;
    [SerializeField] GameObject casesParent;
    Image inventoryBackground;
    Image[,] cases;
    [SerializeField] Button accept;
    [SerializeField] Button cancel;
    [SerializeField] GameObject heAccept;

    [SerializeField] Image inventoryBackgroundHis;
    [SerializeField] Vector2 sizeCaseHis;
    [SerializeField] Vector2 offsetHis;
    [SerializeField] GameObject casesParentHis;
    Image[,] casesHis;

    [SerializeField] Sprite[] itemsSpriteHud;


    [SerializeField] RectTransform HUDInventory;
    [SerializeField] Sprite caseSprite;

    Color transparent = new Color(255f, 255f, 255f, 0f);
    // Use this for initialization
    bool isInitialize = false;


    void EnableTradeHud()
    {
        if (TradeManager.GetTradeManager.IsOpen)
        {
            casesParent.SetActive(true);
            inventoryBackground.enabled = true;
            casesParentHis.SetActive(true);
            inventoryBackgroundHis.enabled = true;
            accept.gameObject.SetActive(true);
            cancel.gameObject.SetActive(true);
            UpdateTradeHud();
            UpdateTradeHudHis();
        }
    }

    void DisableTradeHud()
    {
        casesParent.SetActive(false);
        inventoryBackground.enabled = false;
        casesParentHis.SetActive(false);
        inventoryBackgroundHis.enabled = false;
        accept.gameObject.SetActive(false);
        cancel.gameObject.SetActive(false);
    }

    void UpdateTradeHud()
    {
        if (!TradeManager.GetTradeManager.IsOpen)
        {
            return;
        }

        Item item;
        for (int i = 0; i < TradeManager.GetTradeManager.MyTrade.NbColumns; i++)
        {
            for (int j = 0; j < TradeManager.GetTradeManager.MyTrade.NbLines; j++)
            {
                item = TradeManager.GetTradeManager.MyTrade.GetItem(i, j);
                if (item != null && item.ItemType != Item.TypeItem.unknown && !item.isMoving)
                {
                    cases[i, j].color = Color.white;
                    cases[i, j].sprite = itemsSpriteHud[(int)item.ItemType];
                }
                else
                {
                    cases[i, j].color = transparent;
                }
            }
        }
    }

    void UpdateTradeHudHis()
    {
        if (!TradeManager.GetTradeManager.IsOpen)
        {
            return;
        }

        Item item;
        for (int i = 0; i < TradeManager.GetTradeManager.HisTrade.NbColumns; i++)
        {
            for (int j = 0; j < TradeManager.GetTradeManager.HisTrade.NbLines; j++)
            {
                item = TradeManager.GetTradeManager.HisTrade.GetItem(i, j);
                if (item != null && item.ItemType != Item.TypeItem.unknown && !item.isMoving)
                {
                    casesHis[i, j].color = Color.white;
                    casesHis[i, j].sprite = itemsSpriteHud[(int)item.ItemType];
                }
                else
                {
                    casesHis[i, j].color = transparent;
                }
            }
        }
    }

    void UpdateAccept()
    {
        if (TradeManager.GetTradeManager.HaveValidate)
        {
            ColorBlock colorBlock = accept.colors;
            colorBlock.normalColor = Color.green;
            colorBlock.highlightedColor = Color.green;
            accept.colors = colorBlock;
        }
        else
        {
            ColorBlock colorBlock = accept.colors;
            colorBlock.normalColor = Color.white;
            colorBlock.highlightedColor = Color.white;
            accept.colors = colorBlock;
        }
    }

    void UpdateHisAccept()
    {
        heAccept.SetActive(TradeManager.GetTradeManager.HaveValidateHis);
    }

    IEnumerator Start()
    {
        while (InventoryManager.GetInventoryManager == null)
        {
            yield return null;
        }
        InventoryManager.GetInventoryManager.InventoryClose += DisableTradeHud;
        InventoryManager.GetInventoryManager.InventoryOpen += EnableTradeHud;
        itemsSpriteHud = SpriteManager.GetSpriteManager.ItemsSpriteHud;
        inventoryBackground = GetComponent<Image>();
        casesParent = new GameObject("MyTradeCases");
        casesParent.transform.parent = transform;
        casesParent.transform.localScale = Vector3.one;
        casesParent.transform.position = transform.position;

        RectTransform rc = GetComponent<RectTransform>();
        rc.sizeDelta = new Vector2(TradeManager.GetTradeManager.NbColumnInventoryTrade * (sizeCase.x + offset.x) + offset.x, TradeManager.GetTradeManager.NbLineInventoryTrade * (sizeCase.y + offset.y) + offset.y);

        //Create all cases with images and buttons
        cases = new Image[TradeManager.GetTradeManager.NbColumnInventoryTrade, TradeManager.GetTradeManager.NbLineInventoryTrade];
        for (int i = 0; i < TradeManager.GetTradeManager.NbColumnInventoryTrade; i++)
        {
            for (int j = 0; j < TradeManager.GetTradeManager.NbLineInventoryTrade; j++)
            {

                GameObject tmp = new GameObject("Case " + i + " : " + j);
                Vector2 tmpPos = Vector2.zero; ;
                tmp.transform.parent = casesParent.transform;
                tmpPos.x += i * (sizeCase.x + offset.x) + offset.x;
                tmpPos.y -= j * (sizeCase.y + offset.y) + offset.y;
                cases[i, j] = tmp.AddComponent<Image>();
                RectTransform tmpRect = tmp.GetComponent<RectTransform>();
                tmpRect.pivot = Vector2.up;
                tmpRect.sizeDelta = sizeCase;
                tmpRect.anchoredPosition = tmpPos;
                tmpRect.localScale = Vector3.one;
                cases[i, j].color = Color.white;
                cases[i, j].sprite = caseSprite;

                GameObject tmp2 = Instantiate(tmp);
                tmp2.transform.SetParent(tmp.transform);

                cases[i, j] = tmp2.GetComponent<Image>();
                cases[i, j].color = Color.white;

                int index = i + j * InventoryManager.GetInventoryManager.NbColumns;
                tmp2.AddComponent<Button>().onClick.AddListener(() => ChangeItemFromTradeToInventory(index));
                tmpRect = tmp2.GetComponent<RectTransform>();
                tmpRect.pivot = new Vector2(0.5f, 0.5f);
                tmpRect.anchoredPosition = Vector2.zero;
                tmpRect.sizeDelta = sizeCase * 0.75f;
                tmpRect.localScale = Vector3.one;

            }
        }

        //add methods to delegate of inventoryManager
        TradeManager.GetTradeManager.onOpen += EnableTradeHud;
        TradeManager.GetTradeManager.onClose += DisableTradeHud;
        TradeManager.GetTradeManager.onValueChange += UpdateTradeHud;



        casesParentHis = new GameObject("HisTradeCases");
        casesParentHis.transform.parent = inventoryBackgroundHis.transform;
        casesParentHis.transform.localScale = Vector3.one;
        casesParentHis.transform.position = inventoryBackgroundHis.transform.position;



        //Create all cases with images and buttons
        casesHis = new Image[TradeManager.GetTradeManager.NbColumnInventoryTrade, TradeManager.GetTradeManager.NbLineInventoryTrade];
        for (int i = 0; i < TradeManager.GetTradeManager.NbColumnInventoryTrade; i++)
        {
            for (int j = 0; j < TradeManager.GetTradeManager.NbLineInventoryTrade; j++)
            {

                GameObject tmp = new GameObject("Case " + i + " : " + j);
                Vector2 tmpPos = Vector2.zero; ;
                tmp.transform.parent = casesParentHis.transform;
                tmpPos.x += i * (sizeCase.x + offset.x) + offset.x;
                tmpPos.y -= j * (sizeCase.y + offset.y) + offset.y;
                casesHis[i, j] = tmp.AddComponent<Image>();
                RectTransform tmpRect = tmp.GetComponent<RectTransform>();
                tmpRect.pivot = Vector2.up;
                tmpRect.sizeDelta = sizeCase;
                tmpRect.anchoredPosition = tmpPos;
                tmpRect.localScale = Vector3.one;
                casesHis[i, j].color = Color.white;
                casesHis[i, j].sprite = caseSprite;

                GameObject tmp2 = Instantiate(tmp);
                tmp2.transform.SetParent(tmp.transform);

                casesHis[i, j] = tmp2.GetComponent<Image>();
                casesHis[i, j].color = Color.white;

                tmpRect = tmp2.GetComponent<RectTransform>();
                tmpRect.pivot = new Vector2(0.5f, 0.5f);
                tmpRect.anchoredPosition = Vector2.zero;
                tmpRect.localScale = Vector3.one;
            }
        }

        TradeManager.GetTradeManager.onValueChangeHis += UpdateTradeHudHis;

        TradeManager.GetTradeManager.onValidate += UpdateAccept;
        TradeManager.GetTradeManager.onValidateHis += UpdateHisAccept;


        DisableTradeHud();
        isInitialize = true;
    }

    public void AddSlectedItemToTrade()
    {
        Item selectedItem = InventoryManager.GetInventoryManager.GetSelectedItem();
        if (selectedItem == null || TradeManager.GetTradeManager.MyTrade.IsFull())
        {
            return;
        }
        selectedItem.isMoving = false;
        TradeManager.GetTradeManager.AddItemToMyTrade(selectedItem);
        InventoryManager.GetInventoryManager.RemoveSelectedItem();
        InventoryManager.GetInventoryManager.UpdateInventory();
        InteractionManager.GetInteractionManager.SendItemForTrade(TradeManager.GetTradeManager.GetMyTradeId());
    }

    private void OnDestroy()
    {
        if (isInitialize)
        {
            TradeManager.GetTradeManager.onOpen -= EnableTradeHud;
            TradeManager.GetTradeManager.onClose -= DisableTradeHud;
            TradeManager.GetTradeManager.onValueChange -= UpdateTradeHud;
            TradeManager.GetTradeManager.onValueChangeHis -= UpdateTradeHudHis;
            TradeManager.GetTradeManager.onValidate -= UpdateAccept;
            TradeManager.GetTradeManager.onValidateHis -= UpdateHisAccept;
        }
    }

    void ChangeItemFromTradeToInventory(int id)
    {
        int nbColumns = TradeManager.GetTradeManager.MyTrade.NbColumns;
        int column = id % nbColumns;
        int line = id / nbColumns;
        Item tmpItem = TradeManager.GetTradeManager.MyTrade.GetItem(column, line);
        Item selectedItem = InventoryManager.GetInventoryManager.GetSelectedItem();
        if (selectedItem != null && !TradeManager.GetTradeManager.MyTrade.IsFull())
        {
            selectedItem.isMoving = false;
            TradeManager.GetTradeManager.AddItemToMyTrade(selectedItem);
            InventoryManager.GetInventoryManager.RemoveSelectedItem();
            InventoryManager.GetInventoryManager.UpdateInventory();
            InteractionManager.GetInteractionManager.SendItemForTrade(TradeManager.GetTradeManager.GetMyTradeId());
        }
        else if (((InputManager.GetInputManager.GetKey(KeyCode.LeftShift) || InputManager.GetInputManager.GetKey(KeyCode.RightShift)) && InventoryManager.GetInventoryManager.AddItem(tmpItem)) ||
            (tmpItem != null && InventoryManager.GetInventoryManager.SetSelectdItem(tmpItem)))
        {
            TradeManager.GetTradeManager.RemoveItemToMyTrade(column, line);
            InventoryManager.GetInventoryManager.UpdateInventory();
            InteractionManager.GetInteractionManager.SendItemForTrade(TradeManager.GetTradeManager.GetMyTradeId());
        }
    }

    public void AcceptTrade()
    {
        TradeManager.GetTradeManager.Accept();
    }

    public void CancelTrade()
    {
        TradeManager.GetTradeManager.Cancel();
    }
}
