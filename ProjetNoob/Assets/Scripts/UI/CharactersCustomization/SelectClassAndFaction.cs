using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SelectClassAndFaction : MonoBehaviour
{
    [SerializeField] Button[] allButtons;
    [SerializeField] Button[] buttonOrientation;
    [SerializeField] Image currentFond;
    [SerializeField] Image currentFlag;
    [SerializeField] Sprite[] fond;
    [SerializeField] Sprite[] flag;
    [SerializeField] int nbClasses;
    int currentfaction = 0;
    int currentclass = 0;
    int indexDir = 16;
    Sprite[,,] classesButton;
    [SerializeField] Image cursor;
    [SerializeField] Button[] currentClass;
    [SerializeField] Button[] currentFaction;
    [SerializeField] Image hair;
    [SerializeField] Image body;
    [SerializeField] Image head;
    [SerializeField] Image torse;
    [SerializeField] Image pants;
    [SerializeField] Sprite[] highlightedFaction;
    [SerializeField] Sprite transparency;
    //ceci est un curseur bleu
    [SerializeField] GameObject blueCursor;
    SpriteManager sm;
    Player player;
    Sex sex;

    // Use this for initialization
    void Start()
    {
        sm = SpriteManager.GetSpriteManager;
        player = PlayerManager.GetPlayerManager.Player;
        sex = Sex.Man;
        currentfaction = 0;
        currentclass = 0;
        for (int i = 0; i < currentClass.Length; i++)
        {
            int index = i;
            currentClass[i].onClick.AddListener(() => ChangeClass(index));
        }
        for (int i = 0; i < currentFaction.Length; i++)
        {
            int index = i;
            currentFaction[i].onClick.AddListener(() => ChangeFaction(index));
            currentFaction[i].onClick.AddListener(() => ChangeColorSelectorClass(index));
        }
        classesButton = new Sprite[3, 2, nbClasses];
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < nbClasses; i++)
            {
                classesButton[j, 0, i] = Resources.Load<Sprite>("Sprites/HUD/Buttons/Classes/" + j + "/Basic/" + i);
                classesButton[j, 1, i] = Resources.Load<Sprite>("Sprites/HUD/Buttons/Classes/" + j + "/Highlighted/" + i);
            }
        }
        for (int i = 0; i < buttonOrientation.Length; i++)
        {
            int index = i;
            buttonOrientation[i].onClick.AddListener(() => { ChangeOrientationCharacter(index); });
        }
        ChangeColorSelectorClass(0); ChangeFaction(0);
        ChangeClass(0);
        player.IsSpecialCharacter = false;
        player.SpecialsCharacters = SpecialsCharacters.None;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeColorSelectorClass(int index)
    {
        SpriteState tmpSpriteState = new SpriteState();
        for (int i = 0; i < currentClass.Length; i++)
        {
            currentClass[i].image.sprite = classesButton[index, 0, i];
            tmpSpriteState.highlightedSprite = classesButton[index, 1, i];
            currentClass[i].spriteState = tmpSpriteState;
        }
        for (int i = 0; i < currentFaction.Length; i++)
        {
            currentFaction[i].image.sprite = transparency;
        }
        currentClass[currentclass].image.sprite = classesButton[index, 1, currentclass];
        currentFaction[index].image.sprite = highlightedFaction[index];
        currentFond.sprite = fond[index];
        currentFlag.sprite = flag[index];
        for (int i = 0; i < allButtons.Length; i++)
        {
            allButtons[i].image.sprite = sm.factionButtonMenu[index, 1];
            tmpSpriteState.highlightedSprite = sm.factionButtonMenu[index, 0];
            allButtons[i].spriteState = tmpSpriteState;
        }
    }

    void ResetCharacterPos()
    {
        indexDir = 16;
        hair.sprite = sm.hairList[(int)sex, (int)TypeHair.Classic, (int)ColorHair.Brown][indexDir];
        body.sprite = sm.bodyList[(int)ColorBody.White][indexDir];
        if (sm.armorList[(int)sex, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Head].Count != 0)
        {
            head.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Head][indexDir];
        }

        if (sm.armorList[(int)sex, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Torse].Count != 0)
        {
            torse.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Torse][indexDir]; ;
        }

        if (sm.armorList[(int)sex, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Pant].Count != 0)
        {
            pants.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Pant][indexDir]; ;
        }
    }

    public void ChangeOrientationCharacter(int index)
    {
        if (index == 1)
            indexDir += 4;
        else
            indexDir -= 4;
        if (indexDir < 8)
        {
            indexDir = 20;
        }
        else if (indexDir > 20)
        {
            indexDir = 8;
        }

        hair.sprite = sm.hairList[(int)sex, (int)TypeHair.Classic, (int)ColorHair.Brown][indexDir];
        body.sprite = sm.bodyList[(int)ColorBody.White][indexDir];
        if (sm.armorList[(int)sex, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Head].Count != 0)
        {
            head.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Head][indexDir];
        }

        if (sm.armorList[(int)sex, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Torse].Count != 0)
        {
            torse.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Torse][indexDir]; ;
        }

        if (sm.armorList[(int)sex, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Pant].Count != 0)
        {
            pants.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Pant][indexDir]; ;
        }
    }

    public void ChangeFaction(int index)
    {
        cursor.sprite = sm.Cursors[index];
        player.Faction = (FactionName)index;
        player.CurrentCursor = index;
        currentfaction = index;
    }

    public void ChangeClass(int index)
    {
        if (index != 1)
            sex = Sex.Man;
        else
            sex = Sex.Woman;
        player.Sexe = sex;
        player.Archetype = (Archetype)index;
        currentclass = index;
        ResetCharacterPos();
        blueCursor.transform.position = new Vector2(currentClass[index].transform.position.x - 26.0f, currentClass[index].transform.position.y);

        for (int i = 0; i < currentClass.Length; i++)
        {
            currentClass[i].image.sprite = classesButton[currentfaction, 0, i];
        }
        currentClass[index].image.sprite = classesButton[currentfaction, 1, index];
        if (sm.armorList[(int)sex, index, (int)ColorArmor.Classic, (int)PartBody.Head].Count != 0)
        {
            head.color = Color.white;
            head.sprite = sm.armorList[(int)sex, index, (int)ColorArmor.Classic, (int)PartBody.Head][sm.armorList[(int)sex, index, (int)ColorArmor.Classic, (int)PartBody.Head].FindIndex(item => item.name == "Head_Face_0")];
            player.HasAHat = true;
        }
        else
        {
            head.color = sm.WhiteTransparentColor;
            player.HasAHat = false;
        }

        if (!player.HasAHat)
        {
            hair.sprite = sm.hairList[(int)sex, (int)TypeHair.Classic, (int)ColorHair.Brown][sm.hairList[(int)sex, (int)TypeHair.Classic, (int)ColorHair.Brown].FindIndex(item => item.name == "Hair_Face_0")];
            hair.color = Color.white;
        }
        else
        {
            hair.color = sm.WhiteTransparentColor;
        }

        if (sm.armorList[(int)sex, index, (int)ColorArmor.Classic, (int)PartBody.Torse].Count != 0)
        {
            torse.color = Color.white;
            torse.sprite = sm.armorList[(int)sex, index, (int)ColorArmor.Classic, (int)PartBody.Torse][sm.armorList[(int)sex, index, (int)ColorArmor.Classic, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Face_0")];
        }
        else
        {
            torse.color = sm.WhiteTransparentColor;
        }

        if (sm.armorList[(int)sex, index, (int)ColorArmor.Classic, (int)PartBody.Pant].Count != 0)
        {
            pants.color = Color.white;
            pants.sprite = sm.armorList[(int)sex, index, (int)ColorArmor.Classic, (int)PartBody.Pant][sm.armorList[(int)sex, index, (int)ColorArmor.Classic, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Face_0")];
        }
        else
        {
            pants.color = sm.WhiteTransparentColor;
        }
    }
}
