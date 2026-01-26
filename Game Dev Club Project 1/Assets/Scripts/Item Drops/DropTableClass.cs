using System;
using UnityEngine;

[Serializable]
public class DropTableClass 
{
    [Header("Organization")]
    public string itemName;

    [Header("Drops")]
    public ItemClass itemDropped;
    public int minQuantity = 1;
    public int maxQuantity = 1;
    [Range(1, 100)]
    public int dropChance;
}
