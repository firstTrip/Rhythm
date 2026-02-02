using System.Collections.Generic;
using UnityEngine;

public class RhythmGenerator : MonoSingleton<RhythmGenerator>
{
    private NoteDivision[] _currentPattern;


    public List<BarData> Build(
        float songLength,
        float bpm,
        NoteDivision[] pattern,
        Color[] barColors,
        float startTime = 0f) 
    {
        List<BarData> flattenedBars = new List<BarData>();

        float secPerQuarter = 60f / bpm;
        float secPerBar = secPerQuarter * 4f; 

        int totalBars = Mathf.CeilToInt(songLength / secPerBar);

        float timeCursor = 0f;

        for (int b = 0; b < totalBars; b++)
        {
            BarData bar = new BarData();

            bar.barColor = barColors[b % barColors.Length];
            bar.startTime = timeCursor;

            float subCursor = timeCursor;
            for (int i = 0; i < pattern.Length; i++)
            {
                bar.subBeats.Add(new SubBeatData
                {
                    time = subCursor,
                    division = pattern[i]
                });
                subCursor += DivisionToSeconds(pattern[i], secPerQuarter);
            }

            flattenedBars.Add(bar);
            timeCursor += secPerBar;
        }
        return flattenedBars;
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
