using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechManager : MonoBehaviour
{
    private static SpeechManager instance = null;
    public static SpeechManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
        set { instance = value; }
    }

    public Transform speaker;
    public Transform speechUI;
    Transform speechBubble;
    UISprite speechBubbleSprite;
    Transform speechText;
    UILabel speechTextLabel;
    public bool isSpeechOn;
    int speechBubbleMinHeight = 56;
    int speechTextMinHeight = 28;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        speaker = Player.Instance.transform;
        speechBubble = speechUI.Find("SpeechBubble");
        speechBubbleSprite = speechBubble.GetComponent<UISprite>();
        speechText = speechBubble.Find("SpeechText");
        speechTextLabel = speechText.GetComponent<UILabel>();
        speechTextLabel.text = "hello";
        isSpeechOn = false;
    }

    void Update()
    {
        ResetLocation();
    }

    void ResetLocation()
    {
        Vector3 displayLocation = speaker.position + Vector3.up * 2.7f;
        speechBubble.position = CameraManager.Instance.UICamera.ViewportToWorldPoint(CameraManager.Instance.MainCamera.WorldToViewportPoint(displayLocation));
    }

    public void ShowSpeechUI(string text)
    {
        speechTextLabel.text = text;
        // 말풍선 크기를 텍스트에 따라 조절
        speechBubbleSprite.height = Mathf.Max(speechBubbleMinHeight, speechTextLabel.height + 28);
        speechUI.gameObject.SetActive(true);
    }

    public void HideSpeechUI()
    {
        speechUI.gameObject.SetActive(false);
    }
}
