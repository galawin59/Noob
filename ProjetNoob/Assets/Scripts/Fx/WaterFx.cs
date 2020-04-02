using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFx : MonoBehaviour {

    List<SpriteRenderer> sprites;
    List<SpriteRenderer> spritesParent;
    private void Start()
    {
        sprites = new List<SpriteRenderer>();
        spritesParent = new List<SpriteRenderer>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SpriteRenderer>())
            {
                sprites.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
            }
        }
        spritesParent.Add(transform.parent.GetComponent<SpriteRenderer>());
        for (int i = 1; i < transform.parent.childCount; i++)
        {
            spritesParent.Add(transform.parent.GetChild(i).GetComponent<SpriteRenderer>());
        }
    }

    private void Update()
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].sprite = spritesParent[i].sprite;
            sprites[i].color = spritesParent[i].color;
        }
    }
}
