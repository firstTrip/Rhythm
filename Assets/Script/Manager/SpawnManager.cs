using UnityEditor.EditorTools;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    private StageData _stage;
    private Transform _player;

    private int _currentWaveIndex = -1;
    private float _elapsedTime;
    private float _spawnTimer;
    private bool _isBossSpawned;

    [Header("Spawn Distance")]
    public float minDistance = 15f;
    public float maxDistance = 20f;

    /// <summary>
    /// 게임 시작 시 StageManager로부터 호출됨
    /// </summary>
    public void Init(StageData stageData)
    {
        _stage = stageData;
        _player = PlayerManager.Instance.PlayerTransform;
        _elapsedTime = 0f;
        _spawnTimer = 0f;
        _currentWaveIndex = -1;
        _isBossSpawned = false;

        Debug.Log($"스포너 초기화 완료: {_stage.stageName}");
    }

    void Update()
    {
        if (_stage == null || _player == null || _isBossSpawned) return;

        _elapsedTime += Time.deltaTime;
        _spawnTimer += Time.deltaTime;

        // 1. 보스 스폰 타이밍 체크
        if (_elapsedTime >= _stage.bossSpawnTime)
        {
            SpawnBoss();
            return;
        }

        // 2. 현재 시간에 맞는 웨이브 정보 업데이트
        UpdateWaveIndex();

        // 3. 몬스터 스폰 루틴
        if (_currentWaveIndex != -1)
        {
            var wave = _stage.waves[_currentWaveIndex];
            if (_spawnTimer >= wave.spawnInterval)
            {
                SpawnMonster(wave);
                _spawnTimer = 0f;
            }
        }
    }

    private void UpdateWaveIndex()
    {
        // 최적화: 현재 웨이브 시간을 벗어났을 때만 새로 검색
        if (_currentWaveIndex != -1)
        {
            var current = _stage.waves[_currentWaveIndex];
            if (_elapsedTime >= current.startTime && _elapsedTime <= current.endTime)
                return;
        }

        // 전체 리스트에서 현재 시간에 맞는 웨이브 탐색
        for (int i = 0; i < _stage.waves.Count; i++)
        {
            if (_elapsedTime >= _stage.waves[i].startTime && _elapsedTime <= _stage.waves[i].endTime)
            {
                if (_currentWaveIndex != i)
                {
                    _currentWaveIndex = i;
                    Debug.Log($"Wave 변경: {_stage.waves[i].waveName}");
                }
                return;
            }
        }

        _currentWaveIndex = -1; // 맞는 웨이브가 없는 구간
    }

    private void SpawnMonster(StageData.WaveInfo wave)
    {
        if (wave.monsterDataList.Count == 0) return;

        // 1. 이번에 스폰할 MonsterData(SO)를 랜덤 선택
        MonsterData selectedData = wave.monsterDataList[Random.Range(0, wave.monsterDataList.Count)];

        Vector3 spawnPos = GetRandomSpawnPosition();

        // 2. SO에 적혀있는 poolKey를 이용해 풀에서 프리팹 추출
        GameObject go = PoolingManager.Instance.Get(selectedData.name);

        if (go != null)
        {
            CreatureBase monster = go.GetComponent<CreatureBase>();
            monster.transform.position = spawnPos;
            // 3. 몬스터에게 직접 SO를 주입 (Dependency Injection)
            // 몬스터는 이제 자신이 어떤 데이터를 써야 할지 스스로 알 필요가 없습니다.
            monster.Init(selectedData, _player, _elapsedTime);
        }
    }

    private void SpawnBoss()
    {
        _isBossSpawned = true;
        Vector3 spawnPos = GetRandomSpawnPosition();

        GameObject go = PoolingManager.Instance.Get(_stage.finalBossKey);
        if (go != null)
        {
            CreatureBase boss = go.GetComponent<CreatureBase>();
            go.transform.position = spawnPos;
            //boss.Init(_player);

            //// 보스도 스테이지 배율 적용
            //boss.health *= (1f + (_elapsedTime / 60f) * (_stage.healthMultiplierPerMin - 1f));

            Debug.Log("<color=red>최종 보스가 나타났습니다!</color>");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // 플레이어 기준 시야 밖 랜덤 원형 좌표
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDist = Random.Range(minDistance, maxDistance);
        return _player.position + new Vector3(randomDir.x, randomDir.y, 0) * randomDist;
    }
}
