using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLock : InteractivePuzzle
{
    [Header("추가적인 장치 요소==============================================")]

    public List<string> passWord = new List<string>();
    public int lockLength = 4;

    public List<UILabel> numberLabelList = new List<UILabel>();
    public UISprite spriteLock;
    public List<string> selectCharList = new List<string>();
    private List<string> hideCharList = new List<string>(){
        "0","1","2","3","4","5","6","7","8","9",
        "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
    };

    private List<int> dialingPivotIdx = new List<int>();
    private int lineNumber = 12;
    public void Set()
    {
        List<string> t_showCharList = new List<string>();
        spriteLock.spriteName = "word_lock_locked";
        // INIT
        int preSelectPassWordPos = UnityEngine.Random.Range(0, lineNumber);
        selectCharList = new List<string>();
        dialingPivotIdx = new List<int>();
        selectedDialingPivotIdx = 0;

        for (int idx = 0; idx < lockLength; idx++)
        {
            dialingPivotIdx.Add(0);
            foreach (string t in hideCharList)
            {
                bool putIn = true;
                foreach (string set in passWord)
                {
                    if (set == t)
                    {
                        putIn = false;
                        break;
                    }
                }

                if (putIn)
                    t_showCharList.Add(t);
            }

            int selectPos = UnityEngine.Random.Range(0, lineNumber);
            while (true)
            {
                if (selectPos == preSelectPassWordPos)
                    selectPos = UnityEngine.Random.Range(0, lineNumber);
                else
                    break;
            }

            for (int i = 0; i < lineNumber; i++)
            {
                string putIn = "";
                if (i == selectPos)
                {
                    putIn = passWord[idx];
                    preSelectPassWordPos = selectPos;
                }
                else
                {
                    int t_showCharIdx = UnityEngine.Random.Range(0, t_showCharList.Count);
                    putIn = t_showCharList[t_showCharIdx];
                    t_showCharList.RemoveAt(t_showCharIdx);
                }

                selectCharList.Add(putIn);
                if (i < lineNumber / 2)
                    numberLabelList[idx * (lineNumber / 2) + i].text = putIn;
            }
        }
    }

    private int selectedDialingPivotIdx = 0;
    public void SelectDialing(int _idx)
    {
        selectedDialingPivotIdx = _idx;
    }

    public void NextDialing(bool _up)
    {
        if (_up)
            dialingPivotIdx[selectedDialingPivotIdx]++;
        else
            dialingPivotIdx[selectedDialingPivotIdx]--;

        if (dialingPivotIdx[selectedDialingPivotIdx] >= lineNumber)
            dialingPivotIdx[selectedDialingPivotIdx] = 0;
        else if (dialingPivotIdx[selectedDialingPivotIdx] < 0)
            dialingPivotIdx[selectedDialingPivotIdx] = lineNumber - 1;

        ShowLabelDialing();
        Check();
    }

    void ShowLabelDialing()
    {
        for (int i = 0; i < lineNumber / 2; i++)
        {
            int idx = dialingPivotIdx[selectedDialingPivotIdx] + i >= lineNumber ? dialingPivotIdx[selectedDialingPivotIdx] + i - lineNumber : dialingPivotIdx[selectedDialingPivotIdx] + i;
            numberLabelList[selectedDialingPivotIdx * (lineNumber / 2) + i].text = selectCharList[selectedDialingPivotIdx * lineNumber + idx];
        }
    }

    public void Check()
    {
        bool success = true;
        for(int i = 0 ; i < passWord.Count ; i++)
        {
            if(passWord[i] != numberLabelList[i * (lineNumber / 2) + 2].text)
            {
                success = false;
                break;
            }
        }

        Debug.Log("Check : "+success);
        if(success)
        {
            spriteLock.spriteName = "word_lock_open";
            Clear();
            //TODO Next Step in Clear()
        }
    }

    bool clicked = false;
    Vector3 originMousePos = new Vector3();
    public void ClickBtnDialing()
    {
        clicked = true;
        originMousePos = Input.mousePosition;
    }

    public void ReleaseBtnDialing()
    {
        clicked = false;
    }

    float checkDragDistance = 30f;
    private void Update()
    {
        if (clicked)
        {
            if (originMousePos.y - Input.mousePosition.y > checkDragDistance)
            {
                originMousePos = Input.mousePosition;
                NextDialing(false);
            }
            else if (originMousePos.y - Input.mousePosition.y < -checkDragDistance)
            {
                originMousePos = Input.mousePosition;
                NextDialing(true);
            }
        }
    }

    void Start()
    {
        Set();
    }
}
