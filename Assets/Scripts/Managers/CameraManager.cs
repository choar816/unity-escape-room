using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager : MonoBehaviour
{
    // 싱글턴 인스턴스
    public static CameraManager Instance { get; private set; }

    // 에디터에서 설정할 두 카메라
    public Camera MainCamera;
    public Camera UICamera;

    // PostProcessVolume 변수 추가
    public PostProcessVolume PostProcessVolume;

    // 각 카메라의 PostProcessLayer 컴포넌트 참조
    private PostProcessLayer mainCameraPPL;
    private PostProcessLayer UICameraPPL;

    // Color Grading 변수
    private ColorGrading colorGrading;

    private void Awake()
    {
        // 싱글턴 패턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 카메라로부터 PostProcessLayer 컴포넌트 가져오기
        if (MainCamera != null)
        {
            mainCameraPPL = MainCamera.GetComponent<PostProcessLayer>();
        }

        if (UICamera != null)
        {
            UICameraPPL = UICamera.GetComponent<PostProcessLayer>();
        }

        // ColorGrading 참조 설정
        if (PostProcessVolume != null)
        {
            PostProcessVolume.profile.TryGetSettings(out colorGrading);
        }
    }

    // 메인 카메라의 포스트 프로세싱 토글 함수
    public void ToggleMainCameraPostProcessing(bool isEnabled)
    {
        if (mainCameraPPL != null)
        {
            mainCameraPPL.enabled = isEnabled;
        }
    }

    // UI 카메라의 포스트 프로세싱 토글 함수
    public void ToggleUICameraPostProcessing(bool isEnabled)
    {
        if (UICameraPPL != null)
        {
            UICameraPPL.enabled = isEnabled;
        }
    }

    // Saturation 값을 설정하는 함수
    public void SetSaturation(float value)
    {
        if (colorGrading != null)
        {
            colorGrading.saturation.value = value;
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
            ToggleMainCameraPostProcessing(true);
    }

    public void ChangeMode(ControlCenter.GameMode mode)
    {
        switch (mode)
        {
            case ControlCenter.GameMode.Reality:
                ToggleMainCameraPostProcessing(false);
                break;
            case ControlCenter.GameMode.Thought:
                ToggleMainCameraPostProcessing(true);
                break;
            case ControlCenter.GameMode.Thought_Draw:
                break;
        }
    }
}
