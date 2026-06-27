using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject transitionPanel;
    public GameObject gameFailed;
    public GameObject gameSuccess;

    [Header("옵션창 담당")]
    public GameObject pausePanel;
    public GameObject optionPanel;
    public GameObject exitImage;

    public CanvasGroup canvasGroup;

    private static UIManager uIManager;
    public static UIManager Instance
    {
        get
        {
            if (uIManager == null)
            {
                uIManager = FindObjectOfType<UIManager>();
                if (uIManager == null)
                {
                    GameObject obj = new GameObject(typeof(UIManager).Name);
                    uIManager = obj.AddComponent<UIManager>();
                }
            }
            return uIManager;
        }
    }

    private void Awake()
    {
        if (uIManager == null)
        {
            uIManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (transitionPanel != null)
        {
            canvasGroup = transitionPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = transitionPanel.AddComponent<CanvasGroup>();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausePanel();
        }
    }

    public void TogglePausePanel()
    {
        bool isActive = pausePanel.activeSelf;
        pausePanel.SetActive(!isActive);
        Time.timeScale = isActive ? 1 : 0;  // 게임 일시정지
    }

    public void ToggleOptionPanel()
    {
        bool isActive = optionPanel.activeSelf;
        optionPanel.SetActive(!isActive);
    }

    public void ToggleExitImage()
    {
        bool isActive = exitImage.activeSelf;
        exitImage.SetActive(!isActive);
    }

    public void LoadLobbyScene()
    {
        Time.timeScale = 1;  // 게임 시간 재설정
        TogglePausePanel();
        ToggleExitImage();
        GameManager.Instance.LoadLobbyScene();
    }
    // 씬 전환 시 패널 효과
    public void StartTransition(System.Action onComplete)
    {
        StartCoroutine(FadeIn(onComplete));
    }
    private IEnumerator FadeIn(System.Action onComplete)
    {
        transitionPanel.SetActive(true);
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = 1;

        // 전환 완료 후 콜백 호출
        onComplete?.Invoke();
    }

    // 씬 전환 후 패널 효과
    public void EndTransition()
    {
        StartCoroutine(FadeOut());
    }
    private IEnumerator FadeOut()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        transitionPanel.SetActive(false);
    }
}
