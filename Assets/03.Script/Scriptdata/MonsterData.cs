using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/MonsterData", fileName = "Monster Data")]
public class MonsterData : ScriptableObject
{
    public float health;
    public float damage;
    public float speed;
    public float defense;
    public float attackDelay;
    public float attackSpeed;
    public Vector2 trackView;
    public Vector2 scopeOfAttackView;
}
