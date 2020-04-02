using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class InventoryCanvas : MonoBehaviour
{
    [SerializeField] Vector2 sizeCase;
    [SerializeField] Vector2 offset;
    [SerializeField] Vector2 offsetBorder;
    [SerializeField] GameObject casesParent;
    Image inventoryBackground;
    Image[,] cases;
    [SerializeField] Sprite[] itemsSpriteHud;
    [SerializeField] Sprite caseSprite;
    [SerializeField] Image itemInHand;
    GameObject canvas;
    Color transparent = new Color(255f, 255f, 255f, 0f);
    bool firstFrame = false;
    bool isInitialize = false;
    GraphicRaycaster raycaster;

    #region assessors
    public Vector2 SizeCase
    {
        get
        {
            return sizeCase;
        }
    }

    public Vector2 Offset
    {
        get
        {
            return offset;
        }
    }

    public bool IsInitialize
    {
        get
        {
            return isInitialize;
        }
    }
    #endregion

    void EnableInventoryHud()
    {
        casesParent.SetActive(true);
        inventoryBackground.enabled = true;
        UpdateInventoryHud();
    }

    void DisableInventoryHud()
    {
        itemInHand.enabled = false;
        casesParent.SetActive(false);
        inventoryBackground.enabled = false;
        InventoryManager.GetInventoryManager.MoveSelectedItemToNewCase(-1, -1);
    }

    void UpdateInventoryHud()
    {
        if (!InventoryManager.GetInventoryManager.GetInventory().isOpen)
        {
            itemInHand.enabled = false;
            return;
        }

        Item item;
        for (int i = 0; i < InventoryManager.GetInventoryManager.NbColumns; i++)
        {
            for (int j = 0; j < InventoryManager.GetInventoryManager.NbLines; j++)
            {
                item = InventoryManager.GetInventoryManager.GetItem(i, j);
                if (item != null && item.ItemType != Item.TypeItem.unknown && !item.isMoving)
                {
                    cases[i, j].color = Color.white;
                    cases[i, j].sprite = itemsSpriteHud[(int)item.ItemType];
                    cases[i, j].preserveAspect = true;
                }
                else
                {
                    cases[i, j].color = transparent;
                }
            }
        }
        if (InventoryManager.GetInventoryManager.HaveItemSelected())
        {
            itemInHand.enabled = true;
            itemInHand.sprite = itemsSpriteHud[(int)InventoryManager.GetInventoryManager.GetSelectedItem().ItemType];
            itemInHand.transform.localScale = Vector3.one;
            itemInHand.rectTransform.sizeDelta = Vector2.one * 80f;
            itemInHand.preserveAspect = true;
        }
        else
        {
            itemInHand.enabled = false;
        }
    }

    IEnumerator Start()
    {
        raycaster = gameObject.AddComponent<GraphicRaycaster>();
        while (InventoryManager.GetInventoryManager == null)
        {
            yield return null;
        }
        inventoryBackground = GetComponent<Image>();
        casesParent = new GameObject("inventoryCases");
        RectTransform rcParent = casesParent.AddComponent<RectTransform>();
        casesParent.transform.SetParent(transform);
        rcParent.sizeDelta = Vector2.one;
        rcParent.pivot = Vector2.up;
        rcParent.anchorMax = Vector2.up;
        rcParent.anchorMin = Vector2.up;
        rcParent.anchoredPosition = Vector2.zero;
        casesParent.transform.localScale = Vector3.one;

        //Create all cases with images and buttons
        cases = new Image[InventoryManager.GetInventoryManager.NbColumns, InventoryManager.GetInventoryManager.NbLines];
        Button button;
        for (int i = 0; i < InventoryManager.GetInventoryManager.NbColumns; i++)
        {
            for (int j = 0; j < InventoryManager.GetInventoryManager.NbLines; j++)
            {
                GameObject tmp = new GameObject("Case " + i + " : " + j);
                Vector2 tmpPos = Vector2.zero;
                tmp.transform.parent = casesParent.transform;
                tmpPos.x += i * (sizeCase.x + offset.x) + offset.x + offsetBorder.x;
                tmpPos.y -= j * (sizeCase.y + offset.y) + offset.y + offsetBorder.y;
                cases[i, j] = tmp.AddComponent<Image>();
                RectTransform tmpRect = tmp.GetComponent<RectTransform>();
                tmpRect.pivot = Vector2.up;
                tmpRect.sizeDelta = sizeCase;
                tmpRect.anchoredPosition = tmpPos;
                tmpRect.localScale = Vector3.one;

                cases[i, j].sprite = caseSprite;
                cases[i, j].color = Color.white;

                GameObject tmp2 = Instantiate(tmp);
                tmp2.transform.SetParent(tmp.transform);

                cases[i, j] = tmp2.GetComponent<Image>();
                cases[i, j].color = Color.white;

                ClickableItem clickableItem = tmp2.AddComponent<ClickableItem>();
                clickableItem.onLeftClick += ChangeItemFromInventoryToTrade;
                button = tmp2.AddComponent<Button>();
                int index = i + j * InventoryManager.GetInventoryManager.NbColumns;
                clickableItem.id = index;
                button.onClick.AddListener(() => SelectItem(index));
                tmpRect = tmp2.GetComponent<RectTransform>();
                tmpRect.pivot = new Vector2(0.5f, 0.5f);
                tmpRect.anchoredPosition = Vector2.zero;
                tmpRect.sizeDelta = sizeCase * 0.75f;
                tmpRect.localScale = Vector3.one;

            }
        }
        itemsSpriteHud = SpriteManager.GetSpriteManager.ItemsSpriteHud;


        //Create ItemInHand image
        canvas = GameObject.Find("CanvasInGame");
        GameObject itemInHandGameObject = new GameObject("ItemInHand");
        itemInHandGameObject.transform.SetParent(canvas.transform);
        itemInHand = itemInHandGameObject.AddComponent<Image>();
        itemInHand.rectTransform.anchorMin = Vector2.zero;
        itemInHand.rectTransform.anchorMax = Vector2.zero;
        itemInHand.rectTransform.sizeDelta = sizeCase;
        itemInHand.raycastTarget = false;
        itemInHand.rectTransform.localScale = Vector3.one;

        //add methods to delegate of inventoryManager
        InventoryManager.GetInventoryManager.InventoryOpen += EnableInventoryHud;
        InventoryManager.GetInventoryManager.InventoryClose += DisableInventoryHud;
        InventoryManager.GetInventoryManager.InventoryIsUpdate += UpdateInventoryHud;

        Vector2 newSize = new Vector2();
        newSize.x = InventoryManager.GetInventoryManager.NbColumns * (sizeCase.x + offset.x) + offset.x + offsetBorder.x * 2f;
        newSize.y = InventoryManager.GetInventoryManager.NbLines * (sizeCase.y + offset.y) + offset.y + offsetBorder.y * 2f;
        GetComponent<RectTransform>().sizeDelta = newSize;

        DisableInventoryHud();
        isInitialize = true;
    }

    void Update()
    {
        if (InventoryManager.GetInventoryManager.HaveItemSelected())
        {
            itemInHand.rectTransform.anchoredPosition = GetCanvasPosition(Input.mousePosition);

            //supression d'items
            if (InputManager.GetInputManager.GetKeyDown(KeyCode.Delete, false))
            {
                InventoryManager.GetInventoryManager.RemoveSelectedItem();
                UpdateInventoryHud();

                //FurnitureManager.GetFurnitureManager.housingCanvas.GetComponent<FurnitureCanvas>().UpdateCanvas();
                FurnitureManager.GetFurnitureManager.IsOpen = false;
                FurnitureManager.GetFurnitureManager.IsOpen = true;
            }
        }
    }

    void ChangeItemFromInventoryToTrade(int id)
    {
        if (!TradeManager.GetTradeManager.IsOpen || TradeManager.GetTradeManager.MyTrade.IsFull() ||
            (!InputManager.GetInputManager.GetKey(KeyCode.LeftShift) && !InputManager.GetInputManager.GetKey(KeyCode.RightShift)))
        {
            return;
        }
        int nbColumns = InventoryManager.GetInventoryManager.NbColumns;
        int column = id % nbColumns;
        int line = id / nbColumns;
        Item tmpItem = InventoryManager.GetInventoryManager.GetItem(column, line);
        TradeManager.GetTradeManager.AddItemToMyTrade(tmpItem);
        InventoryManager.GetInventoryManager.RemoveItem(column, line);
        UpdateInventoryHud();
        InteractionManager.GetInteractionManager.SendItemForTrade(TradeManager.GetTradeManager.GetMyTradeId());
    }

    private void OnDestroy()
    {
        if (IsInitialize)
        {
            InventoryManager.GetInventoryManager.InventoryOpen -= EnableInventoryHud;
            InventoryManager.GetInventoryManager.InventoryClose -= DisableInventoryHud;
            InventoryManager.GetInventoryManager.InventoryIsUpdate -= UpdateInventoryHud;
        }
    }

    public void Close()
    {
        InventoryManager.GetInventoryManager.Close();
    }

    void SelectItem(int id)
    {
        firstFrame = true;
        if (InventoryManager.GetInventoryManager.HaveItemSelected())
        {
            InventoryManager.GetInventoryManager.MoveSelectedItemToNewCase(id % InventoryManager.GetInventoryManager.NbColumns, id / InventoryManager.GetInventoryManager.NbColumns);
        }
        else
        {
            InventoryManager.GetInventoryManager.SelectItem(id % InventoryManager.GetInventoryManager.NbColumns, id / InventoryManager.GetInventoryManager.NbColumns);
            itemInHand.rectTransform.anchoredPosition = GetCanvasPosition(Input.mousePosition);
        }
    }

    private void LateUpdate()
    {
        //if (!firstFrame && Input.GetMouseButtonUp(0) && InventoryManager.GetInventoryManager.HaveItemSelected())
        //{
        //    PointerEventData pointerData = new PointerEventData(EventSystem.current);
        //    List<RaycastResult> results = new List<RaycastResult>();
        //    pointerData.position = Input.mousePosition;
        //    raycaster.Raycast(pointerData, results);
        //    if (EventSystem.current.IsPointerOverGameObject())
        //    {
        //        InventoryManager.GetInventoryManager.MoveSelectedItemToNewCase(-1, -1);
        //    }
        //}
        //firstFrame = false;
    }

    Vector2 GetCanvasPosition(Vector2 screenPosition)
    {
        Vector2 sizeCanvas = canvas.GetComponent<RectTransform>().sizeDelta;
        Vector2 position = new Vector2();
        position.x = screenPosition.x / Screen.width * sizeCanvas.x;
        position.y = screenPosition.y / Screen.height * sizeCanvas.y;
        return position;
    }
}
