using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IPAdressUI : MonoBehaviour
{
    [SerializeField] InputField input;
    [SerializeField] PersonalNetworkManager manager;

    private void Start()
    {
        manager = GameObject.Find("@OurNetworkManager").GetComponent<PersonalNetworkManager>();
        input.onValidateInput += delegate (string input, int charIndex, char addedChar) { return MyValidate(addedChar); };
        input.onValueChanged.AddListener(delegate { ChangeIPAdress(); });
    }

    private char MyValidate(char charToValidate)
    {
        char tmpChar = charToValidate;
        //check if it's not one a the following letter
        if (tmpChar != '0' && tmpChar != '1' && tmpChar != '2' && tmpChar != '3' && tmpChar != '4' && tmpChar != '5'
            && tmpChar != '6' && tmpChar != '7' && tmpChar != '8' && tmpChar != '9' && tmpChar != '.')
        {
            // ... if it is change it to an empty character.
            charToValidate = '\0';
        }
        return charToValidate;
    }

    void ChangeIPAdress()
    {
        manager.networkAddress = input.text;
    }
}
