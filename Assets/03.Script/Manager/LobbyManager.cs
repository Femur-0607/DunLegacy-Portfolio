using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public GameObject option_Panel;
    public GameObject bird;

    public bool stopSpawn = true;

    void Start()
    {
        SoundManager.Instance.PlayBGM(SoundManager.Instance.UI_Audio[4]);
        StartCoroutine(SpawnBird());
    }
    // 로비에서 새 날라오는 효과
    IEnumerator SpawnBird()
    {
        while (stopSpawn)
        {
            yield return new WaitForSecondsRealtime(5f);
            Instantiate(bird);
            float randY = Random.Range(-5f, 5f);
            bird.transform.position = new Vector3(-10f, randY);
        }
    }
    
    // 버튼 관련 항목
    public void optionPanelOn()
    {
        option_Panel.transform.gameObject.SetActive(true);
    }
    public void optionPanelOff()
    {
        option_Panel.transform.gameObject.SetActive(false);
    }
    public void SceneChange()
    {
        SoundManager.Instance.StopBGM();
        UIManager.Instance.StartTransition(() =>
        {
            SceneManager.LoadScene("OpeningScene");
            SoundManager.Instance.Volume_Save();
        });
    }
    public void stopSpawning()
    {
        stopSpawn = false;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
