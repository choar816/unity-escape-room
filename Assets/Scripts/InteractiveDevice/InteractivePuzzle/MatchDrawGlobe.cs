using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class MatchDrawGlobe : InteractivePuzzle
{
    [Header("추가적인 장치 요소==============================================")]
    public int selectDepth = 0;
    public List<int> matchIdxList = new List<int>();
    public List<int> currentMatchIdxList = new List<int>();
    public List<UISprite> spritePositionList = new List<UISprite>();
    public List<UISprite> spritePositionGlowList = new List<UISprite>();
    public List<UIButton> btnList = new List<UIButton>();
    public List<UIButton> btnSelectList = new List<UIButton>();

    private void Start()
    {
        EventDelegate onClickEvent = new EventDelegate(this, "SelectDirectionArrow");
        onClickEvent.parameters[0] = new EventDelegate.Parameter(true);
        btnList[0].onClick.Add(onClickEvent);
        onClickEvent = new EventDelegate(btnList[0].GetComponentInChildren<SimpleChangeSpriteTime>(), "Change");
        btnList[0].onClick.Add(onClickEvent);
        onClickEvent = new EventDelegate(this, "SelectDirectionArrow");
        onClickEvent.parameters[0] = new EventDelegate.Parameter(false);
        btnList[1].onClick.Add(onClickEvent);
        onClickEvent = new EventDelegate(btnList[1].GetComponentInChildren<SimpleChangeSpriteTime>(), "Change");
        btnList[1].onClick.Add(onClickEvent);

        onClickEvent = new EventDelegate(this, "SelectDepth");
        onClickEvent.parameters[0] = new EventDelegate.Parameter(0);
        btnSelectList[0].onClick.Add(onClickEvent);
        onClickEvent = new EventDelegate(this, "SelectDepth");
        onClickEvent.parameters[0] = new EventDelegate.Parameter(1);
        btnSelectList[1].onClick.Add(onClickEvent);
        onClickEvent = new EventDelegate(this, "SelectDepth");
        onClickEvent.parameters[0] = new EventDelegate.Parameter(2);
        btnSelectList[2].onClick.Add(onClickEvent);

        spritePositionGlowList[selectDepth].gameObject.SetActive(true);

        for (int i = 0; i < 3; ++i)
        {
            ChangeSphereImage(i, currentMatchIdxList[i]);
        }
    }

    public void SelectDepth(int _depth)
    {
        spritePositionGlowList[selectDepth].gameObject.SetActive(false);
        selectDepth = _depth;
        spritePositionGlowList[selectDepth].gameObject.SetActive(true);
    }

    public void SelectDirectionArrow(bool _left)
    {
        if (_left)
        {
            currentMatchIdxList[selectDepth] += 1;
            if (currentMatchIdxList[selectDepth] >= 6)
                currentMatchIdxList[selectDepth] = 0;
            //btnList[1].GetComponentInChildren<TweenColor>().ResetToBeginning();
        }
        else
        {
            currentMatchIdxList[selectDepth] -= 1;
            if (currentMatchIdxList[selectDepth] < 0)
                currentMatchIdxList[selectDepth] = 5;
            //btnList[0].GetComponentInChildren<TweenColor>().ResetToBeginning();
        }

        ChangeSphereImage(selectDepth, currentMatchIdxList[selectDepth]);
        Check();
        // Debug.Log("Select Depth : " + selectDepth + " || match Idx : " + currentMatchIdxList[selectDepth]);
    }

    public void Check()
    {
        bool success = true;
        for (int i = 0; i < matchIdxList.Count; i++)
        {
            if (matchIdxList[i] != currentMatchIdxList[i])
            {
                success = false;
                break;
            }
        }

        if (success)
        {
            this.GetComponent<Animation>().Play();
            Clear();
            //TODO Next Step in Clear()
        }
    }

    public void ChangeSphereImage(int depth, int idx)
    {
        switch (depth)
        {
            case 0:
                spritePositionList[depth].spriteName = "sphere_top_" + idx;
                break;
            case 1:
                spritePositionList[depth].spriteName = "sphere_mid_" + idx;
                break;
            case 2:
                spritePositionList[depth].spriteName = "sphere_bottom_" + idx;
                break;
        }
    }
}
