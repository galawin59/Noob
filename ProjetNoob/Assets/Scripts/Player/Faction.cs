using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactionName
{
    Coalition,
    Ordre,
    Empire
}

enum CursorColor
{
    Red,
    Green,
    Yellow,
    Admin
}

[Serializable]
public class Faction
{
    FactionName factionName;
    CursorColor cursorColor;

    public Faction(FactionName factionName)
    {
        this.factionName = factionName;
        switch (factionName)
        {
            case FactionName.Empire:
                cursorColor = CursorColor.Yellow;
                break;
            case FactionName.Coalition:
                cursorColor = CursorColor.Red;
                break;
            case FactionName.Ordre:
                cursorColor = CursorColor.Green;
                break;
            default:
                break;
        }
    }

    public int GetCursorColor
    {
        get
        {
            return (int)cursorColor;
        }
        set
        {
            cursorColor = (CursorColor)value;
        }
    }

    public FactionName GetFaction
    {
        get
        {
            return factionName;
        }
        set
        {
            factionName = value;
        }
    }

    public string GetFactionName
    {
        get
        {
            return factionName.ToString();
        }
    }
}
