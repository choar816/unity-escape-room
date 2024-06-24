using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Player : MonoBehaviour
{
    private static Player instance = null;
    public static Player Instance
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

    public class Stat
    {
        public float walkSpeed;
        public float runSpeed;
        public float jumpForce;
        public bool canMove;
        public bool isWalking;
        public bool isRunning;
        public bool isCrawling;
        public bool isJumping;
        public bool isHatOn;
        public Platform.PlatformType isOn;
        public bool canEnterDoor;
        public int nearestDoorNumber;
    }

    public Stat m_Stat;
    public GameObject playerSpine;
    public GameObject interactionKey;
    SkeletonAnimation anim;
    [SerializeField] Camera CameraBG;

    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    float rayScope = 1f;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        Init();
    }

    void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        anim = playerSpine.GetComponent<SkeletonAnimation>();
        interactionKey.SetActive(false);

        m_Stat = new Stat();
        m_Stat.walkSpeed = 10;
        m_Stat.runSpeed = 20;
        m_Stat.jumpForce = 400;
        m_Stat.canMove = true;
        m_Stat.isWalking = false;
        m_Stat.isRunning = false;
        m_Stat.isCrawling = false;
        m_Stat.isJumping = false;
        m_Stat.isHatOn = false;
        m_Stat.canEnterDoor = false;
        m_Stat.nearestDoorNumber = -1;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
        UpdateJump();
        // UpdateCrawl();
        UpdateAnimation();
    }

    void UpdateMove()
    {
        if (!m_Stat.canMove)
            return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector3 movement = Vector3.right * moveHorizontal;
        // 왼쪽, 오른쪽에 따라 플레이어 이미지 반전
        if (moveHorizontal > 0)
        {
            // anim.initialFlipX = false;
            playerSpine.transform.rotation = Quaternion.identity;
        }
        else if (moveHorizontal < 0)
        {
            // anim.initialFlipX = true;
            playerSpine.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        // 서있는지 기고 있는지에 따라 rayScope, rayPosition 다르게 설정
        List<Vector3> rayPositions = new List<Vector3>();
        if (m_Stat.isCrawling)
        {
            // rayScope = 2f;
            rayPositions.Add(transform.position);
            rayPositions.Add(transform.position - Vector3.up * capsuleCollider.size.x * 0.4f);
        }
        else
        {
            rayPositions.Add(transform.position + Vector3.up * capsuleCollider.size.y * 0.4f);
            // rayPositions.Add(transform.position + Vector3.up * capsuleCollider.size.y * 0.4f + Vector3.down * 0.04f);
            rayPositions.Add(transform.position - Vector3.up * capsuleCollider.size.y * 0.4f);
        }

        foreach (Vector3 pos in rayPositions)
        {
#if UNITY_EDITOR
            Debug.DrawRay(pos, movement * rayScope, Color.green);
#endif
            RaycastHit2D hit = Physics2D.Raycast(pos, movement, rayScope, LayerMask.GetMask("Map"));
            if (hit.collider != null && (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Platform")))
            {
                // Debug.Log("hit obstacle");
                movement = Vector3.zero;
            }
        }

        // Stat 설정
        if (movement == Vector3.zero)
        {
            m_Stat.isWalking = false;
            m_Stat.isRunning = false;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                m_Stat.isWalking = false;
                m_Stat.isRunning = true;
            }
            else
            {
                m_Stat.isWalking = true;
                m_Stat.isRunning = false;
            }
        }

        // 움직임
        if (m_Stat.isWalking)
        {
            transform.Translate(movement * m_Stat.walkSpeed * Time.deltaTime);
        }
        else if (m_Stat.isRunning)
        {
            transform.Translate(movement * m_Stat.runSpeed * Time.deltaTime);
        }
    }

    void MoveReverse()
    {

    }

    void UpdateJump()
    {
        if (!m_Stat.canMove)
            return;

        if (rb.velocity.y == 0)
        {
            m_Stat.isJumping = false;
        }
        else
        {
            m_Stat.isJumping = true;
        }

        if (m_Stat.isJumping)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * m_Stat.jumpForce, ForceMode2D.Force);
        }
    }

    void UpdateCrawl()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // 기고 있는 경우 서기로 전환
            if (m_Stat.isCrawling)
            {
                m_Stat.isCrawling = false;
                capsuleCollider.size = new Vector2(84, 252);
                capsuleCollider.offset = new Vector2(0, 18);
                // capsuleCollider.direction = CapsuleDirection2D.Vertical;
            }
            // 서있는 경우 기기로 전환
            else
            {
                m_Stat.isCrawling = true;
                capsuleCollider.size = new Vector2(84, 152);
                capsuleCollider.offset = new Vector2(0, -30);
                // capsuleCollider.direction = CapsuleDirection2D.Horizontal;
                anim.AnimationState.SetAnimation(0, "roll", false);
            }
        }
    }

    void UpdateAnimation()
    {
        string animationName = string.Empty;
        if (!m_Stat.canMove)
            return;

        if (m_Stat.isJumping)
        {
            if (rb.velocity.y > 0 && !anim.AnimationName.StartsWith("jump_1_up"))
            {
                animationName = "jump_1_up";
            }
            else if (rb.velocity.y < 0 && !anim.AnimationName.StartsWith("jump_3_down"))
            {
                animationName = "jump_3_down";
            }
        }
        else if (m_Stat.isRunning && !anim.AnimationName.StartsWith("run_0"))
        {
            animationName = "run_0";
        }
        else if (m_Stat.isWalking && !anim.AnimationName.StartsWith("walking"))
        {
            animationName = "walking";
        }
        else if (!m_Stat.isJumping && !m_Stat.isRunning && !m_Stat.isWalking && !anim.AnimationName.StartsWith("idle_0"))
        {
            animationName = "idle_0";
        }

        if (animationName == string.Empty)
            return;
            
        if (!m_Stat.isHatOn)
        {
            animationName += "_nofedora";
        }
        anim.AnimationState.SetAnimation(0, animationName, true);
    }

    public void ShowInteractionKey()
    {
        interactionKey.SetActive(true);
    }

    public void HideInteractionKey()
    {
        interactionKey.SetActive(false);
    }

    public void DisableMove()
    {
        m_Stat.canMove = false;
        m_Stat.isWalking = false;
        if (m_Stat.isHatOn)
        {
            anim.AnimationState.SetAnimation(0, "idle_0", true);
        }
        else
        {
            anim.AnimationState.SetAnimation(0, "idle_0_nofedora", true);
        }
    }

    public void EnableMove()
    {
        m_Stat.canMove = true;
    }
}
