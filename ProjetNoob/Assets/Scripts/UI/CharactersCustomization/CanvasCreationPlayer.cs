using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CanvasCreationPlayer : NetworkBehaviour
{
    [SerializeField] InputField namePlayer;

    [SerializeField] Text playerName;

    [SerializeField] Image playerNameImage;

    [SerializeField] GameObject startButton;

    [SerializeField] GameObject startButtonColor;

    [SerializeField] Button prefabsButton;

    [SerializeField] Button prefabsButtonArrow;

    [SerializeField] Button prefabsButtonHead;

    [SerializeField] Button prefabsButtonBody;

    [SerializeField] Button prefabsButtonColor;

    [SerializeField] Color[] listColorHair;

    [SerializeField] Color[] listColorBeard;

    [SerializeField] Color[] listColorBody;

    [SerializeField] Color[] listColorCape;

    [SerializeField] Button[] buttonToDisplay;

    [SerializeField] GameObject[] toDisplay;

    [SerializeField] Button continueButton;

    [SerializeField] Button backButton;

    [SerializeField] Sprite[] fond;

    [SerializeField] Image fondDecor;

    [SerializeField] Sprite[] spriteDisabled;

    [SerializeField] Button[] changeDirCharacter;

    [SerializeField] Sprite[] spriteArrow;

    [SerializeField] Sprite[] headSprite;

    [SerializeField] Sprite selectedArmor;

    [SerializeField] Sprite selectedNotArmor;

    Sprite[,,] partButton;


    Button[] buttonHead;
    Button[] buttonHair;
    Button[] buttonTorse;
    Button[] buttonPants;
    Button[] colorHair;
    Button[] colorCape;
    Button[] buttonScars;
    Button[] buttonCape;
    Button[] buttonBeards;
    Button[] buttonBeardsColor;
    Button[] buttonBody;

    int[] indexDisplay;
    int indexDir = 0;

    int currentDisplay;

    int currentColorHead = 1;
    int currentHairCut = 1;
    int currentCape = 0;
    int currentHairColor = 0;
    int currentColorBody = 0;
    int currentColorTorse = 1;
    int currentColorPants = 1;
    int currentScar = 0;
    int currentTypeBeard = 0;
    int currentColorBeard = 0;
    int currentColorCape = 0;

    [SerializeField] Image hairToChange;
    [SerializeField] Image head;
    [SerializeField] Image cursor;
    [SerializeField] Image body;
    [SerializeField] Image cape;
    [SerializeField] Image scar;
    [SerializeField] Image beard;
    [SerializeField] Image torse;
    [SerializeField] Image pants;

    Player player;

    Dictionary<PartBody, ColorArmor> colorArmor;

    SpriteManager sm;
    // Use this for initialization
    void Start()
    {
        sm = SpriteManager.GetSpriteManager;
        player = PlayerManager.GetPlayerManager.Player;
        colorArmor = new Dictionary<PartBody, ColorArmor>();
        namePlayer.characterLimit = 10;
        partButton = new Sprite[4, 2, 4];
        indexDisplay = new int[9];
        for (int i = 0; i < 9; i++)
        {
            indexDisplay[i] = 0;
        }
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                partButton[j, 0, i] = Resources.Load<Sprite>("Sprites/HUD/Buttons/Editor/" + j + "/Basic/" + i);
                partButton[j, 1, i] = Resources.Load<Sprite>("Sprites/HUD/Buttons/Editor/" + j + "/Highlighted/" + i);
            }
        }
        for (int i = 0; i < changeDirCharacter.Length; i++)
        {
            int index = i;
            changeDirCharacter[i].onClick.AddListener(() => { ChangeOrientationCharacter(index); });
        }
        GetCorrectCharacter();
        CreateHair();
        CreateArmor();
        CreateHead();
        CreateBody();
        ChangeDesignButtons();
        fondDecor.sprite = fond[(int)player.Faction];
        for (int i = 0; i < toDisplay.Length; i++)
        {
            toDisplay[i].SetActive(false);
        }

        for (int i = 0; i < buttonToDisplay.Length; i++)
        {
            int index = i;
            buttonToDisplay[i].onClick.AddListener(() => Display(index));
        }
        ValidatePseudo();

        namePlayer.onValidateInput += delegate (string input, int charIndex, char addedChar) { return MyValidate(addedChar); };
        namePlayer.onValueChanged.AddListener(delegate { ValidatePseudo(); });
    }

    private char MyValidate(char charToValidate)
    {
        char tmpChar = charToValidate;
        tmpChar = Char.ToLower(charToValidate);
        //check if it's not one a the following letter
        if (tmpChar != 'a' && tmpChar != 'b' && tmpChar != 'c' && tmpChar != 'd' && tmpChar != 'e'
            && tmpChar != 'f' && tmpChar != 'g' && tmpChar != 'h' && tmpChar != 'i' && tmpChar != 'j'
            && tmpChar != 'k' && tmpChar != 'l' && tmpChar != 'm' && tmpChar != 'n' && tmpChar != 'o'
            && tmpChar != 'p' && tmpChar != 'q' && tmpChar != 'r' && tmpChar != 's' && tmpChar != 't'
            && tmpChar != 'u' && tmpChar != 'v' && tmpChar != 'w' && tmpChar != 'x' && tmpChar != 'y'
            && tmpChar != 'z' && tmpChar != 'é' && tmpChar != 'è' && tmpChar != 'à' && tmpChar != 'ù'
            && tmpChar != 'ê' && tmpChar != 'ë' && tmpChar != 'ï' && tmpChar != 'î' && tmpChar != 'â'
            && tmpChar != 'ô' && tmpChar != 'ö' && tmpChar != 'ä' && tmpChar != 'ü' && tmpChar != 'û')
        {
            // ... if it is change it to an empty character.
            charToValidate = '\0';
        }
        return charToValidate;
    }

    //get all informations about the player and send it to the manager
    //[Command]
    public void CmdValidatePlayer()
    {
        player.ArmorColor = colorArmor;
        player.PlayerName = playerName.text;
        PlayerManager.GetPlayerManager.SavePlayer(player);
    }

    void ChangeDesignButtons()
    {
        SpriteState tmpSpriteState = new SpriteState();
        tmpSpriteState.highlightedSprite = sm.factionButtonMenu[(int)player.Faction, 0];
        tmpSpriteState.disabledSprite = spriteDisabled[(int)player.Faction];

        continueButton.image.sprite = sm.factionButtonMenu[(int)player.Faction, 1];
        continueButton.spriteState = tmpSpriteState;
        playerNameImage.sprite = sm.factionButtonMenu[(int)player.Faction, 1];
        backButton.image.sprite = sm.factionButtonMenu[(int)player.Faction, 1];
        backButton.spriteState = tmpSpriteState;
        for (int i = 0; i < buttonToDisplay.Length; i++)
        {
            tmpSpriteState.highlightedSprite = partButton[(int)player.Faction, 1, i];
            buttonToDisplay[i].image.sprite = partButton[(int)player.Faction, 0, i];
            buttonToDisplay[i].spriteState = tmpSpriteState;
        }
    }

    void GetCorrectCharacter()
    {
        cursor.sprite = sm.Cursors[(int)player.Faction];
        player.HairColor = ColorHair.Brown;
        hairToChange.sprite = sm.hairList[(int)player.Sexe, (int)TypeHair.Classic, (int)ColorHair.Brown][sm.hairList[(int)player.Sexe, (int)TypeHair.Classic, (int)ColorHair.Brown].FindIndex(item => item.name == "Hair_Face_0")];
        player.HairType = TypeHair.Classic;
        player.BodyColor = ColorBody.White;
        player.ColorCape = ColorCape.Green;


        if (sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Face_0") != -1)
        {
            torse.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Torse][sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Face_0")];
            torse.color = Color.white;
            colorArmor[PartBody.Torse] = ColorArmor.Classic;
        }
        else
        {
            torse.color = sm.WhiteTransparentColor;
            colorArmor[PartBody.Torse] = ColorArmor.None;
        }

        if (sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Head].FindIndex(item => item.name == "Head_Face_0") != -1)
        {
            head.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Head][sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Head].FindIndex(item => item.name == "Head_Face_0")];
            head.color = Color.white;
            player.HasAHat = true;
            colorArmor[PartBody.Head] = ColorArmor.Classic;
        }
        else
        {
            head.color = sm.WhiteTransparentColor;
            player.HasAHat = false;
            colorArmor[PartBody.Head] = ColorArmor.None;
        }

        if (player.HasAHat)
            hairToChange.color = sm.WhiteTransparentColor;

        if (sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Face_0") != -1)
        {
            pants.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Pant][sm.armorList[(int)player.Sexe, (int)player.Archetype, (int)ColorArmor.Classic, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Face_0")];
            pants.color = Color.white;

            colorArmor[PartBody.Pant] = ColorArmor.Classic;
        }
        else
        {
            pants.color = sm.WhiteTransparentColor;
            colorArmor[PartBody.Pant] = ColorArmor.None;
        }
    }

    void ResetOrientationCharacter()
    {
        indexDir = 0;
        if (sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor].Count != 0)
            hairToChange.sprite = sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor][sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor].FindIndex(item => item.name == "Hair_Face_0")];

        if (sm.bodyList[currentColorBody].Count != 0)
            body.sprite = sm.bodyList[currentColorBody][sm.bodyList[currentColorBody].FindIndex(item => item.name == "Body_Face_0")];
        if (currentScar != (int)Scars.None && sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Face_0") != -1)
        {
            scar.sprite = sm.scarsList[currentScar][sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Face_0")];
            scar.color = Color.white;
        }
        else
        {
            scar.color = sm.WhiteTransparentColor;
        }

        if (currentTypeBeard != (int)TypeBeard.None && sm.beardList[currentTypeBeard, currentColorBeard].Count != 0 && (!player.HasAHat || currentDisplay == 1))
        {
            beard.sprite = sm.beardList[currentTypeBeard, currentColorBeard][sm.beardList[currentTypeBeard, currentColorBeard].FindIndex(item => item.name == "beard_Face_0")];
            beard.color = Color.white;
        }
        else
        {
            beard.color = sm.WhiteTransparentColor;
        }

        if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].Count != 0)
        {
            head.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].FindIndex(item => item.name == "Head_Face_0")];
        }

        if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].Count != 0)
        {
            torse.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Face_0")];
        }

        if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].Count != 0)
        {
            pants.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Face_0")];
        }
    }

    public void ChangeOrientationCharacter(int index)
    {
        if (index == 0)
            indexDir++;
        else
            indexDir--;
        if (indexDir > 3)
        {
            indexDir = 0;
        }
        else if (indexDir < 0)
        {
            indexDir = 3;
        }
        if (indexDir == 0)
        {
            if (sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor].Count != 0)
                hairToChange.sprite = sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor][sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor].FindIndex(item => item.name == "Hair_Face_0")];

            if (sm.bodyList[currentColorBody].Count != 0)
                body.sprite = sm.bodyList[currentColorBody][sm.bodyList[currentColorBody].FindIndex(item => item.name == "Body_Face_0")];

            if (sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Face_0") != -1)
            {
                scar.sprite = sm.scarsList[currentScar][sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Face_0")];
                scar.color = Color.white;
            }
            else
            {
                scar.color = sm.WhiteTransparentColor;
            }

            if (sm.beardList[currentTypeBeard, currentColorBeard].Count != 0)
            {
                beard.sprite = sm.beardList[currentTypeBeard, currentColorBeard][sm.beardList[currentTypeBeard, currentColorBeard].FindIndex(item => item.name == "beard_Face_0")];
                beard.color = Color.white;
            }
            else
            {
                beard.color = sm.WhiteTransparentColor;
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].Count != 0)
            {
                head.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].FindIndex(item => item.name == "Head_Face_0")];
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].Count != 0)
            {
                torse.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Face_0")];
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].Count != 0)
            {
                pants.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Face_0")];
            }

            if (sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].Count != 0)
            {
                cape.sprite = sm.accessoriesList[(int)player.Cape, (int)player.ColorCape][sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].FindIndex(item => item.name == "Cape_Face_0")];
                cape.color = Color.white;
            }
            else
            {
                cape.color = sm.WhiteTransparentColor;
            }
        }

        if (indexDir == 1)
        {
            if (sm.hairList[(int)player.Sexe, (int)currentHairCut, currentHairColor].Count != 0)
                hairToChange.sprite = sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor][sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor].FindIndex(item => item.name == "Hair_Droite_0")];

            if (sm.bodyList[currentColorBody].Count != 0)
                body.sprite = sm.bodyList[currentColorBody][sm.bodyList[currentColorBody].FindIndex(item => item.name == "Body_Droite_0")];

            if (sm.beardList[currentTypeBeard, currentColorBeard].FindIndex(item => item.name == "beard_Droite_0") != -1)
            {
                beard.sprite = sm.beardList[currentTypeBeard, currentColorBeard][sm.beardList[currentTypeBeard, currentColorBeard].FindIndex(item => item.name == "beard_Droite_0")];
                beard.color = Color.white;
            }
            else
            {
                beard.color = sm.WhiteTransparentColor;
            }

            if (sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Droite_0") != -1)
            {
                scar.sprite = sm.scarsList[currentScar][sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Droite_0")];
                scar.color = Color.white;
            }
            else
            {
                scar.color = sm.WhiteTransparentColor;
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].Count != 0)
            {
                head.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].FindIndex(item => item.name == "Head_Droite_0")];
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].Count != 0)
            {
                torse.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Droite_0")];
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].Count != 0)
            {
                pants.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Droite_0")];
            }

            if (sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].Count != 0)
            {
                cape.sprite = sm.accessoriesList[(int)player.Cape, (int)player.ColorCape][sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].FindIndex(item => item.name == "Cape_Droite_0")];
                cape.color = Color.white;
            }
            else
            {
                cape.color = sm.WhiteTransparentColor;
            }
        }

        if (indexDir == 2)
        {
            if (sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor].Count != 0)
                hairToChange.sprite = sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor][sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor].FindIndex(item => item.name == "Hair_Dos_0")];

            if (sm.bodyList[currentColorBody].Count != 0)
                body.sprite = sm.bodyList[currentColorBody][sm.bodyList[currentColorBody].FindIndex(item => item.name == "Body_Dos_0")];

            if (sm.beardList[currentTypeBeard, currentColorBeard].FindIndex(item => item.name == "beard_Dos_0") != -1)
            {
                beard.sprite = sm.beardList[currentTypeBeard, currentColorBeard][sm.beardList[currentTypeBeard, currentColorBeard].FindIndex(item => item.name == "beard_Dos_0")];
                beard.color = Color.white;
            }
            else
            {
                beard.color = sm.WhiteTransparentColor;
            }

            if (sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Dos_0") != -1)
            {
                scar.sprite = sm.scarsList[currentScar][sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Dos_0")];
                scar.color = Color.white;
            }
            else
            {
                scar.color = sm.WhiteTransparentColor;
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].Count != 0)
            {
                head.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].FindIndex(item => item.name == "Head_Dos_0")];
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].Count != 0)
            {
                torse.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Dos_0")];
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].Count != 0)
            {
                pants.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Dos_0")];
            }

            if (sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].Count != 0)
            {
                cape.sprite = sm.accessoriesList[(int)player.Cape, (int)player.ColorCape][sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].FindIndex(item => item.name == "Cape_Dos_0")];
                cape.color = Color.white;
            }
            else
            {
                cape.color = sm.WhiteTransparentColor;
            }
        }

        if (indexDir == 3)
        {
            if (sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor].Count != 0)
                hairToChange.sprite = sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor][sm.hairList[(int)player.Sexe, currentHairCut, currentHairColor].FindIndex(item => item.name == "Hair_Gauche_0")];

            if (sm.bodyList[currentColorBody].Count != 0)
                body.sprite = sm.bodyList[currentColorBody][sm.bodyList[currentColorBody].FindIndex(item => item.name == "Body_Gauche_0")];

            if (sm.beardList[currentTypeBeard, currentColorBeard].FindIndex(item => item.name == "beard_Gauche_0") != -1)
            {
                beard.sprite = sm.beardList[currentTypeBeard, currentColorBeard][sm.beardList[currentTypeBeard, currentColorBeard].FindIndex(item => item.name == "beard_Gauche_0")];
                beard.color = Color.white;
            }
            else
            {
                beard.color = sm.WhiteTransparentColor;
            }

            if (sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Gauche_0") != -1)
            {
                scar.sprite = sm.scarsList[currentScar][sm.scarsList[currentScar].FindIndex(item => item.name == "scar_Gauche_0")];
                scar.color = Color.white;
            }
            else
            {
                scar.color = sm.WhiteTransparentColor;
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].Count != 0)
            {
                head.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].FindIndex(item => item.name == "Head_Gauche_0")];
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].Count != 0)
            {
                torse.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorTorse, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Gauche_0")];
            }

            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].Count != 0)
            {
                pants.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorPants, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Gauche_0")];
            }

            if (sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].Count != 0)
            {
                cape.sprite = sm.accessoriesList[(int)player.Cape, (int)player.ColorCape][sm.accessoriesList[(int)player.Cape, (int)player.ColorCape].FindIndex(item => item.name == "Cape_Gauche_0")];
                cape.color = Color.white;
            }
            else
            {
                cape.color = sm.WhiteTransparentColor;
            }
        }
    }

    void CreateHair()
    {
        buttonHair = new Button[(int)TypeHair.nbHair + 2];
        for (int i = 0; i < (int)TypeHair.nbHair; i++)
        {
            if ((TypeHair)i == TypeHair.HairLess || sm.hairList[(int)player.Sexe, i, (int)ColorHair.Brown].FindIndex(item => item.name == "Hair_Face_0") != -1)
            {
                Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
                tmpPos.x = tmpPos.x + (i % 2) * 279.0f;
                tmpPos.y = tmpPos.y - (i / 2) * 200.0f;
                buttonHair[i] = Instantiate(prefabsButtonHead, tmpPos, Quaternion.identity, GameObject.Find("HairSprite").transform);
                buttonHair[i].name = "Hair " + i;
                buttonHair[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
                if ((TypeHair)i == TypeHair.Classic)
                {
                    buttonHair[i].GetComponent<Image>().sprite = selectedArmor;
                }
                if ((TypeHair)i != TypeHair.HairLess)
                    buttonHair[i].transform.Find("OnHead").GetComponent<Image>().sprite = sm.hairList[(int)player.Sexe, i, (int)ColorHair.Brown][sm.hairList[(int)player.Sexe, i, (int)ColorHair.Brown].FindIndex(item => item.name == "Hair_Face_0")];
                else
                    buttonHair[i].transform.Find("OnHead").GetComponent<Image>().color = sm.WhiteTransparentColor;


                int index = i;
                buttonHair[i].onClick.AddListener(() => ChangeTypeHair(index));
            }
        }

        colorHair = new Button[(int)ColorHair.nbHair];
        for (int i = 0; i < (int)ColorHair.nbHair; i++)
        {
            Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
            tmpPos.x = startButtonColor.GetComponent<RectTransform>().anchoredPosition.x + (i % 10) * 60.0f;
            tmpPos.y -= ((int)TypeHair.nbHair / 2.0f) * 225.0f;
            colorHair[i] = Instantiate(prefabsButtonColor, tmpPos, Quaternion.identity, GameObject.Find("HairColor").transform);
            colorHair[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
            colorHair[i].GetComponent<Image>().color = listColorHair[i];
            int index = i;
            colorHair[i].onClick.AddListener(() => ChangeColorHair(index));
        }
    }

    void CreateArmor()
    {
        int nbHead = 0;
        int nbTorse = 0;
        int nbPant = 0;
        for (int i = 0; i < (int)ColorArmor.nbColorArmor; i++)
        {
            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Head].Count > 0 || i == 0)
            {
                nbHead++;
            }
            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Torse].Count > 0 || i == 0)
            {
                nbTorse++;
            }
            if (sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Pant].Count > 0 || i == 0)
            {
                nbPant++;
            }
        }

        buttonHead = new Button[nbHead + 2];
        buttonTorse = new Button[nbTorse + 2];
        buttonPants = new Button[nbPant + 2];
        for (int i = 0; i < nbHead; i++)
        {
            Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
            int index = i;
            tmpPos.x = tmpPos.x + (i % 2) * 279.0f;

            buttonHead[i] = Instantiate(prefabsButtonBody, tmpPos, Quaternion.identity, GameObject.Find("ArmorList").transform);
            buttonHead[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
            buttonHead[i].onClick.AddListener(() => ChangeHead(index));
            if (i > 1)
            {
                buttonHead[i].gameObject.SetActive(false);
            }
            if ((ColorArmor)index == ColorArmor.Classic)
            {
                buttonHead[i].GetComponent<Image>().sprite = selectedArmor;
            }

            if ((ColorArmor)index != ColorArmor.None)
            {
                if (sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Head].FindIndex(item => item.name == "Head_Face_0") != -1)
                {
                    buttonHead[i].transform.Find("Armor").GetComponent<Image>().sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Head][sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].FindIndex(item => item.name == "Head_Face_0")];
                    buttonHead[i].transform.Find("Body").GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 10.0f);
                }
                else
                {
                    buttonHead[i].interactable = false;
                    buttonHead[i].transform.Find("Armor").GetComponent<Image>().color = sm.WhiteTransparentColor;
                    buttonHead[i].transform.Find("Body").GetComponent<Image>().color = sm.WhiteTransparentColor;
                }
            }
            else
            {
                buttonHead[i].transform.Find("Body").GetComponent<Image>().color = sm.WhiteTransparentColor;
                buttonHead[i].transform.Find("Armor").GetComponent<Image>().color = sm.WhiteTransparentColor;
            }
        }

        for (int i = 0; i < nbTorse; i++)
        {
            Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
            int index = i;
            tmpPos.x = tmpPos.x + (i % 2) * 279.0f;
            tmpPos.y -= 200.0f;
            buttonTorse[i] = Instantiate(prefabsButtonBody, tmpPos, Quaternion.identity, GameObject.Find("ArmorList").transform);
            buttonTorse[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
            buttonTorse[i].onClick.AddListener(() => ChangeTorse(index));
            if (i > 1)
            {
                buttonTorse[i].gameObject.SetActive(false);
            }
            if ((ColorArmor)index == ColorArmor.Classic)
            {
                buttonTorse[i].GetComponent<Image>().sprite = selectedArmor;
            }
            if ((ColorArmor)index != ColorArmor.None)
            {
                if (sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Face_0") != -1)
                {
                    buttonTorse[i].transform.Find("Armor").GetComponent<Image>().sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Torse][sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Face_0")];
                    buttonTorse[i].transform.Find("Armor").GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 10.0f);
                    buttonTorse[i].transform.Find("Body").GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 10.0f);
                }
                else
                {
                    buttonTorse[i].interactable = false;
                    buttonTorse[i].transform.Find("Armor").GetComponent<Image>().color = sm.WhiteTransparentColor;
                    buttonTorse[i].transform.Find("Body").GetComponent<Image>().color = sm.WhiteTransparentColor;
                }
            }
            else
            {
                buttonTorse[i].transform.Find("Body").GetComponent<Image>().color = sm.WhiteTransparentColor;
                buttonTorse[i].transform.Find("Armor").GetComponent<Image>().color = sm.WhiteTransparentColor;
            }
        }

        for (int i = 0; i < nbPant; i++)
        {
            Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
            int index = i;
            tmpPos.x = tmpPos.x + (i % 2) * 279.0f;
            tmpPos.y -= 400.0f;
            buttonPants[i] = Instantiate(prefabsButtonBody, tmpPos, Quaternion.identity, GameObject.Find("ArmorList").transform);
            buttonPants[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
            buttonPants[i].onClick.AddListener(() => ChangePant(index));
            if (i > 1)
            {
                buttonPants[i].gameObject.SetActive(false);
            }
            if ((ColorArmor)index == ColorArmor.Classic)
            {
                buttonPants[i].GetComponent<Image>().sprite = selectedArmor;
            }
            if ((ColorArmor)index != ColorArmor.None)
            {
                if (sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Face_0") != -1)
                {
                    buttonPants[i].transform.Find("Armor").GetComponent<Image>().sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Pant][sm.armorList[(int)player.Sexe, (int)player.Archetype, i, (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Face_0")];
                    buttonPants[i].transform.Find("Armor").GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 10.0f);
                    buttonPants[i].transform.Find("Body").GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 10.0f);
                }
                else
                {
                    buttonPants[i].interactable = false;
                    buttonPants[i].transform.Find("Body").GetComponent<Image>().color = sm.WhiteTransparentColor;
                    buttonPants[i].transform.Find("Armor").GetComponent<Image>().color = sm.WhiteTransparentColor;
                }
            }
            else
            {
                buttonPants[i].transform.Find("Body").GetComponent<Image>().color = sm.WhiteTransparentColor;
                buttonPants[i].transform.Find("Armor").GetComponent<Image>().color = sm.WhiteTransparentColor;
            }
        }

        int j = 0;
        for (int i = nbHead; i < nbHead + 2; i++)
        {
            Vector3 tmpPos2 = startButton.GetComponent<RectTransform>().anchoredPosition;
            if (j == 0)
                tmpPos2.x = tmpPos2.x - 100.0f;
            else
                tmpPos2.x = tmpPos2.x + 279.0f + 100.0f;

            buttonHead[i] = Instantiate(prefabsButtonArrow, tmpPos2, Quaternion.identity, GameObject.Find("ArmorList").transform);
            if (j == 0)
                buttonHead[i].interactable = false;
            if (j == 1 && nbHead < 4)
                buttonHead[i].interactable = false;
            int index = j;
            buttonHead[i].GetComponent<RectTransform>().anchoredPosition = tmpPos2;
            buttonHead[i].onClick.AddListener(() => ShowOtherChoice(index, nbHead, 1, buttonHead));
            buttonHead[i].GetComponent<Image>().sprite = spriteArrow[j];
            j++;
        }
        j = 0;
        for (int i = nbTorse; i < nbTorse + 2; i++)
        {
            Vector3 tmpPos2 = startButton.GetComponent<RectTransform>().anchoredPosition;
            if (j == 0)
                tmpPos2.x = tmpPos2.x - 100.0f;
            else
                tmpPos2.x = tmpPos2.x + 279.0f + 100.0f;
            tmpPos2.y -= 200.0f;
            buttonTorse[i] = Instantiate(prefabsButtonArrow, tmpPos2, Quaternion.identity, GameObject.Find("ArmorList").transform);
            if (j == 0)
                buttonTorse[i].interactable = false;
            if (j == 1 && nbTorse < 4)
                buttonTorse[i].interactable = false;
            int index = j;
            buttonTorse[i].GetComponent<RectTransform>().anchoredPosition = tmpPos2;
            buttonTorse[i].onClick.AddListener(() => ShowOtherChoice(index, nbTorse, 8, buttonTorse));
            buttonTorse[i].GetComponent<Image>().sprite = spriteArrow[j];
            j++;
        }

        j = 0;
        for (int i = nbPant; i < nbPant + 2; i++)
        {
            Vector3 tmpPos2 = startButton.GetComponent<RectTransform>().anchoredPosition;
            if (j == 0)
                tmpPos2.x = tmpPos2.x - 100.0f;
            else
                tmpPos2.x = tmpPos2.x + 279.0f + 100.0f;
            tmpPos2.y -= 600.0f;
            buttonPants[i] = Instantiate(prefabsButtonArrow, tmpPos2, Quaternion.identity, GameObject.Find("ArmorList").transform);
            if (j == 0)
                buttonPants[i].interactable = false;
            if (j == 1 && nbPant < 4)
                buttonPants[i].interactable = false;
            int index = j;
            buttonPants[i].GetComponent<RectTransform>().anchoredPosition = tmpPos2;
            buttonPants[i].onClick.AddListener(() => ShowOtherChoice(index, nbPant, 5, buttonPants));
            buttonPants[i].GetComponent<Image>().sprite = spriteArrow[j];

            j++;
        }

    }

    void CreateBody()
    {
        buttonBody = new Button[(int)ColorBody.nbColorBody];
        for (int i = 0; i < (int)ColorBody.nbColorBody; i++)
        {
            int index = i;
            Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
            tmpPos.x = tmpPos.x + (i % 2) * 279.0f;
            buttonBody[i] = Instantiate(prefabsButton, tmpPos, Quaternion.identity, GameObject.Find("BodyList").transform);
            buttonBody[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
            buttonBody[i].onClick.AddListener(() => ChangeColorBody(index));
            buttonBody[i].transform.Find("Image").GetComponent<Image>().sprite = sm.bodyList[i][16];
            buttonBody[i].transform.Find("Image").GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 10.0f);
            if ((ColorBody)index == ColorBody.White)
            {
                buttonBody[i].GetComponent<Image>().sprite = selectedArmor;
            }
        }


        buttonCape = new Button[(int)Cape.nbCape];
        for (int i = 0; i < (int)Cape.nbCape; i++)
        {
            int index = i;
            Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
            tmpPos.x = tmpPos.x + (i % 2) * 279.0f;
            tmpPos.y -= 200.0f;
            buttonCape[i] = Instantiate(prefabsButtonBody, tmpPos, Quaternion.identity, GameObject.Find("CapeList").transform);
            buttonCape[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
            buttonCape[i].onClick.AddListener(() => ChangeCape(index));
            if (i == 1)
            {
                buttonCape[i].transform.Find("Armor").GetComponent<Image>().sprite = sm.accessoriesList[i, 0][8];
            }
            else
            {
                buttonCape[i].GetComponent<Image>().sprite = selectedArmor;

                buttonCape[i].transform.Find("Armor").GetComponent<Image>().color = sm.WhiteTransparentColor;
            }
            buttonCape[i].transform.Find("Armor").GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 10.0f);
            buttonCape[i].transform.Find("Body").GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 10.0f);
            buttonCape[i].transform.Find("Body").GetComponent<Image>().sprite = sm.bodyList[0][8];
        }
        player.Cape = Cape.None;

        colorCape = new Button[(int)ColorCape.nbColor];
        for (int i = 0; i < (int)ColorCape.nbColor; i++)
        {
            int index = i;
            Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
            tmpPos.x = startButtonColor.GetComponent<RectTransform>().anchoredPosition.x + (i % 10) * 60.0f;
            tmpPos.y -= 335.0f;
            colorCape[i] = Instantiate(prefabsButtonColor, tmpPos, Quaternion.identity, GameObject.Find("CapeColorList").transform);
            colorCape[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
            colorCape[i].GetComponent<Image>().color = listColorCape[i];
            colorCape[i].onClick.AddListener(() => ChangeColorCape(index));
        }

    }

    void CreateHead()
    {
        buttonScars = new Button[(int)Scars.nbScars + 2];
        buttonBeards = new Button[(int)TypeBeard.nbBeard + 2];
        buttonBeardsColor = new Button[(int)ColorBeard.nbBeards];
        for (int i = 0; i < (int)Scars.nbScars; i++)
        {
            int index = i;
            Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
            tmpPos.x = tmpPos.x + (i % 2) * 279.0f;
            buttonScars[i] = Instantiate(prefabsButtonHead, tmpPos, Quaternion.identity, GameObject.Find("ScarList").transform);
            buttonScars[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
            buttonScars[i].onClick.AddListener(() => ChangeScars(index));
            if ((Scars)index != Scars.None)
            {
                buttonScars[i].transform.Find("OnHead").GetComponent<Image>().sprite = sm.scarsList[i][12];
            }
            else
            {
                buttonScars[i].transform.Find("OnHead").GetComponent<Image>().color = sm.WhiteTransparentColor;
                buttonScars[i].GetComponent<Image>().sprite = selectedArmor;
            }
        }

        int j = 0;
        for (int i = (int)Scars.nbScars; i < (int)Scars.nbScars + 2; i++)
        {
            Vector3 tmpPos2 = startButton.GetComponent<RectTransform>().anchoredPosition;
            if (j == 0)
                tmpPos2.x = tmpPos2.x - 100.0f;
            else
                tmpPos2.x = tmpPos2.x + 279.0f + 100.0f;
            buttonScars[i] = Instantiate(prefabsButtonArrow, tmpPos2, Quaternion.identity, GameObject.Find("ScarList").transform);
            if (j == 0)
                buttonScars[i].interactable = false;
            if (j == 1 && (int)Scars.nbScars < 3)
                buttonScars[i].interactable = false;
            int index = j;
            buttonScars[i].GetComponent<RectTransform>().anchoredPosition = tmpPos2;
            buttonScars[i].onClick.AddListener(() => ShowOtherChoice(index, (int)Scars.nbScars, 5, buttonScars));
            buttonScars[i].GetComponent<Image>().sprite = spriteArrow[j];
            j++;
        }





        if (player.Sexe != Sex.Woman)
        {
            for (int i = 0; i < (int)TypeBeard.nbBeard; i++)
            {
                int index = i;
                Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
                tmpPos.x = tmpPos.x + (i % 2) * 279.0f;
                tmpPos.y -= 200.0f;
                buttonBeards[i] = Instantiate(prefabsButtonHead, tmpPos, Quaternion.identity, GameObject.Find("BeardList").transform);
                if (i > 1)
                {
                    buttonBeards[i].gameObject.SetActive(false);
                }
                buttonBeards[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
                buttonBeards[i].onClick.AddListener(() => ChangeTypeBeard(index));
                if ((TypeBeard)index != TypeBeard.None)
                {
                    buttonBeards[i].transform.Find("OnHead").GetComponent<Image>().sprite = sm.beardList[i, (int)ColorBeard.Brown][12];
                }
                else
                {
                    buttonBeards[i].transform.Find("OnHead").GetComponent<Image>().color = sm.WhiteTransparentColor;
                    buttonBeards[i].GetComponent<Image>().sprite = selectedArmor;
                }
            }

            j = 0;
            for (int i = (int)TypeBeard.nbBeard; i < (int)TypeBeard.nbBeard + 2; i++)
            {
                Vector3 tmpPos2 = startButton.GetComponent<RectTransform>().anchoredPosition;
                if (j == 0)
                    tmpPos2.x = tmpPos2.x - 100.0f;
                else
                    tmpPos2.x = tmpPos2.x + 279.0f + 100.0f;
                tmpPos2.y -= 200.0f;
                buttonBeards[i] = Instantiate(prefabsButtonArrow, tmpPos2, Quaternion.identity, GameObject.Find("BeardList").transform);
                if (j == 0)
                    buttonBeards[i].interactable = false;
                int index = j;
                buttonBeards[i].GetComponent<RectTransform>().anchoredPosition = tmpPos2;
                buttonBeards[i].onClick.AddListener(() => ShowOtherChoice(index, (int)TypeBeard.nbBeard, 6, buttonBeards));
                buttonBeards[i].GetComponent<Image>().sprite = spriteArrow[j];
                j++;
            }

            for (int i = 0; i < (int)ColorBeard.nbBeards; i++)
            {
                int index = i;
                Vector3 tmpPos = startButton.GetComponent<RectTransform>().anchoredPosition;
                tmpPos.x = startButtonColor.GetComponent<RectTransform>().anchoredPosition.x + (i % 10) * 60.0f;
                tmpPos.y -= 335.0f;
                buttonBeardsColor[i] = Instantiate(prefabsButtonColor, tmpPos, Quaternion.identity, GameObject.Find("ColorBeardList").transform);
                buttonBeardsColor[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
                buttonBeardsColor[i].GetComponent<Image>().color = listColorBeard[i];
                buttonBeardsColor[i].onClick.AddListener(() => ChangeColorBeard(index));
            }
        }
    }

    void Display(int index)
    {
        for (int i = 0; i < buttonToDisplay.Length; i++)
        {
            if (i != index)
            {
                buttonToDisplay[i].image.sprite = partButton[(int)player.Faction, 0, i];
                toDisplay[i].SetActive(false);
            }
            else
            {
                currentDisplay = i;
                buttonToDisplay[i].image.sprite = partButton[(int)player.Faction, 1, i];
                toDisplay[i].SetActive(true);
                if (toDisplay[i].name == "HairList")
                {
                    head.color = sm.WhiteTransparentColor;
                    if (currentHairCut != (int)TypeHair.HairLess)
                    {
                        hairToChange.color = Color.white;
                    }
                    if (currentTypeBeard != (int)TypeBeard.None)
                        beard.color = Color.white;
                }
                else if (toDisplay[i].name == "HeadList" && player.Archetype == Archetype.Assassin)
                {
                    head.color = sm.WhiteTransparentColor;
                    if (currentTypeBeard != (int)TypeBeard.None)
                        beard.color = Color.white;
                    if (currentHairCut != (int)TypeHair.HairLess)
                    {
                        hairToChange.color = Color.white;
                    }
                }
                else if (colorArmor[PartBody.Head] != (int)ColorArmor.None)
                {
                    if (sm.armorList[(int)player.Sexe, (int)player.Archetype, currentColorHead, (int)PartBody.Head].Count != 0)
                    {
                        head.color = Color.white;
                        hairToChange.color = sm.WhiteTransparentColor;
                        beard.color = sm.WhiteTransparentColor;
                    }
                }
            }
        }
    }

    void ShowOtherChoice(int index, int nbMax, int indexPart, Button[] buttons)
    {
        if (index == 0)
            indexDisplay[indexPart]--;
        else
            indexDisplay[indexPart]++;
        int j = 0;
        for (int i = 0; i < nbMax; i++)
        {
            if (i >= indexDisplay[indexPart] && i < indexDisplay[indexPart] + 2)
            {
                if (buttons[i] != null)
                {
                    buttons[i].gameObject.SetActive(true);
                    Vector2 tmpPos = buttons[i].GetComponent<RectTransform>().anchoredPosition;
                    tmpPos.x = startButton.GetComponent<RectTransform>().anchoredPosition.x + j * 279.0f;
                    buttons[i].GetComponent<RectTransform>().anchoredPosition = tmpPos;
                    j++;
                }
            }
            else
            {
                if (buttons[i] != null)
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
        ActivateDesactivateArrow(indexDisplay[indexPart], nbMax, buttons);
    }

    void ActivateDesactivateArrow(int index, int nbMax, Button[] buttons)
    {
        if (index == 0)
        {
            buttons[nbMax].interactable = false;
        }
        else
        {
            buttons[nbMax].interactable = true;
        }

        if (index == nbMax - 2)
        {
            buttons[nbMax + 1].interactable = false;
        }
        else
        {
            buttons[nbMax + 1].interactable = true;
        }
    }

    void ChangeColorHair(int index)
    {
        ResetOrientationCharacter();

        if ((TypeHair)currentHairCut != TypeHair.HairLess)
        {
            currentHairColor = index;
            hairToChange.sprite = sm.hairList[(int)player.Sexe, currentHairCut, index][16];
        }
        player.HairColor = (ColorHair)index;
    }

    void ChangeTypeHair(int index)
    {
        ResetOrientationCharacter();
        currentHairColor = 0;
        for (int j = 0; j < (int)ColorHair.nbHair; j++)
        {
            if (sm.hairList[(int)player.Sexe, index, j].Count != 0)
            {
                colorHair[j].gameObject.SetActive(true);
            }
            else
            {
                colorHair[j].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < buttonHair.Length - 2; i++)
        {
            if (i == index)
            {
                buttonHair[i].GetComponent<Image>().sprite = selectedArmor;
            }
            else if(buttonHair[i] != null)
            {
                buttonHair[i].GetComponent<Image>().sprite = selectedNotArmor;
            }
        }
        if ((TypeHair)index != TypeHair.HairLess)
        {
            currentHairCut = index;
            hairToChange.sprite = sm.hairList[(int)player.Sexe, index, currentHairColor][sm.hairList[(int)player.Sexe, index, currentHairColor].FindIndex(item => item.name == "Hair_Face_0")];
            hairToChange.color = Color.white;
        }
        else
            hairToChange.color = sm.WhiteTransparentColor;
        player.HairType = (TypeHair)index;
    }

    void ChangeHead(int index)
    {
        for (int i = 0; i < buttonHead.Length - 2; i++)
        {
            if (i == index)
                buttonHead[i].GetComponent<Image>().sprite = selectedArmor;
            else
                buttonHead[i].GetComponent<Image>().sprite = selectedNotArmor;
        }
        if ((ColorArmor)index != ColorArmor.None)
        {
            currentColorHead = index;
            head.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, index, (int)PartBody.Head][16];
            head.color = Color.white;
            player.HasAHat = true;
            hairToChange.color = sm.WhiteTransparentColor;
        }
        else
        {
            head.color = sm.WhiteTransparentColor;
            player.HasAHat = false;
            hairToChange.color = Color.white;
        }
        colorArmor[PartBody.Head] = (ColorArmor)index;
        ResetOrientationCharacter();
    }

    void ChangeTorse(int index)
    {
        ResetOrientationCharacter();
        for (int i = 0; i < buttonTorse.Length - 2; i++)
        {
            if (i == index)
                buttonTorse[i].GetComponent<Image>().sprite = selectedArmor;
            else
                buttonTorse[i].GetComponent<Image>().sprite = selectedNotArmor;
        }
        if ((ColorArmor)index != ColorArmor.None)
        {
            currentColorTorse = index;
            torse.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, index, (int)PartBody.Torse][16];
            torse.color = Color.white;
        }
        else
            torse.color = sm.WhiteTransparentColor;
        colorArmor[PartBody.Torse] = (ColorArmor)index;
    }

    void ChangePant(int index)
    {
        ResetOrientationCharacter();
        for (int i = 0; i < buttonPants.Length - 2; i++)
        {
            if (i == index)
                buttonPants[i].GetComponent<Image>().sprite = selectedArmor;
            else
                buttonPants[i].GetComponent<Image>().sprite = selectedNotArmor;
        }
        if ((ColorArmor)index != ColorArmor.None)
        {
            currentColorPants = index;
            pants.sprite = sm.armorList[(int)player.Sexe, (int)player.Archetype, index, (int)PartBody.Pant][16];
            pants.color = Color.white;
        }
        else
            pants.color = sm.WhiteTransparentColor;
        colorArmor[PartBody.Pant] = (ColorArmor)index;
    }

    void ChangeScars(int index)
    {
        ResetOrientationCharacter();
        currentScar = index;
        for (int i = 0; i < buttonScars.Length - 2; i++)
        {
            if (i == index)
                buttonScars[i].GetComponent<Image>().sprite = selectedArmor;
            else
                buttonScars[i].GetComponent<Image>().sprite = selectedNotArmor;
        }
        if ((Scars)index != Scars.None)
        {
            scar.sprite = sm.scarsList[index][12];
            scar.color = Color.white;
        }
        else
            scar.color = sm.WhiteTransparentColor;
        player.Scars = (Scars)index;
    }

    void ChangeTypeBeard(int index)
    {
        ResetOrientationCharacter();
        currentTypeBeard = index;
        for (int i = 0; i < buttonBeards.Length - 2; i++)
        {
            if (i == index)
                buttonBeards[i].GetComponent<Image>().sprite = selectedArmor;
            else
                buttonBeards[i].GetComponent<Image>().sprite = selectedNotArmor;
        }
        if ((TypeBeard)index != TypeBeard.None)
        {
            beard.sprite = sm.beardList[index, currentColorBeard][12];
            beard.color = Color.white;
        }
        else
            beard.color = sm.WhiteTransparentColor;
        player.TypeBeard = (TypeBeard)index;
    }

    void ChangeColorBeard(int index)
    {
        ResetOrientationCharacter();
        if ((TypeBeard)currentTypeBeard != TypeBeard.None)
        {
            currentColorBeard = index;
            beard.sprite = sm.beardList[currentTypeBeard, index][12];
        }
        player.ColorBeard = (ColorBeard)index;
    }

    void ChangeColorCape(int index)
    {
        ResetOrientationCharacter();
        if ((Cape)currentCape != Cape.None)
        {
            currentColorCape = index;
            cape.sprite = sm.accessoriesList[currentCape, index][16];
        }
        player.ColorCape = (ColorCape)index;
    }

    void ChangeColorBody(int index)
    {
        ResetOrientationCharacter();
        currentColorBody = index;
        body.sprite = sm.bodyList[index][16];
        player.BodyColor = (ColorBody)index;
        for (int i = 0; i < buttonBody.Length; i++)
        {
            if (i == index)
                buttonBody[i].GetComponent<Image>().sprite = selectedArmor;
            else
                buttonBody[i].GetComponent<Image>().sprite = selectedNotArmor;
        }
        for (int i = 0; i < buttonTorse.Length - 2; i++)
        {
            buttonTorse[i].transform.Find("Body").GetComponent<Image>().sprite = sm.bodyList[index][16];
        }
        for (int i = 0; i < buttonPants.Length - 2; i++)
        {
            buttonPants[i].transform.Find("Body").GetComponent<Image>().sprite = sm.bodyList[index][16];
        }
        for (int i = 0; i < buttonHead.Length - 2; i++)
        {
            buttonHead[i].transform.Find("Body").GetComponent<Image>().sprite = sm.bodyList[index][16];
        }
        for (int i = 0; i < buttonCape.Length; i++)
        {
            buttonCape[i].transform.Find("Body").GetComponent<Image>().sprite = sm.bodyList[index][8];
        }
        for (int i = 0; i < buttonScars.Length - 2; i++)
        {
            buttonScars[i].transform.Find("Head").GetComponent<Image>().sprite = headSprite[index];
        }
        for (int i = 0; i < buttonBeards.Length - 2; i++)
        {
            buttonBeards[i].transform.Find("Head").GetComponent<Image>().sprite = headSprite[index];
        }
        for (int i = 0; i < buttonHair.Length - 2; i++)
        {
            buttonHair[i].transform.Find("Head").GetComponent<Image>().sprite = headSprite[index];
        }
    }

    void ChangeCape(int index)
    {
        ResetOrientationCharacter();
        currentCape = index;
        for (int i = 0; i < (int)Cape.nbCape; i++)
        {
            if (i == index)
                buttonCape[i].GetComponent<Image>().sprite = selectedArmor;
            else
                buttonCape[i].GetComponent<Image>().sprite = selectedNotArmor;
        }
        if ((Cape)index != Cape.None)
        {
            cape.sprite = sm.accessoriesList[index, currentColorCape][16];
            cape.color = Color.white;
        }
        else
            cape.color = sm.WhiteTransparentColor;
        player.Cape = (Cape)index;
    }

    void ValidatePseudo()
    {
        playerName.text = namePlayer.text;
        Color color = continueButton.GetComponentInChildren<Text>().color;
        if (playerName.text.Length > 0)
        {
            continueButton.interactable = true;
            color.a = 1.0f;
        }
        else
        {
            continueButton.interactable = false;
            color.a = 0.5f;
        }
        continueButton.GetComponentInChildren<Text>().color = color;
    }

}
