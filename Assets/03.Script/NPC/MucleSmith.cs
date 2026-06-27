using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MucleSmith : NPCS
{
    public GameObject additionalUIElement;
    public GameObject talkUIChild1; // talkUI의 1번째 자식 오브젝트
    public GameObject ability; // UI어빌리티 오브젝트
    public Transform abilityTransform; // UI어빌리티위치 오브젝트
    public Transform additionalUIElementTransform; // 추가할 UI 요소의 Transform을 담을 변수

    protected override void Awake()
    {
        // 기존 부모 클래스의 Awake 메서드 호출
        base.Awake();

        ability = uiManager.transform.GetChild(6).gameObject;
        // UI 매니저의 0번째 자식의 1번째 자식과 2번째 자식의 Transform을 가져와 변수에 할당
        if (uiManager != null && uiManager.transform.childCount > 0)
        {
            abilityTransform = uiManager.transform.GetChild(6);
            ability = abilityTransform.gameObject;

            Transform child0 = uiManager.transform.GetChild(0);
            if (child0.childCount > 1)
            {
                talkUIChild1 = child0.GetChild(1).gameObject;
            }
            if (child0.childCount > 2)
            {
                additionalUIElementTransform = child0.GetChild(2);
                additionalUIElement = additionalUIElementTransform.gameObject;

                // 추가 UI 요소 초기 상태 설정 (SetActive(false) 상태에서 시작할 경우)
                additionalUIElement.SetActive(false);
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 부모 클래스의 OnTriggerEnter2D 호출
        base.OnTriggerEnter2D(other);

        // talkUI의 1번째 자식 활성화
        if (talkUIChild1 != null)
        {
            talkUIChild1.SetActive(true);
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        // 부모 클래스의 OnTriggerExit2D 호출
        base.OnTriggerExit2D(other);

        // talkUI의 1번째 자식 비활성화
        if (talkUIChild1 != null)
        {
            talkUIChild1.SetActive(false);
        }
        ability.SetActive(false);
    }

    public override void Talk()
    {
        // 부모 Talk 메서드 호출
        base.Talk();

        // 추가 UI 요소 활성화
        if (additionalUIElementTransform != null)
        {
            additionalUIElement.SetActive(true);
        }
    }
}
