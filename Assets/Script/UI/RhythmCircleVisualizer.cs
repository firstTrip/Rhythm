using System.Collections.Generic;
using UnityEngine;

public class RhythmCircleVisualizer : MonoSingleton<RhythmCircleVisualizer>
{
    [SerializeField] private RectTransform container;
    [SerializeField] private float lookAheadTime = 1.5f;
    [SerializeField] private float startRadius = 400f;

    private List<UINoteCircle> _activeNotes = new List<UINoteCircle>();
    private List<SubBeatData> _flatNotes = new List<SubBeatData>();

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
        if (!RhythmClock.Instance.IsRunning) return;

        float currentTime = RhythmClock.Instance.ElapsedTime;

        // 1. 소환 로직
        while (_spawnIndex < _flatNotes.Count && _flatNotes[_spawnIndex].time < currentTime + lookAheadTime)
        {
            SpawnCircleNote(_flatNotes[_spawnIndex]);
            _spawnIndex++;
        }

        // 2. 업데이트 및 청소
        for (int i = _activeNotes.Count - 1; i >= 0; i--)
        {
            if (!_activeNotes[i].gameObject.activeSelf)
            {
                _activeNotes.RemoveAt(i);
                continue;
            }
            _activeNotes[i].UpdatePosition(currentTime, lookAheadTime);
        }
    }

    private void SpawnCircleNote(SubBeatData sb)
    {
        GameObject go = PoolingManager.Instance.Get("Note");
        var note = go.GetComponent<UINoteCircle>();
        note.transform.SetParent(container,false);

        note.Setup(sb.time, sb.beatTemplate.color, startRadius);
        _activeNotes.Add(note);
    }
}
