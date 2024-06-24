using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Room : MonoBehaviour
{
    public int roonNumber;
    public Transform edgeRightTop;
    public Transform edgeLeftBottom;

    // 현재 진행 중
    public int depth = 0;
    public List<SetDeviceList> setDevicesLists = new List<SetDeviceList>();
    public List<SetDevice> setDevices = new List<SetDevice>();
    public List<InteractiveDevice> devices = new List<InteractiveDevice>();
    [Serializable]
    public class SetDeviceList
    {
        public List<SetDevice> devicesList = new List<SetDevice>();
    }
    [Serializable]
    public class SetDevice
    {
        public List<Boolean> checkActivation = new List<bool>();
        public List<InteractiveDevice> devices = new List<InteractiveDevice>();
    }

    void Awake()
    {
        edgeLeftBottom = transform.Find("EdgeLeftBottom");
        edgeRightTop = transform.Find("EdgeRightTop");
    }

    public void CollectAllInteractiveDevices()
    {
        InteractiveDevice[] temp = this.GetComponentsInChildren<InteractiveDevice>(true);
        devices.Clear();
        foreach (InteractiveDevice t in temp)
        {
            devices.Add(t);
        }

        devices.Sort(SortFunc);
        RandomSetInteractiveDevices();
    }

    public void RandomSetInteractiveDevices()
    {
        int prevDepth = 0;
        int prevDepthDevice = 0;
        int prevDepthIdx = 0;
        SetDevice setDevice = new SetDevice();
        setDevicesLists.Clear();

        for (int i = 0; i < devices.Count; i++)
        {
            setDevice = new SetDevice();
            if (i == 0 || prevDepth != devices[i].depth)
            {
                prevDepth = devices[i].depth;
                prevDepthDevice = devices[i].depthDevice;
                prevDepthIdx = devices[i].depthDeviceIdx;
                SetDeviceList newDevice = new SetDeviceList();
                setDevice.checkActivation.Add(false);
                setDevice.devices.Add(devices[i]);
                newDevice.devicesList.Add(setDevice);
                setDevicesLists.Add(newDevice);

                if (devices[i].depth == 0)
                    devices[i].info.canUse = true;
                else
                    devices[i].info.canUse = false;
            }
            else if (prevDepth == devices[i].depth && prevDepthDevice != devices[i].depthDevice)
            {
                prevDepthDevice = devices[i].depthDevice;
                prevDepthIdx = devices[i].depthDeviceIdx;
                setDevice.checkActivation.Add(false);
                setDevice.devices.Add(devices[i]);
                setDevicesLists[prevDepth].devicesList.Add(setDevice);
                devices[i].info.canUse = false;
            }
            else
            {
                prevDepthIdx = devices[i].depthDeviceIdx;
                setDevicesLists[prevDepth].devicesList[prevDepthDevice].checkActivation.Add(false);
                setDevicesLists[prevDepth].devicesList[prevDepthDevice].devices.Add(devices[i]);
                devices[i].info.canUse = false;
            }

            devices[i].room = this;
        }
    }


    public int SortFunc(InteractiveDevice x, InteractiveDevice y)
    {
        int checkX = x.depth;
        int checkY = y.depth;

        if (checkY > checkX)
            return -1;
        else if (checkX > checkY)
            return 1;
        else
            return SortSubFunc(x, y);
    }

    public int SortSubFunc(InteractiveDevice x, InteractiveDevice y)
    {
        int checkX = x.depthDevice;
        int checkY = y.depthDevice;

        if (checkY > checkX)
            return -1;
        else if (checkX > checkY)
            return 1;
        else
            return SortSubFunc(x, y);
    }

    public int SortSubFuncIdx(InteractiveDevice x, InteractiveDevice y)
    {
        int checkX = x.depthDeviceIdx;
        int checkY = y.depthDeviceIdx;

        if (checkY > checkX)
            return -1;
        else if (checkX > checkY)
            return 1;
        else
            return 0;
    }

    public void CheckOnInteractiveDevice(InteractiveDevice _interactiveDevice)
    {
        for (int i = 0; i < setDevicesLists[_interactiveDevice.depth].devicesList[_interactiveDevice.depthDevice].devices.Count; i++)
        {
            if (_interactiveDevice == setDevicesLists[_interactiveDevice.depth].devicesList[_interactiveDevice.depth].devices[i])
            {
                setDevicesLists[_interactiveDevice.depth].devicesList[_interactiveDevice.depth].checkActivation[i] = true;
                if (i + 1 < setDevicesLists[_interactiveDevice.depth].devicesList[_interactiveDevice.depth].devices.Count)
                    setDevicesLists[_interactiveDevice.depth].devicesList[_interactiveDevice.depth].devices[i + 1].info.canUse = true;

                bool check = true;
                for (int devicesIdx = 0; devicesIdx < setDevicesLists[_interactiveDevice.depth].devicesList.Count; devicesIdx++)
                {
                    for (int checkIdx = 0; checkIdx < setDevicesLists[_interactiveDevice.depth].devicesList.Count; checkIdx++)
                    {
                        if (!setDevicesLists[_interactiveDevice.depth].devicesList[devicesIdx].checkActivation[checkIdx])
                        {
                            check = false;
                            break;
                        }
                    }

                    if (!check)
                        break;
                }

                if (check)
                {
                    GameManager.ShowLog("Check : " + check);
                    if (_interactiveDevice.depth + 1 < setDevicesLists.Count)
                    {
                        for (int idx = 0; idx < setDevicesLists[_interactiveDevice.depth + 1].devicesList.Count; idx++)
                        {
                            setDevicesLists[_interactiveDevice.depth + 1].devicesList[idx].checkActivation[0] = false;
                            setDevicesLists[_interactiveDevice.depth + 1].devicesList[idx].devices[0].info.canUse = true;
                            setDevicesLists[_interactiveDevice.depth + 1].devicesList[idx].devices[0].SetReadyAnim();
                        }
                    }
                }
                break;
            }
        }
    }
}
