using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonAnimation : MonoBehaviour
{
    private GameObject animatorGameObject;
    private Animator dungeonAnimator;

    private void Start()
    {
        animatorGameObject = transform.GetChild(0).gameObject;
        dungeonAnimator = animatorGameObject.GetComponent<Animator>();
        if (animatorGameObject == null)
        {
            Debug.LogError("animatorGameObject 오브젝트를 찾을 수 없습니다.");
        }
        if (dungeonAnimator == null)
        {
            Debug.LogError("dungeonAnimator 오브젝트를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.UI_Audio[5]);
            animatorGameObject.SetActive(true);
            dungeonAnimator.SetBool("Idle", true);
            Invoke("ExitAnimation", 1f);
        }
    }

    private void ExitAnimation()
    {
        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.UI_Audio[6]);
        dungeonAnimator.SetBool("Idle", false);
        dungeonAnimator.SetTrigger("Exit");
        StartCoroutine(DeactivateGameObjectAfterDelay(2f));
    }

    private IEnumerator DeactivateGameObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animatorGameObject.SetActive(false);
    }
}