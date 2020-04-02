using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windows : MonoBehaviour
{

    [SerializeField]
    Sprite windowDay;
    [SerializeField]
    Sprite windowNight;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    GameObject light;
    [SerializeField]
    Sprite dayLight;
    [SerializeField]
    Sprite nightLight;
    IEnumerator Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        while (TimeManager.GetTimeManager == null)
        {
            yield return null;
        }
        if (TimeManager.GetTimeManager.IsNight())
        {
            SetWindowNight();
        }
        else
        {
            SetWindowDay();
        }
        TimeManager.GetTimeManager.OnStartDay += SetWindowDay;
        TimeManager.GetTimeManager.OnStartNight += SetWindowNight;
    }

    void OnEnable()
    {
        if (TimeManager.GetTimeManager.IsNight())
        {
            SetWindowNight();
        }
        else
        {
            SetWindowDay();
        }
    }
    void OnDisable()
    {
    }

    void SetWindowDay()
    {
        if (gameObject.activeSelf && spriteRenderer.enabled)
        {

            spriteRenderer.sprite = windowDay;

            light.GetComponent<SpriteRenderer>().sprite = dayLight;
            // light.SetActive(true);
        }
    }

    void SetWindowNight()
    {
        if (gameObject.activeSelf)
        {
            spriteRenderer.sprite = windowNight;
            light.GetComponent<SpriteRenderer>().sprite = nightLight;
            // light.SetActive(false);
        }
    }
}
