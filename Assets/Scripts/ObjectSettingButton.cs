using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSettingButton : MonoBehaviour
{
    ObjectController objectController;
    ObjectController.ButtonType type;

    void Start()
    {
        objectController = transform.parent.transform.parent.GetComponent<ObjectController>();
        type = (ObjectController.ButtonType)Enum.Parse(typeof(ObjectController.ButtonType), gameObject.name);
        objectController.settingButtons.Add(type, this);
        UIEventListener.Get(gameObject).onClick += OnClickHandler;
    }

    void OnClickHandler(GameObject go)
    {
        objectController.OnClickSettingButton(type);
    }
}
