using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ItemSlotType
{
    None,
    Inventory,
    Compound
}

public class ItemSlot : MonoBehaviour
{
    public ItemSlotType type;
    public int index;
    UISprite outline;
    public UISprite itemSprite;
    public ItemInfo info;

    void Awake()
    {
        foreach (UISprite sprite in GetComponentsInChildren<UISprite>())
        {
            if (sprite.name == "Outline")
            {
                outline = sprite;
            }
            else if (sprite.name == "ItemSprite")
            {
                itemSprite = sprite;
            }
        }
        HideOutline();
    }

    void GetItemSprite()
    {
        foreach (UISprite sprite in GetComponentsInChildren<UISprite>())
        {
            if (sprite.name == "ItemSprite")
            {
                itemSprite = sprite;
            }
        }

        if (itemSprite == null)
        {
            Debug.LogError("ItemSlot: Cannot find ItemSprite");
        }
    }

    void GetOutline()
    {
        foreach (UISprite sprite in GetComponentsInChildren<UISprite>())
        {
            if (sprite.name == "Outline")
            {
                outline = sprite;
            }
        }

        if (outline == null)
        {
            Debug.LogError("ItemSlot: Cannot find Outline");
        }
    }

    public void OnClickItemSlot()
    {
        Inventory.Instance.SelectItem(this);
    }

    public void ShowOutline()
    {
        if (outline == null)
        {
            GetOutline();
        }
        outline.enabled = true;
    }

    public void HideOutline()
    {
        if (outline == null)
        {
            GetOutline();
        }
        outline.enabled = false;
    }

    public void SetSprite(string spriteName)
    {
        if (itemSprite == null)
        {
            GetItemSprite();
        }
        itemSprite.spriteName = spriteName;
        if (spriteName == "dot")
        {
            itemSprite.alpha = 0;
        }
        else
        {
            itemSprite.alpha = 1;
        }
    }

    public void SetInfo(ItemInfo itemInfo)
    {
        info = itemInfo;
    }
}
