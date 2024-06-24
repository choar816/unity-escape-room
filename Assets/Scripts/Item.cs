using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using Sub;


[Serializable]
public class ItemInfo
{
    [Header("기본 상태 정보")]
    public bool used = false; // 사용된
    public bool switchOn = false; // 스위치
    public bool canUse; // 사용가능
    public EnumItem uuid; // 아이템 종류
    [Header("장치 정보")]
    public ObjectSpecify specify; // 종류
    public string spriteName;
    public List<ItemTag> tagList; // 태그 종류    

    [Header("이미지 정보")]
    public UISprite sprite; //NGUI 이미지
    [Header("텍스트 정보")]
    public string nameText;
    public string infoText;
    public bool canCompound; // 조합가능

    public ItemInfo(EnumItem _uuid,
                    UISprite _sprite,
                    string _atlas,
                    string _spriteName,
                    string _nameText,
                    string _infoText)
    {
        used = false;
        switchOn = false;
        canUse = false;
        canCompound = false;
        specify = ObjectSpecify.Item;
        uuid = _uuid;
        Func.SetSprite(_sprite, _atlas, _spriteName);
    }

    public ItemInfo(EnumItem _uuid = EnumItem.None, string _spriteName = null, string _nameText = null, string _infoText = null, bool _canUse = false)
    {
        uuid = _uuid;
        spriteName = _spriteName;
        nameText = _nameText;
        infoText = _infoText;
        canUse = _canUse;
    }
}

public class Item : InteractiveAction
{
    public EnumItem uuid;
    UISprite sprite;

    void Awake()
    {
        sprite = GetComponent<UISprite>();
    }

    /// <summary>
    /// 플레이어가 아이템을 줍는 함수
    /// </summary>
    public override void PickUp()
    {
        base.PickUp();
        Inventory.Instance.FindItem_Popup(this);
    }

    public void SetSprite(string spriteName)
    {
        sprite.spriteName = spriteName;
    }
}
