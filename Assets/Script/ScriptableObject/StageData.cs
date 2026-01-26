using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    [System.Serializable]
    public class WaveInfo
    {
        public string waveName;
        [Tooltip("웨이브 시작 시간 (초)")] public float startTime;
        [Tooltip("웨이브 종료 시간 (초)")] public float endTime;
        [Tooltip("스폰 간격 (낮을수록 많이 스폰)")] public float spawnInterval;
        [Tooltip("이 시간대에 등장할 몬스터 키 리스트")] public List<MonsterData> monsterDataList;
    }

    [Header("Stage Basic Info")]
    public string stageName;
    public AudioClip stageBGM;

    [Header("Wave Timeline")]
    public List<WaveInfo> waves = new List<WaveInfo>();

    [Header("Final Boss")]
    public string finalBossKey;
    public float bossSpawnTime = 300f;

    [Header("Difficulty Scaling")]
    public float healthMultiplierPerMin = 1.1f;
    public float damageMultiplierPerMin = 1.05f;
}
