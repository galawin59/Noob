using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoHarvest : MonoBehaviour
{
    TextMesh text;
    SpriteRenderer sr;
    float timer = 0.0f;
    public float maxTimer = 5.0f;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<TextMesh>();
        sr = GetComponentInChildren<SpriteRenderer>();
        GetComponent<MeshRenderer>().sortingLayerName = "HUD";
        GetComponent<MeshRenderer>().sortingOrder = 2000;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;



        transform.position += Vector3.up * Time.deltaTime / 2f;
        float percent = Mathf.Sqrt((maxTimer - timer) / maxTimer);
        text.color = new Color(text.color.r, text.color.g, text.color.b, percent);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, percent);
        if (text.color.a <= 0.0f)
        {
            Destroy(gameObject);
        }


    }
    public void SetText(string str)
    {
        if (text == null)
        {
            text = GetComponent<TextMesh>();
        }
        text.text = str;
    }

    public void SetSprite(Sprite sprite)
    {
        if (sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }
        sr.sprite = sprite;
    }
}
