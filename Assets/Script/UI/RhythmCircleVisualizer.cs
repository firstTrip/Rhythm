using System.Collections.Generic;
using UnityEngine;

public class RhythmCircleVisualizer : MonoSingleton<RhythmCircleVisualizer>
{
    [SerializeField] private RectTransform container;
    [SerializeField] private float startRadius = 400f;

    [SerializeField] private float spawnAheadTime = 1.0f; 

    private Dictionary<BarData, List<UINoteCircle>> _activeBars = new Dictionary<BarData, List<UINoteCircle>>();
    private List<BarData> _keysToRemove = new List<BarData>();
    void Update()
    {
        if (RhythmEngine.Instance._flattenedBars == null) return;

        float now = RhythmClock.Instance.ElapsedTime;

        CheckAndSpawnBars(now);

        UpdateActiveNotes(now);
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

            note.Setup(sb, bar, bar.barColor, startRadius);

            notesInBar.Add(note);
        }

        _activeBars.Add(bar, notesInBar);
    }

    private void UpdateActiveNotes(float now)
    {
        _keysToRemove.Clear();

        foreach (var kvp in _activeBars)
        {
            BarData bar = kvp.Key;
            List<UINoteCircle> notes = kvp.Value;

            if (bar.startTime <= now + spawnAheadTime)
            {
                for (int i = notes.Count - 1; i >= 0; i--)
                {
                    if (!notes[i].UpdatePosition(now, spawnAheadTime))
                    {
                        notes.RemoveAt(i);
                    }
                }
            }

            if (notes.Count == 0)
            {
                _keysToRemove.Add(bar);
            }
        }

        foreach (var key in _keysToRemove)
        {
            _activeBars.Remove(key);
        }
    }

    public void RemoveNote(SubBeatData sb, BarData parentBar)
    {
        if (parentBar != null && _activeBars.TryGetValue(parentBar, out var notes))
        {
            UINoteCircle target = notes.Find(n => n.Data == sb);

            if (target != null)
            {
                notes.Remove(target);
                PoolingManager.Instance.Release(target.gameObject);
            }

            if (notes.Count == 0)
            {
                _activeBars.Remove(parentBar);
            }
        }
    }

    public void SetBarColorGray(BarData bar)
    {
        if (_activeBars.TryGetValue(bar, out var notes))
        {
            foreach (var note in notes)
            {
                if (note != null)
                {
                    note.SetColor(new Color(0.5f, 0.5f, 0.5f));
                }
            }
        }
    }
}
