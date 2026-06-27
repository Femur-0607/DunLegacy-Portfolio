using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : HealthManager, IDamageable
{
    public MonsterData monsterData;
    protected Animator monsterAnimator;
    protected Rigidbody2D monsterRigidbody;
    protected Collider2D monsterCollider;
    protected Collider2D playerCollider;
    protected SpriteRenderer spriteRenderer;
    protected Canvas canvas;
    protected Transform hudPos;
    protected Transform target;

    protected Color originalColor;
    protected LayerMask targetLayer;

    //공격 상태 변수
    public bool isAttacking = false;

    // 추적범위 & 공격범위
    protected Collider2D playerTrackingCollider;
    protected Collider2D scopeOfAttackCollider;

    // 몬스터의 상태 변수
    protected enum MonsterState
    {
        Idle,
        Tracking,
        Attacking,
        Die
    }

    [SerializeField]
    protected MonsterState monsterState = MonsterState.Idle;

    protected override void Awake()
    {
        maxHealth = (int)monsterData.health;
        base.Awake();
    }

    protected virtual void Start()
    {
        monsterAnimator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody2D>();
        monsterCollider = GetComponent<Collider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        canvas = GetComponentInChildren<Canvas>();
        originalColor = spriteRenderer.color;
        targetLayer = 1 << LayerMask.NameToLayer("Player");
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        target = playerObj.transform;
        hudPos = transform.Find("Hud");
        Physics2D.IgnoreCollision(monsterCollider, playerCollider, true);
    }
    protected void FixedUpdate()
    {
        FindAndSetTarget();
        UpdateState();
        FlipSprite();
        UpdateAnimator();
    }

    private void OnDisable()
    {
        currentHealth = maxHealth;
        ObjectPool.Instance.ReturnObject(gameObject);
        spriteRenderer.color = originalColor;
        spriteRenderer.enabled = true;
        canvas.enabled = true;  
        monsterState = MonsterState.Idle;
        isAttacking = false;
        UpdateHealthUI();
    }

    // 플레이어 추적 설정
    protected void FindAndSetTarget()
    {
        if (target != null)
        {
            playerTrackingCollider = Physics2D.OverlapBox(transform.position, monsterData.trackView, 0, targetLayer);
            scopeOfAttackCollider = Physics2D.OverlapBox(transform.position, monsterData.scopeOfAttackView, 0, targetLayer);
        }
    }

    // 몬스터의 상태 열거형
    protected virtual void UpdateState()
    {
        if (monsterState == MonsterState.Die)
        {
            HandleDieState();
            return;
        }

        switch (monsterState)
        {
            case MonsterState.Idle:
                IdleStand();
                break;
            case MonsterState.Tracking:
                TrackTarget();
                break;
            case MonsterState.Attacking:
                if (!isAttacking)
                {
                    StartCoroutine(AttackAfterDelay(monsterData.attackDelay));
                }
                break;
            default:
                break;
        }
    }
    protected void IdleStand()
    {
        if (playerTrackingCollider != null)
        {
            monsterState = MonsterState.Tracking;
        }
    }

    // 플레이어 추적
    protected virtual void TrackTarget()
    {
        if (playerTrackingCollider != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 velocity = new Vector3(directionToTarget.x * monsterData.speed, monsterRigidbody.velocity.y, 0f);
            monsterRigidbody.velocity = velocity;

            if (scopeOfAttackCollider != null)
            {
                monsterState = MonsterState.Attacking;
            }
        }
        else
        {
            Vector3 stopVelocity = new Vector3(0f, monsterRigidbody.velocity.y, 0f);
            monsterRigidbody.velocity = stopVelocity;
            monsterState = MonsterState.Idle;
        }
    }

    // 공격 상태 전환
    IEnumerator AttackAfterDelay(float delay)
    {
        isAttacking = true;
        if (isAttacking)
        {
            if (scopeOfAttackCollider != null)
            {
                monsterRigidbody.velocity = new Vector2(0, monsterRigidbody.velocity.y);

                yield return new WaitForSeconds(delay);

                ExecuteAttackPattern();
                yield return new WaitForSeconds(delay);
                isAttacking = false;
            }
            else
            {
                isAttacking = false;
            }
        }
        monsterState = MonsterState.Idle;
    }

    // 상속받은 몬스터들의 공격 패턴
    protected virtual void ExecuteAttackPattern()
    {

    }

    // 사망상태시 스프라이트 및 캔버스 끔
    private void HandleDieState()
    {
        monsterRigidbody.velocity = Vector2.zero;
        isAttacking = false;
        spriteRenderer.enabled = false;
        canvas.enabled = false;
    }

    // 스프라이트 관련
    protected virtual void FlipSprite()
    {
        if (target != null)
        {
            spriteRenderer.flipX = target.position.x < transform.position.x;
        }
    }

    // 데미지 UI 관련
    public override void OnDamage(float damage)
    {
        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.HitAudio);
        float adjustedDamage = damage - monsterData.defense;
        spriteRenderer.color = Color.red;
        base.OnDamage(adjustedDamage);
        spriteRenderer.color = originalColor;
        ShowDamageText((int)adjustedDamage);
    }
    protected void ShowDamageText(int damage)
    {
        GameObject hudTextObject = ObjectPool.Instance.GetDamageTextObject();
        hudTextObject.transform.position = hudPos.transform.position;

        DamageText damageText = hudTextObject.GetComponent<DamageText>();
        if (damageText == null)
        {
            damageText = hudTextObject.AddComponent<DamageText>(); // DamageText가 없으면 추가
        }

        damageText.damage = damage;
        damageText.Initialize(); // 초기화 메서드 호출

        hudTextObject.SetActive(true);
    }

    // 애니메이션 관련
    protected virtual void UpdateAnimator()
    {
        if (monsterAnimator.parameters != null && monsterAnimator.parameters.Any(param => param.name == "Run"))
        {
            bool isRunning = monsterRigidbody.velocity.magnitude > 0;
            monsterAnimator.SetBool("Run", isRunning);
        }
    }

    protected override void Die()
    {
        monsterState = MonsterState.Die;
        StartCoroutine(HandleDieEffect());
    }

    private IEnumerator HandleDieEffect()
    {
        GameObject dieEffect = ObjectPool.Instance.GetDieEffectObject();
        dieEffect.transform.position = transform.position;
        dieEffect.SetActive(true);

        yield return new WaitForSeconds(dieEffect.GetComponent<DieEffect>().effectDuration);

        ObjectPool.Instance.ReturnObject(dieEffect);

        base.Die();
    }
}
