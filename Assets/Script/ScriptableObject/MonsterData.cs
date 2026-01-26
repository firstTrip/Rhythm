using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("Basic Info")]
    public string monsterName;
    public MonsterType type; // Normal, Rare, Boss 등

    [Header("Stats")]
    public float maxHealth;
    public float moveSpeed;
    public float damage;

    [Header("Visuals")]
    public GameObject prefab; // 필요시 사용
}

public enum MonsterType { Normal, Rare, Boss }
