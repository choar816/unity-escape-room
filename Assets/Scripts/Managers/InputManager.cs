using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance = null;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    public static InputManager Instance
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

    public Camera playerCamera;
    public GameObject lastClickedGameObject;
    public bool checkObjectOnly;

    void Update()
    {
        // 왼쪽 마우스 클릭
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);
            if (hits != null)
            {
                foreach (var hit in hits)
                {
                    GameObject obj = hit.collider.gameObject;

                    // Debug.Log(obj.name);
                    if (UIEventListener.Get(obj).onClick != null)
                    {
                        UIEventListener.Get(obj).onClick.Invoke(obj);
                    }
                }
            }
        }

        // ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.PopUIStack();
        }
    }
}
