using UnityEngine;
using UnityEngine.SceneManagement;

public class GateController : MonoBehaviour
{
    public Animator gateAnimator;
    public GameObject bossEntrance;
    private bool playerInRange = false;

    private void Start()
    {
        gateAnimator = GetComponentInChildren<Animator>();
        bossEntrance = transform.GetChild(1).gameObject;
        gateAnimator.SetTrigger("GateClose");
        bossEntrance.SetActive(false);
    }
    private void Update()
    {
        if (playerInRange && bossEntrance.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.RequestSceneLoad("BossScene");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            gateAnimator.SetTrigger("GateOpen");
            bossEntrance.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            gateAnimator.SetTrigger("GateClose");
            bossEntrance.SetActive(false);
        }
    }
}
