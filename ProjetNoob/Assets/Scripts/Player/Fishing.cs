using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fishing : NetworkBehaviour
{
    bool isLaunched;
    bool isFishing;
    bool possibleHookedFish;
    bool hooked;

    float possibleHookedFishTimer;
    float maxHookedFishTimer;
    float lauchingTimer;
    float maxDist;
    float dist;

    [SerializeField]
    GameObject Canvas;

    [SerializeField]
    GameObject Reward;


    [SerializeField] GameObject Hook;
    [SerializeField] GameObject[] fishingRods;


    GameObject fishingRod = null;
    GameObject currentHook = null;
    GameObject canvas = null;

    #region assessor
    public bool IsFishing
    {
        get
        {
            return isFishing;
        }

        set
        {
            isFishing = value;
        }
    }

    public bool IsLaunched
    {
        get
        {
            return isLaunched;
        }

        set
        {
            isLaunched = value;
        }
    }

    public bool PossibleHookedFish
    {
        get
        {
            return possibleHookedFish;
        }

        set
        {
            possibleHookedFish = value;
        }
    }

    public float Dist
    {
        get
        {
            return dist;
        }

        set
        {
            dist = value;
        }
    }

    public GameObject FishingRod
    {
        get
        {
            return fishingRod;
        }

        set
        {
            fishingRod = value;
        }
    }
    #endregion
    private void Start()
    {
        ResetStates();
        maxDist = 5.0f;
    }

    [Command]
    void CmdDestroyHook()
    {
        foreach (GameObject g in PlayerManager.GetPlayerManager.listConnectedPlayers[GetComponent<PlayerController>().sceneId])
        {
            if (gameObject != g)
                TargetDestroyHook(g.GetComponent<NetworkIdentity>().connectionToClient);
        }
    }


    [TargetRpc]
    void TargetDestroyHook(NetworkConnection _target)
    {
        Destroy(currentHook);
        Destroy(fishingRod);
    }

    [Command]
    void CmdHooked(bool _hooked, float _maxHookedFishTimer)
    {
        foreach (GameObject g in PlayerManager.GetPlayerManager.listConnectedPlayers[GetComponent<PlayerController>().sceneId])
        {
            if (gameObject != g)
                TargetHooked(g.GetComponent<NetworkIdentity>().connectionToClient, _hooked, _maxHookedFishTimer);
        }
    }

    [TargetRpc]
    void TargetHooked(NetworkConnection _target, bool _hooked, float _maxHookedFishTimer)
    {
        currentHook.GetComponent<Hook>().Anim.SetBool("hooked", _hooked);
        currentHook.GetComponent<Hook>().AnimInChild.SetBool("hooked", _hooked);
        if (_hooked)
            maxHookedFishTimer = _maxHookedFishTimer;
        else
            possibleHookedFishTimer = 0.0f;
    }

    [Command]
    void CmdLaunch(Vector2 _p0, Vector2 _p1, Vector2 _p2, Vector2 _dir, float _dist)
    {
        foreach (GameObject g in PlayerManager.GetPlayerManager.listConnectedPlayers[GetComponent<PlayerController>().sceneId])
        {
            if (gameObject != g)
            {
                TargetLaunch(g.GetComponent<NetworkIdentity>().connectionToClient, _p0, _p1, _p2, _dir, _dist);
            }
        }
    }

    [TargetRpc]
    void TargetLaunch(NetworkConnection _target, Vector2 _p0, Vector2 _p1, Vector2 _p2, Vector2 _dir, float _dist)
    {
        currentHook = Instantiate(Hook, _p0, Quaternion.identity);
        Hook h = currentHook.GetComponent<Hook>();
        currentHook.GetComponent<ObjectSceneId>().SceneName = ManagerScene.GetManagerScene.GetActiveSceneName();
        h.RefActivity = this;
        h.P0 = _p0;
        h.P2 = _p2;
        h.P1 = _p1;

        fishingRod = null;

        if (_dir.x > 0.0f && _dir.y == 0.0f)
        {
            fishingRod = Instantiate(fishingRods[2], transform);
        }
        if (_dir.x < 0.0f && _dir.y == 0.0f)
        {
            fishingRod = Instantiate(fishingRods[3], transform);
        }

        if (_dir.x == 0.0f && _dir.y > 0.0f)
        {
            fishingRod = Instantiate(fishingRods[1], transform);
        }
        if (_dir.x == 0.0f && _dir.y < 0.0f)
        {
            fishingRod = Instantiate(fishingRods[0], transform);
        }

        GameObject gm = Instantiate(h.CablePref, transform.position, Quaternion.identity);
        CableComponent cableComp = gm.GetComponentInChildren<CableComponent>();
        cableComp.ObjectToFollow = h.transform;
        cableComp.CableLength = _dist;

        cableComp.transform.position = fishingRod.transform.GetChild(0).transform.position;

    }

    public void LaunchHook(Vector2 _dir, Vector2 _position)
    {
        if (!currentHook && !PlayerManager.GetPlayerManager.playerController.IsStunned
            && !PlayerManager.GetPlayerManager.playerController.IsHarvesting)
        {
            if (_dir.x == 0.0f || _dir.y == 0.0f)
            {
                if (InputManager.GetInputManager.GetButton("Jump"))
                {
                    lauchingTimer += Time.deltaTime * 0.5f;
                }

                if (InputManager.GetInputManager.GetButtonUp("Jump"))
                {
                    IsLaunched = true;
                    currentHook = Instantiate(Hook, _position, Quaternion.identity);
                    currentHook.GetComponent<ObjectSceneId>().SceneName = ManagerScene.GetManagerScene.GetActiveSceneName();
                    Hook h = currentHook.GetComponent<Hook>();
                    dist = Mathf.Clamp((lauchingTimer * 10.0f), 0.0f, maxDist);
                    h.RefActivity = this;
                    h.P0 = _position;
                    h.P2 = _position + _dir.normalized * dist + Random.insideUnitCircle * Random.Range(-0.7f, 0.7f);
                    h.P1 = h.P0 + (h.P2 - h.P0) / 2 + (Vector2.up * dist);
                    lauchingTimer = 0.0f;

                    fishingRod = null;

                    if (_dir.x > 0.0f && _dir.y == 0.0f)
                    {
                        fishingRod = Instantiate(fishingRods[2], transform);
                    }
                    if (_dir.x < 0.0f && _dir.y == 0.0f)
                    {
                        fishingRod = Instantiate(fishingRods[3], transform);
                    }

                    if (_dir.x == 0.0f && _dir.y > 0.0f)
                    {
                        fishingRod = Instantiate(fishingRods[1], transform);
                    }
                    if (_dir.x == 0.0f && _dir.y < 0.0f)
                    {
                        fishingRod = Instantiate(fishingRods[0], transform);
                    }



                    GameObject gm = Instantiate(h.CablePref, transform.position, Quaternion.identity);
                    CableComponent cableComp = gm.GetComponentInChildren<CableComponent>();
                    cableComp.ObjectToFollow = h.transform;
                    cableComp.CableLength = (dist) / 3;

                    cableComp.transform.position = fishingRod.transform.GetChild(0).transform.position;

                    CmdLaunch(h.P0, h.P1, h.P2, _dir, cableComp.CableLength);
                }
            }

            if (!currentHook)
            {
                if (fishingRod)
                    Destroy(fishingRod);
            }
        }
    }

    public void GetFish()
    {
        if (!hooked)
        {
            if (!possibleHookedFish)
            {
                if (Random.Range(0, 500) == 99 && isLocalPlayer)
                {
                    currentHook.GetComponent<Hook>().Anim.SetBool("hooked", true);
                    currentHook.GetComponent<Hook>().AnimInChild.SetBool("hooked", true);
                    maxHookedFishTimer = Random.Range(3.0f, 6.0f);
                    CmdHooked(true, maxHookedFishTimer);
                }
            }
            else
            {
                possibleHookedFishTimer += Time.deltaTime;

                if (possibleHookedFishTimer > maxHookedFishTimer && isLocalPlayer)
                {
                    possibleHookedFish = false;
                    currentHook.GetComponent<Hook>().Anim.SetBool("hooked", false);
                    currentHook.GetComponent<Hook>().AnimInChild.SetBool("hooked", false);
                    CmdHooked(false, 0.0f);
                    possibleHookedFishTimer = 0.0f;
                }

            }

            if (InputManager.GetInputManager.GetButtonUp("Jump"))
            {
                if (possibleHookedFish)
                {
                    hooked = true;
                    possibleHookedFish = false;
                    //create UI
                    canvas = Instantiate(Canvas);


                }
                else
                {
                    CmdDestroyHook();
                    Destroy(currentHook);
                }
            }
        }
        bool haveWin = false;
        if (canvas)
        {
            FishMovement fishmvt = canvas.GetComponentInChildren<FishMovement>();
            haveWin = fishmvt.Win;
            if (fishmvt.Flee)
            {
                CmdDestroyHook();
                Destroy(currentHook);
            }

            if (fishmvt.Win)
            {
                CmdDestroyHook();
                Destroy(currentHook);
                Instantiate(Reward).GetComponent<FishingReward>().fishing = this;
                if (canvas)
                    Destroy(canvas);
            }
        }

        if (!currentHook)
        {
            if (!haveWin)
            {
                isFishing = false;
            }
            ResetStates();
            Destroy(fishingRod);
        }
    }

    public void ResetStates()
    {
        isLaunched = false;
        possibleHookedFish = false;
        hooked = false;
        possibleHookedFishTimer = 0.0f;
        maxHookedFishTimer = 0.0f;
        lauchingTimer = 0.0f;

        if (canvas)
            Destroy(canvas);
    }
}
