using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IconsUI : MonoBehaviour
{
    [SerializeField]
    GameObject prefabMessage;

    [SerializeField]
    Button prefabBtn;

    [SerializeField]
    int nbButtons = 6;

    [SerializeField]
    float radius = 1.0f;

    Button[] btns;
    Vector2[] pos;
    bool enableUI = false;
    bool btnEnable = false;
    bool isLocalPlayer = false;
    Sprite[] mainMenuSprites;
    Sprite[] duelSprites;
    Sprite[] EmoticonSprites;

    PlayerController playerController;

    List<Sprite[]> btnsList;

    public enum Menu
    {
        mainMenu,
        Emoticon,
        Shifumi

    }

    Menu currentMenu;

    void Start()
    {
        isLocalPlayer = PlayerManager.GetPlayerManager.currentPlayer == transform.parent.gameObject;
        if (isLocalPlayer)
        {
            btnsList = new List<Sprite[]>();

            currentMenu = Menu.mainMenu;
            mainMenuSprites = Resources.LoadAll<Sprite>("Sprites/Icons/Main Menu");
            EmoticonSprites = Resources.LoadAll<Sprite>("Sprites/Icons/Emoticons/p1");
            duelSprites = Resources.LoadAll<Sprite>("Sprites/Icons/Duels/Shifumi");

            btnsList.Add(mainMenuSprites);
            btnsList.Add(EmoticonSprites);
            btnsList.Add(duelSprites);

            btns = new Button[nbButtons];
            pos = new Vector2[nbButtons];

            float angle = 0.0f;
            float addAngle = (Mathf.PI * 2) / nbButtons;
            for (int i = 0; i < nbButtons; i++)
            {
                Button btn = Instantiate(prefabBtn);
                btn.transform.SetParent(transform);
                //RectTransform rec = btn.GetComponent<RectTransform>();
                btn.gameObject.SetActive(enableUI);
                btn.GetComponent<Image>().color = Color.clear;

                btns[i] = btn;
                pos[i] = new Vector3(radius * Mathf.Sin(angle), radius * Mathf.Cos(angle), 0.0f);
                angle += addAngle;
            }
        }
        playerController = transform.parent.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (enableUI)
            {
                btnEnable = false;
                int i = 0;
                foreach (Button btn in btns)
                {
                    btn.GetComponent<RectTransform>().localPosition = Vector2.Lerp(btn.GetComponent<RectTransform>().localPosition, pos[i], 0.1f);
                    btn.GetComponent<Image>().color = Color.Lerp(btn.GetComponent<Image>().color, Color.white, 0.1f);
                    i++;
                }
            }
            else
            {
                if (!btnEnable)
                {
                    int i = 0;
                    foreach (Button btn in btns)
                    {
                        btn.GetComponent<RectTransform>().localPosition = Vector2.Lerp(btn.GetComponent<RectTransform>().localPosition, Vector2.zero, 0.1f);
                        btn.GetComponent<Image>().color = Color.Lerp(btn.GetComponent<Image>().color, Color.clear, 0.1f);
                        i++;
                        if (btn.GetComponent<Image>().color.a < 0.1f)
                        {
                            btn.gameObject.SetActive(false);
                            btnEnable = true;
                        }
                    }
                }
            }
        }
    }

    public void EnableIcon()
    {
        enableUI = !enableUI;
        if (enableUI)
        {
            ResetIcons((int)Menu.mainMenu);
            AddButtonsListener(Menu.mainMenu);
        }
    }

    void AddButtonsListener(Menu _index)
    {
        switch (_index)
        {
            case Menu.mainMenu:
                btns[0].onClick.AddListener(() => changeMenu(Menu.Emoticon));
                btns[1].onClick.AddListener(() => changeMenu(Menu.Shifumi));
                break;
            case Menu.Emoticon:
                for (int i = 0; i < 8; i++)
                {
                    string name = btns[i].GetComponent<Image>().sprite.name;
                    btns[i].onClick.AddListener(() => DisplayBubbleLocal(name));
                }
                break;
            case Menu.Shifumi:
                for (int i = 0; i < 3; i++)
                {
                    string name = btns[i].GetComponent<Image>().sprite.name;
                    btns[i].onClick.AddListener(() => DisplayBubbleLocal(name));
                }
                break;
        }
    }

    public void changeMenu(Menu _index)
    {
        currentMenu = _index;
        ResetIcons((int)currentMenu);
        AddButtonsListener(currentMenu);
    }

    void ResetIcons(int _index)
    {
        int i = 0;
        int length = btnsList[_index].Length;
        foreach (Button btn in btns)
        {
            if (i < length)
            {
                btn.GetComponent<Image>().sprite = btnsList[_index][i];
                btn.gameObject.SetActive(true);
            }
            else btn.gameObject.SetActive(false);
            if (enableUI)
            {
                btn.GetComponent<RectTransform>().localPosition = Vector2.zero;
                btn.GetComponent<Image>().color = Color.clear;
            }
            btn.onClick.RemoveAllListeners();
            i++;
        }
    }

    public void DisplayBubbleLocal(string _spriteName)
    {
        enableUI = false;

        playerController.CmdDisplayBubble(_spriteName, playerController.sceneId);
    }

    public void DisplayBubbleClient(string _spriteName, int _idScene)
    {
        if (_idScene == PlayerManager.GetPlayerManager.playerController.sceneId)
        {
            GameObject message = Instantiate(prefabMessage);
            message.transform.SetParent(transform);
            message.GetComponent<RectTransform>().localPosition = new Vector2(1.0f, 0.5f);

            message.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Bubles Message/" + _spriteName);
        }
    }
}
