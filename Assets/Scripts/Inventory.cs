using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    private static Inventory instance = null;
    List<ItemInfo> acquiredItemInfoList;
    List<ItemSlot> itemSlotList;
    List<ItemSlot> compoundItemSlotList;
    List<ItemSlot> selectedItemSlotList;
    List<int> selectedCompoundItemIndexList;
    int lastSelectedItemIndex;

    [Header("아이템 줍기 관련 (Popup) ======================")]
    public UISprite itemFoundPopup_Sprite;
    public UILabel itemFoundPopup_Label;
    public UIButton itemFoundPopup_GetButton;
    public UIButton itemFoundPopup_DumpButton;
    public bool canGetFoundItem;
    [Space()]
    [Space()]

    [Header("아이템 줍기 관련 (Full) ======================")]
    public UISprite itemFoundFull_Sprite;
    public UILabel itemFoundFull_Label;
    [Space()]
    [Space()]

    [Header("인벤토리 관련 ================================")]
    public Transform itemSlotContainer;
    public Transform compoundItemSlotContainer;
    public GameObject itemSlotPrefab;
    public UISprite selectedItemSprite;
    public UILabel selectedItemName;
    public UILabel selectedItemInfo;
    public UIButton itemUseButton;
    public UIButton itemCompoundTryButton;
    [Space()]
    [Space()]

    [Header("아이템 사용 관련 ================================")]
    int equippedItemIndex;
    public EnumItem equippedItem;
    public UISprite equippedItemSprite_Reality;
    public UISprite equippedItemSprite_Thinking;
    [Space()]
    [Space()]

    [Header("아이템 합성 관련 ================================")]
    public List<UISprite> compoundItemSpriteList;
    public List<EnumItem> compoundItemEnumList;
    int selectedCompoundItemCount;

    // [Header("아이템 삭제 관련 ================================")]
    // public UIButton itemRemoveTryButton;
    // public UIButton itemRemoveButton;
    // public UIButton itemRemoveCancelButton;

    [Header("기타 ================================")]
    public Dictionary<EnumItem, ItemInfo> itemData;
    Item foundItem;


    const int MAX_ITEM_COUNT = 10;

    public static Inventory Instance
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

        InitItemData();
        InitAcquiredItems();
        CreateItemSlots();

        selectedItemSlotList = new List<ItemSlot>();
        selectedItemSprite.spriteName = "dot";
        selectedItemName.text = string.Empty;
        selectedItemInfo.text = string.Empty;
        lastSelectedItemIndex = -1;
        equippedItemIndex = -1;
        equippedItem = EnumItem.None;

        compoundItemSlotList = new List<ItemSlot>();
        compoundItemEnumList = new List<EnumItem> { EnumItem.None, EnumItem.None, EnumItem.None, EnumItem.None };
        selectedCompoundItemIndexList = new List<int> { -1, -1, -1, -1 };
        selectedCompoundItemCount = 0;
    }

    public void SelectItem(ItemSlot itemSlot)
    {
        // 개선 필요
        // if (UIManager.Instance.topUiType != itemSlot.type)
        //  return;
        if (itemSlot.type == ItemSlotType.Inventory && UIManager.Instance.PeekUIStack() != UIManager.Instance.InventoryUI)
        {
            return;
        }
        else if (itemSlot.type == ItemSlotType.Compound && UIManager.Instance.PeekUIStack() != UIManager.Instance.ItemCompoundTryUI)
        {
            return;
        }

        if (UIManager.Instance.PeekUIStack() == UIManager.Instance.InventoryUI)
        {
            SelectItem_Inventory(itemSlot);
        }
        else if (UIManager.Instance.PeekUIStack() == UIManager.Instance.ItemCompoundTryUI)
        {
            SelectItem_Compound(itemSlot);
        }
    }

    /// 인벤토리 아이템 선택 함수 (단일 선택)
    /// <param name="itemSlot">선택한 아이템</param>
    public void SelectItem_Inventory(ItemSlot itemSlot)
    {
        // 선택된 아이템 초기화
        selectedItemSlotList.Clear();
        foreach (ItemSlot _itemSlot in itemSlotList)
        {
            _itemSlot.HideOutline();
            selectedItemSlotList.Remove(itemSlot);
        }
        if (itemSlot.info.uuid == EnumItem.None)
        {

            UIManager.Instance.HideItemInfoUI();
            itemUseButton.enabled = false;
            return;
        }

        // 선택한 아이템 테두리 설정, 정보 표시
        itemSlot.ShowOutline();
        selectedItemSlotList.Add(itemSlot);
        SetSelectedItemInfo(itemSlot);
        UIManager.Instance.ShowItemInfoUI();
    }

    /// 합성 아이템 선택 함수 (복수 선택)
    /// <param name="itemSlot">선택한 아이템</param>
    public void SelectItem_Compound(ItemSlot itemSlot)
    {
        // 빈 칸을 클릭하면 아무것도 하지 않음
        if (itemSlot.info.uuid == EnumItem.None)
            return;

        int _firstEmptySlotIndex = -1;
        for (int i = 0; i < selectedCompoundItemIndexList.Count; ++i)
        {
            if (_firstEmptySlotIndex == -1 && selectedCompoundItemIndexList[i] == -1)
            {
                _firstEmptySlotIndex = i;
            }
        }

        // 이미 선택된 아이템 선택
        if (selectedCompoundItemIndexList.Contains(itemSlot.index))
        {
            int _indexOfSelectedItemSprite = selectedCompoundItemIndexList.IndexOf(itemSlot.index);
            compoundItemSpriteList[_indexOfSelectedItemSprite].spriteName = "dot";
            compoundItemSpriteList[_indexOfSelectedItemSprite].color = new Color(1, 1, 1, 0);
            compoundItemEnumList[_indexOfSelectedItemSprite] = EnumItem.None;
            selectedCompoundItemIndexList[_indexOfSelectedItemSprite] = -1;
            selectedCompoundItemCount -= 1;
            itemSlot.HideOutline();
            return;
        }

        // 선택된 아이템이 4개
        if (selectedCompoundItemCount == 4)
        {
            Debug.Log("You already selected 4 compound items");
            return;
        }

        itemSlot.ShowOutline();
        compoundItemSpriteList[_firstEmptySlotIndex].spriteName = itemSlot.info.spriteName;
        compoundItemSpriteList[_firstEmptySlotIndex].color = new Color(1, 1, 1, 1);
        compoundItemEnumList[_firstEmptySlotIndex] = itemSlot.info.uuid;
        selectedCompoundItemIndexList[_firstEmptySlotIndex] = itemSlot.index;
        selectedCompoundItemCount += 1;
    }

    /// 선택된 아이템의 정보대로 상단 이미지, 설명, 버튼을 적용하는 함수
    /// <param name="itemSlot">선택한 아이템</param>
    public void SetSelectedItemInfo(ItemSlot itemSlot)
    {
        UpdateItemSlots();
        selectedItemSprite.spriteName = itemData[itemSlot.info.uuid].spriteName;
        selectedItemName.text = itemData[itemSlot.info.uuid].nameText;
        selectedItemInfo.text = itemData[itemSlot.info.uuid].infoText;
        itemUseButton.enabled = itemData[itemSlot.info.uuid].canUse;
        lastSelectedItemIndex = itemSlotList.IndexOf(itemSlot);
        if (lastSelectedItemIndex == equippedItemIndex)
        {
            UIManager.Instance.ItemUseLabel.text = "사용해제";
        }
        else
        {
            UIManager.Instance.ItemUseLabel.text = "사용하기";
        }
    }

    /// 획득한 아이템을 모두 빈 아이템으로 초기화하는 함수
    void InitAcquiredItems()
    {
        acquiredItemInfoList = new List<ItemInfo>();
        for (int i = 0; i < MAX_ITEM_COUNT; ++i)
        {
            ItemInfo itemInfo = new ItemInfo(EnumItem.None, string.Empty, "no item");
            acquiredItemInfoList.Add(itemInfo);
        }
    }

    /// 아이템 발견 함수 (팝업)
    /// 아이템 근처에서 F키를 눌렀을 때 실행됨
    public void FindItem_Popup(Item item)
    {
        SetFoundItem(item);
        itemFoundPopup_Sprite.spriteName = itemData[item.uuid].spriteName;
        itemFoundPopup_Label.text = itemData[item.uuid].infoText;

        EventDelegate ed = new EventDelegate(this, "FindItem_Acquire");
        itemFoundPopup_GetButton.onClick.Clear();
        itemFoundPopup_GetButton.onClick.Add(ed);
        UIManager.Instance.ShowItemFoundPopupUI();
    }

    /// 아이템 발견 함수 (전체화면)
    public void FindItem_Full(EnumItem enumItem, bool canGet, Action afterAction = null)
    {
        UIManager.Instance.afterAction = afterAction;
        canGetFoundItem = canGet;
        itemFoundFull_Sprite.spriteName = itemData[enumItem].spriteName;
        itemFoundFull_Label.text = itemData[enumItem].nameText;
        UIManager.Instance.ShowItemFoundFullUI();
        UIManager.Instance.isItemFoundFullUIOpen = true;
    }

    /// 아이템을 획득하는 함수
    public void FindItem_Acquire()
    {
        // 맨 처음 빈칸에 얻은 아이템 추가
        for (int i = 0; i < acquiredItemInfoList.Count; ++i)
        {
            if (acquiredItemInfoList[i].uuid == EnumItem.None)
            {
                acquiredItemInfoList[i] = itemData[foundItem.uuid];
                Destroy(foundItem.gameObject);
                return;
            }
        }
    }

    public void SetFoundItem(Item item)
    {
        foundItem = item;
    }

    /// 발견한 아이템을 줍지 않는 함수
    public void FindItem_Dump()
    {
        UIManager.Instance.HideItemFoundPopupUI();
    }

    public void AcquireEnumItem(EnumItem enumItem)
    {
        // 맨 처음 빈칸에 얻은 아이템 추가
        for (int i = 0; i < acquiredItemInfoList.Count; ++i)
        {
            if (acquiredItemInfoList[i].uuid == EnumItem.None)
            {
                acquiredItemInfoList[i] = itemData[enumItem];
                return;
            }
        }

        // 장치2
        if (enumItem == EnumItem.Clock_Complete)
        {
            DeviceManager.Instance.AfterSolveDocuments();
        }
    }

    /// 아이템 슬롯 UI를 만드는 함수
    public void CreateItemSlots()
    {
        if (itemSlotContainer.transform.childCount != 0)
            return;

        int diff = 120;

        itemSlotList = new List<ItemSlot>();
        for (int i = 0; i < MAX_ITEM_COUNT; ++i)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemSlotContainer);
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.type = ItemSlotType.Inventory;
            slot.transform.localPosition += diff * (Vector3.right * (i % (MAX_ITEM_COUNT / 2)) + Vector3.down * (i / (MAX_ITEM_COUNT / 2)));
            itemSlotList.Add(itemSlot);
        }
    }

    public void CreateCompoundItemSlots()
    {
        if (compoundItemSlotContainer.transform.childCount != 0)
            return;

        int diff = 120;

        for (int i = 0; i < MAX_ITEM_COUNT; ++i)
        {
            GameObject slot = Instantiate(itemSlotPrefab, compoundItemSlotContainer);
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.type = ItemSlotType.Compound;
            slot.transform.localPosition += diff * (Vector3.right * (i % (MAX_ITEM_COUNT / 2)) + Vector3.down * (i / (MAX_ITEM_COUNT / 2)));
            compoundItemSlotList.Add(itemSlot);

            compoundItemSlotList[i].SetInfo(itemData[acquiredItemInfoList[i].uuid]);
            compoundItemSlotList[i].SetSprite(itemData[acquiredItemInfoList[i].uuid].spriteName);
            compoundItemSlotList[i].index = i;
        }
    }

    public void CompoundItem_Try()
    {
        CreateCompoundItemSlots();
        UIManager.Instance.ShowCompoundItemUI();
    }

    public void CompoundItem()
    {
        UIManager.Instance.PopUIStack();

        if (selectedCompoundItemCount < 2)
        {
            UIManager.Instance.ShowCompoundFailureUI(CombineFailureReason.OneItem);
        }
        else
        {
            ItemCombinationTable combinationTable = new ItemCombinationTable();

            // 아이템 합성을 시도하고 결과 아이템을 가져옴
            EnumItem resultItem;
            List<EnumItem> validItems = compoundItemEnumList.Where(item => item != EnumItem.None).ToList();
            bool success = combinationTable.CanCombineItems(validItems, out resultItem);

            if (success)
            {
                UIManager.Instance.ShowCompoundSuccessUI(itemData[resultItem].spriteName, itemData[resultItem].nameText);
                foreach (int index in selectedCompoundItemIndexList)
                {
                    RemoveItem(index); // 선택한 재료 아이템 제거
                }
                AcquireEnumItem(resultItem); // 결과 아이템 획득
                // 결과 아이템 획득에 따라 함수 실행, 분리 필요할 듯..?
                if (resultItem == EnumItem.Clock_Complete)
                {
                    DeviceManager.Instance.AfterSolveDocuments();
                }
            }
            else
            {
                UIManager.Instance.ShowCompoundFailureUI(CombineFailureReason.NoCombination);
            }
        }
        Invoke("HideCompoundResultUI", 3);
    }

    void HideCompoundResultUI()
    {
        UpdateItemSlots();
        UIManager.Instance.HideCompoundResultUI();
    }

    /// 아이템 슬롯 UI가 acquiredItems의 정보를 반영하도록 업데이트하는 함수
    public void UpdateItemSlots()
    {
        for (int i = 0; i < MAX_ITEM_COUNT; ++i)
        {
            itemSlotList[i].SetInfo(itemData[acquiredItemInfoList[i].uuid]);
            itemSlotList[i].SetSprite(itemSlotList[i].info.spriteName);
        }
    }

    public void AcquireFoundItem(EnumItem _item)
    {
        UIManager.Instance.HideItemFoundPopupUI();
        // 맨 처음 빈칸에 얻은 아이템 추가
        for (int i = 0; i < acquiredItemInfoList.Count; ++i)
        {
            if (acquiredItemInfoList[i].uuid == EnumItem.None)
            {
                acquiredItemInfoList[i] = itemData[_item];
                return;
            }
        }
    }

    /// 인벤토리에서 맨 첫번째 아이템을 선택하는 함수
    /// 아이템이 없을 시 선택하지 않음
    public void SelectFirstItem()
    {
        if (acquiredItemInfoList[0].uuid == EnumItem.None)
        {
            UIManager.Instance.HideItemInfoUI();
            return;
        }
        SelectItem_Inventory(itemSlotList[0]);
    }

    /// 아이템을 삭제하는 함수
    public void RemoveLastSelectedItem()
    {
        ItemSlot itemSlot = itemSlotList[lastSelectedItemIndex];

        selectedItemSlotList.Clear();
        itemSlotList[lastSelectedItemIndex].HideOutline();

        acquiredItemInfoList[lastSelectedItemIndex] = itemData[EnumItem.None];
        SetSelectedItemInfo(itemSlot);
        UIManager.Instance.HideItemRemoveConfirmUI();
    }

    public void RemoveItem(int index)
    {
        if (index == -1)
            return;

        ItemSlot itemSlot = itemSlotList[index];

        selectedItemSlotList.Clear();
        itemSlotList[index].HideOutline();

        acquiredItemInfoList[index] = itemData[EnumItem.None];
        SetSelectedItemInfo(itemSlot);
        UIManager.Instance.HideItemRemoveConfirmUI();
    }

    public void RemoveItem(ItemInfo _item)
    {

    }

    public void RemoveItem_Try()
    {
        UIManager.Instance.ShowItemRemoveConfirmUI();
    }

    public void RemoveItem_Cancel()
    {
        UIManager.Instance.HideItemRemoveConfirmUI();
    }

    public void EquipOrUnequipItem()
    {
        if (lastSelectedItemIndex == -1)
            return;

        // 착용한 아이템인 경우 -> 착용 해제
        if (lastSelectedItemIndex == equippedItemIndex)
        {
            equippedItem = EnumItem.None;
            equippedItemIndex = -1;
            SetEquippedItemSprite(EnumItem.None);
            UIManager.Instance.ItemUseLabel.text = "사용하기";
        }
        // 아이템을 착용하지 않은 경우 -> 착용
        // 어떤 아이템을 착용하고 있는 상태, 다른 아이템 선택중 -> 선택중인 아이템 착용
        else
        {
            equippedItem = acquiredItemInfoList[lastSelectedItemIndex].uuid;
            equippedItemIndex = lastSelectedItemIndex;
            SetEquippedItemSprite(acquiredItemInfoList[lastSelectedItemIndex].uuid);
            UIManager.Instance.ItemUseLabel.text = "사용해제";
        }
    }

    void SetEquippedItemSprite(EnumItem enumItem)
    {
        if (enumItem == EnumItem.None)
        {
            equippedItemSprite_Reality.color = new Color(1, 1, 1, 0);
            equippedItemSprite_Thinking.color = new Color(1, 1, 1, 0);
        }
        else
        {
            equippedItemSprite_Reality.color = new Color(1, 1, 1, 1);
            equippedItemSprite_Thinking.color = new Color(1, 1, 1, 1);
        }
        equippedItemSprite_Reality.spriteName = itemData[enumItem].spriteName;
        equippedItemSprite_Thinking.spriteName = itemData[enumItem].spriteName;
    }

    /// 얻을 수 있는 모든 아이템의 정보를 itemData에 저장해놓는 함수
    void InitItemData()
    {
        itemData = new Dictionary<EnumItem, ItemInfo>
        {
            { EnumItem.None, new ItemInfo(EnumItem.None, "dot", string.Empty, "no item") },
            { EnumItem.Item_Key, new ItemInfo(EnumItem.Item_Key, "key", "열쇠", "this is key", true) },
            { EnumItem.Item_Hammer, new ItemInfo(EnumItem.Item_Hammer, "hammer", "망치", "hi, i'm hammer", true) },
            { EnumItem.Item_Lighter, new ItemInfo(EnumItem.Item_Lighter, "lighter", "라이터", "i'll burn everything", false) },
            { EnumItem.Hat, new ItemInfo(EnumItem.Hat, "fedora", "모자", "you can wear this", false) },
            { EnumItem.Clock_Complete, new ItemInfo(EnumItem.Clock_Complete, "item_table_clock_0", "시계 (완성)", "완성된 시계", false) },
            { EnumItem.Clock_Incomplete, new ItemInfo(EnumItem.Clock_Incomplete, "item_table_clock_2_glass", "깨진 시계", "깨진 시계입니다.", false) },
            { EnumItem.Clock_Part_1, new ItemInfo(EnumItem.Clock_Part_1, "item_table_clock_2_glass", "깨진 시계", "서류더미에서 발견한 시계 위쪽 부분입니다. 시계를 조합할 수 있습니다.", false) },
            { EnumItem.Clock_Part_2, new ItemInfo(EnumItem.Clock_Part_2, "item_table_clock_3_spring", "깨진 시계", "서류더미에서 발견한 시계 몸통 부분입니다. 시계를 조합할 수 있습니다.", false) },
            { EnumItem.Clock_Part_3, new ItemInfo(EnumItem.Clock_Part_3, "item_table_clock_1_hand", "시계 바늘", "서류더미에서 발견한 시계 바늘 부분입니다. 시계를 조합할 수 있습니다.", false) },
        };
    }
}
