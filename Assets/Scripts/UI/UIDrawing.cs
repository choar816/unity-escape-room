using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIDrawing : MonoBehaviour
{
    public UIButton BtnDraw;
    public UIButton BtnReset;
    // Start is called before the first frame update
    void Start()
    {
        EventDelegate onClickEvent = new EventDelegate(ControlCenter.Instance, "DrawCheckWithAI");
        BtnDraw.onClick.Add(onClickEvent);
        //onClickEvent = new EventDelegate(ControlCenter.Instance, "ResetDrawBoard");
        //BtnReset.onClick.Add(onClickEvent);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
