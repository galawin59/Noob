using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager GetSceenManager { get; private set; }
    Resolution res;
    private void Start()
    {
        res = Screen.currentResolution;

        if (GetSceenManager == null)
        {
            GetSceenManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (GetSceenManager != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (res.width != Screen.currentResolution.width ||
            res.height != Screen.currentResolution.height)
        {
            res = Screen.currentResolution;
            Camera camera = GameObject.Find("Shadow Camera").GetComponent<Camera>();
            camera.targetTexture.Release();
            camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);

            camera = GameObject.Find("Light Camera").GetComponent<Camera>();
            camera.targetTexture.Release();
            camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        }
    }
}
