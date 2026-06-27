using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Boss : HealthManager, IDamageable
{
    public MonsterData bossData;
    private Animator bossAnimator;
    protected Transform hudPos;
    public GameObject bossCanvas;

    [Header("보스 손")]
    [SerializeField]
    private GameObject[] bossHand; // 0. left hand , 1.right hand
    [SerializeField]
    private GameObject dieEffectPrefab;
    [SerializeField]
    private GameObject dieHead;
    [SerializeField]
    private GameObject dieMouth;
    [SerializeField]

    [Header("보스 투사체")]
    private GameObject bossBulletPrefab;
    [SerializeField]
    private GameObject bossSwordPrefab;
    [SerializeField]
    private Transform[] swordPos;
    [SerializeField]
    private Transform bulletPos;
    [SerializeField]
    private GameObject[] bossSword = new GameObject[6];
    [SerializeField]
    private int bulletCount = 0;


    [SerializeField]
    private GameObject[] bossOtherObj = null; // 사망시 꺼줘야하는 다른 보스 관련 오브젝트들 
    [SerializeField]
    private Transform[] dieDirection;

    [SerializeField]
    private int bossPattern = 0; // 보스 패턴
                                 // 1. 레이저 발사, 2. 레이저 3번 발사, 3. 검 날리기, 4. 나선형으로 구체 발사

    protected override void Awake()
    {
        maxHealth = (int)bossData.health;
        base.Awake();
    }
    private void Start()
    {
        bossCanvas.SetActive(false);
        Init();
        bossAnimator = GetComponent<Animator>();
        hudPos = transform.Find("Hud");
    }
    // 칼 복제생성
    void Init()
    {
        for (int i = 0; i < bossSword.Length; i++)
        {
            bossSword[i] = Instantiate(bossSwordPrefab);
            bossSword[i].transform.position = swordPos[i].position;
            bossSword[i].SetActive(false);
        }
    }

    // 3초마다 랜덤 패턴 실행
    public IEnumerator PatternCoroutine()
    {
        yield return new WaitForSeconds(2f);

        if (currentHealth > 0)
        {
            bossPattern = Random.Range(1, 5);
            Attack();
        }
    }
    private void Attack()
    {
        switch (bossPattern)
        {
            case 1:
                int randDir = Random.Range(0, 2);
                ShootLaser(randDir);
                StartCoroutine(PatternCoroutine());
                break;
            case 2:
                StartCoroutine(TripleLaser());
                break;
            case 3:
                StartCoroutine(CreateSwordCoroutine(0));
                break;
            case 4:
                StartCoroutine(ShootBulletCoroutine());
                break;
        }
    }
    // 패턴 1 레이저 발사
    void ShootLaser(int dir)
    {
        GameObject hand = bossHand[dir];

        hand.GetComponent<Animator>().SetBool("isAttack", true);
        StartCoroutine(hand.GetComponent<Boss_Hand>().bossanimator_Coroutine());
        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[3]);
    }

    // 패턴 2 레이저 3번 발사
    IEnumerator TripleLaser()
    {
        int laserDir = 0;
        int laserCount = 0;
        while (true)
        {
            if (laserCount == 3)
            {
                StartCoroutine(PatternCoroutine());
                laserCount = 0;
                break;
            }

            laserCount++;
            int dir = laserDir++ % 2;
            ShootLaser(dir);

            yield return new WaitForSeconds(1f);
        }
    }

    // 패턴 3 검 날리기
    IEnumerator CreateSwordCoroutine(int index)
    {
        while (index < 6)
        {
            yield return new WaitForSeconds(0.2f);

            CreateSword(index++);
        }

        StartCoroutine(ShootSwordCoroutine(0));
    }
    void CreateSword(int index)
    {
        Boss_Sword sword = bossSword[index].GetComponent<Boss_Sword>();

        sword.Sword_Init();
        bossSword[index].SetActive(true);
        bossSword[index].transform.position = swordPos[index].position;
        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[8]);
    }
    IEnumerator ShootSwordCoroutine(int index)
    {
        while (index < 6)
        {
            yield return new WaitForSeconds(0.2f);

            ShootSword(index++);
        }

        StartCoroutine(PatternCoroutine());

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(ReturnSwordCoroutine(0));
    }
    void ShootSword(int index)
    {
        Boss_Sword sword = bossSword[index].GetComponent<Boss_Sword>();
        sword.targetVec = sword.transform.position - sword.target.transform.position;
        sword.shootStart = true;
        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[9]);
    }
    IEnumerator ReturnSwordCoroutine(int index)
    {
        while (index < 6)
        {
            yield return new WaitForSeconds(0.2f);

            ReturnSword(index++);
        }
    }
    void ReturnSword(int index)
    {
        bossSword[index].GetComponent<Boss_Sword>().shootStart = false;
        bossSword[index].SetActive(false);
    }

    // 패턴 4 나선형 으로 구체 날리기
    IEnumerator ShootBulletCoroutine()
    {
        Vector3 newVec = new Vector3(0, 0.2f, 0);
        bossAnimator.SetBool("isAttack", true);
        transform.position += newVec;
        yield return new WaitForSeconds(0.5f);
        while (bulletCount < 50)
        {
            if (currentHealth <= 0)
                break;

            yield return new WaitForSeconds(0.2f);
            ShootBullet();
        }

        StartCoroutine(PatternCoroutine());
        transform.position -= newVec;

        bossAnimator.SetBool("isAttack", false);
        bulletCount = 0;
    }
    void ShootBullet()
    {
        int numBullets = 4; // 발사할 총알 수

        List<GameObject> bullets = new List<GameObject>();

        for (int i = 0; i < numBullets; i++)
        {
            GameObject bullet = Instantiate(bossBulletPrefab, bulletPos.position, Quaternion.identity);
            bullet.transform.rotation = transform.rotation;
            bullets.Add(bullet);

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = CalculateDirectionVector(i);
            rigid.AddForce(dirVec * 5, ForceMode2D.Impulse);

            StartCoroutine(DestroyBulletsAfterDelay(bullets));
        }

        bulletCount += numBullets;

        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[2]);
    }
    Vector2 CalculateDirectionVector(int index)
    {
        switch (index % 4)
        {
            case 0:
                return new Vector2(-Mathf.Cos(Mathf.PI * bulletCount / 50), -Mathf.Sin(Mathf.PI * bulletCount / 50));
            case 1:
                return new Vector2(Mathf.Sin(Mathf.PI * bulletCount / 50), -Mathf.Cos(Mathf.PI * bulletCount / 50));
            case 2:
                return new Vector2(Mathf.Cos(Mathf.PI * bulletCount / 50), Mathf.Sin(Mathf.PI * bulletCount / 50));
            case 3:
                return new Vector2(-Mathf.Sin(Mathf.PI * bulletCount / 50), Mathf.Cos(Mathf.PI * bulletCount / 50));
            default:
                return Vector2.zero;
        }
    }
    IEnumerator DestroyBulletsAfterDelay(List<GameObject> bullets)
    {
        yield return new WaitForSeconds(3f);

        foreach (GameObject bullet in bullets)
        {
            if (bullet != null)
            {
                Destroy(bullet);
            }
        }
    }

    // 보스 사망시
    protected override void Die()
    {
        GameManager.Instance.isDungeonEntryComplete = true;
        SoundManager.Instance.StopBGM();
        StartCoroutine(DieCoroutine());
    }
    IEnumerator DieCoroutine()
    {
        GameManager.Instance.SetPlayerInputActive(false);
        Time.timeScale = 0.5f;
        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[4]);
        StartCoroutine(BossManager.Instance.End_Screen_Coroutine());

        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f;

        int Count = 0;
        int lastCount = 1;

        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                float randPosX = Random.Range(-5f, 5f);
                float randPosY = Random.Range(-4f, 4f);

                GameObject obj = Instantiate(dieEffectPrefab);

                obj.transform.position = new Vector3(transform.position.x + randPosX,
                    transform.position.y + randPosY, 0);

                SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[5]);

                StartCoroutine(DieEffectOffCoroutine(obj));
            }
            yield return new WaitForSeconds(0.3f);

            if (++Count > 15)
                break;
        }

        foreach (GameObject obj in bossOtherObj)
            obj.SetActive(false);

        StartCoroutine(DieHeadMoveCoroutine());
        while (true)
        {
            foreach (Transform dir in dieDirection)
            {
                GameObject obj = Instantiate(dieEffectPrefab);

                obj.transform.position = transform.position;

                Vector3 dirVec = dir.position - transform.position;

                obj.transform.position += dirVec.normalized * lastCount;

                SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[6]);
                SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.boss_Audioclip[7]);

                StartCoroutine(DieEffectOffCoroutine(obj));
            }
            yield return new WaitForSeconds(0.1f);
            if (++lastCount > 6)
                break;
        }
    }
    IEnumerator DieEffectOffCoroutine(GameObject obj)
    {
        yield return new WaitForSeconds(1f);

        obj.SetActive(false);
    }
    public IEnumerator DieHeadMoveCoroutine()
    {
        dieHead.SetActive(true);
        dieMouth.SetActive(true);
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);

        yield return new WaitForSeconds(0.8f);

        Animation dieHeadAnimation = dieHead.GetComponent<Animation>();

        dieHeadAnimation.Play();

        yield return new WaitForSeconds(dieHeadAnimation.clip.length);

        dieHead.SetActive(false);
        dieMouth.SetActive(false);

        BossManager.Instance.TriggerOnBossDeath();

        GameManager.Instance.SetPlayerInputActive(true);

        gameObject.SetActive(false);
    }

    // 데미지 관련
    public override void OnDamage(float damage)
    {
        float adjustedDamage = damage - bossData.defense;
        base.OnDamage(adjustedDamage);
        ShowDamageText((int)adjustedDamage);
    }
    protected void ShowDamageText(int damage)
    {
        GameObject hudTextObject = ObjectPool.Instance.GetDamageTextObject();
        hudTextObject.transform.position = hudPos.transform.position;
        DamageText damageText = hudTextObject.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.damage = damage;
            hudTextObject.SetActive(true);
        }
    }
}
