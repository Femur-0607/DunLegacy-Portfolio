using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    public Boss curBoss;
    public Collider2D bossTriggerCollider;
    
    public Canvas bossCanvas;

    public bool firstCheck = false;
    public bool bossStart = false;
    public bool bossSceneStart = false;
    public int name_Count = 0;

    [SerializeField]
    private Image[] ScreenBox;
    [SerializeField]
    private SpriteRenderer[] bossSpriteRenderers;
    [SerializeField]
    private Text[] BossName_Text;
    [SerializeField]
    private Image End_Screen;
    [SerializeField]
    private CinemachineVirtualCamera cinemachineCamera;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Transform bossTransform;

    public event Action OnBossDeath;
    public GameObject clearSealStone;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        bossTriggerCollider = GetComponent<Collider2D>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;
        OnBossDeath += OnBossDeathHandler;
    }
    void Update()
    {
        Screen_Effect();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossStart = true;
            bossTriggerCollider.enabled = false;
        }
    }

    // 보스 등장시 이펙트 효과
    public void Screen_Effect()
    {
        if (!bossStart)
            return;

        if (!firstCheck)
        {
            SoundManager.Instance.bgmSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[0]);
            firstCheck = true;
            bossSceneStart = true;

            cinemachineCamera.Follow = bossTransform;
            cinemachineCamera.LookAt = bossTransform;
        }
        // 상하 패널 효과
        foreach (Image img in ScreenBox)
        {
            Color alpha = img.color;
            alpha.a += Time.deltaTime * 1f;
            img.color = alpha;

            if (img.color.a >= 1)
            {
                bossStart = false;
            }
        }

        foreach (SpriteRenderer sprite in bossSpriteRenderers)
        {
            Color alpha = sprite.color;
            alpha.a += Time.deltaTime * 1f;
            sprite.color = alpha;
        }


        if (!bossStart)
        {
            SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[1]);
            StartCoroutine(Boss_Name_Coroutine());
        }
    }

    public IEnumerator Boss_Name_Coroutine()
    {
        GameManager.Instance.SetPlayerInputActive(false);

        if (name_Count < 2)
        {
            while (true)
            {
                Color alpha = BossName_Text[name_Count].color;
                alpha.a += Time.deltaTime * 1f;
                BossName_Text[name_Count].color = alpha;

                if (BossName_Text[name_Count].color.a >= 1)
                {
                    name_Count++;
                    break;
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.25f);

        if (name_Count < 2)
        {
            StartCoroutine(Boss_Name_Coroutine());
        }
        else
        {
            yield return new WaitForSeconds(1f);

            bool endflag = false;
            while (true)
            {
                yield return null;

                for (int i = 0; i < 2; i++)
                {
                    Color alphaA = ScreenBox[i].color;
                    Color alphaB = BossName_Text[i].color;

                    alphaA.a -= Time.deltaTime * 1f;
                    alphaB.a -= Time.deltaTime * 1f;

                    ScreenBox[i].color = alphaA;
                    BossName_Text[i].color = alphaB;

                    if (alphaA.a <= 0)
                    {
                        endflag = true;
                    }
                }

                if (endflag)
                {
                    bossCanvas.gameObject.SetActive(true);
                    StartCoroutine(curBoss.PatternCoroutine());
                    cinemachineCamera.Follow = playerTransform;
                    cinemachineCamera.LookAt = playerTransform;
                    GameManager.Instance.SetPlayerInputActive(true);
                    break;
                }
            }
        }
    }

    // 보스 사망시 스크린 효과
    public IEnumerator End_Screen_Coroutine()
    {
        Color al = End_Screen.color;
        al.a = 1;
        End_Screen.color = al;

        while (true)
        {
            Color alpha = End_Screen.color;
            alpha.a -= Time.deltaTime * 0.5f;
            End_Screen.color = alpha;

            if (End_Screen.color.a <= 0)
            {
                break;
            }
            yield return null;
        }
    }

    public void TriggerOnBossDeath()
    {
        if (OnBossDeath != null)
        {
            OnBossDeath.Invoke();
        }
    }

    public void OnBossDeathHandler()
    {
        Animator targetAnimator = clearSealStone.GetComponent<Animator>();
        Collider2D targetCollider = clearSealStone.GetComponent<Collider2D>();

        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger("SealOpen");
        }
        if (targetCollider != null)
        {
            targetCollider.isTrigger = true;
        }
    }
}
