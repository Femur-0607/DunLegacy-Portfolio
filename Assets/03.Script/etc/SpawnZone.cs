using System.Collections;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    public LayerMask playerLayer;
    public float detectionRadius = 3f;

    private bool playerDetected = false;

    private void Update()
    {
        DetectPlayer();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // 플레이어 감지
    private void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (player != null && !playerDetected)
        {
            playerDetected = true;
            SpawnMonster();
        }
    }

    // 몬스터 스폰
    private void SpawnMonster()
    {
        int randomIndex = Random.Range(0, ObjectPool.Instance.monsterPrefabs.Length);
        GameObject monster = ObjectPool.Instance.GetMonsterObject(randomIndex);
        monster.transform.position = transform.position;
        monster.SetActive(true);
    }
}
