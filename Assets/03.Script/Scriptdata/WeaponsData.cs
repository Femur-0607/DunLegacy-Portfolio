using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/WeaponsData", fileName = "Weapons Data")]
public class WeaponsData : ScriptableObject
{
    public float minDamage;
    public float maxDamage;

    public float attackSpeed;
}
