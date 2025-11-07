using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;

    public string itemName;
    public string itemDescription;
    public int itemID;
    public Sprite itemSprite;

    private void Awake()
    {
        if (itemData == null)
        {
            Debug.Log("CharacterData null");
            return;
        }

        itemName = itemData.itemName;
        itemDescription = itemData.itemDescription;
        itemID = itemData.itemID;
    }
}
