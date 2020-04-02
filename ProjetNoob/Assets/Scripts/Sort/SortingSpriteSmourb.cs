using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingSpriteSmourb : MonoBehaviour
{
    Collider2D collider;
    SpriteRenderer spr;
    // Use this for initialization
    void Start()
    {
        //Sorting

        collider = GetComponent<Collider2D>();
        spr = transform.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spr)
        {
            if (collider)
            {
                spr.sortingOrder = (int)(((transform.position.y + collider.offset.y) * 100.0f) * -1 + 1000);
            }
            else
            {
                spr.sortingOrder = (int)((transform.position.y * 100.0f) * -1 + 1000);
            }
        }
    }
}
