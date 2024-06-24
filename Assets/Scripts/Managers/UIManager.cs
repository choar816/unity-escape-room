using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    public static UIManager Instance
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

    [Header("기본 UI ================================")]
    [SerializeField] GameObject TopUI;
    Stack<GameObject> UI_Stack;
    [Space()]
    [Space()]

    [Header("아이템 UI ================================")]
    public GameObject InventoryUI;
    [SerializeField] GameObject ItemInfoUI;
    public GameObject ItemFoundFullUI;
    [SerializeField] GameObject ItemFoundPopupUI;
    [SerializeField] GameObject ItemRemoveConfirmUI;
    public UILabel ItemUseLabel;
    // [SerializeField] GameObject ItemUseUI;

    List<GameObject> UIList;
    public bool isItemFoundPopupUIOpen;
    public bool isItemFoundFullUIOpen;
    bool hasInventoryOpened;
    public Action afterAction; // 아이템 획득 -> 획득 창 닫을 때 호출되는 action
    [Space()]
    [Space()]

    [Header("아이템 UI (합성) ================================")]
    public GameObject ItemCompoundTryUI;
    [SerializeField] GameObject ItemCompoundSuccessUI;
    [SerializeField] UISprite ResultItemSprite;
    [SerializeField] UILabel ResultItemName;
    [SerializeField] GameObject ItemCompoundFailureUI;
    [SerializeField] UILabel CompoundFailureLabel;
    [Space()]
    [Space()]

    [Header("생각/현실 UI ================================")]
    [SerializeField] GameObject BottomRealityUI;
    [SerializeField] GameObject BottomThoughtUI;
    public GameObject UIDrawing;
    [Space()]
    [Space()]

    [Header("장치 UI ================================")]
    [SerializeField] GameObject DeviceUI_Sphere;
    [SerializeField] GameObject DeviceUI_WordLock;


    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        hasInventoryOpened = false;
        isItemFoundPopupUIOpen = false;
        isItemFoundFullUIOpen = false;
        afterAction = null;
        UIList = new List<GameObject> { InventoryUI, ItemInfoUI, ItemFoundPopupUI, ItemRemoveConfirmUI, ItemCompoundTryUI, ItemCompoundSuccessUI, ItemCompoundFailureUI };
        foreach (GameObject UI in UIList)
        {
            UI.SetActive(true);
        }
        UI_Stack = new Stack<GameObject>();
    }

    private void Start()
    {
        foreach (GameObject UI in UIList)
        {
            UI.SetActive(false);
        }
        TopUI.SetActive(true);
        BottomRealityUI.SetActive(true);
    }

    private void Update()
    {
        if (UI_Stack.Count == 0 && Input.GetKeyDown(KeyCode.I))
        {
            ShowInventoryUI();
        }
        if (isItemFoundPopupUIOpen && Input.GetKeyDown(KeyCode.Return))
        {
            HideItemFoundPopupUI();
            Inventory.Instance.FindItem_Acquire();
        }

        // afterAction 하도록 추가
        if (isItemFoundFullUIOpen && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)))
        {
            HideItemFoundFullUI();
            if (Inventory.Instance.canGetFoundItem)
            {
                Inventory.Instance.FindItem_Acquire();
            }
            if (afterAction != null)
            {
                afterAction.Invoke();
                afterAction = null;
            }
        }

        // test
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PushUIStack(DeviceUI_Sphere);
        }
    }

    public void ChangeMode(ControlCenter.GameMode mode)
    {
        switch (mode)
        {
            case ControlCenter.GameMode.Reality:
                SetModeToReality();
                break;
            case ControlCenter.GameMode.Thought:
                SetModeToThought();
                break;
            case ControlCenter.GameMode.Thought_Draw:
                ShowDrawingUI();
                break;
        }
    }


    public void SetModeToReality()
    {
        BottomThoughtUI.SetActive(false);
        BottomRealityUI.SetActive(true);
    }

    public void SetModeToThought()
    {
        BottomRealityUI.SetActive(false);
        BottomThoughtUI.SetActive(true);
    }

    public void ShowDrawingUI()
    {
        UIDrawing.SetActive(true);
    }
    public void HideDrawingUI()
    {
        UIDrawing.SetActive(false);
    }

    void ShowBasicUI()
    {
        TopUI.SetActive(true);
        if (ControlCenter.Instance.gameInfo.mode == ControlCenter.GameMode.Reality)
        {
            BottomRealityUI.SetActive(true);
        }
        else if (ControlCenter.Instance.gameInfo.mode == ControlCenter.GameMode.Thought)
        {
            BottomThoughtUI.SetActive(true);
        }
    }

    void HideBasicUI()
    {
        TopUI.SetActive(false);
        BottomRealityUI.SetActive(false);
        BottomThoughtUI.SetActive(false);
    }

    public GameObject PeekUIStack()
    {
        if (UI_Stack.Count == 0)
            return null;

        return UI_Stack.Peek();
    }

    public void PushUIStack(GameObject ui)
    {
        UI_Stack.Push(ui);
        ui.SetActive(true);
        Player.Instance.DisableMove();

        if (ui == DeviceUI_Sphere || ui == DeviceUI_WordLock)
        {
            HideBasicUI();
        }
    }

    public void PopUIStack()
    {
        if (UI_Stack.Count == 0)
            return;

        GameObject topUI = UI_Stack.Peek();
        UI_Stack.Pop();
        topUI.SetActive(false);

        if (topUI == DeviceUI_Sphere || topUI == DeviceUI_WordLock)
        {
            ShowBasicUI();
        }

        if (UI_Stack.Count == 0)
        {
            Player.Instance.EnableMove();
        }
    }

    //public void OnClieckedThoughtBTN()
    //{
    //    switch (ControlCenter.Instance.gameInfo.mode)
    //    {
    //        case ControlCenter.GameMode.Reality:
    //            ControlCenter.Instance.ChangeMode(ControlCenter.GameMode.Thought);
    //            break;
    //        case ControlCenter.GameMode.Thought:
    //            ControlCenter.Instance.ChangeMode(ControlCenter.GameMode.Reality);
    //            break;
    //        case ControlCenter.GameMode.Thought_Draw:
    //            break;
    //    }
    //}
}
