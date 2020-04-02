using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameManager.GetGameManager.OnReturnMenu += Destroy;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.GetGameManager.OnReturnMenu -= Destroy;
    }
}
