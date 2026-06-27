using Cinemachine;
using System.Collections;
using UnityEngine;

public class BossRoomMove : MonoBehaviour
{
    public Transform bossRoomStart;
    public Collider2D bossCameraCollider;
    public CinemachineConfiner2D confiner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleTransition(other));
        }
    }

    private IEnumerator HandleTransition(Collider2D player)
    {
        UIManager.Instance.StartTransition(() =>
        {
            player.transform.position = bossRoomStart.position;
            confiner.m_BoundingShape2D = bossCameraCollider;
        });

        yield return new WaitForSeconds(1.5f);

        UIManager.Instance.EndTransition();
    }
}
