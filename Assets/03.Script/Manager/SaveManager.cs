using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerAttributes initialAttributes;

    private static SaveManager instance;
    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(SaveManager).Name);
                    instance = obj.AddComponent<SaveManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트는 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 씬에 존재하는 경우 중복 생성을 방지하기 위해 파괴
        }
    }

    public void LoadInitialAttributes(PlayerAttributes attributes)
    {
        attributes.minDamageAmount = initialAttributes.minDamageAmount;
        attributes.maxDamageAmount = initialAttributes.maxDamageAmount;
        attributes.attackSpeed = initialAttributes.attackSpeed;
        attributes.moveSpeed = initialAttributes.moveSpeed;
        attributes.maxHealth = initialAttributes.maxHealth;
    }
}
