using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InteractiveDevice : InteractiveAction
{
    public void GetEffect(ItemTag tag)
    {
        bool _effect = false;
        switch (tag)
        {
            case ItemTag.ChargePhone:
                GameManager.Instance.playInfo.drawValue = 3;
                GameManager.ShowLog("Charged the phone");
                _effect = true;
                break;
            case ItemTag.TurnOnLight:
                _effect = true;
                break;
        }

        GameManager.ShowLog($"Effect Tag : {tag} | Active : {_effect}");
        if (_effect)
            ActiveEnd(true);
    }
}
