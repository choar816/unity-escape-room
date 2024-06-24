using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device : MonoBehaviour
{
    public bool canInteract;
    bool hasBeenRecognized;
    BoxCollider2D collider;
    SpriteRenderer sr;

    void Awake()
    {
        canInteract = false;
        hasBeenRecognized = false;
        collider = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        collider.enabled = true;
    }

    public void EnableInteract()
    {
        // collider.enabled = true;
        canInteract = true;
    }

    public void DisableInteract()
    {
        // collider.enabled = false;
        canInteract = false;
    }

    public void TurnOnHighlight()
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
    }

    public void TurnOffHighlight()
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenRecognized)
        {
            // 맨 처음으로 인식되면 할 일 추가 (컷씬 진행 등)
            hasBeenRecognized = true;
        }

        if (canInteract && other.gameObject == Player.Instance.gameObject)
        {
            DeviceManager.Instance.SetInteractableDevice(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (canInteract && other.gameObject == Player.Instance.gameObject)
        {
            DeviceManager.Instance.UnsetInteractableDevice(this);
        }
    }

    public void TryInteract()
    {
        if (canInteract)
        {
            Debug.Log($"TryInteract: {gameObject.name}");
            DeviceManager.Instance.Interact(this);
        }
    }
}
