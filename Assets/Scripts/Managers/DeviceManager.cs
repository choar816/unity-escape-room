using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class DeviceManager : MonoBehaviour
{
    private static DeviceManager instance = null;
    public static DeviceManager Instance
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

    public Transform devicesParent;
    Dictionary<string, Device> deviceDict = new Dictionary<string, Device>(); // key: Device GameObject name
    Dictionary<string, bool> isDeviceSolved = new Dictionary<string, bool>(); // key: Device GameObject name
    Device interactableDevice;

    // temp
    [SerializeField] GameObject tutorial_room4_blackScreen;
    [SerializeField] GameObject tutorial_sphere;

    void Start()
    {
        if (null == instance)
        {
            instance = this;
        }

        foreach (Transform deviceTransform in devicesParent)
        {
            Device device = deviceTransform.GetComponent<Device>();
            if (device != null)
            {
                device.DisableInteract();
                deviceDict.Add(device.gameObject.name, device);
                isDeviceSolved.Add(device.gameObject.name, false);
            }
        }
        interactableDevice = null;
        deviceDict["Hanger"].EnableInteract();
        tutorial_sphere.SetActive(false);
    }

    void Update()
    {
        // F키를 누르면 장치와 interact
        if (Input.GetKeyDown(KeyCode.F) && interactableDevice != null)
        {
            interactableDevice.TryInteract();
        }
    }

    public bool IsDeviceSolved(string deviceName)
    {
        return isDeviceSolved[deviceName];
    }

    public void SetInteractableDevice(Device device)
    {
        if (interactableDevice != null)
        {
            interactableDevice.TurnOffHighlight();
        }
        device.TurnOnHighlight();
        interactableDevice = device;
        Player.Instance.ShowInteractionKey();
    }

    public void UnsetInteractableDevice(Device device)
    {
        if (interactableDevice == device)
        {
            device.TurnOffHighlight();
            interactableDevice = null;
            Player.Instance.HideInteractionKey();
        }
    }

    public void Interact(Device device)
    {
        switch (device.gameObject.name)
        {
            case "Hanger":
                InteractHanger();
                break;
            case "Documents":
                InteractDocuments();
                break;
            case "Lamp":
                InteractLamp();
                break;
            case "Locker":
                InteractLocker();
                break;
            case "Sphere":
                InteractSphere();
                break;
        }
    }

    void InteractHanger()
    {
        deviceDict["Hanger"].DisableInteract();
        deviceDict["Hanger"].TurnOffHighlight();
        isDeviceSolved["Hanger"] = true;
        Inventory.Instance.FindItem_Full(EnumItem.Hat, false, AfterInteractHanger);
        Player.Instance.HideInteractionKey();
        Player.Instance.DisableMove();
    }

    void AfterInteractHanger()
    {
        Player.Instance.m_Stat.isHatOn = true;
        deviceDict["Documents"].EnableInteract();
        CutSceneManager.Instance.StartCutscene(CutsceneType.Hat_AfterWearing);
    }

    void InteractDocuments()
    {
        deviceDict["Documents"].DisableInteract();
        deviceDict["Documents"].TurnOffHighlight();
        Player.Instance.HideInteractionKey();
        Player.Instance.DisableMove();
        Inventory.Instance.FindItem_Full(EnumItem.Clock_Incomplete, false, AfterInteractDocuments);
        Inventory.Instance.AcquireEnumItem(EnumItem.Clock_Part_1);
        Inventory.Instance.AcquireEnumItem(EnumItem.Clock_Part_2);
        Inventory.Instance.AcquireEnumItem(EnumItem.Clock_Part_3);
    }

    void AfterInteractDocuments()
    {
        CutSceneManager.Instance.StartCutscene(CutsceneType.Clock_FoundBroken);
        // Player.Instance.EnableMove();
    }

    public void AfterSolveDocuments()
    {
        isDeviceSolved["Documents"] = true;
        deviceDict["Lamp"].EnableInteract();
    }

    void InteractLamp()
    {
        // 전구를 착용중인 경우 (일단 hammer로 테스트)
        if (Inventory.Instance.equippedItem == EnumItem.Item_Hammer)
        {
            tutorial_room4_blackScreen.SetActive(false);
            deviceDict["Lamp"].DisableInteract();
            deviceDict["Locker"].canInteract = true;
            CutSceneManager.Instance.StartCutscene(CutsceneType.Lamp_UsedBulb);
        }
        else
        {
            Debug.Log("전구가 없는 전등 이미지 노출");
        }
    }

    void InteractLocker()
    {
        CutSceneManager.Instance.StartCutscene(CutsceneType.Locker_Key);
    }

    void InteractSphere()
    {
        UIManager.Instance.Show_DeviceUI_Sphere();
    }

    public void AfterGettingItemFromDrawing(EnumItem _uuid)
    {
        switch (_uuid)
        {
            case EnumItem.Zigzag:
                break;
            default:
                break;
        }
    }
}