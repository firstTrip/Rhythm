
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PoolItem
{
    public string name;           // 풀 이름 (식별자)
    public GameObject prefab;     // 생성할 프리팹
    public int preloadCount = 10; // 게임 시작 시 미리 만들어 놓을 개수
}

[System.Serializable]
public class MonsterPoolItem
{
    public string monsterName;      // 몬스터 식별자
    public GameObject monsterPrefab;
    public int preloadCount = 20;

    [Header("Dependent Effects")]
    public List<PoolItem> relatedEffects; // 이 몬스터가 사용하는 이펙트들
}

[CreateAssetMenu(fileName = "PoolingSetting", menuName = "ScriptableObjects/PoolingSetting")]
public class PoolingSetting : ScriptableObject
{
    [Header("Monster Groups")]
    public List<MonsterPoolItem> monsterGroups;

    [Header("Common Items (UI, Gems, etc)")]
    public List<PoolItem> commonPools;
}
