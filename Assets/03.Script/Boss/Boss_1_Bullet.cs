using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_1_Bullet : MonoBehaviour
{
    public float Damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnDamage(Damage);
            }
        }
    }
}
