using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureCanvas : MonoBehaviour
{

    public FurnitureManager fm;
    [SerializeField] Image[] images;
    [SerializeField] GameObject selector;
    [SerializeField] GameObject parentImages;
    Sprite[] itemsSpriteHud;

    void Open()
    {
        parentImages.SetActive(true);
    }

    public void Close()
    {
        parentImages.SetActive(false);
    }

    public void UpdateCanvas()
    {
        for (int i = 0; i < images.Length; i++)
        {
            Item item = fm.Inventory.GetItem(i, 0);
            if (item != null && item.ItemType != Item.TypeItem.unknown)
            {
                images[i].sprite = itemsSpriteHud[(int)item.ItemType];
                images[i].enabled = true;
                images[i].preserveAspect = true;
            }
            else
            {
                images[i].enabled = false;
            }
        }
    }

    void UpdateCanvasSelector()
    {
        if (fm.InventoryIndex != -1)
        {
            selector.transform.position = images[fm.InventoryIndex].transform.position;

        }
    }

    // Use this for initialization
    void Start()
    {
        fm.furnitureOpen += Open;
        fm.furnitureClose += Close;
        fm.furnitureUpdate += UpdateCanvas;
        fm.furnitureUpdateSelector += UpdateCanvasSelector;
        itemsSpriteHud = SpriteManager.GetSpriteManager.ItemsSpriteHud;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
