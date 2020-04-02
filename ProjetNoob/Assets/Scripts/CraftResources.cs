using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class CraftResources
{
    [Serializable]
    public enum TypeResources
    {
        unknown = -1,
        wood,
        rock,
        iron,
        gold,
        silver,
        nbResources
    }
    public int amount;
    public TypeResources type;

    public CraftResources(TypeResources type, int amount)
    {
        this.amount = amount;
        this.type = type;
    }

    public CraftResources(TypeResources type)
    {
        amount = 0;
        this.type = type;
    }

    public TypeResources Type
    {
        get
        {
            return type;
        }
    }
}
