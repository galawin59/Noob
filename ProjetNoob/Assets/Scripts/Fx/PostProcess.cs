using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcess : MonoBehaviour
{
    [SerializeField]
    Material[] material;
    [SerializeField]
    Material[] BlurShadow;
    [SerializeField]
    Material CaveLight;

    PlayerController mainPlayer;
    [SerializeField]
    Camera shadowCamera;
    [SerializeField]
    Camera lightCamera;

    [SerializeField]
    int[] SceneIdOutdoor;

    [SerializeField]
    int[] SceneIdCave;

    RenderTexture rendTmp;
    RenderTexture rendTmp2;
    private void Start()
    {
        rendTmp = RenderTexture.GetTemporary(Camera.main.pixelWidth, Camera.main.pixelHeight);
        rendTmp2 = RenderTexture.GetTemporary(Camera.main.pixelWidth, Camera.main.pixelHeight);
        if (shadowCamera)
            shadowCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        if (lightCamera)
            lightCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
    }
    //Todo change settings post process
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        bool outdoor = false;
        bool inCave = false;
        if (!mainPlayer)
        {
            if (PlayerManager.GetPlayerManager.currentPlayer)
                mainPlayer = PlayerManager.GetPlayerManager.currentPlayer.GetComponent<PlayerController>();
        }


        if (mainPlayer)
        {
            foreach (int id in SceneIdOutdoor)
            {
                if (mainPlayer.sceneId == id)
                {
                    outdoor = true;
                    break;
                }
            }

            foreach (int id in SceneIdCave)
            {
                if (mainPlayer.sceneId == id)
                {
                    inCave = true;
                    break;
                }
            }
        }

        if (outdoor)
        {
            for (int i = 0; i < material.Length; i++)
            {
                if (material[i].name == "Shadow")
                {
                    RenderTexture rendShd = RenderTexture.GetTemporary(Camera.main.pixelWidth, Camera.main.pixelHeight);

                    if (BlurShadow.Length > 0)
                    {
                        if (BlurShadow[0])
                            Graphics.Blit(shadowCamera.targetTexture, rendShd, BlurShadow[0]);

                        if (BlurShadow[1])
                            Graphics.Blit(rendShd, shadowCamera.targetTexture, BlurShadow[1]);
                    }
                    material[i].SetTexture("_shadowTex", shadowCamera.targetTexture);

                    rendShd.Release();
                    RenderTexture.ReleaseTemporary(rendShd);


                }

                if (material[i].name == "DayTime")
                {
                    material[i].SetTexture("_lightTex", lightCamera.targetTexture);
                }

                if (i == 0)
                {
                    if (i + 1 == material.Length)
                    {
                        Graphics.Blit(source, destination, material[i]);
                    }
                    else Graphics.Blit(source, rendTmp, material[i]);
                }
                else if (i + 1 == material.Length)
                {
                    Graphics.Blit((i % 2 == 0) ? rendTmp2 : rendTmp, destination, material[i]);
                }
                else Graphics.Blit((i % 2 == 0) ? rendTmp2 : rendTmp, (i % 2 == 0) ? rendTmp : rendTmp2, material[i]);
            }
            rendTmp.Release();
            rendTmp2.Release();
        }
        else if (inCave)
        {
            CaveLight.SetTexture("_lightTex", lightCamera.targetTexture);
            Graphics.Blit(source, destination, CaveLight);
        }
        else
        {
            Graphics.Blit(source, destination);
        }

    }
}
