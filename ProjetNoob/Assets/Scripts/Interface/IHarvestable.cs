using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

interface IHarvestable
{
    CraftResources.TypeResources ResourcesHarvest { get; }
    short Min { get; }
    short Max { get; }
    void Harvest();

}
