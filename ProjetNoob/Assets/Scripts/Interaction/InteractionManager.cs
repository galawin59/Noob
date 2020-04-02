using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.SceneManagement;

public enum Anwser
{
    none,
    yes,
    no,
    cancel
}

public class InteractionManager : NetworkBehaviour
{

    public enum TypeRequest
    {
        trade,
        msg
    }

    private static InteractionManager instance = null;
    Camera mainCamera;
    int layerMask = 1 << 8;
    bool isOpen;
    NetworkIdentity targetIdentity;
    string targetName;
    Anwser anwser = Anwser.none;
    Transform targetTransform;
    PlayerController playerController;
    NetworkIdentity identity;

    public enum Interactions
    {
        trade,
        sendMsg,
        nbInteractions
    }

    #region assessors
    // Game Instance Singleton
    public static InteractionManager GetInteractionManager
    {
        get
        {
            return instance;
        }
    }

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }
    }

    public string TargetName
    {
        get
        {
            return targetName;
        }

        private set
        {
            targetName = value;
        }
    }
    #endregion

    public delegate void InteractionNeedAnswer(string targetName, TypeRequest typeRequest);
    public delegate void InteractionEvent();

    public event InteractionEvent InteractionOpen = () => { };
    public event InteractionEvent InteractionClose = () => { };
    public event InteractionNeedAnswer NeedAnswer = (string targetName, TypeRequest typeRequest) => { };

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            //Destroy(this.gameObject);
            return;
        }
        instance = this;
        mainCamera = Camera.main;
        isOpen = false;
        DontDestroyOnLoad(this.gameObject);
        GameManager.GetGameManager.OnReturnMenu += Destroy;
    }

    IEnumerator Start()
    {
        while (playerController == null)
        {
            playerController = PlayerManager.GetPlayerManager.playerController;
            yield return null;
        }
        while (identity == null)
        {
            identity = GetComponent<NetworkIdentity>();
        }
    }

    private void Update()
    {
        if (TradeManager.GetTradeManager.IsOpen && targetTransform != null && Vector2.Distance(PlayerManager.GetPlayerManager.currentPlayer.transform.position, targetTransform.position) >= 3f)
        {
            CloseTrade();
            TradeManager.GetTradeManager.Close();
            ChatManager.GetChatManager.SendInfoMsg("Echange annulé", Message.MessageType.info);
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (InputManager.GetInputManager.GetMouseButtonUp(1) && !TradeManager.GetTradeManager.IsOpen)
        {
            Collider2D collider = Physics2D.OverlapCircle(mainCamera.ScreenToWorldPoint(Input.mousePosition), 0.01f, layerMask);
            if (collider)
            {
                NetworkIdentity playerIdentity = collider.GetComponent<NetworkIdentity>();
                if (playerIdentity != PlayerManager.GetPlayerManager.playerIdentity)
                {
                    targetTransform = collider.transform;
                    targetIdentity = playerIdentity;
                    targetName = collider.GetComponent<PlayerController>().namePlayer;
                    InteractionOpen();
                    isOpen = true;
                }
            }
            else
            {
                InteractionClose();
                isOpen = false;
            }

        }
        else if (InputManager.GetInputManager.GetMouseButtonUp(0) && isOpen == true)
        {
            InteractionClose();
            isOpen = false;
        }
    }

    public void Interact(int id)
    {
        if (id == 0)
        {
            if (TradeManager.GetTradeManager.IsOpen)
            {
                ChatManager.GetChatManager.SendInfoMsg("Vous êtes occupé", Message.MessageType.info);
            }
            else
            {
                StartCoroutine(ITrade());
            }
        }
        else if (id == 1)
        {
            ChatManager.GetChatManager.OpenChat();
            ChatManager.GetChatManager.targetMsg = targetName;
            ChatManager.GetChatManager.actualMessageType = ChatManager.MessageType.privateMsg;
            ChatManager.GetChatManager.ActiveInputField();
        }
    }

    public void SetAnswer(bool answer)
    {
        StartCoroutine(ISetAnswer(answer));
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.GetGameManager.OnReturnMenu -= Destroy;
    }

    IEnumerator ITrade()
    {
        if (identity == null)
        {
            identity = GetComponent<NetworkIdentity>();
        }
        if (!identity.hasAuthority)
        {
            playerController.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        CmdSendRequestForTrade(targetIdentity, PlayerManager.GetPlayerManager.Player.PlayerName);
        anwser = Anwser.none;
        StartCoroutine(WaitAnswerForTrade());
    }

    [Command]
    void CmdSendRequestForTrade(NetworkIdentity targetIdentity, string requesterName)
    {
        TargetGetRequestForTrade(targetIdentity.connectionToClient, requesterName);
    }

    [TargetRpc]
    void TargetGetRequestForTrade(NetworkConnection networkConnection, string requesterName)
    {
        int indexScene = SceneManager.GetActiveScene().buildIndex;
        GameObject targetPlayer = PlayerManager.GetPlayerManager.listConnectedPlayers[indexScene].Where(x => x.GetComponent<PlayerController>().namePlayer == requesterName).ToArray()[0];
        if (TradeManager.GetTradeManager.IsOpen)
        {
            StartCoroutine(IPlayerIsBusy(targetPlayer.GetComponent<NetworkIdentity>()));
        }
        else
        {
            targetIdentity = targetPlayer.GetComponent<NetworkIdentity>();
            targetName = requesterName;
            NeedAnswer(targetName, TypeRequest.trade);
        }
    }

    IEnumerator IPlayerIsBusy(NetworkIdentity targetIdentity)
    {
        if (identity == null)
        {
            identity = GetComponent<NetworkIdentity>();
        }
        if (!identity.hasAuthority)
        {
            playerController.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        CmdPlayerIsBusy(targetIdentity);
    }

    [Command]
    void CmdPlayerIsBusy(NetworkIdentity targetIdentity)
    {
        TargetPlayerIsBusy(targetIdentity.connectionToClient);
    }

    [TargetRpc]
    void TargetPlayerIsBusy(NetworkConnection networkConnection)
    {
        StopCoroutine(WaitAnswerForTrade());
        ChatManager.GetChatManager.SendInfoMsg(targetName + " est occupé", Message.MessageType.info);
    }

    IEnumerator WaitAnswerForTrade()
    {
        while (anwser == Anwser.none)
        {
            yield return null;
        }
        if (anwser == Anwser.yes)
        {
            TradeManager.GetTradeManager.Open();
            InventoryManager.GetInventoryManager.Open();
        }
        else
        {
            ChatManager.GetChatManager.SendInfoMsg(targetName + " a refusé l'echange", Message.MessageType.info);
        }
    }


    IEnumerator ISetAnswer(bool answer)
    {
        if (!identity.hasAuthority)
        {
            playerController.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        if (answer)
        {
            CmdSendAnswerForTrade(targetIdentity, Anwser.yes);
            TradeManager.GetTradeManager.Open();
            InventoryManager.GetInventoryManager.Open();
        }
        else
        {
            CmdSendAnswerForTrade(targetIdentity, Anwser.no);
        }
    }

    [Command]
    void CmdSendAnswerForTrade(NetworkIdentity targetIdentity, Anwser anwser)
    {
        TargetGetAnswerForTrade(targetIdentity.connectionToClient, anwser);
    }

    [TargetRpc]
    void TargetGetAnswerForTrade(NetworkConnection networkConnection, Anwser anwser)
    {
        this.anwser = anwser;
    }



    //Send item for trade
    IEnumerator ISendItemForTrade(List<int> idItem)
    {
        if (!identity.hasAuthority)
        {
            playerController.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }

        CmdSendItemForTrade(targetIdentity, idItem.ToArray());
    }

    public void SendItemForTrade(List<int> idItem)
    {
        StartCoroutine(ISendItemForTrade(idItem));
    }

    [Command]
    void CmdSendItemForTrade(NetworkIdentity targetIdentity, int[] idItem)
    {
        TargetGetItemForTrade(targetIdentity.connectionToClient, idItem);
    }

    [TargetRpc]
    void TargetGetItemForTrade(NetworkConnection networkConnection, int[] idItem)
    {
        TradeManager.GetTradeManager.SetHisTrade(idItem);
    }



    //Accept Trade
    public void AcceptTrade()
    {
        StartCoroutine(IAcceptTrade());
    }

    IEnumerator IAcceptTrade()
    {
        if (!identity.hasAuthority)
        {
            playerController.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        CmdAcceptTrade(targetIdentity);
    }

    [Command]
    void CmdAcceptTrade(NetworkIdentity targetIdentity)
    {
        TargetAcceptTrade(targetIdentity.connectionToClient);
    }

    [TargetRpc]
    void TargetAcceptTrade(NetworkConnection networkConnection)
    {
        TradeManager.GetTradeManager.HisAccept();
    }


    //Cancel Trade
    public void CancelTrade()
    {
        StartCoroutine(ICancelTrade());
    }

    IEnumerator ICancelTrade()
    {
        if (!identity.hasAuthority)
        {
            playerController.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        CmdCancelTrade(targetIdentity);
    }

    [Command]
    void CmdCancelTrade(NetworkIdentity targetIdentity)
    {
        TargetCancelTrade(targetIdentity.connectionToClient);
    }

    [TargetRpc]
    void TargetCancelTrade(NetworkConnection networkConnection)
    {
        TradeManager.GetTradeManager.HisCancel();
    }


    //Close Trade
    public void CloseTrade()
    {
        if (TradeManager.GetTradeManager.IsOpen)
        {
            StartCoroutine(ICloseTrade());
        }
    }

    IEnumerator ICloseTrade()
    {
        if (!identity.hasAuthority)
        {
            playerController.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        CmdCloseTrade(targetIdentity);
    }

    [Command]
    void CmdCloseTrade(NetworkIdentity targetIdentity)
    {
        TargetCloseTrade(targetIdentity.connectionToClient);
    }

    [TargetRpc]
    void TargetCloseTrade(NetworkConnection networkConnection)
    {
        TradeManager.GetTradeManager.Close();
        ChatManager.GetChatManager.SendInfoMsg("Echange annulé", Message.MessageType.info);
    }


    //Have place in inventory
    public void HavePlaceForTrade(bool havePlace)
    {
        if (TradeManager.GetTradeManager.IsOpen)
        {
            StartCoroutine(IHavePlaceForTrade(havePlace));
        }
    }

    IEnumerator IHavePlaceForTrade(bool havePlace)
    {
        if (!identity.hasAuthority)
        {
            playerController.CmdSetAuthority(identity);
        }
        while (!identity.hasAuthority)
        {
            yield return null;
        }
        CmdHavePlaceForTrade(targetIdentity, havePlace);
    }

    [Command]
    void CmdHavePlaceForTrade(NetworkIdentity targetIdentity, bool havePlace)
    {
        TargetHavePlaceForTrade(targetIdentity.connectionToClient, havePlace);
    }

    [TargetRpc]
    void TargetHavePlaceForTrade(NetworkConnection networkConnection, bool havePlace)
    {
        if (havePlace)
        {
            TradeManager.GetTradeManager.targetHavePlace = Anwser.yes;
        }
        else
        {
            TradeManager.GetTradeManager.targetHavePlace = Anwser.no;
        }
    }

}
