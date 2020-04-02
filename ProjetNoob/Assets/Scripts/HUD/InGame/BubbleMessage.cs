using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMessage : MonoBehaviour
{
    [SerializeField]
    float timeLife = 2.0f;

    RectTransform rect;
    // Use this for initialization
    void Start()
    {
        rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeLife < 0.0f)
        {
            rect.localScale = Vector3.Lerp(rect.localScale, Vector3.zero, 0.1f);
            if(rect.localScale.x < 0.1f)
            {
                Destroy(transform.gameObject);
            }
        }
        else
        {
            rect.localScale = Vector3.Lerp(rect.localScale, Vector3.one, 0.1f);
            timeLife -= Time.deltaTime;
        }
    }
}
