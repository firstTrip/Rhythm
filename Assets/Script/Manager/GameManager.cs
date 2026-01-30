using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Game Settings")]
    public float bpm = 120f;
    public float stageTime = 180f;
    [Header("Initial Pattern")]
    public NoteDivision[] startingPattern = { NoteDivision.Quarter, NoteDivision.Quarter, NoteDivision.Quarter, NoteDivision.Quarter };

    public bool IsGameOver { get; private set; }

    void Start()
    {
        NoteDivision[] startPattern = { NoteDivision.Whole, NoteDivision.Whole, NoteDivision.Whole, NoteDivision.Whole};

        BeatTemplate[] templates = new BeatTemplate[4];

        for (int i = 0; i < templates.Length; i++)
        {
            templates[i] = new BeatTemplate();
            templates[i].laneIndex = i;
        }

        var timeline = RhythmGenerator.Instance.Build(180f, 120, startPattern, templates);

        RhythmEngine.Instance.InitEngine(timeline);

        RhythmClock.Instance.StartClock();

        RhythmCircleVisualizer.Instance.PrepareNotes(timeline);
    }

    public void StartNewStage()
    {
        IsGameOver = false;
        Time.timeScale = 1f;

        var templates = new BeatTemplate[4];
        List<RhythmCircle> newTimeline = RhythmGenerator.Instance.Build(stageTime, bpm, startingPattern, templates);

        RhythmEngine.Instance.InitEngine(newTimeline);

        RhythmClock.Instance.bpm = bpm;
        RhythmClock.Instance.stageDuration = stageTime;
        RhythmClock.Instance.StartClock();

        RhythmClock.Instance.OnStageEnd += HandleStageEnd;
    }

    private void HandleStageEnd()
    {
        IsGameOver = true;
        Debug.Log("<color=orange>3분 종료! 스테이지 클리어!</color>");
    }

    public void UpdateStagePattern(NoteDivision[] newPattern)
    {
        startingPattern = newPattern;
        StartNewStage();
    }
}
