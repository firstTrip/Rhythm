using System.Collections.Generic;
using UnityEngine;

public class RhythmGenerator : MonoSingleton<RhythmGenerator>
{
    private NoteDivision[] _currentPattern;
    private BeatTemplate[] _currentTemplates;

    public List<RhythmCircle> currentTimeline;


    public void RebuildTimeline(float currentTime, float songLength, float bpm)
    {
        currentTimeline = Build(songLength, bpm, _currentPattern, _currentTemplates, currentTime);
    }

    public List<RhythmCircle> Build(
        float songLength,
        float bpm,
        NoteDivision[] pattern,
        BeatTemplate[] templates,
        float startTime = 0f) 
    {
        float secPerQuarter = 60f / bpm;
        float barLength = secPerQuarter * 4;
        float circleLength = barLength * 4;

        int totalCircles = Mathf.FloorToInt((songLength - startTime) / circleLength);
        float timeCursor = startTime;

        var circles = new List<RhythmCircle>();

        for (int c = 0; c < totalCircles; c++)
        {
            var circle = new RhythmCircle();
            for (int b = 0; b < 4; b++)
            {
                var bar = new BarData();
                bar.startTime = timeCursor;

                float subCursor = timeCursor;
                int lane = 0;

                foreach (var div in pattern)
                {
                    float difficultyWeight = GetDifficultyWeight(div);

                    bar.subBeats.Add(new SubBeatData
                    {
                        time = subCursor,
                        division = div,
                        beatTemplate = templates[lane % 4],
                    });

                    subCursor += DivisionToSeconds(div, secPerQuarter);
                    lane++;
                }
                timeCursor += barLength;
                circle.bars.Add(bar);
            }
            circles.Add(circle);
        }
        return circles;
    }

    private float GetDifficultyWeight(NoteDivision div)
    {
        switch (div)
        {
            case NoteDivision.Quarter: return 1.0f;
            case NoteDivision.Eighth: return 1.5f;
            case NoteDivision.Sixteenth: return 2.5f;
            default: return 1.0f;
        }
    }

    float DivisionToSeconds(NoteDivision div, float q)
    {
        return div switch
        {
            NoteDivision.Quarter => q,
            NoteDivision.Eighth => q * 0.5f,
            NoteDivision.Half => q * 2f,
            _ => q
        };
    }
}
