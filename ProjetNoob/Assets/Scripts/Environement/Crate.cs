using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crate : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer content;

    public void SetSpriteCrate(string spriteName)
    {
        if (content)
        {
            int index = -1;
            if (spriteName == "Sword")
            {
                index = 0;
            }
            else if (spriteName == "Fluxball")
            {
                index = 1;
            }
            else if (spriteName == "Fruits")
            {
                index = 2;
            }
            else if (spriteName == "Fruits2")
            {
                index = 3;
            }
            else if (spriteName == "Fruits3")
            {
                index = 4;
            }
            else if (spriteName == "Close")
            {
                index = 5;
            }
            else
            {
                index = -1;
            }
            if (index != -1)
            {
                content.sprite = SpriteManager.GetSpriteManager.GetCrateSprite(index);
            }
        }
    }
}
