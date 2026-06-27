using System.Collections;
using UnityEngine;

public class RushingEnemy : Monster
{
    private float rushDistance = 10f;
    private bool isRushing = false;
    private Vector2 startPosition;
    private float damageRange = 1f;
    private bool hasDamaged = false;
    private float lastDamageTime;

    protected override void ExecuteAttackPattern()
    {
        isRushing = true;
        hasDamaged = false;
        startPosition = transform.position;
        Vector2 rushForce = (spriteRenderer.flipX ? Vector2.left : Vector2.right) * monsterData.speed * 1.5f;
        monsterRigidbody.AddForce(rushForce, ForceMode2D.Impulse);
        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.monsterAudio[0]);
    }

    private void Update()
    {
        if (isRushing)
        {
            float distance = Vector2.Distance(startPosition, transform.position);
            if (distance >= rushDistance)
            {
                isRushing = false;
            }
        }

        float playerDistance = Vector2.Distance(transform.position, target.position);
        if (playerDistance <= damageRange && !hasDamaged && Time.time >= lastDamageTime + monsterData.attackDelay)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnDamage(monsterData.damage);
                hasDamaged = true;
                lastDamageTime = Time.time;
            }
            isRushing = false;
            monsterState = MonsterState.Idle;
        }
    }
}
