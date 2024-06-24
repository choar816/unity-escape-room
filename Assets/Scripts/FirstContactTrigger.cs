using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 처음 접촉했을때 특정 함수 등을 호출하도록 하는 컴포넌트
public class FirstContactTrigger : MonoBehaviour
{
    bool hasBeenRecognized = false;
    public GameObject lampRecognition;

    void Start()
    {
        if (lampRecognition != null)
        {
            lampRecognition.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != Player.Instance.gameObject)
        {
            return;
        }

        if (!hasBeenRecognized)
        {
            if (gameObject.name == "Locker" && DeviceManager.Instance.IsDeviceSolved("Documents"))
            {
                CutSceneManager.Instance.StartCutscene(CutsceneType.Locker_Dark);
                lampRecognition.SetActive(true);
                hasBeenRecognized = true;
            }
            else if (gameObject.name == "LampRecognition")
            {
                CutSceneManager.Instance.StartCutscene(CutsceneType.Lamp_TooHigh);
                hasBeenRecognized = true;
            }
        }
    }
}
