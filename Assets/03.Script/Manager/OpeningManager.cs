using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningManager : MonoBehaviour
{
    private DialogSystem dialogSystem;

    void Start()
    {
        dialogSystem = FindObjectOfType<DialogSystem>();
        SoundManager.Instance.PlayBGM(SoundManager.Instance.UI_Audio[0]);
        UIManager.Instance.EndTransition();
    }
    private void Update()
    {
        // 대화가 모두 표시되고, 사용자 입력 또는 조건에 따라 다음 씬으로 전환
        if (dialogSystem.IsDialogCompleted())
        {
            UIManager.Instance.StartTransition(() =>
            {
                SceneManager.LoadScene("VillageScene");
            });
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UIManager.Instance.StartTransition(() =>
            {
                SceneManager.LoadScene("VillageScene");
            });
        }
    }
}
