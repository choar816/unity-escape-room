using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 오브젝트 분류
/// </summary>
public enum ObjectSpecify
{
    UnEquiped,
    Item,
    InteractiveDevice,
    InteractivePuzzle,
}

public enum EnumItem
{
    None,
    Item_Key,
    Item_Hammer,
    Item_Lighter,
    Hat,
    Clock_Incomplete,
    Clock_Complete,
    Clock_Part_1,
    Clock_Part_2,
    Clock_Part_3,
    Axe,
    Bag,
    Belt,
    Book,
    Bottle,
    Bracelet,
    Bucket,
    Camera,
    Cellphone,
    Chair,
    Computer,
    Drill,
    Eyeglasses,
    Fuse,
    Hamburger,
    Hammer,
    Helmet,
    Key,
    Ladder,
    Letter,
    Lightbulb,
    Lighter,
    Line,
    Map,
    Microphone,
    Paperclip,
    Pliers,
    Purse,
    Rifle,
    Saw,
    Scissors,
    Screwdriver,
    Shovel,
    Spoon,
    Sword,
    Telephone,
    Vase,
    Winebottle,
    Zigzag,
}
//public enum EnumItem
//{
//    None,
//    Key,
//    Lightbulb,
//    Zigzag,
//}

/// <summary>
/// 상호작용 가능한 디바이스(인벤토리에 삽입 불가)
/// </summary>
public enum InteractiveDeviceSpecify
{
    Door,
    Sorcket,
    SwitchButton,
    Locks,
    Chain,
    Button,
    LightBox,
    Pipe,
}

public enum ItemTag
{
    Break,
    BreakGun,
    BreakAxe,
    BreakHammer,
    Drill,
    Lock,
    LockKey,
    LockClip,
    Cut,
    CutScissors,
    CutPliers,
    Storage,
    StorageSolid,
    StorageFluid,
    Melt,
    Loosen,
    ExtinguishFire,
    ChargePhone,
    Hungry,
    SupplyPower,
    Read,
    TurnOnLight,
    TurnOnFire,
    Message,
}

public enum PuzzleSpecify
{
    NineWaySocket,
}

public class InteractiveAction : MonoBehaviour
{
    [Serializable]
    public class Hint
    {
        public EnumItem uuid;
        public string msg;

    }
    /// <summary>
    /// 장치 전용
    /// </summary>
    public virtual void Interact()
    {
        Debug.Log("<color=yellow>Interact</color>");
    }

    /// <summary>
    /// 아이템 전용
    /// </summary>
    public virtual void PickUp()
    {
        Debug.Log("<color=yellow>Pick up</color>");
    }

    /// <summary>
    /// 퍼즐용
    /// </summary>
    public virtual void Control()
    {
        Debug.Log("<color=yellow>Control</color>");
    }
}

