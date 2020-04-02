using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesInventoryCanvas : MonoBehaviour
{
    [SerializeField] Sprite[] resourcesSpriteHud;
    [SerializeField] GameObject ressourceParent;
    [SerializeField] Vector2 sizeIcon;
    [SerializeField] Vector2 offset;
    [SerializeField] Vector2 offsetBorder;
    Image[] imageResources;
    GameObject[] background;
    Text[] textResources;
    Image inventoryBackground;
    [SerializeField] GameObject prefabText;
    bool isInitialize = false;

    [SerializeField] RectTransform HUDInventory;

    void EnableResourcesInventoryHud()
    {
        ressourceParent.SetActive(true);
        inventoryBackground.enabled = true;
        UpdateResourcesInventoryHud();
    }

    void DisableResourcesInventoryHud()
    {
        ressourceParent.SetActive(false);
        inventoryBackground.enabled = false;
    }

    void UpdateResourcesInventoryHud()
    {
        if (!ResourcesInventoryManager.GetResourcesInventoryManager.IsOpen)
        {
            return;
        }

        List<CraftResources> resources = ResourcesInventoryManager.GetResourcesInventoryManager.GetResources();
        for (int i = 0; i < (int)CraftResources.TypeResources.nbResources; i++)
        {
            if (i < resources.Count)
            {
                imageResources[i].gameObject.SetActive(true);
                imageResources[i].sprite = resourcesSpriteHud[(int)resources[i].Type];
                textResources[i].enabled = true;
                textResources[i].text = resources[i].amount.ToString();
            }
            else
            {
                textResources[i].enabled = false;
                imageResources[i].gameObject.SetActive(false);
            }
        }
    }

    // Use this for initialization
    IEnumerator Start()
    {
        InventoryCanvas ic = HUDInventory.GetComponent<InventoryCanvas>();
        while (!ic.IsInitialize)
        {
            yield return null;
        }
        RectTransform rc = GetComponent<RectTransform>();
        Vector2 newSize = new Vector2();
        newSize.x = 2f * (sizeIcon.x + offset.x) + offset.x + offsetBorder.x * 2f;
        newSize.y = (int)CraftResources.TypeResources.nbResources * (sizeIcon.y + offset.y) + offset.y + offsetBorder.y * 2f;
        rc.sizeDelta = newSize;
        inventoryBackground = GetComponent<Image>();
        ressourceParent = new GameObject("inventoryCases");
        RectTransform rcParent = ressourceParent.AddComponent<RectTransform>();
        ressourceParent.transform.SetParent(transform);
        rcParent.sizeDelta = Vector2.one;
        rcParent.pivot = Vector2.up;
        rcParent.anchorMax = Vector2.up;
        rcParent.anchorMin = Vector2.up;
        rcParent.anchoredPosition = Vector2.zero;
        ressourceParent.transform.localScale = Vector3.one;
        background = new GameObject[(int)CraftResources.TypeResources.nbResources];
        imageResources = new Image[(int)CraftResources.TypeResources.nbResources];
        textResources = new Text[(int)CraftResources.TypeResources.nbResources];

        for (int i = 0; i < (int)CraftResources.TypeResources.nbResources; i++)
        {
            background[i] = new GameObject("Resource");
            background[i].transform.parent = ressourceParent.transform;
            background[i].transform.localScale = Vector3.one;
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(background[i].transform);
            icon.transform.localScale = Vector3.one;
            imageResources[i] = icon.AddComponent<Image>();
            imageResources[i].rectTransform.sizeDelta = sizeIcon;
            imageResources[i].rectTransform.pivot = Vector2.up;
            background[i].transform.localPosition = Vector2.zero + Vector2.down * (offset.y + sizeIcon.y) * i + Vector2.right * (offset.x + offsetBorder.x) + Vector2.down * (offset.y + offsetBorder.y);
            GameObject text = Instantiate(prefabText);
            text.transform.SetParent(background[i].transform);
            text.transform.localPosition = new Vector2(sizeIcon.x + 5f, 0f);
            text.transform.localScale = Vector3.one;
            textResources[i] = text.GetComponent<Text>();
            textResources[i].rectTransform.pivot = Vector2.up;
            textResources[i].fontSize = (int)(sizeIcon.x * 2f / 3f);
            textResources[i].rectTransform.sizeDelta = sizeIcon;
            textResources[i].text = "resources";
            textResources[i].color = Color.white;
            textResources[i].alignment = TextAnchor.MiddleLeft;
        }

        resourcesSpriteHud = SpriteManager.GetSpriteManager.ResourcesSpriteHud;

        ResourcesInventoryManager.GetResourcesInventoryManager.InventoryOpen += EnableResourcesInventoryHud;
        ResourcesInventoryManager.GetResourcesInventoryManager.InventoryClose += DisableResourcesInventoryHud;
        ResourcesInventoryManager.GetResourcesInventoryManager.InventoryIsUpdate += UpdateResourcesInventoryHud;
        DisableResourcesInventoryHud();
        isInitialize = true;
    }

    private void OnDestroy()
    {
        if (isInitialize)
        {
            ResourcesInventoryManager.GetResourcesInventoryManager.InventoryOpen -= EnableResourcesInventoryHud;
            ResourcesInventoryManager.GetResourcesInventoryManager.InventoryClose -= DisableResourcesInventoryHud;
            ResourcesInventoryManager.GetResourcesInventoryManager.InventoryIsUpdate -= UpdateResourcesInventoryHud;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
