using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite startSprite;
    [SerializeField]
    Animator animator;
    // Use this for initialization
    IEnumerator Start()
    {
        if (startSprite == null)
        {
            startSprite = spriteRenderer.sprite;
        }
        if (spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        while (TimeManager.GetTimeManager == null)
        {
            yield return null;
        }
        if (TimeManager.GetTimeManager.IsNight())
        {
            EnableLight();
        }
        TimeManager.GetTimeManager.OnStartDay += DisableLight;
        TimeManager.GetTimeManager.OnStartNight += EnableLight;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnEnable()
    {
        if (TimeManager.GetTimeManager.IsNight())
        {
            EnableLight();
        }
        else
        {
            DisableLight();
        }
    }
    void OnDisable()
    {
    }

    void EnableLight()
    {
        animator.enabled = true;
    }

    void DisableLight()
    {
        animator.enabled = false;
        spriteRenderer.sprite = startSprite;
    }
}
