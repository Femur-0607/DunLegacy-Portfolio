using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour, IDamageable
{
    public static HealthManager Instance { get; private set; }

    public static event System.Action HealthManagerInitialized;

    [SerializeField]
    public int maxHealth;
    public int currentHealth;
    public Slider healthSlider;

    protected virtual void Awake()
    {
        Instance = this;
        currentHealth = maxHealth;
        healthSlider = GetComponentInChildren<Slider>();
        UpdateHealthUI();
        if (HealthManagerInitialized != null)
        {
            HealthManagerInitialized.Invoke();
        }
    }

    // 피격 시 호출되는 함수
    public virtual void OnDamage(float damage)
    {
        currentHealth -= (int)damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        healthSlider.value = currentHealth;
    }

    // 체력바 UI 업데이트 함수
    public virtual void UpdateHealthUI()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    // 사망 처리 함수
    protected virtual void Die()
    {
        gameObject.SetActive(false);
    }
}