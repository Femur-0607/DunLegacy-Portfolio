using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public UpgradeManager upgradeManager;
    public Text remainingPointsText; // 남은 포인트를 표시할 UI Text
    public Text[] bigNumberTexts; // 큰 숫자를 표시할 UI Text 배열
    public Text[] smallNumberTexts; // 작은 숫자를 표시할 UI Text 배열

    private int remainingPoints = 10; // 초기 남은 포인트 값

    private int[] upgradeCounts = new int[4]; // 각 어빌리티 업그레이드 횟수 배열
    private float[] smallNumberValues = new float[4]; // 각 어빌리티 작은 숫자 값 배열

    private void OnEnable()
    {
        if (upgradeManager == null)
        {
            upgradeManager = FindObjectOfType<UpgradeManager>();
        }
        // 초기화
        remainingPoints = 10;
        UpdateRemainingPointsText();
        UpdateUI();
    }

    public void OnUpgradeAttackPower()
    {
        if (remainingPoints >= 1)
        {
            upgradeManager.UpgradeAttackPower(0.5f);
            remainingPoints -= 1;
            upgradeCounts[0] += 1;
            UpdateUI();
        }
    }

    public void OnUpgradeAttackSpeed()
    {
        if (remainingPoints >= 1)
        {
            upgradeManager.UpgradeAttackSpeed(-0.1f);
            remainingPoints -= 1;
            upgradeCounts[1] += 1;
            UpdateUI();
        }
    }

    public void OnUpgradeMoveSpeed()
    {
        if (remainingPoints >= 1)
        {
            upgradeManager.UpgradeMoveSpeed(0.1f);
            remainingPoints -= 1;
            upgradeCounts[2] += 1;
            UpdateUI();
        }
    }

    public void OnUpgradeMaxHealth()
    {
        if (remainingPoints >= 1)
        {
            upgradeManager.UpgradeMaxHealth(5f);
            remainingPoints -= 1;
            upgradeCounts[3] += 1;
            UpdateUI();
        }
    }

    private void UpdateRemainingPointsText()
    {
        remainingPointsText.text = "남은 포인트 : " + remainingPoints.ToString();
    }

    private void UpdateUI()
    {
        UpdateRemainingPointsText();

        // 각 버튼의 횟수와 사용된 포인트를 업데이트
        for (int i = 0; i < bigNumberTexts.Length; i++)
        {
            bigNumberTexts[i].text = upgradeCounts[i].ToString();
            smallNumberTexts[i].text = smallNumberValues[i].ToString("F1");
        }
    }
}
