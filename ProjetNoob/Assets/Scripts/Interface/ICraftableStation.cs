using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICraftableStation
{
    List<CraftInfo> ListOfCraft { get; }
    bool IsOpen { get; }
}
