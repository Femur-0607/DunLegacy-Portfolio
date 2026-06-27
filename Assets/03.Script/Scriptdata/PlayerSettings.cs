using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/PlayerSettings", fileName = "PlayerSettings")]
public class PlayerSettings: ScriptableObject
{
    // 플레이어 스탯 변수
    public float moveSpeed = 7f;
    public float jumpForce = 10f;
    public float jumpCount = 1f;
    public float gravityForce = 10f;
    public float raycastBeam = 0.5f;
}