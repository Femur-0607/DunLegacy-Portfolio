using System.Collections;
using UnityEngine;

public class FlyingEnemy : Monster
{
    private Collider2D myCollider;

    protected override void Start()
    {
        base.Start();
        myCollider = GetComponent<Collider2D>();
    }
    protected override void TrackTarget()
    {
        if (playerTrackingCollider != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 velocity = new Vector3(directionToTarget.x * monsterData.speed, directionToTarget.y * monsterData.speed, 0f);
            monsterRigidbody.velocity = velocity;

            if (scopeOfAttackCollider != null)
            {
                monsterState = MonsterState.Attacking;
            }
        }
        else
        {
            monsterRigidbody.velocity = Vector2.zero;
            monsterState = MonsterState.Idle;
        }
    }

    protected override void ExecuteAttackPattern()
    {
        monsterRigidbody.velocity = Vector2.zero;
        Vector2 targetPosition = target.position;

        GameObject bulletObject = ObjectPool.Instance.GetBulletObject();
        if (bulletObject != null)
        {
            SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.monsterAudio[1]);
            bulletObject.transform.position = transform.position;
            bulletObject.SetActive(true);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Fire(targetPosition, monsterData.damage, monsterData.attackSpeed);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Wall" 태그를 가진 오브젝트와 충돌 시 처리
        if (other.CompareTag("wall"))
        {
            // 벽과의 충돌 처리 로직

            // isTrigger를 false로 변경
            myCollider.isTrigger = false;

            // 몇 초 후에 isTrigger를 true로 변경 (예시: 1초 후)
            StartCoroutine(ResetTriggerAfterDelay(1f));
        }
    }

    private IEnumerator ResetTriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // isTrigger를 true로 변경
        myCollider.isTrigger = true;
    }
}
