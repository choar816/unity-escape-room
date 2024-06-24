using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sub;

[Serializable]
public class DeviceInfo
{
    [Header("기본 상태 정보")]
    public bool used = false; // 사용된
    public bool switchOn = false; // 스위치
    public bool canUse = false; // 사용가능
    [Header("장치 정보")]
    public ObjectSpecify specify; // 종류
    public InteractiveDeviceSpecify interactiveDeviceSpecify; // 장치

    [Header("아이템 실행 순차 태그")]
    public int depthTagIdx = 0;

    [Serializable]
    public class Depth
    {
        public List<ItemTag> depthTagItemTagList;
    }

    public List<Depth> depthTagList; // 기본 순차 태그 - 여러개의 조건 값에 따른 태그 분류
    public List<ItemTag> breakTagList; // 순차 무시 태그 - 순차 무시하고 기본 순차하고 같은 실행
    public List<ItemTag> effectTagList; // 이벤트 태그 - 조건 없이 실행되는 경우

    [Header("이미지 정보")]
    public UISprite sprite; //NGUI 이미지

    public DeviceInfo(InteractiveDeviceSpecify _interactiveDeviceSpecify,
                      string _atlas,
                      string _spriteName,
                      string _infoText)
    {
        used = false;
        switchOn = false;
        canUse = false;
        depthTagIdx = 0;
        Func.SetSprite(sprite, _atlas, _spriteName);
    }
}

public partial class InteractiveDevice : InteractiveAction
{
    [Header("기본 정보-----------------------------------------")]
    public DeviceInfo info; // 디바이스 종류
    [Header("실행 정보-----------------------------------------")]
    public int depth = 0; // 현재 맵 진행 단계 - 현재 진행되는 디바이스가 모두 클리어되야 맵이 진행됌.
    public int depthDevice = 0; // 현재 디바이스 진행 단계 - 디바이스들 사이에서 작동되는 순서.
    public int depthDeviceIdx = 0; // 현재 단계에 해당되는 디바이스 순서

    [Header("작동 메시지-----------------------------------------")]
    public List<string> actMSG = new List<string>();
    public List<string> deActMSG = new List<string>();
    public List<Hint> hintMSG = new List<Hint>();
    [Header("작동 애니메이션-----------------------------------------")]
    public List<Sub.AnimPlay> startAnimList; // 시작용 애니 리스트
    public List<Sub.AnimPlay> depthAnimList; // 순차 애니 리스트
    public List<Sub.AnimPlay> breakAnimList; // 순차 무시 애니 리스트
    public List<Sub.AnimPlay> switchAnimList; // 스위치 애니 리스트 - 작동하면 True 로 변환 / 0과 1만 존재
    public List<Sub.AnimPlay> switchTargetAnimList; // 스위치로 인한 움직임을 받는 애니 리스트 0 - 켬, 1 0 - 끔
    public List<Sub.AnimPlay> deActiveAnimList; // 단순 취소 애니 리스트
    [Header("조건 완료 시 작동되는 리스트--------------------------------")]
    public List<Sub.AnimPlay> endAnimList; // 모든 작동 후 or 액션 후 링크된 오브젝트 애니
    public List<InteractiveDevice> endActList; // 모든 작동 후 작동 종료 리스트(예외의 경우 따로 작동)
    [Header("링크------------------------------------------------")]
    public Room room;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Interact()
    {
        if (info.used) // 사용된 후
            return;

        base.Interact();

        if (Vector3.Distance(Player.Instance.transform.position, this.transform.position) > GameManager.Instance.fixedValue.interactiveDistance)
        {
            GameManager.ShowLog("좀 더 가까이 가야한다!!", GameManager.Color.red);
            return;
        }

        GameManager.ShowLog("플레이어가 장치를 작동했어요 : " + info.specify);

        if (!info.canUse) // 사용 가능 상태 아니면 취소
            return;


        bool _active = false;
        bool _effect = false;
        bool _switch = false;

        if (GameManager.Instance.playInfo.equipedItem.specify != ObjectSpecify.UnEquiped) // 아이템 장착 상태
        {
            if (GameManager.Instance.playInfo.equipedItem.tagList.Count <= 0)
            {
                GameManager.ShowLog("아이템 버그", GameManager.Color.red);
                return;
            }

            for (int itemTagIdx = 0; itemTagIdx < GameManager.Instance.playInfo.equipedItem.tagList.Count; itemTagIdx++)
            {
                // 순차 무시
                for (int tagIdx = 0; tagIdx < info.breakTagList.Count; tagIdx++)
                {
                    if (GameManager.Instance.playInfo.equipedItem.tagList[itemTagIdx] == info.breakTagList[tagIdx])
                    {
                        _active = true;
                        ActiveEnd(true);
                        break;
                    }
                }

                // 순차
                if (info.depthTagList.Count > info.depthTagIdx)
                {
                    for (int depthTagItemIdx = 0; depthTagItemIdx < info.depthTagList[info.depthTagIdx].depthTagItemTagList.Count; depthTagItemIdx++)
                    {
                        if (GameManager.Instance.playInfo.equipedItem.tagList[itemTagIdx] == info.depthTagList[info.depthTagIdx].depthTagItemTagList[depthTagItemIdx])
                        {
                            _active = true;
                            if (depthAnimList.Count > info.depthTagIdx)
                                depthAnimList[info.depthTagIdx].Anim.Play(depthAnimList[info.depthTagIdx].AnimName);

                            info.depthTagIdx++;
                            if (info.depthTagList.Count == info.depthTagIdx)
                                ActiveEnd(false);
                            break;
                        }
                    }
                }
            }

            // 스위치
            if (switchAnimList.Count > 0)
            {
                if (info.switchOn)
                {
                    info.switchOn = false;
                    switchAnimList[1].Anim.Play(switchAnimList[1].AnimName);
                    if (switchTargetAnimList.Count == 2)
                        switchTargetAnimList[1].Anim.Play(switchTargetAnimList[0].AnimName);
                }
                else
                {
                    info.switchOn = true;
                    switchAnimList[0].Anim.Play(switchAnimList[0].AnimName);
                    if (switchTargetAnimList.Count == 2)
                        switchTargetAnimList[0].Anim.Play(switchTargetAnimList[0].AnimName);

                    ActiveEnd(false);
                }

                _switch = true;
            }

            for (int idx = 0; idx < info.effectTagList.Count; idx++)
            {
                GetEffect(info.effectTagList[idx]);
                _effect = true;
            }

            if (_effect || _switch)
                return;

            if (GameManager.Instance.playInfo.equipedItem.specify != ObjectSpecify.UnEquiped)
            {
                if (_active)
                    GameManager.Instance.UseEquipedItem();
            }
        }

    }

    public void ActiveEnd(bool destroy)
    {
        if (destroy)
        {
            for (int i = 0; i < breakAnimList.Count; i++)
            {
                breakAnimList[i].Anim.Play(breakAnimList[i].AnimName);
            }
        }

        for (int i = 0; i < endAnimList.Count; i++)
            endAnimList[i].Anim.Play(endAnimList[i].AnimName);

        for (int i = 0; i < endActList.Count; i++)
            room.CheckOnInteractiveDevice(endActList[i]);

        room.CheckOnInteractiveDevice(this);
        info.used = true;
    }

    public void SetReadyAnim()
    {
        for (int i = 0; i < startAnimList.Count; i++)
            startAnimList[i].Anim.Play(startAnimList[i].AnimName);
    }
}
