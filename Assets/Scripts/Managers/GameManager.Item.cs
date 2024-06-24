using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class GameManager : MonoBehaviour
{

    /// <summary>
    /// 아이템 속성값 지정 및 입력
    /// </summary>
    /// <param name="_uuid"></param>
    /// <returns></returns>
    public ItemInfo SetItemProperty(EnumItem _uuid)
    {
        ItemInfo _item = new ItemInfo();
        _item.specify = ObjectSpecify.Item;
        _item.uuid = _uuid;

        //switch (_uuid)
        //{
        //    case EnumItem.Axe:
        //        _item.tagList = new List<ItemTag>()
        //        {
        //            ItemTag.Break,
        //            ItemTag.BreakAxe,
        //        };
        //        break;
        //    case EnumItem.Bag:
        //        _item.tagList = new List<ItemTag>()
        //        {
        //            ItemTag.Storage,
        //            ItemTag.StorageSolid,
        //        };
        //        break;
        //    case EnumItem.Bottle:
        //        _item.tagList = new List<ItemTag>()
        //        {
        //            ItemTag.Storage,
        //            ItemTag.StorageFluid,
        //        };
        //        break;
        //    case EnumItem.Bucket:
        //        _item.tagList = new List<ItemTag>()
        //        {
        //            ItemTag.Storage,
        //        };
        //        break;
        //    default:
        //        ShowLog("What is this?");
        //        return null;
        //}
        switch (_uuid)
        {
            case EnumItem.Key:
                _item.tagList = new List<ItemTag>()
                {
                    ItemTag.LockKey,
                };
                break;
            case EnumItem.Lightbulb:
                _item.tagList = new List<ItemTag>()
                {
                    ItemTag.TurnOnLight,
                };
                break;
            case EnumItem.Zigzag:
                _item.tagList = new List<ItemTag>()
                {
                    ItemTag.LockKey,
                };
                break;
            //case EnumItem.Bucket:
            //    _item.tagList = new List<ItemTag>()
            //    {
            //        ItemTag.Storage,
            //    };
            //    break;
            default:
                ShowLog("What is this?");
                return null;
        }
        return _item;
    }

    public void MakeItem()
    {
        string itemName = RecognizeItemWithAIBarracuda.Instance.classSpecify[RecognizeItemWithAIBarracuda.Instance.prediction.predictedItdx];
        int index = Array.IndexOf(Enum.GetNames(typeof(EnumItem)), itemName);
        EnumItem enumItem = (EnumItem)Enum.Parse(typeof(EnumItem), itemName);

        float predictedValue = RecognizeItemWithAIBarracuda.Instance.prediction.predictedValue[RecognizeItemWithAIBarracuda.Instance.prediction.predictedItdx];

        // Test
        //foreach (string itemNameString in RecognizeItemWithAIBarracuda.Instance.classSpecify)
        //{
            predictedValue = RecognizeItemWithAIBarracuda.Instance.prediction.predictedValue[0];
            GameManager.ShowLog(predictedValue.ToString());
            predictedValue = RecognizeItemWithAIBarracuda.Instance.prediction.predictedValue[1];
            GameManager.ShowLog(predictedValue.ToString());
            predictedValue = RecognizeItemWithAIBarracuda.Instance.prediction.predictedValue[2];
            GameManager.ShowLog(predictedValue.ToString());
        //}
        predictedValue = RecognizeItemWithAIBarracuda.Instance.prediction.predictedValue[RecognizeItemWithAIBarracuda.Instance.prediction.predictedItdx];

        // TODO : 정확도에 따라 맞출지 아닐지 결정해야함 정확도는 기준을 정해야함 0.0~1
        switch ((EnumItem)index)
        {
            case EnumItem.Key:
                {
                    DeviceManager.Instance.AfterGettingItemFromDrawing(enumItem);
                    GameManager.ShowLog(GetName(enumItem) + " " + predictedValue);
                }
                break;
            case EnumItem.Lightbulb:
                {
                    DeviceManager.Instance.AfterGettingItemFromDrawing(enumItem);
                    GameManager.ShowLog(GetName(enumItem) + " " + predictedValue);
                }
                break;
            case EnumItem.Zigzag:
                {
                    DeviceManager.Instance.AfterGettingItemFromDrawing(enumItem);
                    GameManager.ShowLog(GetName(enumItem) + " " + predictedValue);
                }
                break;
            default:
                // TODO : 현재 활성화 된 Drawboard를 복사해서 오브젝트 형태로 띄워야 함
                {
                    DrawingManager.Instance.TurningIntoObject();
                    GameManager.ShowLog("그린 물건은 없기 때문에 오브젝트로 치환합니다.");
                }
                break;
        }
    }

    public string GetName(EnumItem _enumItemName)
    {
        switch (_enumItemName)
        {
            case EnumItem.Key:
                return GameManager.instance.ReturnTranslate(0);
            case EnumItem.Lightbulb:
                return GameManager.instance.ReturnTranslate(1);
            case EnumItem.Zigzag:
                return GameManager.instance.ReturnTranslate(2);
        }
        return ""; // 없는 것으로 "" 고정, 다른 함수에서 없다는 걸 메시지로 표시
    }

    // Scene 1에선 안쓸듯
    public string GetName(InteractiveDeviceSpecify _interactiveDevice)
    {
        switch (_interactiveDevice)
        {
            case InteractiveDeviceSpecify.Door:
                return GameManager.instance.ReturnTranslate(1);
        }
        return ""; // 없는 것으로 "" 고정, 다른 함수에서 없다는 걸 메시지로 표시
    }

    public void GetItem(ItemInfo _itemInfo)
    {
        if (_itemInfo == null)
            return;

        string itemName = GetName(_itemInfo.uuid);
        if (itemName == "")
            itemName = GameManager.instance.ReturnTranslate(3);
        else
            itemName = string.Format(GameManager.instance.ReturnTranslate(4), itemName);

        GameManager.ShowLog(itemName);

        Item t_Item = new Item();
        t_Item.uuid = _itemInfo.uuid;
        Inventory.Instance.FindItem_Popup(t_Item);

        if (playInfo.equipedItem.specify == ObjectSpecify.UnEquiped)
            playInfo.equipedItem = _itemInfo;
    }

    public void UseEquipedItem()
    {
        UnEquipedItem();
        Inventory.Instance.RemoveItem(playInfo.equipedItem);
    }

    public void UnEquipedItem()
    {
        GameManager.ShowLog("The item is removed.");
    }
}
