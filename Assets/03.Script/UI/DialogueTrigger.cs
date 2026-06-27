using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogSystem dialogSystem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            playerInput.enabled = false;
        }
    }
}
