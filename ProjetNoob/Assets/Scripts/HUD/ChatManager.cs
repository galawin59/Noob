using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    public delegate void ChatEvent();

    public event ChatEvent ChatOpen = () => { chatIsactived = true; chatIsOpen = true; timerClose = timeForClose; };
    public event ChatEvent ChatClose = () => { chatIsactived = false; };
    public event ChatEvent ChatIsUpdate = () => { };
    public event ChatEvent ChatActiveInputField = () => { };
    int currentScene;
    public int maxMessages = 10;
    [SerializeField]
    List<Message> messageList;
    public static bool chatIsactived = false;
    public static bool chatIsFocused = false;
    PlayerController pc;
    NetworkIdentity identity;
    public static bool chatIsOpen = false;
    [SerializeField] float timeForCloseChat = 10f;
    static float timeForClose = 0f;
    static float timerClose = 0f;

    List<string> whisperPlayers;

    public MessageType actualMessageType;

    public enum MessageType
    {
        global,
        privateMsg,
        guild,
        tell,
        faction,
        info
    }

    public string targetMsg;
    static ChatManager instance;
    public static ChatManager GetChatManager
    {
        get
        {
            return instance;
        }
    }

    public List<Message> MessageList
    {
        get
        {
            return messageList;
        }

        private set
        {
            messageList = value;
        }
    }

    public List<string> WhisperPlayers
    {
        get
        {
            return whisperPlayers;
        }

        private set
        {
            whisperPlayers = value;
        }
    }

    public bool ChatIsOpen
    {
        get
        {
            return chatIsOpen;
        }

        private set
        {
            chatIsOpen = value;
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

        timeForClose = timeForCloseChat;
        messageList = new List<Message>();
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        GameManager.GetGameManager.OnReturnMenu += Destroy;
    }

    IEnumerator Start()
    {
        while (PlayerManager.GetPlayerManager == null || PlayerManager.GetPlayerManager.playerController == null)
        {
            yield return null;
        }
        pc = PlayerManager.GetPlayerManager.playerController;
        while (identity == null)
        {
            identity = GetComponent<NetworkIdentity>();
            yield return null;
        }
        WhisperPlayers = new List<string>();
    }

    void Update()
    {

        if (!chatIsFocused)
        {
            timerClose -= Time.deltaTime;
        }
        if (timerClose <= 0f)
        {
            chatIsOpen = false;
            ChatClose();
        }
        if (InputManager.GetInputManager.GetMouseButtonDown(0))
        {
            chatIsactived = false;
        }


        if (InputManager.GetInputManager.GetKeyDown(KeyCode.Return, true) || InputManager.GetInputManager.GetKeyDown(KeyCode.KeypadEnter, true))
        {
            if (!chatIsOpen)
            {
                chatIsOpen = true;
                ChatOpen();
            }
        }

        if (InputManager.GetInputManager.GetKeyDown(KeyCode.Escape, true))
        {
            chatIsOpen = false;
            chatIsFocused = false;
            ChatClose();
        }

    }

    void AddMessageToList(Message message)
    {

        if (messageList.Count >= maxMessages)
        {
            for (int i = 0; i < messageList.Count - maxMessages + 1; i++)
            {
                messageList.RemoveAt(0);
            }
        }

        MessageList.Add(message);
        timerClose = timeForClose;
        ChatIsUpdate();

        ChatOpen();

    }

    public void SetTargetLastPersonWhoWhisperedToMe()
    {
        targetMsg = whisperPlayers[0];
    }

    public void OpenChat()
    {
        ChatOpen();

    }
    public void ActiveInputField()
    {
        ChatActiveInputField();
    }

    #region SendMessageAllPlayer
    [Command]
    void CmdSendMessageToAllPlayer(Message message)
    {
        RpcSendMessageToAllPlayer(message);
    }

    [ClientRpc]
    void RpcSendMessageToAllPlayer(Message message)
    {
        AddMessageToList(message);
    }

    IEnumerator ISendMessageToAll(Message message)
    {
        if (!identity.hasAuthority)
        {
            pc.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }

        CmdSendMessageToAllPlayer(message);
    }


    public void SendMessageToAll(string text, Message.MessageType messageType)
    {
        Message message = new Message();
        message.text = text;
        message.messageType = messageType;

        StartCoroutine(ISendMessageToAll(message));
    }

    #endregion
    #region SendInfoMsg



    public void SendInfoMsg(string text, Message.MessageType messageType)
    {
        Message message = new Message();
        message.text = text;
        message.messageType = messageType;
        AddMessageToList(message);
    }

    #endregion

    #region privateMessage
    [Command]
    void CmdSendPrivateMessage(Message message, string targetName, string senderName, NetworkIdentity targetIdentity)
    {
        TargetSendPrivateMessage(targetIdentity.connectionToClient, message, targetName, senderName);
    }

    [TargetRpc]
    void TargetSendPrivateMessage(NetworkConnection networkConnection, Message message, string targetName, string senderName)
    {
        if (WhisperPlayers.Contains(senderName))
        {
            WhisperPlayers.Remove(senderName);
        }
        WhisperPlayers.Add(senderName);
        AddMessageToList(message);
    }

    IEnumerator ISendPrivateMessage(Message message, NetworkIdentity targetIdentity)
    {
        if (!identity.hasAuthority)
        {
            pc.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        CmdSendPrivateMessage(message, targetMsg, PlayerManager.GetPlayerManager.Player.PlayerName, targetIdentity);
    }

    public void SendPrivateMessage(string text, Message.MessageType messageType)
    {
        if (targetMsg == PlayerManager.GetPlayerManager.playerController.namePlayer)
        {
            return;
        }
        bool playerExist = false;

        NetworkIdentity targetIdentity = null;
        foreach (KeyValuePair<int, List<GameObject>> listPlayer in PlayerManager.GetPlayerManager.listConnectedPlayers)
        {
            for (int i = 0; i < listPlayer.Value.Count; i++)
            {
                if (targetMsg == listPlayer.Value[i].GetComponent<PlayerController>().namePlayer)
                {
                    targetIdentity = listPlayer.Value[i].GetComponent<NetworkIdentity>();
                    playerExist = true;
                }
            }
        }

        /*for (int i = 0; i < .Count; i++)
         {
             if (targetMsg == PlayerManager.GetPlayerManager.listConnectedPlayers[i].GetComponent<PlayerController>().namePlayer)
             {
                 playerExist = true;
             }
         }*/
        if (playerExist)
        {
            Message message = new Message();
            message.text = "De " + PlayerManager.GetPlayerManager.Player.PlayerName + " :";
            message.text += text;
            message.messageType = messageType;
            StartCoroutine(ISendPrivateMessage(message, targetIdentity));
            Message ownMessage = new Message();
            ownMessage.text = "A " + targetMsg + " :";
            ownMessage.text += text;
            ownMessage.messageType = messageType;
            AddMessageToList(ownMessage);
        }
        else
        {
            ChatManager.GetChatManager.SendInfoMsg("Ce joueur n'est pas connecté ou n'existe pas.", Message.MessageType.info);
        }
    }
    #endregion
    #region GuildMessage
    [Command]
    void CmdSendMessageToGuild(Message message)
    {
        RpcSendMessageToGuild(message);
    }

    [ClientRpc]
    void RpcSendMessageToGuild(Message message)
    {
        AddMessageToList(message);
    }

    IEnumerator ISendMessageToGuild(Message message)
    {
        if (!identity.hasAuthority)
        {
            pc.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        CmdSendMessageToGuild(message);
    }


    public void SendMessageToGuild(string text, Message.MessageType messageType)
    {
        Message message = new Message();
        message.text = text;
        message.messageType = messageType;
        StartCoroutine(ISendMessageToGuild(message));
    }

    #endregion
    #region FactionMessage
    [Command]
    void CmdSendMessageToFaction(Message message, FactionName faction)
    {
        RpcSendMessageToFaction(message, faction);
    }

    [ClientRpc]
    void RpcSendMessageToFaction(Message message, FactionName faction)
    {
        if (faction == PlayerManager.GetPlayerManager.Player.Faction)
        {
            AddMessageToList(message);
        }
    }

    IEnumerator ISendMessageToFaction(Message message)
    {
        if (!identity.hasAuthority)
        {
            pc.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        CmdSendMessageToFaction(message, PlayerManager.GetPlayerManager.Player.Faction);
    }


    public void SendMessageToFaction(string text, Message.MessageType messageType)
    {
        Message message = new Message();
        message.text = text;
        message.messageType = messageType;
        StartCoroutine(ISendMessageToFaction(message));
    }

    #endregion
    #region TellMessage
    [Command]
    void CmdSendMessageToTell(Message message, int sceneId, Vector3 pos)
    {
        foreach (GameObject g in PlayerManager.GetPlayerManager.listConnectedPlayers[sceneId])
        {
            if (Vector2.Distance(pos, g.transform.position) <= 5)
            {
                TargetSendMessageToTell(g.GetComponent<NetworkIdentity>().connectionToClient, message);
            }
        }
    }

    [TargetRpc]
    void TargetSendMessageToTell(NetworkConnection networkConnection, Message message)
    {
        AddMessageToList(message);
    }

    IEnumerator ISendMessageToTell(Message message)
    {
        if (!identity.hasAuthority)
        {
            pc.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        currentScene = PlayerManager.GetPlayerManager.playerController.sceneId;
        CmdSendMessageToTell(message, currentScene, PlayerManager.GetPlayerManager.playerController.transform.position);
    }


    public void SendMessageToTell(string text, Message.MessageType messageType)
    {
        Message message = new Message();
        message.text = text;
        message.messageType = messageType;
        StartCoroutine(ISendMessageToTell(message));
    }

    #endregion



    void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.GetGameManager.OnReturnMenu -= Destroy;
    }
}
[System.Serializable]
public class Message
{
    public string text;
    public MessageType messageType;

    public enum MessageType
    {
        global,
        privateMessage,
        info,
        guild,
        tell,
        faction,
        jdr
    }
}

