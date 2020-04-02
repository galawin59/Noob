using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureCanvas : MonoBehaviour
{
    [SerializeField] Text captureText;
    float timer;
    int basicSizeFont;
    Color basicColor;
    // Use this for initialization
    void Start()
    {
        timer = 1.0f;
        Smourbiff.OnTryCatch += OnTryCatch;
        Smourbiff.OnCapture += OnCapture;
        Smourbiff.OnFlee += OnFlee;
        captureText.enabled = false;
        basicSizeFont = captureText.fontSize;
        basicColor = captureText.color;
        captureText.enabled = false;
        captureText.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTryCatch(int number)
    {
        captureText.enabled = true;
        captureText.text = number.ToString();
        StartCoroutine(ScaleAndFadeText());
    }

    void OnCapture(int index)
    {
        captureText.enabled = true;
        captureText.text = "Le smourbiff a ete capture";
        StartCoroutine(ScaleAndFadeTextEscape());
    }

    void OnFlee()
    {
        captureText.enabled = true;
        captureText.text = "Le smourbiff s'est echappe";
        StartCoroutine(ScaleAndFadeTextEscape());
    }

    IEnumerator ScaleAndFadeText()
    {
        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            Color color = captureText.color;
            color.a -= Time.deltaTime;
            captureText.fontSize++;
            captureText.color = color;
            yield return null;
        }
        captureText.enabled = false;
        captureText.fontSize = basicSizeFont;
        captureText.color = basicColor;
        timer = 1.0f;
    }

    IEnumerator ScaleAndFadeTextEscape()
    {
        captureText.fontSize = 45;
        timer = 3.0f;
        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer >= 2.0f && captureText.fontSize < 80)
            {
                captureText.fontSize++;
            }
            else if (timer > 0.0)
            {
                Color color = captureText.color;
                color.a -= Time.deltaTime;
                captureText.color = color;
            }
            yield return null;
        }
        captureText.enabled = false;
        captureText.fontSize = basicSizeFont;
        captureText.color = basicColor;
        timer = 1.0f;
    }

    private void OnDestroy()
    {
        Smourbiff.OnTryCatch -= OnTryCatch;
        Smourbiff.OnCapture -= OnCapture;
        Smourbiff.OnFlee -= OnFlee;
    }
}
