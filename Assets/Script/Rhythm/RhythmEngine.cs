using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class RhythmEngine : MonoSingleton<RhythmEngine>
{
    [Header("Judgement Range")]
    public float perfectRange = 0.08f;
    public float normalRange = 0.18f;

    private List<RhythmCircle> _timeline;
    private BarData _lastCheckedBar = null;

    public void InitEngine(List<RhythmCircle> generatedTimeline)
    {
        _timeline = generatedTimeline;

        RhythmClock.Instance.OnBeat += CheckBarOnBeat;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ProcessPlayerInput();
    }

    public void ProcessPlayerInput()
    {
        if (_timeline == null) return;

        float now = RhythmClock.Instance.ElapsedTime;
        SubBeatData closest = null;
        float minDiff = float.MaxValue;

        foreach (var circle in _timeline)
        {
            foreach (var bar in circle.bars)
            {
                if (Mathf.Abs(bar.startTime - now) > 1.0f) continue;

                foreach (var sb in bar.subBeats)
                {
                    float diff = Mathf.Abs(sb.time - now);
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                        closest = sb;
                    }
                }
            }
        }

        if (closest != null && minDiff <= normalRange && !closest.isHit)
        {
            closest.isHit = true;

            BeatRating rating = minDiff <= perfectRange ? BeatRating.Perfect : BeatRating.Normal;
        }
    }

    private void CheckBarOnBeat(int beatIndex)
    {
        if (beatIndex == 0)
        {
            float now = RhythmClock.Instance.ElapsedTime;
            BarData lastBar = FindLastCompletedBar(now);

            if (lastBar != null && lastBar != _lastCheckedBar)
            {
                _lastCheckedBar = lastBar;
                VerifyBarCompletion(lastBar);
            }
        }
    }

    private BarData FindLastCompletedBar(float now)
    {
        foreach (var circle in _timeline)
        {
            foreach (var bar in circle.bars)
            {
                float timeSinceBarStart = now - bar.startTime;
                if (timeSinceBarStart > 1.8f && timeSinceBarStart < 2.5f)
                {
                    return bar;
                }
            }
        }
        return null;
    }

    private void VerifyBarCompletion(BarData bar)
    {
        bool allHit = bar.subBeats.TrueForAll(sb => sb.isHit);

        if (allHit)
        {
            Debug.Log("<color=yellow>★ BAR CLEAR! 강력한 광역 스킬 발동! ★</color>");
            Player.Instance.ProcessAttack(BeatRating.Perfect);
        }
        else
        {
            Debug.Log("<color=gray>Bar Failed: 모든 노트를 맞추지 못했습니다.</color>");
        }
    }
}
