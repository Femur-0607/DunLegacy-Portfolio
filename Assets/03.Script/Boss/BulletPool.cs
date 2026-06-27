using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int poolSize = 20; // 풀 사이즈
    private List<GameObject> bulletPool;

    void Start()
    {
        InitializePool();
    }

    void InitializePool()
    {
        bulletPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false); // 초기에는 비활성화 상태로 생성
            bulletPool.Add(bullet);
        }
    }

    public GameObject GetBullet()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }

        // 만약 사용 가능한 총알이 없다면 새로 생성하지 않고 null 반환
        return null;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false); // 다시 비활성화
        // 필요한 경우 추가적인 초기화 작업이 필요하면 여기서 수행
    }
}
