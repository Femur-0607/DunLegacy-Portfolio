using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public PlayerSettings playerSettings;

    private Rigidbody2D playerRigidbody;
    private Collider2D playerCollider;
    private Animator playerAnimator;

    private GameObject dustFX;
    private GameObject jumpFX;
    private GameObject currentOneWayPlatform;

    [SerializeField] private RoomManager roomManager;

    private bool isGrounded;
    private bool isWalking = false;
    private float footstepDelay = 0.5f;
    private float nextFootstepTime = 0f;

    private INPC currentNPC;

    [Header("레이어 담당")]
    public LayerMask groundLayer;
    public LayerMask npcLayer;

    private static PlayerController playerController;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if(playerAnimator.GetBool("Die") == true)
        {
            playerAnimator.SetBool("Die", false);
        }
    }
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        dustFX = transform.GetChild(3).gameObject;
        jumpFX = transform.GetChild(4).gameObject;
        dustFX.SetActive(false);
        jumpFX.SetActive(false);
    }
    private void FixedUpdate()
    {
        isGrounded = CheckGrounded();
        HandleJump();
        ApplyGravity();
        Move();
        AdjustCameraForConversation();
    }

    // 땅 체크 불타입
    bool CheckGrounded()
    {
        Vector2 raycastOrigin = new Vector2(transform.position.x, playerCollider.bounds.min.y - 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, playerSettings.raycastBeam, groundLayer);
        Debug.DrawRay(raycastOrigin, Vector2.down * playerSettings.raycastBeam, Color.green);
        return hit.collider != null;
    }

    // 플레이어 움직임 관련
    void Move()
    {
        Vector2 moveDirection = new Vector2(PlayerInput.move, playerRigidbody.velocity.y);
        Vector2 moveDistance = moveDirection * playerSettings.moveSpeed;
        playerRigidbody.velocity = new Vector2(moveDistance.x, playerRigidbody.velocity.y);

        if (PlayerInput.move > 0)
            transform.localScale = new Vector2(1, 1);
        else if (PlayerInput.move < 0)
            transform.localScale = new Vector2(-1, 1);

        isWalking = PlayerInput.move != 0;
        playerAnimator.SetFloat("Move", PlayerInput.move);

        dustFX.SetActive(isWalking && isGrounded);

        if (SoundManager.Instance != null && SoundManager.Instance.effectSource != null && SoundManager.Instance.Player_Audio != null && SoundManager.Instance.Player_Audio.Length > 0)
        {
            if (isWalking && Time.time >= nextFootstepTime)
            {
                SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.Player_Audio[0]);
                nextFootstepTime = Time.time + footstepDelay;
            }
        }
    }

    // 플레이어 점프 관련
    void HandleJump()
    {
        playerAnimator.SetBool("Jump", true);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && playerSettings.jumpCount > 0)
        {
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) && currentOneWayPlatform != null)
            {
                StartCoroutine(DisableCollision());
                return;
            }
            Jump();
            StartCoroutine(JumpCooldown());
        }
        playerAnimator.SetBool("Jump", false);
    }
    // 아래 점프
    private IEnumerator DisableCollision()
    {
        if (currentOneWayPlatform != null)
        {
            Collider2D platformCollider = currentOneWayPlatform.GetComponent<Collider2D>();
            if (platformCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, platformCollider);
                yield return new WaitForSeconds(0.5f);
                Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
            }
        }
    }
    // 점프
    void Jump()
    {
        playerSettings.jumpCount--;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(0, playerSettings.jumpForce), ForceMode2D.Impulse);
        jumpFX.SetActive(isGrounded);
    }
    IEnumerator JumpCooldown()
    {
        yield return new WaitUntil(() => isGrounded);
        isGrounded = false;
        playerSettings.jumpCount++;
        yield return new WaitForSeconds(0.5f);
        jumpFX.SetActive(false);
    }

    // 중력 관련
    void ApplyGravity()
    {
        if (!isGrounded)
        {
            playerRigidbody.velocity += Vector2.down * playerSettings.gravityForce * Time.deltaTime;
        }
    }

    // NPC 대화 관련
    private void AdjustCameraForConversation()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentNPC != null)
        {
            currentNPC.Talk();
        }
        else
        {
            currentNPC = null;
        }
    }

    // 문 태그랑 충돌했을때 트리거 엔터 밑의 메서드 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            StartCoroutine(HandleDoorCollision(collision));
        }
    }
    // 방 전환 메서드
    private IEnumerator HandleDoorCollision(Collider2D collision)
    {
        bool transitionCompleted = false;

        UIManager.Instance.StartTransition(() =>
        {
            transitionCompleted = true;
        });

        while (!transitionCompleted)
        {
            yield return null;
        }

        // 충돌한 문이 방의 어느곳인지 저장하고 이동 메서드 작동
        Collider2D[] childColliders = playerCollider.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D childCollider in childColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, childCollider);
        }

        Room currentRoom = roomManager != null ? roomManager.currentRoom : null;

        Vector2Int direction = Vector2Int.zero;

        foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            if (collision.transform.parent.gameObject == currentRoom.GetDoor(dir)?.gameObject && currentRoom != null)
            {
                direction = dir;
                if (currentRoom.gameObject.name == "Room-1")
                {
                    direction = Vector2Int.right;
                }
                break;
            }
        }

        if (direction != Vector2Int.zero)
        {
            MoveToAdjacentRoom(direction);
        }

        yield return new WaitForSeconds(0.5f);

        UIManager.Instance.EndTransition();
    }
    // 씬 전환 후 변수 게임매니저에서 가져오기
    public void SetRoomManager(RoomManager manager)
    {
        roomManager = manager;
    }
    // 주어진 방향에 따라 플레이어를 인접한 방으로 이동시킴
    private void MoveToAdjacentRoom(Vector2Int direction)
    {
        Vector2Int newRoomIndex = roomManager.currentRoom.RoomIndex + direction; // 이동할 방의 인덱스 계산

        Room newRoom = roomManager.GetRoomScriptAt(newRoomIndex); // 해당 인덱스의 방 가져오기
        if (newRoom != null)
        {
            Vector3 newPosition = Vector3.zero;

            // 방향에 따라 새로운 방의 문 위치를 계산
            if (newRoom != null)
            {
                Vector3 targetPosition = GetNewPositionBasedOnDirection(newRoom, direction);
                transform.position = targetPosition;
                roomManager.currentRoom = newRoom;
                // 몬스터 스폰 구문
                DungeonManager dungeonManager = newRoom.GetComponent<DungeonManager>();
                if (dungeonManager != null)
                {
                    dungeonManager.enabled = true;
                }
                roomManager.CollriderChange();
            }
        }
    }
    // 주어진 방향에 따라 플레이어의 새로운 위치를 계산  
    private Vector3 GetNewPositionBasedOnDirection(Room newRoom, Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            return newRoom.doorEntranceDown.position;
        }
        else if (direction == Vector2Int.down)
        {
            return newRoom.doorEntranceUp.position;
        }
        else if (direction == Vector2Int.left)
        {
            return newRoom.doorEntranceRight.position;
        }
        else if (direction == Vector2Int.right)
        {
            return newRoom.doorEntranceLeft.position;
        }

        return Vector3.zero;
    }

    // 바닥 체크 및 NPC 대화 시작할때
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("wall"))
        {
            TilemapCollider2D tilemapCollider = collision.GetComponent<TilemapCollider2D>();

            if (tilemapCollider != null)
            {
                DoorController doorController = FindObjectOfType<DoorController>();
                doorController.currentTilemapCollider = tilemapCollider;
            }
        }
        if ((npcLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            INPC npc = collision.GetComponent<INPC>();

            if (npc != null)
            {
                currentNPC = npc;
            }
        }
    }

    // NPC 대화 나갈때
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentNPC != null && collision.gameObject == ((MonoBehaviour)currentNPC).gameObject)
        {
            currentNPC = null;
        }
    }

    // 하단 점프 시 플랫폼 체크
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    // 하단 점프 시 플랫폼 초기화
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform") && currentOneWayPlatform == collision.gameObject)
        {
            currentOneWayPlatform = null;
        }
    }
}
