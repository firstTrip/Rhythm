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

    [SerializeField] private float hitThreshold = 0.15f; 
    [SerializeField] private float missThreshold = 0.3f;
    public void InitEngine(List<BarData> timeline)
    {
        _flattenedBars.Clear();
        _flattenedBars = timeline;

        RhythmClock.Instance.OnBeat += (beatIndex) => {
            if (beatIndex == 0) _currentBarIndex++; 
        };
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var (closestNote, targetBar) = GetClosestNote();

            if (closestNote == null || targetBar == null) return;

            float now = RhythmClock.Instance.ElapsedTime;
            float diff = Mathf.Abs(closestNote.time - now);

            if (targetBar.isFailed) return;

            if (diff <= hitThreshold)
            {
                closestNote.isHit = true;
                ExecuteBasicAttack(targetBar.barColor);
                RhythmCircleVisualizer.Instance.RemoveNote(closestNote, targetBar);

                if (CheckBarComplete(targetBar))
                {
                    ExecuteBarCompleteAttack(targetBar.barColor);
                }
            }
            else
            {
                targetBar.isFailed = true;
                RhythmCircleVisualizer.Instance.SetBarColorGray(targetBar);
            }
        }
    }

    private (SubBeatData note, BarData bar) GetClosestNote()
    {
        float now = RhythmClock.Instance.ElapsedTime;
        SubBeatData bestNote = null;
        BarData bestBar = null;
        float minDiff = float.MaxValue;

        foreach (var bar in _flattenedBars)
        {
            if (bar.startTime > now + 1.0f) continue;
            if (bar.startTime < now - 1.0f) continue;

            foreach (var sb in bar.subBeats)
            {
                if (sb.isHit) continue; 

                float diff = Mathf.Abs(sb.time - now);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    bestNote = sb;
                    bestBar = bar;
                }
            }
        }

        return (bestNote, bestBar);
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

    private bool CheckBarComplete(BarData bar)
    {
        if (bar.isFailed) return false;

        foreach (var sb in bar.subBeats)
        {
            if (!sb.isHit) return false;
        }

        return true;
    }

    private void ExecuteBasicAttack(Color attackColor)
    {
        Debug.Log($"<color=white>단타 공격!</color> 색상: {attackColor}");

        // ProjectileManager.Instance.FireSmall(attackColor);
    }

    private void ExecuteBarCompleteAttack(Color attackColor)
    {
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(attackColor)}><b> 마디 완주! 추가 강력 공격 발동!! </b></color>");

        // Player.Instance.PlayHeavyAttackAnimation();
        // EffectManager.Instance.SpawnBigExplosion(attackColor);
    }
}
