using System.Collections.Generic;
using UnityEngine;

public class SpawnerZone : MonoBehaviour
{
    public LayerMask playerLayer;
    public List<Transform> spawnerPoint;
    public ObjectPool objPool;
    public bool isClear = false;

    public List<Door> doors; // 문 리스트 추가
    private List<GameObject> spawnedMonsters = new List<GameObject>(); // 스폰된 몬스터 리스트
    public List<GameObject> sealStones = new List<GameObject>();
    private void OnEnable()
    {
        if (!isClear)
        {
            SpawnMonster();
            isClear = true;
        }
    }

    private void Update()
    {
        CheckMonsters(); // 몬스터 체크 로직 추가
    }

    // 몬스터 스폰
    private void SpawnMonster()
    {
        for (int i = 0; i < spawnerPoint.Count; i++)
        {
            int randomIndex = Random.Range(0, ObjectPool.Instance.monsterPrefabs.Length);
            GameObject monster = ObjectPool.Instance.GetMonsterObject(randomIndex);
            monster.transform.position = spawnerPoint[i].position;
            monster.SetActive(true);
            spawnedMonsters.Add(monster);
        }
    }

    private void CheckMonsters()
    {
        if (isClear)
        {
            bool allMonstersCleared = true;

            // 모든 몬스터가 비활성화되었는지 확인
            foreach (GameObject monster in spawnedMonsters)
            {
                if (monster.activeSelf)
                {
                    allMonstersCleared = false;
                    break;
                }
            }

            // 모든 몬스터가 제거되었으면 문을 연다
            if (allMonstersCleared)
            {
                foreach (Door door in doors)
                {
                    door.Open(); // 문을 여는 메서드 호출
                }
            }
        }
    }
}
