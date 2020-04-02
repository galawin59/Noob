using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DropDownArchetype : MonoBehaviour
{
    [SerializeField] Dropdown dd;
    List<string> options = Enum.GetNames(typeof(Archetype)).ToList();
    // Use this for initialization
    void Start()
    {
        dd.ClearOptions();
        dd.AddOptions(options);
    }
}
