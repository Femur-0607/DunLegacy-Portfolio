using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseUpEnemy : Monster
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void ExecuteAttackPattern()
    {
        monsterAnimator.SetTrigger("Attack");
        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.monsterAudio[2]);
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(transform.position, monsterData.scopeOfAttackView, 0, targetLayer);
        foreach (var hitPlayer in hitPlayers)
        {
            IDamageable damageable = hitPlayer.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnDamage(monsterData.damage);
            }
        }
    }

    protected override void Die()
    {
        monsterState = MonsterState.Die;

        Vector3 dieEffectPosition = transform.position + new Vector3(0f, 1.3f, 0f);  // 예시로 몬스터 위에서 실행
        StartCoroutine(HandleDieEffect(dieEffectPosition));
    }

    private IEnumerator HandleDieEffect(Vector3 position)
    {
        GameObject dieEffect = ObjectPool.Instance.GetDieEffectObject();
        dieEffect.transform.position = position;  // 계산된 위치로 다이 이펙트 설정
        dieEffect.SetActive(true);

        yield return new WaitForSeconds(dieEffect.GetComponent<DieEffect>().effectDuration);

        ObjectPool.Instance.ReturnObject(dieEffect);

        gameObject.SetActive(false);
    }
}
