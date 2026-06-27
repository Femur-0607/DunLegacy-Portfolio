using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public float speed = 2.0f; // 적의 이동 속도

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isChasing = false;
    private bool facingRight = true;

    public Canvas enemyCanvas;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (enemyCanvas != null)
        {
            enemyCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isChasing && playerTransform != null)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }
    }

    private void ChasePlayer()
    {
        float step = speed * Time.deltaTime;
        Vector2 targetPosition = new Vector2(playerTransform.position.x, transform.position.y);

        // 플레이어 위치로 이동
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
        animator.SetBool("Run", true);

        // 캔버스 활성화
        if (enemyCanvas != null)
        {
            enemyCanvas.gameObject.SetActive(true);
        }

        // 방향을 바꿀 때 플립을 설정
        if ((facingRight && playerTransform.position.x < transform.position.x) || (!facingRight && playerTransform.position.x > transform.position.x))
        {
            Flip();
        }
    }

    private void StopChasing()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("Run", false);

        if (enemyCanvas != null)
        {
            enemyCanvas.gameObject.SetActive(false);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            isChasing = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            playerTransform = null;
        }
    }
}
