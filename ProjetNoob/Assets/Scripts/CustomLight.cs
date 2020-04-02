using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLight : MonoBehaviour
{
    float range = 0.025f;
    Vector2 startingScale;
    void Start()
    {
        startingScale = transform.localScale;
        transform.localPosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = startingScale + new Vector2(Random.Range(-range, range), Random.Range(-range, range));
    }

}
