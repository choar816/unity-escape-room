using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ControlCenter : MonoBehaviour
{
    private static ControlCenter instance = null;

    public GameInfo gameInfo = new GameInfo();

    public enum GameMode
    {
        Reality,
        Thought,
        Thought_Draw,
        Cutscene,
    }

    /// <summary>
    /// 게임 정보 관련 모음 - 임시적인 부분
    /// </summary>
    [Serializable]
    public class GameInfo
    {
        public GameMode mode = GameMode.Reality;
    }

    public ChapterManager ChapterManager;
    public CutSceneManager CutSceneManager;
    public EffectManager EffectManager;
    public UIManager UIManager;
    public PoolManager PoolManager;
    public DrawingManager DrawingManager;
    public CameraManager CameraManager;


    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    public static ControlCenter Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    public void ChangeMode(ControlCenter.GameMode mode)
    {
        gameInfo.mode = mode;

        CameraManager.Instance.ChangeMode(mode);
        DrawingManager.Instance.ChangeMode(mode);
        UIManager.Instance.ChangeMode(mode);
    }
    // TODO : OnClick이벤트에서 enum을 못써서 이렇게.. 추후 수정 예정
    public void SwitchToRealityMode()
    {
        ChangeMode(ControlCenter.GameMode.Reality);
    }
    public void SwitchToThoughtMode()
    {
        ChangeMode(ControlCenter.GameMode.Thought);
    }
    public void SwitchToDrawMode()
    {
        ChangeMode(ControlCenter.GameMode.Thought_Draw);
    }

    public void SwitchToCutsceneMode()
    {
        ChangeMode(ControlCenter.GameMode.Cutscene);
    }


    public void RestartGame()
    {
        ReturnToNormalMode();
        GameManager.Instance.playInfo.drawValue = GameManager.Instance.fixedValue.drawValueMax;
        //DrawValueSet(false);
    }

    public void ReturnToNormalMode()
    {
        gameInfo.mode = GameMode.Reality;
        //TODO UI 연결
    }

    public void DrawCheckWithAI()
    {
        //// TODO : 배터리 안써서 제거해야 할 코드
        //if (GameManager.Instance.playInfo.drawValue == 0)
        //{
        //    GameManager.ShowLog("Need more Draw Value");
        //    return;
        //}

        if (!RecognizeItemWithAIBarracuda.Instance.isActive)
        {
            RecognizeItemWithAIBarracuda.Instance.ActiveAI(false);
            GameManager.Instance.MakeItem();
            //DrawValueSet(true);
        }
    }

    //public void ResetDrawBoard()
    //{
    //    // TODO : DrawingMaager로 바꿔야 함 
    //    Debug.Log("#1");
    //    if (!RecognizeItemWithAIBarracuda.Instance.isActive)
    //        DrawingManager.Instance.ResetCurrentCanvas();
    //}

    //public void DrawValueSet(bool use)
    //{
    //    if (use)
    //        GameManager.Instance.playInfo.drawValue -= 1;

    //    //TODO #1 배터리 이미지 연결
    //}
}
