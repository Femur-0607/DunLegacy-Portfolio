using Cinemachine;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : HealthManager, IDamageable
{
    private Renderer playerRenderer;

    [SerializeField]
    private bool isInvincible = false;
    [SerializeField]
    private float invincibilityDuration = 0.5f;
    [SerializeField]
    private bool isDead = false;

    protected override void Awake()
    {
        maxHealth = 100;
        base.Awake();
    }
    
    private void OnDisable()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Start()
    {
        playerRenderer = GetComponent<Renderer>();
    }

    public override void OnDamage(float damage)
    {
        if (!isInvincible && !isDead )
        {
            base.OnDamage(damage);
            StartCoroutine(FlashPlayer());
            StartCoroutine(InvincibilityTimer());
        }
    }

    private IEnumerator FlashPlayer()
    {
        playerRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        playerRenderer.material.color = Color.white;
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    protected override void Die()
    {
        isDead = true;
        isInvincible = false;
        GameManager.Instance.isGameOver = true;
    }
}
