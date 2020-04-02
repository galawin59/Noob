using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enum
public enum FacingDirection
{
    Dos,
    HautDroite,
    Droite,
    BasDroite,
    Face,
    BasGauche,
    Gauche,
    HautGauche,
    nbDirection
}

public enum PartBody
{
    Head,
    Torse,
    Pant,
    nbPart
}

public enum TypeHair
{
    HairLess,
    Classic,
    Coupe1,
    Coupe4,
    nbHair
}

public enum ColorHair
{
    Brown,
    Blond,
    Green,
    Blue,
    Red,
    Purple,
    Grey,
    nbHair
}

public enum ColorBody
{
    White,
    Ashe,
    nbColorBody
}

public enum ColorArmor
{
    None,
    Classic,
    Classic2,
    Classic3,
    Classic4,
    Classic5,
    nbColorArmor
}

public enum Sex
{
    Man,
    Woman,
    nbSex
}

public enum ColorBeard
{
    Brown,
    Blond,
    Blue,
    Green,
    Purple,
    nbBeards
}

public enum Scars
{
    None,
    RightEye,
    nbScars
}

public enum TypeBeard
{
    None,
    Normal,
    ZMustaches,
    nbBeard
}

public enum Accessories
{
    Cape,
    nbAccessories
}

public enum Cape
{
    None,
    Classic,
    nbCape
}

public enum ColorCape
{
    Green,
    Blue,
    Red,
    Grey,
    Brown,
    Purple,
    Yellow,
    nbColor
}

public enum SpecialsCharacters
{
    None = -1,
    Artheon,
    JudgeDead,
    Gaea,
    Ivy,
    Omega,
    Sparadrap,
    nbSpecial
}

[Serializable]
public struct CustomPlayer
{
    public Sex sex;
    public ColorBody colorBody;
    public Dictionary<PartBody, ColorArmor> colorArmor;
    public TypeHair typeHair;
    public ColorHair colorHair;
    public TypeBeard typeBeard;
    public ColorBeard colorBeard;
    public Scars scars;
    public bool hasAHat;
    public Cape cape;
    public ColorCape colorCape;
    public bool isSpecialCharacter;
    public SpecialsCharacters specialsCharacters;

}

#endregion

[Serializable]
public class Player
{

    string playerName;
    Classe classe;
    Faction faction;
    FacingDirection currentDirection;
    CustomPlayer customPlayer;
    int currentBody = 0;

    #region assessors
    public FactionName Faction
    {
        get
        {
            return faction.GetFaction;
        }

        set
        {
            faction.GetFaction = value;
        }
    }
    public string PlayerName
    {
        get
        {
            return playerName;
        }
        set
        {
            playerName = value;
        }
    }

    public Classe Classe
    {
        get
        {
            return classe;
        }
    }

    public ColorBody BodyColor
    {
        get
        {
            return customPlayer.colorBody;
        }

        set
        {
            customPlayer.colorBody = value;
        }
    }

    public int Level
    {
        get
        {
            return classe.Level;
        }
    }

    public Sex Sexe
    {
        get
        {
            return customPlayer.sex;
        }

        set
        {
            customPlayer.sex = value;
        }
    }

    public Archetype Archetype
    {
        get
        {
            return classe.Archetype;
        }

        set
        {
            classe.Archetype = value;
        }
    }

    public ColorHair HairColor
    {
        get
        {
            return customPlayer.colorHair;
        }
        set
        {
            customPlayer.colorHair = value;
        }
    }

    public TypeHair HairType
    {
        get
        {
            return customPlayer.typeHair;
        }
        set
        {
            customPlayer.typeHair = value;
        }
    }

    public int CurrentCursor
    {
        get
        {
            return faction.GetCursorColor;
        }
        set
        {
            faction.GetCursorColor = value;
        }
    }

    public int CurrentBody
    {
        get
        {
            return currentBody;
        }
        set
        {
            currentBody = value;
        }
    }

    public FacingDirection Direction
    {
        get
        {
            return currentDirection;
        }
        set
        {
            currentDirection = value;
        }
    }

    public CustomPlayer CustomPlayer
    {
        get
        {
            return customPlayer;
        }
    }

    public Dictionary<PartBody, ColorArmor> ArmorColor
    {
        get
        {
            return customPlayer.colorArmor;
        }

        set
        {
            customPlayer.colorArmor = value;
        }
    }

    public Scars Scars
    {
        get
        {
            return customPlayer.scars;
        }
        set
        {
            customPlayer.scars = value;
        }
    }

    public TypeBeard TypeBeard
    {
        get
        {
            return customPlayer.typeBeard;
        }
        set
        {
            customPlayer.typeBeard = value;
        }
    }

    public ColorBeard ColorBeard
    {
        get
        {
            return customPlayer.colorBeard;
        }
        set
        {
            customPlayer.colorBeard = value;
        }
    }

    public bool HasAHat
    {
        get
        {
            return customPlayer.hasAHat;
        }

        set
        {
            customPlayer.hasAHat = value;
        }
    }

    public Cape Cape
    {
        get
        {
            return customPlayer.cape;
        }

        set
        {
            customPlayer.cape = value;
        }
    }

    public ColorCape ColorCape
    {
        get
        {
            return customPlayer.colorCape;
        }

        set
        {
            customPlayer.colorCape = value;
        }
    }

    public bool IsSpecialCharacter
    {
        get
        {
            return customPlayer.isSpecialCharacter;
        }

        set
        {
            customPlayer.isSpecialCharacter = value;
        }
    }

    public SpecialsCharacters SpecialsCharacters
    {
        get
        {
            return customPlayer.specialsCharacters;
        }

        set
        {
            customPlayer.specialsCharacters = value;
        }
    }
    #endregion


    public Player(string playerName, Classe classe, Faction faction, CustomPlayer customPlayer)
    {
        this.playerName = playerName;
        this.classe = classe;
        this.customPlayer = customPlayer;
        this.faction = faction;
    }

    public Player()
    {
        Classe classe = new Classe(Archetype.Guerrier);
        this.classe = classe;
        Faction faction = new Faction(FactionName.Empire);
        this.faction = faction;
        customPlayer = new CustomPlayer();
        customPlayer.isSpecialCharacter = false;
        customPlayer.specialsCharacters = SpecialsCharacters.None;
       
    }
}
