using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class TimeManager : NetworkBehaviour
{
    public delegate void TimeEvent();

    public event TimeEvent OnStartNight = () => { };
    public event TimeEvent OnStartDay = () => { };
    [SyncVar]
    [SerializeField]
    float actualTime = 0f;
    [SerializeField]
    float maxTime = 24f;
    [SerializeField]
    float realTimePerDay = 300f;
    [SerializeField]
    float startTimeDay = 20f;
    [SerializeField]
    float startTimeNight = 6f;
    [SerializeField]
    Material shaderNight;
    [SerializeField]
    Material skew;

    private static TimeManager instance = null;

    public static TimeManager GetTimeManager
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    [ClientRpc]
    void RpcOnStartDay()
    {
        OnStartDay();
    }

    [ClientRpc]
    void RpcOnStartNight()
    {
        OnStartNight();
    }

    public bool IsDay()
    {
        return actualTime >= startTimeDay || actualTime < startTimeNight;
    }

    public bool IsNight()
    {
        return actualTime < startTimeDay && actualTime >= startTimeNight;
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 24; i++)
        {
            actualTime = i + 0.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            float lastTime = actualTime;
            actualTime += Time.deltaTime / realTimePerDay * maxTime;
            if (lastTime < startTimeDay && actualTime >= startTimeDay)
            {
                RpcOnStartDay();
            }
            else if (lastTime < startTimeNight && actualTime >= startTimeNight)
            {
                RpcOnStartNight();
            }
            if (actualTime > maxTime)
            {
                actualTime = 0f;
            }
        }
        float normalizedValue = Mathf.Cos((actualTime / maxTime) * Mathf.PI * 2f);
        shaderNight.SetFloat("_InGameTime", normalizedValue / 2f);
        skew.SetFloat("_PersY", normalizedValue * 1.5f);
    }
}
