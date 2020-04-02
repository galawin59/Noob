using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingSprite : MonoBehaviour
{
    int depthSort;
    List<SpriteRenderer> sprites;
    Transform pivot;
    // Use this for initialization
    private void Start()
    {
        sprites = new List<SpriteRenderer>();
        sprites.Add(transform.GetComponent<SpriteRenderer>());
        pivot = transform.Find("Pivot");
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SpriteRenderer>())
                sprites.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {

        foreach (SpriteRenderer spr in sprites)
        {
            depthSort = (int)((pivot.position.y) * 100.0f) * -1 + 1000;
            spr.sortingOrder = depthSort;
        }
        sprites[0].sortingOrder -= 1;
        sprites[1].sortingOrder += 3;
        sprites[2].sortingOrder += 3;
        sprites[3].sortingOrder += 4;
        sprites[4].sortingOrder += 3;
        sprites[5].sortingOrder += 4;
        sprites[6].sortingOrder += 1;
        sprites[7].sortingOrder += 2;
        sprites[8].sortingOrder += 1;
        sprites[10].sortingOrder -= 3;
    }
}
