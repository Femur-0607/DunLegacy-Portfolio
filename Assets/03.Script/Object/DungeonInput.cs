using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonInput : MonoBehaviour
{
    public float scaleDownDuration = 1f;
    public Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f);
    public float rotationSpeed = 90;
    public float pullStrength = 10f;
    public float pullSpeed = 2.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(GameManager.Instance.HandlePlayerEntry(other.gameObject, scaleDownDuration, targetScale));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PullPlayerTowardsCenter(other.gameObject));
        }
    }
    // 플레이어 던전 입장 이벤트 메서드
    private IEnumerator PullPlayerTowardsCenter(GameObject player)
    {
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        playerRigidbody.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        Vector2 direction = (Vector2)transform.position - playerRigidbody.position;
        direction.Normalize();
        playerRigidbody.velocity = direction * pullSpeed;
        playerRigidbody.AddForce(direction * pullStrength);

        float elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            player.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
