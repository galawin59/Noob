using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class Hook : NetworkBehaviour
{
    float count = 0.0f;
    float speed = 1.2f;

    Vector2 currentPos;
    Vector2 p0, p1, p2;
    bool inWater = false;
    bool isDestroy = false;
    Fishing refActivity;
    SCamera scam;
    Animator anim;
    Animator animInChild;
    SpriteRenderer sprRend;

    [SerializeField] GameObject cablePref;

    #region assessors
    public Vector2 P0
    {
        get
        {
            return p0;
        }

        set
        {
            p0 = value;
        }
    }

    public Vector2 P1
    {
        get
        {
            return p1;
        }

        set
        {
            p1 = value;
        }
    }

    public Vector2 P2
    {
        get
        {
            return p2;
        }

        set
        {
            p2 = value;
        }
    }

    public bool InWater
    {
        get
        {
            return inWater;
        }

        set
        {
            inWater = value;
        }
    }

    public Fishing RefActivity
    {
        get
        {
            return refActivity;
        }

        set
        {
            refActivity = value;
        }
    }

    public Animator Anim
    {
        get
        {
            return anim;
        }

        set
        {
            anim = value;
        }
    }

    public Animator AnimInChild
    {
        get
        {
            return animInChild;
        }

        set
        {
            animInChild = value;
        }
    }

    public GameObject CablePref
    {
        get
        {
            return cablePref;
        }

        set
        {
            cablePref = value;
        }
    }

    public bool IsDestroy
    {
        get
        {
            return isDestroy;
        }

        set
        {
            isDestroy = value;
        }
    }
    #endregion

    private void Start()
    {
        sprRend = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();
        animInChild = transform.GetChild(0).GetComponent<Animator>();

        Anim.enabled = false;
        animInChild.enabled = false;

        scam = Camera.main.GetComponent<SCamera>();
        if (RefActivity.GetComponent<PlayerController>().isLocalPlayer)
            scam.StopFollowingPlayer = true;
    }

    void FixedUpdate()
    {
        if (RefActivity.GetComponent<PlayerController>().isLocalPlayer)
            scam.FollowObject(transform.position, 0.1f);

        if (count < 1.0f)
        {
            count += speed * Time.deltaTime;

            Vector2 m1 = Vector2.Lerp(P0, P1, count);
            Vector2 m2 = Vector2.Lerp(P1, P2, count);
            transform.position = Vector2.Lerp(m1, m2, count);
            transform.Rotate(Vector3.forward, count * 10.0f);

        }
        else
        {

            if (!InWater)
            {
                InWater = isInWater();
                refActivity.IsFishing = InWater;
                Anim.enabled = true;
                animInChild.enabled = true;
                currentPos = transform.position;
                Anim.Play("idle_hook");
                transform.rotation = Quaternion.identity;
                if (!refActivity.IsFishing)
                {
                    refActivity.ResetStates();
                    Destroy(gameObject);
                    Destroy(refActivity.FishingRod);
                }
            }

            if (sprRend.sprite.name == "hook_10")
            {
                sprRend.enabled = false;
                refActivity.PossibleHookedFish = true;
                transform.position = currentPos + Random.insideUnitCircle * 0.01f;
            }
            else
            {
                refActivity.PossibleHookedFish = false;
                transform.position = currentPos;
                sprRend.enabled = true;
            }
        }

    }
    //To do
    bool isInWater()
    {
        GameObject gm = GameObject.Find("Water");

        if (!gm)
            return false;
        Tilemap tilemap = gm.GetComponent<Tilemap>();

        GameObject upperLayer1 = null;
        GameObject upperLayer2 = null;
        GameObject upperLayer3 = null;
        Tilemap uppertilemap1 = null;
        Tilemap uppertilemap2 = null;
        Tilemap uppertilemap3 = null;
        Sprite sp1 = null;
        Sprite sp2 = null;
        Sprite sp3 = null;

        if (gm.transform.parent.GetChild(gm.transform.GetSiblingIndex() - 1))
            upperLayer1 = gm.transform.parent.GetChild(gm.transform.GetSiblingIndex() - 1).gameObject;
        if (gm.transform.parent.GetChild(gm.transform.GetSiblingIndex() - 2))
            upperLayer2 = gm.transform.parent.GetChild(gm.transform.GetSiblingIndex() - 2).gameObject;
        if (gm.transform.parent.GetChild(gm.transform.GetSiblingIndex() - 3))
            upperLayer3 = gm.transform.parent.GetChild(gm.transform.GetSiblingIndex() - 3).gameObject;

        if (upperLayer1)
            uppertilemap1 = upperLayer1.GetComponent<Tilemap>();
        if (upperLayer2)
            uppertilemap2 = upperLayer2.GetComponent<Tilemap>();
        if (upperLayer3)
            uppertilemap3 = upperLayer3.GetComponent<Tilemap>();

        Vector3Int tilePos = tilemap.WorldToCell(transform.position);

        if (uppertilemap1)
            sp1 = uppertilemap1.GetSprite(tilePos);
        if (uppertilemap2)
            sp2 = uppertilemap2.GetSprite(tilePos);
        if (uppertilemap3)
            sp3 = uppertilemap3.GetSprite(tilePos);

        bool isTranparent = false;
        /*int x = (int)(Mathf.Abs(transform.position.x - tilePos.x) * 48.0f);
        int y = (int)(48 - Mathf.Abs(transform.position.y - tilePos.y) * 48.0f);
        Debug.Log(y +"   "+x);*/
        if (sp1)
        {
            //isTranparent = true
            /* int x = (int)(Mathf.Abs(transform.position.x - tilePos.x) * 48.0f);
             int y = (int)(48 - Mathf.Abs(transform.position.y - tilePos.y) * 48.0f);
             Debug.Log(x + " " + y+" "+ t.sprite.texture.GetPixel(x, y));
             Debug.Log(t.sprite.rect);
             Debug.Log(t.sprite.textureRect);
             Debug.Log(t.sprite.bounds);
             Debug.Log(t.sprite.name);
             if (t.sprite.texture.GetPixel(x, y).r < 0.25f)
             {
                 isTranparent = true;
             }*/
           // Debug.Log("sprite " + sp1.name);
           // Debug.Log("sprite " + sp1.texture.width+" "+ sp1.texture.height);
            /*Debug.Log(sp1.rect);
            Debug.Log(sp1.bounds);
            Debug.Log(sp1.texture.GetPixel(x, y));*/
        }
        else if (sp2)
        {
           // Debug.Log("sprite " + sp2.name);
            /* Debug.Log(sp2.rect);
             Debug.Log(sp2.bounds);
             Debug.Log(sp2.texture.GetPixels()[x + y]);*/
        }
        else if (sp3)
        {
           // Debug.Log("sprite " + sp3.name);
            /* Debug.Log(sp3.rect);
             Debug.Log(sp3.bounds);
             Debug.Log(sp3.texture.GetPixel(x, y));*/

        }
        else
        {
            isTranparent = true;
        }

        return (isTranparent && tilemap.GetTile(tilePos));
    }

    private void OnDestroy()
    {
        if (RefActivity && RefActivity.GetComponent<PlayerController>().isLocalPlayer)
            scam.StopFollowingPlayer = false;
    }
}
