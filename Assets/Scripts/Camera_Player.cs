using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // 포스트 프로세싱 네임스페이스 추가

public class Camera_Player : MonoBehaviour
{
    private static Camera_Player instance = null;
    public static Camera_Player Instance
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

    Camera cameraPlayer;
    public Transform defaultPlayerTransform;
    float cameraSpeed = 10;
    public Transform edgeLeftBottom;
    public Transform edgeRightTop;
    float sizeY;
    float sizeX;
    float cameraLeft, cameraRight, cameraBottom, cameraTop;
    float limitLeft, limitRight, limitTop, limitBottom;
    bool shouldFollowPlayerHorizontal, shouldFollowPlayerVertical;

    // PostProcessLayer 참조 변수 추가
    private PostProcessLayer postProcessLayer;


    // TODO: 점프할때 덜덜거리는 것 해결
    // TODO: if (Mathf.Abs(transform.position.x - targetPos.x) < 0.1f) 말고 왼쪽 모서리에 도달했으면 다시 오른쪽으로 어느정도 갔을때로 조건

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        cameraPlayer = GetComponent<Camera>();
        SetCameraLimit();

        // PostProcessLayer 컴포넌트 참조 초기화
        postProcessLayer = GetComponent<PostProcessLayer>();
    }

    void Update()
    {
        Vector3 cameraPos = cameraPlayer.transform.position;
        cameraLeft = cameraPos.x - sizeX;
        cameraRight = cameraPos.x + sizeX;
        cameraBottom = cameraPos.y - sizeY;
        cameraTop = cameraPos.y + sizeY;
        Vector3 targetPos = Player.Instance.transform.position;

        float clampedX = Mathf.Clamp(targetPos.x, cameraLeft, cameraRight);
        float clampedY = Mathf.Clamp(targetPos.y, cameraBottom, cameraTop);

        if (cameraLeft < limitLeft)
        {
            shouldFollowPlayerHorizontal = false;
            transform.position = new Vector3(limitLeft + sizeX, cameraPos.y, 0);
        }
        else if (cameraRight > limitRight)
        {
            shouldFollowPlayerHorizontal = false;
            transform.position = new Vector3(limitRight - sizeX, cameraPos.y, 0);
        }

        if (cameraBottom < limitBottom)
        {
            shouldFollowPlayerVertical = false;
            transform.position = new Vector3(cameraPos.x, limitBottom + sizeY, 0);
        }
        else if (cameraTop > limitTop)
        {
            shouldFollowPlayerVertical = false;
            transform.position = new Vector3(cameraPos.x, limitTop - sizeY, 0);
        }

        if (shouldFollowPlayerHorizontal)
        {
            // Debug.Log("Horizontal");
            transform.position = Vector3.Lerp(transform.position, new Vector3(clampedX, transform.position.y, 0), cameraSpeed * Time.deltaTime);
        }
        else
        {
            // Debug.Log("! Horizontal");
            if (Mathf.Abs(transform.position.x - targetPos.x) < 0.1f)
            {
                shouldFollowPlayerHorizontal = true;
            }
        }

        if (shouldFollowPlayerVertical)
        {
            // Debug.Log("Vertical");
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, clampedY, 0), cameraSpeed * Time.deltaTime);
        }
        else
        {
            // Debug.Log("! Vertical");
            if (Mathf.Abs(transform.position.y - targetPos.y) < 0.1f)
            {
                shouldFollowPlayerVertical = true;
            }
        }

        // Debug.Log($"{transform.position} | {targetPos}");
    }

    public void SetCameraEdge(Transform _edgeLeftBottom, Transform _edgeRightTop)
    {
        edgeLeftBottom = _edgeLeftBottom;
        edgeRightTop = _edgeRightTop;
    }

    public void SetCameraLimit()
    {
        limitLeft = edgeLeftBottom.position.x;
        limitRight = edgeRightTop.position.x;
        limitBottom = edgeLeftBottom.position.y;
        limitTop = edgeRightTop.position.y;
        sizeY = cameraPlayer.orthographicSize; // 화면 너비의 절반
        sizeX = cameraPlayer.orthographicSize * Screen.width / Screen.height; // 화면 높이의 절반
        shouldFollowPlayerHorizontal = true;
        shouldFollowPlayerVertical = true;
    }

    // 포스트 프로세싱 토글 함수
    public void TogglePostProcessing(bool isEnabled)
    {
        if (postProcessLayer != null)
        {
            postProcessLayer.enabled = isEnabled;
        }
    }
}
