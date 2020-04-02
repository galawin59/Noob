using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager GetSpriteManager { get; private set; }
    [SerializeField] List<Sprite> cursors;

    [SerializeField] Sprite[] itemsSpriteHud;
    [SerializeField] Sprite[] resourcesSpriteHud;
    [SerializeField] int nbSmourbiff;
    [SerializeField] public Sprite[] stunSprite;

    // Dictionary<string, string>
    public List<Sprite>[,,] hairList;
    public List<Sprite>[,,,] armorList;
    public List<Sprite>[] bodyList;
    public List<Sprite>[,] beardList;
    public List<Sprite>[] scarsList;
    public List<Sprite>[,] accessoriesList;
    public Sprite[,] factionButtonMenu;
    public List<Sprite> spritesCrate;
    public List<Sprite>[] smourbiffList;
    public List<Sprite>[] specialsCharacters;
    Color whiteTransparentColor;


    Dictionary<string, List<Sprite>[]> furnitureSprite;

    #region assessors

    public List<Sprite> Cursors
    {
        get
        {
            return cursors;
        }
    }

    public Color WhiteTransparentColor
    {
        get
        {
            return whiteTransparentColor;
        }
    }

    public Sprite[] ResourcesSpriteHud
    {
        get
        {
            return resourcesSpriteHud;
        }
    }

    public Sprite[] ItemsSpriteHud
    {
        get
        {
            return itemsSpriteHud;
        }
    }

    public int NbSmourbiff
    {
        get
        {
            return nbSmourbiff;
        }
    }
    #endregion

    public Sprite GetCrateSprite(int idSprite)
    {
        return spritesCrate[idSprite];
    }

    public Sprite GetFurnitureSprite(string furnitureName, int indexSprite, FurnitureManager.E_FurnitureDirection direction)
    {
        if (furnitureSprite.ContainsKey(furnitureName) && furnitureSprite[furnitureName][(int)direction].Count > indexSprite && indexSprite > -1)
        {
            return furnitureSprite[furnitureName][(int)direction][indexSprite];
        }
        return null;
    }

    void Awake()
    {
        if (GetSpriteManager == null)
        {
            GetSpriteManager = this;
        }
        else if (GetSpriteManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        whiteTransparentColor = Color.white;
        whiteTransparentColor.a = 0;
        hairList = new List<Sprite>[(int)Sex.nbSex, (int)TypeHair.nbHair, (int)ColorHair.nbHair];

        for (int i = 0; i < (int)Sex.nbSex; i++)
        {
            for (int k = 0; k < (int)TypeHair.nbHair; k++)
            {
                for (int j = 0; j < (int)ColorHair.nbHair; j++)
                {
                    hairList[i, k, j] = Resources.LoadAll<Sprite>("Sprites/Characters/" + (Sex)i + "/Hair/" + (TypeHair)k + "/" + (ColorHair)j).ToList();
                }
            }
        }

        armorList = new List<Sprite>[(int)Sex.nbSex, (int)Archetype.nbArchetype, (int)ColorArmor.nbColorArmor, (int)PartBody.nbPart];
        for (int i = 0; i < (int)Sex.nbSex; i++)
        {
            for (int j = 0; j < (int)Archetype.nbArchetype; j++)
            {
                for (int k = 0; k < (int)ColorArmor.nbColorArmor; k++)
                {
                    for (int l = 0; l < (int)PartBody.nbPart; l++)
                    {
                        armorList[i, j, k, l] = Resources.LoadAll<Sprite>("Sprites/Characters/" + (Sex)i + "/Classe/" + (Archetype)j + "/" + (ColorArmor)k + "/" + (PartBody)l).ToList();
                    }
                }
            }
        }

        bodyList = new List<Sprite>[(int)ColorBody.nbColorBody];
        for (int k = 0; k < (int)ColorBody.nbColorBody; k++)
        {
            bodyList[k] = Resources.LoadAll<Sprite>("Sprites/Characters/Body/" + (ColorBody)k).ToList();
        }

        beardList = new List<Sprite>[(int)TypeBeard.nbBeard, (int)ColorBeard.nbBeards];
        for (int i = 0; i < (int)TypeBeard.nbBeard; i++)
        {
            for (int j = 0; j < (int)ColorBeard.nbBeards; j++)
            {
                beardList[i, j] = Resources.LoadAll<Sprite>("Sprites/Characters/Man/Beards/" + (TypeBeard)i + "/" + (ColorBeard)j).ToList();
            }
        }

        scarsList = new List<Sprite>[(int)Scars.nbScars];
        for (int i = 0; i < (int)Scars.nbScars; i++)
        {
            scarsList[i] = Resources.LoadAll<Sprite>("Sprites/Characters/Scars/" + (Scars)i).ToList();
        }

        accessoriesList = new List<Sprite>[(int)Cape.nbCape, (int)ColorCape.nbColor];
        for (int i = 0; i < (int)Cape.nbCape; i++)
        {
            for (int j = 0; j < (int)ColorCape.nbColor; j++)
            {
                accessoriesList[i, j] = Resources.LoadAll<Sprite>("Sprites/Characters/Accessories/Cape/" + (Cape)i + "/" + (ColorCape)j).ToList();
            }
        }

        furnitureSprite = new Dictionary<string, List<Sprite>[]>();
        for (Item.TypeItem i = 0; i < Item.TypeItem.nbItem; i++)
        {
            furnitureSprite[i.ToString()] = new List<Sprite>[(int)FurnitureManager.E_FurnitureDirection.NB_DIR];
            for (FurnitureManager.E_FurnitureDirection j = 0; j < FurnitureManager.E_FurnitureDirection.NB_DIR; j++)
            {
                furnitureSprite[i.ToString()][(int)j] = Resources.LoadAll<Sprite>("Sprites/Furnitures/" + i.ToString() + "/" + j.ToString()).ToList();
            }
        }

        factionButtonMenu = new Sprite[3, 2];
        for (int i = 0; i < 3; i++)
        {
            factionButtonMenu[i, 0] = Resources.Load<Sprite>("Sprites/HUD/Buttons/Menu/" + i + "/Basic/0");
            factionButtonMenu[i, 1] = Resources.Load<Sprite>("Sprites/HUD/Buttons/Menu/" + i + "/Highlighted/0");
        }

        spritesCrate = new List<Sprite>();
        spritesCrate = Resources.LoadAll<Sprite>("Sprites/Crate").ToList();

        smourbiffList = new List<Sprite>[nbSmourbiff];
        for (int i = 0; i < nbSmourbiff; i++)
        {
            smourbiffList[i] = Resources.LoadAll<Sprite>("Sprites/Pets/" + i).ToList();
        }

        specialsCharacters = new List<Sprite>[(int)SpecialsCharacters.nbSpecial];
        for(int i = 0; i < (int)SpecialsCharacters.nbSpecial; i++)
        {
            specialsCharacters[i] = Resources.LoadAll<Sprite>("Sprites/Characters/Specials/" + (SpecialsCharacters)i).ToList();
        }
    }

}
