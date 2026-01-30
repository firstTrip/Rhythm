using System.Collections.Generic;
using UnityEngine;

public class RhythmEngine : MonoSingleton<RhythmEngine>
{
    [Header("Judgement Range")]
    public float perfectRange = 0.08f;
    public float normalRange = 0.18f;

    private List<RhythmCircle> _timeline;
    private BarData _lastCheckedBar = null;
    private int _currentBarIndex = 0;
    public List<BarData> _flattenedBars = new List<BarData>();

    public void InitEngine(List<RhythmCircle> timeline)
    {
        _timeline = timeline;

        _flattenedBars.Clear();
        foreach (var circle in _timeline)
        {
            _flattenedBars.AddRange(circle.bars);
        }

        RhythmClock.Instance.OnBeat += (beatIndex) => {
            if (beatIndex == 0) _currentBarIndex++; 
        };
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ProcessPlayerInput();
    }

    public void ProcessPlayerInput()
    {
        if (_flattenedBars == null || _flattenedBars.Count == 0) return;

        float now = RhythmClock.Instance.ElapsedTime;
        SubBeatData closestNote = null;
        BarData targetBar = null;
        float minDiff = float.MaxValue;

        // 1. 현재 시간 기준으로 앞뒤 1마디씩만 검사 (효율적)
        int start = Mathf.Max(0, _currentBarIndex - 1);
        int end = Mathf.Min(_flattenedBars.Count, _currentBarIndex + 2);

        for (int i = start; i < end; i++)
        {
            var bar = _flattenedBars[i];
            foreach (var sb in bar.subBeats)
            {
                if (sb.isHit) continue; // 이미 맞춘 노트는 건너뜀

                float diff = Mathf.Abs(sb.time - now);
                if (diff < minDiff && diff <= normalRange) // 판정 범위 내 가장 가까운 것
                {
                    minDiff = diff;
                    closestNote = sb;
                    targetBar = bar; // 이 노트가 속한 마디 저장
                }
            }
        }

        // 2. 판정 성공 시
        if (closestNote != null)
        {
            closestNote.isHit = true;
            PlayDivisionSound(closestNote);

            if (IsBarFullClear(targetBar))
            {
                ExecuteBarAttack();
            }
        }
    }
    private void PlayDivisionSound(SubBeatData note)
    {
        SoundType sfxName = note.division switch
        {
            NoteDivision.Half => SoundType.Crash,
            NoteDivision.Quarter => SoundType.Kick, 
            NoteDivision.Eighth => SoundType.Snare,       
            NoteDivision.Sixteenth => SoundType.HiHat,      
            _ => SoundType.Kick
        };

        SoundManager.Instance.PlaySFX(sfxName, 1f);
    }

    private bool IsBarFullClear(BarData bar)
    {
        if (bar == null) return false;

        foreach (var sb in bar.subBeats)
        {
            if (!sb.isHit) return false; // 하나라도 안 맞았으면 실패
        }
        return true;
    }

    private void ExecuteBarAttack()
    {
        Debug.Log("마디 완주! 공격 발동!");
    }
}
