using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public List<Sprite> particles = new List<Sprite>(); // 파티클로 사용될 스프라이트 리스트
    public float particleLifetime = 0.5f; // 파티클이 살아있는 시간
    public float particleForce = 1f; // 파티클에 가해지는 힘

    private bool isDestroyed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDestroyed && collision.GetComponent<WeaponsAttack>() != null)
        {
            SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.HitAudio);
            ShowParticles();
            gameObject.SetActive(false);
            Destroy(gameObject, particleLifetime);
            isDestroyed = true;
        }
    }

    // 파티클 효과
    private void ShowParticles()
    {
        foreach (var sprite in particles)
        {
            GameObject particleObj = new GameObject("Particle");
            particleObj.transform.position = transform.position;
            SpriteRenderer renderer = particleObj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;

            renderer.sortingLayerName = "Front";

            Rigidbody2D rb = particleObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1; // 필요에 따라 중력 조정
            Vector2 randomForce = Random.insideUnitCircle * particleForce;
            rb.AddForce(randomForce, ForceMode2D.Impulse);

            Destroy(particleObj, particleLifetime);
        }
    }
}
