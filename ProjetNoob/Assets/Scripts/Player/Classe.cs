using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum Archetype
{
    /* Archer,
     Berserker,
     Cartomancien,
     Druide,
     Elementaliste,
     Magicien,
     Necromancien,
     Neogicien,
     Paladin,*/
    Guerrier,
    Invocatrice,
    Assassin,
     Pretre,
    /*Berserk,*/
    nbArchetype
}

[Serializable]
public class Classe
{
    public delegate void DelegateLevelUp();
    public event DelegateLevelUp OnLevelUp = () => { };
    static readonly int MAX_LEVEL = 100;

    Archetype archetype;
    int level;
    int currentXP;
    int xpToNextLevel;

    #region assessors
    public int Level
    {
        get
        {
            return level;
        }
    }

    public Archetype Archetype
    {
        get
        {
            return archetype;
        }

        set
        {
            archetype = value;
        }
    }
    #endregion


    public Classe(Archetype archetype)
    {
        this.archetype = archetype;
        level = 1;
        currentXP = 0;
        CalculateXPToNextLevel();
    }

    public Classe(Archetype archetype, int level, int currentXP)
    {
        this.archetype = archetype;
        this.level = level;
        this.currentXP = currentXP;
        CalculateXPToNextLevel();
    }

    public void AddXP(int amount)
    {
        if (level < MAX_LEVEL)
        {
            currentXP += amount;
            while (currentXP > xpToNextLevel && level < MAX_LEVEL)
            {
                level++;
                OnLevelUp();
                CalculateXPToNextLevel();
            }
        }
    }

    void CalculateXPToNextLevel()
    {
        xpToNextLevel = (int)((1.0282 * (level * level * level)) + (0.02 * (level * level)) + (8.09 * level) - 8.2);
    }

}
