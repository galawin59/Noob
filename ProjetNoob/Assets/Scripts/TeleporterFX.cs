using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeleporterFX : NetworkBehaviour
{
    Material defaultMat;
    [SerializeField]
    Material mat;
    Renderer currentRend;
    MaterialPropertyBlock propBlock;
    float Ypos;
    bool alreadyChange;
    bool isExit = false;
    [SyncVar]
    public bool activateEffect;
    // Use this for initialization
    void Start()
    {
        alreadyChange = false;
        activateEffect = false;
        defaultMat = GetComponent<SpriteRenderer>().material;
        currentRend = transform.GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        Ypos = 1.5f;
    }

    private void Update()
    {
        if ((isExit&&!TransitionsManager.GetTransitionsManager.inGame&&GetComponent<NetworkIdentity>().isLocalPlayer)
            /*|| (!GetComponent<NetworkIdentity>().isLocalPlayer&& activateEffect)*/)
        {
            Debug.Log("exit teleporter");
            Ypos += Time.deltaTime * 2.5f;
            GetComponent<PlayerController>().IsTeleporting = true;
            GetComponent<SpriteRenderer>().GetPropertyBlock(propBlock);
            propBlock.SetFloat("_Ypos", Ypos);
            GetComponent<SpriteRenderer>().SetPropertyBlock(propBlock);
            for (int i = 0; i < 10; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().GetPropertyBlock(propBlock);
                propBlock.SetFloat("_Ypos", Ypos);
                transform.GetChild(i).GetComponent<SpriteRenderer>().SetPropertyBlock(propBlock);
            }

            if (Ypos >= 1.5f)
            {
                GetComponent<SpriteRenderer>().material = defaultMat;
                for (int i = 0; i < 10; i++)
                {
                    transform.GetChild(i).GetComponent<SpriteRenderer>().material = defaultMat;
                }
                alreadyChange = false;
                isExit = false;
                activateEffect = false;
                GetComponent<PlayerController>().IsTeleporting = false;

            }
        }

      //   || (activateEffect && !GetComponent<NetworkIdentity>().isLocalPlayer)
    }

    public bool isTeleporting()
    {

        Debug.Log("enter teleporter");

        if (!alreadyChange)
        {
            GetComponent<SpriteRenderer>().material = mat;
            for (int i = 0; i < 10; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().material = mat;
            }
            alreadyChange = true;
        }

        Ypos -= Time.deltaTime * 2.5f;

        GetComponent<SpriteRenderer>().GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Ypos", Ypos);
        GetComponent<SpriteRenderer>().SetPropertyBlock(propBlock);
        for (int i = 0; i < 10; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().GetPropertyBlock(propBlock);
            propBlock.SetFloat("_Ypos", Ypos);
            transform.GetChild(i).GetComponent<SpriteRenderer>().SetPropertyBlock(propBlock);
        }

        if (Ypos > 0.0f)
        {   
            isExit = false;
            return true;
        }
        else
        {
            isExit = true;
            return false;
        }


    }
}

//     GetComponent<SpriteRenderer>().material = defaultMat;


