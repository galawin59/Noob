using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizePlayerSaved : MonoBehaviour
{
    [SerializeField] GameObject visual;
    [SerializeField] Image cursor;
    [SerializeField] Image hair;
    [SerializeField] Image scar;
    [SerializeField] Image beard;
    [SerializeField] Image body;
    [SerializeField] Image head;
    [SerializeField] Image torse;
    [SerializeField] Image pants;
    [SerializeField] Image cape;
    [SerializeField] Image special;
    [SerializeField] Text textName;
    [SerializeField] Button[] buttons;
    SpriteManager sm;

    // Use this for initialization
    void Start()
    {
        sm = SpriteManager.GetSpriteManager;
        if (SaveDataManager.IsSaveExists())
        {
            PlayerManager.GetPlayerManager.LoadPlayer();
            Player tmpPlayer = PlayerManager.GetPlayerManager.Player;
            cursor.sprite = sm.Cursors[tmpPlayer.CurrentCursor];
            textName.text = tmpPlayer.PlayerName;
            if (tmpPlayer.IsSpecialCharacter)
            {
                special.color = Color.white;
                special.sprite = sm.specialsCharacters[(int)tmpPlayer.SpecialsCharacters][16];
                hair.color = sm.WhiteTransparentColor;
                beard.color = sm.WhiteTransparentColor;
                scar.color = sm.WhiteTransparentColor;
                head.color = sm.WhiteTransparentColor;
                torse.color = sm.WhiteTransparentColor;
                cape.color = sm.WhiteTransparentColor;
                pants.color = sm.WhiteTransparentColor;
            }
            else
            {
                if (tmpPlayer.HairType != TypeHair.HairLess && !tmpPlayer.HasAHat)
                {
                    hair.sprite = sm.hairList[(int)tmpPlayer.Sexe, (int)tmpPlayer.HairType, (int)tmpPlayer.HairColor][16];
                    hair.color = Color.white;
                }
                else
                {
                    hair.color = sm.WhiteTransparentColor;
                }

                if (tmpPlayer.TypeBeard != TypeBeard.None && tmpPlayer.ArmorColor[PartBody.Head] == ColorArmor.None)
                {
                    beard.sprite = sm.beardList[(int)tmpPlayer.TypeBeard, (int)tmpPlayer.ColorBeard][12];
                    beard.color = Color.white;
                }
                else
                {
                    beard.color = sm.WhiteTransparentColor;
                }


                if (tmpPlayer.Scars != Scars.None)
                {
                    scar.sprite = sm.scarsList[(int)tmpPlayer.Scars][12];
                    scar.color = Color.white;
                }
                else
                {
                    scar.color = sm.WhiteTransparentColor;
                }

                if (tmpPlayer.ArmorColor[PartBody.Head] != ColorArmor.None)
                {
                    head.sprite = sm.armorList[(int)tmpPlayer.Sexe, (int)tmpPlayer.Classe.Archetype, (int)tmpPlayer.ArmorColor[PartBody.Head], (int)PartBody.Head]
                        [sm.armorList[(int)tmpPlayer.Sexe, (int)tmpPlayer.Archetype, (int)tmpPlayer.ArmorColor[PartBody.Head], (int)PartBody.Head].FindIndex(item => item.name == "Head_Face_0")];
                    head.color = Color.white;
                }
                else
                {
                    head.color = sm.WhiteTransparentColor;
                }

                if (tmpPlayer.ArmorColor[PartBody.Torse] != ColorArmor.None)
                {
                    torse.sprite = sm.armorList[(int)tmpPlayer.Sexe, (int)tmpPlayer.Classe.Archetype, (int)tmpPlayer.ArmorColor[PartBody.Torse], (int)PartBody.Torse]
                        [sm.armorList[(int)tmpPlayer.Sexe, (int)tmpPlayer.Archetype, (int)tmpPlayer.ArmorColor[PartBody.Torse], (int)PartBody.Torse].FindIndex(item => item.name == "Torse_Face_0")];
                    torse.color = Color.white;
                }
                else
                {
                    torse.color = sm.WhiteTransparentColor;
                }

                if (tmpPlayer.ArmorColor[PartBody.Pant] != ColorArmor.None)
                {
                    pants.sprite = sm.armorList[(int)tmpPlayer.Sexe, (int)tmpPlayer.Classe.Archetype, (int)tmpPlayer.ArmorColor[PartBody.Pant], (int)PartBody.Pant]
                        [sm.armorList[(int)tmpPlayer.Sexe, (int)tmpPlayer.Archetype, (int)tmpPlayer.ArmorColor[PartBody.Pant], (int)PartBody.Pant].FindIndex(item => item.name == "Pant_Face_0")];
                    pants.color = Color.white;
                }
                else
                {
                    pants.color = sm.WhiteTransparentColor;
                }

                if (tmpPlayer.Cape != Cape.None)
                {
                    cape.sprite = sm.accessoriesList[(int)tmpPlayer.Cape, (int)tmpPlayer.ColorCape]
                        [sm.accessoriesList[(int)tmpPlayer.Cape, (int)tmpPlayer.ColorCape].FindIndex(item => item.name == "Cape_Face_0")];
                    cape.color = Color.white;
                }
                else
                {
                    cape.color = sm.WhiteTransparentColor;
                }
                special.color = sm.WhiteTransparentColor;
                body.sprite = sm.bodyList[(int)tmpPlayer.BodyColor][16];
                buttons[0].interactable = true;
                buttons[1].interactable = true;
            }
        }
        else
        {
            visual.gameObject.SetActive(false);
            buttons[0].interactable = false;
            buttons[1].interactable = false;
            textName.gameObject.SetActive(false);
        }
    }

    public void CreateNewCharacter()
    {
        PlayerManager.GetPlayerManager.CreatePlayer();
    }
}
