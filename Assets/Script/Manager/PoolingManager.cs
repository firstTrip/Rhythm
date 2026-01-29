using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    Monster,
    Effect,
    Projectile,
    Item
}

public class PoolingManager : MonoSingleton<PoolingManager>
{
    [SerializeField] private PoolingSetting poolingSetting;

    private Dictionary<string, Queue<GameObject>> _pools = new();
    private Dictionary<string, GameObject> _prefabMap = new();

    protected override void Awake()
    {
        base.Awake();
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var group in poolingSetting.monsterGroups)
        {
            _prefabMap[group.monsterName] = group.monsterPrefab;
            CreatePool(group.monsterName, group.monsterPrefab, group.preloadCount);

            foreach (var effect in group.relatedEffects)
            {
                _prefabMap[effect.name] = effect.prefab;
                CreatePool(effect.name, effect.prefab, effect.preloadCount);
            }
        }

        foreach (var common in poolingSetting.commonPools)
        {
            _prefabMap[common.name] = common.prefab;
            CreatePool(common.name, common.prefab, common.preloadCount);
        }
    }

    private void CreatePool(string key, GameObject prefab, int count)
    {
        if (prefab == null || _pools.ContainsKey(key)) return;

        _pools[key] = new Queue<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GenerateNewObject(key, prefab);
        }
    }
    private GameObject GenerateNewObject(string key, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.name = key;
        obj.SetActive(false);
        _pools[key].Enqueue(obj);
        return obj;
    }

    private void PreWarm(List<PoolItem> items)
    {
        foreach (var item in items)
        {
            if (item.prefab == null) continue;

            string key = item.name;
            if (!_pools.ContainsKey(key)) _pools[key] = new Queue<GameObject>();

            for (int i = 0; i < item.preloadCount; i++)
            {
                GameObject obj = Instantiate(item.prefab, transform);
                obj.name = key;
                obj.SetActive(false);
                _pools[key].Enqueue(obj);
            }
        }
    }

    public GameObject Get(string key, Vector3 pos, Quaternion rot)
    {
        if (!_pools.ContainsKey(key))
        {
            Debug.LogError($"[PoolManager] {key}에 해당하는 풀이 존재하지 않습니다!");
            return null;
        }

        if (_pools[key].Count == 0)
        {
            if (_prefabMap.TryGetValue(key, out GameObject prefab))
            {
                Debug.Log($"[PoolManager] {key} 풀이 부족하여 새로 생성합니다.");
                GenerateNewObject(key, prefab);
            }
            else
            {
                Debug.LogError($"[PoolManager] {key} 프리팹 정보를 찾을 수 없어 확장할 수 없습니다.");
                return null;
            }
        }

        GameObject obj = _pools[key].Dequeue();
        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);
        return obj;
    }

    public void Release(GameObject obj)
    {
        string key = obj.name;
        obj.SetActive(false);
        if (_pools.ContainsKey(key)) _pools[key].Enqueue(obj);
    }
}
