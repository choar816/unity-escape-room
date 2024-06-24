using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

// 컷씬 타입
public enum CutsceneType
{
    None,
    Hat_LetsWear,
    Hat_AfterWearing,
    Clock_FoundBroken,
    Clock_LetsCombine,
    Locker_CantFind,
    Locker_Dark,
    Lamp_TooHigh,
    Lamp_LetsJump,
    Lamp_NoBulb,
    Lamp_GotBulb,
    Lamp_GotNoBulb,
    Lamp_UsedBulb,
    Locker_Key,
    Sphere_AfterSolve,
    Sphere_Keyhole,
    Letter_LetsSee,
    Letter_AfterSee,
    Lock_GotPipe
}

public class CutSceneManager : MonoBehaviour
{
    private static CutSceneManager instance = null;
    public static CutSceneManager Instance
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
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    Dictionary<CutsceneType, List<Action>> cutsceneData; // key: 컷씬 타입, value: 컷씬에서 진행할 action들
    Dictionary<CutsceneType, bool> cutscenePlayedData; // key: 컷씬 타입, value: 한번이라도 컷씬이 나왔는지 여부
    CutsceneType cutsceneTypeInProgress; // 진행중인 컷씬 타입
    int cutsceneProgressIndex = -1; // cutsceneData의 action list에서 어디까지 진행했는지 저장
    public bool isCutsceneOn;

    void Start()
    {
        isCutsceneOn = false;
        InitCutsceneData();
        // Invoke("Temp", 1f);
    }

    // test
    void Temp()
    {
        StartCutscene(CutsceneType.Locker_Dark);
    }

    void Update()
    {
        if (isCutsceneOn && Input.GetMouseButtonDown(0))
        {
            ProgressCutscene();
        }
    }

    public void StartCutscene(CutsceneType type)
    {
        if (SpeechManager.Instance == null)
        {
            SpeechManager.Instance = FindObjectOfType<SpeechManager>();
        }

        isCutsceneOn = true;
        cutsceneTypeInProgress = type;
        cutsceneProgressIndex = 0;
        cutsceneData[cutsceneTypeInProgress][cutsceneProgressIndex].Invoke();
        Player.Instance.DisableMove();
    }

    void ProgressCutscene()
    {
        cutsceneProgressIndex += 1;
        if (cutsceneProgressIndex == cutsceneData[cutsceneTypeInProgress].Count)
        {
            FinishCutscene();
            return;
        }

        cutsceneData[cutsceneTypeInProgress][cutsceneProgressIndex].Invoke();
    }

    void FinishCutscene()
    {
        isCutsceneOn = false;
        cutsceneTypeInProgress = CutsceneType.None;
        cutsceneProgressIndex = -1;
        SpeechManager.Instance.HideSpeechUI();
        Player.Instance.EnableMove();
    }

    Action GetShowSpeechFunc(string speech)
    {
        return () =>
        {
            SpeechManager.Instance.ShowSpeechUI(speech);
        };
    }

    void InitCutsceneData()
    {
        cutsceneData = new Dictionary<CutsceneType, List<Action>>
        {
            {
                CutsceneType.Hat_LetsWear,
                new List<Action>
                {
                    GetShowSpeechFunc("음…아멜리아가 마지막으로 발견된 장소로 가보자. 일단, 내 외투를 입어야 겠군…"),
                }
            },
            {
                CutsceneType.Hat_AfterWearing,
                new List<Action>
                {
                    GetShowSpeechFunc("이제야 나갈 준비를 마쳤군…아 시계를 깜빡했군…시계를 어디에 뒀더라?!"),
                }
            },
            {
                CutsceneType.Clock_FoundBroken,
                new List<Action>
                {
                    GetShowSpeechFunc("음..시계가 깨져 있군, 깨진 것을 활용할 수 있는지 인벤토리에서 확인해 보자."),
                }
            },
            {
                CutsceneType.Clock_LetsCombine,
                new List<Action>
                {
                    GetShowSpeechFunc("깨져 있는 시계를 조립할 수 있겠군…"),
                }
            },
            {
                CutsceneType.Locker_CantFind,
                new List<Action>
                {
                    GetShowSpeechFunc("왓슨, 퍼즐 상자 어디에 뒀는지 아니? 아니다…내가 찾아볼게."),
                }
            },
            {
                CutsceneType.Locker_Dark,
                new List<Action>
                {
                    GetShowSpeechFunc("사물함 문이 열리지 않네. 어두워서 보이지도 않잖아? 일단, 불을 먼저 켜볼까?!"),
                }
            },
            {
                CutsceneType.Lamp_TooHigh,
                new List<Action>
                {
                    GetShowSpeechFunc("음…손이 닿지 않네? 일단 생각을 좀 해보자! 생각 버튼을 클릭해보자."),
                }
            },
            {
                CutsceneType.Lamp_LetsJump,
                new List<Action>
                {
                    GetShowSpeechFunc("이걸 밟고 올라갈 수 있겠지? 점프해 보자."),
                }
            },
            {
                CutsceneType.Lamp_NoBulb,
                new List<Action>
                {
                    GetShowSpeechFunc("음…전등이 비어 있군. 전구가 필요해 보인다. 어떤 전구를 사용 해야 하지?! 생각을 해보자! 생각 버튼을 클릭해보자."),
                }
            },
            {
                CutsceneType.Lamp_GotBulb,
                new List<Action>
                {
                    GetShowSpeechFunc("생각대로 전구를 끼워 넣으면 불이 들어오겠지? 인벤토리에서 전구를 사용해보자!"),
                }
            },
            {
                CutsceneType.Lamp_GotNoBulb,
                new List<Action>
                {
                    GetShowSpeechFunc("전구가 필요한 건데…이상한 물건이 나왔군…다시 생각을 통해 전구를 그려보자!"),
                }
            },
            {
                CutsceneType.Lamp_UsedBulb,
                new List<Action>
                {
                    GetShowSpeechFunc("전등에 불이 들어오니, 잠긴 사물함을 열 수 있겠군."),
                }
            },
            {
                CutsceneType.Locker_Key,
                new List<Action>
                {
                    GetShowSpeechFunc("문이 잠긴 이유를 알겠네~ 열쇠가 어떤 건지 생각해보자! 생각 버튼을 클릭!"),
                }
            },
            {
                CutsceneType.Sphere_AfterSolve,
                new List<Action>
                {
                    GetShowSpeechFunc("누가 깜찍하게 이런 짓을…다시 퍼즐 상자를 열어보자."),
                }
            },
            {
                CutsceneType.Sphere_Keyhole,
                new List<Action>
                {
                    GetShowSpeechFunc("열쇠가 있을리가 없잖아, 생각을 클릭해보자!"),
                }
            },
            {
                CutsceneType.Letter_LetsSee,
                new List<Action>
                {
                    GetShowSpeechFunc("음…쪽지 내용을 살펴보자!"),
                }
            },
            {
                CutsceneType.Letter_AfterSee,
                new List<Action>
                {
                    GetShowSpeechFunc("쪽지를 살펴보니 이걸 열 수 있겠군…"),
                }
            },
            {
                CutsceneType.Lock_GotPipe,
                new List<Action>
                {
                    GetShowSpeechFunc("형이 새 파이프를 보냈군!! 그럼 이제 현장으로 나가볼까?!"),
                }
            },
        };
    }
}
