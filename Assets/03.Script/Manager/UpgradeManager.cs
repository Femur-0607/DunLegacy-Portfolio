using System.Collections;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public PlayerAttributes playerAttributes;
    public Weapons longSword;
    public PlayerHealth playerHealth;
    public PlayerController playerController;

    private static UpgradeManager instance;
    public static UpgradeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UpgradeManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(UpgradeManager).Name);
                    instance = obj.AddComponent<UpgradeManager>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트는 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 씬에 존재하는 경우 중복 생성을 방지하기 위해 파괴
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            playerAttributes.minDamageAmount += 100;
            playerAttributes.maxDamageAmount += 100;
            longSword.weaponData.minDamage = playerAttributes.minDamageAmount;
            longSword.weaponData.maxDamage = playerAttributes.maxDamageAmount;
        }
    }
    public void UpgradeAttackPower(float amount)
    {
        playerAttributes.minDamageAmount += amount;
        playerAttributes.maxDamageAmount += amount;
        longSword.weaponData.minDamage = playerAttributes.minDamageAmount;
        longSword.weaponData.maxDamage = playerAttributes.maxDamageAmount;
    }

    public void UpgradeAttackSpeed(float amount)
    {
        playerAttributes.attackSpeed += amount;
        longSword.weaponData.attackSpeed = playerAttributes.attackSpeed;
    }

    public void UpgradeMoveSpeed(float amount)
    {
        playerAttributes.moveSpeed += amount;
        playerController.playerSettings.moveSpeed = playerAttributes.moveSpeed;
    }

    public void UpgradeMaxHealth(float amount)
    {
        playerAttributes.maxHealth += amount;
        playerHealth.maxHealth = (int)playerAttributes.maxHealth;
        playerHealth.currentHealth = playerHealth.maxHealth;
        playerHealth.UpdateHealthUI();
    }
}
