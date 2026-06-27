using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningNPC : MonoBehaviour
{
    public string playerLayerName = "Player";
    public GameObject targetObject1;
    public GameObject targetObject2;
    public DialogSystem dialogSystem;
    public bool isTextDisplayed = false;
    Animator animator;

    private OpeningBackScrolling openingBackScrolling;
    private TilemapScrolling[] tilemapScrollings;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        openingBackScrolling = FindObjectOfType<OpeningBackScrolling>();
        tilemapScrollings = FindObjectsOfType<TilemapScrolling>();
        dialogSystem = FindObjectOfType<DialogSystem>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            targetObject1.SetActive(true);
            targetObject2.SetActive(true);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isTextDisplayed && collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName) && Input.GetKeyDown(KeyCode.F))
        {
            isTextDisplayed = true;
            StartCoroutine(ShowTextAndStartDialog());
        }
    }

    private IEnumerator ShowTextAndStartDialog()
    {
        HandleDialogStarted();
        openingBackScrolling.StartMovingBackground();
        foreach (var tilemapScrolling in tilemapScrollings)
        {
            tilemapScrolling.StartMoving();
        }
        yield return new WaitForSeconds(2f);
        dialogSystem.StartDialog();
    }

    private void HandleDialogStarted()
    {
        targetObject1.SetActive(false);
        targetObject2.SetActive(false);
        animator.SetTrigger("Move");
    }
}
