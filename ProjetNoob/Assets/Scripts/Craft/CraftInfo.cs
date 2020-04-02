using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CraftInfo
{
    public List<CraftResources> resourcesToCraft;
    public Item.TypeItem itemCraft;

    public CraftInfo(List<CraftResources> resourcesToCraft)
    {
        this.resourcesToCraft = resourcesToCraft;
        itemCraft = Item.TypeItem.unknown;
    }

    public CraftInfo()
    {
        itemCraft = Item.TypeItem.unknown;
        resourcesToCraft = new List<CraftResources>();
    }

    public CraftInfo(Item.TypeItem itemCraft)
    {
        this.itemCraft = itemCraft;
        resourcesToCraft = new List<CraftResources>();
    }

    public CraftInfo(Item.TypeItem itemCraft, List<CraftResources> resourcesToCraft)
    {
        this.itemCraft = itemCraft;
        this.resourcesToCraft = resourcesToCraft;
    }
}
