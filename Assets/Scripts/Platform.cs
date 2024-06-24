using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public enum PlatformType
    {
        None,
        Default,
        Ice,
        Bouncy,
        Reverse,
        Sticky
    }

    BoxCollider2D platformCollider;
    SpriteRenderer spriteRenderer;
    PhysicsMaterial2D newPhysicsMaterial;
    public Material blueMaterial;
    public Material blackMaterial;
    public Material yellowMaterial;
    public Material pinkMaterial;
    public PlatformType type;
    public float bounciness;
    public float friction;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        SetPhysicsMaterial();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Init()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        type = PlatformType.Default;
    }

    void SetPhysicsMaterial()
    {
        newPhysicsMaterial = new PhysicsMaterial2D();

        if (gameObject.CompareTag("Platform_Ice"))
        {
            type = PlatformType.Ice;
            bounciness = 0.3f;
            friction = 0;
            spriteRenderer.material = blueMaterial;
        }
        else if (gameObject.CompareTag("Platform_Bouncy"))
        {
            type = PlatformType.Bouncy;
            bounciness = 1;
            friction = 0.5f;
            spriteRenderer.material = yellowMaterial;
        }
        else if (gameObject.CompareTag("Platform_Reverse"))
        {
            type = PlatformType.Reverse;
            bounciness = 0.5f;
            friction = 1;
            spriteRenderer.material = pinkMaterial;
            // wip
        }
        else if (gameObject.CompareTag("Platform_Sticky"))
        {
            type = PlatformType.Sticky;
            bounciness = 0;
            friction = 1;
            spriteRenderer.material = blackMaterial;
        }

        platformCollider.sharedMaterial = newPhysicsMaterial;
    }
}
