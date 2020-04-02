using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Item
{
    public enum TypeItem
    {
        unknown = -1,
        chair,
        table,
        largeTable,
        //lamp,
        carpet,
        bed,
        board,
        corkBoard,
        katana,
        shelf,
        largeShelf,
        clock,
        flowerPot,
        warriorArmor,
        warriorArmor1,
        warriorArmor2,
        warriorArmor3,
        warriorArmor4,
        warriorArmor5,
        workbench,
        barrel,
        sofa,
        spear,
        shield,
        largeIronTable,
        cauldron,
        swordfishTrophey,
        redFishTrophey,
        greenFishTrophey,
        platypusTrophey,
        nbItem
    }
    TypeItem itemType;
    public bool isMoving;

    //public bool isReversible;//pour flip en Y à implémenter plus tard;

    #region assessors
    public TypeItem ItemType
    {
        get
        {
            return itemType;
        }
    }
    #endregion

    public Item(TypeItem typeItem)
    {
        isMoving = false;
        this.itemType = typeItem;
    }

    public Item()
    {
        isMoving = false;
        this.itemType = TypeItem.unknown;
    }

}
