using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortSprite : MonoBehaviour
{
    Collider2D collider;
    PolygonCollider2D poly;
    SpriteRenderer spr;
    // Use this for initialization
    void Start()
    {
        //Sorting

        collider = GetComponent<Collider2D>();
        poly = GetComponent<PolygonCollider2D>();
        spr = transform.GetComponent<SpriteRenderer>();
        if (spr)
        {
            if (collider)
            {
                spr.sortingOrder = (int)(((transform.position.y + collider.offset.y) * 100.0f) * -1 + 1000);
            }
            else if (poly)
            {
                spr.sortingOrder = (int)(((transform.position.y + poly.offset.y) * 100.0f) * -1 + 1000);

            }
            else
            {
                spr.sortingOrder = (int)((transform.position.y * 100.0f) * -1 + 1000);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
