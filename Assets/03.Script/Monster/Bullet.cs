using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 targetPosition;
    private float bulletdamage;
    private float bulletspeed;
    private Animator bulletanimator;

    private void Start()
    {
        bulletanimator = GetComponent<Animator>();
    }

    public void Fire(Vector2 targetPosition, float damage, float speed)
    {
        this.targetPosition = targetPosition;
        this.bulletdamage = damage;
        this.bulletspeed = speed;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        GetComponent<Rigidbody2D> ().velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Player")
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnDamage(bulletdamage);
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                bulletanimator.SetTrigger("OnHit");
            }
        }
        else if (tag == "wall")
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            bulletanimator.SetTrigger("OnHit");
        }
        else
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision, true);
        }
    }

    public void OnHitAnimationEnd()
    {
        ObjectPool.Instance.ReturnObject(gameObject);
    }
}
