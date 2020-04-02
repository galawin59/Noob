using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance = null;
    
    //Axis
    public float GetAxis(string axisName, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return 0f;
        }
        return Input.GetAxis(axisName);
    }

    public float GetAxisRaw(string axisName, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return 0f;
        }
        return Input.GetAxisRaw(axisName);
    }

    //Button
    public bool GetButton(string buttonName, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return false;
        }
        return Input.GetButton(buttonName);
    }

    public bool GetButtonDown(string buttonName, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return false;
        }
        return Input.GetButtonDown(buttonName);
    }

    public bool GetButtonUp(string buttonName, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return false;
        }
        return Input.GetButtonUp(buttonName);
    }

    //MouseButton
    public bool GetMouseButton(int mouseButton, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return false;
        }
        return Input.GetMouseButton(mouseButton);
    }

    public bool GetMouseButtonDown(int mouseButton, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return false;
        }
        return Input.GetMouseButtonDown(mouseButton);
    }

    public bool GetMouseButtonUp(int mouseButton, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return false;
        }
        return Input.GetMouseButtonUp(mouseButton);
    }

    //Key
    /// <summary>
    /// Fortement déconseillé : la touche d'entrée de "GetKey" ne peut être modifiée par l'uttilisateur.
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="overrideChat"></param>
    /// <returns></returns>
    public bool GetKey(KeyCode keyCode, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return false;
        }
        return Input.GetKey(keyCode);
    }

    /// <summary>
    /// Fortement déconseillé : la touche d'entrée de "GetKeyDown" ne peut être modifiée par l'uttilisateur.
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="overrideChat"></param>
    /// <returns></returns>
    public bool GetKeyDown(KeyCode keyCode, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return false;
        }
        return Input.GetKeyDown(keyCode);
    }

    /// <summary>
    /// Fortement déconseillé : la touche d'entrée de "GetKeyUp" ne peut être modifiée par l'uttilisateur.
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="overrideChat"></param>
    /// <returns></returns>
    public bool GetKeyUp(KeyCode keyCode, bool overrideChat = false)
    {
        if (!overrideChat && ChatManager.chatIsFocused)
        {
            return false;
        }
        return Input.GetKeyUp(keyCode);
    }


    // Game Instance Singleton
    public static InputManager GetInputManager { get; private set; }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (GetInputManager == null)
        {
            GetInputManager = this;
        }
        else if (GetInputManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
