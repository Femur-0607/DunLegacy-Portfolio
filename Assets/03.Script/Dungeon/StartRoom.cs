using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartRoom : MonoBehaviour
{
    public GameObject sealStoneObject;
    public GameObject startObject;

    private void Start()
    {
        StartCoroutine(DelayedSetup());
    }

    private IEnumerator DelayedSetup()
    {
        yield return new WaitForSeconds(1f);

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "DungeonScene" || currentSceneName == "BossScene")
        {
            SetupRoomObjects();
        }
    }

    public void SetupRoomObjects()
    {
        if (sealStoneObject != null)
        {
            Animator sealStoneAnimator = sealStoneObject.GetComponent<Animator>();
            if (sealStoneAnimator != null)
            {
                sealStoneAnimator.SetTrigger("SealOpen");
            }
        }

        if (startObject != null)
        {
            Animator startAnimator = startObject.GetComponent<Animator>();
            if (startAnimator != null)
            {
                startAnimator.SetTrigger("GateClose");
            }
        }
    }
}
