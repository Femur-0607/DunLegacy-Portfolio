using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarController : MonoBehaviour
{
    public GameObject gaugePrefab;  // 게이지 단위 프리팹
    public RectTransform loadingBar;  // 로딩바 배경
    public RectTransform leftBarrier;  // 왼쪽 게이지 막
    public RectTransform rightBarrier;  // 오른쪽 게이지 막
    public int totalSteps = 10;  // 총 게이지 단위 개수
    public float stepInterval = 0.5f;  // 게이지 단위가 추가되는 간격

    private float gaugeWidth;
    private int currentStep = 0;
    private float startPosX;
    private float endPosX;

    void Start()
    {
        gaugeWidth = gaugePrefab.GetComponent<RectTransform>().sizeDelta.x;
        startPosX = leftBarrier.anchoredPosition.x + gaugeWidth / 2;  // 왼쪽 막의 위치에서 시작
        endPosX = rightBarrier.anchoredPosition.x - gaugeWidth / 2;  // 오른쪽 막의 위치 전에 멈춤
        StartCoroutine(IncrementGauge());
    }

    private IEnumerator IncrementGauge()
    {
        while (currentStep < totalSteps && startPosX + currentStep * (gaugeWidth + 10) < endPosX)
        {
            AddGauge();
            currentStep++;
            yield return new WaitForSeconds(stepInterval);
        }
    }

    private void AddGauge()
    {
        GameObject newGauge = Instantiate(gaugePrefab, loadingBar);
        RectTransform rt = newGauge.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(startPosX + currentStep * (gaugeWidth + 10), 0);  // 10은 칸 사이의 간격
    }
}
