using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnInteract();

interface IInteractable
{
    event OnInteract onInteract;
    bool Interact();
}
