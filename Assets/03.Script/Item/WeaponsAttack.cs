using UnityEngine;

public class WeaponsAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Weapons weapon = GetComponentInParent<Weapons>();
                if (weapon != null && weapon.weaponData != null)
                {
                    float damageAmount = Random.Range(weapon.weaponData.minDamage, weapon.weaponData.maxDamage);
                    damageable.OnDamage(damageAmount);
                }
            }
        }
    }
}
