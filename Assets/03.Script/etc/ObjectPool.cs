using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public GameObject damageTextPrefab;
    public GameObject bulletPrefab;
    public GameObject dieEffectPrefab;
    public GameObject[] monsterPrefabs;

    [SerializeField]
    private List<GameObject> damageTextPool = new List<GameObject>();
    [SerializeField]
    private List<GameObject> bulletPool = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dieEffectPool = new List<GameObject>();
    [SerializeField]
    private List<GameObject>[] monsterPools;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DeactivateAllObjects();
    }

    private void DeactivateAllObjects()
    {
        foreach (var obj in damageTextPool) { obj.SetActive(false); }
        foreach (var obj in bulletPool) { obj.SetActive(false); }
        foreach (var obj in dieEffectPool) { obj.SetActive(false); }
        if (monsterPools != null)
        {
            foreach (var pool in monsterPools)
            {
                if (pool != null)
                {
                    foreach (var obj in pool) { obj.SetActive(false); }
                }
            }
        }
    }

    // 텍스트 관련 오브젝트 풀링
    public GameObject GetDamageTextObject()
    {
        return GetObjectFromPool(damageTextPrefab, damageTextPool);
    }

    // 보통 총알 관련 오브젝트 풀링
    public GameObject GetBulletObject()
    {
        return GetObjectFromPool(bulletPrefab, bulletPool);
    }

    public GameObject GetDieEffectObject()
    {
        return GetObjectFromPool(dieEffectPrefab, dieEffectPool);
    }

    // 몬스터 관련 오브젝트 풀링
    public GameObject GetMonsterObject(int index)
    {
        if (monsterPools == null)
        {
            monsterPools = new List<GameObject>[monsterPrefabs.Length];
        }

        if (monsterPools[index] == null)
        {
            monsterPools[index] = new List<GameObject>();
        }

        return GetObjectFromPool(monsterPrefabs[index], monsterPools[index]);
    }

    // 오브젝트 풀로 반환
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    // 오브젝트 풀에서 비활성화된 오브젝트 가져오기
    private GameObject GetObjectFromPool(GameObject prefab, List<GameObject> pool)
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab);
        DontDestroyOnLoad(newObj);
        pool.Add(newObj);
        return newObj;
    }
}
