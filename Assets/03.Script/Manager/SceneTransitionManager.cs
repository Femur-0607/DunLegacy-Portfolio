using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    // 씬 전환 시 플레이어의 위치를 저장할 키값
    private const string SCENE_NAME_KEY = "LastSceneName";
    private const string PLAYER_POSITION_KEY = "PlayerPosition";

    // 씬이 로드될 때 호출되는 콜백 메서드
    private void OnEnable()
    {
        // 이전 씬이 빌리지씬이나 던전씬이었을 때만 플레이어 위치를 설정
        string lastSceneName = PlayerPrefs.GetString(SCENE_NAME_KEY);
        if (lastSceneName == "VillageScene" || lastSceneName == "DungeonScene")
        {
            // 저장된 플레이어 위치를 가져옴
            Vector3 savedPosition = GetSavedPlayerPosition();

            // 플레이어를 저장된 위치로 이동
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = savedPosition;
            }
        }
    }

    // 다음 씬으로 이동하는 메서드
    public void LoadNextScene(string sceneName)
    {
        // 현재 씬의 이름과 목표 씬의 이름을 저장
        PlayerPrefs.SetString(SCENE_NAME_KEY, SceneManager.GetActiveScene().name);

        // 다음 씬으로 이동
        SceneManager.LoadScene(sceneName);
    }

    // 플레이어의 위치를 저장하는 메서드
    private void SavePlayerPosition(Vector3 position)
    {
        PlayerPrefs.SetFloat(PLAYER_POSITION_KEY + "_X", position.x);
        PlayerPrefs.SetFloat(PLAYER_POSITION_KEY + "_Y", position.y);
        PlayerPrefs.SetFloat(PLAYER_POSITION_KEY + "_Z", position.z);
    }

    // 저장된 플레이어의 위치를 가져오는 메서드
    private Vector3 GetSavedPlayerPosition()
    {
        float x = PlayerPrefs.GetFloat(PLAYER_POSITION_KEY + "_X");
        float y = PlayerPrefs.GetFloat(PLAYER_POSITION_KEY + "_Y");
        float z = PlayerPrefs.GetFloat(PLAYER_POSITION_KEY + "_Z");

        return new Vector3(x, y, z);
    }
}
