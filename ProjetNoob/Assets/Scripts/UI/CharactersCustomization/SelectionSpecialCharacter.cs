using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SelectionSpecialCharacter : MonoBehaviour
{
    [SerializeField] Button[] allButtons;
    [SerializeField] Button[] buttonOrientation;
    [SerializeField] int nbClasses;
    int indexDir = 16;
    int currentCharacter = 0;
    Sprite[,,] classesButton;
    [SerializeField] Image cursor;
    [SerializeField] Image special;
    [SerializeField] Image hair;
    [SerializeField] Image body;
    [SerializeField] Image head;
    [SerializeField] Image torse;
    [SerializeField] Image pants;
    [SerializeField] Button[] currentClass;
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
        hair.color = sm.WhiteTransparentColor;
        body.color = sm.WhiteTransparentColor;
        head.color = sm.WhiteTransparentColor;
        torse.color = sm.WhiteTransparentColor;
        pants.color = sm.WhiteTransparentColor;
        special.color = Color.white;
        special.sprite = sm.specialsCharacters[(int)SpecialsCharacters.Artheon][16];
        for (int i = 0; i < currentClass.Length; i++)
        {
            int index = i;
            currentClass[i].onClick.AddListener(() => ChangeClass(index));
        }
        for (int i = 0; i < buttonOrientation.Length; i++)
        {
            int index = i;
            buttonOrientation[i].onClick.AddListener(() => { ChangeOrientationCharacter(index); });
        }

        player.IsSpecialCharacter = true;
        ChangeClass(0);
    }

    public void CmdValidatePlayer()
    {
        player.Scars = Scars.None;
        player.TypeBeard = TypeBeard.None;
        player.ArmorColor = new Dictionary<PartBody, ColorArmor>();
        for (int i = 0; i < (int)PartBody.nbPart; i++)
            player.ArmorColor[(PartBody)i] = ColorArmor.None;
        player.Cape = Cape.None;
        player.HairType = TypeHair.HairLess;
        player.PlayerName = ((SpecialsCharacters)currentCharacter).ToString();
        if ((SpecialsCharacters)currentCharacter == SpecialsCharacters.JudgeDead)
        {
            player.CurrentCursor = (int)CursorColor.Admin;
        }
        else
        {
            player.CurrentCursor = (int)CursorColor.Yellow;
        }
        PlayerManager.GetPlayerManager.SavePlayer(player);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ResetCharacterPos()
    {
        indexDir = 16;
        special.sprite = sm.specialsCharacters[currentCharacter][indexDir];
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
        special.sprite = sm.specialsCharacters[currentCharacter][indexDir];
    }

    public void ChangeClass(int index)
    {
        if (index != 1)
            sex = Sex.Man;
        else
            sex = Sex.Woman;
        player.Sexe = sex;
        player.Archetype = (Archetype)index;

        player.SpecialsCharacters = (SpecialsCharacters)index;
        ResetCharacterPos();
        currentCharacter = index;
        blueCursor.transform.position = new Vector2(currentClass[index].transform.position.x - 26.0f, currentClass[index].transform.position.y);
        special.sprite = sm.specialsCharacters[index][16];
        if ((SpecialsCharacters)currentCharacter == SpecialsCharacters.JudgeDead)
        {
            cursor.sprite = sm.Cursors[(int)CursorColor.Admin];
            player.CurrentCursor = (int)CursorColor.Admin;
        }
        else
        {
            cursor.sprite = sm.Cursors[(int)CursorColor.Yellow];
            player.CurrentCursor = (int)CursorColor.Yellow;
        }
    }
}
