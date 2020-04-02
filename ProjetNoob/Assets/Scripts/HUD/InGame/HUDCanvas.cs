using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCanvas : MonoBehaviour
{
    [SerializeField]
    Image background;

    [SerializeField]
    Sprite tabSelected;
    [SerializeField]
    Sprite tabUnselected;

    [SerializeField]
    Image inventory;
    [SerializeField]
    Image craft;
    [SerializeField]
    Image smourbif;

    [SerializeField]
    List<GameObject> buttons;

    void EnableInventoryHud()
    {
        background.enabled = true;
        foreach (GameObject go in buttons)
        {
            go.SetActive(true);
        }
    }

    void DisableInventoryHud()
    {
        background.enabled = false;
        foreach (GameObject go in buttons)
        {
            go.SetActive(false);
        }
    }


    // Use this for initialization
    IEnumerator Start()
    {
        while (HUDManager.GetHUDManager == null ||
            InventoryManager.GetInventoryManager == null ||
            CraftManager.GetCraftManager == null ||
            EncyclopediaSmourbiff.GetEncyclopediaSmourbiff == null ||
            ResourcesInventoryManager.GetResourcesInventoryManager == null)
        {
            yield return null;
        }
        HUDManager.GetHUDManager.HUDClose += DisableInventoryHud;
        HUDManager.GetHUDManager.HUDOpen += EnableInventoryHud;
        CraftManager.GetCraftManager.CraftOpen += SetTabCraft;
        ResourcesInventoryManager.GetResourcesInventoryManager.InventoryOpen += SetTabResources;
        InventoryManager.GetInventoryManager.InventoryOpen += SetTabInventory;
        EncyclopediaSmourbiff.GetEncyclopediaSmourbiff.EncyclopediaOpen += SetTabSmourbif;
    }

    void SetTabResources()
    {
        inventory.sprite = tabUnselected;
        smourbif.sprite = tabUnselected;
        craft.sprite = tabSelected;
    }

    void SetTabCraft()
    {
        inventory.sprite = tabUnselected;
        smourbif.sprite = tabUnselected;
        craft.sprite = tabSelected;
    }

    void SetTabInventory()
    {
        inventory.sprite = tabSelected;
        smourbif.sprite = tabUnselected;
        craft.sprite = tabUnselected;
    }

    void SetTabSmourbif()
    {
        inventory.sprite = tabUnselected;
        smourbif.sprite = tabSelected;
        craft.sprite = tabUnselected;
    }

    public void OpenCraft()
    {
        CraftManager.GetCraftManager.Open();
        SetTabCraft();
    }

    public void OpenInventory()
    {
        InventoryManager.GetInventoryManager.Open();
        SetTabInventory();
    }

    public void OpenResources()
    {
        ResourcesInventoryManager.GetResourcesInventoryManager.Open();
        SetTabResources();
    }

    public void OpenSmourbif()
    {
        EncyclopediaSmourbiff.GetEncyclopediaSmourbiff.OpenEncyclopedia();
        SetTabSmourbif();
    }

    public void Close()
    {
        HUDManager.GetHUDManager.Close();
    }



    // Update is called once per frame
    void Update()
    {

    }
}
