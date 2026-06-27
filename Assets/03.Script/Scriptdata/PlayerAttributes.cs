using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/PlayerAttributes", fileName = "PlayerSettings")]
[System.Serializable]
public class PlayerAttributes : ScriptableObject
{
    public float minDamageAmount = 7f;
    public float maxDamageAmount = 10f;
    public float attackSpeed = 1f;
    public float moveSpeed = 5f;
    public float maxHealth = 100f;

    // 초기화 메서드
    public void ResetToInitialValues()
    {
        minDamageAmount = 7f;
        maxDamageAmount = 10f;
        attackSpeed = 1f;
        moveSpeed = 5f;
        maxHealth = 100f;
    }
}
