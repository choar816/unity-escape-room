using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIManager : MonoBehaviour
{
    public void Show_DeviceUI_Sphere()
    {
        DeviceUI_Sphere.SetActive(true);
    }

    public void Hide_DeviceUI_Sphere()
    {
        DeviceUI_Sphere.SetActive(false);
    }
}