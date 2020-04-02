using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager GetPlayerManager { get; private set; }

    [SerializeField] GameObject prefabsPlayer;
    public GameObject currentPlayer;
    public Dictionary<int, List<GameObject>> listConnectedPlayers;
    //public List<GameObject> listConnectedPlayers;
    public PlayerController playerController;
    public NetworkIdentity playerIdentity;
    Player player;




    #region assessors
    public Player Player
    {
        get
        {
            return player;
        }
    }

    public ColorHair HairColor
    {
        get
        {
            return player.HairColor;
        }
    }

    public int CurrentCursor
    {
        get
        {
            return player.CurrentCursor;
        }
    }
    #endregion

    private void Awake()
    {
        if (GetPlayerManager == null)
        {
            GetPlayerManager = this;
        }
        else if (GetPlayerManager != this)
        {
            Destroy(gameObject);
        }
        listConnectedPlayers = new Dictionary<int, List<GameObject>>();

        DontDestroyOnLoad(gameObject);
    }
    public void ClearListConnectedPlayers()
    {
        foreach (KeyValuePair<int, List<GameObject>> list in listConnectedPlayers)
        {
            list.Value.Clear();
        }
    }
    //give xp to player
    public void AddXpToPlayer(int amount)
    {
        Player.Classe.AddXP(amount);
    }

    //current player's level
    public int GetLevel()
    {
        return Player.Level;
    }


    /*public void CreatePlayer(string name, Classe classe, Faction faction, CustomPlayer customPlayer)
    {
        player = new Player(name, classe, faction,customPlayer);
        SaveDataManager.Save(Player);
    }*/

    public void CreatePlayer()
    {
        player = new Player();
    }

    public void SavePlayer(Player _player)
    {
        player = _player;
        SaveDataManager.Save(_player);
    }

    public void LoadPlayer()
    {
        player = (Player)SaveDataManager.Load();
    }
}