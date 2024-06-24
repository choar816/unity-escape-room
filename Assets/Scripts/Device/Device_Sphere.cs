using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device_Sphere : MonoBehaviour
{
    [Header("장치 UI (Sphere) ================================")]
    [SerializeField] Animator sphereAnim;
    [SerializeField] List<UIButton> spherePartList; // 0: top, 1: mid, 2: bottom
    [SerializeField] GameObject sphereOnTop;
    [SerializeField] GameObject sphereOnMid;
    [SerializeField] GameObject sphereOnBottom;
    [SerializeField] UIButton prevButton;
    [SerializeField] UIButton nextButton;
    List<List<string>> sphereSpriteNameList;
    List<int> spherePartIndexList;
    int selectedSpherePart;

    void Start()
    {
        Init_DeviceUI_Sphere();
    }

    void Init_DeviceUI_Sphere()
    {
        sphereSpriteNameList = new List<List<string>> {
            new List<string> { "sphere_top_0", "sphere_top_1", "sphere_top_2", "sphere_top_3", "sphere_top_4", "sphere_top_5" },
            new List<string> { "sphere_mid_0", "sphere_mid_1", "sphere_mid_2", "sphere_mid_3", "sphere_mid_4", "sphere_mid_5" },
            new List<string> { "sphere_bottom_0", "sphere_bottom_1", "sphere_bottom_2", "sphere_bottom_3", "sphere_bottom_4", "sphere_bottom_5" }
        };
        spherePartIndexList = new List<int> { 0, 0, 5 };
        UpdateSphereImage();
        DeselectSpherePart();
    }

    public void SelectSpherePart_Top()
    {
        sphereOnTop.SetActive(true);
        sphereOnMid.SetActive(false);
        sphereOnBottom.SetActive(false);
        selectedSpherePart = 0;
    }

    public void SelectSpherePart_Mid()
    {
        sphereOnTop.SetActive(false);
        sphereOnMid.SetActive(true);
        sphereOnBottom.SetActive(false);
        selectedSpherePart = 1;
    }

    public void SelectSpherePart_Bottom()
    {
        sphereOnTop.SetActive(false);
        sphereOnMid.SetActive(false);
        sphereOnBottom.SetActive(true);
        selectedSpherePart = 2;
    }

    public void DeselectSpherePart()
    {
        sphereOnTop.SetActive(false);
        sphereOnMid.SetActive(false);
        sphereOnBottom.SetActive(false);
        selectedSpherePart = -1;
    }

    public void SelectPrevSpherePart()
    {
        if (selectedSpherePart == -1)
        {
            SelectSpherePart_Mid();
        }

        spherePartIndexList[selectedSpherePart] = (spherePartIndexList[selectedSpherePart] + 5) % 6;
        spherePartList[selectedSpherePart].normalSprite = sphereSpriteNameList[selectedSpherePart][spherePartIndexList[selectedSpherePart]];
        SolveSphere();
    }

    public void SelectNextSpherePart()
    {
        if (selectedSpherePart == -1)
        {
            SelectSpherePart_Mid();
        }

        spherePartIndexList[selectedSpherePart] = (spherePartIndexList[selectedSpherePart] + 1) % 6;
        spherePartList[selectedSpherePart].normalSprite = sphereSpriteNameList[selectedSpherePart][spherePartIndexList[selectedSpherePart]];
        SolveSphere();
    }

    void UpdateSphereImage()
    {
        for (int i = 0; i < 3; ++i)
        {
            spherePartList[i].normalSprite = sphereSpriteNameList[i][spherePartIndexList[i]];
        }
    }

    void SolveSphere()
    {
        if (spherePartIndexList[0] != 0 || spherePartIndexList[1] != 0 || spherePartIndexList[2] != 0)
            return;

        sphereAnim.SetTrigger("SolveTrigger");
        prevButton.isEnabled = false;
        nextButton.isEnabled = false;
        sphereOnTop.SetActive(false);
        sphereOnMid.SetActive(false);
        sphereOnBottom.SetActive(false);
        for (int i = 0; i < 3; ++i)
        {
            spherePartList[i].enabled = false;
        }
    }

    void HideSphereParts()
    {
        for (int i = 0; i < 3; ++i)
        {
            spherePartList[i].gameObject.SetActive(false);
        }
    }
}
