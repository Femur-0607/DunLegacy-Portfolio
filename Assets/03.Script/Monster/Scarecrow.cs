using System.Collections;
using UnityEngine;

public class Scarecrow : Monster
{
    protected override void Start()
    {
        base.Start();
        monsterRigidbody.velocity = Vector2.zero;
    }
    private void OnDisable()
    {
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    protected override void UpdateState()
    {
        monsterState = MonsterState.Idle;
    }

    // 데미지를 받을 때만 동작
    public override void OnDamage(float damage)
    {
        float adjustedDamage = damage - monsterData.defense;
        base.OnDamage(adjustedDamage);
    }
   
    // 필요없는 기능 비활성화
    protected override void TrackTarget()
    {

    }
    protected override void ExecuteAttackPattern()
    {

    }
    protected override void UpdateAnimator()
    {

    }
    protected override void FlipSprite()
    {

    }
}
