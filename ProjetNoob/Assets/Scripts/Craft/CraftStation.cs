using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftStation : MonoBehaviour, ICraftableStation, IInteractable
{
    bool isOpen = false;
    [SerializeField]
    List<CraftInfo> craftInfos;
    List<CraftInfo> ICraftableStation.ListOfCraft
    {
        get
        {
            return craftInfos;
        }
    }

    bool ICraftableStation.IsOpen
    {
        get
        {
            return isOpen;
        }
    }

    public event OnInteract onInteract;

    bool IInteractable.Interact()
    {
        if (!isOpen)
        {
            isOpen = true;
            CraftManager.GetCraftManager.AddCraftToList(craftInfos);
            CraftManager.GetCraftManager.Open();
        return false;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            float distance = Vector3.Distance(transform.position, PlayerManager.GetPlayerManager.currentPlayer.transform.position);
            if (distance >= 2f)
            {
                isOpen = false;
                CraftManager.GetCraftManager.RemoveCraftToList(craftInfos);
            }
        }
    }
}
