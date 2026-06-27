using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public bool hasLongsword;
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    private string savePath;

    public bool hasLongsword = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "playerData.json");
            LoadData();
            hasLongsword = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveData()
    {
        PlayerData data = new PlayerData();
        data.hasLongsword = hasLongsword;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            hasLongsword = data.hasLongsword;
        }
        else
        {
            hasLongsword = false;
        }
    }
}
