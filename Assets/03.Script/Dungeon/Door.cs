using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<GameObject> sealStones = new List<GameObject>(); // SealStone 오브젝트 리스트

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        // 테스트용: Y 키를 눌러서 초기화 메서드 호출
        if (Input.GetKey(KeyCode.Y))
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        sealStones.Clear(); // 초기화 시 기존 리스트를 비워줍니다.

        // GetComponentsInChildren을 사용하여 비활성화된 오브젝트도 포함하여 가져옵니다.
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.CompareTag("SealStone"))
            {
                sealStones.Add(child.gameObject);
            }
        }
    }

    public void Open()
    {
        // 모든 SealStone 오브젝트를 비활성화
        foreach (GameObject sealStone in sealStones)
        {
            sealStone.SetActive(false);
        }
    }

    public void Close()
    {
        // 모든 SealStone 오브젝트를 활성화
        foreach (GameObject sealStone in sealStones)
        {
            sealStone.SetActive(true);
        }
    }
}
