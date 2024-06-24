using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

class RoomInfo
{
    public Transform edgeLeftBottom;
    public Transform edgeRightTop;
    // TODO 문 추가

    public RoomInfo(Transform _edgeLeftBottom, Transform _edgeRightTop)
    {
        edgeLeftBottom = _edgeLeftBottom;
        edgeRightTop = _edgeRightTop;
    }
}

public class RoomManager : MonoBehaviour
{
    private static RoomManager instance = null;
    public static RoomManager Instance
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

    public GameObject currentScene;
    Dictionary<int, RoomInfo> roomList; // key: roomNumber
    Dictionary<int, Door> doorList; // key: doorNumber
    Dictionary<int, int> doorConnection; // 어떤 문이 어떤 문에 연결되는지에 대한 정보
                                         // ex) {1, 2}: 1번 문은 2번 문으로 연결

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        roomList = new Dictionary<int, RoomInfo>();
        doorList = new Dictionary<int, Door>();
        doorConnection = new Dictionary<int, int>();

        // scene의 직계 자식 순회 -> roomList 채우기
        foreach (Transform roomTransform in currentScene.transform)
        {
            Room room = roomTransform.GetComponent<Room>();
            if (room == null)
                continue;
        
            roomList.Add(room.roonNumber, new RoomInfo(room.edgeLeftBottom, room.edgeRightTop));

            // room의 직계 자식 순회 -> doorList, doorConnection 채우기
            foreach (Transform transform in roomTransform)
            {
                Door door = transform.GetComponent<Door>();
                if (door != null)
                {
                    doorList.Add(door.doorNumber, door);
                    doorConnection.Add(door.doorNumber, door.destDoor.doorNumber);
                }
            }
        }

        // door마다 속한 roomNumber 입력
        foreach (KeyValuePair<int, Door> kvp in doorList)
        {
            Door door = kvp.Value;
            door.roomNumber = door.transform.parent.GetComponent<Room>().roonNumber;
        }
    }

    void Update()
    {
        if (Player.Instance.m_Stat.canEnterDoor && Input.GetKeyDown(KeyCode.F))
        {
            GoToDoor(Player.Instance.m_Stat.nearestDoorNumber);
        }
    }

    void GoToDoor(int doorNumber)
    {
        Door dest_Door = doorList[doorNumber].destDoor;
        Player.Instance.transform.position = dest_Door.transform.position + Vector3.up * 0.5f;
        Player.Instance.interactionKey.SetActive(true);
        Player.Instance.m_Stat.canEnterDoor = true;
        Player.Instance.m_Stat.nearestDoorNumber = dest_Door.doorNumber;

        Camera_Player.Instance.SetCameraEdge(roomList[dest_Door.roomNumber].edgeLeftBottom, roomList[dest_Door.roomNumber].edgeRightTop);
        Camera_Player.Instance.SetCameraLimit();
        Camera_Player.Instance.transform.position = Player.Instance.transform.position;
    }
}
