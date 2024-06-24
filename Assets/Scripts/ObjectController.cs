using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public enum ButtonType
    {
        None,
        LockButton,
        MoveButton,
        RotateButton,
        OkButton,
        CancelButton,
        RemoveButton,
        ResizeButton,
    }

    Transform objectSettingTransform;
    BoxCollider2D objectCollider;
    [SerializeField] Camera playerCamera;
    bool canObjectMove = false;
    ButtonType activatedButtonType = ButtonType.None;
    public Dictionary<ButtonType, ObjectSettingButton> settingButtons;
    Vector3 lastPosition;

    //private Vector2 initialDirection; // 핸들을 처음 잡았을 때의 방향 벡터
    private bool isDraggingHandle = false; // 회전 핸들을 드래그 중인지 여부를 확인하기 위한 변수
    //[SerializeField] private GameObject rotationHandlePrefab; // 회전 핸들 프리팹 참조
    //private GameObject rotationHandleInstance; // 생성된 회전 핸들 인스턴스 참조
    private Vector3 lastDragPosition;


    void Awake()
    {
        objectCollider = GetComponent<BoxCollider2D>();
        objectSettingTransform = transform.Find("ObjectSetting");
        HideObjectSetting();
        settingButtons = new Dictionary<ButtonType, ObjectSettingButton>();

        UIEventListener.Get(gameObject).onClick += OnClickHandler;
    }

    void Update()
    {
        //if (isDraggingHandle)
        //{
        //    Vector2 currentDirection = (Vector2)playerCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        //    float rotationDelta = Vector2.SignedAngle(initialDirection, currentDirection);
        //    transform.Rotate(0, 0, rotationDelta);
        //    initialDirection = currentDirection;
        //}
    }

    void OnDragHandler(GameObject go, Vector2 delta)
    {
        Vector3 cursorPosition = Input.mousePosition;
        cursorPosition.z = 0;
        Vector3 worldPosition = playerCamera.ScreenToWorldPoint(cursorPosition);
        transform.position = worldPosition;
    }

    void OnClickHandler(GameObject go)
    {
        if (!objectSettingTransform.gameObject.activeInHierarchy)
        {
            ShowObjectSetting();
        }
    }

    public void OnClickSettingButton(ButtonType type)
    {
        if (activatedButtonType == type)
            return;

        foreach (var kvp in settingButtons)
        {
            kvp.Value.GetComponent<SpriteRenderer>().color = Color.white;
        }
        settingButtons[type].GetComponent<SpriteRenderer>().color = Color.blue;

        ResetUIEvents();

        switch (type)
        {
            case ButtonType.MoveButton:
                activatedButtonType = ButtonType.MoveButton;
                UIEventListener.Get(gameObject).onDrag += OnDragHandler;
                //objectCollider.isTrigger = true;
                break;
            case ButtonType.RotateButton:
                activatedButtonType = ButtonType.RotateButton;
                UIEventListener.Get(gameObject).onPress += HandleRotatePress;
                UIEventListener.Get(gameObject).onDrag += HandleRotateDrag;
                break;
            case ButtonType.OkButton:
                activatedButtonType = ButtonType.OkButton;
                //objectCollider.isTrigger = false;
                ResetUIEvents();
                HideObjectSetting();
                break;
                // 다른 케이스 구문들은 필요에 따라 여기에 추가하실 수 있습니다.
        }
    }
    public void ResetUIEvents()
    {
        // 모든 이벤트 리스너를 제거합니다.
        UIEventListener listener = UIEventListener.Get(gameObject);
        listener.onClick -= OnClickHandler;
        listener.onDrag -= OnDragHandler;
        listener.onPress -= HandleRotatePress;
        listener.onDrag -= HandleRotateDrag;
        // 필요하다면 여기에 추가 이벤트 리스너 제거 코드를 추가할 수 있습니다.
    }

    void HandleRotatePress(GameObject go, bool isPressed)
    {
        isDraggingHandle = isPressed;
        if (isDraggingHandle)
        {
            lastDragPosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void HandleRotateDrag(GameObject go, Vector2 delta)
    {
        if (!isDraggingHandle) return;

        Vector3 currentMousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 fromVector = lastDragPosition - this.transform.position;
        Vector3 toVector = currentMousePosition - this.transform.position;
        float angle = Vector3.SignedAngle(fromVector, toVector, Vector3.forward);
        this.transform.Rotate(Vector3.forward, angle);
        lastDragPosition = currentMousePosition;
    }


    public void ShowObjectSetting()
    {
        foreach (var kvp in settingButtons)
        {
            kvp.Value.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (activatedButtonType == ButtonType.OkButton)
        {
            activatedButtonType = ButtonType.None;
        }
        if (activatedButtonType != ButtonType.None)
        {
            settingButtons[activatedButtonType].GetComponent<SpriteRenderer>().color = Color.blue;
        }
        objectSettingTransform.gameObject.SetActive(true);
    }

    public void HideObjectSetting()
    {
        objectSettingTransform.gameObject.SetActive(false);
    }
}
