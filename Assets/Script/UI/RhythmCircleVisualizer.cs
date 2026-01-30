using System.Collections.Generic;
using UnityEngine;

public class RhythmCircleVisualizer : MonoSingleton<RhythmCircleVisualizer>
{
    [SerializeField] private RectTransform container;
    [SerializeField] private float lookAheadTime = 1.5f;
    [SerializeField] private float startRadius = 400f;

    private List<UINoteCircle> _activeNotes = new List<UINoteCircle>();
    private List<SubBeatData> _flatNotes = new List<SubBeatData>();

    [SerializeField] private float spawnAheadTime = 1.0f; // 노트를 실제 시간보다 얼마나 미리 생성할지

    // 마디(BarData)를 키값으로 하여, 그 마디에 속한 UI 원(UINoteCircle)들을 리스트로 관리합니다.
    private Dictionary<BarData, List<UINoteCircle>> _activeBars = new Dictionary<BarData, List<UINoteCircle>>();

    private int _spawnIndex = 0;

    public void PrepareNotes(List<RhythmCircle> timeline)
    {
        _flatNotes.Clear();
        _spawnIndex = 0;

        foreach (var circle in timeline)
            foreach (var bar in circle.bars)
                _flatNotes.AddRange(bar.subBeats);
    }

    void Update()
    {
        if (RhythmEngine.Instance._flattenedBars == null) return;

        float now = RhythmClock.Instance.ElapsedTime;
        CheckAndSpawnBars(now);
    }
    private void CheckAndSpawnBars(float currentTime)
    {
        foreach (var bar in RhythmEngine.Instance._flattenedBars)
        {
            // 아직 생성되지 않았고, 생성 타이밍(마디 시작 시간 - 미리보기 시간)이 되었다면
            if (!_activeBars.ContainsKey(bar) && bar.startTime <= currentTime + spawnAheadTime)
            {
                // 이미 지난 마디가 아니라면 생성
                if (bar.startTime > currentTime - 0.5f)
                {
                    SpawnBar(bar);
                }
            }
        }
    }

    private void SpawnBar(BarData bar)
    {
        List<UINoteCircle> notesInBar = new List<UINoteCircle>();

        foreach (var sb in bar.subBeats)
        {
            GameObject go = PoolingManager.Instance.Get("Note");
            var note = go.GetComponent<UINoteCircle>();
            note.transform.SetParent(container, false);

            note.Setup(sb.time, bar.barColor, startRadius);
        }

        _activeBars.Add(bar, notesInBar);
    }
}
