using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftCanvas : MonoBehaviour
{
    [SerializeField] Vector2 sizeCase;
    [SerializeField] Vector2 offset;
    [SerializeField] Vector2 offsetBorder;
    [SerializeField] GameObject casesParent;
    [SerializeField] GameObject prefabItemCraft;
    [SerializeField] GameObject selectedItem;
    [SerializeField] Sprite[] itemsSpriteHud;
    GameObject[,] casesGO;
    Image inventoryBackground;
    [SerializeField] Image hammer;
    [SerializeField] Sprite hammerEnable;
    [SerializeField] Sprite hammerDisable;
    Image[,] cases;
    GameObject canvas;

    GameObject[] resourcesForCraft;
    Text[] textResourcesForCraft;
    [SerializeField] Vector2 sizeResourceCraft;
    [SerializeField] Vector2 offsetResourceCraft;
    [SerializeField] int ressourcesFontSize;
    [SerializeField] int nbMaxResourcesForCraft = 6;
    [SerializeField] int nbResourcesForCraftPerLine = 3;
    [SerializeField] Sprite[] resourcesSpriteHud;
    [SerializeField] GameObject resourceBackground;
    [SerializeField] GameObject scrollView;
    [SerializeField] RectTransform scrollViewRectTransform;
    [SerializeField] RectTransform contentRectTransform;

    int idCraft = 0;

    [SerializeField] Sprite caseSprite;
    bool isInitialize = false;

    Vector2 posItemCraft;
    //Color grey = new Color(150f, 150f, 150f);


    void EnableCraftHud()
    {
        resourceBackground.SetActive(true);
        casesParent.SetActive(true);
        selectedItem.SetActive(true);
        inventoryBackground.enabled = true;
        hammer.enabled = true;
        CraftItem(idCraft);
        UpdateCraftHud();
        scrollView.SetActive(true);
    }

    void DisableCraftHud()
    {
        resourceBackground.SetActive(false);
        hammer.enabled = false;
        casesParent.SetActive(false);
        selectedItem.SetActive(false);
        scrollView.SetActive(false);
        inventoryBackground.enabled = false;
    }

    void UpdateCraftHud()
    {
        if (!CraftManager.GetCraftManager.IsOpen)
        {
            return;
        }
        Item item;
        for (int i = 0; i < CraftManager.GetCraftManager.NbColumns; i++)
        {
            for (int j = 0; j < CraftManager.GetCraftManager.NbLines; j++)
            {
                item = CraftManager.GetCraftManager.GetItem(i, j);
                if (item != null)
                {
                    casesGO[i, j].SetActive(true);
                    cases[i, j].preserveAspect = true;
                    cases[i, j].sprite = itemsSpriteHud[(int)item.ItemType];
                    if (CraftManager.GetCraftManager.CanCraftItem(i + j * CraftManager.GetCraftManager.NbColumns))
                    {
                        cases[i, j].color = Color.white;
                    }
                    else
                    {
                        cases[i, j].color = Color.black;
                    }
                }
                else
                {
                    casesGO[i, j].SetActive(false);
                }
            }
        }
    }

    void MouseOverEnter(int id)
    {
        CraftInfo craftInfo = CraftManager.GetCraftManager.GetCraftInfo(id);
        bool haveResource;
        hammer.sprite = hammerEnable;
        for (int i = 0; i < nbMaxResourcesForCraft; i++)
        {
            if (i < craftInfo.resourcesToCraft.Count)
            {
                resourcesForCraft[i].SetActive(true);
                int nbResources = ResourcesInventoryManager.GetResourcesInventoryManager.GetNbResources(craftInfo.resourcesToCraft[i].Type);
                string text = nbResources.ToString() + "/\n" + craftInfo.resourcesToCraft[i].amount.ToString() + ' ';
                textResourcesForCraft[i].text = text;
                haveResource = ResourcesInventoryManager.GetResourcesInventoryManager.HaveResources(craftInfo.resourcesToCraft[i].Type, craftInfo.resourcesToCraft[i].amount);
                textResourcesForCraft[i].color = haveResource ? Color.green : Color.red;
                posItemCraft.x = 20f + i * (sizeResourceCraft.x + offsetResourceCraft.x) + offsetResourceCraft.x;
                posItemCraft.y = -offsetResourceCraft.y - 20f;
                resourcesForCraft[i].GetComponent<RectTransform>().anchoredPosition = posItemCraft;
                resourcesForCraft[i].GetComponent<Image>().sprite = resourcesSpriteHud[(int)craftInfo.resourcesToCraft[i].Type];
                if (!haveResource)
                {
                    hammer.sprite = hammerDisable;
                }
            }
            else
            {
                resourcesForCraft[i].SetActive(false);
            }
        }
    }

    void SetInfoCraft(int id)
    {
        CraftInfo craftInfo = CraftManager.GetCraftManager.GetCraftInfo(id);
        bool haveResource;
        hammer.sprite = hammerEnable;
        for (int i = 0; i < nbMaxResourcesForCraft; i++)
        {
            if (i < craftInfo.resourcesToCraft.Count)
            {
                resourcesForCraft[i].SetActive(true);
                int nbResources = ResourcesInventoryManager.GetResourcesInventoryManager.GetNbResources(craftInfo.resourcesToCraft[i].Type);
                string text = nbResources.ToString() + "/\n" + craftInfo.resourcesToCraft[i].amount.ToString() + ' ';
                textResourcesForCraft[i].text = text;
                haveResource = ResourcesInventoryManager.GetResourcesInventoryManager.HaveResources(craftInfo.resourcesToCraft[i].Type, craftInfo.resourcesToCraft[i].amount);
                textResourcesForCraft[i].color = haveResource ? Color.green : Color.red;
                posItemCraft.x = 20f + i * (sizeResourceCraft.x + offsetResourceCraft.x) + offsetResourceCraft.x;
                posItemCraft.y = -offsetResourceCraft.y - 20f;
                resourcesForCraft[i].GetComponent<RectTransform>().anchoredPosition = posItemCraft;
                resourcesForCraft[i].GetComponent<Image>().sprite = resourcesSpriteHud[(int)craftInfo.resourcesToCraft[i].Type];
                if (!haveResource)
                {
                    hammer.sprite = hammerDisable;
                }
            }
            else
            {
                resourcesForCraft[i].SetActive(false);
            }
        }
    }

    void MouseOverExit(int id)
    {

    }

    public void CraftItem(int id)
    {
        idCraft = id;
        selectedItem.transform.position = casesGO[id % CraftManager.GetCraftManager.NbColumns, id / CraftManager.GetCraftManager.NbColumns].transform.position;
        SetInfoCraft(id);
    }

    public void CraftItem()
    {
        if (hammer.sprite == hammerEnable)
        {
            CraftManager.GetCraftManager.CraftItem(idCraft);
            SetInfoCraft(idCraft);
        }
    }

    IEnumerator Start()
    {
        RectTransform rc = GetComponent<RectTransform>();
        Vector2 tmpPos;
        canvas = GameObject.Find("CanvasInGame");
        inventoryBackground = GetComponent<Image>();
        if (casesParent == null)
        {
            casesParent = new GameObject("inventoryCases");
            casesParent.transform.SetParent(transform);
            casesParent.AddComponent<RectTransform>();
        }
        RectTransform rcParent = casesParent.GetComponent<RectTransform>();
        rcParent.sizeDelta = Vector2.one;
        rcParent.pivot = Vector2.up;
        rcParent.anchorMax = Vector2.up;
        rcParent.anchorMin = Vector2.up;
        rcParent.anchoredPosition = Vector2.zero;
        casesParent.transform.localScale = Vector3.one;
        cases = new Image[CraftManager.GetCraftManager.NbColumns, CraftManager.GetCraftManager.NbLines];
        casesGO = new GameObject[CraftManager.GetCraftManager.NbColumns, CraftManager.GetCraftManager.NbLines];
        Button button;
        for (int i = 0; i < CraftManager.GetCraftManager.NbColumns; i++)
        {
            for (int j = 0; j < CraftManager.GetCraftManager.NbLines; j++)
            {
                casesGO[i, j] = new GameObject("case " + i + " : " + j);
                RectTransform tmpRect = casesGO[i, j].AddComponent<RectTransform>();
                tmpPos = Vector2.zero; ;
                casesGO[i, j].transform.SetParent(casesParent.transform);
                tmpPos.x += i * (sizeCase.x + offset.x) + offset.x + offsetBorder.x;
                tmpPos.y -= j * (sizeCase.y + offset.y) + offset.y;
                casesGO[i, j].AddComponent<Image>().sprite = caseSprite;
                tmpRect.pivot = Vector2.up;
                tmpRect.anchorMax = Vector2.up;
                tmpRect.anchorMin = Vector2.up;
                tmpRect.sizeDelta = sizeCase;
                tmpRect.anchoredPosition = tmpPos;
                tmpRect.localScale = Vector3.one;

                GameObject tmp2 = Instantiate(casesGO[i, j]);
                tmp2.transform.SetParent(casesGO[i, j].transform);

                casesGO[i, j].SetActive(false);

                cases[i, j] = tmp2.GetComponent<Image>();
                cases[i, j].color = Color.white;

                button = tmp2.AddComponent<Button>();
                int idCraftItem = i + j * CraftManager.GetCraftManager.NbColumns;
                button.onClick.AddListener(() => CraftItem(idCraftItem));
                tmpRect = tmp2.GetComponent<RectTransform>();
                tmpRect.pivot = new Vector2(0.5f, 0.5f);
                tmpRect.anchorMax = new Vector2(0.5f, 0.5f);
                tmpRect.anchorMin = new Vector2(0.5f, 0.5f);
                tmpRect.sizeDelta = sizeCase * 0.75f;
                tmpRect.anchoredPosition = Vector2.zero;
                tmpRect.localScale = Vector3.one;

                //MouseOverCraft tmpMouseOver = tmp2.AddComponent<MouseOverCraft>();
                //tmpMouseOver.id = idCraftItem;
                //tmpMouseOver.onMouseOverEnter += MouseOverEnter;
                //tmpMouseOver.onMouseOverExit += MouseOverExit;
            }
        }
        resourcesForCraft = new GameObject[nbMaxResourcesForCraft];
        textResourcesForCraft = new Text[nbMaxResourcesForCraft];
        for (int i = 0; i < nbMaxResourcesForCraft; i++)
        {
            resourcesForCraft[i] = Instantiate(prefabItemCraft);
            resourcesForCraft[i].transform.SetParent(resourceBackground.transform);
            RectTransform rectTransform = resourcesForCraft[i].GetComponent<RectTransform>();
            rectTransform.pivot = Vector2.up;
            rectTransform.anchorMax = Vector2.up;
            rectTransform.anchorMin = Vector2.up;
            rectTransform.sizeDelta = sizeResourceCraft;
            resourcesForCraft[i].transform.localScale = Vector3.one;
            textResourcesForCraft[i] = resourcesForCraft[i].GetComponentInChildren<Text>();
            textResourcesForCraft[i].fontSize = ressourcesFontSize;
            textResourcesForCraft[i].rectTransform.anchoredPosition = Vector3.down * (sizeResourceCraft + offsetResourceCraft);
            resourcesForCraft[i].SetActive(false);
        }

        selectedItem.GetComponent<RectTransform>().sizeDelta = casesGO[0, 0].GetComponent<RectTransform>().sizeDelta;

        itemsSpriteHud = SpriteManager.GetSpriteManager.ItemsSpriteHud;
        resourcesSpriteHud = SpriteManager.GetSpriteManager.ResourcesSpriteHud;

        CraftManager.GetCraftManager.CraftOpen += EnableCraftHud;
        CraftManager.GetCraftManager.CraftClose += DisableCraftHud;
        CraftManager.GetCraftManager.CraftIsUpdate += UpdateCraftHud;
        posItemCraft = new Vector2();

        rc.sizeDelta = new Vector2(CraftManager.GetCraftManager.NbColumns * (sizeCase.x + offset.x) + offset.x + offsetBorder.x * 2f, 3f * (sizeCase.x + offset.x) + offset.x + offsetBorder.x * 2f);
        scrollViewRectTransform.sizeDelta = new Vector2(rc.sizeDelta.x, rc.sizeDelta.y - offsetBorder.y * 2f);
        while (CraftManager.GetCraftManager.GetRealNbLines() == 0)
        {
            yield return null;
        }
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, CraftManager.GetCraftManager.GetRealNbLines() * (sizeCase.y + offset.y) + offset.y);
        DisableCraftHud();
        isInitialize = true;
    }

    private void OnDestroy()
    {
        if (isInitialize)
        {
            CraftManager.GetCraftManager.CraftOpen -= EnableCraftHud;
            CraftManager.GetCraftManager.CraftClose -= DisableCraftHud;
            CraftManager.GetCraftManager.CraftIsUpdate -= UpdateCraftHud;
        }
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
