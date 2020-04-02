using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToMenu : MonoBehaviour
{
    [SerializeField] Text text;

    [SerializeField] Canvas menu;

    [SerializeField] Canvas touchPress;

    [SerializeField] GameObject vizu;
    float timer;
    bool isConnection;
    // Use this for initialization
    void Start()
    {
        timer = 0.0f;
        isConnection = false;
        if(GameManager.GetGameManager.HasAlreadySeeMenu)
        {
            OnReturnMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.GetInputManager.GetKeyDown(KeyCode.Escape, true) && isConnection)
        {
            Debug.Log("nop");
            menu.gameObject.SetActive(false);
            vizu.SetActive(false);
            touchPress.transform.GetChild(0).gameObject.SetActive(true);
            isConnection = false;
        }
        if ((InputManager.GetInputManager.GetKeyDown(KeyCode.Return, true) || InputManager.GetInputManager.GetKeyDown(KeyCode.KeypadEnter,  true)) && !isConnection)
        {
            //TransitionsManager.GetTransitionsManager.startTransition(1);
            menu.gameObject.SetActive(true);
            vizu.SetActive(true);
            touchPress.transform.GetChild(0).gameObject.SetActive(false);
            isConnection = true;
            GameManager.GetGameManager.HasAlreadySeeMenu = true;
        }
        if (!isConnection)
        {
            timer += Time.deltaTime;
            Color color = text.color;
            color.a = Mathf.Abs(Mathf.Cos(timer));
            text.color = color;
        }

    }

    void OnReturnMenu()
    {
        menu.gameObject.SetActive(true);
        vizu.SetActive(true);
        touchPress.transform.GetChild(0).gameObject.SetActive(false);
        isConnection = true;
    }
}
