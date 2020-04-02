using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawShadow : MonoBehaviour
{
    SpriteRenderer spr;
    PostProcess postProcess;

    // Use this for initialization
    void Awake()
    {
        SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
        GameObject gm = new GameObject("shadow");
        spr = gm.AddComponent<SpriteRenderer>();
        spr.sprite = GetComponent<SpriteRenderer>().sprite;
        spr.material = Resources.Load<Material>("Materials/Skew");
        gm.transform.parent = transform;
        gm.transform.localPosition = Vector3.zero;
        gm.transform.localScale = Vector3.one;
        gm.layer = 12;
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        spr.sprite = GetComponent<SpriteRenderer>().sprite;

    }
}
