using Cinemachine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public DialogSystem dialogSystem;

    [Header("어빌리티 관련")]
    public UpgradeManager upgradeManager;
    public SaveManager saveManager;
    public PlayerAttributes playerAttributes;

    [Header("씬 전환 및 플레이어 관련")]
    public RoomManager roomManager;
    public GameObject entrance;
    public GameObject player;
    public Animator playeranimator;


    public CinemachineVirtualCamera cinemachineCamera;

    public bool isGameOver = false;
    public bool isGameClear = false;
    public bool isDungeonEntryComplete = false;
    public bool isPanelActive = false;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playeranimator = player.GetComponent<Animator>();
        saveManager.LoadInitialAttributes(playerAttributes);
        ApplyUpgrades();
    }

    private void ApplyUpgrades()
    {
        upgradeManager.UpgradeAttackPower(0); // 기존 값 적용
        upgradeManager.UpgradeAttackSpeed(0); // 기존 값 적용
        upgradeManager.UpgradeMoveSpeed(0); // 기존 값 적용
        upgradeManager.UpgradeMaxHealth(0); // 기존 값 적용
    }

    public void SetPlayerInputActive(bool isActive)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = isActive;
        }
        PlayerEquipment playerEquipment = player.GetComponent<PlayerEquipment>();
        if (playerEquipment != null)
        {
            playerEquipment.enabled = isActive;
        }
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        if (playerRigidbody != null)
        {
            if (!isActive)
            {
                playerRigidbody.velocity = Vector2.zero;
            }
        }
    }

    private void Update()
    {
        cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (dialogSystem != null && dialogSystem.IsDialogCompleted())
        {
            RequestSceneLoad("VillageScene");
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            RequestSceneLoad("VillageScene");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            RequestSceneLoad("DungeonScene");
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            RequestSceneLoad("BossScene");
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            RequestSceneLoad("DungeonScene2");
        }
        if (isGameClear && Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
        if (isGameOver)
        {
            StartCoroutine(DieAniator());
            isGameOver = false;
        }
        if (SceneManager.GetActiveScene().name.StartsWith("DungeonScene") && Input.GetKeyDown(KeyCode.Tab))
        {
            isPanelActive = !isPanelActive;

          
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 전환 메서드
    public void RequestSceneLoad(string sceneName)
    {
        UIManager.Instance.StartTransition(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "OpeningScene":
                if(!player.activeSelf)
                {
                    player.SetActive(true);
                }
                dialogSystem = FindObjectOfType<DialogSystem>();
                SoundManager.Instance.PlayBGM(SoundManager.Instance.UI_Audio[0]);
                InitializeCommon();
                break;
            case "VillageScene":
                SoundManager.Instance.PlayBGM(SoundManager.Instance.UI_Audio[0]);
                player.gameObject.SetActive(true);
                InitializeCommon();
                UIManager.Instance.gameFailed.SetActive(false);
                isGameOver = false;
                break;
            case "DungeonScene":
                roomManager = FindObjectOfType<RoomManager>();
                StartCoroutine(WaitForDungeonGeneration());
                break;
            case "BossScene":
                SoundManager.Instance.StopBGM();
                InitializeCommon();
                break;
            case "DungeonScene2":
                SoundManager.Instance.PlayBGM(SoundManager.Instance.UI_Audio[7]);
                InitializeCommon();
                StartCoroutine(HandleDungeonScene2Transition());
                break;
        }
    }
    private IEnumerator DieAniator()
    {
        SoundManager.Instance.StopBGM();
        playeranimator.SetBool("Die", true);
        Rigidbody2D playerRigidbody2D = player.GetComponent<Rigidbody2D>();
        playerRigidbody2D.velocity = Vector3.zero;
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        PlayerEquipment playerEquipment = player.GetComponent<PlayerEquipment>();
        playerInput.enabled = false;
        playerEquipment.enabled = false;

        yield return new WaitForSeconds(2f);

        playerInput.enabled = true;
        playerEquipment.enabled = true;
        player.gameObject.SetActive(false);
        Vector3 cameraPosition = cinemachineCamera.transform.position;
        UIManager.Instance.gameFailed.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, UIManager.Instance.gameFailed.transform.position.z);
        UIManager.Instance.gameFailed.SetActive(true);

        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.UI_Audio[2]);

        yield return new WaitForSeconds(3f);

        RequestSceneLoad("VillageScene");
    }

    private IEnumerator WaitForDungeonGeneration()
    {
        while (!roomManager.generationComplete)
        {
            yield return null;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.SetRoomManager(roomManager);
        SoundManager.Instance.PlayBGM(SoundManager.Instance.UI_Audio[1]);

        InitializeCommon();
    }

    private void InitializeCommon()
    {
        InitializeCamera();
        InitializePlayer();
        UIManager.Instance.EndTransition();
    }

    // 플레이어 추적 카메라
    private void InitializeCamera()
    {
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null && player != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }

    // 플레이어의 위치와 상태를 초기화하는 메서드
    private void InitializePlayer()
    {
        if (player != null)
        {
            SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (!playerController.enabled || !playerSpriteRenderer.enabled)
            {
                playerSpriteRenderer.enabled = true;
                playerController.enabled = true;

                GameObject dustFX = player.transform.GetChild(3).gameObject;
                dustFX.SetActive(true);

                GameObject thand = player.transform.GetChild(0).gameObject;
                thand.SetActive(true);
            }

            if (entrance == null)
            {
                entrance = GameObject.FindGameObjectWithTag("Start");
            }
            player.transform.position = entrance.transform.position;
            player.transform.rotation = Quaternion.identity;
            player.transform.localScale = Vector3.one;
        }
    }

    // 플레이어의 던전 입장 이벤트 메서드
    public IEnumerator HandlePlayerEntry(GameObject player, float scaleDownDuration, Vector3 targetScale)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.enabled = false;

        GameObject screamer = player.transform.GetChild(2).gameObject;
        screamer.SetActive(true);
        GameObject dustFX = player.transform.GetChild(3).gameObject;
        dustFX.SetActive(false);

        yield return new WaitForSeconds(1f);
        screamer.SetActive(false);

        yield return new WaitForSeconds(1f);
        StartCoroutine(ScaleDownPlayer(player, scaleDownDuration, targetScale));

        yield return new WaitForSeconds(0.5f);
        GameObject thand = player.transform.GetChild(0).gameObject;
        thand.SetActive(false);
        SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        playerSpriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.5f);
        OnTransitionComplete();

        if (!isDungeonEntryComplete)
        {
            RequestSceneLoad("DungeonScene");
        }
        else
        {
            RequestSceneLoad("DungeonScene2");
        }
    }
    private IEnumerator HandleDungeonScene2Transition()
    {
        GameObject screamer = player.transform.GetChild(2).gameObject;
        screamer.SetActive(true);

        yield return new WaitForSeconds(3f);

        GameObject transitionPanel = UIManager.Instance.transitionPanel;

        UIManager.Instance.StartTransition(() =>
        {
            StartCoroutine(ActivateTextMeshProObjectsWithDelay(transitionPanel, 1f));
            isGameClear = true;
        });
    }

    private IEnumerator ActivateTextMeshProObjectsWithDelay(GameObject transitionPanel, float delay)
    {
        yield return new WaitForSeconds(delay);

        TextMeshProUGUI textMeshProObject = transitionPanel.GetComponentInChildren<TextMeshProUGUI>(true);

        if (textMeshProObject != null)
        {
            textMeshProObject.gameObject.SetActive(true);
        }
    }

    private IEnumerator ScaleDownPlayer(GameObject player, float scaleDownDuration, Vector3 targetScale)
    {
        Vector3 initialScale = player.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < scaleDownDuration)
        {
            player.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scaleDownDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        player.transform.localScale = targetScale;
    }
    private void OnTransitionComplete()
    {
        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.SaveData();
        }
    }

    // 게임 종료
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    public void LoadLobbyScene()
    {
        Cleanup();

        SceneManager.LoadScene("LobbyScene");
    }

    private void Cleanup()
    {
        if (player != null)
        {
            player.SetActive(false);
        }
    }
}
