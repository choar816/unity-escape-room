using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public ItemInfo equipedItem = new ItemInfo(); // 현재 장착 중인 아이템 정보

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    /// <summary>
    /// 플레이 정보 - 저장이 필요한 정보들 모음
    /// </summary>
    public class PlayInfo
    {
        public int drawValue = 3;
        public ItemInfo equipedItem = new ItemInfo(); // 현재 장착 중인 아이템 정보
        public Language language = Language.Kr;
        

        //TODO 인벤토리도 여기에 포함하여 한번에 저장하도록 
    }

    public PlayInfo playInfo = new PlayInfo();

    public FixedValue fixedValue = new FixedValue();

    /// <summary>
    /// 고정 값
    /// </summary>
    public class FixedValue
    {
        public float interactiveDistance = 10.0f;
        public int drawValueMax = 3;
    }

    public enum Color
    {
        red,
        yellow,
        gray,
        green
    }

    public static void ShowLog(string text, Color color = default(Color))
    {
        string _color = "white";
        switch (color)
        {
            case Color.red:
                _color = "red";
                break;
            case Color.yellow:
                _color = "yellow";
                break;
            case Color.gray:
                _color = "gray";
                break;
            case Color.green:
                _color = "green";
                break;
        }
        Debug.Log($"<color={_color}>Show Message : {text} </color>");
    }
}
