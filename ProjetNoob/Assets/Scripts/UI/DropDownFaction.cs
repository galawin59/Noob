using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class DropDownFaction : MonoBehaviour
{

    [SerializeField] Dropdown dd;
    List<string> options = Enum.GetNames(typeof(FactionName)).ToList();
    // Use this for initialization
    void Start()
    {
        dd.ClearOptions();
        dd.AddOptions(options);
    }
}
