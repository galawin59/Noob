using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChatCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextHandler;
    [SerializeField] Button InputEmoji;
    [SerializeField] GameObject displayEmoji;
    [SerializeField] GameObject emoji;
    [SerializeField] GameObject channel;
    [SerializeField] GameObject chatPanel;
    [SerializeField] GameObject textObject;
    [SerializeField] TMP_InputField chatBox;
    [SerializeField] InputField chatInfo;
    [SerializeField] Text infoText;
    [SerializeField] TextMeshProUGUI textColorField;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject globalButton;
    [SerializeField] GameObject localButton;
    [SerializeField] GameObject guildButton;
    [SerializeField] GameObject factionButton;
    [SerializeField] GameObject globalButtonActive;
    [SerializeField] GameObject localButtonActive;
    [SerializeField] GameObject guildButtonActive;
    [SerializeField] GameObject factionButtonActive;
    [SerializeField] GameObject privateButtonActive;
    [SerializeField] GameObject global;
    [SerializeField] GameObject local;
    [SerializeField] GameObject guilde;
    [SerializeField] GameObject factionn;
    [SerializeField] GameObject privatee;
    [SerializeField] GameObject activeButton;
    [SerializeField] GameObject display;

    [SerializeField] Color playerMessage;
    [SerializeField] Color info;
    [SerializeField] Color privateMessage;
    [SerializeField] Color guild;
    [SerializeField] Color tell;
    [SerializeField] Color faction;
    //oui c'est en francais et je m'en bat ma couille
    [SerializeField] Sprite cocher;
    [SerializeField] Sprite decocher;
    //[SerializeField] Color jdr;
    [SerializeField] int maxMessageInChat = 10;
    public bool globalCanalIsActived = true;
    public bool localCanalIsActived = true;
    public bool guildCanalIsActived = true;
    public bool factionCanalIsActived = true;
    public bool privateCanalIsActived = true;
    bool ChatIsAsk = false;
    bool chanelIsDesable = true;
    bool emojiIsDisplay = false;
    bool canvasIsActive = false;
    bool activeCanal = false;
    bool notLaunch = false;
    bool focusedActive = false;

    List<GameObject> textObjects;


    float lastScrollSize = 1f;

    // Use this for initialization
    IEnumerator Start()
    {

        canvas.SetActive(false);
        while (ChatManager.GetChatManager == null)
        {
            yield return null;
        }
        textObjects = new List<GameObject>();
        ChatManager.GetChatManager.ChatClose += Close;
        ChatManager.GetChatManager.ChatOpen += Open;
        ChatManager.GetChatManager.ChatIsUpdate += UpdateHUD;
        ChatManager.GetChatManager.ChatActiveInputField += ActiveField;
        if (maxMessageInChat > ChatManager.GetChatManager.maxMessages)
        {
            maxMessageInChat = ChatManager.GetChatManager.maxMessages;
        }
        maxMessageInChat = ChatManager.GetChatManager.maxMessages;
    }

    IEnumerator Wait()
    {
        while (!ChatIsAsk)
        {
            yield return null;
        }
        if (ChatIsAsk)
        {
            canvasIsActive = true;
            canvas.SetActive(true);

        }
    }


    void Open()
    {
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);




        //c'est la que jai fait de la merde le chatIsask et passe automatiquemeent a true et passe plus dans la condition 
        if (!canvasIsActive && !ChatIsAsk)
        {

            List<Message> messages = ChatManager.GetChatManager.MessageList;
            int i = messages.Count;

            if (i != 0)
            {

                if (globalCanalIsActived && messages[i - 1].messageType == Message.MessageType.global ||
                    localCanalIsActived && messages[i - 1].messageType == Message.MessageType.tell ||
                    guildCanalIsActived && messages[i - 1].messageType == Message.MessageType.guild ||
                    factionCanalIsActived && messages[i - 1].messageType == Message.MessageType.faction ||
                   privateCanalIsActived && messages[i - 1].messageType == Message.MessageType.privateMessage
                   || messages[i - 1].messageType == Message.MessageType.info)
                {

                    notLaunch = false;
                }
                else
                {

                    notLaunch = true;
                }
            }

        }
        if (!ChatIsAsk)
        {
            ChatIsAsk = true;

        }
        if (!notLaunch)
        {
            StartCoroutine(Wait());
        }

        globalButton.SetActive(false);
        localButton.SetActive(false);
        guildButton.SetActive(false);
        factionButton.SetActive(false);
        channel.SetActive(false);
        displayEmoji.SetActive(true);
        activeButton.SetActive(true);
        chanelIsDesable = true;

    }

    void Close()
    {
        chatBox.text = "";
        ChatIsAsk = false;
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        canvas.SetActive(false);
        canvasIsActive = false;
        emojiIsDisplay = false;
        emoji.SetActive(false);
        activeCanal = true;
      
    }

    void RoolTheDice(int dice, int nbDice)
    {
        if (nbDice == 1)
        {
            int roolDice;

            roolDice = Random.Range(1, dice + 1);

            chatBox.text = "";
            ChatManager.GetChatManager.SendMessageToTell("<i>" + PlayerManager.GetPlayerManager.Player.PlayerName + " lance les dés et obtient : " + roolDice + "/" + dice + "</i>", Message.MessageType.info);
        }
        else
        {
            int[] repeatRoolDice = new int[nbDice];
            int resultCumulDice = 0;
            string tempRollDice = "";
            for (int i = 0; i < nbDice; i++)
            {
                repeatRoolDice[i] = Random.Range(1, dice + 1);
                tempRollDice += " " + repeatRoolDice[i] + "/" + dice;
                resultCumulDice += repeatRoolDice[i];
            }
            chatBox.text = "";
            ChatManager.GetChatManager.SendMessageToTell("<i>" + PlayerManager.GetPlayerManager.Player.PlayerName + " lance les dés et obtient : " + tempRollDice + " pour un total de: " + resultCumulDice + "/" + nbDice * dice + "</i>", Message.MessageType.info);
        }

    }
    void ActiveField()
    {


        chatBox.ActivateInputField();
        if (ChatManager.GetChatManager.actualMessageType == ChatManager.MessageType.privateMsg)
        {
            textColorField.color = privateMessage;
            infoText.color = privateMessage;
            infoText.text = "/w";
        }
        //say
        else if (ChatManager.GetChatManager.actualMessageType == ChatManager.MessageType.tell)
        {
            textColorField.color = tell;
            infoText.color = tell;
            infoText.text = "/d";
        }
        //guild
        else if (ChatManager.GetChatManager.actualMessageType == ChatManager.MessageType.guild)
        {
            textColorField.color = guild;
            infoText.color = guild;
            infoText.text = "/g";
        }
        //faction
        else if (ChatManager.GetChatManager.actualMessageType == ChatManager.MessageType.faction)
        {
            textColorField.color = faction;
            infoText.color = faction;
            infoText.text = "/f";
        }
        //global
        else if (ChatManager.GetChatManager.actualMessageType == ChatManager.MessageType.global)
        {
            textColorField.color = playerMessage;
            infoText.color = playerMessage;
            infoText.text = "/all";
        }
        //info
        else
        {
            infoText.color = playerMessage;
            infoText.text = "/all";
        }

    }

    void UpdateHUD()
    {
        List<Message> messages = ChatManager.GetChatManager.MessageList;
        int messageCount = messages.Count;

        GameObject newText;
        newText = Instantiate(textObject, chatPanel.transform);

        TextMeshProUGUI text = newText.GetComponent<TextMeshProUGUI>();
        text.text = messages[messageCount - 1].text;
        text.color = MessageTypeColor(messages[messageCount - 1].messageType);

        textObjects.Add(newText);

        int countObject = textObjects.Count;

        if (countObject > maxMessageInChat)
        {
            for (int i = 0; i < countObject - maxMessageInChat; i++)
            {
                Destroy(textObjects[0], 0.01f);
                textObjects.RemoveAt(0);
            }
        }
        UpdateActiveMessage();
    }

    void UpdateActiveMessage()
    {

        List<Message> messages = ChatManager.GetChatManager.MessageList;
        for (int i = 0; i < textObjects.Count; i++)
        {
            if (globalCanalIsActived && messages[i].messageType == Message.MessageType.global ||
                localCanalIsActived && messages[i].messageType == Message.MessageType.tell ||
                guildCanalIsActived && messages[i].messageType == Message.MessageType.guild ||
                factionCanalIsActived && messages[i].messageType == Message.MessageType.faction ||
               privateCanalIsActived && messages[i].messageType == Message.MessageType.privateMessage ||
              messages[i].messageType == Message.MessageType.info)
            {
                textObjects[i].SetActive(true);
            }
            else
            {
                textObjects[i].SetActive(false);
            }
        }

    }

    Color MessageTypeColor(Message.MessageType messageType)
    {
        Color color = info;

        switch (messageType)
        {
            case Message.MessageType.global:
                color = playerMessage;
                break;
            case Message.MessageType.info:
                color = info;
                break;
            case Message.MessageType.privateMessage:
                color = privateMessage;
                break;
            case Message.MessageType.guild:
                color = guild;
                break;
            case Message.MessageType.tell:
                color = tell;
                break;
            case Message.MessageType.faction:
                color = faction;
                break;
            case Message.MessageType.jdr:
                color = info;
                break;
            default:
                break;
        }

        return color;
    }
    bool isValidForRollTheDice(char caract, bool use0 = true)
    {
        int caratParse = int.Parse(caract.ToString());
        for (int i = use0 ? 0 : 1; i < 10; i++)
        {

            if (caratParse == i)
            {

                return true;
            }
        }

        return false;
    }

   

    // Update is called once per frame
    void Update()
    {
        

        
          
        

        //private
        if (chatBox.text.Length > 3 && chatBox.text[0] == '/' && chatBox.text[1] == 'w' && chatBox.text[2] == ' ' && chatBox.text[chatBox.text.Length - 1] == ' ')
        {
            string tempPseudo = "";
            for (int i = 3; i < chatBox.text.Length - 1; i++)
            {
                tempPseudo += chatBox.text[i];
            }
            ChatManager.GetChatManager.actualMessageType = ChatManager.MessageType.privateMsg;
            ChatManager.GetChatManager.targetMsg = tempPseudo;

            textColorField.color = privateMessage;
            infoText.color = privateMessage;
            infoText.text = "/w";
            chatBox.text = "";
        }
        //say
        else if (chatBox.text == "/d ")
        {
            LocalMsg();
        }
        //guild
        else if (chatBox.text == "/g ")
        {
            GuildMsg();
        }
        //faction
        else if (chatBox.text == "/f ")
        {
            FactionMsg();
        }
        //global
        else if (chatBox.text == "/all ")
        {
            GlobalMsg();
        }
        //rool the dice

        //else if (chatBox.text == ";)")
        //{
        //    chatBox.text = "<sprite=5>";
        //    chatBox.MoveTextEnd(false);
        //}

        //answer
        else if (chatBox.text == "/r ")
        {
            if (ChatManager.GetChatManager.WhisperPlayers.Count != 0)
            {
                ChatManager.GetChatManager.actualMessageType = ChatManager.MessageType.privateMsg;
                ChatManager.GetChatManager.SetTargetLastPersonWhoWhisperedToMe();
            }
            chatBox.text = "";
            textColorField.color = privateMessage;
            infoText.color = privateMessage;
            infoText.text = "/r";
        }
        if (chatBox.text != "")
        {



            if (InputManager.GetInputManager.GetKeyDown(KeyCode.Return, true) || InputManager.GetInputManager.GetKeyDown(KeyCode.KeypadEnter, true))
            {

                ActiveField();

                if (chatBox.text[0] == '/')
                {
                    if (chatBox.text == "/help")
                    {
                        HelpCommand();

                    }
                    if( chatBox.text.Length > 3 && chatBox.text.Length < 6 && chatBox.text[2] == 'd' && isValidForRollTheDice(chatBox.text[1], false)  )
                    {
                        int i = int.Parse(chatBox.text[1].ToString());
                        if (chatBox.text[3] == '4')
                        {
                            RoolTheDice(4, i);
                        }
                        else if (chatBox.text[3] == '6')
                        {
                            RoolTheDice(6, i);
                        }
                        else if (chatBox.text[3] == '8')
                        {
                            RoolTheDice(8, i);
                        }
                        else if (chatBox.text[3] == '1' && chatBox.text[4] == '0' && chatBox.text.Length == 6 && chatBox.text[5] == '0')
                        {
                            RoolTheDice(100, i);
                        }
                        else if (chatBox.text[3] == '1' && chatBox.text[4] == '0')
                        {
                            RoolTheDice(10, i);
                        }
                        else if (chatBox.text[3] == '1' && chatBox.text[4] == '2')
                        {
                            RoolTheDice(12, i);
                        }
                        else if (chatBox.text[3] == '2' && chatBox.text[4] == '0')
                        {
                            RoolTheDice(20, i);
                        }
                       


                    }
                    else if (chatBox.text.Length > 3  && chatBox.text.Length < 8 && chatBox.text[3] == 'd' && isValidForRollTheDice(chatBox.text[1], false)
                         && isValidForRollTheDice(chatBox.text[2], true))
                    {
                        int i = int.Parse(chatBox.text[1].ToString() + chatBox.text[2].ToString());
                        if (chatBox.text[4] == '4')
                        {
                            RoolTheDice(4, i);
                        }
                        else if (chatBox.text[4] == '6')
                        {
                            RoolTheDice(6, i);
                        }
                        else if (chatBox.text[4] == '8')
                        {
                            RoolTheDice(8, i);
                        }
                        else if (chatBox.text[4] == '1' && chatBox.text[5] == '0' && chatBox.text.Length == 7 && chatBox.text[6] == '0')
                        {
                            RoolTheDice(100, i);
                        }
                        else if (chatBox.text[4] == '1' && chatBox.text[5] == '0')
                        {
                            RoolTheDice(10, i);
                        }
                        else if (chatBox.text[4] == '1' && chatBox.text[5] == '2')
                        {
                            RoolTheDice(12, i);
                        }
                        else if (chatBox.text[4] == '2' && chatBox.text[5] == '0')
                        {
                            RoolTheDice(20, i);
                        }
                    }
               
                    else
                    {
                        ChatManager.GetChatManager.SendInfoMsg("Commande inconnue.", Message.MessageType.info);
                        ChatManager.GetChatManager.SendInfoMsg("/help pour savoir la liste des commandes.", Message.MessageType.info);
                    }
                }
                else
                {
                    switch (ChatManager.GetChatManager.actualMessageType)
                    {
                        case ChatManager.MessageType.global:
                            ChatManager.GetChatManager.SendMessageToAll(PlayerManager.GetPlayerManager.Player.PlayerName + ": " + chatBox.text, Message.MessageType.global);
                            break;
                        case ChatManager.MessageType.privateMsg:
                            ChatManager.GetChatManager.SendPrivateMessage(chatBox.text, Message.MessageType.privateMessage);
                            break;
                        case ChatManager.MessageType.guild:

                            ChatManager.GetChatManager.SendMessageToGuild(PlayerManager.GetPlayerManager.Player.PlayerName + ": " + chatBox.text, Message.MessageType.guild);
                            break;
                        case ChatManager.MessageType.tell:

                            ChatManager.GetChatManager.SendMessageToTell(PlayerManager.GetPlayerManager.Player.PlayerName + ": " + chatBox.text, Message.MessageType.tell);
                            break;
                        case ChatManager.MessageType.faction:

                            ChatManager.GetChatManager.SendMessageToFaction(PlayerManager.GetPlayerManager.Player.PlayerName + ": " + chatBox.text, Message.MessageType.faction);
                            break;
                        default:
                            break;
                    }
                }
                chatBox.text = "";


            }
        }

        else
        {

            if (InputManager.GetInputManager.GetKeyDown(KeyCode.Return) || InputManager.GetInputManager.GetKeyDown(KeyCode.KeypadEnter))
            {

                if (!ChatManager.chatIsFocused)
                {
                   
                    Open();
                    ChatManager.chatIsFocused = true;
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.ActivateInputField();

                }

            }
            
        }
       
        ChatManager.chatIsFocused = chatBox.isFocused;
    }
    //pour activer desactiver les canaux de chat(bouton a droite)
    public void DesabableChannel()
    {
        globalButton.SetActive(false);
        localButton.SetActive(false);
        guildButton.SetActive(false);
        factionButton.SetActive(false);

        chanelIsDesable = true;
    }
    //pour afficher desafficher les canaux (bouton en haut a gauche)
    public void DesactiveChannel()
    {
        if (!activeCanal)
        {
            channel.SetActive(false);
            TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
            chatBox.ActivateInputField();

            activeCanal = true;
        }
        else
        {
            channel.SetActive(true);
            TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
            chatBox.ActivateInputField();

            activeCanal = false;
        }
    }
    public void GlobalMsg()
    {
        ChatManager.GetChatManager.actualMessageType = ChatManager.MessageType.global;
        chatBox.text = "";
        textColorField.color = playerMessage;
        infoText.color = playerMessage;
        infoText.text = "/all";
        ChatManager.chatIsactived = true;

        DesabableChannel();
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        chatBox.ActivateInputField();

    }
    public void LocalMsg()
    {
        ChatManager.GetChatManager.actualMessageType = ChatManager.MessageType.tell;
        chatBox.text = "";
        infoText.color = tell;
        infoText.text = "/d";
        textColorField.color = tell;
        ChatManager.chatIsactived = true;

        DesabableChannel();
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        chatBox.ActivateInputField();


    }
    public void GuildMsg()
    {
        ChatManager.GetChatManager.actualMessageType = ChatManager.MessageType.guild;
        chatBox.text = "";
        infoText.color = guild;
        infoText.text = "/g";
        textColorField.color = guild;
        ChatManager.chatIsactived = true;

        DesabableChannel();
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        chatBox.ActivateInputField();

    }

    public void FactionMsg()
    {
        ChatManager.GetChatManager.actualMessageType = ChatManager.MessageType.faction;
        chatBox.text = "";
        infoText.color = faction;
        infoText.text = "/f";
        textColorField.color = faction;
        ChatManager.chatIsactived = true;

        DesabableChannel();
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        chatBox.ActivateInputField();

    }

    public void HelpCommand()
    {
        ChatManager.GetChatManager.SendInfoMsg("Tapez /d pour parler aux joueurs aux alentours", Message.MessageType.info);
        ChatManager.GetChatManager.SendInfoMsg("Tapez /all pour parler à tous le monde", Message.MessageType.info);
        ChatManager.GetChatManager.SendInfoMsg("Tapez /g pour parler à votre guilde", Message.MessageType.info);
        ChatManager.GetChatManager.SendInfoMsg("Tapez /f pour parler à votre faction", Message.MessageType.info);
        ChatManager.GetChatManager.SendInfoMsg("Tapez /w Pseudo pour envoyer un message privé", Message.MessageType.info);
        ChatManager.GetChatManager.SendInfoMsg("Tapez /r pour répondre à un message privé", Message.MessageType.info);
        ChatManager.GetChatManager.SendInfoMsg("Tapez /(le nombre de dés)d(4,6,8,10,12,20,100) pour lancer les dés", Message.MessageType.info);
        ChatManager.chatIsactived = true;
    }

    public void GlobalActiveMsg()
    {
        if (globalCanalIsActived)
        {

            globalButtonActive.GetComponent<Image>().color = Color.gray;

            global.GetComponent<Image>().sprite = decocher;


            globalCanalIsActived = false;
        }
        else
        {
            globalButtonActive.GetComponent<Image>().color = playerMessage;

            global.GetComponent<Image>().sprite = cocher;
            globalCanalIsActived = true;
        }
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        chatBox.ActivateInputField();
        UpdateActiveMessage();
        ChatManager.chatIsactived = true;
    }
    public void LocalActiveMsg()
    {
        if (localCanalIsActived)
        {
            localButtonActive.GetComponent<Image>().color = Color.gray;
            local.GetComponent<Image>().sprite = decocher;
            localCanalIsActived = false;
        }
        else
        {
            localButtonActive.GetComponent<Image>().color = tell;
            local.GetComponent<Image>().sprite = cocher;
            localCanalIsActived = true;
        }
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        chatBox.ActivateInputField();
        UpdateActiveMessage();
        ChatManager.chatIsactived = true;
    }
    public void GuildActiveMsg()
    {
        if (guildCanalIsActived)
        {
            guildButtonActive.GetComponent<Image>().color = Color.gray;
            guilde.GetComponent<Image>().sprite = decocher;
            guildCanalIsActived = false;
        }
        else
        {
            guildButtonActive.GetComponent<Image>().color = guild;
            guilde.GetComponent<Image>().sprite = cocher;
            guildCanalIsActived = true;
        }
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        chatBox.ActivateInputField();
        UpdateActiveMessage();
        ChatManager.chatIsactived = true;

    }
    public void FactionActiveMsg()
    {
        if (factionCanalIsActived)
        {
            factionButtonActive.GetComponent<Image>().color = Color.gray;
            factionn.GetComponent<Image>().sprite = decocher;
            factionCanalIsActived = false;
        }
        else
        {
            factionButtonActive.GetComponent<Image>().color = faction;
            factionn.GetComponent<Image>().sprite = cocher;
            factionCanalIsActived = true;
        }
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        chatBox.ActivateInputField();
        UpdateActiveMessage();
        ChatManager.chatIsactived = true;
    }
    public void PrivateActiveMsg()
    {
        if (privateCanalIsActived)
        {
            privateButtonActive.GetComponent<Image>().color = Color.gray;
            privatee.GetComponent<Image>().sprite = decocher;
            privateCanalIsActived = false;
        }
        else
        {
            privateButtonActive.GetComponent<Image>().color = privateMessage;
            privatee.GetComponent<Image>().sprite = cocher;
            privateCanalIsActived = true;
        }
        TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
        chatBox.ActivateInputField();
        UpdateActiveMessage();
        ChatManager.chatIsactived = true;
    }

    public void DisplayChannel()
    {
        if (!ChatIsAsk)
        {
            chatBox.ActivateInputField();
        }

        if (chanelIsDesable)
        {
            globalButton.SetActive(true);
            localButton.SetActive(true);
            guildButton.SetActive(true);
            factionButton.SetActive(true);

            chanelIsDesable = false;
            TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
            chatBox.ActivateInputField();
        }
        else
        {
            globalButton.SetActive(false);
            localButton.SetActive(false);
            guildButton.SetActive(false);
            factionButton.SetActive(false);

            chanelIsDesable = true;
            TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
            chatBox.ActivateInputField();
        }
    }

    public void DisplayEmote()
    {
        if (!emojiIsDisplay)
        {
            emoji.SetActive(true);
            emojiIsDisplay = true;
            TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
            chatBox.ActivateInputField();
        }
        else
        {
            emoji.SetActive(false);
            emojiIsDisplay = false;
            TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
            chatBox.ActivateInputField();
        }

    }
    public void InputEmojiInInputField(int sprite)
    {
        if (emojiIsDisplay)
        {

            chatBox.ActivateInputField();
            switch (sprite)
            {
                case 0:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=9>";

                    break;
                case 1:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=10>";
                    break;
                case 2:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=11>";
                    break;
                case 3:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=12>";
                    break;
                case 4:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=13>";
                    break;
                case 5:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=14>";
                    break;
                case 6:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=5>";
                    break;
                case 7:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=6>";
                    break;
                case 8:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=7>";
                    break;
                case 9:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=8>";
                    break;
                case 10:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=0>";
                    break;
                case 11:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=1>";
                    break;
                case 12:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=2>";
                    break;
                case 13:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=3>";
                    break;
                case 14:
                    TextHandler.transform.position = new Vector2(TextHandler.transform.position.x, 0f);
                    chatBox.text += "<sprite=4>";
                    break;
                default:
                    break;
            }

            chatBox.MoveTextEnd(false);
        }
    }

    private void LateUpdate()
    {
        if (InputManager.GetInputManager.GetKeyDown(KeyCode.Return, false) || InputManager.GetInputManager.GetKeyDown(KeyCode.KeypadEnter, false))
        {
            //Debug.Log("LateUpdate");
            notLaunch = false;
        }
    }

}
