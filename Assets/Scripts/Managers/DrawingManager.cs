using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public GameObject CameraObject;  
    public GameObject PenPointer;
    public Camera Camera;
    public LayerMask Drawing_Layers;
    public GameObject CopiedSpritePrefab;
    public GameObject Drawboard;

    private List<Vector2> points;
    private EdgeCollider2D edgeCollider;

    // TODO : 그림 클릭 및 편집에 작업
    private List<GameObject> drawingObjects;
    private GameObject currentlyActiveDrawingObject;
    private int currentlyActiveDrawingObjectIndex = 0;

    private static DrawingManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        points = new List<Vector2>();
        drawingObjects = new List<GameObject>();
    }

    public static DrawingManager Instance
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

    private void Start()
    {
        // ActivateNextBoard();
    }

    private void Update()
    {
        // Convert mouse coordinates to world coordinates
        Vector2 mouse_world_position = Camera.ScreenToWorldPoint(Input.mousePosition);
        //Pen_Pointer.transform.position = mouse_world_position;

        // Check if the current mouse position overlaps our image
        Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
        mouse_world_position = new Vector2(mouse_world_position.x / this.transform.localScale.x, mouse_world_position.y / this.transform.localScale.y);

        // This line:
        //mouse_world_position = new Vector2(mouse_world_position.x / this.transform.localScale.x, mouse_world_position.y / this.transform.localScale.y);

        // Should be removed.


        if (hit != null && hit.transform != null)
        {
            if (Drawboard != null)
            {
                if (Input.GetMouseButtonDown(0))
                {

                    //points = new List<Vector2>();
                    //currentObject = Instantiate(physicsObjectPrefab);
                    AddPointToCurrentObject();
                }
                else if (Input.GetMouseButton(0))
                {
                    AddPointToCurrentObject();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    //// 마우스 버튼을 놓았을 때 LineRenderer를 비활성화
                    //if (lineRenderer)
                    //{
                    //    lineRenderer.enabled = false;
                    //}
                    EdgeCollider2D originalCollider = Drawboard.GetComponent<EdgeCollider2D>();
                    //originalCollider.points.Length;
                    Debug.LogWarning($"No board available at index {originalCollider.points.Length}.");
                }
            }
        }

        else
        {
            //// We're not over our destination texture
            //previous_drag_position = Vector2.zero;
            //if (!mouse_was_previously_held_down)
            //{
            //    // This is a new drag where the user is left clicking off the canvas
            //    // Ensure no drawing happens until a new drag is started
            //    no_drawing_on_current_drag = true;
            //}
        }
    }
    
    private void AddPointToCurrentObject()
    {
        Vector2 mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
        float halfThickness = 0.02f;  // LineRenderer의 두께의 절반

        if (points.Count == 0 || Vector2.Distance(mousePosition, points[points.Count - 1]) > .1f)
        {
            points.Add(mousePosition);
            LineRenderer lineRenderer = Drawboard.GetComponent<LineRenderer>();

            // 선 크기 설정 TODO : 드로우 선이랑 크기 맞춰야 함 추후 public 으로 에디터에서 변경 가능 하게끔 해야함
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPosition(points.Count - 1, mousePosition);

            edgeCollider = Drawboard.GetComponent<EdgeCollider2D>();

            // LineRenderer의 포인트를 로컬 좌표계로 변환하여 EdgeCollider2D의 포인트로 설정
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);
            Vector2[] colliderPoints = new Vector2[positions.Length * 2];  // 두 개의 EdgeCollider2D 때문에 두 배

            for (int i = 0; i < positions.Length; i++)
            {
                Vector2 normal;
                if (i == 0)
                {
                    if (positions.Length > 1)
                        normal = GetNormal(positions[i + 1] - positions[i]);
                    else
                        continue;  // 첫 번째 포인트이므로 다음 반복으로 넘어갑니다.
                }
                else
                {
                    normal = GetNormal(positions[i] - positions[i - 1]);
                }
                Vector2 offsetPoint = (Vector2)positions[i] + normal * halfThickness;
                Vector2 inverseOffsetPoint = (Vector2)positions[i] - normal * halfThickness;
                colliderPoints[i] = Drawboard.transform.InverseTransformPoint(offsetPoint);
                colliderPoints[colliderPoints.Length - 1 - i] = Drawboard.transform.InverseTransformPoint(inverseOffsetPoint);
            }

            edgeCollider.points = colliderPoints;
        }
    }

    // 벡터의 수직 벡터를 반환하는 도우미 함수
    Vector2 GetNormal(Vector2 vector)
    {
        return new Vector2(-vector.y, vector.x).normalized;
    }

    public void ResetCurrentCanvas()
    {
        if (!RecognizeItemWithAIBarracuda.Instance.isActive && Drawboard)
        {
            Drawable drawable = Drawboard.GetComponent<Drawable>();

            drawable.ResetCanvas();
            points.Clear();
        }       
    }

    public void TurningIntoObject()
    {
        // 원본 UI Sprite
        SpriteRenderer spriteRenderer = Drawboard.GetComponent<SpriteRenderer>();
        Sprite originalSprite = spriteRenderer.sprite;
        Texture2D copiedTexture = new Texture2D((int)originalSprite.rect.width, (int)originalSprite.rect.height);

        Color[] originalColors = originalSprite.texture.GetPixels((int)originalSprite.rect.x, (int)originalSprite.rect.y, (int)originalSprite.rect.width, (int)originalSprite.rect.height);
        Color[] copiedColors = new Color[originalColors.Length];

        for (int i = 0; i < originalColors.Length; i++)
        {
            if (originalColors[i] == Color.black)
            {
                copiedColors[i] = new Color(0, 0, 0, 0); // 투명한 색상
            }
            else
            {
                //copiedColors[i] = originalColors[i];
                copiedColors[i] = Color.yellow;
            }
        }

        copiedTexture.SetPixels(copiedColors);
        copiedTexture.Apply();

        // 테두리의 경계 상자 계산
        int minX = copiedTexture.width, minY = copiedTexture.height, maxX = 0, maxY = 0;
        for (int y = 0; y < copiedTexture.height; y++)
        {
            for (int x = 0; x < copiedTexture.width; x++)
            {
                Color pixelColor = copiedTexture.GetPixel(x, y);
                if (pixelColor == Color.yellow)
                {
                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                    maxX = Mathf.Max(maxX, x);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }

        Vector2 size = new Vector2(maxX - minX, maxY - minY);
        

        // 원본 Sprite의 textureRect 값으로 새로운 Sprite 생성 (피봇 조절)
        Sprite copiedSprite = Sprite.Create(
            copiedTexture,
            new Rect(minX, minY, size.x, size.y),
            new Vector2(0.5f, 0.5f)
        );

        GameObject newObject;        
        newObject = Instantiate(CopiedSpritePrefab);
        newObject.name = "DrawingObject";
        spriteRenderer = newObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = copiedSprite;
        spriteRenderer.sortingOrder = 1;
        
        // 원본 오브젝트의 Edge Collider 2D를 찾기
        EdgeCollider2D originalCollider = Drawboard.GetComponent<EdgeCollider2D>();

        if (originalCollider)
        {
            // 새 오브젝트에 Edge Collider 2D 추가
            EdgeCollider2D newCollider = newObject.GetComponent<EdgeCollider2D>();
            BoxCollider2D boxCollider = newObject.GetComponent<BoxCollider2D>();

            

            Vector2[] adjustedPoints = new Vector2[originalCollider.points.Length];

            Vector2 center = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);
            center.x -= copiedTexture.width / 2f; // 1024 / 2f; 
            center.y -= copiedTexture.height / 2f; // 512 / 2f;
            center /= Drawboard.transform.localScale / 2f; // 100f

            boxCollider.size = new Vector2(maxX - minX, maxY - minY);
            boxCollider.size /= Drawboard.transform.localScale / 2; // 100f

            for (int i = 0; i < originalCollider.points.Length; i++)
            {
                // 중심을 기준으로 점들의 위치 조정
                adjustedPoints[i] = originalCollider.points[i] - center;
            }

            newCollider.points = adjustedPoints;
            newCollider.edgeRadius = originalCollider.edgeRadius;
            newCollider.isTrigger = originalCollider.isTrigger;
            newCollider.usedByEffector = originalCollider.usedByEffector;
            newCollider.offset = originalCollider.offset;

            // 카메라 및 타겟 위치 찾기
            Camera playerCamera = GameObject.Find("Camera_Player").GetComponent<Camera>();
            Transform cameraPos = playerCamera.transform;

            // 크기 조정
            float uiDrawingScale = UIManager.Instance.UIDrawing.transform.localScale.x;
            uiDrawingScale = 1f / uiDrawingScale;
            newObject.transform.localScale *= uiDrawingScale;
            center *= uiDrawingScale;

            // 새로 생성된 오브젝트 위치 조정
            newObject.transform.position = cameraPos.position;
            newObject.transform.position += new Vector3(center.x, center.y, 0);

            // 중력 추가
            Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
            //rb.simulated = false;

            drawingObjects.Add(newObject);
            currentlyActiveDrawingObject = newObject;
            currentlyActiveDrawingObjectIndex = drawingObjects.Count - 1;
        }

        //newObject.transform.forward = playerCamera.transform.forward;


        //// 필요한 경우 다음과 같이 거리를 조정할 수 있다.
        //float desiredDistance = 3f;  // 원하는 거리를 설정
        //Vector3 newPosition = newObject.transform.position;
        //newPosition.y = newPosition.y - desiredDistance;
        //newObject.transform.position = newPosition;

        // 새로운 오브젝트의 레이어 변경하기
        // TODO : Map이 아닌 따로 구분해야 할듯
        newObject.layer = LayerMask.NameToLayer("Map");
                
        //UIManager.Instance.UIDrawing.SetActive(false);
        ControlCenter.Instance.ChangeMode(ControlCenter.GameMode.Thought);
    }

    public void ChangeMode(ControlCenter.GameMode mode)
    {
        if (!Drawboard)
            return;

        switch (mode)
        {
            case ControlCenter.GameMode.Reality:
                UIManager.Instance.HideDrawingUI();
                break;
            case ControlCenter.GameMode.Thought:
                UIManager.Instance.HideDrawingUI();
                break;
            case ControlCenter.GameMode.Thought_Draw:
                ResetCurrentCanvas();
                UIManager.Instance.ShowDrawingUI();
                break;
        }
    }

    
}
