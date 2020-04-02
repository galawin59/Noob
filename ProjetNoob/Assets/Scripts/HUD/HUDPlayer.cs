using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HUDPlayer : NetworkBehaviour
{ 
    [SerializeField] Text level;
    // Use this for initialization
    void Start()
    {
        //level.text = PlayerManager.GetPlayerManager.GetLevel().ToString();
        //PlayerManager.GetPlayerManager.Player.Classe.OnLevelUp += OnLevelUp;
    }

    void OnLevelUp()
    {
       // level.text = PlayerManager.GetPlayerManager.GetLevel().ToString();
    }
}
