using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public int itemID;
}
