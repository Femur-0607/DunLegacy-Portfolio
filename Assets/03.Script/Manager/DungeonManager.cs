using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public List<Transform> spawnerPoint;

    public bool isClear = false;
    public bool openSealStonesCalled = false;

    [SerializeField]
    private List<GameObject> spawnedMonsters = new List<GameObject>(); // 스폰된 몬스터 리스트
    [SerializeField]
    private List<GameObject> sealStones = new List<GameObject>(); // 씰스톤 오브젝트 리스트


    private void OnEnable()
    {
        if (!isClear)
        {
            InitializeDoors();
            SpawnMonsters();
        }
    }

    private void Update()
    {
        CheckMonsters();
        if (isClear && !openSealStonesCalled)
        {
            OpenSealStones();
            openSealStonesCalled = true; // 한 번만 호출되도록 설정
        }
    }

    private void InitializeDoors()
    {
        Transform parentTransform = this.transform; // 부모 오브젝트의 Transform
                                                    // List<Transform> sealStones = new List<Transform>();

        FindChildrenByName(parentTransform, "SealStone", sealStones);
    }
    void FindChildrenByName(Transform parent, string name, List<GameObject> result)
    {
        foreach (Transform child in parent)
        {

            if (child.name == name && child.parent.gameObject.activeSelf && child != null)
            {
                result.Add(child.gameObject);
            }
            FindChildrenByName(child, name, result);
        }
    }

    private void SpawnMonsters()
    {
        // 스폰 포인트에서 몬스터 스폰
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
        // 모든 몬스터가 제거되었는지 확인
        bool allMonstersCleared = true;
        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster.activeSelf)
            {
                allMonstersCleared = false;
                break;
            }
        }

        // 모든 몬스터가 제거되었으면 문을 열기
        if (allMonstersCleared)
        {
            isClear = true;
        }
    }

    private void OpenSealStones()
    {
        foreach (GameObject sealStone in sealStones)
        {
            if (sealStone.activeSelf) // 활성화된 오브젝트에 대해서만 작업을 수행합니다.
            {
                Animator animator = sealStone.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("SealOpen");
                }

                Collider2D collider = sealStone.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.isTrigger = true;
                }
            }
        }
    }
}
